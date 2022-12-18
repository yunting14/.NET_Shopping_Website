using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Models;
using Team2_DotNetCA.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using System.Xml.Linq;
using System.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Team2_DotNetCA.Controllers
{
    public class PurchaseController : Controller
    {
        private PurchaseData PD;
        private RatingChangeTracker RCT;
        private int userId;
 
        public PurchaseController(IConfiguration configuration)
        {
            PD = new PurchaseData(configuration.GetConnectionString("db_conn"));
            RCT = new RatingChangeTracker(configuration.GetConnectionString("db_conn"));
        }


        public IActionResult Index()
        {

            User user = PD.GetUserBySession(Request.Cookies["SessionId"]);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewData["name"] = user.Name;
            List<Purchase> list = PD.GetPurchaseDetails(user.UserId);
            ViewData["list"] = list;
            return View();
        }

        public string GetStarRating()
        {
            User user = ProductData.GetUserBySession(Request.Cookies["SessionId"]);
            
            if(user == null)
            {
                return null;
            }

            RCT.UserId = user.UserId;
            var json = JsonSerializer.Serialize<List<ProductRating>>(RCT.GetStar());
            Console.WriteLine(json);
            return json;
        }
        public string SetStarRating(int ProductId,int Rating)
        {
            User user = ProductData.GetUserBySession(Request.Cookies["SessionId"]);

            RCT.UserId = user.UserId;
            return RCT.SetStarRating(ProductId,Rating);
        }

    }
}

