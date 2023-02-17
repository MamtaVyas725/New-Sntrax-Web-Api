namespace SntraxWebAPI.Model.ShipData
{

    public class Body
    {
        public get_EIMRmaResponse get_EIMRmaResponse { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

    public class get_EIMRmaResponse
    {
        public get_EIMRmaResult get_EIMRmaResult { get; set; }
    }

    public class get_EIMRmaResult
    {
        public string SerialNumber { get; set; }
        public string ShipDate { get; set; }
        public string CountryCode { get; set; }
        public int ReturnFrequency { get; set; }
        public int ReplacementFrequency { get; set; }
        public string StolenProduct { get; set; }
        public int ProcessCode { get; set; }
        public List<string> PartNumberList = new List<string>();
    }

    public class shipData
    {
        public Envelope Envelope { get; set; }
    }


}