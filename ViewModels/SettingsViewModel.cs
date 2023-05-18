using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace ElvUiUpdater.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly IDialogControl _dialogControl;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private Wpf.Ui.Appearance.ThemeType _currentTheme = Wpf.Ui.Appearance.ThemeType.Unknown;

        public SettingsViewModel(IDialogService dialogService)
        {
            _dialogControl = dialogService.GetDialogControl();
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            CurrentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();
            AppVersion = $"ElvUI Updater - {GetAssemblyVersion()}";

            _dialogControl.ButtonRightClick += DialogControlOnButtonRightClick;

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }

        private void DialogControlOnButtonRightClick(object sender, RoutedEventArgs e)
        {
            var dialogControl = (IDialogControl)sender;
            dialogControl.Hide();
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Light)
                        break;

                    Properties.Settings.Default.IsDark = false;
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Light;

                    break;

                default:
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Dark)
                        break;

                    Properties.Settings.Default.IsDark = true;
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Dark;

                    break;
            }
        }

        [RelayCommand]
        private void OnSelectFolder()
        {
            FolderBrowserDialog openFolderDlg = new FolderBrowserDialog();
            if (openFolderDlg.ShowDialog() != DialogResult.OK)
                return;

            Properties.Settings.Default.WoWPath = openFolderDlg.SelectedPath;

            if (Directory.Exists(Path.Combine(openFolderDlg.SelectedPath, "_retail_")))
                Properties.Settings.Default.Retail = Path.Combine(openFolderDlg.SelectedPath, "_retail_");
            else
                Properties.Settings.Default.Retail = String.Empty;
            if (Directory.Exists(Path.Combine(openFolderDlg.SelectedPath, "_ptr_")))
                Properties.Settings.Default.PTR = Path.Combine(openFolderDlg.SelectedPath, "_ptr_");
            else
                Properties.Settings.Default.PTR = String.Empty;
            if (Directory.Exists(Path.Combine(openFolderDlg.SelectedPath, "_classic_")))
                Properties.Settings.Default.Classic = Path.Combine(openFolderDlg.SelectedPath, "_classic_");
            else
                Properties.Settings.Default.Classic = String.Empty;

            if (string.IsNullOrEmpty(Properties.Settings.Default.Retail))
                _dialogControl.Show("Ошибка", "Вы выбрали не правильниый путь с игрой");

            Properties.Settings.Default.Save();
        }
    }
}
