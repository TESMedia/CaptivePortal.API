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
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace CaptivePortal.API.Controllers
{
    public class AdminController : Controller
    {
        CPDBContext db = new CPDBContext();
        string ConnectionString = ConfigurationManager.ConnectionStrings["CPDBContext"].ConnectionString;

        StringBuilder sb = new StringBuilder(String.Empty);
        FormControl objFormControl = new FormControl();
        //int orgId = 0;
        //int compId = 0;
        //int siteId = 0;
        //int formId = 0;
        //string imagepath = null;
        //Form objForm = new Form();


        /// <summary>
        /// login operation for global admin.
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GAlogin")]
        public ActionResult GALogin(AdminLoginViewModel admin)
        {
            try
            {
                string retString = "-1";
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

        // GET: Global Admin
        public ActionResult Login()
        {
            return View();
        }
        //Get:Admin will create user.
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Populate company list in dropdown.
        /// </summary>
        /// <returns>Company details</returns>
        ///Get:Create new site 
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
                             value = item.Organisation.OrganisationId,
                             text = item.Organisation.OrganisationName
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult doesSiteNameExist(string SiteName)
        {
            var site = db.Site.Any(x => x.SiteName == SiteName);
            return Json(site);
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
            try
            {

                string imagepath = null;
                int orgId = inputData.organisationDdl;
                string compId = inputData.CompanyDdl;

                //organisation
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
                //company
                if (inputData.CompanyName != null)
                {
                    Company objCompany = new Company
                    {
                        CompanyName = inputData.CompanyName,
                        OrganisationId = orgId,
                    };
                    db.Company.Add(objCompany);
                    db.SaveChanges();
                    compId = objCompany.CompanyId.ToString();
                }

                //site
                Site objSite = new Site
                {
                    SiteName = inputData.SiteName,
                    CompanyId = compId==null?null:(int ?)Convert.ToInt32(compId),
                    AutoLogin = inputData.AutoLogin
                };
                db.Site.Add(objSite);
                db.SaveChanges();

                //image path
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

                //Term and condition
                if (Request.Files["Term_conditions"].ContentLength > 0)
                {
                    var httpPostedFile = Request.Files["Term_conditions"];
                    string savedPath = HostingEnvironment.MapPath("/Upload/" + objSite.SiteId);
                    imagepath = "/Upload/" + objSite.SiteId + "/" + httpPostedFile.FileName;
                    string completePath = Path.Combine(savedPath, httpPostedFile.FileName);

                    if (!System.IO.Directory.Exists(savedPath))
                    {
                        Directory.CreateDirectory(savedPath);
                    }
                    httpPostedFile.SaveAs(completePath);
                    inputData.BannerIcon = "/Upload/" + httpPostedFile.FileName;
                }

                //Form
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
                var formId = objForm.FormId;

                //Alter table with generating dynamic html code.
                //string dynamicHtmlCode = null;
                //if (fieldLabel.Length > 1)
                //{
                //    int i;
                //    for (i = 0; i < dataType.Length; i++)
                //    {
                //        var datatype = dataType[i];
                        //var controltype = controlType[i];
                        //var fieldlabel = fieldLabel[i];
                        //string sqlString = "alter table [Users] add" + " " + fieldlabel + " " + datatype + " " + "NULL";
                        //db.Database.ExecuteSqlCommand(sqlString);
                        //StringBuilder sb = new StringBuilder(string.Empty);

                        //FormControl objFormControl = new FormControl();
                        //objFormControl.ControlType = controltype;
                        //objFormControl.LabelName = fieldlabel;
                        //objFormControl.FormId = objForm.FormId;
                        //db.FormControl.Add(objFormControl);
                        //db.SaveChanges();
                        ////div start
                        //sb.Append("<div>");
                        //sb.Append("<input type=" + '"' + controltype + '"' + " " + "id=" + '"' + fieldlabel + '"' + " " + "name=" + '"' + fieldlabel + '"' + " " + "placeholder=" + '"' + "Enter" + " " + fieldlabel + '"' + "/>");
                        ////div end
                        //sb.Append("</div>");

                        //dynamicHtmlCode += sb.ToString();
                    //}
               // }
                //objForm.HtmlCodeForLogin = dynamicHtmlCode;
                //db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                throw ex;
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
                List<string> columnsList = db.Database.SqlQuery<string>("select column_name from information_schema.columns where table_name = 'users'").ToList();
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
                objViewModel.AutoLogin = objForm.Site.AutoLogin;
                objViewModel.RegistrationPageTitle = objForm.RegistrationPageTitle;
                objViewModel.fieldlabel = columnsList;
                if (db.Site.Any(m => m.SiteId == SiteId))
                {
                    objViewModel.CompanyDdl = db.Site.FirstOrDefault(m => m.SiteId == SiteId).CompanyId.ToString();
                }
                objViewModel.FormControls = db.FormControl.Where(m => m.FormId == objForm.FormId).ToList();
                return View(objViewModel);
            }
            catch (Exception ex)
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
                    var file = Request.Files[fc["BannerIcon"]];
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
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

        /// <summary>
        /// On Update site Detail Submit
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSiteAndLoginRegisterConf(FormViewModel inputData, FormCollection fc, FormControlViewModel model, string[] fieldLabel)
        {
            if (inputData.CompanyName == null)
            {
                string imagepath = null;
                if (Request.Files["BannerIcon"].ContentLength > 0)
                {
                    var httpPostedFile = Request.Files["BannerIcon"];
                    string savedPath = HostingEnvironment.MapPath("/Images/" + inputData.SiteId);
                    imagepath = "/Images/" + inputData.SiteId + "/" + httpPostedFile.FileName;
                    string completePath = Path.Combine(savedPath, httpPostedFile.FileName);

                    if (!System.IO.Directory.Exists(savedPath))
                    {
                        Directory.CreateDirectory(savedPath);
                    }
                    httpPostedFile.SaveAs(completePath);
                    inputData.BannerIcon = "/Images/" + httpPostedFile.FileName;
                }
                //form
                Form objForm = new Form
                {
                    FormId = inputData.FormId,
                    SiteId = inputData.SiteId,
                    BannerIcon = imagepath,
                    IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
                    BackGroundColor = inputData.BackGroundColor,
                    LoginWindowColor = inputData.LoginWindowColor,
                    LoginPageTitle = inputData.LoginPageTitle,
                    RegistrationPageTitle = inputData.RegistrationPageTitle
                };
                db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Admin");
        }

        
        [HttpPost]
        public JsonResult SaveFormControls(FormControlViewModel model,string IsMandetory)
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
            }
            catch (Exception ex)
            {
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
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("ConfigureSite", new { SiteId = objForm.SiteId });
        }

        //public ActionResult FormLogin()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult OnFormLoginSubmit(FormCollection form)
        //{
        //    Site result = db.Site.First();

        //    Form objForm = new Form
        //    {
        //        FormName = form["formName"],
        //        SiteId = result.SiteId
        //    };
        //    db.Form.Add(objForm);
        //    var res = db.SaveChanges();
        //    var id = objForm.FormId;

        //    FormControl objFormControl = new FormControl
        //    {
        //        FormId = id,
        //        ControlType = form["controlType"],
        //        LabelName = form["labelName"],
        //        SiteUrl = form["siteUrl"]
        //    };
        //    db.FormControl.Add(objFormControl);
        //    db.SaveChanges();
        //    return Content("hi");
        //}

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
        public ActionResult UserDetails(int? siteId,int? userId, int? page, string userName, string foreName, string surName)
        {
            UserlistViewModel list = new UserlistViewModel();
            list.UserViewlist = new List<UserViewModel>();
            int currentPageIndex = page.HasValue ? page.Value : 1;
            int PageSize = 5;
            double TotalPages = 0;
            var userList = db.Users.Where(m => m.SiteId == siteId).ToList();
            //If Searching on the basis of the single parameter
            if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(foreName) || !string.IsNullOrEmpty(surName))
            {
                if (!string.IsNullOrEmpty(foreName))
                {
                    //For the parameter contain only foreName  for searching or filter
                    if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(surName))
                    {
                        userList = db.Users.Where(p => p.FirstName.ToLower() == foreName.ToLower()).ToList().Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                        TotalPages = Math.Ceiling((double)db.Users.Where(p => p.FirstName.ToLower() == foreName.ToLower()).Count() / PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(surName))
                {
                    //For the parameter contain only surName  for searching or filter
                    if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(foreName))
                    {
                        userList = db.Users.Where(p => p.LastName.ToLower() == surName.ToLower()).ToList().Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                        TotalPages = Math.Ceiling((double)db.Users.Where(p => p.LastName.ToLower() == surName.ToLower()).Count() / PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    //For the parameter contain only username  for searching or filter
                    if (string.IsNullOrEmpty(foreName) && string.IsNullOrEmpty(surName))
                    {
                        userList = db.Users.Where(p => p.UserName.ToLower() == userName.ToLower()).ToList().Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                        TotalPages = Math.Ceiling((double)db.Users.Where(p => p.UserName.ToLower() == userName.ToLower()).Count() / PageSize);
                    }
                }
            }
            //If the Searching contain no parameter
            else
            {
                userList = userList.Skip(((int)currentPageIndex - 1) * PageSize).Take(PageSize).ToList();
                TotalPages = Math.Ceiling((double)db.Users.Count() / PageSize);
            }
            var userViewModelList = (from item in userList
                                     select new UserViewModel()
                                     {
                                         SiteId=siteId.Value,
                                         UserId = item.UserId,
                                         UserName = item.UserName,
                                         FirstName = item.FirstName,
                                         LastName = item.LastName,
                                         CreationDate = item.CreationDate,
                                         Password = item.Password
                                     }).ToList();
            list.UserViewlist.AddRange(userViewModelList);

            if (userId != null)
            {
                list.UserView = userViewModelList.FirstOrDefault(m => m.UserId == userId);
            }
            else
            {
                list.UserView = userViewModelList.FirstOrDefault();
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
        public ActionResult UserWithProfile(int UserId)
        {
            var userDetail = db.Users.FirstOrDefault(m => m.UserId == UserId);
            UserViewModel objUserViewModel = new UserViewModel();
            if (userDetail != null)
            {
                objUserViewModel.Password = userDetail.Password;
                objUserViewModel.UserName = userDetail.UserName;
                objUserViewModel.Gender = userDetail.Gender;
                objUserViewModel.AgeRange = userDetail.Age;
                objUserViewModel.AutoLogin = Convert.ToBoolean(userDetail.AutoLogin);
            }
            return PartialView("_UserDetails", objUserViewModel);
        }

        [HttpPost]
        public ActionResult UpdateUser(FormCollection fc)
        {
            var UserNameBefore = fc["UserName"];
            int userId = 0;

            //Updating the Users table
            if (db.Users.Any(m => m.UserName == UserNameBefore))
            {
                userId = db.Users.Where(m => m.UserName == UserNameBefore).FirstOrDefault().UserId;
                var objUser = db.Users.Find(userId);
                {
                    objUser.UserName = fc["UserName"];
                    objUser.Gender = fc["Gender"];
                    objUser.Age = fc["AgeRange"];
                    //objUser.MobileNumber = fc["MobileNumber"];
                    //objUser.IntStatus = Convert.ToString(fc["Status"]);
                    objUser.Email = fc["UserName"];
                    db.Entry(objUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
                return RedirectToAction("UserDetails","Admin");
        }

        [HttpPost]
        public ActionResult DeleteUser(int UserId)
        {
            Users user = db.Users.Find(UserId);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("UserDetails", "Admin");
        }

        public ActionResult UpdatePassword(int UserId)
        {
            return View();
        }

        public ActionResult MacAddress(int UserId)
        {
            return View();
        }

        //#region
        //private void InsertIntoOrganisation(FormViewModel inputData)
        //{
        //    try
        //    {
        //        Organisation objOrganisation = new Organisation
        //        {
        //            OrganisationName = inputData.OrganisationName
        //        };
        //        db.Organisation.Add(objOrganisation);
        //        db.SaveChanges();
        //        var orgId = objOrganisation.OrganisationId;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void InsertIntoCompany(FormViewModel inputData)
        //{
        //    try
        //    {
        //        Company objCompany = new Company
        //        {
        //            CompanyName = inputData.CompanyName,
        //            //OrganisationId = orgId,
        //        };
        //        db.Company.Add(objCompany);
        //        db.SaveChanges();
        //        var compId = objCompany.CompanyId;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void InsertIntoSite(FormViewModel inputData)
        //{
        //    try
        //    {
        //        Site objSite = new Site
        //        {
        //            SiteName = inputData.SiteName,
        //            //CompanyId = compId
        //        };
        //        db.Site.Add(objSite);
        //        db.SaveChanges();
        //        var siteId = objSite.SiteId;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private JsonResult InsertIntoForm(FormViewModel inputData)
        //{
        //    try
        //    {
        //        Form objForm = new Form
        //        {
        //            SiteId = siteId,
        //            BannerIcon = imagepath,
        //            BackGroundColor = inputData.BackGroundColor,
        //            LoginWindowColor = inputData.LoginWindowColor,
        //            IsPasswordRequire = Convert.ToBoolean(inputData.IsPasswordRequire),
        //            LoginPageTitle = inputData.LoginPageTitle,
        //            RegistrationPageTitle = inputData.RegistrationPageTitle,
        //            //HtmlCodeForLogin = dynamicHtmlCode
        //        };
        //        db.Form.Add(objForm);
        //        db.SaveChanges();
        //        formId = objForm.FormId;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Json(formId);
        //}

        //private void UpadteForm(FormViewModel inputData)
        //{
        //    try
        //    {
        //        Form objForm = new Form
        //        {
        //            FormId = inputData.FormId,
        //            SiteId = inputData.SiteId,
        //            LoginPageTitle = inputData.LoginPageTitle,
        //            RegistrationPageTitle = inputData.RegistrationPageTitle
        //        };
        //        db.Entry(objForm).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //#endregion
    }
}