using DemoAppAdo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DemoAppAdo.Controllers
{


    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This will look for Index.cshtml
        }
    }
}
