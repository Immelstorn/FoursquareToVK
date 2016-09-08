using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace FoursquareToVK
{
    class NetworkClass
    {
        private static NetworkClass _instance;
        private static object _syncLock = new object();
        private static CookieContainer _cookieContainer = new CookieContainer();

        private NetworkClass()
        {
            if (!(File.Exists("log.txt")))
            {
                File.Create("log.txt");
            }
            if (!(File.Exists("threadlog.txt")))
            {
                File.Create("threadlog.txt");
            }
        }

        public static NetworkClass GetNetworkClass()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NetworkClass();
                    }
                }
            }
            return _instance;
        }

        public void GetRequestAndResponse(string uri, out string responseUri, out string stream, string method = "GET")
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            if (request != null)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_7; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.205 Safari/534.16";
                request.CookieContainer = _cookieContainer;
                request.Method = method;
            }
            stream = "";
            responseUri = "";
            if (request != null)
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (Stream receiveStream = response.GetResponseStream())
                        {
                            if (receiveStream != null)
                            {
                                var readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                stream = readStream.ReadToEnd();
                            }
                        }
                        responseUri = response.ResponseUri.ToString();
                    }
                }
        }
        public string GetDataFromXmlNode(XmlNode input)
        {
            if (input == null || String.IsNullOrEmpty(input.InnerText))
            {
                return "";
            }
            return input.InnerText;
        }
        public void WriteLog(string file, string logstring)
        {
            var fi = new FileInfo(file);
            if (fi.Length > 10485760)
            {
                //если больше 10 мб то бекапим лог и создаем новый
                File.Replace(file, file + DateTime.Now, file + "old");
                File.AppendAllText(file, "Old log was renamed to " + file + DateTime.Now);
            }

            File.AppendAllText(file, string.Format("{0}.{1}.{2} {3}:{4}:{5} => {6}\n",
                                      DateTime.Now.Day, DateTime.Now.Date.Month, DateTime.Now.Date.Year,
                                      DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
                                      logstring));
        }
        public void WriteLogToConsole()
        {
            var log = File.ReadAllLines("log.txt");
            Console.WriteLine("***********Log*************");
            foreach (var s in log)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("***********Log*************");

        }
    }
}
