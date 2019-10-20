using DiveLog.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DiveLog.API.Test")]
namespace DiveLog.API.Helpers
{
	public class DiveLogStatHelper : IDiveLogStatHelper
	{

		public Tuple<int, int>CalculateBottomTime(LogEntry logEntry)
		{
			_ = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
			_ = logEntry.DataPoints ?? throw new NullReferenceException("Datapoints are null.");
			
			if (logEntry.DataPoints.Count <= 1)
			{
				throw new InvalidOperationException("At least two data points are required.");
			}

			var extendedDataPoints = CreateDataPointExtendedList(logEntry.DataPoints);
			var result = DeriveBottomTimeFromDataPoints(logEntry.MaxDepth, extendedDataPoints);

			return result;
		}

		private Tuple<int, int> DeriveBottomTimeFromDataPointsv2(List<DataPointExtended> extendedDataPoints)
		{
			return null;
		}

		private Tuple<int, int> DeriveBottomTimeFromDataPoints(decimal maxDepth, List<DataPointExtended> extendedDataPoints)
		{
			var logIntervalSeconds = extendedDataPoints[1].Time - extendedDataPoints[0].Time;
			var holdingDataPoints = new List<DataPointExtended>();

			// Start from half max depth to rule out any stops in shallows at BEGINNING of dive
			var halfMaxDepth = maxDepth / 2;
			var holdingDepth = false;

			// Loop through datapoints
			foreach(var dataPoint in extendedDataPoints)
			{
				// Not concerned about any data points prior to this
				if (dataPoint.Depth >= halfMaxDepth)
				{
					// We are now stationary, at the bottom (in theory)
					if (dataPoint.Holding.HasValue)
					{
						holdingDepth = true;
						holdingDataPoints.Add(dataPoint);
						continue;
					}
					else if (holdingDepth)
					{
						break;
					}
				}

				holdingDepth = false;
			}

			if (!holdingDataPoints.Any())
			{
				// Couldn't calculate so return -1
				return new Tuple<int, int>(-1, -1);
			}

			var meanDepth = Math.Round(holdingDataPoints.Select(x => x.Depth).Average());
			var meanDepthInt = Convert.ToInt32(meanDepth);
			var timeAtDepthSeconds = logIntervalSeconds * holdingDataPoints.Count;
			var timeMintues = TimeSpan.FromSeconds(timeAtDepthSeconds).Minutes;

			return new Tuple<int, int>(meanDepthInt, timeMintues);
		}

		private List<DataPointExtended> CreateDataPointExtendedList(List<DataPoint> dataPoints)
		{
			var extendedDataPoints = new List<DataPointExtended>();
			DataPointExtended previous = null;

			foreach (var datapoint in dataPoints)
			{
				var extendedDataPoint = new DataPointExtended(datapoint.Time, datapoint.Depth, previous);
				extendedDataPoints.Add(extendedDataPoint);
				previous = extendedDataPoint;
			}

			return extendedDataPoints.OrderBy(x => x.Time).ToList(); ;
		}

		internal class DataPointExtended
		{
			private const decimal DepthVariance = 1;
			private DataPointExtended _previous;

			public DataPointExtended(int time, decimal depth, DataPointExtended previous)
			{
				Time = time;
				Depth = depth;
				_previous = previous;

				CalculateStatisics();
			}

			private void CalculateStatisics()
			{
				CalculateDiff();
				CalculateDoubleDiff();
				CalculateDescending();
				CalcualteAscending();
				CalculateHolding();
			}

			private void CalculateDiff()
			{
				if (_previous == null)
				{
					return;
				}

				Diff = Depth - _previous.Depth;
			}

			private void CalculateDoubleDiff()
			{
				if (_previous == null)
				{
					return;
				}

				DoubleDiff = Diff + _previous.Diff;
			}

			private void CalculateDescending()
			{
				if (DoubleDiff > DepthVariance)
				{
					Descending = Depth;
				}
			}

			private void CalcualteAscending()
			{
				if (DoubleDiff < (DepthVariance * -1))
				{
					Ascending = Depth;
				}
			}

			private void CalculateHolding()
			{
				if (DoubleDiff >= (DepthVariance * -1) && DoubleDiff <= DepthVariance)
				{
					Holding = Depth;
				}
			}

			internal int Time { get; private set; }

			internal decimal Depth { get; private set; }

			internal decimal? Diff { get; private set; }

			internal decimal? DoubleDiff { get; private set; }

			internal decimal? Descending { get; private set; }

			internal decimal? Ascending { get; private set; }

			internal decimal? Holding { get; private set; }
		}
	}
}
