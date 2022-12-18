using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Team2_DotNetCA.Models;

namespace Team2_DotNetCA.Data
{
    public class UserData
    {
        private string connStr;
        public UserData(string connStr)
        {
            this.connStr = connStr;
        }

        public User UserUsername(string username)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();

                string q = string.Format(@"SELECT * FROM [User]
                WHERE Username = '{0}'", username);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = (int) reader["UserId"],
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Name = reader.GetString(3)
                            };
                        }
                    }
                }

                conn.Close();
            }

            return user;
        }


        public void AddSession(int userId, string sessionId)
        {

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();

                string q = string.Format(@"INSERT INTO ShoppingSession(
                SessionId, UserId) VALUES('{0}', '{1}')",
                        sessionId, userId);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Debug.WriteLine("SUCCESSFUL");
                    }
                }

                conn.Close();
            }

            return;
        }

        public bool RemoveSession(string SessionId)
        {
            

            bool status = false;

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();

                string q = string.Format(@"DELETE FROM ShoppingSession
                WHERE SessionId = '{0}'", SessionId);

                Debug.WriteLine(SessionId);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }

                conn.Close();
            }

            return status;
        }


    }

}

