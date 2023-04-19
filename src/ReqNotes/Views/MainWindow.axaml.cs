using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ReqNotes.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AvaloniaXamlLoader.Load(this);

        this.DataContext = new AppStateModel();
    }

    private void ButtonSettings_Click(object sender, RoutedEventArgs e)
    {
        SettingsWindow settingsWindow = new();
        settingsWindow.ShowDialog(this);
    }
}