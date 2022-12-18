using Team2_DotNetCA.Models;
using System.Data.SqlClient;

namespace Team2_DotNetCA.Data
{
    public class CartData
    {
        // GET ALL ITEMS FROM CART THAT BELONGS TO 1 SPECIFIC USER
        public static Cart GetCartItems(int userId)
        {
            Cart cart = new Cart();

            Dictionary<Product, int> cartItems = new Dictionary<Product, int>();

            List<int> user = new List<int>(); // to store the repeated userId for each row when reading SQL

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = @$" select U.UserId, U.[Name], P.ProductId, P.ProductName, P.ProductDescription, P.Price, P.ProductImage, C.Qty
                             from [User] U join Cart C
                             on U.UserId = C.UserId
                             join Product P
                             on P.ProductId = C.ProductId
                             where U.UserId = {userId}
                                ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    user.Add((int)reader["UserId"]);

                    Product product = new Product()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        Description = (string)reader["ProductDescription"],
                        Price = (int)reader["Price"],
                        Image = (string)reader["ProductImage"]
                    };

                    cartItems.Add(product, (int)reader["Qty"]);
                }

                if (user.Count > 0)
                {
                    cart.UserId = user[0];
                }
                else
                    cart.UserId = 0;
                
                cart.CartItems = cartItems;

            }
            
            return cart;

        }

        public static void SaveInCart(int productId, int userId, int quantity)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = String.Format($@"INSERT INTO Cart(productId, 
                    userId,Qty) VALUES({productId},{userId},{quantity})");

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

        public static bool UpdateCartQty(int userId, int productId, int quantity)
        {

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = @$"update Cart set Qty = {quantity} where UserId = {userId} and ProductId = {productId}";

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                    return false;

            }
        }
        public static bool DeleteProductFromCart(int userId, int productId)
        {

            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = @$"delete from Cart where UserId = {userId} and ProductId = {productId}";

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else 
                    return false;

            }
        }

        public static bool DeleteAllUserItemsFromCart(int userId)
        {
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = @$"delete from Cart where UserId = {userId}";

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }

        public static bool TrfFrCartToPurchHist(int userId) //4-STEP process as detailed below
        {
            bool success = false;

            // STEP 1: get cart items from DB by userId
            Cart cart = GetCartItems(userId);

            // STEP 2: remove cart items from DB by UserId
            DeleteAllUserItemsFromCart(userId);

            //STEP 3 & 4
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();

                // STEP 3: update "PurchaseHistory" DB table
                // need to do this step first as purchaseId needs to be created, and add purchaseId into "PurchaseDetails"

                // 3a) select max PurchaseId in from PurchaseHistory and +1 to create new PurchaseId
                int curr_max_id = 0;

                string sql = @$"SELECT COALESCE((select max(PurchaseId) from PurchaseHistory) , 0);";
                //coalesce returns 0 if first parameter is null

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    curr_max_id = (int)reader[""];
                }

                conn.Close();

                int new_purchaseId = curr_max_id + 1;

                // 3b): update "PurchaseHistory" table

                // get current date
                string date = DateTime.Now.ToString("yyyy-MM-dd");

                conn.Open();

                string sql2 = @$"insert into PurchaseHistory values ({new_purchaseId}, {userId}, '{date}')";
                SqlCommand cmd2 = new SqlCommand(sql2, conn);

                if (cmd2.ExecuteNonQuery() == 1)
                {
                    success = true;
                }
                else
                    success = false;

                conn.Close();

                // STEP 4: add items into "PurchaseDetails" DB table

                Dictionary<Product, int> cartItems = cart.CartItems;
                conn.Open();
                foreach (KeyValuePair<Product, int> kvp in cartItems)
                {
                    int productId = kvp.Key.ProductId;
                    int quantity = kvp.Value;

                    for (int i = 0; i < quantity; i++)
                    {
                        Guid AC = Guid.NewGuid();

                        string sql3 = @$"insert into PurchaseDetails values ({new_purchaseId}, {productId}, '{AC}')";
                        SqlCommand cmd3 = new SqlCommand(sql3, conn);

                        if (cmd3.ExecuteNonQuery() == 1)
                        {
                            success = true;
                        }
                        else
                            success = false;
                    }
                }
            }

            return success;
        }

        public static Cart CheckTheCart(int productId, int userId)
        {
            Cart cart = null;

            using (SqlConnection conn = new SqlConnection
            (DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = String.Format($@"SELECT * From Cart WHERE UserId = {userId} AND ProductID = {productId}");
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cart = new Cart
                            {
                                UserId = reader.GetInt32(0),
                                ProductId = reader.GetInt32(1),
                                Quantity = reader.GetInt32(2)
                            };
                        }
                    }
                }
                conn.Close();
            }
            return cart;
        }

    }
}
