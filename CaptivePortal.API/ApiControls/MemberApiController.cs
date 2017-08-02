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

namespace CaptivePortal.API.ApiControls
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class MemberApiController : ApiController
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(MemberApiController));
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

        public MemberApiController()
        {

        }
        public MemberApiController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        /// <param name="objUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/Login")]
        public HttpResponseMessage Login(ApplicationUser objUser)
        {
            //log.Info("Enter in a8Captiveportal/V1/Login");
            string logInfoLogin = "Enter in a8Captiveportal/V1/Login";
            string logInfoForSessionIdRet = null;

            string sessionId = null;
            try
            {
                if (objUser.UserName.ToLower() == "testpurple@mail.com")
                {
                    if (db.Users.Any(m => m.UserName.ToLower() == objUser.UserName.ToLower() && m.PasswordHash == objUser.PasswordHash))
                    {
                        var UserId = db.Users.FirstOrDefault(m => m.UserName == objUser.UserName).Id;
                        log.Info(UserId + "inside a8Captiveportal/V1/Login");
                        SessionIDManager manager = new SessionIDManager();
                        sessionId = manager.CreateSessionID(HttpContext.Current);
                        //Insert the UserSession data with SessionId
                        if (db.UserSession.Any(m => m.UserId == UserId))
                        {
                            // log.Info("Insert the UserSession data with SessionId");
                            logInfoForSessionIdRet = "Insert the UserSession data with SessionId";
                            ApiAccessUserSession UserSession = db.UserSession.FirstOrDefault(m => m.UserId == UserId);
                            UserSession.SessionId = sessionId;
                            db.Entry(UserSession).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ApiAccessUserSession objUserSession = new ApiAccessUserSession();
                            objUserSession.SessionId = sessionId;
                            objUserSession.UserId = UserId;
                            db.UserSession.Add(objUserSession);
                            db.SaveChanges();
                        }
                        retStr = sessionId;
                        retVal = Convert.ToInt32(ReturnCode.LoginSuccess);
                        retType = "Session";
                    }
                    else
                    {
                        retStr = "Incorrect Password";
                        retVal = Convert.ToInt32(ErrorCodeWarning.IncorrectPassword);
                        retType = ReturnCode.Warning.ToString();
                    }
                }
                else
                {
                    retStr = "Username does not exist";
                    retVal = Convert.ToInt32(ErrorCodeWarning.usernameisnotexist);
                    retType = ReturnCode.Warning.ToString();
                }

                if (debugStatus == DebugMode.on.ToString())
                {
                    string logMsg = String.Concat(logInfoLogin, logInfoForSessionIdRet, retStr);
                    log.Info(logMsg);
                }

            }
            catch (Exception ex)
            {
                //log.Error(ex.Message);
                retStr = "Some Error Occured";
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retType = ReturnCode.Failure.ToString();
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Error(retStr);
                }
            }

            objReturn.msg = retStr;
            objReturn.returncode = retVal;
            objReturn.type = retType;
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objReturn), Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/CreateUser")]
        public HttpResponseMessage CreateUserExpose(CreateUserViewModel objUser)
        {
            //log.Info("Enter in a8Captiveportal/V1/CreateUser");
            string logInfoCreateUser = "Enter in a8Captiveportal/V1/CreateUser";
            string logInfoForCreateUserSuccess = null;
            Site objSite = null;
            string logInfoIsSessionId = null;
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    //First check the Manadatory validation and show the Error Messages
                    if (string.IsNullOrEmpty(objUser.UserName))
                    {
                        retStr = "Username missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.UserNameRequired);
                    }
                    else if (string.IsNullOrEmpty(objUser.Password))
                    {
                        retStr = "Password missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.PasswordRequired);
                    }
                    else if (objUser.SiteId == 0)
                    {
                        retStr = "SiteId missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.SiteIDRequired);
                    }
                    else if (!(db.Site.Any(m => m.SiteId == objUser.SiteId)))
                    {
                        retStr = "SiteId Not Exist";
                        retVal = Convert.ToInt32(ErrorCodeWarning.SiteIdNotExist);
                    }
                    else if (string.IsNullOrEmpty(objUser.UserId))
                    {
                        retStr = "UserId missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.UserIdRequired);
                    }
                    else if (string.IsNullOrEmpty(objUser.SessionId))
                    {
                        retStr = "SessionId missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.SessionIdRequired);
                    }
                    else if (db.Users.Any(m => m.UniqueUserId == objUser.UserId))
                    {
                        retStr = "UserId already exists";
                        retVal = Convert.ToInt32(ErrorCodeWarning.UserUniqueIdAlreadyExist);
                    }

                    //if No validation Error then Insert the data into the table
                    if (string.IsNullOrEmpty(retStr))
                    {
                        //log.Info("if No validation Error then Insert the data into the table");
                        logInfoForCreateUserSuccess = "if No validation Error then Insert the data into the table";
                        objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);
                        //if Same User with Site Exist then don't allow
                        if (!(db.Users.Any(m => m.UserName == objUser.UserName && m.SiteId == objUser.SiteId)))
                        {

                            if (IsAuthorize(objUser.SessionId))
                            {

                                CustomPasswordHasher objCustomPasswordHasher = new CustomPasswordHasher();
                                //log.Info("Checked User is authorized.");
                                logInfoIsSessionId = "Checked User is authorized.";
                                ApplicationUser objUsers = new ApplicationUser();
                                objUsers.CreationDate = DateTime.Now;
                                objUsers.UpdateDate = DateTime.Now;
                                objUsers.UserName = objUser.UserName;
                                objUsers.Email = objUser.Email;
                                objUsers.FirstName = objUser.FirstName;
                                objUsers.LastName = objUser.LastName;
                                objUsers.PasswordHash = objCustomPasswordHasher.HashPassword(objUser.Password);
                                objUsers.SiteId = objUser.SiteId;
                                objUsers.UniqueUserId = objUser.UserId;
                                objUsers.BirthDate = objUser.BirthDate;
                                objUsers.AgeId = objUser.AgeId;
                                objUsers.GenderId = objUser.GenderId;
                                objUsers.MobileNumer = objUser.MobileNumber;
                              

                                var result = UserManager.CreateAsync(objUsers);
                               
                                if(result.Result.Succeeded)
                                {
                                    UserManager.AddToRole(objUsers.Id, "WiFiUser");

                                    //Save all the Users data in MySql DataBase
                                    objRegisterDB.CreateNewUser(objUsers.UserName, objUsers.PasswordHash, objUsers.Email, objUsers.FirstName, objUsers.LastName);
                                }
                                
                                retVal = Convert.ToInt32(ReturnCode.CreateUserSuccess);
                                retType = ReturnCode.Success.ToString();
                                retStr = "User created";
                                dbContextTransaction.Commit();
                            }
                            else
                            {

                                retVal = Convert.ToInt32(ErrorCodeWarning.NonAuthorize);
                                retType = ReturnCode.Warning.ToString();
                                retStr = "Not Authorize with Sessions" + " " + objUser.SessionId;
                            }
                        }
                        else
                        {
                            retVal = Convert.ToInt32(ErrorCodeWarning.MacAddressorUserNameExist);
                            retStr = "Username already exists" + " " + objSite.SiteName;
                            retType = ReturnCode.Warning.ToString();
                        }
                    }
                    else
                    {
                        retType = ReturnCode.Warning.ToString();
                    }
                    if (debugStatus == DebugMode.on.ToString())
                    {
                        string logMsg = string.Concat(logInfoCreateUser, logInfoForCreateUserSuccess, logInfoIsSessionId, retStr);
                        log.Info(logMsg);
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    retVal = Convert.ToInt32(ReturnCode.Failure);
                    retType = ReturnCode.Warning.ToString();
                    retStr = "Error Occured";
                    if (debugStatus == DebugMode.off.ToString())
                    {
                        log.Error(retStr);
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


        /// <summary>
        /// it will add or delete one or more mac adddress of a user.
        /// </summary>
        /// <param name="objUserMac"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("a8Captiveportal/V1/UpdateMacAddress")]
        public HttpResponseMessage UpdateMacAddress(AddMacModel objUserMac)
        {
            string logInfoUpdateMacAddress = "enterd into a8Captiveportal/V1/UpdateMacAddress";
            string logInfoMacAddressAdded = null;
            string logInfoMacAddressDeleted = null;
            try
            {
                MacAddress objMac = new MacAddress();
                int userId;

                if (objUserMac.OperationType == 0)
                {
                    retStr = "please send operation type i.e 1(add) or 2(delete).";
                    retVal = Convert.ToInt32(ErrorCodeWarning.OperationTypeMissing); ;
                }
                if (objUserMac.OperationType == OperationType.Add)
                {
                    //check mandatory request.
                    if (objUserMac.SiteId == 0)
                    {
                        retStr = "SiteId Missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.SiteIDRequired);
                    }
                    else if (!(db.Site.Any(m => m.SiteId == objUserMac.SiteId)))
                    {
                        retStr = "Incorrect SiteId";
                        retVal = Convert.ToInt32(ErrorCodeWarning.SiteIdNotExist);
                    }
                    else if (string.IsNullOrEmpty(objUserMac.UserId))
                    {
                        retStr = "UserId Missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.UserIdRequired);
                    }
                    else if (!(db.Users.Any(m => m.UniqueUserId == objUserMac.UserId)))
                    {
                        retStr = "Incorrect UserId";
                        retVal = Convert.ToInt32(ErrorCodeWarning.IncorrectUserId);
                    }
                    else if (string.IsNullOrEmpty(objUserMac.SessionId))
                    {
                        retStr = "SessionId Missing";
                        retVal = Convert.ToInt32(ErrorCodeWarning.SessionIdRequired);
                    }

                    if (string.IsNullOrEmpty(retStr))
                    {
                        if (IsAuthorize(objUserMac.SessionId))
                        {
                            userId = db.Users.FirstOrDefault(m => m.UniqueUserId == objUserMac.UserId).Id;
                            foreach (var item in objUserMac.MacAddressList)
                            {
                                if (db.MacAddress.Any(m => m.MacAddressValue == item.MacAddress))
                                {
                                    retStr = "mac address already exist";
                                    retVal = Convert.ToInt32(ErrorCodeWarning.MacAddressorUserNameExist);
                                    retType = ReturnCode.Warning.ToString();
                                }
                                else
                                {
                                    objMac.MacAddressValue = item.MacAddress;
                                    objMac.UserId = userId;
                                    db.MacAddress.Add(objMac);
                                    db.SaveChanges();

                                    if(!db.MacAddress.First(m=>m.MacAddressValue==objMac.MacAddressValue).IsRegisterInRtls)
                                    {
                                        using (CommunicateRTLS objCommunicateRtls = new CommunicateRTLS())
                                        {
                                            string retResult= objCommunicateRtls.RegisterInRealTimeLocationService(new[] { item.MacAddress }).Result;
                                            Notification objServiceReturn = JsonConvert.DeserializeObject<Notification>(retResult);
                                            if(objServiceReturn.result.returncode== Convert.ToInt32(RTLSResult.Success))
                                            {
                                                objMac.IsRegisterInRtls = true;
                                                db.Entry(objMac).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    
                                    retStr = "mac address added";
                                    logInfoMacAddressAdded = "mac address added";
                                    retType = ReturnCode.Success.ToString();
                                    retVal = Convert.ToInt32(ReturnCode.UpdateMacAddressuccess);
                                }
                            }
                        }
                        else
                        {

                            retVal = Convert.ToInt32(ErrorCodeWarning.NonAuthorize);
                            retType = ReturnCode.Warning.ToString();
                            retStr = "Invalid SessionId";
                        }
                    }
                    else
                    {
                        retVal = Convert.ToInt32(ReturnCode.Warning);
                        retType = ReturnCode.Warning.ToString();
                    }
                }
                if (objUserMac.OperationType == OperationType.Delete)
                {
                    foreach (var macaddress in objUserMac.MacAddressList)
                    {
                        if (db.MacAddress.Any(m => m.MacAddressValue == macaddress.MacAddress))
                        {
                            db.MacAddress.RemoveRange(db.MacAddress.Where(c => c.MacAddressValue == macaddress.MacAddress));
                            db.SaveChanges();
                            retStr = "mac address deleted ";
                            logInfoMacAddressDeleted = "mac address deleted ";
                            retType = ReturnCode.Success.ToString();
                            retVal = Convert.ToInt32(ReturnCode.UpdateMacAddressuccess);
                        }
                        else
                        {
                            retStr = " mac address doesnot exist ";
                            retType = ReturnCode.Warning.ToString();
                            retVal = Convert.ToInt32(ErrorCodeWarning.MacAddressNotExist);
                        }
                    }
                }
                if (debugStatus == DebugMode.on.ToString())
                {
                    string logMsg = string.Concat(logInfoUpdateMacAddress, logInfoMacAddressAdded, logInfoMacAddressDeleted, retStr);
                    log.Info(logMsg);
                }
            }

            catch (Exception ex)
            {
                //log.Error(ex.Message);
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retType = ReturnCode.Warning.ToString();
                retStr = "some problem occured";
                //log.Error(ex.Message);
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Error(retStr);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/GetMacAddresses")]
        public HttpResponseMessage GetMacAddress(CreateUserViewModel model)
        {
            //First check the Manadatory 
            //validation and show the Error Messages
            string logInfoGetMacAddress = "entered into a8Captiveportal/V1/GetMacAddresses";
            string logInfoGetMacAddressRet = null;
            if (model.SiteId == 0)
            {
                retStr = "SiteId missing";
                retVal = Convert.ToInt32(ErrorCodeWarning.SiteIDRequired);
            }
            else if (!(db.Site.Any(m => m.SiteId == model.SiteId)))
            {
                retStr = "SiteId Not Exist";
                retVal = Convert.ToInt32(ErrorCodeWarning.SiteIdNotExist);
            }
            else if (string.IsNullOrEmpty(model.UserId))
            {
                retStr = "UserId missing";
                retVal = Convert.ToInt32(ErrorCodeWarning.UserIdRequired);
            }
            else if (string.IsNullOrEmpty(model.SessionId))
            {
                retStr = "SessionId missing";
                retVal = Convert.ToInt32(ErrorCodeWarning.SessionIdRequired);
            }

            ReturnMacAesddress objReturnMac = new ReturnMacAesddress();

            try
            {
                if (string.IsNullOrEmpty(retStr))
                {
                    if (IsAuthorize(model.SessionId))
                    {
                        if (db.Users.Any(m => m.UniqueUserId == model.UserId && m.SiteId == model.SiteId))
                        {
                            int UserId = db.Users.FirstOrDefault(m => m.UniqueUserId == model.UserId && m.SiteId == model.SiteId).Id;
                            foreach (var item in db.MacAddress.Where(m => m.UserId == UserId))
                            {
                                MacAddesses objMacAddress = new MacAddesses();
                                objMacAddress.MacAddress = item.MacAddressValue;
                                objReturnMac.MacAddressList.Add(objMacAddress);
                            }
                            retVal = Convert.ToInt32(ReturnCode.GetMacAddressSuccess);
                            retStr = "Successfully return the MacAddresses";
                            //log.Info("Successfully return the MacAddresses");
                            logInfoGetMacAddressRet = "Successfully return the MacAddresses";
                            retType = ReturnCode.Success.ToString();
                        }
                        else
                        {
                            retVal = Convert.ToInt32(ReturnCode.Warning);
                            retType = ReturnCode.Warning.ToString();
                            retStr = "UserId or SiteId Not Exist";
                        }
                    }
                    else
                    {

                        retVal = Convert.ToInt32(ErrorCodeWarning.NonAuthorize);
                        retType = ReturnCode.Warning.ToString();
                        retStr = "Invalid SessionId" + " " + model.SessionId;
                    }
                }
                else
                {
                    retType = ReturnCode.Warning.ToString();
                }
                if (debugStatus == DebugMode.on.ToString())
                {
                    string logMsg = string.Concat(logInfoGetMacAddress, logInfoGetMacAddressRet, retStr);
                    log.Info(logMsg);
                }
            }
            catch (Exception ex)
            {
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retStr = "Error Occured";
                retType = ReturnCode.Failure.ToString();
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Error(retStr);
                }
            }

            objReturnMac.returncode = retVal;
            objReturnMac.msg = retStr;
            objReturnMac.type = retType;

            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objReturnMac), Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("a8Captiveportal/V1/DeleteUser")]
        public HttpResponseMessage DeleteWiFiUser(CreateUserViewModel model)
        {
            string logInfoDeleteUser = "inside a8Captiveportal/V1/DeleteUser";
            string logIfoUserDeleteSuccess = null;

            //First check the Manadatory 
            //validation and show the Error Messages
            if (string.IsNullOrEmpty(model.UserId))
            {
                retStr = "UserId Missing";
                retVal = Convert.ToInt32(ErrorCodeWarning.UserIdRequired);
            }
            else if (model.SiteId == 0)
            {
                retStr = "SiteId Missing";
                retVal = Convert.ToInt32(ErrorCodeWarning.SiteIDRequired);
            }
            else if (!(db.Site.Any(m => m.SiteId == model.SiteId)))
            {
                retStr = "Incorrect SiteId";
                retVal = Convert.ToInt32(ErrorCodeWarning.SiteIdNotExist);
            }
            else if (!(db.Users.Any(m => m.UniqueUserId == model.UserId)))
            {
                retStr = "Incorrect UserId";
                retVal = Convert.ToInt32(ErrorCodeWarning.IncorrectUserId);
            }
            else if (string.IsNullOrEmpty(model.SessionId))
            {
                retStr = "SessionId missing";
                retVal = Convert.ToInt32(ErrorCodeWarning.SessionIdRequired);
            }

            try
            {
                if (string.IsNullOrEmpty(retStr))
                {
                    if (IsAuthorize(model.SessionId))
                    {
                        var user = db.Users.FirstOrDefault(m => m.UniqueUserId == model.UserId);
                        if (user != null)
                        {
                            if (db.MacAddress.First(m => m.UserId == user.Id).IsRegisterInRtls)
                            {
                                using (CommunicateRTLS objCommunicateRtls = new CommunicateRTLS())
                                {
                                    var lstMacAddress = db.MacAddress.Where(m => m.UserId == user.Id).Select(m => m.MacAddressValue).ToArray();
                                    string retResult = objCommunicateRtls.RegisterInRealTimeLocationService(lstMacAddress).Result;
                                    Notification objServiceReturn = JsonConvert.DeserializeObject<Notification>(retResult);
                                    if (objServiceReturn.result.returncode == Convert.ToInt32(RTLSResult.Success))
                                    {
                                        db.Users.Remove(user);
                                        db.SaveChanges();
                                        retStr = "User deleted ";
                                        logIfoUserDeleteSuccess = "User deleted";
                                        retVal = Convert.ToInt32(ReturnCode.DeleteUserSuccess);
                                        retType = ReturnCode.Success.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            retStr = "User not found";
                            retVal = Convert.ToInt32(ReturnCode.Warning);
                            retType = ReturnCode.Warning.ToString();
                        }
                    }
                    else
                    {
                        retVal = Convert.ToInt32(ErrorCodeWarning.NonAuthorize);
                        retType = ReturnCode.Warning.ToString();
                        retStr = "Invalid SessionId";
                    }
                }
                else
                {
                    retStr = "something went wrong";
                    retType = ReturnCode.Failure.ToString();
                    retVal = Convert.ToInt32(ReturnCode.Failure);
                }
                objReturn.returncode = retVal;
                objReturn.msg = retStr;
                objReturn.type = retType;

                if (debugStatus == DebugMode.on.ToString())
                {
                    string logMsg = string.Concat(logInfoDeleteUser, logIfoUserDeleteSuccess, retStr);
                    log.Info(logMsg);
                }
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retStr = "Error Occured";
                retType = ReturnCode.Failure.ToString();
                if (debugStatus == DebugMode.off.ToString())
                {
                    log.Error(retStr);
                }
            }
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objReturn), Encoding.UTF8, "application/json")
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/UpdateUser")]
        public HttpResponseMessage UpdateUser(CreateUserViewModel objUser)
        {
            string logInfoUpdateUser = "entered into a8Captiveportal/V1/UpdateUser";
            string logInfoUpdateUserInSqlDb = null;
            string logInfoUpdateUserInMySqlDb = null;
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (IsAuthorize(objUser.SessionId))
                    {
                        ApplicationUser user = new ApplicationUser();

                        //check mandatory request.
                        if (objUser.SiteId == 0)
                        {
                            retStr = "SiteId missing";
                            retVal = Convert.ToInt32(ErrorCodeWarning.SiteIDRequired);
                        }
                        else if (!(db.Site.Any(m => m.SiteId == objUser.SiteId)))
                        {
                            retStr = "Incorrect SiteId";
                            retVal = Convert.ToInt32(ErrorCodeWarning.SiteIdNotExist);
                        }
                        else if (string.IsNullOrEmpty(objUser.UserId))
                        {
                            retStr = "UserId missing";
                            retVal = Convert.ToInt32(ErrorCodeWarning.UserIdRequired);
                        }
                        else if (string.IsNullOrEmpty(objUser.UserName))
                        {
                            retStr = "Username missing";
                            retVal = Convert.ToInt32(ErrorCodeWarning.UserNameRequired);
                        }
                        else if (!(db.Users.Any(m => m.UniqueUserId == objUser.UserId)))
                        {
                            retStr = "Incorrect UserId";
                            retVal = Convert.ToInt32(ErrorCodeWarning.IncorrectUserId);
                        }
                        else if (string.IsNullOrEmpty(objUser.SessionId))
                        {
                            retStr = "SessionId missing";
                            retVal = Convert.ToInt32(ErrorCodeWarning.SessionIdRequired);
                        }
                        if (string.IsNullOrEmpty(retStr))
                        {
                            int userId = db.Users.FirstOrDefault(m => m.UniqueUserId == objUser.UserId).Id;
                            user = db.Users.Find(userId);
                            if (!string.IsNullOrEmpty(objUser.UserName))
                            {
                                user.UserName = objUser.UserName;
                            }
                            if (!string.IsNullOrEmpty(objUser.Password))
                            {
                                user.PasswordHash = objUser.Password;
                            }
                            if (!string.IsNullOrEmpty(objUser.FirstName))
                            {
                                user.FirstName = objUser.FirstName;
                            }
                            if (!string.IsNullOrEmpty(objUser.LastName))
                            {
                                user.LastName = objUser.LastName;
                            }
                            if (objUser.MobileNumber != 0)
                            {
                                user.MobileNumer = objUser.MobileNumber;
                            }
                            user.BirthDate = objUser.BirthDate;
                            if (!string.IsNullOrEmpty(objUser.Email))
                            {
                                user.Email = objUser.Email;
                            }
                            if (objUser.GenderId != null)
                            {
                                user.GenderId = objUser.GenderId;
                            }
                            if (objUser.AgeId != null)
                            {
                                user.AgeId = objUser.AgeId;
                            }
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();

                            //log.Info("User Data upadated in user Table");
                            logInfoUpdateUserInSqlDb = "User Data upadated in user Table";

                            retStr = "user details updated ";
                            retType = ReturnCode.Success.ToString();
                            retVal = Convert.ToInt32(ReturnCode.UpdateUserSuccess);


                            objUpdateDb.UpdateUser(objUser.UserName, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);

                            retVal = Convert.ToInt32(ReturnCode.UpdateUserSuccess);
                            retType = ReturnCode.Success.ToString();
                            retStr = "user details updated ";
                            logInfoUpdateUserInMySqlDb = "user details updated ";
                            dbContextTransaction.Commit();
                            //log.Info("User data commited successfully");

                        }
                    }
                    else
                    {
                        retVal = Convert.ToInt32(ReturnCode.Warning);
                        retType = ReturnCode.Warning.ToString();
                        retStr = "Invalid SessionId";
                    }
                    if (debugStatus == DebugMode.on.ToString())
                    {
                        string logMsg = string.Concat(logInfoUpdateUser, logInfoUpdateUserInSqlDb, logInfoUpdateUserInMySqlDb, retStr);
                        log.Info(retStr);
                    }
                }

                catch (Exception ex)
                {
                    //log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    retVal = Convert.ToInt32(ReturnCode.Failure);
                    retType = ReturnCode.Warning.ToString();
                    retStr = "some problem occured";
                    //log.Error(ex.Message);
                    if (debugStatus == DebugMode.off.ToString())
                    {
                        log.Error(retStr);
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

            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(pingList), Encoding.UTF8, "application/json")
            };
        }


        /// <summary>
        /// Check the session Authorize to allow the particular User or not
        /// </summary>
        /// <returns></returns>
        public bool IsAuthorize(string SessionId)
        {
            log.Info("inside IsAuthorize method");
            bool retval;
            try
            {
                if (db.UserSession.Any(m => m.SessionId == SessionId))
                {
                    retval = true;
                }
                else
                {
                    retval = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                retval = false;
            }
            return retval;
        }
    }
}
