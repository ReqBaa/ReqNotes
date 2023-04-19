using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReqNotes.ViewModels;

public class LoginWindowViewModel : ReactiveValidationObject
{
    private readonly AppStateModel _appStateModel;

    public LoginWindowViewModel()
    {
        _appStateModel = RxApp.SuspensionHost.GetAppState<AppStateModel>();

        this.ValidationRule(
            viewModel => viewModel.AppPassword,
            password => password?.Length >= 6,
            "Password must at least 6.");

        this.WhenAnyValue(x => x.HasErrors)
                .Subscribe(x => this.IsFormReady = !HasErrors);

        LoginCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            return await this.CheckPasswordAsync(this.AppPassword, _appStateModel.CryptedAppPassword);
        });
    }

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

    private string passwordError = string.Empty;

    public string PasswordError
    {
        get => passwordError;
        set => this.RaiseAndSetIfChanged(ref passwordError, value);
    }

    public ReactiveCommand<Unit, bool> LoginCommand { get; }

    private CancellationTokenSource? _cancellationTokenSource;

    public Task<bool> CheckPasswordAsync(string appPassword, string hash)
    {
        IsBusy = true;

        this.PasswordError = "";

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return Task.Run(() =>
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(appPassword))
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);

                    result = Cryptor.CheckMD5(appPassword, hash);
                    if (result)
                        Cryptor.CryptKey = appPassword;
                    else
                        this.PasswordError = "Error: Incorrect password.";
                }
            }

            IsBusy = false;
            return result;
        });
    }
}