namespace SntraxWebAPI.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    //public class Body
    //{
    //    public GetEIMRmaResponse get_EIMRmaResponse { get; set; }
    //}

    //public class Envelope
    //{
    //    public Body Body { get; set; }
    //}

    public class GetEIMRmaResponse
    {
        public GetEIMRmaResult get_EIMRmaResult { get; set; }
    }

    public class GetEIMRmaResult
    {
        public string SerialNumber { get; set; }
        public string ShipDate { get; set; }
        public string CountryCode { get; set; }
        public int ReturnFrequency { get; set; }
        public int ReplacementFrequency { get; set; }
        public string StolenProduct { get; set; }
        public int ProcessCode { get; set; }
        public List<PartNumberList> PartNumberList { get; set; }
    }

    public class PartNumberList
    {
        public List<string> PartNumber { get; set; } = new List<string>();
    }

    //public class shipData
    //{
    //    public Envelope Envelope { get; set; }
    //}


}