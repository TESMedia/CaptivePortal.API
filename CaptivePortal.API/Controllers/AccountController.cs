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

namespace CaptivePortal.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AccountController));
        private RegisterDB objRegisterDB = new RegisterDB();
        private ReturnModel ObjReturnModel = new ReturnModel();
        CPDBContext db = new CPDBContext();
        StatusReturn objReturn = new StatusReturn();
        private string retStr="";
        private string retType="";
        private int retVal=0;


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("a8Captiveportal/V1/CreateUser")]
        public HttpResponseMessage CreateUserExpose(CreateUserViewModel objUser)
        {
            Site objSite = null;
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    //First check the Manadatory validation and show the Error Messages
                    if (string.IsNullOrEmpty(objUser.UserName))
                    {
                        retStr = "Need UserName for Registration";
                    }
                    else if (string.IsNullOrEmpty(objUser.Password))
                    {
                        retStr = "Need Password for Registration";
                    }
                    else if (objUser.SiteId == 0 )
                    {
                        retStr = "Need SiteId for Registration";
                    }
                    else if(!(db.Site.Any(m=>m.SiteId==objUser.SiteId)))
                    {
                        retStr = "This particular SiteId Not Exist Please try again with others";
                    }
                    else if(string.IsNullOrEmpty(objUser.UserId))
                    {
                        retStr = "Need UserId for Registration";
                    }

                    //if No validation Error then Insert the data into the table
                    if (string.IsNullOrEmpty(retStr))
                    {
                        objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);
                        //if Same User with Site Exist then don't allow
                        if (!(db.Users.Any(m => m.UserName == objUser.UserName && m.SiteId == objUser.SiteId)))
                        {
                            
                            Users objUsers = new Users();
                            objUsers.CreationDate = DateTime.Now;
                            objUsers.UpdateDate = DateTime.Now;
                            objUsers.UserName = objUser.UserName;
                            objUsers.FirstName = objUser.FirstName;
                            objUsers.LastName = objUser.LastName;
                            objUsers.Password = objUser.Password;
                            objUsers.SiteId = objUser.SiteId;
                            //objUsers.UniqueUserId = objUser.UserId;
                            objUsers.BirthDate = objUser.BirthDate;
                            objUsers.AgeId = objUser.AgeId;
                            objUsers.GenderId = objUser.GenderId;
                            objUsers.MobileNumer = objUser.MobileNumber;

                                                       
                            db.Users.Add(objUsers);

                            MacAddress objMacAddress = new MacAddress();
                            objMacAddress.UserId = objUsers.UserId;
                            //objMacAddress.MacAddressValue=objUser.ma
                            db.MacAddress.Add(objMacAddress);

                            db.SaveChanges();

                            log.Info("User Data saved in user Table");

                            //Save all the Users data in MySql DataBase
                            objRegisterDB.CreateNewUser(objUser.UserName, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);

                            retVal = Convert.ToInt32(ReturnCode.Success);
                            retType = ReturnCode.Success.ToString();
                            retStr = "Successfully Creted the User";
                            dbContextTransaction.Commit();

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
        [Route("a8Captiveportal/V1/CreateUserWifi")]
        public HttpResponseMessage CreateUserWifi(UserMacAddressDetails objUserMac)
        { 
           
            Site objSite = null;
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if(string.IsNullOrEmpty(objUserMac.objUser.UserName))
                    {
                        retStr = "Need UserName for Registration";  
                    }
                    else if(string.IsNullOrEmpty(objUserMac.objUser.Password))
                    {
                        retStr = "Need Password for Registration";
                    }
                    else if(objUserMac.objUser.SiteId==0)
                    {
                        retStr = "Please send the SiteId for Registrartion";
                    }

                    if (string.IsNullOrEmpty(retStr))
                    {

                        objSite = db.Site.FirstOrDefault(m => m.SiteId == objUserMac.objUser.SiteId);

                        //If macAdress is null then alllow to create without checking th null one
                        if ((string.IsNullOrEmpty(objUserMac.objMacAddress.MacAddressValue)))
                        {
                            objUserMac.objMacAddress.MacAddressValue = "default";
                        }

                        if (!(db.Users.Any(m => m.UserName == objUserMac.objUser.UserName && m.SiteId == objUserMac.objUser.SiteId)) && !(db.MacAddress.Any(m => m.MacAddressValue == objUserMac.objMacAddress.MacAddressValue)))
                        {
                            log.Info("Enter in Register Method");

                            if (objUserMac.objMacAddress.MacAddressValue == "default")
                            {
                                objUserMac.objMacAddress.MacAddressValue = null;
                            }

                            //Save the Users data into Users table
                            objUserMac.objUser.CreationDate = DateTime.Now;
                            objUserMac.objUser.UpdateDate = DateTime.Now;
                            var users=db.Users.Add(objUserMac.objUser);

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
        public HttpResponseMessage GetMacAddress(Users model)
        {
            ReturnMacAesddress objReturnMac = new ReturnMacAesddress();

            try
            {
                if (db.Users.Any(m => m.UserName == model.UserName && m.SiteId == model.SiteId))
                {
                    int UserId = db.Users.FirstOrDefault(m => m.UserId == model.UserId && m.SiteId == model.SiteId).UserId;
                    foreach (var item in db.MacAddress.Where(m => m.UserId ==UserId))
                    {
                        MacAddesses objMacAddress = new MacAddesses();
                        objMacAddress.MacAddress = item.MacAddressValue;
                        objReturnMac.lstMacAddresses.Add(objMacAddress);
                    }
                    retVal = Convert.ToInt32(ReturnCode.Success);
                    retStr = "Successfully return the MacAddresses";
                    retType = ReturnCode.Success.ToString();
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
        [Route("a8Captiveportal/V1/LoginWIthNewMacAddress")]
        public HttpResponseMessage LoginWIthNewMacAddress(LoginWIthNewMacAddressModel model)
        {
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
            lstMacAddresses = new List<MacAddesses>();
            StatusReturn = new StatusReturn();
        }
        public List<MacAddesses> lstMacAddresses { get; set; }
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

        public int MobileNumber { get; set;}

        public int ? GenderId { get; set; }

        public int ? AgeId { get; set; }

        public DateTime BirthDate { get; set; }

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
        public  Users objUser { get; set; }
        public MacAddress objMacAddress { get; set; }
    }

    public enum ReturnCode
    {
        Success = 1,
        Failure = -1,
        Warning = 2,
    }
}


