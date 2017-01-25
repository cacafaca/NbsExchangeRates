using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procode.NbsExchangeRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Procode.NbsExchangeRate.Tests
{
    [TestClass()]
    public class ExchangeRateExportTests
    {
        [TestMethod()]
        public void Get_Exchange_Rate_For_31_12_2016()
        {
            string expected = "30.12.2016";
            var s = ExchangeRateExport.GetExchangeRate(expected);

            Assert.IsNotNull(s);
            Assert.IsTrue(s.StartsWith("BrojKursneListe"));

            var n = s.Split('\n', '\r');
            Assert.IsTrue(n.Length > 1);
            Assert.AreEqual(expected, s.Split('\n', '\r')[1].Split(',')[1]);

        }

        [TestMethod()]
        public void Get_Today_Exchange_Rate_And_Get_Exchange_Rate_For_31_12_2016()
        {
            // 1.
            var s1 = ExchangeRateExport.GetTodayExchangeRate();
            Assert.IsNotNull(s1);
            Assert.IsTrue(s1.StartsWith("BrojKursneListe"));

            var n = s1.Split('\n', '\r');
            Assert.IsTrue(n.Length > 1);
            DateTime expected1 = DateTime.MinValue;
            DateTime nbsOfficialRateChangeTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            if (DateTime.Now >= nbsOfficialRateChangeTime)
                expected1 = DateTime.Now;
            else
                expected1 = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)); // In case that I run test early in the morning, or at night.
            Assert.AreEqual(expected1.ToString("dd.MM.yyyy"),
                s1.Split('\n', '\r')[1].Split(',')[1]);

            System.Threading.Thread.Sleep(3000);

            // 2.
            string expected2 = "30.12.2016";
            var s2 = ExchangeRateExport.GetExchangeRate(expected2);

            Assert.IsNotNull(s2);
            Assert.IsTrue(s2.StartsWith("BrojKursneListe"));

            var n2 = s2.Split('\n', '\r');
            Assert.IsTrue(n2.Length > 1);
            Assert.AreEqual(expected2, s2.Split('\n', '\r')[1].Split(',')[1]);

        }
    }
}