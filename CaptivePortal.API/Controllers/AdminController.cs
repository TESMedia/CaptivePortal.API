using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CaptivePortal.API.Controllers
{
    public class AdminController : Controller
    {
        CPDBContext db = new CPDBContext();

        //private AdminManagementDbContext db = new AdminManagementDbContext();

        string retString = "-1";
        [HttpPost]
        [Route("GAlogin")]
        public ActionResult GALogin(AdminLoginViewModel admin)
        {
            try
            {
                if (!string.IsNullOrEmpty(admin.UserName) && !string.IsNullOrEmpty(admin.Password))
                {
                    Users user = db.Users.Where(m => m.UserName == admin.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        retString = Convert.ToString(user);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index", "Admin");
        }

        // GET: AdminManagement
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult FormLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OnFormLoginSubmit(FormCollection form)
        {
            Site result = db.Site.First();
           
            Form objForm = new Form
            {
                FormName = form["formName"],
                SiteId= result.SiteId
            };
            db.Form.Add(objForm);
            var res=db.SaveChanges();
            var id = objForm.FormId;

            FormControl objFormControl = new FormControl
            {
                FormId = id,
                ControlType = form["controlType"],
                LabelName=form["labelName"],
                SiteUrl=form["siteUrl"]
            };
            db.FormControl.Add(objFormControl);
            db.SaveChanges();
            return Content("hi");
        }

        public ActionResult CreateNewSite()
        {
            return View();
        }

        public ActionResult ConfigureSite()
        {
            return View();
        }

        // GET: AdminIndex
        public ActionResult Index()
        {
            return View();
        }
    }
}