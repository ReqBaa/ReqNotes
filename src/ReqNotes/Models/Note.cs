using System;

namespace ReqNotes.Models;

public class Note
{
    public Note()
    {
    }

    public Note(string title = "", string content = "")
    {
        this.Id = Guid.NewGuid().ToString("N");
        this.Title = title;
        this.Content = content;
        this.CreatedDateTime = DateTime.Now;
    }

    public Note(string id, string title = "", string content = "", DateTime? createdDateTime = null)
    {
        this.Id = id;
        this.Title = title;
        this.Content = content;
        this.CreatedDateTime = createdDateTime ?? DateTime.Now;
    }

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
}