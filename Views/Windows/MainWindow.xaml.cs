using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace ElvUiUpdater.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        private bool _initialized = false;

        private string[] _skipPages = { "home", "curseforge", "settings", "faq" };

        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService, IDialogService dialogService)
        {
            ViewModel = viewModel;
            DataContext = this;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.WoWPath))
            {
                if (Directory.Exists(Path.Combine(Properties.Settings.Default.WoWPath, "_retail_")))
                    Properties.Settings.Default.Retail = Path.Combine(Properties.Settings.Default.WoWPath, "_retail_");
                else
                    Properties.Settings.Default.Retail = String.Empty;
                if (Directory.Exists(Path.Combine(Properties.Settings.Default.WoWPath, "_ptr_")))
                    Properties.Settings.Default.PTR = Path.Combine(Properties.Settings.Default.WoWPath, "_ptr_");
                else
                    Properties.Settings.Default.PTR = String.Empty;
                if (Directory.Exists(Path.Combine(Properties.Settings.Default.WoWPath, "_classic_")))
                    Properties.Settings.Default.Classic = Path.Combine(Properties.Settings.Default.WoWPath, "_classic_");
                else
                    Properties.Settings.Default.Classic = String.Empty;
            }

            InitializeComponent();
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);
            dialogService.SetDialogControl(RootDialog);

            Loaded += (_, _) => InvokeSplashScreen();

            Theme.Apply(Properties.Settings.Default.IsDark ? ThemeType.Dark : ThemeType.Light);
        }

        #region INavigationWindow methods

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ElvUiUpdater.Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void InvokeSplashScreen()
        {
            if (_initialized)
                return;

            _initialized = true;

            RootMainGrid.Visibility = Visibility.Collapsed;
            RootWelcomeGrid.Visibility = Visibility.Visible;

            Task.Run(async () =>
            {
                await Task.Delay(4);

                await Dispatcher.InvokeAsync(() =>
                {
                    RootWelcomeGrid.Visibility = Visibility.Hidden;
                    RootMainGrid.Visibility = Visibility.Visible;

                    Navigate(typeof(Pages.HomePage));
                });

                return true;
            });
        }

        private void RootNavigation_OnNavigated(INavigation sender, RoutedNavigationEventArgs e)
        {
            RootFrame_Header.Visibility = _skipPages.Any(s => s.Equals(sender?.Current?.PageTag)) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}