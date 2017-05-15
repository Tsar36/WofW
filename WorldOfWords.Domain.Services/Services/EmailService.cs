using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;
using WorldOfWords.Domain.Services.Messages;

namespace WorldOfWords.Domain.Services.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await ConfigSendGridasync(message);
        }

        private async Task ConfigSendGridasync(IdentityMessage message)
        {
            string html = StringContainer.MessageMaker(message.Body);

            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new MailAddress("WoW@example.com", "WoW Team");
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = html;

            var credentials = new NetworkCredential
                (
                    ConfigurationSettings.AppSettings["emailService:Account"],
                    ConfigurationSettings.AppSettings["emailService:Password"]
                );

            var transportWeb = new Web(credentials);

            if (transportWeb == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                await transportWeb.DeliverAsync(myMessage);
                await Task.FromResult(0);
            }
        }
    }
}