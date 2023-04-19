using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReqNotes.ViewModels;
using System;

namespace ReqNotes.Views;

public partial class SettingsWindow : ReactiveWindow<SettingsWindowViewModel>
{
    private readonly SettingsWindowViewModel _settingsWindowViewModel;

    public SettingsWindow()
    {
        InitializeComponent();

        _settingsWindowViewModel = new SettingsWindowViewModel();
        this.DataContext = _settingsWindowViewModel;

        this.WhenActivated(d => d(ViewModel!.SaveSettingsCommand.Subscribe(DoSaveSettings)));
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void DoSaveSettings(bool settingsResult)
    {
        if (settingsResult)
            this.Close();
    }
}