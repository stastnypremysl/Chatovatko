using Premy.Chatovatko.Client.Helpers;
using Premy.Chatovatko.Libs;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.SearchContact;
using Premy.Chatovatko.Libs.Logging;
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
	public partial class SearchUser : ContentPage, ILoggable
	{
        App app;
		public SearchUser (App app)
		{
			InitializeComponent ();
            this.app = app;
		}

        private async void close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void start_Clicked(object sender, EventArgs e)
        {
            try
            {
                SearchCServerCapsula capsula = null;
                using (LoadingLock theLock = new LoadingLock(this, "Trying to find the user..."))
                {
                    await Task.Run(() =>
                    {
                        if (userNameEntry.Text == null || !Validators.ValidateRegexUserName(userNameEntry.Text))
                        {
                            capsula = app.connection.SearchContact(int.Parse(publicIdEntry.Text));
                        }
                        else
                        {
                            capsula = app.connection.SearchContact(userNameEntry.Text);
                        }
                    });
                }

                await Navigation.PopModalAsync();
                if (!capsula.Succeeded)
                {   
                    ShowError("Searching user", "User not found");
                }
                else
                {
                    await app.MainPage.Navigation.PushModalAsync(new NavigationPage(new ContactDetail(app.settings, capsula, app)));
                }

            }
            catch(Exception ex)
            {
                app.logger.LogException(this, ex);
                ShowError(ex.Source, ex.Message);
            }

            
        }

        private async void ShowError(String name, String message)
        {
            await DisplayAlert(name, message, "OK");
        }

        public string GetLogSource()
        {
            return "Search user dialog";
        }
    }
}
