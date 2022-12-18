using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Models;
using Team2_DotNetCA.Data;
using System.Diagnostics;

namespace Team2_DotNetCA.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewCart(int userId)
        {

            string? sessionId = Request.Cookies["SessionId"];
            
            // if: not logged-in = cart information stored in cookies
            if (sessionId == null)
            {

                Cart cart_guest = new Cart();

                for (int i = 1; i <= 6; i++) 
                {
                    string? quantity = Request.Cookies[i.ToString()];

                    if (quantity == null) 
                        continue;
                    else 
                    {
                        int productId = i;

                        Product product = ProductData.GetProductById(productId);

                        cart_guest.CartItems.Add(product, Convert.ToInt32(quantity));
                    }
                }

                ViewData["cart"] = cart_guest;

                return View();
            }

            // else: there is sessionId = user has logged in
            else
            {
                User user = ProductData.GetUserBySession(sessionId);

                Cart cart1 = CartData.GetCartItems(user.UserId);

                ViewData["cart"] = cart1;

                return View();
            }

        }

        [HttpPost]
        public IActionResult UpdateCart([FromBody] Cart cart)
        {

            Debug.WriteLine(cart.Quantity + " " + cart.ProductId + " " + cart.UserId);

            int uId = cart.UserId;

            if (uId == null || uId == 0)
            {
                return RedirectToAction("UpdateCartCookies", new { prodId = cart.ProductId, qty = cart.Quantity });
            }
            else
            {
                return RedirectToAction("UpdateCartDB", new { uId = cart.UserId, prodId = cart.ProductId, qty = cart.Quantity });
            }
        }

        // for guests who have not logged in
        public IActionResult UpdateCartCookies(int prodId, int qty)
        {
            if (qty == 0)
            {
                // remove product if quantity is 0. Need to refresh to remove product from View Cart page
                Response.Cookies.Delete(prodId.ToString());
            }
            else if (qty >= 1)
            {
                //update cookie with product's new value
                Response.Cookies.Delete(prodId.ToString());
                Response.Cookies.Append(prodId.ToString(), qty.ToString());
            }

            // check if cookie is successfully added
            if (Request.Cookies[prodId.ToString()] == qty.ToString())
            {
                return Json(new { isSuccess = true }); // create new json obj which contains the response text
            }
            else
            {
                return Json(new { isSuccess = false });
            }
        }

        // for guests who has logged in 
        public IActionResult UpdateCartDB(int uId, int prodId, int qty) 
        {
            string sessionId = Request.Cookies["sessionId"];

            User? user = ProductData.GetUserBySession(sessionId);

            if (user != null) //userId found in ShoppingSession db
            {
                if (qty == 0)
                {
                    bool removeProd = CartData.DeleteProductFromCart(uId, prodId);

                    if (removeProd)
                        return Json(new { isSuccess = true });
                    else
                        return Json(new { isSuccess = false });
                }
                else
                {
                    bool updateProdinCart = CartData.UpdateCartQty(uId, prodId, qty);

                    if (updateProdinCart)
                        return Json(new { isSuccess = true });
                    else
                        return Json(new { isSuccess = false });
                }
            }
            else //userId not found in ShoppingSession db
            {
                return Json(new { isSuccess = false });
            }
        }
    }
}
