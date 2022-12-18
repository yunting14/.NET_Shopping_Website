using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Team2_DotNetCA.Models;
using System.Text.RegularExpressions;

namespace Team2_DotNetCA.Data
{
    public class PurchaseData
    {
        /// <summary>
        /// for test
        /// </summary>
        private string connStr;
        public PurchaseData(string connStr)
        {
            this.connStr = connStr;
        }

        public User GetUserByUsername(string username)
        {
            User user = null;

            string commandText = "SELECT * FROM [User] WHERE Username = @username;";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@username", username);

                conn.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        Name = reader.GetString(3)
                    };
                }
                conn.Close();
            }
            return user;
        }

        public User GetUserBySession(string sessionId)
        {

            User user = null;
            string commandText = "SELECT [User].UserId,[User].Username,[User].[Password],[User].[Name] FROM ShoppingSession, [User] WHERE ShoppingSession.UserId = [User].UserId AND SessionId = @sessionId;";
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@sessionId", sessionId);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        Name = reader.GetString(3)
                    };
                }
                connection.Close();
            }
            return user;

        }

        public string AddSession(int userId)
        {
            string sessionId = null;
            Guid guid = Guid.NewGuid();

            string commandText =
                "INSERT INTO ShoppingCartCA.dbo.ShoppingSession" +
                "(SessionId, UserId) VALUES(@sessionId,@userId);";
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@sessionId", guid);
                command.Parameters.Add("@userId", System.Data.SqlDbType.Int);
                command.Parameters["@userId"].Value = userId;
                if(command.ExecuteNonQuery() == 1)
                {
                    sessionId = guid.ToString();
                }

                connection.Close();
            }

            return sessionId;
        }

        public bool RemoveSession(string sessionId)
        {
            bool status = false;
            string query = String.Format(@"DELETE FROM ShoppingSession WHERE SessionId='{0}';", sessionId);
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if(cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }
                connection.Close();
            }

            return status;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Purchase> GetPurchaseDetails(int userId)
        {
            List<Purchase> purDs = new List<Purchase>();

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand sCom1 = new SqlCommand();
                connection.Open();

                //Get purchase ID by userId
                sCom1.CommandText = "SELECT PurchaseId,PurchaseDate " +
                    "FROM PurchaseHistory " +
                    "WHERE userId = @userId " +
                    "ORDER BY PurchaseDate;";
                sCom1.Parameters.AddWithValue("@userId", userId.ToString());
                sCom1.Connection = connection;
                SqlDataReader reader = sCom1.ExecuteReader();
                while (reader.Read())
                {
                    Purchase pd = new Purchase();
                    pd.PurchaseId = reader.GetInt32(0);
                    pd.PurchaseDate = reader.GetDateTime(1);
                    purDs.Add(pd);
                }
                connection.Close();
            }

            foreach (Purchase pD in purDs)
            {
                pD.p_list = GetProductDetails(pD.PurchaseId);
            }

            return purDs;

        }


        public List<PurchasedProduct> GetProductDetails(int purchaseId)
        {
            List<PurchasedProduct> ProDsList = new List<PurchasedProduct>();

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                SqlCommand sCom2 = new SqlCommand();
                sCom2.CommandText = "SELECT p.ProductImage,pd.ProductId,p.ProductName,p.ProductDescription FROM Product p,PurchaseDetails pd WHERE P.ProductId = pd.ProductId AND pd.PurchaseId = @PurchaseId GROUP BY pd.ProductId,p.ProductImage,p.ProductName,p.ProductDescription;";

                sCom2.Parameters.AddWithValue("@PurchaseId", purchaseId.ToString());
                sCom2.Connection = connection;
                SqlDataReader reader = sCom2.ExecuteReader();
                while (reader.Read())
                {
                    PurchasedProduct proD = new PurchasedProduct();
                    proD.Image = reader.GetString(0);
                    proD.ProductId = reader.GetInt32(1);
                    proD.Name = reader.GetString(2);
                    proD.Details = reader.GetString(3);
                    ProDsList.Add(proD);
                }
                connection.Close();
            }
            foreach (PurchasedProduct pD in ProDsList)
            {
                pD.ACList = GetAC(purchaseId, pD.ProductId);
            }

            return ProDsList;
        }


        public List<Guid> GetAC(int purchaseId, int productId)
        {
            List<Guid> acList = new List<Guid>();

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                SqlCommand sCom3 = new SqlCommand();
                sCom3.CommandText = "SELECT ActivationCode " +
                    "FROM PurchaseDetails " +
                    "WHERE ProductId = @productId " +
                    "AND PurchaseId = @purchaseId;";
                sCom3.Parameters.AddWithValue("@productId", productId.ToString());
                sCom3.Parameters.AddWithValue("@purchaseId", purchaseId.ToString());
                sCom3.Connection = connection;

                SqlDataReader reader = sCom3.ExecuteReader();
                while (reader.Read())
                {
                    Guid ac = new Guid();
                    ac = reader.GetGuid(0);
                    acList.Add(ac);
                }
                connection.Close();
            }
            return acList;
        }



    }
}

