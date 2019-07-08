using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiveLog.Web.Helpers;
using DiveLog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace DiveLog.Web.Pages
{
    public class LogViewerModel : PageModel
    {
        public async Task<IActionResult> OnGetDiveLogs()
        {
            var dives = await APIHelper.GetAllDives();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Shearwater dive #4
            var first = dives[2];
            var datapoints = new List<DataPoint>();
            foreach (var dp in first.DataPoints)
            {
                var datapoint = new DataPoint(dp.Time / 10, Convert.ToDouble(dp.Depth));
                datapoints.Add(datapoint);
            }

            ViewData["Datapoints"] = JsonConvert.SerializeObject(datapoints.OrderBy(x => x.X));

            return Page();
        }
    }
}