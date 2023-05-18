using System.Diagnostics;
using System.Windows.Navigation;
using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.Views.Pages
{
    public partial class ElvUIPage : INavigableView<ViewModels.ElvUIViewModel>
    {
        public ViewModels.ElvUIViewModel ViewModel
        {
            get;
        }

        public ElvUIPage(ViewModels.ElvUIViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent(); 
        }
    }
}
