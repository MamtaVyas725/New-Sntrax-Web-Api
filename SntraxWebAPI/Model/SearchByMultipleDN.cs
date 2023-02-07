namespace SntraxWebAPI.Model.SearchByMultipleDN
{
    #region Input
    public class InputBody
    {
        public GetR4cSntraxOrchsSearchByMultipleDN Get_r4cSntraxOrchs_SearchByMultipleDN { get; set; }
    }

    public class SearchByMultipleDNEnvelope
    {
        public InputBody Body { get; set; }
    }

    public class GetR4cSntraxOrchsSearchByMultipleDN
    {
        public SearchByMultipleDNList list { get; set; }
    }

    public class SearchByMultipleDNList
    {
        public List<DNList> DNList { get; set; }
    }

    public class DNList
    {
        public string DN { get; set; }
    }

    public class SearchByMultipleDN
    {
        public SearchByMultipleDNEnvelope Envelope { get; set; }
    }

    #endregion

    #region Output

    public class OutputBody
    {
        public GetR4cSntraxOrchsSearchByMultipleDN Get_r4cSntraxOrchs_SearchByMultipleDN { get; set; }
    }

    public class OutputEnvelope
    {
        public OutputBody Body { get; set; }
    }

    public class SearchByMultipleDNOutput
    {
        public OutputEnvelope Envelope { get; set; }
    }


    public class cls_r4cSntraxOrchs_DataByMultipleDN
    {
        public string SN { get; set; }
        public string MMID { get; set; }
        public string DN { get; set; }
        public string Shipdate { get; set; }
        public string Status { get; set; }
    }


    public class GetR4cSntraxOrchsSearchByMultipleDNResponse
    {
        public GetR4cSntraxOrchsSearchByMultipleDNResult searchByMultipleDNResult { get; set; }
    }

    public class GetR4cSntraxOrchsSearchByMultipleDNResult
    {
        public List<cls_r4cSntraxOrchs_DataByMultipleDN> dataByMultipleDN { get; set; }
    }

    #endregion
}
