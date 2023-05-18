using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.ViewModels
{
    public partial class HomeViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private SolidColorBrush _primaryAccent = new SolidColorBrush(Accent.SystemAccent);

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
            _isInitialized = true;
        }
    }
}
