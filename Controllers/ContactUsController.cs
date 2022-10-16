using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;

namespace UmbracoProject.Controllers
{
    public class ContactUsController : Umbraco.Cms.Web.Website.Controllers.SurfaceController
    {
        public ContactUsController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
        }
        [HttpPost]
        public async Task<IActionResult> Save(Contact contact)
        {
            bool success = true;
            string message_JsonResult = "Data has been saved successfully.";
            try
            {
                if (contact.Email != "")
                {
                    var from = "umbracositedev@gmail.com";
                    var to = "sales@roofstiles.co.uk";
                    SendGridClient client = new SendGridClient("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");
                    var msgtosales = new SendGridMessage()
                    {
                        From = new EmailAddress(from, "Roof Tiles"),
                        Subject = contact.Subject,
                        PlainTextContent = "",
                        HtmlContent = "Name: " + contact.FirstName + " " + contact.LastName + "<br/>Message: " + contact.Message
                    };
                    msgtosales.AddTo(new EmailAddress(to, ""));
                    var response = await client.SendEmailAsync(msgtosales).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var msgtouser = new SendGridMessage()
                        {
                            From = new EmailAddress(from, "Roof Tiles"),
                            Subject = "Contact us",
                            PlainTextContent = "Thank you for Contacting us.",
                            HtmlContent = ""
                        };
                        msgtouser.AddTo(new EmailAddress(contact.Email, ""));
                        response = await client.SendEmailAsync(msgtouser).ConfigureAwait(false);
                    }

                }
            }
            catch (Exception ex)
            {
                message_JsonResult = ex.Message;
            }
            return Json(new
            {
                Success = success,
                Message = message_JsonResult,
            });
        }
    }

    public class Contact

    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
