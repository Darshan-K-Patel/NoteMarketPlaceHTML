using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Web.Security;
using System.IO;
using System.Data.Entity.Validation;
using Trails.Models;

namespace Trails.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        NotesEntities6 objcon = new NotesEntities6();
        [HttpGet]
        public ActionResult SignUpMethod()
        {
            return View();
        }

        [HttpPost]

        public ActionResult SignUpMethod(Users objUsr)
        {

            objUsr.EmailVarification = false;
            var IsExists = IsEmailExists(objUsr.Email);
            if (IsExists)
            {
                ModelState.AddModelError("EmailExists", "Email Already Exists");
                return View();
            }

            objUsr.ActivationCode = Guid.NewGuid();

            objUsr.Password = Trails.Models.encryptPassword.textToEncrypt(objUsr.Password);
            objcon.Users.Add(objUsr);
            objcon.SaveChanges();
            SendEmailToUser(objUsr.Email, objUsr.ActivationCode.ToString());
            var Message = "Registration Completed. Please Check Your Email :" + objUsr.Email;
            ViewBag.Message = Message;
            return View("Email_Verification");
        }

        public bool IsEmailExists(string eMail)
        {
            var IsCheck = objcon.Users.Where(email => email.Email == eMail).FirstOrDefault();
            return IsCheck != null;
        }

        public void SendEmailToUser(string emailId, string activationCode)
        {
            var GenarateUserVerificationLink = "/Register/UserVerification/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("dp735732@gmail.com", "Darshan");
            var fromEmailpassword = "dkp@123456";
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Registration Completed-Demo";
            Message.Body = "<br/> Your registration completed succesfully." +
                           "<br/> please click on the below link for account verification" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }

        public ActionResult UserVerification(string id)
        {
            bool Status = false;

            objcon.Configuration.ValidateOnSaveEnabled = false; // Ignor to password confirmation     
            var IsVerify = objcon.Users.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();

            if (IsVerify != null)
            {
                IsVerify.EmailVarification = true;
                objcon.SaveChanges();
                ViewBag.Message = "Email Verification completed";
                Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid Request...Email not verify";
                ViewBag.Status = false;
            }

            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin LgnUsr)
        {
            var _passWord = Trails.Models.encryptPassword.textToEncrypt(LgnUsr.Password);
            bool Isvalid = objcon.Users.Any(x => x.Email == LgnUsr.Email &&
            x.Password == _passWord);
            if (Isvalid)
            {
                int timeout = LgnUsr.Rememberme ? 60 : 5; // Timeout in minutes, 60 = 1 hour.    
                var ticket = new FormsAuthenticationTicket(LgnUsr.Email, false, timeout);
                string encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "UserDash");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Information... Please try again!");
            }
            return View();
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Register");
        }



        [HttpGet]
        [Route("ForgotPassword")]

        public ActionResult ForgotPassWord()
        {
            Forgotpassword obj = new Forgotpassword();
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]


        public ActionResult ForgotPassWord(Forgotpassword xyz)
        {
            if (objcon.Users.Any(model => model.Email == xyz.Email))
            {
                ForgotPassSentEmail(xyz);
                return View();

            }
            else
            {
                ModelState.AddModelError("Error", "Email Id does not exists");
                return View();
            }


        }

        private void ForgotPassSentEmail(Forgotpassword xyz)
        {
            var check = objcon.Users.Where(x => x.Email == xyz.Email).FirstOrDefault();
            using (MailMessage mm = new MailMessage("dp735732@gmail.com", xyz.Email))
            {
                mm.Subject = "NoteMarketPlace - Temporary Password";

                var body = "<p>Hello,</p> <p>Your newly generated password is:<p> <p>{0}</p> <p>Thanks,</p><p>Team Notes MarketPlace</p>";
                string NewPassword = GeneratePassword().ToString();

                body = string.Format(body, NewPassword);
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;

                NetworkCredential Network = new NetworkCredential("dp735732@gmail.com", "dkp@123456");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = Network;
                smtp.Port = 587;
                smtp.Send(mm);

                if (NewPassword != null)
                {
                    var Replace = objcon.Users.Where(x => x.Email == xyz.Email).FirstOrDefault();
                    if (Replace != null)
                    {
                        Replace.Password = NewPassword;
                        Replace.Password = Trails.Models.encryptPassword.textToEncrypt(Replace.Password);

                        objcon.SaveChanges();


                    }


                }

            }
        }

        public string GeneratePassword()
        {
            string PassLength = "6";
            string NewPass = "";

            String AllowChar = "";

            AllowChar = "1,2,3,4,5,6,7,8,9,0";

            char[] Seperated = { ',' };

            string[] arr = AllowChar.Split(Seperated);

            string IDString = "";
            string Temp = "";

            Random Rand = new Random();

            for (int i = 0; i < Convert.ToInt32(PassLength); i++)
            {
                Temp = arr[Rand.Next(0, arr.Length)];
                IDString += Temp;
                NewPass = IDString;
            }
            return NewPass;
        }




    }
}