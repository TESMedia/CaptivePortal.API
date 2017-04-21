using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
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
                SiteId = result.SiteId
            };
            db.Form.Add(objForm);
            var res = db.SaveChanges();
            var id = objForm.FormId;

            FormControl objFormControl = new FormControl
            {
                FormId = id,
                ControlType = form["controlType"],
                LabelName = form["labelName"],
                SiteUrl = form["siteUrl"]
            };
            db.FormControl.Add(objFormControl);
            db.SaveChanges();
            return Content("hi");
        }

        public ActionResult CreateNewSite()

        {
            ViewBag.companies = from item in db.Company.ToList()
                                select new SelectListItem()
                                {
                                    Text = item.CompanyName,
                                    Value = item.CompanyId.ToString(),
                                };
            return View();
        }

        public JsonResult GetOrganisations(int companyId)
        {
            var result = from item in db.Company.Where(m => m.CompanyId == companyId).ToList()
                         select new
                         {
                             value = item.Organisation.OrganisationId,
                             text = item.Organisation.OrganisationName
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateSiteAndLoginRegisterConf(FormViewModel inputData, FormCollection fc, string[] dataType, string[] controlType, string[] fieldLabel)
        {
            try
            {

                string imagepath = null;
                int orgId = inputData.organisationDdl;
                int compId = inputData.CompanyDdl;
                if (inputData.OrganisationName != null)
                {
                    Organisation objOrganisation = new Organisation
                    {
                        OrganisationName = inputData.OrganisationName
                    };
                    db.Organisation.Add(objOrganisation);
                    db.SaveChanges();
                    orgId = objOrganisation.OrganisationId;
                }


                if (inputData.CompanyName != null)
                {
                    Company objCompany = new Company
                    {
                        CompanyName = inputData.CompanyName,
                        OrganisationId = orgId,
                    };
                    db.Company.Add(objCompany);
                    db.SaveChanges();
                    compId = objCompany.CompanyId;
                }

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
                    string savedPath = HostingEnvironment.MapPath("/Images/" + objSite.SiteId);
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
                    //HtmlCodeForLogin = dynamicHtmlCode
                };
                db.Form.Add(objForm);
                db.SaveChanges();

                string dynamicHtmlCode = null;
                if (dataType.Length != 0)
                {
                    int i;
                    for (i = 0; i < dataType.Length; i++)
                    {
                        var datatype = dataType[i];
                        var controltype = controlType[i];
                        var fieldlabel = fieldLabel[i];
                        string sqlString = "alter table [Users] add" + " " + fieldlabel + " " + datatype + " " + "NULL";
                        db.Database.ExecuteSqlCommand(sqlString);
                        StringBuilder sb = new StringBuilder(string.Empty);

                        FormControl objFormControl = new FormControl();
                        objFormControl.ControlType = controltype;
                        objFormControl.LabelName = fieldlabel;
                        objFormControl.FormId = objForm.FormId;
                        db.FormControl.Add(objFormControl);
                        db.SaveChanges();
                        //div start
                        sb.Append("<div>");
                        sb.Append("<input type=" + '"' + controltype + '"' + " " + "id=" + '"' + fieldlabel + '"' + " " + "name=" + '"' + fieldlabel + '"' + " " + "placeholder=" + '"' + "Enter" + " " + fieldlabel + '"' + "/>");
                        //div end
                        sb.Append("</div>");

                        dynamicHtmlCode += sb.ToString();
                    }
                }
                objForm.HtmlCodeForLogin = dynamicHtmlCode;
                db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Content("Configured successfully!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UpdateSiteAndLoginRegisterConf(FormViewModel inputData, FormCollection fc)
        {
            if(inputData.CompanyName==null)
            {
                Form objForm = new Form
                {
                    FormId=inputData.FormId,
                    SiteId = inputData.SiteId,
                    LoginPageTitle = inputData.LoginPageTitle,
                    RegistrationPageTitle=inputData.RegistrationPageTitle
                };
                db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Content("hi");
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
        public ActionResult AddField(string inputField, string inputType)
        {
            string sqlString = "alter table [Users] add" + " " + inputField + " " + inputType + " " + "NULL";
            db.Database.ExecuteSqlCommand(sqlString);
            return Content("Field Added!!");
        }


        public ActionResult SiteDetails()
        {
            var lstSite = (from item in db.Site.ToList()
                           select new SiteViewModel()
                           {
                               CmpName = item.Company.CompanyName,
                               OrgName = item.Company.Organisation.OrganisationName,
                               SiteName = item.SiteName,
                               SiteId = item.SiteId
                           }).ToList();
            return View(lstSite);
        }


        [HttpGet]
        public JsonResult GetCompanyAndOrganistaionDetails()
        {
            var companyResult = db.Company.Select(x => x.CompanyName).ToList();

            return Json(companyResult);
        }

        public ActionResult ConfigureSite(int SiteId)
        {
            try
            {
                ViewBag.companies = from item in db.Company.ToList()
                                    select new SelectListItem()
                                    {
                                        Text = item.CompanyName,
                                        Value = item.CompanyId.ToString(),
                                    };

                FormViewModel objViewModel = new FormViewModel();

                Form objForm = db.Form.FirstOrDefault(m => m.SiteId == SiteId);
                objForm.SiteId = SiteId;
                objViewModel.FormId = objForm.FormId;
                objViewModel.SiteName = db.Site.FirstOrDefault(m => m.SiteId == SiteId).SiteName;
                objViewModel.BannerIcon = objForm.BannerIcon;
                objViewModel.BackGroundColor = objForm.BackGroundColor;
                objViewModel.LoginWindowColor = objForm.LoginWindowColor;
                objViewModel.IsPasswordRequire = objForm.IsPasswordRequire;
                objViewModel.LoginPageTitle = objForm.LoginPageTitle;
                objViewModel.RegistrationPageTitle = objForm.RegistrationPageTitle;
                if (db.Site.Any(m => m.SiteId == SiteId))
                {
                    objViewModel.CompanyDdl = (int)db.Site.FirstOrDefault(m => m.SiteId == SiteId).CompanyId;
                }
               

                objViewModel.FormControls = db.FormControl.Where(m => m.FormId == objForm.FormId).ToList();
                return View(objViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        // GET: AdminIndex
        public ActionResult Index()
        {
            return View();
        }
    }
}