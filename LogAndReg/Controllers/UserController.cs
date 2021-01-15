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

                use.CountryList = db.Countries.Select(e => new SelectListItem { Text = e.Cname.ToString(), Value = e.Countryid.ToString() }).ToList();
            }
            return View(use);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")]UserViewModel userViewModel)
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
                        return View("Valid");
                        // return View(userViewModel);
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
                        use.CityId = userViewModel.CityId;


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

            return View();
        }

        public ActionResult Valid()
        {
            return View();
        }



        public ActionResult LogAndReg()
        {
            UserDbEntities db = new UserDbEntities();
            return View(db.Uses.ToList());

        }

        public ActionResult ViewDatas()
        {
            UserDbEntities db = new UserDbEntities();
            return View(db.Uses.ToList());
        }

        public ActionResult EditData()
        {

            UserDbEntities db = new UserDbEntities();
            return View(db.Uses.ToList());

        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            UserDbEntities db = new UserDbEntities();
            var model = db.Uses.Find(id);

            return View(model);
        }

        public ActionResult Edit(Use use)
        {
            try
            {
                UserDbEntities db = new UserDbEntities();
                db.Entry(use).State = System.Data.Entity.EntityState.Modified;
                use.Password = Crypto.Hash(use.Password);
                db.SaveChanges();
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
            return RedirectToAction("EditData");
        }

        public ActionResult Delete(int id)
        {
            UserDbEntities db = new UserDbEntities();
            var model = db.Uses.Find(id);
            db.Uses.Remove(model);
            db.SaveChanges();
            return RedirectToAction("EditData");

        }


        public ActionResult AdminReg()
        {
            return View();
        }

        public ActionResult AdminLog()
        {
            return View();

        }

           [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLog(Login login)
        {
            var message = "";
            bool status = false;
            using (UserDbEntities db = new UserDbEntities())
            {
                var v = db.Admins.Where(a => a.Email == login.Email).FirstOrDefault();
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

        public ActionResult AdminLogout()
        {
            return RedirectToAction("AdminLog");
        }

        public ActionResult AdminForgot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminForgot(Forgot forgot)
        {

            string message = "";
            bool status = false;


            using (UserDbEntities db = new UserDbEntities())
            {

                var v = db.Admins.Where(a => a.Email == forgot.Email).FirstOrDefault();
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminReg(AdminViewModel adminViewModel)
        {

            bool Status = false;
            string message = "";

            //Model Validation
            try
            {


                if (ModelState.IsValid)
                {
                    //Email already exist



                    var IsExist = IsAdminEmailExist(adminViewModel.Email);
                    if (IsExist)
                    {
                        ModelState.AddModelError("EmailExist", "Email already Exist");
                        return View("Valid");
                        // return View(userViewModel);
                    }

                    adminViewModel.Password = Crypto.Hash(adminViewModel.Password);

                    using (UserDbEntities db = new UserDbEntities())
                    {
                        Admin admin = new Admin();
                        admin.Email = adminViewModel.Email;
                        admin.Password = adminViewModel.Password;


                        db.Admins.Add(admin);
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
            return View();
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

        [NonAction]
        public bool IsAdminEmailExist(string Email)
        {
            using (UserDbEntities db = new UserDbEntities())
            {
                var v = db.Admins.Where(a => a.Email == Email).FirstOrDefault();
                return v != null;
            }

        }

        [HttpGet]
        public JsonResult GetStateName(int? Countryid)
        {
            UserDbEntities db = new UserDbEntities();
            var result = db.States.Where(e => e.Countryid == Countryid).Select(e => new SelectListItem { Text = e.Sname, Value = e.StateId.ToString() }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCityName(int? StateId)
        {
            UserDbEntities db = new UserDbEntities();
            var result = db.Cities.Where(e => e.StateId == StateId).Select(e => new SelectListItem { Text = e.Cityname, Value = e.CityId.ToString() }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
