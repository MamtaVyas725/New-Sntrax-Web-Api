
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SntraxWebAPI.Model
{

    #region OutputDN
    public class IBaseData
    {
        public string? GapInd { get; set; }
        public string? SN { get; set; }
        public string? StockingID { get; set; }
        public string? ShipDate { get; set; }
        public string? MaterialID { get; set; }
        public string? DN { get; set; }
        public string? ShipToID { get; set; }
        public string? ShipToCountry { get; set; }
        public string? SoldToID { get; set; }
        public string? SalesOrg { get; set; }
        public string? DistChannel { get; set; }
        public string? OpCode { get; set; }
        public string? SOType { get; set; }
        public int? ShipId { get; set; }
        public string? LineItem { get; set; }
        public string? SalesOrder { get; set; }

        public List<IBaseChild> IBaseChildList { get; set; } = new List<IBaseChild>();

    }

    public class IBaseChild
    {
        public string? CompSN { get; set; }
        public string? StockingID { get; set; }
        public List<IBaseGrandChild> IBaseGrandChildList { get; set; } = new List<IBaseGrandChild>();
    }


    public class IBaseGrandChild
    {
        public string? CompSN { get; set; }
        public string? StockingID { get; set; }
    }

    #endregion DNOutput

    #region InputDN
    public class Body
    {
        public IBaseGetDataByDN IBaseGetDataByDN { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }

        [JsonProperty("_xmlns:xsi")]
        public string _xmlnsxsi { get; set; }

        [JsonProperty("_xmlns:xsd")]
        public string _xmlnsxsd { get; set; }

        [JsonProperty("_xmlns:soap")]
        public string _xmlnssoap { get; set; }
    }

    public class IBaseDNList
    {
        public string DN { get; set; }
    }

    public class IBaseSNList
    {
        public string sn { get; set; }
    }

    public class IBaseGetDataByDN
    {
        public List list { get; set; }
    }

    public class List
    {
        public List<IBaseDNList> DN { get; set; }
    }

    public class IBaseGetSingleData
    {
        public string sn { get; set; }
    }  

    public class Root
    {
        public Envelope Envelope { get; set; }
    }

}


#endregion InputDN
