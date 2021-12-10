using StackExchange.Redis;
using scabackend.Settings;
using scabackend.Models;
using Newtonsoft.Json;
namespace scabackend.Classes
{
    public class RedisClass
    {
        public IDatabase _database { get; set; }

        public RedisClass(IDatabase database)
        {
            this._database = database;
        }

        public bool SetUserSingle(UserDataModel userDataModel)
        {
            string datakey = "scauser_" + userDataModel.username;

            bool result = false;

            try
            {
                string datavalue = JsonConvert.SerializeObject(userDataModel);

                result = this._database.StringSet(datakey, datavalue);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }

        public UserDataModel GetUserSingle(string username)
        {
            string datakey = "scauser_" + username;

            UserDataModel userDataModel = new UserDataModel();

            try
            {
                var data = this._database.StringGet(datakey);

                userDataModel = JsonConvert.DeserializeObject<UserDataModel>(data);
            }
            catch
            {}

            return userDataModel;
        }
    }
}
