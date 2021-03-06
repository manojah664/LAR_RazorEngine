﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using LogAndReg.Models;
using Syncfusion.XlsIO;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;

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
        // [ValidateAntiForgeryToken]
        public ActionResult Index(NewView newView)
        {
            string message = "";
            bool status = false;
            if (!ModelState.IsValid)
            {
                return View();
            }

            using (UserDBEntities db = new UserDBEntities())

            {
                var s = db.Uses.Where(a => a.Username == newView.UserName).FirstOrDefault();
                if (s != null)
                {


                    if (newView.UserName.Trim() == "Admin")
                    {
                        message = "User login Success";
                        status = true;
                        ViewBag.Message = message;
                        ViewBag.Status = status;
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

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
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


        public ActionResult Logic()
        {
            using (UserDBEntities userDB = new UserDBEntities())
            {

                var innerJoin = (from e in db.Uses
                                 join d in db.Countries on e.Countryid equals d.Countryid
                                 join f in db.States on e.StateId equals f.StateId
                                 join h in db.Cities on f.StateId equals h.StateId


                                 select new User
                                 {
                                     Username = e.Username,
                                     Email = e.Email.ToString(),
                                     Uid = e.Uid,
                                     DateOfBirth = e.DateOfBirth.Value,
                                     MobileNumber = e.MobileNumber.ToString(),
                                     Address = e.Address.ToString(),
                                     Gender = e.Gender.ToString(),
                                     Cname = d.Cname,
                                     Sname = f.Sname,
                                     Cityname = h.Cityname,
                                     IsActive = e.IsActive.Value


                                 }).ToList();
                return View(innerJoin);
            }




            //  return View(userDB.Uses.ToList());

        }

        public ActionResult LogAndReg()
        {
            string message = "Login Successfully";
            bool status = true;

            ViewBag.Message = message;
            ViewBag.Status = status;

            return View();

        }

        public ActionResult UserView()
        {
            UserDBEntities db = new UserDBEntities();
            return View(db.Uses.ToList());
        }


        public ActionResult ViewDatas()
        {
            var UserName = Session["UserName"].ToString();
            UserDBEntities db = new UserDBEntities();
            var v = db.Uses.Where(a => a.Username == UserName).Select(a => new User

            {
                Username = a.Username,
                Email = a.Email,
                Uid = a.Uid,
                Password = a.Password,
                DateOfBirth = a.DateOfBirth.Value,
                MobileNumber = a.MobileNumber,
                Address = a.Address,
                Gender = a.Gender,
                IsEmailVerified = a.IsEmailVerified,
                ActivationCode = a.ActivationCode,
                Countryid = a.Countryid,
                StateId = a.StateId,
                CityId = a.CityId


            }).FirstOrDefault();

            return View(v);
        }

        [HttpPost]
        public ActionResult ViewDatas(User user)
        {
            bool status = false;
            string message = "";

            try
            {
                if (!ModelState.IsValid)
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
                    message = "Updateds Are Saved";
                    status = true;

                }
                else
                {
                    message = "Invalid Request";
                }
                ViewBag.Message = message;
                ViewBag.Status = status;
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


            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();



        }

        public ActionResult EditData()
        {
            UserDBEntities db = new UserDBEntities();
            return View(db.Uses.ToList());
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


        public ActionResult UpdateStatusDetails(int id, bool Status)
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


        UserDBEntities db = new UserDBEntities();
        private object filename;
        private object itfunda;

        // GET: ExportData
        public ActionResult ExportToExcel()
        {
            UserDBEntities db = new UserDBEntities();
            var data = db.Uses.ToList().Select(p => new
            {
                p.Uid,
                p.Username,
                p.Email,
                p.MobileNumber,
                p.Address,
                p.Gender,
                p.DateOfBirth,
                p.IsEmailVerified,
                p.Countryid,
                p.StateId,
                p.CityId,
                p.IsActive

            });

            // instantiate the GridView control from System.Web.UI.WebControls namespace
            // set the data source
            GridView gridview = new GridView();
            gridview.DataSource = data;
            gridview.DataBind();

            // Clear all the content from the current response
            Response.ClearContent();
            Response.Buffer = true;
            // set the header
            Response.AddHeader("content-disposition", "attachment;filename = itfunda.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            // create HtmlTextWriter object with StringWriter
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // render the GridView to the HtmlTextWriter
                    gridview.RenderControl(htw);
                    // Output the GridView content saved into StringWriter
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            return View();
        }

        public ActionResult ContactForm()
        {
            return View();

        }

        [HttpPost]
        public ActionResult ContactForm(MemberModels ImageFiles, HttpPostedFileBase ImageFile)
        {
            UserDBEntities user = new UserDBEntities();


            //string fileName = Path.GetFileName(member.ImagePath);


            string FileName = Path.GetFileName(ImageFile.FileName);
            // string path = Path.Combine(Server.MapPath("~/Models/Extended"), FileName);

            /// string file = MimeMapping.GetMimeMapping(FileName);

            string path = "~/MyGallery/"+FileName;
            
            ImageFile.SaveAs(Server.MapPath(path));

            var a = ImageFiles.Name;
            var b = ImageFiles.PhoneNumber;
            var c = path;
            var d = FileName;


            Upload u = new Upload();
            u.MemberName = a;
            u.PhoneNumber = b;
            u.ImagePath = c;
            u.ImageName = d;
            user.Uploads.Add(u);
          
            user.SaveChanges();
            Session["MemberId"] = u.MemberId;
            ViewBag.FullName = Session["MemberId"];

            return RedirectToAction("ContactForm");
        }

        public ActionResult GetData( )

        {
            UserDBEntities user = new UserDBEntities();
            var data = user.Uploads.ToList();
            return View(data);
        }
        
        public JsonResult GetPop(int id,string ImageName)
        {

            string result = "Fail";
            UserDBEntities user = new UserDBEntities();
            Upload upload = new Upload();

            var model = db.Uploads.Find(id);
            model.ModifiedName = ImageName;

            if (model.ModifiedName!=null)
            {
                model.IsModified = true;
                upload = model;
                user.Entry(upload).State = System.Data.Entity.EntityState.Modified;
                user.SaveChanges();
                result = "successfully Modified";

            }


            //if (upload.MemberId == id)
            //{
            //    var data = user.Uploads.FirstOrDefault();
            //    return View(data);
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
          

        }


        public JsonResult Modify(string imageName)
        {
            string result = "Fail";

            UserDBEntities userDB = new UserDBEntities();

            // Upload upload = new Upload();
           Upload upload = new Upload();
            var data = userDB.Uploads.FirstOrDefault();
            data.ModifiedName = imageName;
            if(data.ModifiedName!=null)
            {
                data.IsModified = true;
                upload = data;
                userDB.Entry(upload).State = System.Data.Entity.EntityState.Modified;
                userDB.SaveChanges();
                
            }

           
            //userDB.SaveChanges();
            result = "success";
          
            

            return Json(result, JsonRequestBehavior.AllowGet);


        }

          

     



        public ActionResult Cancel()
        {
            return RedirectToAction("ContactForm");
        }


        public ActionResult File()
        {
            return View();
        }


        [HttpPost]

        public ActionResult File(HttpPostedFileBase file_Uploader)

        {

            if (file_Uploader != null)

            {

                string fileName = string.Empty;

                string destinationPath = string.Empty;



                List<MemberModel> uploadFileModel = new List<MemberModel>();



                fileName = Path.GetFileName(file_Uploader.FileName);

                destinationPath = Path.Combine(Server.MapPath("~/Models/Image/"), fileName);

                file_Uploader.SaveAs(destinationPath);

                if (Session["fileUploader"] != null)

                {

                    var isFileNameRepete = ((List<MemberModel>)Session["fileUploader"]).Find(x => x.FileName == fileName);

                    if (isFileNameRepete == null)

                    {

                        uploadFileModel.Add(new MemberModel { FileName = fileName, FilePath = destinationPath });

                        ((List<MemberModel>)Session["fileUploader"]).Add(new MemberModel { FileName = fileName, FilePath = destinationPath });

                        ViewBag.Message = "File Uploaded Successfully";

                    }

                    else

                    {

                        ViewBag.Message = "File is already exists";

                    }

                }

                else

                {

                    uploadFileModel.Add(new MemberModel { FileName = fileName, FilePath = destinationPath });

                    Session["fileUploader"] = uploadFileModel;

                    Session["FileName"] = fileName;

                    Session["FilePath"] = destinationPath;

                    ViewBag.FileName = fileName;

                    ViewBag.FilePath = destinationPath;

                    ViewBag.Message = "File Uploaded Successfully";

                }

            }

            return View();

        }


       

     


    }



}
//public ActionResult Image()
//{

//    return View();

//}


//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult Image(HttpPostedFileBase file)
//{
//    Gallery gallery = new Gallery();

//    if (file != null)
//    {
//        string fileName = Path.GetFileName(file.FileName);
//        string path = Path.Combine(Server.MapPath("~/Models/Image"), fileName);


//        fileName = DateTime.Now.ToString("yyyyMMdd") + "-" + fileName.Trim() + path;

//        UserDBEntities userDB = new UserDBEntities();
//        userDB.Galleries.Add(new Gallery
//        {

//        });
//        userDB.SaveChanges();




//    }

//    return RedirectToAction("Index");
//}


