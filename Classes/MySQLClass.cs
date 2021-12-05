using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using scabackend.Settings;

namespace scabackend.Classes
{
    public class MySQLClass
    {
        private MySqlConnection mysqlCon { get; set; }

        private string ConnectionString { get; set; }

        public MySQLClass(MySQLSettings mySQLSettings)
        {
            this.ConnectionString = "Server=" + mySQLSettings.host + "; Port=" + mySQLSettings.port + "; Uid=" + mySQLSettings.username + "; Password=" + mySQLSettings.password + "; Database=" + mySQLSettings.database + ";";

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
