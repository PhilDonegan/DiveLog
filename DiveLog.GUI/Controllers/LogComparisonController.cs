using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiveLog.GUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiveLog.GUI.Controllers
{
    public class LogComparisonController : Controller
    {
        public IActionResult LogComparison()
        {
            return View();
        }

		public IActionResult LogComparisonResults(LogComparisonModel model)
		{
			return RedirectToAction("LogComparison");
		}
	}
}