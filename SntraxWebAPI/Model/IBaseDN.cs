using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SntraxWebAPI.Model
{

    #region IbaseDNInput
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

    public class IBaseDataList
    {
        public string DN { get; set; }
    }

    public class IBaseGetDataByDN
    {
        public List list { get; set; }
    }

    public class List
    {
        public List<IBaseDataList> IBaseData { get; set; }
    }

    public class Root
    {
        public Envelope Envelope { get; set; }
    }

}

#endregion IbaseDNInput


