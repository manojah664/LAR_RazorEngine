using System;
using System.Collections.Generic;
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
            using (UserDBEntities1 db = new UserDBEntities1())
            {
                var v = db.tblUses.Where(a => a.Email == login.Email).FirstOrDefault();
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


            using (UserDBEntities1 db = new UserDBEntities1())
            {

                var v = db.tblUses.Where(a => a.Email == forgot.Email).FirstOrDefault();
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
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] tblUse tblUse)
        {
            bool Status = false;
            string message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                //Email already exist



                var IsExist = IsEmailExist(tblUse.Email);
                if (IsExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already Exist");
                    return View(tblUse);
                }

                //Activation code
                tblUse.ActivationCode = Guid.NewGuid();

                //Password hash
                tblUse.Password = Crypto.Hash(tblUse.Password);

                tblUse.IsEmailVerified = true;

                //save data in db
                using (UserDBEntities1 db = new UserDBEntities1())
                {
                    db.tblUses.Add(tblUse);
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

            return View(tblUse);
        }



        [NonAction]
        public bool IsEmailExist(string Email)
        {
            using (UserDBEntities1 db = new UserDBEntities1())
            {
                var v = db.tblUses.Where(a => a.Email == Email).FirstOrDefault();
                return v != null;
            }

        }


    }
}
