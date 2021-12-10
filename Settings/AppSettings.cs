namespace scabackend.Settings
{
    public class AppSettings
    {
        public Timezones timezones { get; set; }
    }

    public class Timezones
    {
        public string windows { get; set; }
        public string linux { get; set; }
    }
}
