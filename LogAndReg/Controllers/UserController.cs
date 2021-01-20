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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(NewView newView)
        {
            string message = "";
            bool status = false;
            if(!ModelState.IsValid)
            {
                return View();
            }

            using (UserDBEntities db = new UserDBEntities())
            {
                var s= db.Uses.Where(a => a.Username == newView.UserName).FirstOrDefault();
                if (s!= null)
                {
                    if (newView.UserName.Trim() =="Admin")
                    {
                        message = "User login Success";
                        status = true;
                        ViewBag.Message = message;
                        ViewBag.status = status;
                        return RedirectToAction("LogAndReg");
                    }
                    else
                    {
                        message = "Admin login Success";
                        status = true;
                        ViewBag.Message = message;
                        ViewBag.status = status;
                        Session["UserName"] = newView.UserName.Trim();
                         return RedirectToAction("ViewDatas");
                    }
                }
                else
                {
                    message = "You are not a Valid user please signup";
                    status = false;
                    ViewBag.Message = message;
                    ViewBag.status = status;
                    return View();
                }
            }
           


        }




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
            using (UserDBEntities db = new UserDBEntities())
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

        public ActionResult UserLogin()
        {
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


            using (UserDBEntities db = new UserDBEntities())
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
            using (UserDBEntities db = new UserDBEntities())
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

                    //userViewModel.IsActive = true;

                    //save data in db
                    using (UserDBEntities db = new UserDBEntities())
                    {

                        //var Country = db.Countries.ToList();

                        //var CountryList = new SelectList(db.Countries, "CountryId", "Cname");

                        Use use = new Use();
                        use.Countryid =userViewModel.Countryid;
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
                        use.IsActive = userViewModel.IsActive;
                        


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
            UserDBEntities db = new UserDBEntities();
            return View(db.Uses.ToList());

        }

        public ActionResult ViewData()
        {
            UserDBEntities db = new UserDBEntities();
            return View(db.Uses.ToList());
        }


        public ActionResult ViewDatas(string Password)
        {
            var UserName = Session["UserName"].ToString();
            UserDBEntities db = new UserDBEntities();
            var v = db.Uses.Where(a => a.Username == UserName).Select(a => new User
            {
                Username = a.Username,
                Email=a.Email,
                Uid=a.Uid,
                Password=a.Password,
                DateOfBirth=a.DateOfBirth.Value,
                MobileNumber=a.MobileNumber,
                Address=a.Address,
                Gender=a.Gender,
                IsEmailVerified=a.IsEmailVerified,
                ActivationCode =a.ActivationCode,
                Countryid=a.Countryid,
                StateId = a.StateId,
                CityId=a.CityId


            }).FirstOrDefault();
           
            return View(v);
        }

        [HttpPost]
        public ActionResult ViewDatas(User user )
        {
            try
            {
                using (UserDBEntities us = new UserDBEntities())
                {

                    Use use = new Use();
                    use.Uid = user.Uid;
                    use.Address = user.Address;
                    use.Email = user.Email;
                    use.DateOfBirth = user.DateOfBirth;
                    use.Gender = user.Gender;
                    use.Password = user.Password;
                    use.MobileNumber = user.MobileNumber;
                    use.Username = user.Username;
                    use.IsEmailVerified = user.IsEmailVerified;
                    use.ActivationCode = user.ActivationCode;
                    use.StateId = user.StateId;
                    use.Countryid = user.Countryid;
                    use.CityId = user.CityId;
                    use.IsActive = user.IsActive;


                    us.Entry(use).State = System.Data.Entity.EntityState.Modified;
                    us.SaveChanges();
                }
              
              
               
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
            return RedirectToAction("Index");



        }
    



        public ActionResult EditData()
        {
            UserDBEntities db = new UserDBEntities();
            return View(db.Uses.ToList());
        }
        

       

        public ActionResult EditUser(int id)
        {
            UserDBEntities db = new UserDBEntities();
            var model = db.Uses.Find(id);

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            UserDBEntities db = new UserDBEntities();
            var model = db.Uses.Find(id);
            
            return View(model);
        }

        public ActionResult Edit(Use use)
        {
            try
            {
                UserDBEntities db = new UserDBEntities();
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
            UserDBEntities db = new UserDBEntities();
            var model = db.Uses.Find(id);
            //Use use = new Use();
            db.Uses.Remove(model);
            db.SaveChanges();
            return RedirectToAction("EditData");

        }

        [NonAction]
        public bool IsEmailExist(string Email)
        {
            using (UserDBEntities db = new UserDBEntities())
            {
                var v = db.Uses.Where(a => a.Email == Email).FirstOrDefault();
                return v != null;
            }

        }


        [HttpGet]
        public JsonResult GetStateName(int? Countryid)
        {
            UserDBEntities db = new UserDBEntities();
            var result = db.States.Where(e => e.Countryid == Countryid).Select(e => new SelectListItem { Text = e.Sname, Value = e.StateId.ToString() }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }





        [HttpGet]
        public JsonResult GetCityName(int? StateId)
        {
            UserDBEntities db = new UserDBEntities();
            var result = db.Cities.Where(e => e.StateId == StateId).Select(e => new SelectListItem { Text = e.Cityname, Value = e.CityId.ToString() }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult UpdateStatusDetails(int id,bool Status)
        {
            UserDBEntities user = new UserDBEntities();
            var result = user.Uses.Where(e => e.Uid == id).FirstOrDefault();
            Use use = new Use();
            if (Status == true)
                result.IsActive = false;
            else
                result.IsActive = true;
            user.Entry(result).State = System.Data.Entity.EntityState.Modified;
            user.SaveChanges();
            return RedirectToAction("EditData");
        }

    }
}
