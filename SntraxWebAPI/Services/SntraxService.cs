using EnttlOrchestrationLayer.Utilities;
using SntraxWebAPI.Model;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace SntraxWebAPI.Services
{
    public class SntraxService
    {
        public List<IBaseData> getIbaseData(DataSet dataSet, bool isDN)
        {
            string methodName = "getIbaseData";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<IBaseData> returnList = new List<IBaseData>();
            string? comp_sn = string.Empty;
            string? parent_sn = string.Empty;

            try
            {
                if (dataSet.Tables.Count > 0)
                {
                    DataTable dtParent = new DataTable();
                    DataTable dtChild = new DataTable();
                    DataTable dtGrandChild = new DataTable();
                    dtParent = dataSet.Tables[0];
                    dtChild = dataSet.Tables[1];
                    dtGrandChild = dataSet.Tables[2];
                    for (int i = 0; i < dtParent.Rows.Count; i++)
                    {
                        IBaseData ibaseParentData = new IBaseData();
                        ibaseParentData.DistChannel = dtParent.Rows[i]["dist_channel"].ToString();
                        // ibaseParentData.DN = dtParent.Rows[i]["dn"].ToString();
                        ibaseParentData.GapInd = dtParent.Rows[i]["gap_ind"].ToString();
                        ibaseParentData.MaterialID = dtParent.Rows[i]["material_id"].ToString();
                        ibaseParentData.OpCode = dtParent.Rows[i]["op_code"].ToString();
                        ibaseParentData.SalesOrg = dtParent.Rows[i]["sales_org"].ToString();
                        ibaseParentData.ShipDate = dtParent.Rows[i]["ship_date"].ToString();
                        ibaseParentData.ShipToCountry = dtParent.Rows[i]["shipto_country"].ToString();
                        ibaseParentData.ShipToID = dtParent.Rows[i]["shipto_id"].ToString();
                        ibaseParentData.SN = dtParent.Rows[i]["sn"].ToString();
                        ibaseParentData.SoldToID = dtParent.Rows[i]["soldto_id"].ToString();
                        ibaseParentData.SOType = dtParent.Rows[i]["so_type"].ToString();
                        ibaseParentData.StockingID = dtParent.Rows[i]["stocking_id"].ToString();
                        ibaseParentData.ShipId = Convert.ToInt32(dtParent.Rows[i]["ship_id"].ToString());
                        if (isDN)
                        {
                            ibaseParentData.LineItem = dtParent.Rows[i]["sls_ord_id"].ToString();
                            ibaseParentData.SalesOrder = dtParent.Rows[i]["sls_ord_itm_nbr"].ToString();
                        }
                        returnList.Add(ibaseParentData);
                    }
                    for (int i = 0; i < dtChild.Rows.Count; i++)
                    {
                        IBaseChild baseChild = new IBaseChild();
                        parent_sn = dtChild.Rows[i]["sn"].ToString();
                        baseChild.CompSN = dtChild.Rows[i]["comp_sn"].ToString();
                        baseChild.StockingID = dtChild.Rows[i]["stocking_id"].ToString();
                        returnList.Where(p => p.SN != null && p.SN.Equals(parent_sn)).FirstOrDefault().IBaseChildList.Add(baseChild);
                    }
                    for (int i = 0; i < dtGrandChild.Rows.Count; i++)
                    {
                        IBaseGrandChild baseGrandChild = new IBaseGrandChild();
                        parent_sn = dtGrandChild.Rows[i]["sn"].ToString();
                        comp_sn = dtGrandChild.Rows[i]["comp_sn"].ToString();
                        baseGrandChild.CompSN = dtGrandChild.Rows[i]["comp_sn"].ToString();
                        baseGrandChild.StockingID = dtGrandChild.Rows[i]["stocking_id"].ToString();
                        returnList.Where(p => p.SN != null && p.SN.Equals(parent_sn)).FirstOrDefault().IBaseChildList.Find(c => c.CompSN == comp_sn).IBaseGrandChildList.Add(baseGrandChild);
                    }
                }

            }
            catch (Exception ex)
            {
                CLogger.LogInfo(methodName + " exception : " + ex.Message);
            }
            stopwatch.Stop();
            CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);

            return returnList;
        }

        public string Serialize(object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }


        public string ReplaceXmlTag(string xmlstring)
        {
            string returnXmlstring = string.Empty; ;
            returnXmlstring = xmlstring.ToString().Replace("<ArrayOfIBaseData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "").Replace("</ArrayOfIBaseData>", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            returnXmlstring = returnXmlstring.ToString().Replace("<ArrayOfGetEIMRmaResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "").Replace("</ArrayOfGetEIMRmaResult>", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            return returnXmlstring.ToString();
        }

        public List<GetEIMRmaResult> getEIMRmaResult(DataSet dataSet,string SerialNumber)
        {

            DataTable dtEIMRma = new DataTable();
            DataTable dtPartNumberList = new DataTable();
            List<GetEIMRmaResult> getEIMRmaList = new List<GetEIMRmaResult>();
            List<PartNumberList> partNumberList = new List<PartNumberList>();
            try
            {
                if (dataSet.Tables.Count > 0)
                {
                    dtEIMRma = dataSet.Tables[0];
                    dtPartNumberList = dataSet.Tables[1];
                    for (int i = 0; i < dtEIMRma.Rows.Count; i++)
                    {
                        GetEIMRmaResult getEIMRma = new GetEIMRmaResult();
                        getEIMRma.ShipDate = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", dtEIMRma.Rows[i]["shipdate"].ToString());
                        getEIMRma.CountryCode = dtEIMRma.Rows[i]["country_code"].ToString();
                        getEIMRma.ReturnFrequency = int.Parse(dtEIMRma.Rows[i]["frequency"].ToString());
                        getEIMRma.ProcessCode = int.Parse(dtEIMRma.Rows[i]["process_code"].ToString());
                        getEIMRma.ReplacementFrequency = int.Parse(dtEIMRma.Rows[i]["rfrequency"].ToString());
                        getEIMRma.StolenProduct = dtEIMRma.Rows[i]["stolen_prod"].ToString();
                        getEIMRma.SerialNumber = SerialNumber;
                        getEIMRmaList.Add(getEIMRma);
                    }
                    for (int j = 0; j < dtPartNumberList.Rows.Count; j++)
                    {
                        PartNumberList pt = new PartNumberList();
                        pt.PartNumber.Add(dtPartNumberList.Rows[j]["PartNum"].ToString());
                        partNumberList.Add(pt);
                    }
                    getEIMRmaList[0].PartNumberList = partNumberList;
                    //ship.CountryCode = sqlReader.GetValue(1).ToString();
                    //ship.ReturnFrequency = Int32.Parse(sqlReader.GetValue(2).ToString());
                    //ship.ProcessCode = Int32.Parse(sqlReader.GetValue(3).ToString());
                    //ship.ReplacementFrequency = Int32.Parse(sqlReader.GetValue(4).ToString()); ////ADD SRIKANTH
                    //ship.StolenProduct = sqlReader.GetValue(5).ToString(); ////ADD SRIKANTH


                }
            }
            catch (Exception ex)
            {

            }

            return getEIMRmaList;
        }
    }
}
