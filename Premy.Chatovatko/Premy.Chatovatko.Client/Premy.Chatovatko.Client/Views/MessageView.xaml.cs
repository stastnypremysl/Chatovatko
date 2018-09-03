using Premy.Chatovatko.Client.Libs.Database;
using Premy.Chatovatko.Client.Libs.Database.InsertModels;
using Premy.Chatovatko.Client.Libs.Database.Models;
using Premy.Chatovatko.Client.Libs.Sync;
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
	public partial class MessageView : ContentPage, IUpdatable, ILoggable
	{
        private App app;
        private long threadId;
        private long withUserId;

        public MessageView(App app, long threadId)
        {
            InitializeComponent();
            this.app = app;
            this.threadId = threadId;
            app.AddUpdatable(this);

            messageLabel.Text = GetMessages(threadId);

            using (Context context = new Context(app.settings.Config))
            {
                withUserId = context.MessagesThread
                    .Where(u => u.PublicId == threadId)
                    .Select(u => u.WithUser)
                    .Single();
                if(!context.Contacts
                    .Where(u => u.PublicId == withUserId)
                    .Select(u => u.Trusted == 1)
                    .Single())
                {
                    ShowFatalError(new Exception("Can't communicate with untrusted user."));
                }
            }

            toSend.Completed += (sender, e) => {
                send_Clicked(null, null);
            };
        }

        private String GetMessages(long threadId)
        {
            String format = "{1}; {0}:\n{2}\n\n";
            StringBuilder builder = new StringBuilder();

            using (Context context = new Context(app.settings.Config))
            {
                builder.AppendLine();

                foreach (var message in context.Messages
                    .Where(u => u.IdMessagesThread == threadId)
                    .OrderBy(u => -u.Id))
                {
                    builder.AppendLine(string.Format(format, message.Date, InfoTools.GetMessageSenderShowName(context, message.Id), message.Text));
                }
            }
            return builder.ToString();
        }


        private async void send_Clicked(object sender, EventArgs e)
        {
            try
            {
                var strToSend = toSend.Text;
                toSend.Text = "";
                await Task.Run(() =>
                {
                    CMessage message = new CMessage(threadId, strToSend, DateTime.Now, withUserId, app.settings.UserPublicId);
                    using (Context context = new Context(app.settings.Config))
                    {
                        PushOperations.Insert(context, message, withUserId, app.settings.UserPublicId);
                    }
                    Update();
                });
                
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
        }

        private void close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        public void Update()
        {
            var message = GetMessages(threadId);
            
            if (!message.Equals(messageLabel.Text))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    messageLabel.Text = message;
                });
            }
        }

        private async Task ShowError(Exception ex)
        {
            app.logger.LogException(this, ex);
            await DisplayAlert(ex.Source, ex.Message, "OK");
        }

        private async Task ShowFatalError(Exception ex)
        {
            await ShowError(ex);
            await Navigation.PopModalAsync();
        }

        public string GetLogSource()
        {
            return "Message View";
        }
    }
}
