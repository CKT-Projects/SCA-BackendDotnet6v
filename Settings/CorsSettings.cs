namespace scabackend.Settings
{
    public class CorsSettings
    {
        public List<AllowSpecificOrigins> AllowSpecificOrigins { get; set; }
    }

    public class AllowSpecificOrigins
    {
        public string domain { get; set; }
    }
}
