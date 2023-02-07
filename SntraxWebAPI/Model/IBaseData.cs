
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SntraxWebAPI.Model
{

    #region OutputDN
    public class IBaseData
    {
        public string? Gap_Ind { get; set; }
        public string? SN { get; set; }
        public string? Stocking_ID { get; set; }
        public string? Ship_Date { get; set; }
        public string? Material_ID { get; set; }
        public string? DN { get; set; }
        public string? ShipTo_ID { get; set; }
        public string? ShipTo_Country { get; set; }
        public string? SoldTo_ID { get; set; }
        public string? Sales_Org { get; set; }
        public string? Dist_Channel { get; set; }
        public string? Op_Code { get; set; }
        public string? SO_Type { get; set; }
        public int? Ship_Id { get; set; }
        public string? LineItem { get; set; }
        public string? SalesOrder { get; set; }

        public List<IBaseChild> IBaseChildList { get; set; } = new List<IBaseChild>();

    }

    public class IBaseChild
    {
        public string? Comp_SN { get; set; }
        public string? Stocking_ID { get; set; }
        public List<IBaseGrandChild> IBaseGrandChildList { get; set; } = new List<IBaseGrandChild>();
    }


    public class IBaseGrandChild
    {
        public string? Comp_SN { get; set; }
        public string? Stocking_ID { get; set; }
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
