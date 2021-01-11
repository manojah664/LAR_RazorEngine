using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using LogAndReg.Models;


namespace LogAndReg.Controllers
{
    public class UserController : Controller
    {
      
        // Get: User Registration
        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login)
        {
            var message = "";
            bool status = false;
            using (UserDbEntities db = new UserDbEntities())
            {
                var v = db.Uses.Where(a => a.Email == login.Email).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        message = "Login Success";
                        status = true;
                    }
                    else if (v.Password == "")
                    {

                        message = "Please enter a valid Password";
                        status = true;

                    }
                    else
                    {
                        message = "Please enter a valid Password";
                        status = false;
                    }
                }
                else
                {
                    message = "Please enter a valid Email and Password";

                }
            }
            ViewBag.Message = message;
            ViewBag.status = status;
            return View();
        }
        

        public ActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forgot(Forgot forgot)
        {
            string message = "";
            bool status = false;


            using (UserDbEntities db = new UserDbEntities())
            {

                var v = db.Uses.Where(a => a.Email == forgot.Email).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(forgot.NewPassword, forgot.ConfirmPassword) == 0)
                    {
                        var m = Crypto.Hash(forgot.NewPassword);
                        v.Password = m;
                        db.SaveChanges();
                        message = "Successfully Completed";
                        status = true;
                    }
                    else
                    {
                        message = "enter a right password";
                        status = false;
                    }
                }
                else
                {
                    message = "email is invalid";
                    status = false;
                }

            }

                ViewBag.Message = message;
                ViewBag.Status = status;
                return View();

            
        }


       


        [HttpGet]
        public ActionResult Registration()
        {
            UserViewModel use = new UserViewModel();
            using (UserDbEntities db = new UserDbEntities())
            {
             
                use.CountryList = db.Countries.Select(e => new SelectListItem { Text = e.Cname.ToString(), Value = e.Countryid.ToString()}).ToList();
            }
            return View(use);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")]UserViewModel userViewModel )
        {
            bool Status = false;
            string message = "";

            //Model Validation
            try
            {
                if (ModelState.IsValid)
                {
                    //Email already exist



                    var IsExist = IsEmailExist(userViewModel.Email);
                    if (IsExist)
                    {
                        ModelState.AddModelError("EmailExist", "Email already Exist");
                        return View(userViewModel);
                    }

                    //Activation code
                    userViewModel.ActivationCode = Guid.NewGuid();

                    //Password hash
                    userViewModel.Password = Crypto.Hash(userViewModel.Password);

                    userViewModel.IsEmailVerified = true;



                    //save data in db
                    using (UserDbEntities db = new UserDbEntities())
                    {

                        //var Country = db.Countries.ToList();

                        //var CountryList = new SelectList(db.Countries, "CountryId", "Cname");

                        Use use = new Use();
                        use.Countryid = userViewModel.Countryid;
                        use.Address = userViewModel.Address;
                        use.ActivationCode = userViewModel.ActivationCode;
                        use.Email = userViewModel.Email;
                        use.DateOfBirth = userViewModel.DateOfBirth;
                        use.Gender = userViewModel.Gender;
                        use.IsEmailVerified = userViewModel.IsEmailVerified;
                        use.Password = userViewModel.Password;
                        use.StateId = userViewModel.StateId;
                        use.MobileNumber = userViewModel.MobileNumber;
                        use.Username = userViewModel.Username;


                        db.Uses.Add(use);
                        db.SaveChanges();

                    }
                    Status = true;
                    message = "Registration Completed";

                }
                else
                {
                    message = "Invalid Request";
                }
                ViewBag.Message = message;
                ViewBag.Status = Status;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return View(userViewModel);
        }



        [NonAction]
        public bool IsEmailExist(string Email)
        {
            using (UserDbEntities db = new UserDbEntities())
            {
                var v = db.Uses.Where(a => a.Email == Email).FirstOrDefault();
                return v != null;
            }

        }

        [HttpGet]
        public JsonResult GetStateName(int? Countryid)
        {
            UserDbEntities db = new UserDbEntities();
            var result = db.States.Where(e => e.Countryid == Countryid).Select(e => new SelectListItem { Text = e.Sname, Value = e.StateId.ToString()}).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }






        //[HttpGet]
        //public JsonResult GetCityName(int? StateId)
        //{
        //    UserDbEntities db = new UserDbEntities();
        //    var result = db.Cities.Where(e => e.StateId == StateId).Select(e => new SelectListItem { Text = e.Cityname, Value = e.CityId.ToString() }).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}


    }
}
