using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trails.Models;


namespace Trails.Controllers
{
    public class UserProfileController : Controller
    {

        NotesEntities6 dbobj = new NotesEntities6();
        // GET: UserProfile
        [Authorize]
        [HttpGet]
        [Route("UserProfile")]
        public ActionResult UserProfile()
        {
            var user = dbobj.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            var userprofile = dbobj.UserProfile.Where(x => x.UserID == user.UserId).FirstOrDefault();

            UserProfileModel userprofilemodel = new UserProfileModel();

            if (userprofile != null)
            {
                userprofilemodel.CountryList = dbobj.Countries.Where(x => x.IsActive == true).ToList();
                userprofilemodel.GenderList = dbobj.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true).ToList();
                userprofilemodel.USerID = user.UserId;
                userprofilemodel.Email = user.Email;
                userprofilemodel.FirstName = user.FirstName;
                userprofilemodel.LastName = user.LastName;
                userprofilemodel.DOB = userprofile.DOB;
                userprofilemodel.CountryCode = userprofile.CountryCode;
                userprofilemodel.PhoneNumber = userprofile.PhoneNumber;
                userprofilemodel.Gender = userprofile.Gender;
                userprofilemodel.Address_Line_1 = userprofile.AddressLine1;
                userprofilemodel.Address_Line_2 = userprofile.AddressLine2;
                userprofilemodel.City = userprofile.City;
                userprofilemodel.State = userprofile.State;
                userprofilemodel.Zip_Code = userprofile.ZipCode;
                userprofilemodel.Country = userprofile.Country;
                userprofilemodel.University = userprofile.University;
                userprofilemodel.College = userprofile.College;
                userprofilemodel.profileUrl = userprofile.ProfilePicture;

            }


            else
            {
                userprofilemodel.CountryList = dbobj.Countries.Where(x => x.IsActive == true).ToList();
                userprofilemodel.GenderList = dbobj.ReferenceData.Where(x => x.RefCategory == "gender" && x.IsActive == true).ToList();
                userprofilemodel.USerID = user.UserId;
                userprofilemodel.Email = user.Email;
                userprofilemodel.FirstName = user.FirstName;
                userprofilemodel.LastName = user.LastName;
            }

            return View(userprofilemodel);

        }

        [HttpPost]
        [Route("UserProfile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileModel userprofilemodel)
        {

            var user = dbobj.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {

                if (userprofilemodel.ProfilePicture != null)
                {
                    var PicSize = userprofilemodel.ProfilePicture.ContentLength;
                    if (PicSize > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Profile_Picture", "Image size is 10MB");

                        userprofilemodel.CountryList = dbobj.Countries.Where(x => x.IsActive == true).ToList();
                        userprofilemodel.GenderList = dbobj.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true).ToList();
                        return View(userprofilemodel);
                    }
                }


                var profile = dbobj.UserProfile.Where(x => x.UserID == user.UserId).FirstOrDefault();

                //edit profile

                if (profile != null)
                {
                    profile.DOB = userprofilemodel.DOB;
                    profile.Gender = userprofilemodel.Gender;
                    profile.CountryCode = userprofilemodel.CountryCode.Trim();
                    profile.PhoneNumber = userprofilemodel.PhoneNumber.Trim();
                    profile.AddressLine1 = userprofilemodel.Address_Line_1.Trim();
                    profile.AddressLine2 = userprofilemodel.Address_Line_2.Trim();
                    profile.City = userprofilemodel.City.Trim();
                    profile.State = userprofilemodel.State.Trim();
                    profile.ZipCode = userprofilemodel.Zip_Code.Trim();
                    profile.Country = userprofilemodel.Country.Trim();
                    profile.University = userprofilemodel.University.Trim();
                    profile.College = userprofilemodel.College.Trim();

                    //delete old profile pic
                    if (userprofilemodel.ProfilePicture != null && profile.ProfilePicture != null)
                    {
                        string path = Server.MapPath(profile.ProfilePicture);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // if user upload profile picture then save it and store path in database

                    if (userprofilemodel.ProfilePicture != null)
                    {



                        string filename = Path.GetFileName(userprofilemodel.ProfilePicture.FileName);
                        string fileextension = Path.GetExtension(userprofilemodel.ProfilePicture.FileName);
                        string newfilename = "DP_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + user.UserId + "/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        profile.ProfilePicture = profilepicturepath + newfilename;
                        userprofilemodel.ProfilePicture.SaveAs(path);
                    }

                    dbobj.Entry(profile).State = EntityState.Modified;
                    dbobj.SaveChanges();

                    user.FirstName = userprofilemodel.FirstName.Trim();
                    user.LastName = userprofilemodel.LastName.Trim();
                    dbobj.Entry(user).State = EntityState.Modified;
                    dbobj.SaveChanges();
                }

                // new userprofile

                else
                {
                    UserProfile userProfile = new UserProfile();

                    userProfile.UserID = user.UserId;
                    userProfile.SecondaryEmailAddress = user.Email;
                    userProfile.DOB = userprofilemodel.DOB;
                    userProfile.Gender = userprofilemodel.Gender;
                    userProfile.CountryCode = userprofilemodel.CountryCode.Trim();
                    userProfile.PhoneNumber = userprofilemodel.PhoneNumber;
                    userProfile.AddressLine1 = userprofilemodel.Address_Line_1;
                    userProfile.AddressLine2 = userprofilemodel.Address_Line_2;
                    userProfile.City = userprofilemodel.City;
                    userProfile.State = userprofilemodel.State;
                    userProfile.ZipCode = userprofilemodel.Zip_Code;
                    userProfile.Country = userprofilemodel.Country;
                    userProfile.University = userprofilemodel.University;
                    userProfile.College = userprofilemodel.College;

                    if (userprofilemodel.ProfilePicture != null)
                    {


                        string filename = Path.GetFileName(userprofilemodel.ProfilePicture.FileName);
                        string fileextension = Path.GetExtension(userprofilemodel.ProfilePicture.FileName);
                        string newfilename = "DP_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + user.UserId + "/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        userProfile.ProfilePicture = profilepicturepath + newfilename;
                        userprofilemodel.ProfilePicture.SaveAs(path);
                    }

                    dbobj.UserProfile.Add(userProfile);
                    dbobj.SaveChanges();

                    user.FirstName = userprofilemodel.FirstName.Trim();
                    user.LastName = userprofilemodel.LastName.Trim();
                    dbobj.Entry(user).State = EntityState.Modified;
                    dbobj.SaveChanges();
                }

                return RedirectToAction("SearchNote", "SearchNote");

            }

            // for invalid ModelState

            else
            {
                userprofilemodel.CountryList = dbobj.Countries.Where(x => x.IsActive == true).ToList();
                userprofilemodel.GenderList = dbobj.ReferenceData.Where(x => x.RefCategory == "Gender" && x.IsActive == true).ToList();
                return View(userprofilemodel);
            }

        }

        private void CreateDirectoryIfMissing(string folderpath)
        {
            // check if directory exists
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            // if directory does not exists then create
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }
    }
}