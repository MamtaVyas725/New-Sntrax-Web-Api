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
        private string _outerDNXML = string.Empty;
        private string _validate_SSD_CPU_ShipToResponseOuterXML = string.Empty;
        private string _iBaseGetSingleDataXML = string.Empty;
        private string _multipleSNXML = string.Empty;
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
            _outerDNXML = _rootObjectCommon.GetValue<string>("XmlDNConfiguration:OuterXML");
            _eIMRmaXML = _rootObjectCommon.GetValue<string>("XmlDNConfiguration:EIMRmaXML");
            _multipleSNXML = _rootObjectCommon.GetValue<string>("XmlDNConfiguration:MultipleSNXML");
            _DNXML = _rootObjectCommon.GetValue<string>("OuterXml:DNXML");
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
                    if (dbSuccess == 0)
                    {
                        DataSet dataSet = new DataSet();
                        SqlParameter[] param = {
                           new SqlParameter("@param_dn",dnString),
                          };
                        dataSet = Repo.GetDataSet(sDBName, AppConstants.SP_INT_IBASE_GET_DN, param);
                        returnList = sntraxService.getIbaseData(dataSet, true);
                        stringwriter = sntraxService.Serialize(returnList);
                        FinalDNXml = string.Format(_outerDNXML, sntraxService.ReplaceXmlTag(stringwriter,""));
                    }
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
            List<IBaseData> IBaseData = new List<IBaseData>();

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
        [Consumes("application/xml")]
        [Route("UploadSNv6")]
        public string UploadSNv6(XmlDocument doc)
        {

            string methodName = "UploadSNv6";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string sDBName = string.Empty;
            string stringwriter = string.Empty;
            string FinalDNXml = string.Empty;
            var soapBody = doc.GetElementsByTagName("UploadSNv6")[0];
            string innerObject = soapBody.InnerXml;
            var myJsonResponse = Repo.XmlToJson(innerObject);
            SNv6Root myDeserializedClass = JsonConvert.DeserializeObject<SNv6Root>(myJsonResponse);
            List<SNv6> SNv6List = myDeserializedClass.SNv6List.SNv6.ToList();
            List<SNv6> _rtnList = new List<SNv6>(); //New list for returning to caller
            DateTime update_dt = DateTime.Now;
            try
            {
                // Test DB Connection with Retry
                dbSuccess = Repo.ConnectToRetry(ref sDBName, _dbRetry);
                if (dbSuccess == 0)
                {
                    foreach (SNv6 _SNv6 in SNv6List)
                    {
                        SNv6 _rtnSNv6 = new SNv6(); //New SNv6 to be added to the return list
                        char _hasError = '0';
                        string _msg = "";
                        string _comp_msg = "";
                        try
                        {
                            _msg = sntraxService.validateSNv6(_SNv6);
                            if (!_msg.Equals(""))
                                _hasError = '1';
                            foreach(Components _component in _SNv6.ComponentList.Component)
                            {
                                _comp_msg += sntraxService.validateSNv6_comp(_component);
                                if (!string.IsNullOrWhiteSpace(_comp_msg) && _hasError == '0')
                                    _hasError = '2';
                                if (!string.IsNullOrWhiteSpace(_comp_msg) && _hasError == '1')
                                    _hasError = '3';
                            }
                            _msg += _comp_msg;
                            SqlParameter[] param = {
                              new SqlParameter("@rec_type","NOCNF"),
                              new SqlParameter("@typ",_SNv6.Type),
                              new SqlParameter("@siteid", (_SNv6.Site ?? "").Trim()),
                              new SqlParameter("@sn", (_SNv6.SN ?? "").Trim()),
                              new SqlParameter("@fcst_prd_nm", (_SNv6.ProductName ?? "").Trim()),
                              new SqlParameter("@workorder", (_SNv6.WorkOrder ?? "").Trim()),
                              new SqlParameter("@intel_pn", ""),
                              new SqlParameter("@vendor", ""),
                              new SqlParameter("@vendor_sn", ""),
                              new SqlParameter("@cust_pn", ""),
                              new SqlParameter("@cust_sn", (_SNv6.CustomerSN ?? "").Trim()),
                              new SqlParameter("@workdate", (_SNv6.BuildDate ?? "").Trim()),
                              new SqlParameter("@coo_flag", (_SNv6.COO ?? "").Trim()),
                              new SqlParameter("@descr", ""),
                              new SqlParameter("@batch_no", (_SNv6.Batch ?? "").Trim()),
                              new SqlParameter("@mat_id", _SNv6.MM),
                              new SqlParameter("@version", (_SNv6.Version ?? "").Trim()),
                              new SqlParameter("@carton_id", (_SNv6.CartonID ?? "").Trim()),
                              new SqlParameter("@pallet_id", (_SNv6.PalletID ?? "").Trim()),
                              new SqlParameter("@receipt_id", (_SNv6.ReceiptID ?? "").Trim()),
                              new SqlParameter("@update_dt", update_dt),
                              new SqlParameter("@hasError", _hasError),
                              new SqlParameter("@msg", (_msg ?? "").Trim()),
                        };
                            Repo.ExecuteNonQuery(sDBName, AppConstants.SP_INT_WS_INSERT_BUILD_UPLOAD, param);

                        }
                        catch (Exception ex)
                        {

                        }
                        //Assign _SNv6 to return variables
                        _rtnSNv6 = _SNv6;
                        //_rtnSNv6._status = _hasError.ToString();
                       // _rtnSNv6._msg = _msg;

                        //Temp component list
                        List<Components> tempComp = new List<Components>();

                        foreach (Components _component in _SNv6.ComponentList.Component)
                        {
                            Components _rtnComponent = new Components(); //New return component

                            try
                            {

                                SqlParameter[] param = {
                                    new SqlParameter("@rec_type", "CONF"),
                                new SqlParameter("@typ", _SNv6.Type),
                                new SqlParameter("@siteid", ""),
                                new SqlParameter("@sn", (_SNv6.SN ?? "").Trim()),
                                new SqlParameter("@fcst_prd_nm", ""),
                                new SqlParameter("@workorder", ""),
                                new SqlParameter("@intel_pn", (_component.IntelPartNumber ?? "").Trim()),
                                new SqlParameter("@vendor", (_component.Vendor ?? "").Trim()),
                                new SqlParameter("@vendor_sn", (_component.VendorSN ?? "").Trim()),
                                new SqlParameter("@cust_pn", (_component.ManufacturerPartNumber ?? "").Trim()),
                                new SqlParameter("@cust_sn", ""),
                                new SqlParameter("@workdate", ""),
                                new SqlParameter("@coo_flag", ""),
                                new SqlParameter("@descr", (_component.Desc ?? "").Trim()),
                                new SqlParameter("@batch_no", ""),
                                new SqlParameter("@mat_id", ""),
                                new SqlParameter("@version", ""),
                                new SqlParameter("@carton_id", ""),
                                new SqlParameter("@pallet_id", ""),
                                new SqlParameter("@receipt_id", ""),
                                new SqlParameter("@update_dt", update_dt),
                                new SqlParameter("@hasError", _hasError),
                                new SqlParameter("@msg", ""),
                            };
                              Repo.ExecuteNonQuery(sDBName, AppConstants.SP_INT_WS_INSERT_BUILD_UPLOAD, param);
                            }
                            catch (Exception eX)
                            {
                                //   clsSendMail sm = new clsSendMail("UploadSNv6", Environment.MachineName);
                                //  sm.SendEmail(eX.Message.ToString());
                            }

                            //Assign component to return component
                            _rtnComponent = _component;

                            //Assign component msg to return component
                            //_rtnComponent._msg = _comp_msg;

                            //Add component to return variable
                            tempComp.Add(_rtnComponent);

                        }
                        _rtnSNv6.ComponentList = tempComp;
                        //Add return variable to return list
                        _rtnList.Add(_rtnSNv6);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            stringwriter = sntraxService.Serialize(_rtnList);
            var FinalDNXml1 = string.Format(_outerDNXML, sntraxService.ReplaceXmlTag(stringwriter,""));
            return FinalDNXml1;
        }



        [HttpPost]
        [Consumes("application/xml")]
        [Route("Get_r4cSntraxOrchs_SearchByMultipleSN")]
        public string Get_r4cSntraxOrchs_SearchByMultipleSN(XmlDocument doc)
        {
            string methodName = "Get_r4cSntraxOrchs_SearchByMultipleSN";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string sDBName = string.Empty;
            string stringwriter = string.Empty;
            string FinalDNXml = string.Empty;
            var soapBody = doc.GetElementsByTagName("Get_r4cSntraxOrchs_SearchByMultipleSN")[0];
            string innerObject = soapBody.InnerXml;
            var myJsonResponse = Repo.XmlToJson(innerObject);
            var r = myJsonResponse.Replace("\"SNList\":{", "\"SNList\":[{").Replace("}}}", "}]}}");
            SearchByMultipleSNList myDeserializedClass = JsonConvert.DeserializeObject<SearchByMultipleSNList>(r);
            var SNList = myDeserializedClass.SNList.ToList();
            List<ClsOLDataByMultipleSN> returnList = new List<ClsOLDataByMultipleSN>();
            string snString = "";

            if (SNList != null && SNList.Count > 0)
            {
                snString = string.Join("|", SNList.Select(x => x.SN)).TrimEnd(',');
            }
            if (snString != "")
            {
                try
                {
                    // Test DB Connection with Retry
                    dbSuccess = Repo.ConnectToRetry(ref sDBName, _dbRetry);
                    if (dbSuccess == 0)
                    {
                        DataTable dataTable = new DataTable();
                        SqlParameter[] param = {
                           new SqlParameter("@param_sn",snString),
                          };
                        dataTable = Repo.GetDataTable(sDBName, AppConstants.SPGET_R4C_SNTRAX_ORCHS_SEARCH_BYSN, param);
                        returnList = sntraxService.getDataByMultipleSN(dataTable, true);
                        stringwriter = sntraxService.Serialize(returnList);
                        FinalDNXml = string.Format(_multipleSNXML, sntraxService.ReplaceXmlTag(stringwriter,""));
                    }
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

    }
}
