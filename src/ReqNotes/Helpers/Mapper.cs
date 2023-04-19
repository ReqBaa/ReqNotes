using ReqNotes.Models;
using ReqNotes.ViewModels;
using System.Collections.Generic;

namespace ReqNotes.Helpers;

public static partial class Mapper
{
    public static NoteViewModel Map(Note note) => new(note.Id, note.Title, note.Content);

    public static Note Map(NoteViewModel note) => new(note.Id, note.Title, note.Content);

    public static O Map<O>(IEnumerable<Note> items)
        where O : ICollection<NoteViewModel>, new()
    {
        O o = new();
        foreach (var item in items)
            o.Add(new NoteViewModel(item.Id, item.Title, item.Content));
        return o;
    }

    public static O Map<O>(IEnumerable<NoteViewModel> items)
        where O : ICollection<Note>, new()
    {
        O o = new();
        foreach (var item in items)
            o.Add(new Note(item.Id, item.Title, item.Content));
        return o;
    }
}