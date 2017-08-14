//using CaptivePortal.API.Models;
//using Facebook;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Web;
//using System.Web.Http.Cors;
//using System.Web.Mvc;
//using System.Web.Script.Serialization;
//using System.Web.Security;

//namespace CaptivePortal.API.Controllers
//{
//    [EnableCors(origins: "*", headers: "*", methods: "*")]
//    public class ExternalLoginController : Controller
//    {
        
//        public ActionResult Index()
//        {  
//            return View();
//        }
//        private Uri RedirectUri
//        {
//            get
//            {
//                var uriBuilder = new UriBuilder(Request.Url);
//                uriBuilder.Query = null;
//                uriBuilder.Fragment = null;
//                uriBuilder.Path = Url.Action("FacebookCallback");
//                return uriBuilder.Uri;
//            }
//        }

//        [AllowAnonymous]
//        public ActionResult login()
//        {
//            return View();
//        }

//        public ActionResult logout()
//        {
//            FormsAuthentication.SignOut();
//            return View("Login");
//        }

//        [AllowAnonymous]
//        public ActionResult Facebook()
//        {
//            var fb = new FacebookClient();
//            var loginUrl = fb.GetLoginUrl(new
//            {
//                //client_id = "275827382889134",
//                //client_secret = "25b0438224195ebd72cf2cd859f70bfc",
//                client_id = "1426387114107357",
//                client_secret = "fd043e87f6d0b8c0051b729e832dcb65",
//                redirect_uri = RedirectUri.AbsoluteUri,
//                response_type = "code",
//                scope = "email" // Add other permissions as needed
//            });

//            return Redirect(loginUrl.AbsoluteUri);
//        }
//        [HttpGet]
//        public HttpResponseMessage FacebookCallback(string code)
//        {
//            List<string> userlist = new List<string>();
//            var fb = new FacebookClient();
//            dynamic result = fb.Post("oauth/access_token", new
//            {
//                //client_id = "275827382889134",
//                //client_secret = "25b0438224195ebd72cf2cd859f70bfc",
//                client_id = "1426387114107357",
//                client_secret = "fd043e87f6d0b8c0051b729e832dcb65",
//                redirect_uri = RedirectUri.AbsoluteUri,
//                code = code
//            });

//            var accessToken = result.access_token;

//            // Store the access token in the session for farther use
//            Session["AccessToken"] = accessToken;

//            // update the facebook client with the access token so 
//            // we can make requests on behalf of the user
//            fb.AccessToken = accessToken;

//            // Get the user's information
//            dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
//            userlist.Add(me.email);
//            userlist.Add(me.first_name);
//            userlist.Add(me.middle_name);
//            userlist.Add(me.last_name);
            

//            // Set the auth cookie
//            FormsAuthentication.SetAuthCookie(me.email, false);
//            JavaScriptSerializer objSerialization = new JavaScriptSerializer();
//            return new HttpResponseMessage()
//            {
//                Content = new StringContent(objSerialization.Serialize(userlist), Encoding.UTF8, "application/json")
//            };
//        }
//    }
//}