using DiveLog.DAL.Models;
using System;
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

			throw new NotImplementedException();
		}

		internal class DataPointExtended
		{
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

			}

			private void CalcualteAscending()
			{

			}

			private void CalculateHolding()
			{

			}

			internal int Time { get; private set; }

			internal decimal Depth { get; private set; }

			internal decimal? Diff { get; private set; }

			internal decimal? DoubleDiff { get; private set; }

			internal decimal? Decending { get; private set; }

			internal decimal? Ascending { get; private set; }

			internal decimal? Holding { get; private set; }
		}
	}
}
