using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для FAQPage.xaml
    /// </summary>
    public partial class FAQPage : INavigableView<ViewModels.FAQViewModel>
    {
        public ViewModels.FAQViewModel ViewModel
        {
            get;
        }

        public FAQPage(ViewModels.FAQViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}
