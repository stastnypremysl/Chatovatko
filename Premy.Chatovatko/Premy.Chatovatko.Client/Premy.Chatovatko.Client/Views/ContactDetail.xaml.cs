using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels.SearchContact;
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
	public partial class ContactDetail : ContentPage
	{
        private bool _adding;
        private bool Adding
        {
            get
            {
                return _adding;
            }
            set
            {
                _adding = value;
                if (value)
                {
                    ToolbarItem item = new ToolbarItem("Add", "add.png", AddUser);
                    ToolbarItems.Add(item);
                }
                else
                {
                    ToolbarItem item = new ToolbarItem("Save", "save.png", SaveUser);
                    ToolbarItems.Add(item);
                }
            }
        }

		public ContactDetail (SettingsCapsula settings, long userPublicId)
		{
			InitializeComponent ();
            Adding = false;
		}

        public ContactDetail(SettingsCapsula settings, SearchCServerCapsula user)
        {
            InitializeComponent();
            Adding = true;
        }

        private void DiscardChanges()
        {
            Navigation.PopModalAsync();
        }

        private void AddUser()
        {
            Navigation.PopModalAsync();
        }

        private void SaveUser()
        {
            Navigation.PopModalAsync();
        }
    }
}
