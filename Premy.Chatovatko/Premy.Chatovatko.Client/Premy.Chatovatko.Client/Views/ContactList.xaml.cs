using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Premy.Chatovatko.Client.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ContactList : ContentPage
	{
        private SettingsCapsula settings;
        private readonly Action<Page> pushToNavigation;

		public ContactList (SettingsCapsula settings, Action<Page> pushToNavigation)
		{
			InitializeComponent ();
            this.settings = settings;
            this.pushToNavigation = pushToNavigation;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new ContactsViewModel(settings);
        }

        private async void OnContactTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var contact = (Contacts)e.Item;
                await Navigation.PushModalAsync(new ContactDetail(settings, contact.PublicId));
            }
        }

    }
}
