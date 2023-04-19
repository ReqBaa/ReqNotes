using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;

namespace ReqNotes.ViewModels;

public class NoteViewModel : ReactiveValidationObject
{
    public NoteViewModel(string title = "", string content = "")
    {
        this.Id = Guid.NewGuid().ToString("N");
        this.Title = title;
        this.Content = content;
        this.CreatedDateTime = DateTime.Now;
    }

    public NoteViewModel(string id, string title = "", string content = "", DateTime? createdDateTime = null)
    {
        this.Id = id;
        this.Title = title;
        this.Content = content;
        this.CreatedDateTime = createdDateTime ?? DateTime.Now;

        this.ValidationRule(
            viewModel => viewModel.Title,
            title => !string.IsNullOrWhiteSpace(title),
            "Title shouldn't be null or white space.");
        this.ValidationRule(
            viewModel => viewModel.Content,
            content => !string.IsNullOrWhiteSpace(content),
            "Content shouldn't be null or white space.");
    }

    private string id = string.Empty;

    public string Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    private string title = string.Empty;

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    private string content = string.Empty;

    public string Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public DateTime CreatedDateTime { get; set; }
}