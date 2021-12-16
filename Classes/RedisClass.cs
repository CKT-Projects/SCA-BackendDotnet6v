using StackExchange.Redis;
using scabackend.Settings;
using scabackend.Models;
using Newtonsoft.Json;
namespace scabackend.Classes
{
    public class RedisClass
    {
        public static IDatabase idatabase { get; set; }

        public bool SetUserModelSingle(UserModel userModel, string datakey = "allnewusers")
        {
            bool result = false;

            try
            {
                string datavalue = JsonConvert.SerializeObject(userModel);

                result = idatabase.StringSet(datakey, datavalue);
            }
            catch
            { }

            return result;
        }

        public async Task<bool> SetUserSingle(UserDataModel userDataModel)
        {
            string datakey = userDataModel.username + "_" + Guid.NewGuid().ToString();

            bool result = false;

            try
            {
                string datavalue = JsonConvert.SerializeObject(userDataModel);

                TimeSpan setTTL = TimeSpan.FromSeconds(10);

                result = await idatabase.StringSetAsync(datakey, datavalue, setTTL);
            }
            catch
            { }

            return result;
        }

        public async Task<UserModel> GetUserModelSingle(string datakey = "allnewusers")
        {
            UserModel userModel = new UserModel();

            try
            {
                var data = await idatabase.StringGetAsync(datakey);

                userModel = JsonConvert.DeserializeObject<UserModel>(data);
            }
            catch
            { }

            return userModel;
        }

        public async Task<UserDataModel> GetUserDataModelSingle(string datakey)
        {
            UserDataModel userDataModel = new UserDataModel();

            try
            {
                // var data = idatabase.StringGet(datakey);

                var data = await idatabase.StringGetAsync(datakey);

                userDataModel = JsonConvert.DeserializeObject<UserDataModel>(data);
            }
            catch
            { }

            return userDataModel;
        }

        public async Task<UserModel> CheckUserDuplicate(string username)
        {
            UserModel userModel = new UserModel();

            try
            {
                userModel.data = new List<UserDataModel>();

                IConnectionMultiplexer multiplexer = idatabase.Multiplexer;

                IServer server = multiplexer.GetServer(multiplexer.Configuration);

                foreach (var key in server.Keys(pattern: "*" + username + "*"))
                {
                    UserDataModel userDataModel = await this.GetUserDataModelSingle(key);
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
