using Premy.Chatovatko.Client.Libs.UserData;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Premy.Chatovatko.Client.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage(SettingsCapsula settings)
        {
            InitializeComponent();
            {
                var navigationPage = new NavigationPage(new ThreadsList(settings));
                navigationPage.Title = "Threads";
                Children.Add(navigationPage);
            }
            {
                var navigationPage = new NavigationPage(new ContactList(settings));
                navigationPage.Title = "Contacts";
                Children.Add(navigationPage);
            }
            
        }
    }
}
