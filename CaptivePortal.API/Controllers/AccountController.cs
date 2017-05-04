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

namespace CaptivePortal.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {   
        private static ILog Log { get; set;}
        ILog log =LogManager.GetLogger(typeof(AccountController));
        private RegisterDB objRegisterDB = new RegisterDB();
        private ReturnModel ObjReturnModel = new ReturnModel();
        CPDBContext db = new CPDBContext();


        [HttpPost]
        [Route("Register")]
        public HttpResponseMessage Register(Users objUser)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    log.Info("enter in Register Method");

                    if (!(db.Users.Any(m=>m.MacAddress==objUser.MacAddress && m.SiteId==objUser.SiteId)))
                    {
                        //Save all the users Data in SqlServer Global DataBase
                        objUser.CreationDate = DateTime.Now;
                        objUser.UpdateDate = DateTime.Now;
                        db.Users.Add(objUser);
                        
                        db.SaveChanges();
                        log.Info("User Data saved in user Table");

                        //Save all the Users data in MySql DataBase
                        objRegisterDB.CreateNewUser(objUser.Email, objUser.Password, objUser.Email, objUser.FirstName, objUser.LastName);

                       

                    }
                    dbContextTransaction.Commit();
                    return new HttpResponseMessage() {
                        Content = new StringContent("success")
                    };
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }

        }

        [HttpPost]
        [Route("Login")]
        public HttpResponseMessage Login(Users objUser)
        {
            try
            {
                var objSite = db.Site.FirstOrDefault(m => m.SiteId == objUser.SiteId);
                //check the particular UserName for a particular Site Is Exist or Not
                if (db.Users.Any(m=>m.UserName==objUser.UserName && m.SiteId==objUser.SiteId))
                {
                    log.Info("enter into if con..");
                   
                    var User = db.Users.FirstOrDefault(m => m.UserName == objUser.UserName && m.SiteId == objUser.SiteId);
                    //if Exist then check the MacAddress is there or not
                    //if(string.IsNullOrEmpty(User.MacAddress))
                    //{
                    //    User.MacAddress = objUser.MacAddress;
                    //    db.Entry(User).State = System.Data.Entity.EntityState.Modified;

                        //Need to check the MacAddress exist with Autologin of User true or AutoLogin of Site true
                        if (db.Users.Any(m => (m.MacAddress == objUser.MacAddress && m.SiteId == objUser.SiteId) && (m.AutoLogin == true || objSite.AutoLogin == true)))
                        {
                        log.Info("enter into if con2..");
                        Uri URI = new Uri(User.Site.ControllerIpAddress);
                            log.Info(URI);
                            using (WebClient client = new WebClient())
                            {
                                System.Collections.Specialized.NameValueCollection postData =
                                    new System.Collections.Specialized.NameValueCollection()
                                   {
                                      { "userid", User.UserName },
                                      { "password", User.Password },
                                   };
                                string pagesource = Encoding.UTF8.GetString(client.UploadValues(URI.AbsoluteUri, postData));
                                log.Info(pagesource);
                            }
                        }
                    //}

                }
                //Then Users with this UserName for a particular Site not exist so need to Register first
                else
                {

                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent("success")
                };
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }




    }



}
