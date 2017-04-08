using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace CaptivePortal.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
       [HttpGet]
        public ActionResult LogsDownload()
        {
            try
            {
                
                    string path = Server.MapPath("~/Logs/log.txt");
                    System.IO.FileInfo file = new System.IO.FileInfo(path);
                    if (file.Exists)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                        return File(fileBytes, MediaTypeNames.Application.Octet, "log.txt");
                    }
               


            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


        }
    }
}
