using System;

namespace Procode.NbsExchangeRate.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ExchangeRate er = new ExchangeRate();
            string date = string.Empty;
            DateTime parsedDate = DateTime.MinValue;
            string cvs = string.Empty;

            if (args.Length > 0)
            {
                date = args[0];
                if (DateTime.TryParseExact(date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out parsedDate))
                {
                    try
                    {
                        cvs = er.GetExchangeRate(parsedDate);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Can't read from NBS. Error: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    System.Console.WriteLine("Expect date in format dd.MM.yyyy");
                    return;
                }
            }
            else
            {
                try
                {
                    cvs = er.GetExchangeRate();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Can't read from NBS for todays date. Error: " + ex.Message);
                    return;
                }
            }

            System.Console.Write(cvs);
        }
    }
}
