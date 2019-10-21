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

        public async Task<IActionResult> LogComparison()
        {
			var availableComparisons = await _apiHelper.GetAvailableComparisons();
			var model = new LogComparisonModel();
			model.AvailableComparisons = availableComparisons;

            return View(model);
        }

		public async Task<IActionResult> LogComparisonResults(LogComparisonModel model)
		{
			//return await OriginalComparison(model);
			return await EndLoadedComparison(model);
		}

		private async Task<IActionResult> EndLoadedComparison(LogComparisonModel model)
		{
			var dives = await _apiHelper.CompareDives(model.Depth, model.Time, model.DiveType);

			if (!ModelState.IsValid)
			{
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}

			var resultsModel = new LogComparisonResultsModel();
			if (dives.Any())
			{
				var longestDive = dives.OrderByDescending(x => x.DiveLength).First();
				foreach (var dive in dives)
				{
					var diveProfile = new DiveProfile();
					var diveOffset = longestDive.DiveLength.Subtract(dive.DiveLength);
					var orderedDatapoints = dive.DataPoints.OrderBy(x => x.Time).ToList();
					var logInterval = orderedDatapoints[1].Time - orderedDatapoints[0].Time;

					var blankValuesCount = diveOffset.TotalSeconds / logInterval;
					var count = 0;
					while (blankValuesCount > 0)
					{
						diveProfile.Datapoints.Add(new DataPoint(count * logInterval, 0));
						count++;
						blankValuesCount--;
					}

					foreach (var dp in orderedDatapoints)
					{
						var datapoint = new DataPoint(dp.Time + diveOffset.TotalSeconds, Convert.ToDouble(dp.Depth));
						diveProfile.Datapoints.Add(datapoint);
					}

					resultsModel.DiveProfiles.Add(diveProfile);
				}
			}

			return View(resultsModel);
		}

		private async Task<IActionResult> OriginalComparison(LogComparisonModel model)
		{
			var dives = await _apiHelper.CompareDives(model.Depth, model.Time, model.DiveType);

			if (!ModelState.IsValid)
			{
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}

			var resultsModel = new LogComparisonResultsModel();
			foreach (var dive in dives)
			{
				var diveProfile = new DiveProfile();
				foreach (var dp in dive.DataPoints.OrderBy(x => x.Time))
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