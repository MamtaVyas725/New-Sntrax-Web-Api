namespace SntraxWebAPI.Model
{
    public class ShipToResult
    {
      //  public ShipToResult() { }
        public int RecordFound { get; set; }
        public string ProductCode { get; set; }
        public string ShippingDate { get; set; }
        public string FERT { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string WarrantyExpire { get; set; }
        public string CustomerRegion { get; set; }
    }
}