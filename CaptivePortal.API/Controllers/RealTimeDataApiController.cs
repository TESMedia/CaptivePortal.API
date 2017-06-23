using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using CaptivePortal.API.Context;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace CaptivePortal.API.Controllers
{
    [RoutePrefix("RealTimeDataApi")]
    public class RealTimeDataApiController : ApiController
    {
        private DbContext db = new DbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SitedId"></param>
        /// <returns></returns>
       [HttpPost]
       [Route("GetAnalysisData")]
       public HttpResponseMessage GetAnalysisDataForDashboard(RequestModel model)
        {
            ReturnContainer objContainer = new ReturnContainer();
            JavaScriptSerializer objSerializer = new JavaScriptSerializer();
            try
            {
                DateTime ToCurrentDateTime = DateTime.Now.ToLocalTime();
                DateTime FromDateTime = DateTime.Now.ToLocalTime();

                if (model.searchCategory == Convert.ToInt32(Searchcriteria.CurrentPeriod))
                {
                    FromDateTime = ToCurrentDateTime.AddHours(-24);
                }
                else if (model.searchCategory == Convert.ToInt32(Searchcriteria.CurrentWeek))
                {
                    FromDateTime = ToCurrentDateTime.AddHours(-(24 * 7));
                }
                else if (model.searchCategory == Convert.ToInt32(Searchcriteria.CurrentMonth))
                {
                    FromDateTime = ToCurrentDateTime.AddHours(-(24 * 30));
                }
                else if (model.searchCategory == Convert.ToInt32(Searchcriteria.CurrentYear))
                {
                    FromDateTime = ToCurrentDateTime.AddHours(-(24 * 365));
                }

                db.Database.Connection.Open();
                objContainer.ResultAreaGraph = db.Database.SqlQuery<ReturnCurrentAreaGraphData>("exec GetAnalysisDataNew @SiteId,@SiteName,@SearchCategory,@FromDateTime,@ToDateTime", new SqlParameter("@SiteId", model.SitedId), new SqlParameter("@SiteName", model.SiteName), new SqlParameter("@SearchCategory", model.searchCategory), new SqlParameter("@FromDateTime", FromDateTime), new SqlParameter("@ToDateTime", ToCurrentDateTime)).ToList();
                objContainer.Result = db.Database.SqlQuery<ReturnModel>("exec GetAnalysisDataAsPerSite_New @SiteId,@SiteName,@SearchCategory,@FromDateTime,@ToDateTime", new SqlParameter("@SiteId", model.SitedId), new SqlParameter("@SiteName", model.SiteName), new SqlParameter("@SearchCategory", model.searchCategory), new SqlParameter("@FromDateTime", FromDateTime), new SqlParameter("@ToDateTime", ToCurrentDateTime)).ToList();
                db.Database.Connection.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
            return new HttpResponseMessage()
            {
                Content = new StringContent(objSerializer.Serialize(objContainer), Encoding.UTF8, "application/json")
            };
        }
    }

    public class ReturnModel
    {
        public Int64 TotalUsers { get; set; }

        public Int64 TotalSessions { get; set; }

        public Int64 MaleUsers { get; set; }

        public Int64 FemaleUsers { get; set; }

        public Int64 RegisteredUsers { get; set; }

        public Int64 NoOfAndriodUsers { get; set; }

        public Int64 NoOfWindowUsers { get; set;}
        public Int64 NoOfIosUsers { get; set; }

        public Int64 NoOfOthersUsers { get; set; }

        public Decimal PercentageMale { get; set; }

        public Decimal PercentageFemale { get; set; }

        public Int64 NetworkUsageUp { get; set; }

        public Int64 NetWorkUsageDown { get; set; }

    }

    public class ReturnCurrentAreaGraphData
    {
        public String Name { get; set; }

        public int Value { get; set; }
    }

    public class ReturnContainer
    {
        public List<ReturnModel> Result { get; set; }

        public List<ReturnCurrentAreaGraphData> ResultAreaGraph { get; set; }
    }
    public class RequestModel
    {
      public  int SitedId { get; set; }
       public string SiteName { get; set; }
      public  int searchCategory { get; set;}
    }

    public enum Searchcriteria
    {
        CurrentPeriod=0,
        CurrentWeek=1,
        CurrentMonth=2,
        CurrentYear=3
    }
}
