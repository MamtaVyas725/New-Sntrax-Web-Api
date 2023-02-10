namespace SntraxWebAPI.Model
{
    #region Input

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class SearchByMultipleSNBody
    {
        public GetR4cSntraxOrchsSearchByMultipleSN Get_r4cSntraxOrchs_SearchByMultipleSN { get; set; }
    }

    public class SearchByMultipleSNEnvelope
    {
        public SearchByMultipleSNBody Body { get; set; }
    }

    public class GetR4cSntraxOrchsSearchByMultipleSN
    {
        public SearchByMultipleSNList list { get; set; }
    }

    public class SearchByMultipleSNList
    {
        public List<SNList> SNList { get; set; }
    }

    public class SearchByMultipleSN
    {
        public SearchByMultipleSNEnvelope Envelope { get; set; }
    }

    public class SNList
    {
        public string SN { get; set; }
    }


    #endregion input



    #region output
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class cls_OL_DataByMultipleSN
    {
        public string ULTID { get; set; }
        public string MMID { get; set; }
        public string Shipdate { get; set; }
        public string DeliveryNote { get; set; }
        public string Spec_cd { get; set; }
        public string Product_Code { get; set; }
        public string Sales_Org { get; set; }
        public string Dist_Channel { get; set; }
        public string ShipToId { get; set; }
        public string ShiptoName { get; set; }
        public string SoldtoId { get; set; }
        public string SoldtoName { get; set; }
        public string Status { get; set; }
    }

    public class GetR4cSntraxOrchsSearchByMultipleSNResult
    {
        public cls_OL_DataByMultipleSN cls_OL_DataByMultipleSN { get; set; }
    }

    public class SearchByMultipleSNOutput
    {
        public GetR4cSntraxOrchsSearchByMultipleSNResult Get_r4cSntraxOrchs_SearchByMultipleSNResult { get; set; }
    }


    #endregion output
}
