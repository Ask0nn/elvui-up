using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ElvUiUpdater.Dialogs;
using ElvUiUpdater.Properties;
using MaterialDesignThemes.Wpf;
using Ookii.Dialogs.Wpf;
using PropertyChanged;

namespace ElvUiUpdater.ViewModel;

[AddINotifyPropertyChangedInterface]
internal class MainWindowViewModel
{
    public string WowPath { get; set; }
    public ObservableCollection<string> ReleaseVersions { get; set; }
    public string ReleaseVersion { get; set; }

    public MainWindowViewModel()
    {
        WowPath = Settings.Default.WowPath;
        ReleaseVersions = new ObservableCollection<string>();
        ReleaseVersion = Settings.Default.ReleaseVersion;

        if (!string.IsNullOrEmpty(WowPath) && !WowPath.Equals("Root game directory"))
            ReleaseVersions = FindReleaseVersion(WowPath);
    }

    public ICommand SetPathCommand => new DelegateCommand((obj) =>
    {
        var dl = new VistaFolderBrowserDialog();
        if (dl.ShowDialog() == false) return;
        WowPath = dl.SelectedPath;
        ReleaseVersions = FindReleaseVersion(WowPath);
        if (ReleaseVersions.Count < 1)
            MessageBox.Show("Selected path not correct.\nPlease select root folder of game.");
        Settings.Default.WowPath = WowPath;
    });

    public ICommand InstallCommand => new DelegateCommand(async (obj) =>
    {
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                DialogsController.CloseMessages();
                await DialogsController.ShowLoadingDialog("MainDialog");
            });

            var ui = TukuiApi.GetInstance?.GetElvUi();
            var path = Path.Combine(WowPath, ReleaseVersion, "Interface", "Addons");

            WowPath = path;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (InstallerService.CheckUiVersion(path, ui.Version))
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    DialogsController.CloseMessages();
                    await DialogsController.ShowAlertDialog("You have the latest version.", "MainDialog");
                });
                return;
            }

            var uiZip = TukuiApi.GetInstance?.DownloadUI(ui.Url!, path);
            ZipFile.ExtractToDirectory(uiZip, path, true);

            File.Delete(uiZip);

            Application.Current.Dispatcher.Invoke(async () =>
            {
                DialogsController.CloseMessages();
                await DialogsController.ShowAlertDialog("Done!", "MainDialog");
            });

            Settings.Default.ReleaseVersions = new StringCollection();
            Settings.Default.ReleaseVersions.AddRange(ReleaseVersions.ToArray() ?? Array.Empty<string>());
            Settings.Default.ReleaseVersion = ReleaseVersion;
        });
    });

    public static ObservableCollection<string> FindReleaseVersion(string path)
    {
        var result = new ObservableCollection<string>();
        foreach (var dirName in Directory.GetDirectories(path).Select(Path.GetFileName).ToArray())
            if (Regex.IsMatch(dirName, "_\\w*_"))
                result.Add(dirName);
        return result;
    }
}