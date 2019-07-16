﻿using DiveLog.GUI.Helpers;
using DiveLog.GUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.GUI.Controllers
{
    public class LogController : Controller
    {
        private readonly APIHelper _apiHelper;

        public LogController(APIHelper apiHelper)
        {
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
        }

        public IActionResult LogSearcher()
        {
            return View();
        }

        public async Task<IActionResult> GetDives()
        {
            var dives = await _apiHelper.GetAllDives();

            if (!ModelState.IsValid)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            // Shearwater dive #10
            var dive = dives[8];
            var datapoints = new List<DataPoint>();
            foreach (var dp in dive.DataPoints)
            {
                var datapoint = new DataPoint(dp.Time / 10, Convert.ToDouble(dp.Depth));
                datapoints.Add(datapoint);
            }

            TempData["Datapoints"] = JsonConvert.SerializeObject(datapoints.OrderBy(x => x.X));

            return RedirectToAction("LogSearcher");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
