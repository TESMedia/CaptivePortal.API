using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CaptivePortal.API.Models;
using CP.Lib;
using FP.Radius;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using CaptivePortal.API.Context;
using System.Web.Script.Serialization;
using log4net;
using System.Net.Mime;
using System.Reflection;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Web.Http.Cors;
using System.Web.SessionState;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Net.NetworkInformation;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CaptivePortal.API.ViewModels;
using CaptivePortal.API.Enums;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CaptivePortal.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AccountController));
        private RegisterDB objRegisterDB = new RegisterDB();
        private ReturnModel ObjReturnModel = new ReturnModel();
        Context.DbContext db = new Context.DbContext();
        StatusReturn objReturn = new StatusReturn();
        AutoLoginStatus returnStatus = new AutoLoginStatus();
        private string retStr = "";
        private string retType = "";
        private int retVal = 0;
        private UpdateDb objUpdateDb = new UpdateDb();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        string debugStatus = ConfigurationManager.AppSettings["DebugStatus"];
        
        public AccountController()
        {

        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
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
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/CreateUserWifi")]
        public async Task<HttpResponseMessage> CreateUserWifi(UserMacAddressDetails objUserMac)
        {
            //log.Info("Inside in a8Captiveportal/V1/CreateUserWifi");
            Notification objNotification = new Notification();
            string logInfoCreateUserWifi = "Inside in a8Captiveportal/V1/CreateUserWifi";
            string logInfoCreateWifiUserSuccess = null;
            string logInfoCreateUserWifiToMySql = null;
            //Site objSite = null;
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    log.Info(objUserMac.objUser.SiteId);
                    log.Info(objUserMac.objUser.UserName);
                    log.Info(objUserMac.objUser.PasswordHash);

                    if (string.IsNullOrEmpty(objUserMac.objUser.UserName))
                    {
                        retStr = "Need UserName for Registration";
                    }
                    else if (string.IsNullOrEmpty(objUserMac.objUser.PasswordHash))
                    {
                        retStr = "Need Password for Registration";
                    }
                    else if (objUserMac.objUser.SiteId == 0)
                    {
                        retStr = "Please send the SiteId for Registrartion";
                    }
                    else if(string.IsNullOrEmpty(objUserMac.objMacAddress.MacAddressValue))
                    {
                        retStr = "MacAddress not able to detect please contact Admin";
                    }
                    else if(db.Users.Any(m=>m.UserName== objUserMac.objUser.UserName && m.SiteId== objUserMac.objUser.SiteId))
                    {
                        retStr = "Same UserName already exist with particular Site";
                    }
                    else if(db.MacAddress.Any(m=>m.MacAddressValue== objUserMac.objMacAddress.MacAddressValue && m.Users.SiteId== objUserMac.objUser.SiteId))
                    {
                        retStr = "Same MacAddress already exist so directly Login";
                    }

                    if (string.IsNullOrEmpty(retStr))
                    {
                        CustomPasswordHasher objPassword = new CustomPasswordHasher();
                        //Save the Users data into Users table
                        objUserMac.objUser.CreationDate = DateTime.Now;
                        objUserMac.objUser.UpdateDate = DateTime.Now;
                        objUserMac.objUser.AutoLogin = false;
                        objUserMac.objUser.PasswordHash= objPassword.HashPassword(objUserMac.objUser.PasswordHash);
                        //var users = db.Users.Add(objUserMac.objUser);
                        var result =await UserManager.CreateAsync(objUserMac.objUser);
                       
                        log.Info("User Data saved in user Table");
                        logInfoCreateWifiUserSuccess = "User Data saved in user Table";
                        if (result.Succeeded)
                        {
                            UserManager.AddToRole(objUserMac.objUser.Id, "WiFiUser");

                            log.Info("Auto Login" + objUserMac.objUser.AutoLogin);
                            log.Info(objUserMac.objUser.Id);
                            objUserMac.objMacAddress.UserId = objUserMac.objUser.Id;
                            db.MacAddress.Add(objUserMac.objMacAddress);
                            log.Info(objUserMac.objMacAddress);
                            objUserMac.objAddress.UserId = objUserMac.objUser.Id;
                            db.UsersAddress.Add(objUserMac.objAddress);
                            db.SaveChanges();
                        }


                        //Save all the Users data in MySql DataBase
                        if (objRegisterDB.CreateNewUser(objUserMac.objUser.UserName, objUserMac.objUser.PasswordHash, objUserMac.objUser.Email, objUserMac.objUser.FirstName, objUserMac.objUser.LastName) == 0)
                        {
                            var ObjUser = db.Users.FirstOrDefault(m => m.Id == objUserMac.objUser.Id);
                            if (!string.IsNullOrEmpty(ObjUser.UniqueUserId) && !db.MacAddress.First(m => m.MacAddressValue == objUserMac.objMacAddress.MacAddressValue).IsRegisterInRtls)
                            {
                                using (CommunicateRTLS objCommunicateRtls = new CommunicateRTLS())
                                {
                                    string retResult = objCommunicateRtls.RegisterInRealTimeLocationService(new[] { objUserMac.objMacAddress.MacAddressValue }).Result;
                                    Notification objServiceReturn = JsonConvert.DeserializeObject<Notification>(retResult);
                                    if (objServiceReturn.result.returncode == Convert.ToInt32(RTLSResult.Success))
                                    {
                                        var objMac = db.MacAddress.FirstOrDefault(m => m.MacAddressValue == objUserMac.objMacAddress.MacAddressValue);
                                        objMac.IsRegisterInRtls = true;
                                        db.Entry(objMac).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }

                        retVal = Convert.ToInt32(ReturnCode.Success);
                        retType = ReturnCode.Success.ToString();
                        retStr = "Successfully Creted the User in database";
                        dbContextTransaction.Commit();


                        log.Info("user data saved and commited successfully");
                        logInfoCreateUserWifiToMySql = "user data saved and commited successfully";
                    }
                    else
                    {
                        retType = ReturnCode.Warning.ToString();
                        retVal = Convert.ToInt32(ReturnCode.Warning);
                    }
                    if (debugStatus == DebugMode.on.ToString())
                    {
                        string logMsg = string.Concat(logInfoCreateUserWifi, logInfoCreateWifiUserSuccess, logInfoCreateUserWifiToMySql, retStr);
                        log.Info(logMsg);
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    retVal = Convert.ToInt32(ReturnCode.Failure);
                    retType = ReturnCode.Failure.ToString();
                    retStr = ex.InnerException.ToString();
                    if (debugStatus == DebugMode.off.ToString())
                    {
                        log.Info(retStr);
                    }
                }

                objReturn.returncode = retVal;
                objReturn.msg = retStr;
                objReturn.type = retType;

                JavaScriptSerializer objSerialization = new JavaScriptSerializer();
                return new HttpResponseMessage()
                {
                    Content = new StringContent(objSerialization.Serialize(objReturn), Encoding.UTF8, "application/json")
                };
            }
        }


        [HttpPost]
        [Route("a8Captiveportal/V1/LoginWithNewMacAddress")]
        public HttpResponseMessage Login(LoginWIthNewMacAddressModel model)
        {
            log.Info("Inside Login");
           // string logInfoLoginWithNewMac = "a8Captiveportal/V1/LoginWithNewMacAddress";
            try
            {
                log.Info(model.SiteId);
                log.Info(model.UserName);
                log.Info(model.Password);
                log.Info(model.MacAddress);

                if (string.IsNullOrEmpty(model.UserName))
                {
                    retStr = "Need UserName for Registration";
                }
                else if (string.IsNullOrEmpty(model.Password))
                {
                    retStr = "Need Password for Registration";
                }
                else if (model.SiteId == 0)
                {
                    retStr = "Please send the SiteId for Registrartion";
                }
                else if (string.IsNullOrEmpty(model.MacAddress))
                {
                    retStr = "MacAddress not able to detect please contact Admin";
                }
                //else if(!(db.MacAddress.Any(m=>m.MacAddressValue==model.MacAddress && m.WifiUsers.SiteId==model.SiteId)))
                //{
                //    retStr = "Please Register before Login";
                //}
                else if (!(db.Users.Any(m => m.UserName == model.UserName && m.PasswordHash == model.Password && m.SiteId == model.SiteId)))
                {
                    retStr = "Please Register before Login ";
                }


                if (string.IsNullOrEmpty(retStr))
                {
                    var objWifiUsers = db.Users.FirstOrDefault(m => m.UserName == model.UserName && m.SiteId == model.SiteId);
                    if (!(db.MacAddress.Any(m => m.MacAddressValue == model.MacAddress && m.UserId == objWifiUsers.Id)))
                    {
                        //log.Info("Check that the particular MacAddress exist or Not for particualr User with Different Site";
                        MacAddress objMac = new MacAddress();
                        objMac.MacAddressValue = model.MacAddress;
                        objMac.UserId = objWifiUsers.Id;
                        objMac.BrowserName = model.BrowserName;
                        objMac.UserAgentName = model.UserAgentName;
                        objMac.OperatingSystem = model.OperatingSystem;
                        objMac.IsMobile = model.IsMobile;
                        db.MacAddress.Add(objMac);
                        db.SaveChanges();


                        //Save all the Users data in MySql DataBase

                        if ( String.IsNullOrEmpty(objMac.Users.UniqueUserId) && !db.MacAddress.First(m => m.MacAddressValue == model.MacAddress).IsRegisterInRtls)
                        {
                            using (CommunicateRTLS objCommunicateRtls = new CommunicateRTLS())
                            {
                                string retResult = objCommunicateRtls.RegisterInRealTimeLocationService(new[] { model.MacAddress }).Result;
                                Notification objServiceReturn = JsonConvert.DeserializeObject<Notification>(retResult);
                                if (objServiceReturn.result.returncode == Convert.ToInt32(RTLSResult.Success))
                                {
                                    var ObjMacAddress = db.MacAddress.FirstOrDefault(m => m.MacAddressValue == model.MacAddress);
                                    ObjMacAddress.IsRegisterInRtls = true;
                                    db.Entry(ObjMacAddress).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }

                        retStr = "Successfully add the new Maccadress with Authorize to connect to wifi";
                        retVal = Convert.ToInt32(ReturnCode.Success);
                        retType = ReturnCode.Success.ToString();
                    }
                    retVal = Convert.ToInt32(ReturnCode.Success);
                    retType = ReturnCode.Success.ToString();
                    retStr = "Successfully Authorize to connect wifi with Exist MacAddress";
                }
                else
                {
                    retVal = Convert.ToInt32(ReturnCode.Warning);
                    retType = ReturnCode.Warning.ToString();
                }
            }
            
            catch (Exception ex)
            {
                log.Error(ex.Message);
                retType = ReturnCode.Success.ToString();
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retStr = ex.Message.ToString();
            }
            objReturn.returncode = retVal;
            objReturn.msg = retStr;
            objReturn.type = retType;
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objReturn), Encoding.UTF8, "application/json")
            };
        }




        ///<summary>
        ///Check operational status for Site
        ///</summary>
        [HttpPost]
        [Route("a8Captiveportal/V1/SiteOperationalStatus")]
        public HttpResponseMessage OperationalStatus(LoginWIthNewMacAddressModel model)
        {
            var siteDetails = db.Site.FirstOrDefault(m => m.SiteId == model.SiteId);
            var replyMessage = "";
            List<string> pingList = new List<string>();
            string[] data = new string[4];
            data[0] = siteDetails.ControllerIpAddress;
            data[1] = siteDetails.MySqlIpAddress;
            data[2] = siteDetails.RtlsUrl;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    Ping myPing = new Ping();
                    if (data[i] != null)
                    {
                        PingReply reply = myPing.Send(data[i], 1000);
                        if (reply != null)
                        {
                            replyMessage = reply.Status.ToString();
                        }
                    }
                    else
                    {
                        replyMessage = "NotDeployed";

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                pingList.Add(replyMessage);
            }

            SqlParameter parameter1 = new SqlParameter("@SiteId", model.SiteId);
          // int result = db.Database.SqlQuery<int>("Exec GetSessionDataPerSite @siteId", parameter1).First();
            string filepath = HttpContext.Current.Server.MapPath("~/Logs/log.txt");
            var lineCount = File.ReadLines(filepath).Count();

          //  pingList.Add(result.ToString());
            pingList.Add(lineCount.ToString());
            pingList.Add(siteDetails.RtlsUrl);
            pingList.Add(siteDetails.DashboardUrl);
            pingList.Add(siteDetails.MySqlIpAddress);

            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(pingList), Encoding.UTF8, "application/json")
            };
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("GetStatusOfUserForSite")]
        public HttpResponseMessage GetStatusOfUserForSite(LoginWIthNewMacAddressModel model)
        {
            try
            {
                //Need to check the MacAddress exist for the particular Site with Autologin true
                if (db.MacAddress.Any(m => m.MacAddressValue == model.MacAddress && m.Users.SiteId == model.SiteId))
                {
                    log.Info("inside Is Any MacAddressExist For Particular Site");
                    var objMac = db.MacAddress.FirstOrDefault(m => m.MacAddressValue == model.MacAddress && m.Users.SiteId == model.SiteId);

                   
                    var objUserAsPerUserId = db.Users.FirstOrDefault(m => m.Id == objMac.UserId);
                    log.Info(objUserAsPerUserId.AutoLogin);
                    if (objUserAsPerUserId.AutoLogin == true)
                    {
                        log.Info("Check the AutoLogin of Site or User");
                        //objReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                        returnStatus.UserName = objUserAsPerUserId.UserName;
                        returnStatus.Password = objUserAsPerUserId.PasswordHash;
                        returnStatus.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                    }
                }
                else
                {
                    returnStatus.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Warning);
                }
                if (debugStatus == DebugMode.on.ToString())
                {
                    log.Info(retStr);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                returnStatus.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Info(retStr);
                }
            }
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(returnStatus), Encoding.UTF8, "application/json")
            };
        }

    }
}


