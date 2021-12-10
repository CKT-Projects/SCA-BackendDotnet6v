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

        public bool SaveChanges(UserDataModel userDataModel)
        {
            using (MySqlConnection sqlCon = this._mysqlCon)
            {

                string fields = "public_uuid, username, email, mobile, ";
                fields += "password, hint, firstname, middlename, lastname, role, worker_of, ";
                fields += "is_active, last_logged_in_ipaddress, last_logged_in, created_at, updated_at";

                string values = "@public_uuid, @username, @email, @mobile, ";
                values += "@password, @hint, @firstname, @middlename, @lastname, @role, @worker_of, ";
                values += "@is_active, @last_logged_in_ipaddress, @last_logged_in, @created_at, @updated_at";

                string query = string.Format("INSERT INTO users ({0}) VALUES ({1});", fields, values);

                using (MySqlCommand sqlCmd = new MySqlCommand(query, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@public_uuid", userDataModel.public_uuid);
                    sqlCmd.Parameters.AddWithValue("@username", userDataModel.username);
                    sqlCmd.Parameters.AddWithValue("@email", userDataModel.email);
                    sqlCmd.Parameters.AddWithValue("@mobile", userDataModel.mobile);
                    sqlCmd.Parameters.AddWithValue("@password", userDataModel.password);
                    sqlCmd.Parameters.AddWithValue("@hint", userDataModel.hint);
                    sqlCmd.Parameters.AddWithValue("@firstname", userDataModel.firstname);
                    sqlCmd.Parameters.AddWithValue("@middlename", userDataModel.middlename);
                    sqlCmd.Parameters.AddWithValue("@lastname", userDataModel.lastname);
                    sqlCmd.Parameters.AddWithValue("@role", userDataModel.role);
                    sqlCmd.Parameters.AddWithValue("@worker_of", userDataModel.worker_of);
                    sqlCmd.Parameters.AddWithValue("@is_active", userDataModel.is_active);
                    sqlCmd.Parameters.AddWithValue("@last_logged_in_ipaddress", "NA");
                    sqlCmd.Parameters.AddWithValue("@last_logged_in", userDataModel.created_at);
                    sqlCmd.Parameters.AddWithValue("@created_at", userDataModel.created_at);
                    sqlCmd.Parameters.AddWithValue("@updated_at", userDataModel.created_at);

                    sqlCon.Open();
                    int result = sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();

                    if (result > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Int64 SaveChangesWithLastId(UserDataModel userDataModel)
        {
            Int64 result = 0;

            using (MySqlConnection sqlCon = this._mysqlCon)
            {

                string fields = "public_uuid, username, email, mobile, ";
                fields += "password, hint, firstname, middlename, lastname, role, worker_of, ";
                fields += "is_active, last_logged_in_ipaddress, last_logged_in, created_at, updated_at";

                string values = "@public_uuid, @username, @email, @mobile, ";
                values += "@password, @hint, @firstname, @middlename, @lastname, @role, @worker_of, ";
                values += "@is_active, @last_logged_in_ipaddress, @last_logged_in, @created_at, @updated_at";

                string query = string.Format("INSERT INTO users ({0}) VALUES ({1}); SELECT LAST_INSERT_ID();", fields, values);

                using (MySqlCommand sqlCmd = new MySqlCommand(query, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@public_uuid", userDataModel.public_uuid);
                    sqlCmd.Parameters.AddWithValue("@username", userDataModel.username);
                    sqlCmd.Parameters.AddWithValue("@email", userDataModel.email);
                    sqlCmd.Parameters.AddWithValue("@mobile", userDataModel.mobile);
                    sqlCmd.Parameters.AddWithValue("@password", userDataModel.password);
                    sqlCmd.Parameters.AddWithValue("@hint", userDataModel.hint);
                    sqlCmd.Parameters.AddWithValue("@firstname", userDataModel.firstname);
                    sqlCmd.Parameters.AddWithValue("@middlename", userDataModel.middlename);
                    sqlCmd.Parameters.AddWithValue("@lastname", userDataModel.lastname);
                    sqlCmd.Parameters.AddWithValue("@role", userDataModel.role);
                    sqlCmd.Parameters.AddWithValue("@worker_of", userDataModel.worker_of);
                    sqlCmd.Parameters.AddWithValue("@is_active", userDataModel.is_active);
                    sqlCmd.Parameters.AddWithValue("@last_logged_in_ipaddress", "NA");
                    sqlCmd.Parameters.AddWithValue("@last_logged_in", userDataModel.created_at);
                    sqlCmd.Parameters.AddWithValue("@created_at", userDataModel.created_at);
                    sqlCmd.Parameters.AddWithValue("@updated_at", userDataModel.created_at);

                    sqlCon.Open();
                    result = Convert.ToInt64(sqlCmd.ExecuteScalar());
                    sqlCon.Close();
                }
            }

            return result;
        }
    }
}
