﻿using System;
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
        CPDBContext db = new CPDBContext();
        StatusReturn objReturn = new StatusReturn();
        private string retStr = "";
        private string retType = "";
        private int retVal = 0;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/Login")]
        public HttpResponseMessage Login(Users objUser)
        {
            log.Info("Enter in a8Captiveportal/V1/Login");
            string sessionId = null;
            try
            {
                if (objUser.UserName.ToLower() == "testpurple@mail.com")
                {
                    if (db.Users.Any(m => m.UserName.ToLower() == objUser.UserName.ToLower() && m.Password == objUser.Password))
                    {
                        int UserId = db.Users.FirstOrDefault(m => m.UserName == objUser.UserName).UserId;
                        SessionIDManager manager = new SessionIDManager();
                        sessionId = manager.CreateSessionID(HttpContext.Current);
                        //Insert the UserSession data with SessionId
                        if (db.UserSession.Any(m => m.UserId == UserId))
                        {
                            log.Info("Insert the UserSession data with SessionId");
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

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                retStr = "Some Error Occured";
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retType = ReturnCode.Failure.ToString();
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
            log.Info("Enter in a8Captiveportal/V1/CreateUser");
            Site objSite = null;
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
                        log.Info("if No validation Error then Insert the data into the table");
                        objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);
                        //if Same User with Site Exist then don't allow
                        if (!(db.Users.Any(m => m.UserName == objUser.UserName && m.SiteId == objUser.SiteId)))
                        {

                            if (IsAuthorize(objUser.SessionId))
                            {
                                log.Info("Checked User is authorized."); log.Info("Checked User is authorized.");
                                Users objUsers = new Users();
                                objUsers.CreationDate = DateTime.Now;
                                objUsers.UpdateDate = DateTime.Now;
                                objUsers.UserName = objUser.UserName;
                                objUsers.FirstName = objUser.FirstName;
                                objUsers.LastName = objUser.LastName;
                                objUsers.Password = objUser.Password;
                                objUsers.SiteId = objUser.SiteId;
                                objUsers.UniqueUserId = objUser.UserId;
                                objUsers.BirthDate = objUser.BirthDate;
                                objUsers.AgeId = objUser.AgeId;
                                objUsers.GenderId = objUser.GenderId;
                                objUsers.MobileNumer = objUser.MobileNumber;


                                db.Users.Add(objUsers);

                                //MacAddress objMacAddress = new MacAddress();
                                //objMacAddress.UserId = objUsers.UserId;
                                ////objMacAddress.MacAddressValue=objUser.ma
                                //db.MacAddress.Add(objMacAddress);

                                db.SaveChanges();

                                log.Info("User Data saved in user Table");

                                //Save all the Users data in MySql DataBase
                                objRegisterDB.CreateNewUser(objUser.UserName, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);

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
                    //else
                    //{
                    //    retType = ReturnCode.Warning.ToString();
                    //    retVal = Convert.ToInt32(ReturnCode.Warning);
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    retVal = Convert.ToInt32(ReturnCode.Failure);
                    retType = ReturnCode.Warning.ToString();
                    retStr = "Error Occured";
                    log.Error(ex.Message);
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/CreateUserWifi")]
        public HttpResponseMessage CreateUserWifi(UserMacAddressDetails objUserMac)
        {
            log.Info("Inside in a8Captiveportal/V1/CreateUserWifi");
            Site objSite = null;
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (string.IsNullOrEmpty(objUserMac.objUser.UserName))
                    {
                        retStr = "Need UserName for Registration";
                    }
                    else if (string.IsNullOrEmpty(objUserMac.objUser.Password))
                    {
                        retStr = "Need Password for Registration";
                    }
                    else if (objUserMac.objUser.SiteId == 0)
                    {
                        retStr = "Please send the SiteId for Registrartion";
                    }

                    if (string.IsNullOrEmpty(retStr))
                    {

                        objSite = db.Site.FirstOrDefault(m => m.SiteId == objUserMac.objUser.SiteId);

                        //If macAdress is null then allow to create without checking the null one
                        if ((string.IsNullOrEmpty(objUserMac.objMacAddress.MacAddressValue)))
                        {
                            log.Info("Allowing macaddress if it is null with Default value ");
                            objUserMac.objMacAddress.MacAddressValue = "default";
                        }

                        if (!(db.Users.Any(m => m.UserName == objUserMac.objUser.UserName && m.SiteId == objUserMac.objUser.SiteId)) && !(db.MacAddress.Any(m => m.MacAddressValue == objUserMac.objMacAddress.MacAddressValue)))
                        {


                            if (objUserMac.objMacAddress.MacAddressValue == "default")
                            {
                                objUserMac.objMacAddress.MacAddressValue = null;
                            }

                            //Save the Users data into Users table
                            objUserMac.objUser.CreationDate = DateTime.Now;
                            objUserMac.objUser.UpdateDate = DateTime.Now;
                            var users = db.Users.Add(objUserMac.objUser);

                            objUserMac.objMacAddress.UserId = objUserMac.objUser.UserId;
                            db.MacAddress.Add(objUserMac.objMacAddress);
                            db.SaveChanges();

                            log.Info("User Data saved in user Table");

                            //Save all the Users data in MySql DataBase
                            objRegisterDB.CreateNewUser(objUserMac.objUser.UserName, objUserMac.objUser.Password, objUserMac.objUser.Email, objUserMac.objUser.FirstName, objUserMac.objUser.LastName);

                            retVal = Convert.ToInt32(ReturnCode.Success);
                            retType = ReturnCode.Success.ToString();
                            retStr = "Successfully Creted the User";
                            dbContextTransaction.Commit();
                            log.Info("user data saved and commited successfully");

                        }
                        else
                        {
                            retVal = Convert.ToInt32(ReturnCode.Warning);
                            retStr = "MacAddress or UserName already exist for same site named" + " " + objSite.SiteName;
                            retType = ReturnCode.Warning.ToString();
                        }
                    }
                    else
                    {
                        retType = ReturnCode.Warning.ToString();
                        retVal = Convert.ToInt32(ReturnCode.Warning);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    retVal = Convert.ToInt32(ReturnCode.Failure);
                    retType = ReturnCode.Warning.ToString();
                    retStr = "some problem occured";
                    log.Error(ex.Message);
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/GetMacAddresses")]
        public HttpResponseMessage GetMacAddress(CreateUserViewModel model)
        {
            //First check the Manadatory 
            //validation and show the Error Messages
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
                            int UserId = db.Users.FirstOrDefault(m => m.UniqueUserId == model.UserId && m.SiteId == model.SiteId).UserId;
                            foreach (var item in db.MacAddress.Where(m => m.UserId == UserId))
                            {
                                MacAddesses objMacAddress = new MacAddesses();
                                objMacAddress.MacAddress = item.MacAddressValue;
                                objReturnMac.MacAddressList.Add(objMacAddress);
                            }
                            retVal = Convert.ToInt32(ReturnCode.GetMacAddressSuccess);
                            retStr = "Successfully return the MacAddresses";
                            log.Info("Successfully return the MacAddresses");
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

                        retVal = Convert.ToInt32(ReturnCode.Warning);
                        retType = ReturnCode.Warning.ToString();
                        retStr = "Invalid SessionId" + " " + model.SessionId;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retStr = "Error Occured";
                retType = ReturnCode.Failure.ToString();
            }

            objReturnMac.StatusReturn.returncode = retVal;
            objReturnMac.StatusReturn.msg = retStr;
            objReturnMac.StatusReturn.type = retType;

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
        [HttpPost]
        [Route("a8Captiveportal/V1/DeleteUser")]
        public HttpResponseMessage DeleteWiFiUser(CreateUserViewModel model)
        {
            log.Info("inside a8Captiveportal/V1/DeleteUser");

            //First check the Manadatory 
            //validation and show the Error Messages
            if (string.IsNullOrEmpty(model.UserId))
            {
                retStr = "Need UserId to delete Mac address";
            }
            else if (model.SiteId == 0)
            {
                retStr = "Need SiteId to delete Mac address";
            }
            else if (!(db.Site.Any(m => m.SiteId == model.SiteId)))
            {
                retStr = "This particular SiteId Not Exist Please try again with others";
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
                            db.Users.Remove(user);
                            db.SaveChanges();
                        retStr = "User is deleted with its associated mac address";
                        retVal = Convert.ToInt32(ReturnCode.Success);
                        retType = ReturnCode.Success.ToString();
                    }
                        else
                        {
                            retStr = "User is not found";
                            retVal = Convert.ToInt32(ReturnCode.Failure);
                            retType = ReturnCode.Failure.ToString();
                        }
                    }
                    else
                    {
                        retVal = Convert.ToInt32(ReturnCode.Warning);
                        retType = ReturnCode.Warning.ToString();
                        retStr = "Not Authorize with Sessions" + " " + model.SessionId;
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
            }
            catch (Exception ex)
            {
                log.Error(ex);
                retVal = Convert.ToInt32(ReturnCode.Failure);
                retStr = "Error Occured";
                retType = ReturnCode.Failure.ToString();
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/LoginWIthNewMacAddress")]
        public HttpResponseMessage LoginWIthNewMacAddress(LoginWIthNewMacAddressModel model)
        {
            log.Info(" inside a8Captiveportal/V1/LoginWIthNewMacAddress");
            StatusReturn objAutoLoginReturn = new StatusReturn();
            try
            {
                log.Info(model.UserName);
                log.Info(model.Password);

                if (db.Users.Any(m => m.UserName == model.UserName))
                {
                    //If User exist then Save the User in MacAddresses table
                    if (db.Users.Any(m => m.UserName == model.UserName && m.Password == model.Password))
                    {
                        log.Info("If User exist then Save the User in MacAddresses table");
                        int UserId = db.Users.Where(m => m.UserName == model.UserName).FirstOrDefault().UserId;
                        if (!(db.MacAddress.Any(m => m.MacAddressValue == model.MacAdress && m.UserId == UserId)))
                        {
                            MacAddress objMac = new MacAddress();
                            objMac.MacAddressValue = model.MacAdress;
                            objMac.UserId = UserId;
                            objMac.BrowserName = model.BrowserName;
                            objMac.UserAgentName = model.UserAgentName;
                            objMac.OperatingSystem = model.OperatingSystem;
                            objMac.IsMobile = model.IsMobile;
                            db.MacAddress.Add(objMac);
                            db.SaveChanges();

                            retStr = "Successfully add the Maccadress";
                            log.Info("Successfully add the Macaddress");
                            retVal = Convert.ToInt32(ReturnCode.Success);
                            retType = ReturnCode.Success.ToString();

                        }
                    }
                    //If Not exist then return User Not exist so try to register first then Login
                    else
                    {
                        retStr = "Please Check the Credential you have Entered";
                        retVal = Convert.ToInt32(ReturnCode.Success);
                        retType = ReturnCode.Success.ToString();
                    }
                }
                else
                {
                    retStr = "UserName Not exist Please Register First";
                    retVal = Convert.ToInt32(ReturnCode.Success);
                    retType = ReturnCode.Success.ToString();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                objReturn.returncode = Convert.ToInt32(ReturnCode.Failure);

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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("GetStatusOfUserForSite")]
        //public HttpResponseMessage GetStatusOfUserForSite(Users model)
        //{
        //    try
        //    { 
        //        var objSite = db.Site.FirstOrDefault(m => m.SiteId == model.SiteId);
        //        log.Info(objSite);

        //        //Need to check the MacAddress exist for the particular Site with Autologin true
        //        if (IsAnyMacAddressExist(model))
        //        {
        //            log.Info("inside IsAnyMacAddressExist");
        //            var objUser = db.Users.FirstOrDefault(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId);

        //            //Check the AutoLogin of Site or User 
        //            if (objUser.AutoLogin == true || objSite.AutoLogin == true)
        //            {
        //                log.Info("Check the AutoLogin of Site or User");

        //                objAutoLoginReturn.StatusReturn = new StatusReturn();
        //                objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Success);
        //                objAutoLoginReturn.UserName = objUser.UserName;
        //                objAutoLoginReturn.Password = objUser.Password;

        //            }
        //        }
        //        else
        //        {
        //            objAutoLoginReturn.StatusReturn = new StatusReturn();
        //            objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
        //    }
        //    JavaScriptSerializer objSerialization = new JavaScriptSerializer();
        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(objSerialization.Serialize(objAutoLoginReturn), Encoding.UTF8, "application/json")
        //    };
        //}
    }


    public class StatusReturn
    {
        public int returncode { get; set; }
        public string msg { get; set; }
        public string type { get; set; }
    }

    public class MacAddesses
    {
        public string MacAddress { get; set; }
    }

    public class ReturnMacAesddress
    {
        public ReturnMacAesddress()
        {
            MacAddressList = new List<MacAddesses>();
            StatusReturn = new StatusReturn();
        }
        public List<MacAddesses> MacAddressList { get; set; }
        public StatusReturn StatusReturn { get; set; }
    }
   
    public class CreateUserViewModel
    {
        public int SiteId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool AutoLogin { get; set; }

        public int MobileNumber { get; set; }

        public int? GenderId { get; set; }

        public int? AgeId { get; set; }

        public DateTime BirthDate { get; set; }

        public string SessionId { get; set; }


    }
    public class LoginWIthNewMacAddressModel
    {
        public string UserName { get; set; }

        public string MacAdress { get; set; }

        public string Password { get; set; }

        public string BrowserName { get; set; }
        public string UserAgentName { get; set; }

        public string OperatingSystem { get; set; }

        public bool IsMobile { get; set; }

    }

    public class UserMacAddressDetails
    {
        public Users objUser { get; set; }
        public MacAddress objMacAddress { get; set; }
    }

    public enum ReturnCode
    {
        Success = 1,
        LoginSuccess = 202,
        CreateUserSuccess = 204,
        GetMacAddressSuccess = 206,
        Failure = 511,
        Warning = HttpStatusCode.Found
    }


    public enum ErrorCodeWarning
    {
        IncorrectPassword = 310,
        usernameisnotexist = 311,
        UserNameRequired = 312,
        PasswordRequired = 313,
        SiteIDRequired = 314,
        SiteIdNotExist = 315,
        UserIdRequired = 316,
        SessionIdRequired = 317,
        UserUniqueIdAlreadyExist = 318,
        MacAddressorUserNameExist = 319,
        NonAuthorize = 320
    }
}


