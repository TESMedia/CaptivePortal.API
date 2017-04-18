using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CaptivePortal.API.Controllers
{
    public class GenerateHtmlCodeController : Controller
    {

        CPDBContext db = new CPDBContext();
        [HttpPost]
        public string GenerateHtmlForLogin(GeneratingHtmlViewModel objHtmlViewModel)
        {
            

            string data = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(objHtmlViewModel.LoginUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            return data;
        }


        [HttpPost]
        public string GenerateHtmlForLoginTest(GeneratingHtmlViewModel htmlViewModel)
        {
            var formResult = db.Form.Where(m => m.SiteId == htmlViewModel.SiteId).ToList();

            string html = null;
            HtmlWeb web = new HtmlWeb();
            string urlAddress = htmlViewModel.LoginUrl;
            HtmlDocument document = web.Load(urlAddress);

            var body = document.DocumentNode.Descendants()
                                .Where(n => n.Id == "usr_password")
                                .FirstOrDefault();
            
            var checkBoxValue = formResult[0].IsPasswordRequire;
            if (checkBoxValue == true)
            {
                body.Attributes.Add("style", "display:none");
             html = document.DocumentNode.OuterHtml;
            Form objForm = new Form
            {
                HtmlCodeForLogin= html,
                SiteId= htmlViewModel.SiteId
            };
            db.Form.Add(objForm);
            db.SaveChanges();
            }
            else
            {
                return html;
            }
            return html;
        }

        // GET: GenerateHtmlCode
        public ActionResult Index()
        {
            return View();
        }
    }
}