using Premy.Chatovatko.Client.Helpers;
using Premy.Chatovatko.Client.Libs.UserData;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Premy.Chatovatko.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        App app;
        public MainPage(App app, SettingsCapsula settings)
        {
            this.app = app;
            InitializeComponent();
            {
                var navigationPage = new NavigationPage(new ThreadsList(settings, PushToNavigation));
                navigationPage.Title = "Threads";
                Children.Add(navigationPage);
            }
            {
                var navigationPage = new NavigationPage(new ContactList(settings, PushToNavigation));
                navigationPage.Title = "Contacts";
                Children.Add(navigationPage);
            }
            
        }

        private void PushToNavigation(Page page)
        {
            ChangeMainPage(new CustomNavigationPage(ChangeMainPage, this, page));
            
        }

        private void ChangeMainPage(Page page)
        {
            app.MainPage = page;
        }
    }
}
