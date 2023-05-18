using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.Views.Pages
{
    public partial class AUIPage : INavigableView<ViewModels.AUIViewModel>
    {
        public ViewModels.AUIViewModel ViewModel
        {
            get;
        }

        public AUIPage(ViewModels.AUIViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}
