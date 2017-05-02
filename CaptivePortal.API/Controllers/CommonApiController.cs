using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CaptivePortal.API.Models;
using CaptivePortal.API.Context;
using System.Text;

namespace CaptivePortal.API.Controllers
{
    public class CommonApiController : ApiController
    {
        private CPDBContext db = new CPDBContext();
        public HttpResponseMessage GetStatus(RequestViewModel model)
        {
            try
            {
                var objSite = db.Site.FirstOrDefault(m => m.SiteId == model.SiteId);
                //Need to check the MacAddress exist for the particular Site with Autologin true
                if (db.Users.Any(m => (m.MacAddress == model.MacAddress && m.SiteId == model.SiteId) && (m.AutoLogin == true || objSite.AutoLogin == true)))
                {
                    //If the user exist then we need to try
                    var User = db.Users.FirstOrDefault(m => m.MacAddress == model.MacAddress && m.SiteId == model.SiteId);

                    string URI = string.Concat(User.Site.ControllerIpAddress, "/vpn/loginUser");

                    using (WebClient client = new WebClient())
                    {
                        System.Collections.Specialized.NameValueCollection postData =
                            new System.Collections.Specialized.NameValueCollection()
                           {
                                  { "userid", User.UserName },
                                  { "password", User.Password },
                           };
                        string pagesource = Encoding.UTF8.GetString(client.UploadValues(URI, postData));
                    }
                }
                return new HttpResponseMessage()
                {
                    Content = new StringContent("")
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
