using Premy.Chatovatko.Client.Helpers;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.Sync;
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
	public partial class ContactList : ContentPage, IUpdatable
	{
        private SettingsCapsula settings;
        private App app;

		public ContactList (App app, SettingsCapsula settings)
		{
			InitializeComponent ();
            this.settings = settings;
            this.app = app;
            app.AddUpdatable(this);
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
                await Navigation.PushModalAsync(new NavigationPage(new ContactDetail(settings, contact.PublicId)));
            }
        }

        public void Update()
        {
            
        }

    }
}
