using System.Runtime.InteropServices;
using scabackend.Settings;

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

        public static DateTime DateTimeNow(AppSettings appSettings)
        {
            string timezone = appSettings.timezones.windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                timezone = appSettings.timezones.linux;
            }

            DateTime currentTime = TimeZoneInfo.ConvertTime(
                    DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById(timezone)
            );

            return currentTime;
        }

        public static string DateTimeNow(AppSettings appSettings, string format = "dddd, dd MMMM h:mm:ss tt")
        {
            DateTime currentTime = DateTimeNow(appSettings);

            string timezone = appSettings.timezones.windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                timezone = appSettings.timezones.linux;
            }

            return string.Format("{0}: {1}", timezone, currentTime.ToString(format));
        }

    }
}
