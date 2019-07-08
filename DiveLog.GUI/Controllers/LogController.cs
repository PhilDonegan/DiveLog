using DiveLog.GUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiveLog.GUI.Controllers
{
    public class LogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
