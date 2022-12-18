using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Team2_DotNetCA.Models;
using System.Collections.Generic;
using System.Web;

namespace Team2_DotNetCA.Data
{
    public class RatingChangeTracker
    {
        public long ChgTimestamp { get; set; }
        private string connStr;
        public int UserId { get; set; }
        public RatingChangeTracker(string connStr)
        {
            this.connStr = connStr;
        }
        
        public List<ProductRating> GetStar()
        {
            List<ProductRating> starList = new List<ProductRating>();
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                Console.WriteLine(UserId);
                string q = String.Format("SELECT * FROM ProductRating WHERE UserId = {0};",UserId);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        { 
                            ProductRating star = new ProductRating()
                            {
                                ProductId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Rating = reader.GetInt32(2)
                            };
                            starList.Add(star);
                            Console.WriteLine(star.ProductId);
                        }
                    }
                    
                }

                conn.Close();
            }
      
            return starList;
        }

        public string SetStarRating(int productId, int rating)
        {
            ChgTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            bool status = SetStar(productId, rating);

            return status ? "success" : "fail";
        }

        public bool SetStar(int productId,int rating)
        {
            if (UpdateStar( productId,  rating))
            {
                return true;
            }

            return AddStar( productId, rating);
        }

        public bool UpdateStar(int productId,int rating)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string q = String.Format(@"update ProductRating set rating = {0} where UserId = {1} and ProductId = {2}",rating,UserId,productId);
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

        public bool AddStar(int productId,int rating)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DB.CONNECTION_STRING))
            {
                conn.Open();
                string q = String.Format("insert into ProductRating values ({0},{1}, {2})", productId, UserId, rating);
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

