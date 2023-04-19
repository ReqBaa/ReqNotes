using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReqNotes.Views;
using System;
using System.IO;
using System.Text.Json;

namespace ReqNotes
{
    public partial class App : Application
    {
        public static readonly string AppVersion = "1.7";

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {

            //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            //{
            //    desktop.MainWindow = new MainWindow
            //    {
            //        DataContext = new MainWindowViewModel(),
            //    };
            //}

            // Create initial application folders and files
            string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appBaseFolder = Path.Combine(user, "ReqBaa");
            if (!Directory.Exists(appBaseFolder))
                Directory.CreateDirectory(appBaseFolder);
            string appFolder = Path.Combine(appBaseFolder, "ReqNotes");
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);
            string appstatefilefullname = Path.Combine(appFolder, "appstate.data");

            // Check version
            bool needUpdate = CheckUpdateRequired();
            if (needUpdate)
                UpdateAction(appstatefilefullname);

            // Create the AutoSuspendHelper.
            AutoSuspendHelper? suspension = new(ApplicationLifetime!);
            RxApp.SuspensionHost.CreateNewAppState = () => new AppStateModel();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new JsonSuspensionDriver(appstatefilefullname));
            suspension.OnFrameworkInitializationCompleted();

            // Load the saved view model state.
            AppStateModel? state = RxApp.SuspensionHost.GetAppState<AppStateModel>();

            if (string.IsNullOrEmpty(state.CryptedAppPassword))
                new CreatePasswordWindow().Show();
            else
                new LoginWindow().Show();

            base.OnFrameworkInitializationCompleted();
        }

        public static bool CheckUpdateRequired()
        {
            bool needUpdate = false;
            string currentAppVersion = App.AppVersion;
            Version currentVersion = new(currentAppVersion);
            string usersAppVersion = "1.0";
            Version usersVersion = new(usersAppVersion);
            switch (usersVersion.CompareTo(currentVersion))
            {
                case 0:
                    // users version is the same
                    break;

                case 1:
                    // users version is larger
                    break;

                case -1:
                    // users version is earlier
                    needUpdate = true;
                    break;
            }
            return needUpdate;
        }

        public static void UpdateAction(string appstatefilefullname)
        {
            if (!File.Exists(appstatefilefullname)) return;
            string file = File.ReadAllText(appstatefilefullname);
            AppStateModel? statefile = JsonSerializer.Deserialize<AppStateModel>(file);
            if (statefile == null) return;

            // update state info
            statefile.AppVersion = App.AppVersion;

            // TODO: new version updates

            // save
            var lines = JsonSerializer.Serialize(statefile, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true
            });
            File.WriteAllText(appstatefilefullname, lines);
        }
    }
}