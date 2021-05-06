using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trails.Models;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web.Security;

namespace Trails.Controllers
{
    public class ContactUsController : Controller
    {



        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ContactUs(ContactUs xyz)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {1}</p><p>Hello,</p><p>{2}</p><br><p>Regards,</p><p>{0}</p>";
                var Message = new MailMessage();
                Message.To.Add(new MailAddress("dp735732@gmail.com"));

                Message.From = new MailAddress(xyz.EmailID);
                Message.Subject = xyz.Subject;
                Message.Body = string.Format(body, xyz.FullName, xyz.EmailID, xyz.Comments);
                Message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "dp735732@gmail.com",
                        Password = "dkp@123456"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(Message);
                }

            }
            return View();

        }
    }
}