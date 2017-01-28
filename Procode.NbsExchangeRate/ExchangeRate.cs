using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Procode.NbsExchangeRate
{
    public class ExchangeRate
    {
        string JSessionId, FacesViewState;

        private void InitializeSession()
        {
            JSessionId = string.Empty;
            FacesViewState = string.Empty;
            var pageContent = ReadPageWithGetMethod("http://nbs.rs/kursnaListaModul/naZeljeniDan.faces?lang=lat");
            FacesViewState = GetStringBetween(pageContent, "name=\"javax.faces.ViewState\"", "/>");
            FacesViewState = GetStringBetween(FacesViewState, "value=\"", "\"");
            FacesViewState = FacesViewState.Replace(":", "%3A");
        }

        public string GetStringBetween(string source, string start, string end)
        {
            string between = source.Remove(0, source.IndexOf(start) + start.Length);
            between = between.Remove(between.IndexOf(end));
            return between;
        }

        public string ReadPageWithGetMethod(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //request.Host = "nbs.rs";
            request.KeepAlive = true;
            request.Headers.Add("Accept-Language: sr,sr-RS;q=0.8,sr-CS;q=0.6,en-US;q=0.4,en;q=0.2");
            request.Headers.Add("Upgrade-Insecure-Requests: 1");

            var response = (HttpWebResponse)request.GetResponse();
            JSessionId = GetStringBetween(response.Headers[HttpResponseHeader.SetCookie], "JSESSIONID=", ";");
            var responseStream = response.GetResponseStream();
            byte[] buff = new byte[1024];
            int count = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                do
                {
                    count = responseStream.Read(buff, 0, buff.Length);
                    string tmpStr = null;
                    switch (response.CharacterSet)
                    {
                        case "UTF-8":
                            tmpStr = Encoding.UTF8.GetString(buff, 0, count);
                            break;
                        default:
                            tmpStr = Encoding.Unicode.GetString(buff, 0, count);
                            break;
                    }
                    sb.Append(tmpStr);

                }
                while (count > 0);
            }
            finally
            {
                request.Abort();
            }
            return sb.ToString();
        }

        public string ReadPageWithPostMethod(Uri uri, string inputData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language: sr,sr-RS;q=0.8,sr-CS;q=0.6,en-US;q=0.4,en;q=0.2");
            request.Referer = "http://nbs.rs/kursnaListaModul/naZeljeniDan.faces?lang=lat";
            request.Method = "POST";
            request.KeepAlive = false;
            request.CookieContainer = new CookieContainer(1);
            request.CookieContainer.Add(new Cookie("JSESSIONID", JSessionId, string.Empty, uri.Host));
            request.Headers.Add("Upgrade-Insecure-Requests: 1");
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] inputArray = Encoding.UTF8.GetBytes(inputData);
            request.ContentLength = inputArray.Length;
            Stream inputStream = request.GetRequestStream();
            inputStream.Write(inputArray, 0, inputArray.Length);
            inputStream.Close();

            StringBuilder sb = new StringBuilder();
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    JSessionId = response.Headers[HttpResponseHeader.SetCookie];
                    var responseStream = response.GetResponseStream();
                    byte[] buff = new byte[1024];
                    int count = 0;
                    do
                    {
                        count = responseStream.Read(buff, 0, buff.Length);
                        string tmpStr = null;
                        switch (response.CharacterSet)
                        {
                            case "UTF-8":
                                tmpStr = Encoding.UTF8.GetString(buff, 0, count);
                                break;
                            default:
                                tmpStr = Encoding.Unicode.GetString(buff, 0, count);
                                break;
                        }
                        sb.Append(tmpStr);
                    }
                    while (count > 0);
                }
            }
            finally
            {
                request.Abort();
            }
            return sb.ToString();
        }

        public string GetExchangeRate(DateTime date, RateType rateType, ReturnDataType returnDataType)
        {
            if (!SessionInitialized())
                InitializeSession();

            string exchangeRateList = null;
            string input = string.Format("index=index&index:brKursneListe=&index:year={0}&index:inputCalendar1={1}&index:vrsta={2}&index:prikaz={3}&index:buttonShow=Prika%C5%BEi&javax.faces.ViewState={4}",
                date.Year, date.ToString("dd.MM.yyyy."), (int)rateType,
                (int)returnDataType, FacesViewState);
            exchangeRateList = ReadPageWithPostMethod(new Uri("http://nbs.rs/kursnaListaModul/naZeljeniDan.faces"), input);

            return exchangeRateList;
        }

        public string GetExchangeRate(DateTime date)
        {
            return GetExchangeRate(date, RateType, ReturnDataType);
        }

        /// <summary>
        /// This parameterless method will return todays exchange rate.
        /// </summary>
        /// <returns></returns>
        public string GetExchangeRate()
        {
            return GetExchangeRate(DateTime.Now);
        }

        public bool SessionInitialized()
        {
            return !string.IsNullOrEmpty(JSessionId) && !string.IsNullOrEmpty(FacesViewState);
        }

        public ExchangeRate(string host, RateType rateType, ReturnDataType returnDataType)
        {
            Host = host;
            RateType = rateType;
            ReturnDataType = returnDataType;
        }

        /// <summary>
        /// Initializes with default return data type as CVS.
        /// </summary>
        /// <param name="host">Host of NBS</param>
        public ExchangeRate(string host)
            : this(host, DefaultRateType, DefaultReturnDataType)
        {
        }

        public ExchangeRate()
            : this(DefaultHost)
        {
        }

        private const string DefaultHost = "nbs.rs";
        private const RateType DefaultRateType = RateType.Foreign;
        private const ReturnDataType DefaultReturnDataType = ReturnDataType.Csv;

        private string Host;
        private RateType RateType;
        private ReturnDataType ReturnDataType;
    }
}
