using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Premy.Chatovatko.Client.Helpers
{
    public class EmptyPage : Page
    {
        public Page DefaultPage;
        private readonly Action<Page> changeMainPage;

        public EmptyPage(Action<Page> changeMainPage, Page defaultPage)
        {
            this.changeMainPage = changeMainPage;
            DefaultPage = defaultPage;
        }

        protected override void OnAppearing()
        {
            changeMainPage(DefaultPage);
        }
    }
}
