using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.ViewModels
{
    public class Notification
    {
        public Notification()
        {
            result = new Result();
        }
        //public static bool IsSuccess { get; set; }
        public Result result { get; set; }
    }
    public class Result
    {
        public int returncode { get; set; }
        public string errmsg { get; set; }
    }
}