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
        private string _outerDNXML = string.Empty;
        private static int dbSuccess = -1;




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
            _outerDNXML = _rootObjectCommon.GetValue<string>("XmlDNConfiguration:OuterXML");
            CLogger._logger = _logger;
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
            var IBaseDataDNList = myDeserializedClass.list.IBaseData.ToList();
            List<IBaseData>returnList = new List<IBaseData>();
            string dnString = "";
            SntraxService sntraxService = new SntraxService();
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
                    FinalDNXml = string.Format(_outerDNXML, sntraxService.ReplaceXmlTag(stringwriter));
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
        [Route("IBaseGetSingleData")]
        public List<IBaseData> IBaseGetSingleData(List<IBaseData> list)
        {
            List<IBaseData> IBaseData = new List<IBaseData>();
            return IBaseData;
        }

        [HttpPost]
        [Route("Validate_SSD_CPU_ShipTo")]
        public ShipToResult Validate_SSD_CPU_ShipTo(string RequestType, string RequestValue)
        {
            ShipToResult shipResult = new ShipToResult();
            return shipResult;
        }

        [HttpPost]
        [Route("get_EIMRma")]
        public shipData get_EIMRma(string strSN)
        {
            shipData ship = new shipData();
            return ship;
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
