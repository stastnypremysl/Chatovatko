using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Premy.Chatovatko.Client.Cryptography;
using Premy.Chatovatko.Libs.Cryptography;
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
            IsLoadingFile = false;
        }

        private async void Generate()
        {
            X509Certificate2 clientCert = null;
            IsGenerating = true;
            await Task.Run(() =>
            {
                clientCert = X509Certificate2Generator.GenerateCACertificate(logger);
            });
            
            app.AfterCertificateSelected(clientCert);
        }

        private async void LoadFromFile()
        {
            IsLoadingFile = true;
            FileData fileData = await CrossFilePicker.Current.PickFile();
            if (fileData == null)
            {
                IsLoadingFile = false;
                return;
            }

            X509Certificate2 cert = X509Certificate2Utils.ImportFromPkcs12(fileData.DataArray);

            app.AfterCertificateSelected(cert);
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

        private bool _IsLoadingFile;
        public bool IsLoadingFile
        {
            get { return _IsLoadingFile; }
            set
            {
                _IsLoadingFile = value;
                loadingFileLabel.IsVisible = value;
                activityIndicator.IsVisible = value;
                activityIndicator.IsRunning = value;

                introLabel.IsVisible = !value;
                buttonLayout.IsVisible = !value;
            }
        }

    }
}
