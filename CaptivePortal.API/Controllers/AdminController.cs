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



        // GET: AdminIndex
        public ActionResult Index()
        {
            return View();
        }
    }
}