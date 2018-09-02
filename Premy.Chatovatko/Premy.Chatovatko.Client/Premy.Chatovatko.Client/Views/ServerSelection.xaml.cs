using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Premy.Chatovatko.Client.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ServerSelection : ContentPage
	{
        private X509Certificate2 cert;
        private App app;

		public ServerSelection (App app, X509Certificate2 cert)
		{
			InitializeComponent ();
            this.cert = cert;
            this.app = app;
		}

        public void Confirm()
        {
            app.AfterServerSelected(cert, serverAddressEntry.Text, serverAddressEntry.Text);
        }

	}
}
