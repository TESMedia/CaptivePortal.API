using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.Lib
{
    public class UpdateDb
    {
        private string myConnectionString;
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(RegisterDB));

        //int length;
        public UpdateDb()
        {
            myConnectionString = ConfigurationManager.ConnectionStrings["radiusConnectionString"].ToString();
            log.Info(myConnectionString);
        }

        public int UpdateUser(string userName, string UserPassword, string Email, string firstname, string lastname)
        {


            int retCode = 0;
            MySqlConnection myConnection = new MySqlConnection(myConnectionString);
            myConnection.Open();
            log.Info(myConnection.State);
            MySqlCommand myCommand = myConnection.CreateCommand();
            MySqlTransaction myTrans;

            // Start a local transaction
            myTrans = myConnection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            myCommand.Connection = myConnection;
            myCommand.Transaction = myTrans;
            try
            {
                myCommand.CommandText = "UPDATE radcheck SET username=" + '"' + userName + '"' + ", value=" + '"' + UserPassword + '"' + " WHERE username=" + '"' + userName + '"' + " ";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
                log.Info("Users records are written to database.");

            }



            catch (Exception e)
            {
                retCode = -1;
                try
                {
                    myTrans.Rollback();
                }
                catch (Exception ex)
                {
                    if (myTrans.Connection != null)
                    {

                        log.Info("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                log.Info("An exception of type " + e.GetType() + "was encountered while inserting the data.");
                log.Info("Neither record was written to database.");
            }
            finally
            {
                myConnection.Close();
            }
            return retCode;
        }
    }
}
