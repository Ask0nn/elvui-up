using Wpf.Ui.Common.Interfaces;

namespace ElvUiUpdater.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddonsListPage.xaml
    /// </summary>
    public partial class AddonsListPage : INavigableView<ViewModels.AddonsListViewModel>
    {
        public ViewModels.AddonsListViewModel ViewModel
        {
            get;
        }

        public AddonsListPage(ViewModels.AddonsListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
