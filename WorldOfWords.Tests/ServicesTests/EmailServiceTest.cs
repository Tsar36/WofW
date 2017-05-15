using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.Infrastructure.Data.EF.UnitOfWork;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using SendGrid;
using WorldOfWords.Domain.Services.Messages;
using WorldOfWords.Domain.Services.Services;
using System.Configuration;


// made by Fominov Mykhaylo
// reviewed by Andrii Huchko
namespace WorldOfWords.Tests.ServicesTests
{
    [TestFixture]
    public class EmailServiceTest //: IIdentityMessageService
    {
        [Test]
        public async Task EmailServiceTestMethod()
        {
            IdentityMessage messageIdentity = new IdentityMessage()
            {
                Body = null,
                Destination = "some destination",
                Subject = "some subject"
            };
            EmailService eMail = new EmailService();
            var actual = eMail.SendAsync(messageIdentity);
            

            Assert.IsInstanceOf<Task>(actual);
        }
    }
}
