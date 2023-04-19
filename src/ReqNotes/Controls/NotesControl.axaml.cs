using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReqNotes.ViewModels;
using ReqNotes.Views;
using System;
using System.Reactive.Linq;

namespace ReqNotes.Controls;

public partial class NotesControl : ReactiveUserControl<NotesControlViewModel>
{
    private readonly AppStateModel _appStateModel;

    public NotesControl()
    {
        InitializeComponent();

        AvaloniaXamlLoader.Load(this);
        //this.WhenActivated(disposable => { });

        this.WhenActivated(d => d(ViewModel!.NewNoteCommand.Subscribe(DoShowNoteWindow)));
        this.WhenActivated(d => d(ViewModel!.EditNoteCommand.Subscribe(DoShowNoteWindow)));

        _appStateModel = RxApp.SuspensionHost.GetAppState<AppStateModel>();
        _appStateModel.NotesControlViewModel.FillNotes();
        this.DataContext = _appStateModel.NotesControlViewModel;
    }

    private void DoShowNoteWindow(NoteViewModel note)
    {
        NoteWindow noteWindow = new(note.Id, note.Title, note.Content);
        noteWindow.ShowDialog(this.VisualRoot as Window);
    }
}