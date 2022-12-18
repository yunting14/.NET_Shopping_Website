using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Team2_DotNetCA.Data;
using Team2_DotNetCA.Models;

namespace Team2_DotNetCA.Controllers;

public class HomeController : Controller
{
    private UserData useraccess;

    public HomeController(IConfiguration cfg)
    {
        useraccess = new UserData(cfg.GetConnectionString("db_conn"));
    }

    public IActionResult Index()
    {
        string? usernameInSession = HttpContext.Session.GetString("username");
        if (usernameInSession == null)
        {
            return RedirectToAction("Index", "Browse");

        }
        else
        {
            string? nameInSession = HttpContext.Session.GetString("name");
            ViewData["name"] = nameInSession;
            return RedirectToAction("Index", "Browse");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

