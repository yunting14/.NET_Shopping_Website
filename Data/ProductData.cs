using Team2_DotNetCA.Models;
using Team2_DotNetCA.Data;
using System.Data.SqlClient;

namespace Team2_DotNetCA.Data

{
    public class ProductData
    {
        public static List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = @"SELECT * FROM Product";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        Description = (string)reader["ProductDescription"],
                        Price = (int)reader["Price"],
                        Image = (string)reader["ProductImage"],
                        Rating = GetProductRating((int)reader["ProductId"])
                    };
                    products.Add(product);
                }
            }
            return products;

        }

        public static Product GetProductById(int productId)
        {
            Product product = new Product();

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = @$"SELECT * FROM Product where ProductId = {productId}";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    product.ProductId = (int)reader["ProductId"];
                    product.ProductName = (string)reader["ProductName"];
                    product.Description = (string)reader["ProductDescription"];
                    product.Price = (int)reader["Price"];
                    product.Image = (string)reader["ProductImage"];
                    product.Rating = GetProductRating(productId);
                }
            }

            return product;
        }

        public static int GetProductRating(int ProductId)
        {
            int productsRating = 0;

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = $@"select AVG(Rating) as PR from ProductRating group by ProductId having ProductId = {ProductId}";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    productsRating = (int)reader["PR"];
                }
            }
            return productsRating;

        }

        public static User GetUserBySession(string sessionId)
        {
            User user = new User();
            if(sessionId == null)
            {
                return null;
            }

            
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = string.Format($@"SELECT u.UserID, u.Username, u.Password 
                                            FROM ShoppingSession ss
                                            JOIN [User] u 
                                            on ss.userId = u.userID
                                            WHERE sessionId = '{sessionId}'");

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    user.UserId = (int)reader["UserID"];
                    user.Username = (string)reader["Username"];
                    user.Password = (string)reader["Password"];
                }
            }

            if (user.UserId == 0 || user.Username == null || user.Password == null)
            {
                user = null;
                return user;
            }
            else
            {
                return user;
            }
        }

        public static void UpdateInCart(int productId, int userId, int quantity)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = String.Format($@"Update Cart SET Qty = {quantity} WHERE 
                    userId = {userId} AND productID ={productId}");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }

                conn.Close();
            }

        }



        public static int CheckTheCartQuantity(int productId, int userId)
        {

            int quantity = 0;
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = String.Format($@"SELECT Sum(Qty) From Cart  
                    WHERE userId = {userId} AND productID = {productId}");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantity = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        }
                    }

                }

                conn.Close();
            }
            return quantity;

        }

        public static int TotalQuantity(int userId)
        {

            int quantity = 0;
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = String.Format($@"SELECT Sum(Qty) From Cart  
                    WHERE userId = {userId}");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantity = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        }
                    }

                }

                conn.Close();
            }
            return quantity;

        }

    }
}
