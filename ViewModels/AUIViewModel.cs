using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElvUiUpdater.Api;
using System;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using System.Windows;
using System.Linq;

namespace ElvUiUpdater.ViewModels
{
    public partial class AUIViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly INavigationService _navigationService;
        private readonly IDialogControl _dialogControl;

        private readonly AtrocityApi _atrocityApi = new AtrocityApi();

        [ObservableProperty]
        private string _installedVersionRetail = "Not installed";

        [ObservableProperty]
        private string _installedVersionClassic = "Not installed";

        [ObservableProperty]
        private string _installedVersionPTR = "Not installed";

        [ObservableProperty]
        private string _webVersion = "Loading...";

        [ObservableProperty]
        private Models.VersionType _versionType = Models.VersionType._retail_;

        [ObservableProperty]
        private Models.VersionType[] _versionTypes = Enum.GetValues<Models.VersionType>();

        [ObservableProperty]
        private bool _buttonIsEnabled = true;

        [ObservableProperty]
        private string _changelog = string.Empty;

        [ObservableProperty]
        private string _key = string.Empty;

        [ObservableProperty]
        private bool _isRetentionInstall = false;

        public AUIViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogControl = dialogService.GetDialogControl();
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            LoadVersions();
        }

        public void OnNavigatedFrom()
        {
        }

        private void DialogControlOnButtonRightClick(object sender, RoutedEventArgs e)
        {
            var dialogControl = (IDialogControl)sender;
            dialogControl.Hide();
        }

        private async void InitializeViewModel()
        {
            Changelog = await _atrocityApi.GetChangelog();
            Key = await _atrocityApi.GetKey();
            ButtonIsEnabled = true;

            _dialogControl.ButtonRightClick += DialogControlOnButtonRightClick;

            _isInitialized = true;
        }

        private async void LoadVersions()
        {
            InstalledVersionRetail = _atrocityApi.GetVersion(Models.VersionType._retail_);
            InstalledVersionClassic = _atrocityApi.GetVersion(Models.VersionType._classic_);
            InstalledVersionPTR = _atrocityApi.GetVersion(Models.VersionType._ptr_);
            WebVersion = await _atrocityApi.GetOnlineVersion();
        }

        private async Task Install(string rootVersionPath)
        {
            if (!Directory.Exists(rootVersionPath))
                throw new DirectoryNotFoundException(rootVersionPath);

            foreach (string folder in AtrocityApi.FOLDERS_TO_CHECK)
                if (!Directory.Exists(Path.Combine(rootVersionPath, "___" + folder)))
                {
                    if (Directory.Exists(Path.Combine(rootVersionPath, folder)))
                        Directory.Move(Path.Combine(rootVersionPath, folder), Path.Combine(rootVersionPath, "___" + folder));
                }
                else
                    throw new ArgumentException(Path.Combine(rootVersionPath, "___" + folder));

            var auiZip = await _atrocityApi.DownloadAUI(rootVersionPath);
            ZipFile.ExtractToDirectory(auiZip, rootVersionPath, true);

            foreach (string folder in AtrocityApi.FOLDERS_TO_EXTRACT)
                Directory.Move(Path.Combine(rootVersionPath, "aui-load-main", folder), Path.Combine(rootVersionPath, folder));

            Directory.Delete(Path.Combine(rootVersionPath, "aui-load-main"), true);
            File.Delete(auiZip);

            if (IsRetentionInstall)
            {
                var addonRootDir = Path.Combine(rootVersionPath, "Interface", "Addons");
                var dirs = Directory.GetDirectories(Path.Combine(rootVersionPath, "___Interface", "Addons"));
                foreach (var dir in dirs)
                {
                    var path = Path.Combine(addonRootDir, Path.GetFileName(dir));
                    if (Directory.Exists(path)) continue;
                    Directory.Move(dir, Path.Combine(addonRootDir, Path.GetFileName(dir)));
                }
            }
        }

        [RelayCommand]
        private void OnDeleteOldFolders()
        {
            var rootVersionPath = VersionType == Models.VersionType._retail_ ? Properties.Settings.Default.Retail :
                VersionType == Models.VersionType._classic_ ? Properties.Settings.Default.Classic : Properties.Settings.Default.PTR;
            foreach (string folder in AtrocityApi.FOLDERS_TO_CHECK)
                if (Directory.Exists(Path.Combine(rootVersionPath, "___" + folder)))
                        Directory.Delete(Path.Combine(rootVersionPath, "___" + folder));

            _dialogControl.Show("", "Готово");
        }

        [RelayCommand]
        private void OnUriNavigate(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return;

            System.Diagnostics.ProcessStartInfo sInfo = new(new Uri(parameter).AbsoluteUri)
            {
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(sInfo);
        }

        [RelayCommand]
        private void OnNavigate(string parameter)
        {
            switch (parameter)
            {
                case "navigate_to_settings":
                    _navigationService.Navigate(typeof(Views.Pages.SettingsPage));
                    return;                    
            }
        }

        [RelayCommand]
        private void OnInstall()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.WoWPath))
            {
                _dialogControl.Show("Предупреждение", "Перед установкой AUI, вам нужно указать путь к папке с игрой.");
                return;
            }
            else if (string.IsNullOrEmpty(Properties.Settings.Default.Retail) && VersionType.Equals(Models.VersionType._retail_))
            {
                _dialogControl.Show("Предупреждение", "Путь к папке _retail_ не найден!");
                return;
            }
            else if (string.IsNullOrEmpty(Properties.Settings.Default.Classic) && VersionType.Equals(Models.VersionType._classic_))
            {
                _dialogControl.Show("Предупреждение", "Путь к папке _classic_ не найден!");
                return;
            }
            else if (string.IsNullOrEmpty(Properties.Settings.Default.PTR) && VersionType.Equals(Models.VersionType._ptr_))
            {
                _dialogControl.Show("Предупреждение", "Путь к папке _ptr_ не найден!");
                return;
            }

            Task.Run(async () =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ButtonIsEnabled = false;
                });

                await Task.Delay(3000);

                try
                {
                    switch (VersionType)
                    {
                        case Models.VersionType._retail_:
                                await Install(Properties.Settings.Default.Retail);
                            break;
                        case Models.VersionType._classic_:
                                await Install(Properties.Settings.Default.Classic);
                            break; ;
                        case Models.VersionType._ptr_:
                                await Install(Properties.Settings.Default.PTR);
                            break;
                    }

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ButtonIsEnabled = true;
                        _dialogControl.Show("", "Готово!");
                        LoadVersions();
                    });
                }
                catch (DirectoryNotFoundException ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ButtonIsEnabled = true;
                        _dialogControl.Show("Ошибка", $"Папка '{ex.Message}' не найдена");
                    });
                }
                catch (ArgumentException ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ButtonIsEnabled = true;
                        _dialogControl.Show("Ошибка", $"Папка '{ex.Message}' уже существует");
                    });
                }
            });
        }
    }
}
