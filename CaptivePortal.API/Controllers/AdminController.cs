using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CaptivePortal.API.Controllers
{
    public class AdminController : Controller
    {
        CPDBContext db = new CPDBContext();
        string ConnectionString = ConfigurationManager.ConnectionStrings["CPDBContext"].ConnectionString;

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
        [HttpPost]
        public ActionResult CreateSiteAndLoginRegisterConf(FormViewModel inputData,FormCollection fc)
        {
            try
            {
                string imagepath = null;
                Organisation objOrganisation = new Organisation
                {
                    OrganisationName = inputData.OrganisationName
                };
                db.Organisation.Add(objOrganisation);
                db.SaveChanges();
                var orgId = objOrganisation.OrganisationId;

                Company objCompany = new Company
                {
                    CompanyName = inputData.CompanyName,
                    OrganisationId = orgId,
                };
                db.Company.Add(objCompany);
                db.SaveChanges();
                var compId = objCompany.CompanyId;

                Site objSite = new Site
                {
                    SiteName = inputData.SiteName,
                    CompanyId = compId
                };
                db.Site.Add(objSite);
                db.SaveChanges();

                if (Request.Files["BannerIcon"].ContentLength > 0)
                {
                    var httpPostedFile = Request.Files["BannerIcon"];
                    string savedPath= HostingEnvironment.MapPath("/Images/" + objSite.SiteId);
                    imagepath = "/Images/" + objSite.SiteId + "/" + httpPostedFile.FileName;
                   string completePath = Path.Combine(savedPath, httpPostedFile.FileName);

                    if (!System.IO.Directory.Exists(savedPath))
                    {
                        Directory.CreateDirectory(savedPath);
                    }
                    httpPostedFile.SaveAs(completePath);
                    inputData.BannerIcon = "/Images/" + httpPostedFile.FileName;
                }
                Form objForm = new Form
                {
                    SiteId = objSite.SiteId,
                    BannerIcon = imagepath,
                    BackGroundColor = inputData.BackGroundColor,
                    LoginWindowColor = inputData.LoginWindowColor,
                    IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
                    LoginPageTitle = inputData.LoginPageTitle,
                    RegistrationPageTitle = inputData.RegistrationPageTitle,
                };
                db.Form.Add(objForm);
                db.SaveChanges();
                return Content("Configured successfully!");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFile(FormCollection fc)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase file = Request.Files[0];
                    string fname;

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }

                    // Get the complete folder path and store the file inside it.  
                    fname = Path.Combine(Server.MapPath("~/Images/" + fc["fanSpaceAppId"]));
                    string completePath = fname + "\\" + file.FileName;
                    CheckDirectory(fname);
                    file.SaveAs(completePath);

                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public void CheckDirectory(string folderPath)
        {
            if (!System.IO.Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

       
        [HttpPost]
        public ActionResult AddField(string inputField,string inputType)
        {
            string sqlString = "alter table [Users] add" + " " + inputField + " " + inputType + " " + "NULL";
            db.Database.ExecuteSqlCommand(sqlString);
            return Content("Field Added!!");
        }

        [HttpGet]
        public void GetAllData()
        {

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