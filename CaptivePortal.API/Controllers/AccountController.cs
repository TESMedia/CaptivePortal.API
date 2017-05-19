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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace CaptivePortal.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
                AutoLoginStatusReturn objAutoLoginReturn = new AutoLoginStatusReturn();
                try
                {
                    log.Info("enter in Register Method");
                    var objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);

                    if (!(IsAnyMacAddressExist(objUser)))
                    {
                        //Save all the users Data in SqlServer Global DataBase
                        objUser.CreationDate = DateTime.Now;
                        objUser.UpdateDate = DateTime.Now;
                        objUser.UserName = objUser.Email;
                        db.Users.Add(objUser);
                        db.SaveChanges();
                        log.Info("User Data saved in user Table");

                        //Save all the Users data in MySql DataBase
                        objRegisterDB.CreateNewUser(objUser.Email, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);


                        dbContextTransaction.Commit();

                            objAutoLoginReturn.UserName = objUser.Email;
                            objAutoLoginReturn.Password = objUser.Password;
                            objAutoLoginReturn.StatusReturn = new StatusReturn();
                            objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                        
                        

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
                    objAutoLoginReturn.StatusReturn.msg = "some problem occured";
                    
                }
                JavaScriptSerializer objSerialization = new JavaScriptSerializer();
                return new HttpResponseMessage()
                {
                    Content = new StringContent(objSerialization.Serialize(objAutoLoginReturn), Encoding.UTF8, "application/json")
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
                log.Info(model.MacAddress);
                log.Info(model.SiteId);
                //Check the MacAdress exist for the particular Site or not
                if (db.Users.Any(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId))
                {
                    log.Info("Check the MacAdress exist for the particular Site or not");
                    rtnMessage = true;
                    //check the MacAddress is exist or not for particular User then allow to update 
                    //if (db.Users.Any(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId && m.UserId == model.UserId))
                    //{
                    //    var objUser = db.Users.FirstOrDefault(m => m.MacAddress == model.MacAddress && m.UserId == model.UserId && m.SiteId == model.SiteId);
                    //    //If MacAddres Not exist for the User then Save the MacAddress
                    //    if (string.IsNullOrEmpty(objUser.MacAddress))
                    //    {
                    //        log.Info("If MacAddres Not exist for the User");
                    //        objUser.MacAddress = model.MacAddress;
                    //        db.Entry(objUser).State = System.Data.Entity.EntityState.Modified;
                    //        db.SaveChanges();
                    //    }
                    //}
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
            log.Info(rtnMessage);
            return rtnMessage;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("LoginWIthNewMacAddress")]
        public HttpResponseMessage LoginWIthNewMacAddress(Users model)
        {
            AutoLoginStatusReturn objAutoLoginReturn = new AutoLoginStatusReturn();
            try
            {
                log.Info(model.UserName);
                log.Info(model.Password);
                var _userDetails = db.Users.Where(m => m.UserName == model.UserName).FirstOrDefault();
                if (_userDetails.Password == model.Password)
                {
                    CreateUser(model);
                    objAutoLoginReturn.StatusReturn = new StatusReturn();
                    objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                    objAutoLoginReturn.UserName = model.UserName;
                    objAutoLoginReturn.Password = model.Password;

                }
                else
                {
                    objAutoLoginReturn.StatusReturn = new StatusReturn();
                    objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);

            }

            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objAutoLoginReturn), Encoding.UTF8, "application/json")
            };
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
            AutoLoginStatusReturn objAutoLoginReturn = new AutoLoginStatusReturn();
            try
            {

                var objSite = db.Site.FirstOrDefault(m => m.SiteId == model.SiteId);
                log.Info(objSite);
                //Need to check the MacAddress exist for the particular Site with Autologin true
                if (IsAnyMacAddressExist(model))
                {
                    log.Info("inside IsAnyMacAddressExist");
                    var objUser = db.Users.FirstOrDefault(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId);

                    //Check the AutoLogin of Site or User 
                    if (objUser.AutoLogin == true || objSite.AutoLogin == true)
                    {
                        log.Info("Check the AutoLogin of Site or User");

                        objAutoLoginReturn.StatusReturn = new StatusReturn();
                        objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Success);
                        objAutoLoginReturn.UserName = objUser.UserName;
                        objAutoLoginReturn.Password = objUser.Password;

                    }
                }
                else
                {
                    objAutoLoginReturn.StatusReturn = new StatusReturn();
                    objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                objAutoLoginReturn.StatusReturn.returncode = Convert.ToInt32(ReturnCode.Failure);
            }
            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerialization.Serialize(objAutoLoginReturn), Encoding.UTF8, "application/json")
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
        Success = 1,
        Failure = -1
    }
}

