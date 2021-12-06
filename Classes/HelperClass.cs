namespace scabackend.Classes
{
    public class HelperClass
    {
        public static string CheckIsNullOrEmptyString(string value, string returnDefault = "NA")
        {
            return String.IsNullOrEmpty(value) ? returnDefault : value;
        }

        public static int CheckIsNullOrEmptyInt(string value, int returnDefault = 0)
        {
            return String.IsNullOrEmpty(value) ? returnDefault : Convert.ToInt32(value);
        }

        public static Int64 CheckIsNullOrEmptyInt64(string value, int returnDefault = 0)
        {
            return String.IsNullOrEmpty(value) ? returnDefault : Convert.ToInt64(value);
        }

        public static DateTime CheckIsNullOrEmptyDateTime(string value)
        {
            return String.IsNullOrEmpty(value) ? DateTime.Now : Convert.ToDateTime(value);
        }

    }
}
