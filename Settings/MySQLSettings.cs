namespace scabackend.Settings
{
    public class MySQLSettings
    {
        public MySQLDetails new_database { get; set; }
        public MySQLDetails old_database { get; set; }  
    }

    public class MySQLDetails
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string database { get; set; }
    }
}
