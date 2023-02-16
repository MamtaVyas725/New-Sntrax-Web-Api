using EnttlOrchestrationLayer.Utilities;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;

namespace SntraxWebAPI.Repository
{
    public static class Repo
    {
        public static string connStringPrimary;
        public static string connStringSecondary;
        public static SqlConnection m_SqlConn;

        // Method for converting XML data to JSON
      
        public static DataTable GetDataTable(string connString, string procedureName, params SqlParameter[] commandParameters)
        {
            string methodName = "GetDataTable";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CLogger.LogInfo(methodName + " Started");
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Convert.ToInt32(AppConstants.DB_MAX_TIMEOUT); 
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger.LogInfo(methodName + " exception : " + ex.Message);
                throw new Exception(AppConstants.DATABASE);
            }
            finally
            {

                dt.Dispose();
            }
            stopwatch.Stop();
            CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);
            return dt;
        }

        public static DataSet GetDataSet(string connString, string procedureName, params SqlParameter[] commandParameters)
        {
            string methodName = "GetDataSet";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CLogger.LogInfo(methodName + " Started");
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = ( procedureName == AppConstants.SP_IN_SSD_GET_SHIPTORESULT || procedureName == AppConstants.SPGET_R4C_SNTRAX_ORCHS_SEARCH_BY_SN ||
                        procedureName == AppConstants.SP_INT_IBASE_GET_SN || procedureName == AppConstants.SP_IN_EIM_GET_SHIPRMA_DATA )
                        ? Convert.ToInt32(AppConstants.DB_MAX_TIMEOUT) : 0;
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                dataSet = null;
                CLogger.LogInfo(methodName + " exception : " + ex.Message);
                throw new Exception(AppConstants.DATABASE);
            }
            finally
            {
                if (dataSet != null)
                    dataSet.Dispose();
            }
            stopwatch.Stop();
            CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);
            return dataSet;
        }

        public static int ConnectToDB(string dbName, ref string dbConnectionName)
        {
            try
            {
                object connectionStr = "";
                dbConnectionName = "";
                if (dbName == AppConstants.DBPRIMARY)
                {
                    connectionStr = connStringPrimary;
                }
                else if (dbName == AppConstants.DBSECONDARY)
                {
                    connectionStr = connStringSecondary;
                }

                m_SqlConn = new SqlConnection(connectionStr.ToString());
                m_SqlConn.Open();
                dbConnectionName = connectionStr.ToString();
                m_SqlConn.Close();
                return 0;
            }
            catch (Exception eX)
            {
                m_SqlConn.Close();
               // clsSendMail sm = new clsSendMail("ConnectToDB", Environment.MachineName);
              //  sm.SendEmail(eX.Message.ToString());
                return -1;
            }
        }

        public static int ConnectToRetry(ref string strDB,string _dbRetry)
        {
            int returnCode = -1;
            int iRetry = int.Parse(_dbRetry);
            for (int i = 0; i < iRetry; i++)
            {
                returnCode = ConnectToDB(AppConstants.DBPRIMARY, ref strDB);
                if (returnCode == 0)
                    i = iRetry;
            }
            if (returnCode < 0)
            {
                for (int i = 0; i < iRetry; i++)
                {
                    returnCode = ConnectToDB(AppConstants.DBSECONDARY, ref strDB);
                    if (returnCode == 0)
                        i = iRetry;
                }
            }
            return returnCode;
        }

        // Method for converting XML data to JSON
        public static string XmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }
        // INSERT,UPDATE, DELETE
        public static void ExecuteNonQuery(string connString, string procedureName, params SqlParameter[] commandParameters)
        {
            //int result = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    cmd.ExecuteNonQuery();
                    //if (cmd.Parameters["@rowCount"].Value != null)
                    //    result = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
               // result = -1;
                throw new Exception(ex.ToString());
            }
            //return result;
        }

    }
}
