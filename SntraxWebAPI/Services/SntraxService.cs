﻿using EnttlOrchestrationLayer.Utilities;
using SntraxWebAPI.Model;
using System;
using System.Collections.Generic;
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


        public string ReplaceXmlTag(string xmlstring, string methodName)
        {
            string returnXmlstring = string.Empty;
            if (methodName == "get_EIMRma" || methodName == "IBaseGetDataByDN") //Mamta - change this if() as per your needs
            {
                returnXmlstring = xmlstring.ToString().Replace("<ArrayOfIBaseData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "").Replace("</ArrayOfIBaseData>", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
                returnXmlstring = returnXmlstring.ToString().Replace("<ArrayOfGetEIMRmaResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "").Replace("</ArrayOfGetEIMRmaResult>", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

            }
            if (methodName == "IBaseGetSingleData")
            {
                returnXmlstring = xmlstring.ToString().Replace("<ArrayOfIBaseData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<IBaseGetSingleDataResult>").Replace("</ArrayOfIBaseData>", "</IBaseGetSingleDataResult>").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            }
            if (methodName == "Validate_SSD_CPU_ShipTo")
            {
                returnXmlstring = xmlstring.ToString().Replace("<ShipToResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<Validate_SSD_CPU_ShipToResult>").Replace("</ShipToResult>", "</Validate_SSD_CPU_ShipToResult>").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            }

            return returnXmlstring.ToString();
        }

        public List<GetEIMRmaResult> getEIMRmaResult(DataSet dataSet, string SerialNumber)
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

        public string validateSNv6(SNv6 _SNv6)
        {

            string msg_rtn = "";
            if (string.IsNullOrWhiteSpace(_SNv6.Type).Equals(0))
            {
                msg_rtn += "Type cannot be empty.";
            }
            if ((_SNv6.Type ?? "").Trim().Length > 1)
            {
                msg_rtn += "Type length cannot be more than one character long. ";
            }
            if (!(_SNv6.Type ?? "").Trim().Equals("A") && !(_SNv6.Type ?? "").Trim().Equals("U"))
            {
                msg_rtn += "Type must be either A or U";
            }
            if ((_SNv6.Site ?? "").Trim().Length == 0)
            {
                msg_rtn += "Site cannot be empty.";
            }
            if ((_SNv6.Site ?? "").Trim().Length > 5)
            {
                msg_rtn += "Site length cannot be more than five character long. ";
            }
            if ((_SNv6.SN ?? "").Trim().Length == 0)
            {
                msg_rtn += "SN cannot be empty.";
            }
            if ((_SNv6.SN ?? "").Trim().Length > 30)
            {
                msg_rtn += "SN length cannot be more than 30 character long. ";
            }
            if ((_SNv6.ProductName ?? "").Trim().Length == 0)
            {
                msg_rtn += "Product Name cannot be empty.";
            }
            if ((_SNv6.ProductName ?? "").Trim().Length > 15)
            {
                msg_rtn += "Product Name cannot be over 15 character long. ";
            }
            if ((_SNv6.WorkOrder ?? "").Trim().Length == 0)
            {
                msg_rtn += "Workorder cannot be empty.";
            }
            if ((_SNv6.WorkOrder ?? "").Trim().Length > 15)
            {
                msg_rtn += "Workorder length cannot be more than 15 character long. ";
            }
            if ((_SNv6.CustomerSN ?? "").Trim().Length > 50)
            {
                msg_rtn += "Customer SN length cannot be more than 50 character long. ";
            }
            //if (_SNv6.BuildDate.ToString().Length > 10)
            //    _hasError = 'Y';
            if ((_SNv6.BuildDate ?? "").Trim().Length == 0)
            {
                msg_rtn += "Build Date cannot be empty.";
            }
            if ((_SNv6.COO ?? "").Trim().Length > 2)
            {
                msg_rtn += "COO flag length cannot be more than two character long. ";
            }
            if ((_SNv6.Batch ?? "").Trim().Length > 10)
            {
                msg_rtn += "Batch ID length cannot be more than 10 character long. ";
            }
            if ((_SNv6.Batch ?? "").Trim().Length == 0)
            {
                msg_rtn += "Batch ID cannot be empty.";
            }
            if ((_SNv6.MM ?? "").Trim().Length > 6)
            {
                msg_rtn += "MM length cannot be more than six character long. ";
            }
            if ((_SNv6.MM ?? "").Trim().Length == 0)
            {
                //msg_rtn += "MM cannot be empty.";
            }
            if ((_SNv6.Version ?? "").Trim().Length > 15)
            {
                msg_rtn += "Version length cannot be more than 15 character long. ";
            }
            if ((_SNv6.Version ?? "").Trim().Length == 0)
            {
                //msg_rtn += "Version cannot be empty.";
            }
            if ((_SNv6.CartonID ?? "").Trim().Length > 10)
            {
                msg_rtn += "Carton ID length cannot be more than 10 character long. ";
            }
            if ((_SNv6.PalletID ?? "").Trim().Length > 10)
            {
                msg_rtn += "Pallet ID length cannot be more than 10 character long. ";
            }
            if ((_SNv6.ReceiptID ?? "").Trim().Length > 15)
            {
                msg_rtn += "Receipt ID length cannot be more than 15 character long. ";
            }

            return msg_rtn;
        }
        public string validateSNv6_comp(Components _component)
        {
            string msg_rtn = "";
            if ((_component.IntelPartNumber ?? "").Trim().Length > 15)
            {
                msg_rtn += "Intel Part Number cannot be more than 15 character long. ";
            }
            if ((_component.Vendor ?? "").Trim().Length > 6)
            {
                msg_rtn += "Vendor cannot be more than six character long. ";
            }
            if ((_component.VendorSN ?? "").Trim().Length > 64)
            {
                msg_rtn += "Vendor SN cannot be more than 64 character long. ";
            }
            if ((_component.Desc ?? "").Trim().Length > 40)
            {
                msg_rtn += "Description cannot be more than 40 character long. ";
            }
            if ((_component.ManufacturerPartNumber ?? "").Trim().Length > 20)
            {
                msg_rtn += "Manufacturer Part Number cannot be more than 20 character long. ";
            }
            //Add Intel PN and Vendor SN to msg.
            if (msg_rtn.Length > 0)
            {
                msg_rtn = "[IntelPN: " + _component.IntelPartNumber + "][Vendor SN: " + _component.VendorSN + "]" + msg_rtn;
            }

            return msg_rtn;
        }

        public List<ClsOLDataByMultipleSN> getDataByMultipleSN(DataTable dataTable, bool isDN)
        {
            string methodName = "getIbaseData";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<ClsOLDataByMultipleSN> returnList = new List<ClsOLDataByMultipleSN>();
            try
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ClsOLDataByMultipleSN item = new ClsOLDataByMultipleSN();
                    item.ULTID = dataTable.Rows[i]["ULTID"].ToString();
                    item.MMID = dataTable.Rows[i]["MMID"].ToString();
                    item.Shipdate = dataTable.Rows[i]["Shipdate"].ToString();
                    item.DeliveryNote = dataTable.Rows[i]["DeliveryNote"].ToString();
                    item.Spec_cd = dataTable.Rows[i]["Spec_cd"].ToString();
                    item.Product_Code = dataTable.Rows[i]["Product_Code"].ToString();
                    item.Sales_Org = dataTable.Rows[i]["MSales_OrgMID"].ToString();
                    item.Dist_Channel = dataTable.Rows[i]["Dist_Channel"].ToString();
                    item.ShipToId = dataTable.Rows[i]["ShipToId"].ToString();
                    item.ShiptoName = dataTable.Rows[i]["ShiptoName"].ToString();
                    item.SoldtoId = dataTable.Rows[i]["SoldtoId"].ToString();
                    item.SoldtoName = dataTable.Rows[i]["SoldtoName"].ToString();
                    item.Status = string.IsNullOrWhiteSpace(dataTable.Rows[i]["MMID"].ToString()) ? "NF" : "F";
                    returnList.Add(item);
                }

            }
            catch (Exception ex)
            {
                //  clsSendMail sm = new clsSendMail("Get_r4cSntraxOrchs_SearchByMultipleSN", Environment.MachineName);
                // sm.SendEmail(eX.Message.ToString());
                CLogger.LogInfo(methodName + " exception : " + ex.Message);
            }
            stopwatch.Stop();
            CLogger.LogInfo(methodName + " completed in : " + stopwatch.Elapsed);

            return returnList;
        }


        public ShipToResult GetShippingDetails(DataSet dataSet)
        {
            ShipToResult shipResult = new()
            {
                RecordFound = 0 //default or record not found
            };
            try
            {
                if (dataSet.Tables.Count > 0)
                {
                    DataTable dtCustomerDetails = new();
                    dtCustomerDetails = dataSet.Tables[0];
                    shipResult.RecordFound = 1; //record found
                    shipResult.ProductCode = (dtCustomerDetails.Rows[0].Field<string>(0) ?? "");
                    shipResult.ShippingDate = (dtCustomerDetails.Rows[0].Field<string>(1) ?? "");
                    shipResult.FERT = (dtCustomerDetails.Rows[0].Field<string>(2) ?? "");
                    shipResult.CustomerID = (dtCustomerDetails.Rows[0].Field<string>(3) ?? "");
                    shipResult.CustomerName = (dtCustomerDetails.Rows[0].Field<string>(4) ?? "");
                    shipResult.WarrantyExpire = (dtCustomerDetails.Rows[0].Field<string>(5) ?? "");
                    shipResult.CustomerRegion = (dtCustomerDetails.Rows[0].Field<string>(6) ?? "");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return shipResult;
        }


    }
}
