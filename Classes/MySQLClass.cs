using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using scabackend.Settings;
using scabackend.Models;

namespace scabackend.Classes
{
    public class MySQLClass
    {
        private MySqlConnection mysqlCon { get; set; }

        private string ConnectionString { get; set; }

        public MySQLClass(MySQLDetails mySQLDetails)
        {
            this.ConnectionString = "Server=" + mySQLDetails.host + "; Port=" + mySQLDetails.port + "; Uid=" + mySQLDetails.username + "; Password=" + mySQLDetails.password + "; Database=" + mySQLDetails.database + ";";

            mysqlCon = new MySqlConnection(this.ConnectionString);
        }

        public bool Open()
        {
            try
            {
                if (IsOpened)
                {
                    Close();
                }
                mysqlCon.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        public bool Close()
        {
            if (mysqlCon.State == ConnectionState.Open)
            {
                mysqlCon.Dispose();
                mysqlCon.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOpened
        {
            get
            {
                if (mysqlCon.State == ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<UserModel> GetUserList(string sqlQuery)
        {
            List<UserModel> userModels = new List<UserModel>();
            using (MySqlConnection sqlCon = mysqlCon)
            {
                using (MySqlCommand sqlCmd = new MySqlCommand(sqlQuery, sqlCon))
                {
                    sqlCon.Open();

                    using (MySqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserModel userModel = new UserModel();
                            userModel.username = reader["username"].ToString();
                            userModel.email = reader["email"].ToString();
                            userModel.mobile = reader["mobile"].ToString();
                            userModel.hint = reader["hint"].ToString();
                            userModel.firstname = reader["firstname"].ToString();
                            userModel.middlename = reader["middlename"] == null ? "NA" : reader["middlename"].ToString();
                            userModel.lastname = reader["lastname"].ToString();
                            userModel.role = reader["role"] == null ? 0 : Convert.ToInt32(reader["role"]);

                            var dd = reader["connected_to"];

                            userModel.connected_to = reader["connected_to"] == null ? 0 : Convert.ToInt64(reader["connected_to"]);

                            userModel.is_active = reader["is_active"] == null ? 0 : Convert.ToInt32(reader["is_active"]);
                            userModel.created_at = Convert.ToDateTime(reader["created_at"]);
                            userModel.updated_at = Convert.ToDateTime(reader["updated_at"]);
                            userModels.Add(userModel);
                        }

                        reader.Close();
                    }

                    sqlCon.Close();
                }
            }
            return userModels;
        }

        public bool Save(string tableName, NameValueCollection nameValueCollection)
        {
            DateTime created_at = DateTime.Now;

            using (MySqlConnection sqlCon = mysqlCon)
            {
                string fields = string.Empty;
                string values = string.Empty;

                for (int i = 0; i < nameValueCollection.Count; i++)
                {
                    fields += nameValueCollection.GetKey(i) + ", ";
                    values += "@" + nameValueCollection.GetKey(i) + ", ";
                }

                fields += "created_at, updated_at";
                values += "@created_at, @updated_at";

                string query = string.Format("INSERT INTO {0} ({1}) VALUES ({2});", tableName, fields, values);

                using (MySqlCommand sqlCmd = new MySqlCommand(query, sqlCon))
                {
                    for (int i = 0; i < nameValueCollection.Count; i++)
                    {
                        sqlCmd.Parameters.AddWithValue("@" + nameValueCollection.GetKey(i), nameValueCollection.Get(i));
                    }

                    sqlCmd.Parameters.AddWithValue("@createdAt", created_at);
                    sqlCmd.Parameters.AddWithValue("@updatedAt", created_at);

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
    }
}
