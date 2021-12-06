using MySql.Data.MySqlClient;
using scabackend.Models;
using scabackend.Classes;

namespace scabackend.Services
{
    public class UserService
    {
        private MySqlConnection _mysqlCon { get; set; }

        public UserService(MySqlConnection mysqlCon)
        {
            this._mysqlCon = mysqlCon;
        }

        public UserDataModel Get(UserLoginModel userLogin)
        {
            return null;
        }

        public UserModel GetOldUserList(string sqlQuery)
        {
            UserModel userModel = new UserModel();

            List<UserDataModel> userDataModels = new List<UserDataModel>();

            try
            {
                using (MySqlConnection sqlCon = this._mysqlCon)
                {
                    using (MySqlCommand sqlCmd = new MySqlCommand(sqlQuery, sqlCon))
                    {
                        sqlCon.Open();

                        using (MySqlDataReader reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserDataModel userDataModel = new UserDataModel();
                                userDataModel.username = HelperClass.CheckIsNullOrEmptyString(reader["username"].ToString());
                                userDataModel.email = HelperClass.CheckIsNullOrEmptyString(reader["email"].ToString());
                                userDataModel.mobile = HelperClass.CheckIsNullOrEmptyString(reader["mobile"].ToString());
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
