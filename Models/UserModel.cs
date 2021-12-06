namespace scabackend.Models
{
    public class UserModel
    {
        public Int64 id { get; set; }
        public string token_access { get; set; }
        public string public_uuid { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string password { get; set; }
        public string hint { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public int role { get; set; }
        public Int64 connected_to { get; set; }
        public int is_active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
       
    }
}
