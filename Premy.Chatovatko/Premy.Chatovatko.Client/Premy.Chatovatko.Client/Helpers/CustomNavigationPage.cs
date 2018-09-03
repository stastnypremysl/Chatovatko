using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Premy.Chatovatko.Client.Helpers
{
    public class CustomNavigationPage : NavigationPage
    {
        private readonly Action<Page> changeMainPage;
        private Page _defaultPage;
        public Page DefaultPage {
            get
            {
                return _defaultPage;
            }
            set
            {
                _defaultPage = value;
                emptyPage.DefaultPage = value;
            }
        }
        private readonly EmptyPage emptyPage;

        public CustomNavigationPage(Action<Page> changeMainPage, Page defaultPage, Page goToPage) : base()
        {
            this.changeMainPage = changeMainPage;
            emptyPage = new EmptyPage(changeMainPage, defaultPage);
            this.PushAsync(emptyPage).ContinueWith((u) => { PushAsync(goToPage); });

            DefaultPage = defaultPage;
            
        }
               
    }
}
