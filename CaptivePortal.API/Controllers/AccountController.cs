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
        public HttpResponseMessage Register(RegisterViewModel objRegisterModel)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    log.Info("enter in Register Method");
                    
                    //save user data in user table
                     Users _objUser = new Users();
                    _objUser.UserName = objRegisterModel.Email;
                    _objUser.FirstName = objRegisterModel.FirstName;
                    _objUser.LastName = objRegisterModel.LastName;
                    _objUser.Email = objRegisterModel.Email;
                    _objUser.CreationBy = objRegisterModel.CreationBy;
                    _objUser.CreationDate = System.DateTime.Now;
                    _objUser.UpdateDate = System.DateTime.Now;
                    _objUser.Password = objRegisterModel.UserPassword;
                    _objUser.Gender = objRegisterModel.Gender;
                    _objUser.Age = objRegisterModel.Age;
                    _objUser.Term_conditions = ConfigurationManager.AppSettings["Version"];
                    _objUser.promotional_email = objRegisterModel.promotional_email;
                    _objUser.AutoLogin = Convert.ToBoolean(objRegisterModel.AutoLogin);
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
                    log.Info("User Data saved in user Table");

                    objRegisterDB.CreateNewUser(objRegisterModel.Email, objRegisterModel.UserPassword, objRegisterModel.Email,objRegisterModel.FirstName,objRegisterModel.LastName);
                    ObjReturnModel.Id = 1;
                    ObjReturnModel.Message = "Success";
                    dbContextTransaction.Commit();
                    JavaScriptSerializer objSerializer = new JavaScriptSerializer();
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri("http://planetsbrainvm.cloudapp.net/login.aspx");
                    return response;
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }

        }

        //[HttpPost]
        //[Route("Login")]
        //public HttpResponseMessage Login(LoginViewModel objLoginModel)
        //{
        //    try
        //    {
        //        var args = new string[4];
        //        args[0] = "122.166.202.201";
        //       // args[0] = "192.168.1.12";
        //        args[1] = "testing123";
        //        args[2] = objLoginModel.UserName;
        //        args[3] = objLoginModel.UserPassword;

        //        if (args.Length != 4)
        //        {
        //            Authenticate.ShowUsage();
        //        }

        //        try
        //        {
        //            log.Info("enter login Authenticate()");
        //            Authenticate.Authentication(args).Wait();

        //        }
        //        catch (Exception e)
        //        {
        //            log.Error(e.Message);
        //            throw (e);


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




    }
           

    
}
