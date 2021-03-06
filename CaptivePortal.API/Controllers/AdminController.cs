﻿using CaptivePortal.API.Context;
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
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mime;
using System.Net;
using log4net;
using System.Net.Mail;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web.Security;

namespace CaptivePortal.API.Controllers
{
    public class AdminController : Controller
    {
        Context.DbContext db = new Context.DbContext();
        StringBuilder sb = new StringBuilder(String.Empty);
        FormControl objFormControl = new FormControl();
        string debugStatus = ConfigurationManager.AppSettings["DebugStatus"];
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AdminController));
        private string retStr = "";


        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AdminController()
        {

        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        [HttpPost]
        [Route("GAlogin")]
        public async Task<ActionResult> GALogin(AdminLoginViewModel model, string returnUrl)
        {
            int companyId = 0;
            try
            {
                ApplicationUser existUser = db.Users.Where(u => u.Email == model.UserName).FirstOrDefault();
                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.PasswordHash, model.RememberMe, shouldLockout: false);
                if (existUser.SiteId != null)
                {
                    companyId = Convert.ToInt32(db.Site.FirstOrDefault(m => m.SiteId == existUser.SiteId).CompanyId);
                }
                switch (result)
                {
                    case SignInStatus.Success:
                        //return RedirectToAction("Home", "Admin", new { SiteId = existUser.SiteId, UserName = existUser.UserName });
                        //return Json("success", JsonRequestBehavior.AllowGet);
                        string roleName = UserManager.GetRoles(existUser.Id).FirstOrDefault();
                        if (roleName == "BusinessUser")
                        {
                            return RedirectToAction("Home", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Home", "Admin");
                        }

                    case SignInStatus.Failure:
                    default:
                        // ModelState.AddModelError("", "Invalid login attempt.");
                        TempData["SuccessReset"] = "Invalid login attempt.";
                        return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Admin");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Global Admin
        public ActionResult Login(int? Id)
        {
            return View();
        }
        //Get:Admin will create user.
        public ActionResult Register()
        {
            ViewBag.groups = from item in db.Group.ToList()
                             select new SelectListItem()
                             {
                                 Text = item.GroupName,
                                 Value = item.GroupId.ToString(),
                             };

            return View();
        }


        public ActionResult ResetPassword(int userId, string code)
        {
            ResetPasswordViewModel objResetPassword = new ResetPasswordViewModel();
            try
            {
                using (var db = new Context.DbContext())
                {
                    if (userId != 0)
                    {
                        objResetPassword.Email = db.Users.Where(m => m.Id == userId).FirstOrDefault().Email;
                        var Code = code.Replace(" ", "+");
                        objResetPassword.Code = Code;
                    }
                    return View(objResetPassword);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View(objResetPassword);
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ResetPasswordForNewUser(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("", "Same EmailId is not exist ");
                return View();
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                string roleName = UserManager.GetRoles(user.Id).FirstOrDefault();
                int siteId = Convert.ToInt32(db.Users.FirstOrDefault(m => m.Id == user.Id).SiteId);
                int companyId = Convert.ToInt32(db.Site.FirstOrDefault(m => m.SiteId == siteId).CompanyId);
                if (roleName == "BusinessUser")
                {
                    return RedirectToAction("Login", "Admin", new { Id = companyId });
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            return View();
        }


        public JsonResult GetRestrictedSite(int siteId)
        {
            int compId = db.Site.FirstOrDefault(m => m.SiteId == siteId).Company.CompanyId;
            var result = from item in db.Site.Where(m => m.CompanyId == compId).ToList()
                         select new
                         {
                             value = item.SiteId,
                             text = item.SiteName,
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageUser(int? siteId, int? page, string userName, int? NumberOfLines)
        {
            UserlistViewModel list = new UserlistViewModel();
            var userList = db.Users.Where(m => m.SiteId == siteId).ToList();
            int PageSize = Convert.ToInt32(NumberOfLines);
            if (NumberOfLines != null)
            {
                PageSize = Convert.ToInt32(NumberOfLines);
                ViewBag.selectedNumber = NumberOfLines;
            }
            else
            {
                PageSize = 20;
            }

            var TotalPages = (int)Math.Ceiling((decimal)userList.Count / (decimal)PageSize);
            try
            {
                if (siteId != null)
                {
                    var userId = User.Identity.GetUserId();
                    list.UserViewlist = new List<UserViewModel>();
                    int currentPageIndex = page.HasValue ? page.Value : 1;
                    var startPage = currentPageIndex - 5;
                    int endPage = currentPageIndex + 4;
                    if (startPage <= 0)
                    {
                        endPage -= (startPage - 1);
                        startPage = 1;
                    }
                    if (endPage > TotalPages)
                    {
                        endPage = TotalPages;
                        if (endPage > 10)
                        {
                            startPage = endPage - 9;
                        }
                    }

                    userList = userList.Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                    // TotalPages = (int)Math.Ceiling((decimal)userList.Count / PageSize);

                    var userViewModelList = (from item in userList
                                             select new UserViewModel()
                                             {
                                                 SiteId = siteId.Value,
                                                 UserId = item.Id.ToString(),
                                                 UserName = item.UserName,
                                                 CreationDate = item.CreationDate,
                                                 Lastlogin = item.UpdateDate,
                                                 //Status = item.Status
                                                 Role = UserManager.GetRoles(item.Id).FirstOrDefault()


                                             }).ToList();
                    list.UserViewlist.AddRange(userViewModelList);

                    //if (userId != null)
                    //{
                    //    list.UserView = userViewModelList.FirstOrDefault(m => m.UserId == userId);
                    //}
                    //else
                    //{
                    //list.UserView = userViewModelList.FirstOrDefault();
                    //}
                    ViewBag.CurrentPage = currentPageIndex;
                    ViewBag.PageSize = PageSize;
                    ViewBag.TotalPages = TotalPages;
                    ViewBag.userName = userName;
                }
                else
                {
                    TempData["SiteIdCheck"] = "Please select any of the site and then manage user or If site is not there create new site";
                    return RedirectToAction("Home", "Admin");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(list);


        }



        /// <summary>
        /// Populate company list in dropdown.
        /// </summary>
        /// <returns>Company details</returns>
        ///Get:Create new site 
        public ActionResult CreateNewSite()
        {
            try
            {
                ViewBag.companies = from item in db.Company.ToList()
                                    select new SelectListItem()
                                    {
                                        Text = item.CompanyName,
                                        Value = item.CompanyId.ToString(),
                                    };
                retStr = "view company details";
                if (debugStatus == DebugMode.on.ToString())
                {
                    log.Info(retStr);
                }
            }
            catch (Exception ex)
            {
                retStr = "some problem occured";
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
            }
            return View();
        }

        /// <summary>
        /// papulate organisation list in dropdown on select of company.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public JsonResult GetOrganisations(int companyId)
        {
            var result = from item in db.Company.Where(m => m.CompanyId == companyId).ToList()
                         select new
                         {
                             value = item.Organisation == null ? 0 : item.Organisation.OrganisationId,
                             text = item.Organisation == null ? "" : item.Organisation.OrganisationName,
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult doesSiteNameExist(string SiteName)
        {
            var site = db.Site.Any(x => x.SiteName == SiteName);
            return Json(site);
        }


        public ActionResult CreatePromotionalMaterial(int? SiteId)
        {
            if (SiteId == null)
            {
                TempData["SiteIdCheck"] = "Please select any of the site and then manage promotional thing or If site is not there create new site";
                return RedirectToAction("Home", "Admin");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult StorePromotionalMaterial(ManagePromotion model)
        {
            string optionalPicturePath = null;
            string OptionalPictureForSuccessPage = null;
            ManagePromotion objManagePromotion = new ManagePromotion();
            var promo = db.ManagePromotion.ToList();
            if (promo.Count != 0)
            {
                var pro = db.ManagePromotion.Where(m => m.SiteId == model.SiteId).FirstOrDefault();
                if (pro != null)
                {
                    db.ManagePromotion.Remove(pro);
                    db.SaveChanges();
                }

                //image path
                if (Request.Files["OptionalPicture"].ContentLength > 0)
                {
                    var httpPostedFile = Request.Files["OptionalPicture"];
                    string savedPath = HostingEnvironment.MapPath("/Images/" + model.SiteId);
                    optionalPicturePath = "/Images/" + model.SiteId + "/" + httpPostedFile.FileName;
                    string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                    if (!System.IO.Directory.Exists(savedPath))
                    {
                        Directory.CreateDirectory(savedPath);
                    }
                    httpPostedFile.SaveAs(completePath);
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                    OptionalPictureForSuccessPage = baseUrl + optionalPicturePath;
                }

                objManagePromotion.SiteId = model.SiteId;
                objManagePromotion.SuccessPageOption = model.SuccessPageOption;
                objManagePromotion.WebPageURL = model.WebPageURL;
                objManagePromotion.OptionalPictureForSuccessPage = OptionalPictureForSuccessPage;
                db.ManagePromotion.Add(objManagePromotion);
                db.SaveChanges();
            }
            else
            {
                //image path
                if (Request.Files["OptionalPicture"].ContentLength > 0)
                {
                    var httpPostedFile = Request.Files["OptionalPicture"];
                    string savedPath = HostingEnvironment.MapPath("/Images/" + model.SiteId);
                    optionalPicturePath = "/Images/" + model.SiteId + "/" + httpPostedFile.FileName;
                    string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                    if (!System.IO.Directory.Exists(savedPath))
                    {
                        Directory.CreateDirectory(savedPath);
                    }
                    httpPostedFile.SaveAs(completePath);
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                    OptionalPictureForSuccessPage = baseUrl + optionalPicturePath;
                }

                objManagePromotion.SiteId = model.SiteId;
                objManagePromotion.SuccessPageOption = model.SuccessPageOption;
                objManagePromotion.WebPageURL = model.WebPageURL;
                objManagePromotion.OptionalPictureForSuccessPage = OptionalPictureForSuccessPage;
                db.ManagePromotion.Add(objManagePromotion);
                db.SaveChanges();
            }
            return RedirectToAction("Home", "Admin");
        }


        /// <summary>
        /// Create new site/org/comp/field.
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="fc"></param>
        /// <param name="dataType"></param>
        /// <param name="controlType"></param>
        /// <param name="fieldLabel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSiteAndLoginRegisterConf(FormViewModel inputData, FormCollection fc, FormControlViewModel model, string[] dataType, string[] controlType, string[] fieldLabel)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    retStr = "entered to create new site";
                    string imagepath = null;
                    string bannerPath = null;
                    string companyIcon = null;
                    int orgId = inputData.OrganisationDdl;
                    string compId = inputData.CompanyDdl;
                    string fileName = null;
                    string TandD = null;
                    Company objCompany = new Company();

                    //organisation
                    if (inputData.OrganisationName != null)
                    {
                        Organisation objOrganisation = new Organisation
                        {
                            OrganisationName = inputData.OrganisationName
                        };
                        db.Organisation.Add(objOrganisation);
                        db.SaveChanges();
                        orgId = Convert.ToInt32(objOrganisation.OrganisationId);
                    }
                    //company
                    if (inputData.CompanyName != null)
                    {

                        objCompany.CompanyName = inputData.CompanyName;
                        objCompany.OrganisationId = orgId == 0 ? null : (int?)Convert.ToInt32(orgId);


                    }
                    db.Company.Add(objCompany);
                    db.SaveChanges();
                    compId = objCompany.CompanyId.ToString();


                    if (Request.Files["CompanyIcon"].ContentLength > 0)
                    {
                        var httpPostedFile = Request.Files["CompanyIcon"];
                        string savedPath = HostingEnvironment.MapPath("/Images/" + objCompany.CompanyId);
                        imagepath = "/Images/" + objCompany.CompanyId + "/" + httpPostedFile.FileName;
                        string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                        if (!System.IO.Directory.Exists(savedPath))
                        {
                            Directory.CreateDirectory(savedPath);
                        }
                        httpPostedFile.SaveAs(completePath);
                        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                        companyIcon = baseUrl + imagepath;
                    }
                    objCompany.CompanyId = objCompany.CompanyId;
                    objCompany.CompanyIcon = companyIcon;
                    db.Entry(objCompany).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();




                    //Term and condition
                    if (Request.Files["Term_conditions"].ContentLength > 0)
                    {
                        var httpPostedFile = Request.Files["Term_conditions"];
                        string savedPath = HostingEnvironment.MapPath("/Upload/");
                        imagepath = "/Upload/" + httpPostedFile.FileName;
                        string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                        if (!System.IO.Directory.Exists(savedPath))
                        {
                            Directory.CreateDirectory(savedPath);
                        }
                        httpPostedFile.SaveAs(completePath);
                        fileName = httpPostedFile.FileName;
                        string name = httpPostedFile.ContentType;
                        if (httpPostedFile.ContentType == "application/pdf")
                        {
                            StringBuilder text = new StringBuilder();
                            using (PdfReader reader = new PdfReader(completePath))
                            {
                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                {
                                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                                }
                            }
                            TandD = text.ToString();
                        }
                        else
                        {

                            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                            object miss = System.Reflection.Missing.Value;
                            object path = completePath;
                            object readOnly = true;
                            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
                            string totaltext = "";
                            for (int i = 0; i < docs.Paragraphs.Count; i++)
                            {
                                totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString();
                            }

                            TandD = totaltext;
                        }

                    }

                    //site
                    Site objSite = new Site
                    {
                        SiteName = inputData.SiteName,
                        CompanyId = compId == null ? null : (int?)Convert.ToInt32(compId),
                        AutoLogin = inputData.AutoLogin,
                        ControllerIpAddress = inputData.ControllerIpAddress,
                        MySqlIpAddress = inputData.MySqlIpAddress,
                        Term_conditions = inputData.Term_conditions,
                        TermsAndCondDoc = TandD,
                        DashboardUrl = inputData.DashboardUrl,
                        RtlsUrl = inputData.RtlsUrl

                    };
                    db.Site.Add(objSite);
                    db.SaveChanges();



                    //image path
                    if (Request.Files["BannerIcon"].ContentLength > 0)
                    {
                        var httpPostedFile = Request.Files["BannerIcon"];
                        string savedPath = HostingEnvironment.MapPath("/Images/" + objSite.SiteId);
                        imagepath = "/Images/" + objSite.SiteId + "/" + httpPostedFile.FileName;
                        string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                        if (!System.IO.Directory.Exists(savedPath))
                        {
                            Directory.CreateDirectory(savedPath);
                        }
                        httpPostedFile.SaveAs(completePath);
                        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                        bannerPath = baseUrl + imagepath;
                    }



                    //Form
                    Form objForm = new Form
                    {
                        SiteId = objSite.SiteId,
                        BannerIcon = bannerPath,
                        BackGroundColor = inputData.BackGroundColor,
                        LoginWindowColor = inputData.LoginWindowColor,
                        IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
                        LoginPageTitle = inputData.LoginPageTitle,
                        RegistrationPageTitle = inputData.RegistrationPageTitle,

                        //HtmlCodeForLogin = dynamicHtmlCode
                    };
                    db.Form.Add(objForm);
                    db.SaveChanges();
                    var formId = objForm.FormId;
                    dbContextTransaction.Commit();

                    if (debugStatus == DebugMode.on.ToString())
                    {
                        log.Info(retStr);
                    }
                    return RedirectToAction("Home", "Admin");
                }
                catch (Exception ex)
                {
                    retStr = "some problem occured";
                    dbContextTransaction.Rollback();
                    if (debugStatus == DebugMode.off.ToString())
                    {
                        log.Info(retStr);
                    }

                    throw ex;
                }
            }
        }

        /// <summary>
        /// Show existing site details.
        /// </summary>
        /// <returns></returns>
        /// Get:Site details.
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

        /// <summary>
        /// Populate Site details or form details of existing site Or create new org/comp/field/.
        /// </summary>
        /// <param name="SiteId"></param>
        /// <returns></returns>
        public ActionResult ConfigureSite(int? SiteId)
        {
            FormViewModel objViewModel = new FormViewModel();
            try
            {
                if (SiteId != null)
                {
                    retStr = "populated selected site details to configure";
                    ViewBag.companies = from item in db.Company.ToList()
                                        select new SelectListItem()
                                        {
                                            Text = item.CompanyName,
                                            Value = item.CompanyId.ToString(),
                                        };
                    List<string> columnsList = db.Database.SqlQuery<string>("select column_name from information_schema.columns where table_name = 'users'").ToList();
                    int compId = Convert.ToInt32(db.Site.FirstOrDefault(m => m.SiteId == SiteId).CompanyId);
                    Form objForm = db.Form.FirstOrDefault(m => m.SiteId == SiteId);
                    objForm.SiteId = Convert.ToInt32(SiteId);
                    objViewModel.SiteId = Convert.ToInt32(SiteId);
                    objViewModel.FormId = objForm.FormId;
                    objViewModel.SiteName = db.Site.FirstOrDefault(m => m.SiteId == SiteId).SiteName;
                    objViewModel.BannerIcon = objForm.BannerIcon;
                    objViewModel.CompanyIcon = db.Company.FirstOrDefault(m => m.CompanyId == compId).CompanyIcon;
                    objViewModel.BackGroundColor = objForm.BackGroundColor;
                    objViewModel.LoginWindowColor = objForm.LoginWindowColor;
                    objViewModel.IsPasswordRequire = objForm.IsPasswordRequire;
                    objViewModel.LoginPageTitle = objForm.LoginPageTitle;
                    objViewModel.AutoLogin = Convert.ToBoolean(objForm.Site.AutoLogin);
                    objViewModel.RegistrationPageTitle = objForm.RegistrationPageTitle;
                    objViewModel.ControllerIpAddress = db.Site.FirstOrDefault(m => m.SiteId == SiteId).ControllerIpAddress;
                    objViewModel.MySqlIpAddress = db.Site.FirstOrDefault(m => m.SiteId == SiteId).MySqlIpAddress;
                    objViewModel.Term_conditions = db.Site.FirstOrDefault(m => m.SiteId == SiteId).Term_conditions;
                    objViewModel.DashboardUrl = db.Site.FirstOrDefault(m => m.SiteId == SiteId).DashboardUrl;
                    objViewModel.RtlsUrl = db.Site.FirstOrDefault(m => m.SiteId == SiteId).RtlsUrl;

                    objViewModel.TermsAndCondDoc = db.Site.FirstOrDefault(m => m.SiteId == SiteId).TermsAndCondDoc;
                    objViewModel.Fieldlabel = columnsList;
                    if (db.Site.Any(m => m.SiteId == SiteId))
                    {
                        objViewModel.CompanyDdl = db.Site.FirstOrDefault(m => m.SiteId == SiteId).CompanyId.ToString();
                    }
                    objViewModel.FormControls = db.FormControl.Where(m => m.FormId == objForm.FormId).ToList();

                    if (debugStatus == DebugMode.on.ToString())
                    {
                        log.Info(retStr);
                    }

                }
                else
                {

                    TempData["SiteIdCheck"] = "Please select any of the site you want to cofigure if site is not available please create new site and configure";
                    return RedirectToAction("Home", "Admin");
                }
            }

            catch (Exception ex)
            {
                retStr = "some problem occured";
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
                throw ex;
            }
            return View(objViewModel);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFile(FormCollection fc)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    var file = Request.Files[fc["BannerIcon"]];
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    if (debugStatus == DebugMode.on.ToString())
                    {
                        log.Info(retStr);
                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    retStr = "some problem occured";
                    if (debugStatus == DebugMode.on.ToString())
                    {
                        log.Info(retStr);
                    }
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        /// <summary>
        /// On Update site Detail Submit
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSiteAndLoginRegisterConf(FormViewModel inputData, FormCollection fc, FormControlViewModel model, string[] fieldLabel)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    retStr = "entered to configure site";
                    if (inputData.CompanyName == null)
                    {
                        string imagepath = null;
                        string bannerPath = null;
                        string companyIconPath = null;
                        string filePath = null;
                        string fileName = null;
                        string TandD = null;
                        int compId = Convert.ToInt32(inputData.CompanyDdl);
                        if (Request.Files["BannerIcon"].ContentLength > 0)
                        {
                            var httpPostedFile = Request.Files["BannerIcon"];
                            string savedPath = HostingEnvironment.MapPath("/Images/" + inputData.SiteId);
                            imagepath = "/Images/" + inputData.SiteId + "/" + httpPostedFile.FileName;
                            string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                            if (!System.IO.Directory.Exists(savedPath))
                            {
                                Directory.CreateDirectory(savedPath);
                            }
                            httpPostedFile.SaveAs(completePath);
                            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                            bannerPath = baseUrl + imagepath;
                        }
                        else
                        {
                            bannerPath = inputData.BannerIcon;
                        }

                        if (Request.Files["CompanyIcon"].ContentLength > 0)
                        {
                            var httpPostedFile = Request.Files["CompanyIcon"];
                            string savedPath = HostingEnvironment.MapPath("/Images/" + inputData.SiteId);
                            imagepath = "/Images/" + compId + "/" + httpPostedFile.FileName;
                            string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                            if (!System.IO.Directory.Exists(savedPath))
                            {
                                Directory.CreateDirectory(savedPath);
                            }
                            httpPostedFile.SaveAs(completePath);
                            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                            companyIconPath = baseUrl + imagepath;
                        }
                        else
                        {
                            companyIconPath = inputData.CompanyIcon;
                        }

                        //Term and condition
                        if (Request.Files["Term_conditions"].ContentLength > 0)
                        {
                            var httpPostedFile = Request.Files["Term_conditions"];
                            string savedPath = HostingEnvironment.MapPath("/Upload/");
                            filePath = "/Upload/" + httpPostedFile.FileName;
                            string completePath = System.IO.Path.Combine(savedPath, httpPostedFile.FileName);

                            if (!System.IO.Directory.Exists(savedPath))
                            {
                                Directory.CreateDirectory(savedPath);
                            }
                            httpPostedFile.SaveAs(completePath);
                            fileName = httpPostedFile.FileName;

                            if (httpPostedFile.ContentType == "application/pdf")
                            {
                                StringBuilder text = new StringBuilder();
                                using (PdfReader reader = new PdfReader(completePath))
                                {
                                    for (int i = 1; i <= reader.NumberOfPages; i++)
                                    {
                                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                                    }
                                }
                                TandD = text.ToString();
                            }
                            else
                            {

                                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                                object miss = System.Reflection.Missing.Value;
                                object path = completePath;
                                object readOnly = true;
                                Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
                                string totaltext = "";
                                for (int i = 0; i < docs.Paragraphs.Count; i++)
                                {
                                    totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString();
                                }

                                TandD = totaltext;
                            }
                        }
                        else
                        {
                            TandD = inputData.TermsAndCondDoc;
                        }

                        //site
                        Site objSite = new Site
                        {
                            SiteName = inputData.SiteName,
                            SiteId = inputData.SiteId,

                            CompanyId = compId == null ? null : (int?)Convert.ToInt32(compId),
                            AutoLogin = inputData.AutoLogin,
                            ControllerIpAddress = inputData.ControllerIpAddress,
                            MySqlIpAddress = inputData.MySqlIpAddress,
                            Term_conditions = inputData.Term_conditions,
                            TermsAndCondDoc = TandD,
                            DashboardUrl = inputData.DashboardUrl,
                            RtlsUrl = inputData.RtlsUrl


                        };

                        db.Entry(objSite).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //form
                        Form objForm = new Form
                        {
                            FormId = inputData.FormId,
                            SiteId = inputData.SiteId,
                            BannerIcon = bannerPath,
                            IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
                            BackGroundColor = inputData.BackGroundColor,
                            LoginWindowColor = inputData.LoginWindowColor,
                            LoginPageTitle = inputData.LoginPageTitle,
                            RegistrationPageTitle = inputData.RegistrationPageTitle
                        };
                        db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        int companyId = Convert.ToInt32(compId);
                        Company objCompany = new Company
                        {
                            CompanyId = companyId,
                            CompanyIcon = companyIconPath,
                        };
                        db.Entry(objCompany).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();

                    }
                    if (debugStatus == DebugMode.on.ToString())
                    {
                        log.Info(retStr);
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    retStr = "some problem occured";
                    if (debugStatus == DebugMode.off.ToString())
                    {
                        log.Info(retStr);
                    }
                    throw ex;
                }
                return RedirectToAction("Home", "Admin");
            }
        }


        [HttpPost]
        public JsonResult SaveFormControls(FormControlViewModel model, string IsMandetory)
        {
            try
            {
                Form objForm = db.Form.FirstOrDefault(m => m.FormId == model.FormId);

                if (model.fieldlabel == "Custom3" || model.fieldlabel == "Custom5")
                {
                    sb.Append("<div>");
                    sb.Append("<select name=" + '"' + model.LabelNameToDisplay + '"' + ">");
                    foreach (var item in model.arrayValue)
                    {
                        sb.Append("<option value=" + '"' + item + '"' + ">" + item + "</option>");
                    }
                    sb.Append("</select>");
                    sb.Append("</div>");
                }
                else if (model.fieldlabel == "Gender")
                {
                    sb.Append("<div>");
                    sb.Append("<select name=" + '"' + model.LabelNameToDisplay + '"' + ">");
                    sb.Append("<option value=" + '"' + "male" + '"' + ">" + "male" + "</option>");
                    sb.Append("<option value=" + '"' + "male" + '"' + ">" + "male" + "</option>");
                    sb.Append("</select>");
                    sb.Append("</div>");
                }
                else if (model.fieldlabel == "AgeRange")
                {
                    sb.Append("<div>");
                    sb.Append("<select name=" + '"' + model.LabelNameToDisplay + '"' + ">");
                    sb.Append("<option value=" + '"' + "9-19" + '"' + ">" + "9-19" + "</option>");
                    sb.Append("<option value=" + '"' + "19-29" + '"' + ">" + "19-29" + "</option>");
                    sb.Append("<option value=" + '"' + "29-39" + '"' + ">" + "29-39" + "</option>");
                    sb.Append("<option value=" + '"' + "39-49" + '"' + ">" + "39-49" + "</option>");
                    sb.Append("<option value=" + '"' + "49-59" + '"' + ">" + "49-59" + "</option>");
                    sb.Append("<option value=" + '"' + "59-69" + '"' + ">" + "59-69" + "</option>");
                    sb.Append("</select>");
                    sb.Append("</div>");
                }
                else if (model.fieldlabel == "Custom5" || model.fieldlabel == "Custom6")
                {
                    sb.Append("<div>");
                    foreach (var item in model.arrayValue)
                    {
                        sb.Append("<input type=" + "checkbox" + " " + "id=" + '"' + model.LabelNameToDisplay + '"' + " " + "name=" + '"' + model.fieldlabel + '"' + " " + "value=" + '"' + item + '"' + ">" + item);
                    }
                    sb.Append("</div>");
                }
                //else if (model.controlType == "radio")
                //{
                //    sb.Append("<div>");
                //    foreach (var item in model.arrayValue)
                //    {
                //        sb.Append("<input type=" + '"' + model.controlType + '"' + " " + "id=" + '"' + model.fieldlabel + '"' + " " + "name=" + '"' + model.fieldlabel + '"' + " " + "value=" + '"' + item + '"' + ">" + item);
                //    }
                //    sb.Append("</div>");
                //}
                else
                {
                    if (model.chkOptOrMand == "true")
                    {
                        sb.Append("<div class='form-group'>");
                        sb.Append("<label class='control-label col-sm-2'>" + model.LabelNameToDisplay + "</label>");
                        sb.Append("<div class='col-sm-10'>");
                        sb.Append("<input type=" + '"' + "text" + '"' + '"' + "" + "class='form-control'" + " " + "placeholder = " + '"' + "Enter" + " " + model.fieldlabel + '"' + "required" + "/> ");
                        sb.Append("</div>");
                        sb.Append("</div>");
                    }
                    else
                    {
                        sb.Append("<div class='form-group'>");
                        sb.Append("<label class='control-label col-sm-2'>" + model.LabelNameToDisplay + "</label>");
                        sb.Append("<div class='col-sm-10'>");
                        sb.Append("<input type=" + '"' + "text" + '"' + '"' + "" + "class='form-control'" + " " + "placeholder = " + '"' + "Enter" + " " + model.fieldlabel + '"' + " /> ");
                        sb.Append("</div>");
                        sb.Append("</div>");
                    }
                }

                FormControl objFormControl = new FormControl();
                objFormControl.ControlType = model.controlType;
                objFormControl.LabelName = model.fieldlabel;
                objFormControl.LabelNameToDisplay = model.LabelNameToDisplay;
                objFormControl.IsMandetory = Convert.ToBoolean(IsMandetory);
                objFormControl.FormId = model.FormId;
                objFormControl.HtmlString = sb.ToString();
                db.FormControl.Add(objFormControl);

                //db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                //objForm.HtmlCodeForLogin = sb.ToString();
                db.SaveChanges();
                if (debugStatus == DebugMode.on.ToString())
                {
                    log.Info(retStr);
                }
            }
            catch (Exception ex)
            {
                retStr = "some problem occured";
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
                return Json("Failure");
            }
            return Json("Success");
        }

        [HttpGet]
        public ActionResult DeleteFormControl(int Id)
        {
            FormControl objFormControl = null;
            Form objForm = null;
            try
            {
                objFormControl = db.FormControl.FirstOrDefault(m => m.FormControlId == Id);
                objForm = db.FormControl.FirstOrDefault(m => m.FormControlId == Id).Forms;
                db.FormControl.Remove(objFormControl);
                db.SaveChanges();
                if (debugStatus == DebugMode.on.ToString())
                {
                    log.Info(retStr);
                }
            }
            catch (Exception ex)
            {
                retStr = "some problem occured";
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
            }
            return RedirectToAction("ConfigureSite", new { SiteId = objForm.SiteId });
        }



        // GET: AdminIndex
        public ActionResult Index()
        {
            ViewBag.sites = from item in db.Site.ToList()
                            select new SelectListItem()
                            {
                                Value = item.SiteId.ToString(),
                                Text = item.SiteName
                            };
            return View();
        }

        //[Authorize(Roles = "GlobalAdmin,CompanyAdmin")]
        public ActionResult Home()
        {
            AdminlistViewModel list = new AdminlistViewModel();
            list.AdminViewlist = new List<AdminViewModel>();

            int userId = User.Identity.GetUserId<int>();
            string role = UserManager.GetRoles(userId).FirstOrDefault();
            ViewBag.roleOfUser = role;
            int siteId = Convert.ToInt32(db.Users.FirstOrDefault(m => m.Id == userId).SiteId);

            retStr = "entered in home to view overall estate";
            int compId = 0;
            //int orgId = Convert.ToInt32(db.Company.FirstOrDefault(m => m.CompanyId == compId).Organisation.OrganisationId) == 0 ? 0 : Convert.ToInt32(db.Company.FirstOrDefault(m => m.CompanyId == compId).Organisation.OrganisationId);
            try
            {
                if (siteId != 0)
                {
                    if (role == "CompanyAdmin")
                    {
                        compId = db.Site.FirstOrDefault(m => m.SiteId == siteId).Company.CompanyId;

                        var accessSite = db.AdminSiteAccess.Where(m => m.UserId == userId).ToList();
                        var accessSiteDetails = (from site in accessSite
                                                 select new AdminViewModel()
                                                 {
                                                     // OrganisationName = db.Organisation.FirstOrDefault(m=>m.OrganisationId==orgId).OrganisationName,
                                                     CompanyName = db.Company.FirstOrDefault(m => m.CompanyId == compId).CompanyName,
                                                     SiteName = site.SiteName,
                                                     MySqlIpAddress = db.Site.FirstOrDefault(m => m.SiteId == site.SiteId).MySqlIpAddress,
                                                     DashboardUrl = db.Site.FirstOrDefault(m => m.SiteId == site.SiteId).DashboardUrl,
                                                     RtlsUrl = db.Site.FirstOrDefault(m => m.SiteId == site.SiteId).RtlsUrl,
                                                     DefaultSite = db.Users.FirstOrDefault(m => m.Id == userId).PhoneNumber,//default site to access
                                                     SiteId = site.SiteId
                                                 }).ToList();
                        list.AdminViewlist.AddRange(accessSiteDetails);
                    }
                    else
                    {

                        compId = db.Site.FirstOrDefault(m => m.SiteId == siteId).Company.CompanyId;
                        var result = db.Site.Where(m => m.CompanyId == compId).ToList();
                        var siteDetails = (from item in result
                                           select new AdminViewModel()
                                           {
                                               OrganisationName = item.Company.Organisation == null ? null : item.Company.Organisation.OrganisationName,

                                               CompanyName = item.Company.CompanyName,
                                               SiteName = item.SiteName,
                                               MySqlIpAddress = item.MySqlIpAddress,
                                               DashboardUrl = item.DashboardUrl,
                                               RtlsUrl = item.RtlsUrl,
                                               SiteId = item.SiteId
                                           }
                                         ).ToList();
                        list.AdminViewlist.AddRange(siteDetails);
                        if (debugStatus == DebugMode.on.ToString())
                        {
                            log.Info(retStr);
                        }
                    }
                }
                else
                {
                    var result = db.Site.ToList();

                    var siteDetails = (from item in result
                                       select new AdminViewModel()
                                       {
                                           OrganisationName = item.Company.Organisation == null ? null : item.Company.Organisation.OrganisationName,
                                           CompanyName = item.Company.CompanyName,
                                           SiteName = item.SiteName,
                                           DashboardUrl = item.DashboardUrl,
                                           RtlsUrl = item.RtlsUrl,
                                           SiteId = item.SiteId
                                       }
                                     ).ToList();
                    list.AdminViewlist.AddRange(siteDetails);
                }
            }

            catch (Exception ex)
            {
                retStr = "some problem occured";
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
                throw ex;
            }
            return View(list);
        }

        public ActionResult UploadFile(int? siteId)
        {
            if (siteId != 0 && siteId != null)
            {
                return View();
            }
            else
            {
                TempData["SiteIdCheck"] = "Please select any of the site and then upload";
                return RedirectToAction("Home", "Admin");
            }
        }

        public ActionResult Locations(int? siteId)
        {
            if (siteId != 0 && siteId != null)
            {
                return View();
            }
            else
            {
                TempData["SiteIdCheck"] = "Please select any of the site and then got location mapping";
                return RedirectToAction("Home", "Admin");
            }
        }

        public ActionResult CreateLocation()
        {
            return View();
        }

        public ActionResult EditLocation(int? id, string SiteName)
        {
            return View();
        }

        public ActionResult Locationdashboard()
        {
            return View();
        }



        public ActionResult CreateUser(int? siteId)
        {
            SitelistViewModel list = new SitelistViewModel();
            try
            {
                int compId = db.Site.FirstOrDefault(m => m.SiteId == siteId).Company.CompanyId;
                list.SiteViewlist = new List<SiteViewModel>();
                var siteList = db.Site.Where(m => m.CompanyId == compId).ToList();

                var siteViewModelList = (from item in siteList
                                         select new SiteViewModel()
                                         {
                                             SiteName = item.SiteName,
                                             SiteId = item.SiteId
                                         }).ToList();
                list.SiteViewlist.AddRange(siteViewModelList);
                ViewBag.sites = from item in db.Site.Where(m => m.CompanyId == compId).ToList()
                                select new SelectListItem()
                                {
                                    Value = item.SiteId.ToString(),
                                    Text = item.SiteName
                                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(list);

        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> CreateUserWithRole(CreateUserWithRoleViewModel model, FormCollection fc, string[] RestrictedSites)
        {
            string defaultSiteName = db.Site.FirstOrDefault(m => m.SiteId == model.SiteDdl).SiteName;
            try
            {

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CreationDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    SiteId = model.SiteDdl,
                    Status = Status.Active.ToString(),
                    PhoneNumber = defaultSiteName//Store the SiteName As default Site in Identity Column named PhoneNumber
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await this.UserManager.AddToRoleAsync(user.Id, model.RoleId);
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Admin", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Welcome to the Captive portal Dashboard", "You are receiving this email as you have been set up as a user of the captive portal Dashboard. To complete the registration process please click <a href=\"" + callbackUrl + "\">here</a>" + " " + "to reset your password and login.If you have any issues with the login process, or were not expecting this email, please email support@airloc8.com.");
                    TempData["Success"] = "An Email has sent to your Inbox.";

                    AdminSiteAccess objAdminSite1 = new AdminSiteAccess();
                    objAdminSite1.UserId = user.Id;
                    objAdminSite1.SiteId = model.SiteDdl;
                    objAdminSite1.SiteName = db.Site.FirstOrDefault(m => m.SiteId == model.SiteDdl).SiteName;
                    db.AdminSiteAccess.Add(objAdminSite1);
                    db.SaveChanges();

                    //sites which are selected by admin to give access to company admin ,store in AdminSiteAccessTable
                    //foreach (var item in RestrictedSites)
                    //{
                    //    AdminSiteAccess objAdminSite = new AdminSiteAccess();
                    //    int x = 0;
                    //    Int32.TryParse(item, out x);
                    //    objAdminSite.UserId = user.Id;
                    //    objAdminSite.SiteId = model.SiteDdl;
                    //    objAdminSite.SiteName = db.Site.FirstOrDefault(m => m.SiteId == x).SiteName;
                    //    db.AdminSiteAccess.Add(objAdminSite);
                    //    db.SaveChanges();
                    //}
                }
                else
                {
                    TempData["Success"] = "Username" + " " + model.Email + " " + "already taken.";
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("CreateUser", "Admin", new { SiteId = model.SiteDdl });
        }



        public JsonResult GetCompany(int orgId)
        {
            var result = from item in db.Company.Where(m => m.CompanyId == orgId).ToList()
                         select new
                         {
                             value = item.CompanyId,
                             text = item.CompanyName
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSite(int compId)
        {
            var result = from item in db.Site.Where(m => m.CompanyId == compId).ToList()
                         select new
                         {
                             value = item.SiteId,
                             text = item.SiteName
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDetails(int? siteId, int? userId, int? page, string userName, string foreName, string surName, int? NumberOfLines, int? GroupName)
        {
            ViewBag.groups = from item in db.Group.ToList()
                             select new SelectListItem()
                             {
                                 Text = item.GroupName,
                                 Value = item.GroupId.ToString(),
                             };
            //userId = User.Identity.GetUserId();
            WifiUserlistViewModel list = new WifiUserlistViewModel();
            list.WifiUserViewlist = new List<WifiUserViewModel>();
            int currentPageIndex = page.HasValue ? page.Value : 1;
            int PageSize = Convert.ToInt32(NumberOfLines);
            var userList = db.Users.Where(m => m.SiteId == siteId).ToList();
            int roleId = 4;
            userList = db.Users
    .Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId))
    .ToList();

            if (NumberOfLines != null)
            {
                PageSize = Convert.ToInt32(NumberOfLines);
                ViewBag.selectedNumber = NumberOfLines;
            }
            else
            {
                PageSize = 20;
            }

            var TotalPages = (int)Math.Ceiling((decimal)userList.Count / (decimal)PageSize);

            var startPage = currentPageIndex - 5;
            int endPage = currentPageIndex + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > TotalPages)
            {
                endPage = TotalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            //Search user according to Group
            if (GroupName != 0 & GroupName != null)
            {
                userList = db.Users.Where(m => m.GroupId == GroupName).ToList();
            }
            //var userList = db.Users.Where(m => m.SiteId == siteId).ToList();
            //If Searching on the basis of the single parameter
            if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(foreName) || !string.IsNullOrEmpty(surName))
            {
                if (!string.IsNullOrEmpty(foreName))
                {
                    //For the parameter contain only foreName  for searching or filter
                    if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(surName))
                    {
                        userList = db.Users.Where(p => p.FirstName.ToLower().Contains(foreName.ToLower())).ToList().Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                        TotalPages = (int)Math.Ceiling((double)db.Users.Where(p => p.FirstName.ToLower() == foreName.ToLower()).Count() / PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(surName))
                {
                    //For the parameter contain only surName  for searching or filter
                    if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(foreName))
                    {
                        userList = db.Users.Where(p => p.LastName.ToLower().Contains(surName.ToLower())).ToList().Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                        TotalPages = (int)Math.Ceiling((double)db.Users.Where(p => p.LastName.ToLower() == surName.ToLower()).Count() / PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    //For the parameter contain only username  for searching or filter
                    if (string.IsNullOrEmpty(foreName) && string.IsNullOrEmpty(surName))
                    {
                        userList = db.Users.Where(p => p.UserName.ToLower().Contains(userName.ToLower())).ToList().Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                        TotalPages = (int)Math.Ceiling((double)db.Users.Where(p => p.UserName.ToLower() == userName.ToLower()).Count() / PageSize);
                    }
                }
            }
            //If the Searching contain no parameter
            else
            {
                userList = userList.Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                //TotalPages = (int)Math.Ceiling((decimal)db.Users.Count() / PageSize);
            }
            //if (userList.Count != 1)
            //{
            var userViewModelList = (from item in userList
                                     select new WifiUserViewModel()
                                     {
                                         SiteId = Convert.ToInt32(item.SiteId),
                                         UserId = item.Id,
                                         UserName = item.UserName,
                                         FirstName = item.FirstName,
                                         LastName = item.LastName,
                                         CreationDate = item.CreationDate,
                                         Lastlogin = item.UpdateDate,
                                         //SiteName= SiteName
                                         // Password = item.Password,
                                         MacAddress = db.MacAddress.Where(x => x.UserId == item.Id).OrderByDescending(x => x.MacId).Take(1).Select(x => x.MacAddressValue).ToList().FirstOrDefault()

                                     }).ToList();
            list.WifiUserViewlist.AddRange(userViewModelList);
            //}
            //else
            //{
            //    TempData["userSuc"] = "No data found";
            //}

            if (userId != null)
            {
                list.WifiUserView = userViewModelList.FirstOrDefault(m => m.UserId == userId);
            }
            else
            {
                list.WifiUserView = userViewModelList.FirstOrDefault();
            }
            ViewBag.CurrentPage = currentPageIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalPages = TotalPages;
            ViewBag.foreName = foreName;
            ViewBag.surName = surName;
            ViewBag.userName = userName;
            return View(list);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserWithProfile(int SiteId, int userId)
        {
            ViewBag.groups = from item in db.Group.ToList()
                             select new SelectListItem()
                             {
                                 Text = item.GroupName,
                                 Value = item.GroupId.ToString(),
                             };
            var userDetail = db.Users.FirstOrDefault(m => m.Id == userId);
            var termConditionVersion = db.Site.FirstOrDefault(m => m.SiteId == SiteId).Term_conditions;
            var siteName = db.Site.FirstOrDefault(m => m.SiteId == SiteId).SiteName;
            var model = new MacAddressViewModel();
            WifiUserViewModel objUserViewModel = new WifiUserViewModel();
            if (userDetail != null)
            {
                objUserViewModel.Password = userDetail.PasswordHash;
                objUserViewModel.UserName = userDetail.UserName;
                objUserViewModel.Gender = db.Gender.FirstOrDefault(m => m.GenderId == userDetail.GenderId) == null ? null : db.Gender.FirstOrDefault(m => m.GenderId == userDetail.GenderId).Value;
                objUserViewModel.AgeRange = db.Age.FirstOrDefault(m => m.AgeId == userDetail.AgeId) == null ? null : db.Age.FirstOrDefault(m => m.AgeId == userDetail.AgeId).Value;
                objUserViewModel.AutoLogin = Convert.ToBoolean(userDetail.AutoLogin);
                objUserViewModel.Term_conditions = termConditionVersion;
                objUserViewModel.PromotionEmailOptIn = Convert.ToBoolean(userDetail.promotional_email);
                objUserViewModel.ThirdPartyOptIn = Convert.ToBoolean(userDetail.ThirdPartyOptIn);
                objUserViewModel.UserOfDataOptIn = Convert.ToBoolean(userDetail.UserOfDataOptIn);
                //objUserViewModel.Status = (Status)Enum.Parse(typeof(Status), userDetail.Status);
                var mac = db.MacAddress.Where(m => m.Users.SiteId == SiteId).ToList();
                //var lastEntry = db.MacAddress.LastOrDefault(m => m.UserId == UserId).MacAddressValue;
                //objUserViewModel.MacAddress = lastEntry;
                objUserViewModel.MacAddressList = mac;
                //string connectionString = ConfigurationManager.ConnectionStrings["CPDBContext"].ToString();
                //SqlConnection con = new SqlConnection(connectionString);
                //con.Open();
                //using (SqlDataAdapter da = new SqlDataAdapter())
                //{
                //    da.SelectCommand = new SqlCommand("GetSessionObject", con);
                //    da.SelectCommand.CommandType = CommandType.StoredProcedure;

                //    DataSet ds = new DataSet();
                //    da.Fill(ds, "result_name");

                //    DataTable dt = ds.Tables["result_name"];

                //    foreach (DataRow row in dt.Rows)
                //    {
                //        var lastLogin = row.ItemArray[1];
                //        //var sessionlength=row.ItemArray[]
                //    }
                //}
            }
            return PartialView("_UserDetails", objUserViewModel);
        }

        [HttpPost]
        public ActionResult UpdateUser(FormCollection fc)
        {
            var UserNameBefore = fc["UserName"];
            int userId = Convert.ToInt16(fc["UserId"]);


            //Updating the Users table
            if (db.Users.Any(m => m.UserName == UserNameBefore))
            {
                //userId = db.Users.Where(m => m.UserName == UserNameBefore).FirstOrDefault().UserId;
                var objUser = db.Users.Find(userId);
                {
                    objUser.UserName = fc["UserName"];
                    //objUser.GenderId = Convert.ToInt32(fc["GenderId"]);
                    //objUser.AgeId = Convert.ToInt32(fc["AgeId"]);
                    objUser.GroupId = Convert.ToInt32(fc["GroupDdl"]);
                    //objUser.MobileNumber = fc["MobileNumber"];
                    //objUser.IntStatus = Convert.ToString(fc["Status"]);
                    objUser.Status = fc["Status"].ToString();

                    objUser.Email = fc["UserName"];
                    db.Entry(objUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("UserDetails", "Admin");
        }

        [HttpPost]
        public void DeleteUser(int UserId)
        {
            ApplicationUser user = db.Users.Find(UserId);
            db.Users.Remove(user);
            db.SaveChanges();
        }

        public ActionResult UpdatePassword(int UserId)
        {
            return View();
        }


        [HttpPost]
        public ActionResult UpdatePassword(FormCollection fc)
        {
            int userId = Convert.ToInt16(fc["UserId"]);
            if (fc["NewPassword"] == fc["ConfirmPassword"])
            {
                var objUser = db.Users.Find(userId);
                {
                    objUser.PasswordHash = fc["NewPassword"];
                    db.Entry(objUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("UserDetails", "Admin");
        }

        public ActionResult AddMacAddress(FormCollection fc)
        {
            int userId = Convert.ToInt16(fc["UserId"]);
            var objUser = db.Users.Find(userId);
            {

                MacAddress mac = new MacAddress();
                mac.MacAddressValue = fc["MacAddress"];
                mac.UserId = userId;
                db.MacAddress.Add(mac);
                //db.Entry(objUser).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("UserDetails", "Admin");

        }

        [HttpPost]
        public ActionResult DeleteMacAddress(int MacId)
        {
            MacAddress objMac = db.MacAddress.Find(MacId);
            {
                db.MacAddress.Remove(objMac);
                db.SaveChanges();
            }
            return RedirectToAction("UserDetails", "Admin");
        }

        [HttpGet]
        public ActionResult LogsDownload()
        {
            try
            {
                retStr = "download log file";
                string path = Server.MapPath("~/Logs/log.txt");
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                if (file.Exists)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, MediaTypeNames.Application.Octet, "log.txt");
                }
                if (debugStatus == DebugMode.on.ToString())
                {
                    log.Info(retStr);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                retStr = "some problem occured";
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult TestSetUpRtls()
        {
            return View();
        }

        public ActionResult ViewRtlsData()
        {
            return View();
        }

        public ActionResult ViewErrorLogRtls()
        {
            return View();
        }





    }
}