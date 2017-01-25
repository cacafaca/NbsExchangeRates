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
    public class ExchangeRateTests
    {
        [TestMethod()]
        public void Get_Todays_Exchange_Rate()
        {
            var er = new ExchangeRate();
            var s = er.GetExchangeRate();

            Assert.IsNotNull(s);
            Assert.IsTrue(s.StartsWith("BrojKursneListe"));

            var n = s.Split('\n', '\r');
            Assert.IsTrue(n.Length > 1);
            DateTime expected = DateTime.MinValue;
            DateTime nbsOfficialRateChangeTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            if (DateTime.Now >= nbsOfficialRateChangeTime)
                expected = DateTime.Now;
            else
                expected = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)); // In case that I run test early in the morning, or at night.
            Assert.AreEqual(expected.ToString("dd.MM.yyyy"),
                s.Split('\n', '\r')[1].Split(',')[1]);
        }

        [TestMethod()]
        public void Get_Rate_For_Day_09_01_2017()
        {
            DateTime expected = new DateTime(2017, 1, 9);
            var er = new ExchangeRate();
            var s = er.GetExchangeRate(expected);

            Assert.IsNotNull(s);
            Assert.IsTrue(s.StartsWith("BrojKursneListe"));

            var n = s.Split('\n', '\r');
            Assert.IsTrue(n.Length > 1);
            Assert.AreEqual(expected.ToString("dd.MM.yyyy"),
                s.Split('\n', '\r')[1].Split(',')[1]);
        } 
    }
}