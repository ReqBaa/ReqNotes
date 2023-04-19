using ReactiveUI;
using ReqNotes.ViewModels;
using System.Runtime.Serialization;

namespace ReqNotes;

[DataContract]
public class AppStateModel : ReactiveObject
{
    public string AppName = "ReqNotes";

    private string appVersion = App.AppVersion;
    [DataMember]
    public string AppVersion
    {
        get => appVersion;
        set => this.RaiseAndSetIfChanged(ref appVersion, value);
    }

    private NotesControlViewModel notesControlViewModel = new();

    [DataMember]
    public NotesControlViewModel NotesControlViewModel
    {
        get => notesControlViewModel;
        set => this.RaiseAndSetIfChanged(ref notesControlViewModel, value);
    }

    private string cryptedAppPassword = string.Empty;

    [DataMember]
    public string CryptedAppPassword
    {
        get => cryptedAppPassword;
        set => this.RaiseAndSetIfChanged(ref cryptedAppPassword, value);
    }
}