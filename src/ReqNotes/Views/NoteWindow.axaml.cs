using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReqNotes.ViewModels;
using System;

namespace ReqNotes.Views;

public partial class NoteWindow : ReactiveWindow<NoteWindowViewModel>
{
    private readonly NoteWindowViewModel? _noteWindowViewModel;

    public NoteWindow()
    {
        InitializeComponent();

        _noteWindowViewModel = new NoteWindowViewModel();
        this.DataContext = _noteWindowViewModel;

        this.WhenActivated(d => d(ViewModel!.SaveNoteCommand.Subscribe(DoSaveNote)));
    }

    public NoteWindow(string id = "", string title = "", string content = "")
    {
        InitializeComponent();

        _noteWindowViewModel = new NoteWindowViewModel(new NoteViewModel(id, title, content));
        this.DataContext = _noteWindowViewModel;

        this.WhenActivated(d => d(ViewModel!.SaveNoteCommand.Subscribe(DoSaveNote)));
    }

    private void DoSaveNote(bool saveNoteResult)
    {
        if (saveNoteResult)
            this.Close();
    }

    private void ButtonNoteCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}