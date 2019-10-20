using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DiveLog.GUI.Helpers;
using DiveLog.GUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiveLog.GUI.Controllers
{
    public class LogComparisonController : Controller
    {
		private readonly APIHelper _apiHelper;

		public LogComparisonController(APIHelper apiHelper)
		{
			_apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
		}

        public IActionResult LogComparison()
        {
            return View();
        }

		public async Task<IActionResult> LogComparisonResults(LogComparisonModel model)
		{
			var dives = await _apiHelper.CompareDives(model.Depth, model.Time, model.DiveType);

			if (!ModelState.IsValid)
			{
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}

			var resultsModel = new LogComparisonResultsModel();
			foreach(var dive in dives)
			{
				var diveProfile = new DiveProfile();
				foreach(var dp in dive.DataPoints.OrderBy(x => x.Time))
				{
					var datapoint = new DataPoint(dp.Time, Convert.ToDouble(dp.Depth));
					diveProfile.Datapoints.Add(datapoint);
				}

				resultsModel.DiveProfiles.Add(diveProfile);
			}

			return View(resultsModel);
		}
	}
}