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
        private VersionType _versionType = VersionType._retail_;

        [ObservableProperty]
        private VersionType[] _versionTypes = Enum.GetValues<VersionType>();

        [ObservableProperty]
        private bool _buttonIsEnabled = true;

        [ObservableProperty]
        private string _changelog = string.Empty;

        [ObservableProperty]
        private string _key = string.Empty;

        public AUIViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogControl = dialogService.GetDialogControl();
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            ValidateInstallationConditions();
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
            InstalledVersionRetail = _atrocityApi.GetVersion(VersionType._retail_);
            InstalledVersionClassic = _atrocityApi.GetVersion(VersionType._classic_);
            InstalledVersionPTR = _atrocityApi.GetVersion(VersionType._ptr_);
            WebVersion = await _atrocityApi.GetOnlineVersion();
        }

        private bool ValidateInstallationConditions()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.WoWPath) &&
                (string.IsNullOrEmpty(Properties.Settings.Default.Retail) || string.IsNullOrEmpty(Properties.Settings.Default.Classic)))
            {
                _dialogControl.Show("Предупреждение", "Перед установкой AUI, вам нужно указать путь к папке с игрой.");
                return false;
            }
            return true;
        }

        private bool ValidatePTRInstallationConditions()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.WoWPath) || string.IsNullOrEmpty(Properties.Settings.Default.PTR))
            {
                _dialogControl.Show("Предупреждение", "Версия PTR доступна только если у вас установлена PTR версия игры.");
                return false;
            }
            return true;
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
                        case VersionType._retail_:
                            if (ValidateInstallationConditions())
                                await Install(Properties.Settings.Default.Retail);
                            break;
                        case VersionType._classic_:
                            if (ValidateInstallationConditions())
                                await Install(Properties.Settings.Default.Classic);
                            break; ;
                        case VersionType._ptr_:
                            if (ValidatePTRInstallationConditions())
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
