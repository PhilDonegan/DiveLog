using DiveLog.API.Helpers;
using DiveLog.DAL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DiveLog.API.Test
{
    [TestClass]
    public class HashGeneratorTests 
    {
        [TestMethod]
        public void TestReliableHash()
        {
            var datapoints = new List<DataPoint>()
            {
                new DataPoint { Id = 1, Depth = 2, LogEntryId = 1, AveragePPO2 = 0.11M, Time = 10, WaterTemp = 21 },
                new DataPoint { Id = 2, Depth = 2.5M, LogEntryId = 1, AveragePPO2 = 0.16M, Time = 20, WaterTemp = 21 }
            };

            var hash = HashGenerator.GenerateKey(datapoints);
            Assert.AreEqual("A354FACF69D9FC64CCA3FEC4DBF4ED93", hash);
        }

        [TestMethod]
        public void TestReliableHashSmallChange()
        {
            var datapoints = new List<DataPoint>()
            {
                new DataPoint { Id = 1, Depth = 2, LogEntryId = 1, AveragePPO2 = 0.11M, Time = 10, WaterTemp = 21 },
                new DataPoint { Id = 2, Depth = 2.51M, LogEntryId = 1, AveragePPO2 = 0.16M, Time = 20, WaterTemp = 21 }
            };

            var hash = HashGenerator.GenerateKey(datapoints);
            Assert.AreNotEqual("A354FACF69D9FC64CCA3FEC4DBF4ED93", hash);
        }
    }
}
