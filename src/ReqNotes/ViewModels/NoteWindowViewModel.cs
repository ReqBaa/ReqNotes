using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace ReqNotes.ViewModels;

public class NoteWindowViewModel : ReactiveValidationObject
{
    private readonly AppStateModel _appStateModel;

    public NoteWindowViewModel(NoteViewModel? noteViewModel = null)
    {
        this.noteViewModel = noteViewModel ?? new NoteViewModel();

        _appStateModel = RxApp.SuspensionHost.GetAppState<AppStateModel>();

        this.WhenAnyValue(x => x.NoteViewModel.HasErrors)
                .Subscribe(x => this.IsFormReady = !NoteViewModel.HasErrors);

        SaveNoteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            return await this.SaveNoteAsync();
        });
    }

    private NoteViewModel noteViewModel;

    public NoteViewModel NoteViewModel
    {
        get => noteViewModel;
        set => this.RaiseAndSetIfChanged(ref noteViewModel, value);
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

    public ReactiveCommand<Unit, bool> SaveNoteCommand { get; }

    private CancellationTokenSource? _cancellationTokenSource;

    public Task<bool> SaveNoteAsync()
    {
        IsBusy = true;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        return Task.Run(() =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                NoteViewModel? note = _appStateModel.NotesControlViewModel.Notes.FirstOrDefault(x => x.Id == this.NoteViewModel.Id);
                if (note == null)
                {
                    //note = Mapper.Map(this.NoteViewModel);
                    _appStateModel.NotesControlViewModel.Notes.Add(this.NoteViewModel);
                }
                else
                {
                    note.Title = this.NoteViewModel.Title;
                    note.Content = this.NoteViewModel.Content;
                }
            }
            IsBusy = false;
            return true;
        });
    }
}