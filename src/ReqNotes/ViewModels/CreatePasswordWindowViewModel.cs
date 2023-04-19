using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReqNotes.ViewModels;

public class CreatePasswordWindowViewModel : ReactiveValidationObject
{
    private readonly AppStateModel _appStateModel;

    public CreatePasswordWindowViewModel()
    {
        _appStateModel = RxApp.SuspensionHost.GetAppState<AppStateModel>();

        this.ValidationRule(
            viewModel => viewModel.AppPassword,
            password => password?.Length >= 6,
            "Password must at least 6.");

        this.ValidationRule(
            viewModel => viewModel.ReAppPassword,
            password => password?.Length >= 6,
            "Password must at least 6.");

        var passwordsObservable = this.WhenAnyValue(
        x => x.AppPassword,
        x => x.ReAppPassword,
        (password, repassword) =>
            new { Password = password, RePassword = repassword });
        this.ValidationRule(
            vm => vm.ReAppPassword,
            passwordsObservable,
            state => state.Password == state.RePassword,
            state => $"Passwords must match.");

        this.WhenAnyValue(x => x.HasErrors)
                .Subscribe(x => this.IsFormReady = !HasErrors);

        CreatePasswordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            return await this.CreatePasswordAsync(this.AppPassword);
        });
    }

    public ReactiveCommand<Unit, bool> CreatePasswordCommand { get; }

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

    private string appPassword = string.Empty;

    public string AppPassword
    {
        get => appPassword;
        set => this.RaiseAndSetIfChanged(ref appPassword, value);
    }

    private string reAppPassword = string.Empty;

    public string ReAppPassword
    {
        get => reAppPassword;
        set => this.RaiseAndSetIfChanged(ref reAppPassword, value);
    }

    private CancellationTokenSource? _cancellationTokenSource;

    public Task<bool> CreatePasswordAsync(string appPassword)
    {
        IsBusy = true;

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
                string hash = Cryptor.HashMD5(appPassword);
                _appStateModel.CryptedAppPassword = hash;
                Cryptor.CryptKey = appPassword;
            }

            IsBusy = false;
            return true;
        });
    }
}