using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Premy.Chatovatko.Client.Views;
using System.Reflection;
using Premy.Chatovatko.Libs.Logging;
using Premy.Chatovatko.Client.Libs.UserData;
using Premy.Chatovatko.Client.Libs.Database;
using Premy.Chatovatko.Client.Libs.ClientCommunication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Premy.Chatovatko.Libs.DataTransmission.JsonModels;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Premy.Chatovatko.Client
{
    public partial class App : Application, ILoggable
    {
        Logger logger;
        IClientDatabaseConfig config;
        DBInitializator initializator;
        SettingsLoader settingsLoader;
        SettingsCapsula settings = null;
        Connection connection = null;

        public App()
        {
            InitializeComponent();
            logger = new Logger (new DebugLoggerOutput());
            config = new ClientDatabaseConfig();
            initializator = new DBInitializator(config, logger);
            initializator.DBEnsureCreated();

            settingsLoader = new SettingsLoader(config, logger);

            if (settingsLoader.Exists())
            {
                Log("Settings exists and will be loaded.");
                settings = settingsLoader.GetSettingsCapsula();
                connection = new Connection(logger, settings);
            }
            else
            {
                Log("Settings doesn't exist.");
                MainPage = new CertificateSelection(this, logger);
            }

            Init();

        }



        private void Init()
        {

        }

        
        public void AfterCertificateSelected(X509Certificate2 cert)
        {
            MainPage = new ServerSelection(this, cert);
        }

        public async void AfterServerSelected(X509Certificate2 clientCert, String address, String password, String userName)
        {
            MainPage = new Loading("Server informations are downloading.");
            try
            {
                InfoConnection infoConnection = new InfoConnection(address, logger);
                ServerInfo info = null;

                await Task.Run(() =>
                {
                    info = infoConnection.DownloadInfo();
                });

                MainPage = new ServerVerification(this, info, clientCert, address, password, userName);
            }
            catch (Exception ex)
            {
                MainPage = new ServerSelection(this, clientCert, address, password, userName, ex.Message);
            }
        }
        
        public async void AfterServerConfirmed(X509Certificate2 clientCert, X509Certificate2 serverCert, String address, String password, String userName)
        {
            MainPage = new Loading("Client is being registred.");
            try
            { 
                await Task.Run(() =>
                {
                    Register(clientCert, serverCert, address, password);
                });
                Init();
            }
            catch(Exception ex)
            {
                MainPage = new ServerSelection(this, clientCert, address, password, userName, ex.Message);
            }
        }

        public void Register(X509Certificate2 clientCert, X509Certificate2 serverCert, String address, String password)
        {

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public string GetLogSource()
        {
            return "Log";
        }

        private void Log(String message)
        {
            logger.Log(this, message);
        }
    }
}
