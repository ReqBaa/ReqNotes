using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ReqNotes.ViewModels;

public class SettingsWindowViewModel : ReactiveValidationObject
{
    private readonly AppStateModel _appStateModel;

    public SettingsWindowViewModel()
    {
        _appStateModel = RxApp.SuspensionHost.GetAppState<AppStateModel>();

        this.ValidationRule(
            viewModel => viewModel.CurrentPassword,
            password => password?.Length >= 6,
            "Password must at least 6.");

        this.ValidationRule(
            viewModel => viewModel.NewPassword,
            password => password?.Length >= 6,
            "Password must at least 6.");

        this.ValidationRule(
            viewModel => viewModel.ReNewPassword,
            password => password?.Length >= 6,
            "Password must at least 6.");

        var passwordsObservable = this.WhenAnyValue(
        x => x.NewPassword,
        x => x.ReNewPassword,
        (password, repassword) =>
            new { Password = password, RePassword = repassword });
        this.ValidationRule(
            vm => vm.ReNewPassword,
            passwordsObservable,
            state => state.Password == state.RePassword,
            state => $"Passwords must match.");

        this.WhenAnyValue(x => x.HasErrors)
                .Subscribe(x => this.IsFormReady = !HasErrors);

        SaveSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            return await this.SaveSettingsAsync();
        });
    }

    public ReactiveCommand<Unit, bool> SaveSettingsCommand { get; }

    private bool isBusy;

    public bool IsBusy
    {
        get => isBusy;
        set
        {
            this.RaiseAndSetIfChanged(ref isBusy, value);
            this.RaisePropertyChanged(nameof(IsFormReady));
        }
    }

    private bool isFormReady;

    public bool IsFormReady
    {
        get => !isBusy & isFormReady;
        set => this.RaiseAndSetIfChanged(ref isFormReady, value);
    }

    private string currentPassword = string.Empty;

    public string CurrentPassword
    {
        get => currentPassword;
        set => this.RaiseAndSetIfChanged(ref currentPassword, value);
    }

    private string newPassword = string.Empty;

    public string NewPassword
    {
        get => newPassword;
        set => this.RaiseAndSetIfChanged(ref newPassword, value);
    }

    private string reNewPassword = string.Empty;

    public string ReNewPassword
    {
        get => reNewPassword;
        set => this.RaiseAndSetIfChanged(ref reNewPassword, value);
    }

    private string formError = string.Empty;

    public string FormError
    {
        get => formError;
        set => this.RaiseAndSetIfChanged(ref formError, value);
    }

    private CancellationTokenSource? _cancellationTokenSource;

    public Task<bool> SaveSettingsAsync()
    {
        IsBusy = true;
        this.FormError = string.Empty;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return Task.Run(() =>
        {
            Thread.Sleep(1000);

            if (this.HasErrors)
            {
                IsBusy = false;
                return false;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                bool checkPasswordResult = CheckAppPassword(this.CurrentPassword, _appStateModel.CryptedAppPassword);
                if (!checkPasswordResult)
                {
                    this.FormError = $"Error: Incorrent Current Password.";
                    IsBusy = false;
                    return false;
                }
                ChangeAppPassword(this.NewPassword);
                var notes = _appStateModel.NotesControlViewModel.Notes.ToList();
                string notesJ = JsonSerializer.Serialize(notes);
                string cryptedNotes = Cryptor.Crypt(notesJ);
                _appStateModel.NotesControlViewModel.CryptedAppNotes = cryptedNotes;
            }

            IsBusy = false;
            return true;
        });
    }

    private static bool CheckAppPassword(string appPassword, string hash)
    {
        bool result = Cryptor.CheckMD5(appPassword, hash);
        return result;
    }

    private void ChangeAppPassword(string appPassword)
    {
        string hash = Cryptor.HashMD5(appPassword);
        _appStateModel.CryptedAppPassword = hash;
        Cryptor.CryptKey = appPassword;
    }
}