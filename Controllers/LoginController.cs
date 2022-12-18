using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Data;
using Team2_DotNetCA.Models;

namespace Team2_DotNetCA.Controllers
{
    public class LoginController : Controller
    {
        private UserData useraccess;
        private PurchaseData PD;

        public LoginController(IConfiguration cfg)
        {
            useraccess = new UserData(cfg.GetConnectionString("db_conn"));
            PD = new PurchaseData(cfg.GetConnectionString("db_conn"));
        }


        [HttpGet]
        public IActionResult Index()
        {
            string? usernameInSession = HttpContext.Session.GetString("username");
            if (usernameInSession == null)
            {
                return View();
            }
            else
            {
                ViewData["username"] = usernameInSession;
                return RedirectToAction("Index", "Browse");
            }
        }

        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {

            string Username = form["Username"];
            string Password = form["Password"];

            User user = useraccess.UserUsername(Username);
            if (user != null)
            {
                string? returnURL = (string)TempData["returnURL"];

                if (user.Password == Password && returnURL != null)
                {
                    

                    string sessionId = HttpContext.Session.Id;
                    useraccess.AddSession(user.UserId, sessionId);

                    Response.Cookies.Append("SessionId", sessionId);

                    HttpContext.Session.SetString("username", Username);
                    HttpContext.Session.SetString("name", user.Name);

                    string? nameInLogin = HttpContext.Session.GetString("name");
                    ViewData["nameinlogin"] = nameInLogin;


                    CookiesToCartDB(user.UserId, sessionId);

                    return Redirect(returnURL);


                }
                else
                {
                    ViewData["UserLoginFailed"] = "Username/Password is incorrect";
                }

            }
            ViewData["UserLoginFailed"] = "Username/Password is incorrect";
            return View();
        }

        [HttpPost]
        public IActionResult IsLogin()
        {
            string? usernameInSession = HttpContext.Session.GetString("name");
            if (usernameInSession == null)
            {
                return Json(new { isLogin = false });

            }
            else
            {
                return Json(new { isLogin = true, username = usernameInSession });
            }
        }


        public bool CookiesToCartDB(int userId, string sessionId)
        {
            bool success = false;

            // check if user has logged in --> found in ShoppingSesionDB
            User user = ProductData.GetUserBySession(sessionId);
            if (user.UserId != userId)
                return false;

            // Pull data from cookies and save to Cart DB. Delete if successfully added.
            for (int i = 1; i <= 6; i++)
            {
                if (Request.Cookies.ContainsKey(i.ToString()))
                {
                    int productId = i;

                    string qty_string = Request.Cookies[i.ToString()];
                    int qty = Convert.ToInt32(qty_string);
                    try
                    {
                        CartData.SaveInCart(productId, userId, qty);
                    } //try create cart data if data already inside cart db, catch will update the qty
                    catch
                    {
                        int cartItem = ProductData.CheckTheCartQuantity(i, user.UserId);
                        qty = qty + cartItem; //ver3.4 update
                        ProductData.UpdateInCart(productId, userId, qty);
                    }
                    

                    // Check if product successfully updated. If yes, delete product from cookies 
                    Cart cart = CartData.CheckTheCart(productId, userId);
                    if (cart.Quantity == qty)
                    {
                        Response.Cookies.Delete(i.ToString());
                        success = true;
                    }
                }
            }

            return success;
        }


    }
}

