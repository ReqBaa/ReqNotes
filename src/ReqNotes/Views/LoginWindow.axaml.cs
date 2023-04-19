using Avalonia.ReactiveUI;
using ReactiveUI;
using ReqNotes.ViewModels;
using System;
using System.Reactive.Linq;

namespace ReqNotes.Views;

public partial class LoginWindow : ReactiveWindow<LoginWindowViewModel>
{
    private readonly LoginWindowViewModel _loginWindowViewModel = new();

    public LoginWindow()
    {
        InitializeComponent();
        this.DataContext = _loginWindowViewModel;
        //_loginWindowViewModel.LoginCommand.Subscribe(x => DoShowMainWindow(x));
        this.WhenActivated(d => d(ViewModel!.LoginCommand.Subscribe(DoShowMainWindow)));
    }

    private void DoShowMainWindow(bool checkPasswordResult)
    {
        if (checkPasswordResult)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}