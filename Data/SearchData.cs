using Team2_DotNetCA.Models;
using Team2_DotNetCA.Data;
using System.Data.SqlClient;

namespace Team2_DotNetCA.Data

{
    public class SearchData
    {
        public static List<Product> GetSearchProducts(string keyword)
        {
            List<Product> products = new List<Product>();
            
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string sql = $@"SELECT * FROM Product where Product.ProductName like '%{keyword}%' or Product.ProductDescription like '%{keyword}%' ";
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
                        Rating = ProductData.GetProductRating((int)reader["ProductId"])
                    };
                    products.Add(product);
                }
                conn.Close();
            }
            
            return products;

        }
    }
}
