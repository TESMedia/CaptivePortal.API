using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CP.Lib
{
  public class CommunicateRTLS:IDisposable
    {

        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MacAddresses"></param>
        /// <returns></returns>
        public async Task<string> RegisterInRealTimeLocationService(string [] MacAddresses)
        {
            string resultContent = null;
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["RTLSBaseUrl"]);
                string strMacAddresses=JsonConvert.SerializeObject(MacAddresses);
                
                var result = await httpClient.PostAsync("RealTimeLocation/AddDevices", new StringContent(strMacAddresses, Encoding.UTF8, "application/x-www-form-urlencoded"));
                if (result.IsSuccessStatusCode)
                {
                    resultContent = await result.Content.ReadAsStringAsync();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return resultContent;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="MacAddresses"></param>
        /// <returns></returns>
        public async Task<string> DeregisterInRealTimeLocationServices(string[] MacAddresses)
        {
            string resultContent = null;
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["RTLSBaseUrl"]);
                string strMacAddresses = JsonConvert.SerializeObject(MacAddresses);

                var result = await httpClient.PostAsync("RealTimeLocation/DeRegisterDevices", new StringContent(strMacAddresses, Encoding.UTF8, "application/x-www-form-urlencoded"));
                if (result.IsSuccessStatusCode)
                {
                    resultContent = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultContent;
        }

     

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
