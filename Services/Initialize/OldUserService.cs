using MySql.Data.MySqlClient;
using scabackend.Models;
using scabackend.Classes;
using scabackend.Enums;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace scabackend.Services.Initialize
{
    public class OldUserService
    {
        private RedisClass _redisClass { get; set; }
        private MySqlConnection _mysqlCon { get; set; }

        public OldUserService(MySqlConnection mysqlCon)
        {
            this._mysqlCon = mysqlCon;

            _redisClass = new RedisClass();
        }

        public void Start(UserService userService)
        {
            UserModel userOldData = this.GetUserList("SELECT " +
                                                                "user_name AS username, " +
                                                                "email, " +
                                                                "mobilenumber AS mobile, " +
                                                                "hint, " +
                                                                "last_name AS lastname, " +
                                                                "first_name AS firstname, " +
                                                                "middle_name AS middlename, " +
                                                                "role, " +
                                                                "worker_of, " +
                                                                "active AS is_active, " +
                                                                "created_at, " +
                                                                "updated_at " +
                                                                "FROM tbl_users WHERE active = 1 ORDER BY username ASC;");

            foreach (UserDataModel userData in userOldData.data)
            {
                UserModel result = _redisClass.CheckUserDuplicate(userData.username.Trim());

                if (result.status == 200)
                {
                    int indexCount = 0;

                    UserModuleAccess userModuleAccess = new UserModuleAccess();

                    userModuleAccess.modules = new int[result.data.Count + 1];

                    userModuleAccess.modules = access_module(userModuleAccess.modules, userData.role, indexCount);

                    foreach (UserDataModel userDataModel in result.data)
                    {
                        indexCount++;
                        userModuleAccess.modules = access_module(userModuleAccess.modules, userDataModel.role, indexCount);
                    }

                    if(indexCount > 0)
                    {
                        string value = JsonConvert.SerializeObject(userModuleAccess);

                        NameValueCollection updateData = new NameValueCollection()
                        {
                            { "access_permitted", value },
                        };

                        userService.UpdateChanges(updateData, String.Format("username = '{0}'", userData.username));
                    }
                }
                else
                {
                    if (result.status != 500)
                    {
                        UserModuleAccess userModuleAccess = new UserModuleAccess();

                        userModuleAccess.modules = new int[1];

                        userModuleAccess.modules = access_module(userModuleAccess.modules, userData.role, 0);

                        userData.access_permitted = JsonConvert.SerializeObject(userModuleAccess);

                        Int64 lastId = userService.SaveChangesWithLastId(userData);
                    }
                }

                _redisClass.SetUserSingle(userData);
            }
        }

        private int[] access_module(int[] modules, int role, int indexCount)
        {

            switch (role)
            {
                case (int)RoleOldEnum.Worker:
                    modules[indexCount] = (int)RoleEnum.Worker;
                    break;
                case (int)RoleOldEnum.AppraiserBoss:
                    modules[indexCount] = (int)RoleEnum.AppraiserBoss;
                    break;
                case (int)RoleOldEnum.AppraiserMaster:
                    modules[indexCount] = (int)RoleEnum.AppraiserManager;
                    break;
                case (int)RoleOldEnum.Appraiser:
                    modules[indexCount] = (int)RoleEnum.Appraiser;
                    break;
                case (int)RoleOldEnum.Seller:
                    modules[indexCount] = (int)RoleEnum.Seller;
                    break;
                case (int)RoleOldEnum.Buyer:
                    modules[indexCount] = (int)RoleEnum.Buyer;
                    break;
                case (int)RoleOldEnum.Admin:
                    modules[indexCount] = (int)RoleEnum.Admin;
                    break;
                default:
                    modules[indexCount] = (int)RoleEnum.Default;
                    break;
            }

            return modules;
        }

        public UserModel GetUserList(string sqlQuery)
        {
            UserModel userModel = new UserModel();

            List<UserDataModel> userDataModels = new List<UserDataModel>();

            try
            {
                using (MySqlConnection sqlCon = _mysqlCon)
                {
                    using (MySqlCommand sqlCmd = new MySqlCommand(sqlQuery, sqlCon))
                    {
                        sqlCon.Open();

                        using (MySqlDataReader reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserDataModel userDataModel = new UserDataModel();
                                userDataModel.public_uuid = Guid.NewGuid().ToString();
                                userDataModel.username = HelperClass.CheckIsNullOrEmptyString(reader["username"].ToString());
                                userDataModel.email = HelperClass.CheckIsNullOrEmptyString(reader["email"].ToString());
                                userDataModel.mobile = HelperClass.CheckIsNullOrEmptyString(reader["mobile"].ToString());
                                userDataModel.password = HelperClass.CheckIsNullOrEmptyString(reader["hint"].ToString());
                                userDataModel.hint = HelperClass.CheckIsNullOrEmptyString(reader["hint"].ToString());
                                userDataModel.firstname = HelperClass.CheckIsNullOrEmptyString(reader["firstname"].ToString());
                                userDataModel.middlename = HelperClass.CheckIsNullOrEmptyString(reader["middlename"].ToString());
                                userDataModel.lastname = HelperClass.CheckIsNullOrEmptyString(reader["lastname"].ToString());
                                userDataModel.role = HelperClass.CheckIsNullOrEmptyInt(reader["role"].ToString());
                                userDataModel.worker_of = HelperClass.CheckIsNullOrEmptyInt64(reader["worker_of"].ToString());
                                userDataModel.is_active = HelperClass.CheckIsNullOrEmptyInt(reader["is_active"].ToString());
                                userDataModel.created_at = HelperClass.CheckIsNullOrEmptyDateTime(reader["created_at"].ToString());
                                userDataModel.updated_at = HelperClass.CheckIsNullOrEmptyDateTime(reader["updated_at"].ToString());
                                userDataModels.Add(userDataModel);
                            }

                            reader.Close();
                        }

                        sqlCon.Close();
                    }
                }

                userModel.status = 200;
                userModel.message = "Success";
                userModel.data = userDataModels;
            }
            catch (Exception ex)
            {
                userModel.status = 500;
                userModel.message = ex.Message;
                userModel.data = null;
            }

            return userModel;
        }
    }
}
