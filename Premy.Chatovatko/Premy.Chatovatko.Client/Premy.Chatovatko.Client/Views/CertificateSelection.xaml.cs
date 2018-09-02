using Premy.Chatovatko.Client.Cryptography;
using Premy.Chatovatko.Libs.Logging;
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
    public partial class CertificateSelection : ContentPage
    {
        App app;
        Logger logger;

        public CertificateSelection(App app, Logger logger)
        {
            InitializeComponent();
            this.app = app;
            this.logger = logger;
            IsGenerating = false;
        }

        private async void Generate()
        {
            X509Certificate2 clientCert;
            IsGenerating = true;
            await Task.Run(() =>
            {
                clientCert = X509Certificate2Generator.GenerateCACertificate(logger);
            });
            IsGenerating = false;

        }

        private void LoadFromFile()
        {

        }

        private bool _IsGenerating;
        public bool IsGenerating
        {
            get { return _IsGenerating; }
            set
            {
                _IsGenerating = value;
                generatingLabel.IsVisible = value;
                activityIndicator.IsVisible = value;
                activityIndicator.IsRunning = value;

                introLabel.IsVisible = !value;
                buttonLayout.IsVisible = !value;
            }
        }

    }
}
