using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using ReqNotes.Helpers;
using ReqNotes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReqNotes.ViewModels;

public class NotesControlViewModel : ReactiveValidationObject
{
    public NotesControlViewModel()
    {
        NewNoteCommand = ReactiveCommand.Create(() => new NoteViewModel());

        EditNoteCommand = ReactiveCommand.Create(new Func<string, NoteViewModel>((id) =>
        {
            return this.Notes.FirstOrDefault(x => x.Id == id, new NoteViewModel());
        }));

        RemoveNoteCommand = ReactiveCommand.Create(new Func<string, bool>((id) =>
        {
            NoteViewModel? note = this.Notes.FirstOrDefault(x => x.Id == id);
            if (note != null)
                this.Notes.Remove(note);
            return true;
        }));
    }

    [JsonIgnore]
    [IgnoreDataMember]
    public ReactiveCommand<Unit, NoteViewModel> NewNoteCommand { get; }

    [JsonIgnore]
    [IgnoreDataMember]
    public ReactiveCommand<string, NoteViewModel> EditNoteCommand { get; }

    [JsonIgnore]
    [IgnoreDataMember]
    public ReactiveCommand<string, bool> RemoveNoteCommand { get; }

    private string searchQuery = string.Empty;

    [DataMember]
    public string SearchQuery
    {
        get => searchQuery;
        set
        {
            this.RaiseAndSetIfChanged(ref searchQuery, value);
            this.RaisePropertyChanged(nameof(this.NotesResult));
        }
    }

    [JsonIgnore]
    [IgnoreDataMember]
    public ObservableCollection<NoteViewModel> NotesResult
    {
        get
        {
            if (string.IsNullOrWhiteSpace(this.SearchQuery))
                return notes;
            else
                return new ObservableCollection<NoteViewModel>(notes.Where(x => x.Title.Contains(this.SearchQuery)));
        }
    }

    private ObservableCollection<NoteViewModel> notes = new();

    [JsonIgnore]
    [IgnoreDataMember]
    public ObservableCollection<NoteViewModel> Notes
    {
        get => notes;
        set
        {
            this.RaiseAndSetIfChanged(ref notes, value);
            notes.CollectionChanged += Notes_CollectionChanged;
        }
    }

    private void Notes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        string notesJ = JsonSerializer.Serialize(this.notes.ToList());

        string cryptedNotes = Cryptor.Crypt(notesJ);
        this.CryptedAppNotes = cryptedNotes;
        this.RaisePropertyChanged(nameof(this.NotesResult));
    }

    private string cryptedAppNotes = string.Empty;

    [DataMember]
    public string CryptedAppNotes
    {
        get => cryptedAppNotes;
        set => this.RaiseAndSetIfChanged(ref cryptedAppNotes, value);
    }

    public void FillNotes()
    {
        if (!string.IsNullOrWhiteSpace(this.CryptedAppNotes))
        {
            string decrypedNotesJ = Cryptor.Decrypt(this.CryptedAppNotes);
            List<Note>? notes = JsonSerializer.Deserialize<List<Note>>(decrypedNotesJ);
            if (notes != null)
                this.Notes = Mapper.Map<ObservableCollection<NoteViewModel>>(notes);
        }
        else
            this.Notes = new ObservableCollection<NoteViewModel>();
    }
}