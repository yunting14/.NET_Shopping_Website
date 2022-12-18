using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Team2_DotNetCA.Controllers
{
    public class LogoutController : Controller
    {
        private UserData useraccess;

        public LogoutController(IConfiguration cfg)
        {
            useraccess = new UserData(cfg.GetConnectionString("db_conn"));
        }

        public IActionResult Logout()
        {

            string sessionid = HttpContext.Session.Id;

            if (sessionid != null)
            {
                Response.Cookies.Delete("UserId");
                Response.Cookies.Delete("SessionId");
                useraccess.RemoveSession(sessionid);
                HttpContext.Session.Clear();
                Debug.WriteLine(sessionid);
            }
            

            return RedirectToAction("Index", "Browse");
        }




    }
}

