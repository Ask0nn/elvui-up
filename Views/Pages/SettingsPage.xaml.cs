using System.Diagnostics;
using System.Windows.Navigation;
using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.Views.Pages
{
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            };
            p.Start();
            e.Handled = true;
        }
    }
}