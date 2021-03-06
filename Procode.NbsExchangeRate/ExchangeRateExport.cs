﻿using RGiesecke.DllExport;
using System;

namespace Procode.NbsExchangeRate
{
    public class ExchangeRateExport
    {
        [DllExport("GetTodayExchangeRate", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string GetTodayExchangeRate()
        {
            var er = new ExchangeRate();
            return er.GetExchangeRate();
        }

        [DllExport("GetExchangeRate", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string GetExchangeRate(string date)
        {
            if (date.EndsWith("."))
            {
                date = date.Remove(date.Length - 1, 1);
            }

            IFormatProvider fp = new MyFormat();
            DateTime convertedDate;
            if (DateTime.TryParseExact(date, "dd.MM.yyyy", fp, System.Globalization.DateTimeStyles.AssumeLocal, out convertedDate))
            {
                var er = new ExchangeRate();
                return er.GetExchangeRate(convertedDate);
            }
            else
            {
                throw new ArgumentException(string.Format("Date '{0}' is not in correct format.", date));
            }
        }
    }

    public class MyFormat : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            //throw new NotImplementedException();
            return this;
        }
    }
}
