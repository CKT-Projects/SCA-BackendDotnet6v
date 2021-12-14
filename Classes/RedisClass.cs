using StackExchange.Redis;
using scabackend.Settings;
using scabackend.Models;
using Newtonsoft.Json;
namespace scabackend.Classes
{
    public class RedisClass
    {
        public static IConfiguration iconfig { get; set; }
        public static IDatabase idatabase { get; set; }
        //private ConnectionMultiplexer Connect()
        //{
        //    string redisHost = iconfig["RedisSettings:host"];
        //    string redisPort = iconfig["RedisSettings:port"];

        //    ConfigurationOptions configurationOptions = new ConfigurationOptions
        //    {
        //        EndPoints = { string.Format("{0}:{1}", redisHost, redisPort) }
        //    };

        //    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configurationOptions);

        //    return redis;
        //}

        public bool SetUserSingle(UserDataModel userDataModel)
        {
            string datakey = userDataModel.username + "_" + Guid.NewGuid().ToString();

            bool result = false;

            try
            {
                string datavalue = JsonConvert.SerializeObject(userDataModel);

                result = idatabase.StringSet(datakey, datavalue);
            }
            catch 
            {}

            return result;
        }

        public UserDataModel GetUserSingle(string datakey)
        {
            UserDataModel userDataModel = new UserDataModel();

            try
            {
                var data = idatabase.StringGet(datakey);

                userDataModel = JsonConvert.DeserializeObject<UserDataModel>(data);
            }
            catch
            {}

            return userDataModel;
        }

        public UserModel CheckUserDuplicate(string username)
        {
            UserModel userModel = new UserModel();

            try
            {
                userModel.data = new List<UserDataModel>();

                IConnectionMultiplexer multiplexer = idatabase.Multiplexer;

                IServer server = multiplexer.GetServer(multiplexer.Configuration);

                foreach (var key in server.Keys(pattern: "*" + username + "*"))
                {
                    UserDataModel userDataModel = this.GetUserSingle(key);
                    userDataModel.key = key;
                    userModel.data.Add(userDataModel);
                }

                if (userModel.data.Count > 0)
                {
                    userModel.status = 200;
                    userModel.message = "Success";
                }
                else
                {
                    userModel.status = 404;
                    userModel.message = "No data";
                }
            }
            catch (Exception ex)
            {
                userModel.status = 500;
                userModel.message = ex.Message;
            }

            return userModel;
        }
    }
}
