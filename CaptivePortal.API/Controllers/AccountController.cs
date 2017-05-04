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
using System.Collections;

namespace CaptivePortal.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AccountController));
        private RegisterDB objRegisterDB = new RegisterDB();
        private ReturnModel ObjReturnModel = new ReturnModel();
        CPDBContext db = new CPDBContext();
        StatusReturn objReturn = new StatusReturn();


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="objUser"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Register")]
        //public HttpResponseMessage Register(Users objUser)
        //{
        //    using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
        //    {
        //        try
        //        {
        //            log.Info("enter in Register Method");
        //            var objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);

        //            if (!(db.Users.Any(m => m.MacAddress == objUser.MacAddress && m.SiteId == objUser.SiteId)))
        //            {
        //                //Save all the users Data in SqlServer Global DataBase
        //                db.Users.Add(objUser);
        //                db.SaveChanges();
        //                log.Info("User Data saved in user Table");

        //                //Save all the Users data in MySql DataBase
        //                objRegisterDB.CreateNewUser(objUser.Email, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);

        //                //Need to check the MacAddress exist with Autologin of User true or AutoLogin of Site true
        //                if (db.Users.Any(m => (m.MacAddress == objUser.MacAddress && m.SiteId == objUser.SiteId) && (m.AutoLogin == true || objSite.AutoLogin == true)))
        //                {
        //                    //If the user Not exist then we need to try
        //                    var User = db.Users.FirstOrDefault(m => m.MacAddress == objUser.MacAddress && m.SiteId == objUser.SiteId);

        //                    string URI = string.Concat(User.Site.ControllerIpAddress, "/vpn/loginUser");

        //                    using (WebClient client = new WebClient())
        //                    {
        //                        System.Collections.Specialized.NameValueCollection postData =
        //                            new System.Collections.Specialized.NameValueCollection()
        //                           {
        //                              { "userid", User.UserName },
        //                              { "password", User.Password },
        //                           };
        //                        string pagesource = Encoding.UTF8.GetString(client.UploadValues(URI, postData));
        //                    }
        //                }

        //            }

        //            return new HttpResponseMessage() {
        //                Content = new StringContent("")
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error(ex.Message);
        //            dbContextTransaction.Rollback();
        //            throw ex;
        //        }
        //    }

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="objUser"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Login")]
        //public HttpResponseMessage Login(Users objUser)
        //{
        //    try
        //    {
        //        var objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);
        //        //check the particular UserName for a particular Site Is Exist or Not
        //        if (db.Users.Any(m => m.UserName == objUser.UserName && m.SiteId == objUser.SiteId))
        //        {

        //            var User = db.Users.FirstOrDefault(m => m.UserName == objUser.UserName && m.SiteId == objUser.SiteId);
        //            //if Exist then check the MacAddress is there or not
        //            if (string.IsNullOrEmpty(User.MacAddress))
        //            {
        //                User.MacAddress = objUser.MacAddress;
        //                db.Entry(User).State = System.Data.Entity.EntityState.Modified;

        //                //Need to check the MacAddress exist with Autologin of User true or AutoLogin of Site true
        //                if (db.Users.Any(m => (m.MacAddress == objUser.MacAddress && m.SiteId == objUser.SiteId) && (m.AutoLogin == true || objSite.AutoLogin == true)))
        //                {
        //                    string URI = string.Concat(User.Site.ControllerIpAddress, "/vpn/loginUser");

        //                    using (WebClient client = new WebClient())
        //                    {
        //                        System.Collections.Specialized.NameValueCollection postData =
        //                            new System.Collections.Specialized.NameValueCollection()
        //                           {
        //                              { "userid", User.UserName },
        //                              { "password", User.Password },
        //                           };
        //                        string pagesource = Encoding.UTF8.GetString(client.UploadValues(URI, postData));
        //                    }
        //                }
        //            }

        //        }
        //        //Then Users with this UserName for a particular Site not exist so need to Register first
        //        else
        //        {

        //        }

        //        return new HttpResponseMessage()
        //        {
        //            Content = new StringContent("success")
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateUser")]
        public HttpResponseMessage CreateUser(Users objUser)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    log.Info("enter in Register Method");
                    var objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);

                    if (!(IsAnyMacAddressExist(objUser)))
                    {
                        //Save all the users Data in SqlServer Global DataBase
                        db.Users.Add(objUser);
                        db.SaveChanges();
                        log.Info("User Data saved in user Table");

                        //Save all the Users data in MySql DataBase
                        objRegisterDB.CreateNewUser(objUser.Email, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);
                        objReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                        objReturn.msg = "Successfully Creted the User";

                    }
                }
                catch (Exception ex)
                {
                    objReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
                    objReturn.msg = "some problem occured";
                    log.Error(ex.Message);
                }
                JavaScriptSerializer objSerialization = new JavaScriptSerializer();
                return new HttpResponseMessage()
                {
                    Content = new StringContent(objSerialization.Serialize(objReturn))
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMacAddress")]
        public HttpResponseMessage GetMacAddress(Users model)
        {
            try
            {
                IEnumerable lstMacAddress;
                if (IsAnyMacAddressExist(model))
                {
                    lstMacAddress = db.Users.Where(m => m.UserName == model.UserName && m.SiteId == model.SiteId).Select(m => new { mac = m.MacAddress });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objReturn))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsAnyMacAddressExist(Users model)
        {
            bool rtnMessage;
            try
            {
                //Check the MacAdress exist for the particular Site or not
                if (db.Users.Any(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId))
                {
                    rtnMessage = true;
                    //check the MacAddress is exist or not for particular User then allow to update 
                    if (db.Users.Any(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId && m.UserId == model.UserId))
                    {
                        var objUser = db.Users.FirstOrDefault(m => m.MacAddress == model.MacAddress && m.UserId == model.UserId && m.SiteId == model.SiteId);
                        //If MacAddres Not exist for the User then Save the MacAddress
                        if (string.IsNullOrEmpty(objUser.MacAddress))
                        {
                            objUser.MacAddress = model.MacAddress;
                            db.Entry(objUser).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    rtnMessage = false;
                }
            }
            catch (Exception ex)
            {
                rtnMessage = false;
            }
            return rtnMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetStatusOfUserForSite")]
        public HttpResponseMessage GetStatusOfUserForSite(Users model)
        {
            try
            {
                AutoLoginStatusReturn objReturn = new AutoLoginStatusReturn();
                var objSite = db.Site.FirstOrDefault(m => m.SiteId == model.SiteId);
                //Need to check the MacAddress exist for the particular Site with Autologin true
                if (IsAnyMacAddressExist(model))
                {
                    var objUser = db.Users.FirstOrDefault(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId);

                    //Check the AutoLogin of Site or User 
                    if (objUser.AutoLogin == true || objSite.AutoLogin == true)
                    {
                        objReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                        objReturn.UserName = objUser.UserName;
                        objReturn.Password = objUser.Password;
                    }
                }
            }
            catch (Exception ex)
            {
                objReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
            }
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objReturn))
            };
        }
    }

    public class StatusReturn
    {
        public int returncode { get; set; }
        public string msg { get; set; }
    }

    public class AutoLoginStatusReturn
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public StatusReturn StatusReturn { get; set; }
    }

    public enum ReturnCode
    {
        Success = 0,
        Failure = 1
    }
}

