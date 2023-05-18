using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElvUiUpdater.Api;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace ElvUiUpdater.ViewModels
{
    public partial class ElvUIViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly INavigationService _navigationService;
        private readonly IDialogControl _dialogControl;

        private readonly TukuiApi _tukuiApi = new TukuiApi();

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
        private string _buttonContent = "Установить";

        [ObservableProperty]
        private VersionType[] _versionTypes = Enum.GetValues<VersionType>();

        [ObservableProperty]
        private bool _buttonIsEnabled = true;

        public ElvUIViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogControl = dialogService.GetDialogControl();
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            LoadVersions(); 
            ValidateInstallationConditions();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            ButtonIsEnabled = true;

            _dialogControl.ButtonRightClick += DialogControlOnButtonRightClick;

            _isInitialized = true;
        }

        private async void LoadVersions()
        {
            InstalledVersionRetail = _tukuiApi.GetVersion(VersionType._retail_);
            InstalledVersionClassic = _tukuiApi.GetVersion(VersionType._classic_);
            InstalledVersionPTR = _tukuiApi.GetVersion(VersionType._ptr_);
            WebVersion = await _tukuiApi.GetOnlineVersion(VersionType._retail_);

            switch (VersionType)
            {
                case VersionType._retail_:
                    if (string.IsNullOrEmpty(Properties.Settings.Default.Retail))
                        ButtonContent = InstalledVersionRetail == "Not installed" ? "Установить" : "Обновить";
                    break;
                case VersionType._classic_:
                    if (string.IsNullOrEmpty(Properties.Settings.Default.Classic))
                        ButtonContent = InstalledVersionClassic == "Not installed" ? "Установить" : "Обновить";
                    break;
                case VersionType._ptr_:
                    if (string.IsNullOrEmpty(Properties.Settings.Default.PTR))
                        ButtonContent = InstalledVersionPTR == "Not installed" ? "Установить" : "Обновить";
                    else ButtonContent = "Установить";
                    break;
            }
        }

        private bool ValidateInstallationConditions()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.WoWPath) &&
                (string.IsNullOrEmpty(Properties.Settings.Default.Retail) || string.IsNullOrEmpty(Properties.Settings.Default.Classic)))
            {
                _dialogControl.Show("Предупреждение", "Перед установкой ElvUI, вам нужно указать путь к папке с игрой.");
                return false;
            }
            return true;
        }

        partial void OnVersionTypeChanged(VersionType value)
        {
            switch (VersionType)
            {
                case VersionType._retail_:
                    if (string.IsNullOrEmpty(Properties.Settings.Default.Retail))
                        ButtonContent = InstalledVersionRetail == "Not installed" ? "Установить" : "Обновить";
                    break;
                case VersionType._classic_:
                    if (string.IsNullOrEmpty(Properties.Settings.Default.Classic))
                        ButtonContent = InstalledVersionClassic == "Not installed" ? "Установить" : "Обновить";
                    break;
                case VersionType._ptr_:
                    if (string.IsNullOrEmpty(Properties.Settings.Default.PTR))
                        ButtonContent = InstalledVersionPTR == "Not installed" ? "Установить" : "Обновить";
                    else ButtonContent = "Установить";
                    break;
            }
        }

        private void DialogControlOnButtonRightClick(object sender, RoutedEventArgs e)
        {
            var dialogControl = (IDialogControl)sender;
            dialogControl.Hide();
        }

        private async Task InstallRetail()
        {
            var addonsPath = Path.Combine(Properties.Settings.Default.Retail, "Interface", "Addons");

            if (!Directory.Exists(addonsPath))
                throw new DirectoryNotFoundException(addonsPath);

            var elvuiZip = await _tukuiApi.DownloadElvUI(addonsPath);

            ZipFile.ExtractToDirectory(elvuiZip, addonsPath, true );
            File.Delete(elvuiZip);
        }

        private async Task InstallClassic()
        {
            var addonsPath = Path.Combine(Properties.Settings.Default.Classic, "Interface", "Addons");

            if (!Directory.Exists(addonsPath))
                throw new DirectoryNotFoundException(addonsPath);

            var elvuiZip = await _tukuiApi.DownloadElvUI(addonsPath);

            ZipFile.ExtractToDirectory(elvuiZip, addonsPath, true);
            File.Delete(elvuiZip);
        }

        private async Task InstallPTR()
        {
            var addonsPath = Path.Combine(Properties.Settings.Default.PTR, "Interface", "Addons");

            if (!Directory.Exists(addonsPath))
                throw new DirectoryNotFoundException(addonsPath);

            var elvuiZip = await _tukuiApi.DownloadElvUIPTR(addonsPath);

            ZipFile.ExtractToDirectory(elvuiZip, addonsPath, true);

            foreach (string folder in TukuiApi.FOLDERS_TO_EXTRACT)
            {
                var _folder = Path.Combine(addonsPath, folder);
                if (Directory.Exists(_folder)) Directory.Delete(_folder, true);
                Directory.Move(Path.Combine(addonsPath, "ElvUI-ptr", folder), _folder);
            }

            Directory.Delete(Path.Combine(addonsPath, "ElvUI-ptr"), true);
            File.Delete(elvuiZip);
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
                            await InstallRetail();
                            break;
                        case VersionType._classic_:
                            await InstallClassic();
                            break;
                        case VersionType._ptr_:
                            await InstallPTR();
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
            });
        }
    }
}
