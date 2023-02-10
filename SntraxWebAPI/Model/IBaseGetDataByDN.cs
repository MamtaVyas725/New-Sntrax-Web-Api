namespace SntraxWebAPI.Model.IBaseGetDataByDN
{
    public class Body
    {
        public IBaseGetDataByDN IBaseGetDataByDN { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

    public class IBaseDatum
    {
        public int DN { get; set; }
    }

    public class IBaseGetDataByDN
    {
        public List list { get; set; }
    }

    public class List
    {
        public List<IBaseDatum> IBaseData { get; set; }
    }

    public class Root
    {
        public Envelope Envelope { get; set; }
    }

}
