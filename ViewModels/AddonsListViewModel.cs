using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace ElvUiUpdater.ViewModels
{
    public partial class AddonsListViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private readonly INavigationService _navigationService;
        private readonly IDialogControl _dialogControl;

        public AddonsListViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
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
            _dialogControl.ButtonRightClick += DialogControlOnButtonRightClick;
            _isInitialized = true;
        }

        private void DialogControlOnButtonRightClick(object sender, RoutedEventArgs e)
        {
            var dialogControl = (IDialogControl)sender;
            dialogControl.Hide();
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
    }
}
