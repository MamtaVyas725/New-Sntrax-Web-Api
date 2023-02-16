using Newtonsoft.Json;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SntraxWebAPI.Model
{
    public class Body
    {
        public UploadSNv6 UploadSNv6 { get; set; }
    }

    public class Component
    {
        public string IntelPartNumber { get; set; }
        public string Vendor { get; set; }
        public string VendorSN { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string Desc { get; set; }
        public string Msg { get; set; }
    }

    public class ComponentList
    {
        public List<Component> Component { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

    public class Root
    {
        public Envelope Envelope { get; set; }
    }

    public class SNv6
    {
        internal string _status = "";
        internal string _msg = "";
        public string Type { get; set; }
        public string Site { get; set; }
        public string SN { get; set; }
        public string ProductName { get; set; }
        public string WorkOrder { get; set; }
        public string CustomerSN { get; set; }
        public string BuildDate { get; set; }
        public string COO { get; set; }
        public string Batch { get; set; }
        public string MM { get; set; }
        public string Version { get; set; }
        public string CartonID { get; set; }
        public string PalletID { get; set; }
        public string ReceiptID { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }
        public ComponentList ComponentList { get; set; }
    }

    public class SNv6List
    {
        public List<SNv6> SNv6 { get; set; }
    }

    public class UploadSNv6
    {
        public SNv6List SNv6List { get; set; }
    }

}
