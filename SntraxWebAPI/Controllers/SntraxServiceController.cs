using EnttlOrchestrationLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SntraxWebAPI.Model;
using SntraxWebAPI.Repository;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Diagnostics.CodeAnalysis;
using SntraxWebAPI.Services;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using SntraxWebAPI.Utilities;

namespace SntraxWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SntraxServiceController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _rootObjectCommon;
        private string _constrPrimary = string.Empty;
        private string _constrSecondary = string.Empty;
        private string _dbRetry = string.Empty;
        private string _emailRecipent = string.Empty;
        private string _emailSender = string.Empty;
        private string _DNXML = string.Empty;
        private string _EIMRmaXML = string.Empty;
        private string _eIMRmaXML = string.Empty;
        private string _validate_SSD_CPU_ShipToResponseOuterXML = string.Empty;
        private string _iBaseGetSingleDataXML = string.Empty;
        private static int dbSuccess = -1;

        private SntraxService sntraxService;


        public SntraxServiceController()
        {
        }

        [ActivatorUtilitiesConstructor]
        public SntraxServiceController(IConfiguration rootObjectCommon, ILogger<SntraxServiceController> logger)
        {
            _logger = logger;
            _rootObjectCommon = rootObjectCommon;
            _constrPrimary = _rootObjectCommon.GetValue<string>("ConnectionStrings:ConstrPrimary");
            _constrSecondary = _rootObjectCommon.GetValue<string>("ConnectionStrings:ConstrSecondry");
            Repo.connStringPrimary = _constrPrimary;
            Repo.connStringSecondary = _constrSecondary;
            _dbRetry = _rootObjectCommon.GetValue<string>("EmailConfiguration:dbRetry");
            _emailRecipent = _rootObjectCommon.GetValue<string>("EmailConfiguration:emailRecipent");
            _emailSender = _rootObjectCommon.GetValue<string>("EmailConfiguration:emailSender");
            _DNXML = _rootObjectCommon.GetValue<string>("OuterXml:DNXML");
            _eIMRmaXML = _rootObjectCommon.GetValue<string>("OuterXml:EIMRmaXML");
            _iBaseGetSingleDataXML = _rootObjectCommon.GetValue<string>("OuterXML:IBaseGetSingleDataXML");
            _validate_SSD_CPU_ShipToResponseOuterXML = _rootObjectCommon.GetValue<string>("OuterXml:Validate_SSD_CPU_ShipToResponseXML");
            CLogger._logger = _logger;
            sntraxService = new SntraxService();
        }


        [HttpPost]
        [Consumes("application/xml")]
        [Route("IBaseGetDataByDN")]
        public string IBaseGetDataByDN(XmlDocument doc)
        {
            string methodName = "IBaseGetDataByDN";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string sDBName = string.Empty;
            string stringwriter = string.Empty;
            string FinalDNXml = string.Empty;
            var soapBody = doc.GetElementsByTagName("IBaseGetDataByDN")[0];
            string innerObject = soapBody.InnerXml;
            var myJsonResponse = Repo.XmlToJson(innerObject);
            IBaseGetDataByDN myDeserializedClass = JsonConvert.DeserializeObject<IBaseGetDataByDN>(myJsonResponse);
            var IBaseDataDNList = myDeserializedClass.list.DN.ToList();
            List<IBaseData> returnList = new List<IBaseData>();
            string dnString = "";

            List<IBaseData> IBaseData = new List<IBaseData>();
            if (IBaseDataDNList != null && IBaseDataDNList.Count > 0)
            {
                dnString = string.Join(",", IBaseDataDNList.Select(x => x.DN)).TrimEnd(',');
            }
            if (dnString != "")
            {
                try
                {
                    // Test DB Connection with Retry
                    dbSuccess = Repo.ConnectToRetry(ref sDBName, _dbRetry);
                    DataSet dataSet = new DataSet();
                    SqlParameter[] param = {
                           new SqlParameter("@param_dn",dnString),
                          };
                    dataSet = Repo.GetDataSet(sDBName, AppConstants.SP_INT_IBASE_GET_DN, param);
                    returnList = sntraxService.getIbaseData(dataSet, true);
                    stringwriter = sntraxService.Serialize(returnList);
                    FinalDNXml = string.Format(_DNXML, sntraxService.ReplaceXmlTag(stringwriter, "IBaseGetDataByDN"));
                }
                catch (Exception ex)
                {
                    CLogger.LogInfo(methodName + " exception : " + ex.Message);
                }
                stopwatch.Stop();
                CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);
            }
            return FinalDNXml;
        }

        [HttpPost]
        [Consumes("application/xml")]
        [Route("IBaseGetSingleData")]
        public string IBaseGetSingleData(XmlDocument doc)
        {
            string methodName = "IBaseGetSingleData";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string sDBName = string.Empty;
            string stringwriter = string.Empty;
            string FinalDNXml = string.Empty;            
            var soapBody = doc.GetElementsByTagName("IBaseGetSingleData")[0];
            string innerObject = soapBody.InnerText;
            string snString = Regex.Replace(innerObject, @"\s+", string.Empty);
        
            SntraxService sntraxService = new SntraxService();

            if (snString != "")
            {
                try
                {
                    // Test DB Connection with Retry
                    dbSuccess = Repo.ConnectToRetry(ref sDBName, _dbRetry);
                    DataSet dataSet = new DataSet();
                    SqlParameter[] param = {
                           new SqlParameter("@param_sn",snString),
                          };
                    dataSet = Repo.GetDataSet(sDBName, AppConstants.SP_INT_IBASE_GET_SN, param);
                    List<IBaseData> iBaseList = sntraxService.getIbaseData(dataSet, false);
                    stringwriter = sntraxService.Serialize(iBaseList);
                    FinalDNXml = string.Format(_iBaseGetSingleDataXML, sntraxService.ReplaceXmlTag(stringwriter, "IBaseGetSingleData"));
                }
                catch (Exception ex)
                {
                    CLogger.LogInfo(methodName + " exception : " + ex.Message);
                }
                stopwatch.Stop();
                CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);
            }
            return FinalDNXml;
        }

        [HttpPost]
        [Consumes("application/xml")]
        [Route("Validate_SSD_CPU_ShipTo")]
        [ResponseCache(Duration = 30)]
        public string Validate_SSD_CPU_ShipTo(string RequestType, string RequestValue)
        {
            ShipToResult result = new ShipToResult();
            string sDBName = string.Empty;
            string stringwriter = string.Empty;
            string FinalDNXml = string.Empty;

            try
            {
                // Test DB Connection with Retry
                dbSuccess = Repo.ConnectToRetry(ref sDBName, _dbRetry);

                ////Collin WW14.3-2015: Make sure RequestType and RequestValue is not null
                RequestType = RequestType == null ? "1" : RequestType;
                RequestValue = RequestValue == null ? "" : RequestValue;
                ////Collin WW14.3-2015: Assign a default value for RequestType
                if (RequestType.Trim() != "0" && RequestType.Trim() != "1")
                    RequestType = "1";

                DataSet dataSet = new DataSet();
                SqlParameter[] param = {
                           new SqlParameter("@type", RequestType),
                            new SqlParameter("@value", RequestValue),
                          };


                if (RequestValue.Trim() != "")
                {
                    if (dbSuccess == 0)
                    {

                        dataSet = Repo.GetDataSet(sDBName, AppConstants.SP_IN_SSD_GET_SHIPTORESULT, param);
                        result = sntraxService.GetShippingDetails(dataSet);
                        stringwriter = sntraxService.Serialize(result);
                        FinalDNXml = string.Format(_validate_SSD_CPU_ShipToResponseOuterXML, sntraxService.ReplaceXmlTag(stringwriter, "Validate_SSD_CPU_ShipTo"));
                    }
                    else
                    {
                        //if db connection failed 
                        result.RecordFound = 2;
                    }
                }

            }
            catch (Exception eX)
            {
                SendMail sm = new SendMail("Validate_SSD_CPU_ShipTo", Environment.MachineName);
                sm.SendEmail(eX.Message.ToString());
                result.RecordFound = 2;
            }
            return FinalDNXml;
        }

[HttpPost]
[Consumes("application/xml")]
[Route("get_EIMRma")]
public string get_EIMRma(XmlDocument doc)
{
    string methodName = "IBaseGetDataByDN";
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    string sDBName = string.Empty;
    string FinalEIMRmaXml = string.Empty;
    List<GetEIMRmaResult> getEIMRmaResult = new List<GetEIMRmaResult>();

    try
    {
        var soapBody = doc.GetElementsByTagName("strSN")[0];
        string SerialNumber = soapBody.InnerXml;
        // Test DB Connection with Retry
        dbSuccess = Repo.ConnectToRetry(ref sDBName, _dbRetry);
        DataSet dataSet = new DataSet();
        SqlParameter[] param = {
                           new SqlParameter("@sn",SerialNumber),
                          };
        dataSet = Repo.GetDataSet(sDBName, AppConstants.SP_IN_EIM_GET_SHIPRMA_DATA, param);
        getEIMRmaResult = sntraxService.getEIMRmaResult(dataSet, SerialNumber);
        string stringwriter = sntraxService.Serialize(getEIMRmaResult);
        FinalEIMRmaXml = string.Format(_DNXML, sntraxService.ReplaceXmlTag(stringwriter, "get_EIMRma"));

    }
    catch (Exception ex)
    {
        CLogger.LogInfo(methodName + " exception : " + ex.Message);
    }
    stopwatch.Stop();
    CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);

    return FinalEIMRmaXml;
}

[HttpPost]
[Route("UploadSNv6")]
public List<SNv6> UploadSNv6(List<SNv6> SNv6List)
{
    List<SNv6> _rtnList = new List<SNv6>();
    return _rtnList;
}
    }
}
