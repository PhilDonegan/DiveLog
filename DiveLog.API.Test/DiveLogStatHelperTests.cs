using DiveLog.API.Helpers;
using DiveLog.DAL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static DiveLog.API.Helpers.DiveLogStatHelper;

namespace DiveLog.API.Test
{
	[TestClass]
	public class DiveLogStatHelperTests
	{
		private List<DataPointExtended> dataPoints;

		[TestInitialize]
		public void TestSetup()
		{
			dataPoints = new List<DataPointExtended>();

			var dataPoint1 = new DataPointExtended(0, 1.4M, null);
			var dataPoint2 = new DataPointExtended(10, 2.2M, dataPoint1);
			var dataPoint3 = new DataPointExtended(20, 2.8M, dataPoint2);

			dataPoints.Add(dataPoint1);
			dataPoints.Add(dataPoint2);
			dataPoints.Add(dataPoint3);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullLogEntry()
		{
			var helper = new DiveLogStatHelper();
			helper.CalculateBottomTime(null);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void TestNullDataPoints()
		{
			var helper = new DiveLogStatHelper();
			var logEntry = new LogEntry();

			helper.CalculateBottomTime(logEntry);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestMinimumDataPoints()
		{
			var helper = new DiveLogStatHelper();
			var logEntry = new LogEntry();
			var dataPoint = new DataPoint();

			logEntry.DataPoints = new List<DataPoint> { dataPoint };

			helper.CalculateBottomTime(logEntry);
		}

		[TestMethod]
		public void TestDiff()
		{
			Assert.IsNull(dataPoints[0].Diff);
			Assert.AreEqual(0.8M, dataPoints[1].Diff);
			Assert.AreEqual(0.6M, dataPoints[2].Diff);
		}

		[TestMethod]
		public void TestDoubleDiff()
		{
			Assert.IsNull(dataPoints[0].DoubleDiff);
			Assert.IsNull(dataPoints[1].DoubleDiff);
			Assert.AreEqual(1.4M, dataPoints[2].DoubleDiff);
		}

		[TestMethod]
		public void TestDecending()
		{
			Assert.IsNull(dataPoints[0].Decending);
			Assert.IsNull(dataPoints[1].Decending);
			Assert.AreEqual(2.8M, dataPoints[2].Decending);
		}
	}
}
