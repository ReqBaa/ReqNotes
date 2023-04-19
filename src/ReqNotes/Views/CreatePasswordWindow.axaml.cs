using Avalonia.ReactiveUI;
using ReactiveUI;
using ReqNotes.ViewModels;
using System;
using System.Reactive.Linq;

namespace ReqNotes.Views;

public partial class CreatePasswordWindow : ReactiveWindow<CreatePasswordWindowViewModel>
{
    private readonly CreatePasswordWindowViewModel _createPasswordWindowViewModel = new();

    public CreatePasswordWindow()
    {
        InitializeComponent();

        this.DataContext = _createPasswordWindowViewModel;

        //_createPasswordWindowViewModel.CreatePasswordCommand.Subscribe(x => DoShowMainWindow(x));
        this.WhenActivated(d => d(ViewModel!.CreatePasswordCommand.Subscribe(DoShowMainWindow)));
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