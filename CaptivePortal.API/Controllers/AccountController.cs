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

namespace CaptivePortal.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
       // private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private RegisterDB objRegisterDB = new RegisterDB();
        private ReturnModel ObjReturnModel = new ReturnModel();
        CPDBContext db = new CPDBContext();


        [HttpPost]
        [Route("Register")]
        public HttpResponseMessage Register(RegisterViewModel objRegisterModel)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Log.Info("enter in register");

                   

                    //save user data in user table
                    Users _objUser = new Users();
                   _objUser.UserName = objRegisterModel.UserName;
                    _objUser.CreationBy = objRegisterModel.CreationBy;
                    _objUser.CreationDate = System.DateTime.Now;
                    _objUser.UpdateDate = System.DateTime.Now;
                    _objUser.Password = objRegisterModel.UserPassword;
                    db.Users.Add(_objUser);

                    //save  user address in Address table
                    UsersAddress _objUserAddress = new UsersAddress();
                    _objUserAddress.Addresses = objRegisterModel.Addresses;
                    _objUserAddress.City = objRegisterModel.City;
                    _objUserAddress.State = objRegisterModel.State;
                    _objUserAddress.Country = objRegisterModel.Country;
                    _objUserAddress.Zip = objRegisterModel.Zip;
                    _objUserAddress.Notes = objRegisterModel.Notes;
                    db.UsersAddress.Add(_objUserAddress);
                    

                    ////save user site information in Site table
                    //Site _objSite = new Site();
                    //_objSite.SiteName = objRegisterModel.SiteName;
                    //_objSite.CompanyId = comanies.CompanyId;
                    
                    //db.Site.Add(_objSite);
                    db.SaveChanges();

                    objRegisterDB.CreateNewUser(objRegisterModel.UserName, objRegisterModel.UserPassword, objRegisterModel.Email);
                    ObjReturnModel.Id = 1;
                    ObjReturnModel.Message = "Success";
                    dbContextTransaction.Commit();
                    JavaScriptSerializer objSerializer = new JavaScriptSerializer();
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(objSerializer.Serialize(ObjReturnModel))
                    };
                }
                catch (Exception ex)
                {
                    // Log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }

        }

        [HttpPost]
        [Route("Login")]
        public HttpResponseMessage Login(LoginViewModel objLoginModel)
        {
            try
            {
                var args = new string[4];
                // args[0] = "122.166.202.201";
                args[0] = "192.168.1.15";
                args[1] = "testing123";
                args[2] = objLoginModel.UserName;
                args[3] = objLoginModel.UserPassword;

                if (args.Length != 4)
                {
                    Authenticate.ShowUsage();
                }

                try
                {
                    Authenticate.Authentication(args).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent("success")
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public HttpResponseMessage GetFile()
        {
            string localFilePath = HttpContext.Current.Server.MapPath("~/App_Data/log.txt");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "log.txt";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return response;
        }
    }
}
