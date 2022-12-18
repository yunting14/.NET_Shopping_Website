using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Data;
using Team2_DotNetCA.Controllers;

namespace Team2_DotNetCA.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CheckoutHandler(int uId)
        {
            
            if (uId == 0) 
            {
                // GUEST USERS

                // STEP 1: redirect to login page for user to log in
                return RedirectToAction("Index", "Login"); 

                // STEP 2: after login, add cart information from cookies to database --> using /Login/CookiesToCartDB
            }
            else 
            {
                // FOR USERS WHO ALREADY LOGGED IN
                
                // remove items from Cart and add to purchase history
                bool checkoutCart = CartData.TrfFrCartToPurchHist(uId);

                // check if successful 
                if (checkoutCart)
                {
                    return RedirectToAction("Index", "Purchase"); //go to view Purchase History
                }
                else
                {
                    return RedirectToAction("ViewCart", "Cart"); // if checkout unsuccessful, go back to View Cart
                }
            }
        }
    }
}
