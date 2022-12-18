using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Models;
using Team2_DotNetCA.Data;

namespace Team2_DotNetCA.Controllers
{
    public class BrowseController : Controller
    {
        public IActionResult Index()
        {
            List<Product> products = ProductData.GetAllProducts();
            ViewData["products"] = products;
            return View();
        }

        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index", "Browse");
            }

            string input = keyword.Trim();

            List<Product> products = SearchData.GetSearchProducts(input);
            ViewData["products"] = products;
            ViewData["keyword"] = keyword;

            return View();
        }

        [HttpPost]
        public Action AddToCart([FromBody]string productId)
        {

            //Guest User Add to Cart
            if (Request.Cookies["SessionId"] == null)
            {

                int? his = 0;
                string? prd = Request.Cookies[productId];
                his = Convert.ToInt32(prd);
                int? numOfPrd = his + 1;
                CookieOptions options = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(10)
                };
                //Save the data on the cookie
                Response.Cookies.Append(productId, numOfPrd.ToString(), options);

                return null;
            }
            else
            {
                User user = ProductData.GetUserBySession(Request.Cookies["SessionId"]);
                AddToAction(productId, user.UserId.ToString());
                return null;
            }


        }
        [HttpPost]
        public Action AddToAction([FromBody] string productId, string userId)
        {
            User user = ProductData.GetUserBySession(Request.Cookies["SessionId"]);
            int cartItem = ProductData.CheckTheCartQuantity(Convert.ToInt32(productId), user.UserId);
            if (cartItem > 0)
            {
                ProductData.UpdateInCart(Convert.ToInt32(productId), user.UserId, cartItem + 1);
            }
            else
            {
                CartData.SaveInCart(Convert.ToInt32(productId), user.UserId, 1);
            }
            return null;
        }





        public ActionResult UserAddItem()
        {
            if (Request.Cookies["SessionId"] != null)
            {
                User user = ProductData.GetUserBySession(Request.Cookies["SessionId"]);
                for (int i = 1; i <= 6; i++)
                {
                    if (Request.Cookies[i.ToString()] == null)
                        continue;
                    else
                    {
                        int quantity = Convert.ToInt32(Request.Cookies[i.ToString()]);
                        //Cart? cart = ProductData.CheckTheCart(i, user.UserId);
                        int cartItem = ProductData.CheckTheCartQuantity(i, user.UserId);
                        //int cartItem = ProductData.CheckTheCartQuantity(i, user.UserId);
                        if (cartItem > 0)
                        {
                            ProductData.UpdateInCart(i, user.UserId, cartItem + quantity);

                            Response.Cookies.Delete(i.ToString());
                        }
                        else
                        {
                            CartData.SaveInCart(i, user.UserId, quantity);
                            Response.Cookies.Delete(i.ToString());
                        }
                    }
                }

                return RedirectToAction("ViewCart", "Cart");
            }
            else
            {
                return RedirectToAction("ViewCart", "Cart");
            }
        }

        public IActionResult ShowQuantity() {
            if (Request.Cookies["SessionId"] != null)
            {
                User user = ProductData.GetUserBySession(Request.Cookies["SessionId"]);
                int total = ProductData.TotalQuantity(user.UserId);
                return Json(new { mattTotal = total });
            }
            else {
                return Json(new {mattTotal=0});
            }

        }



    }





}
