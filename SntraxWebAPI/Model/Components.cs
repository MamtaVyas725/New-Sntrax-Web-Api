namespace SntraxWebAPI.Model
{
    public class Components
    {
        /// <summary>
        /// Intel Part Number accepts value up to 15 characters
        /// </summary>
        public string IntelPartNumber { get; set; }
        /// <summary>
        /// Vendor accepts value up to 6 characters
        /// </summary>
        public string Vendor { get; set; }
        /// <summary>
        /// Vendor SN accepts value up to 15 characters
        /// </summary>
        public string VendorSN { get; set; }
        /// <summary>
        /// Manufacturing PN accepts value up to 20 characters
        /// </summary>
        public string ManufacturerPartNumber { get; set; }
        /// <summary>
        /// Description accepts value up to 40 characters
        /// </summary>
        public string Desc { get; set; }

        public string Msg { get; set; }

    }
}
