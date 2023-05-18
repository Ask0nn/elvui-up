using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.ViewModels
{
    public partial class FAQViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private async void InitializeViewModel()
        {
            _isInitialized = true;
        }

        [RelayCommand]
        private void CopyToClipboard(string parameter)
        {
            Clipboard.SetText(parameter);
        }
    }
}
