using System;
using System.Threading;


namespace FoursquareToVK
{
    class Program
    {
        private static API _api = API.GetApi();
        private static string _prevcheckin = "";
        private static NetworkClass _network = NetworkClass.GetNetworkClass();
        private static Thread _mainThr;
        static void Main()
        {
            _network.WriteLogToConsole();
            _mainThr = new Thread(MainThread);
            _mainThr.Start();

        }
        private static void MainAfterExeption()
        {
            _network.WriteLog("threadlog.txt","Sleeping after Exception");
            Thread.Sleep(TimeSpan.FromMinutes(10));
            _mainThr = new Thread(MainThread);
            _mainThr.Start();
        }

        private static void MainThread()
        {
            try
            {
                while (true)
                {
                    string shout;
                    var recentCheckin = _api.ParseRecentCheckinFromXML(_api.GetSelf(), out shout);
                    _network.WriteLog("threadlog.txt", "Previous checkin ==> " + _prevcheckin);
                    _network.WriteLog("threadlog.txt", "Recent checkin ==> " + recentCheckin);
                    if (recentCheckin != _prevcheckin)
                    {
                        _network.WriteLog("threadlog.txt", "Entering first if");
                        _prevcheckin = recentCheckin;
                        if (recentCheckin == "Immelstorn's Home")
                        {
                            recentCheckin = "Home";
                        }
                        _api.StatusSet(string.Format("{0} @ {1}", shout, recentCheckin));
                        _network.WriteLog("threadlog.txt", "Status set ==>" + string.Format("{0} @ {1}", shout, recentCheckin));
                        _network.WriteLog("log.txt", string.Format("{0} @ {1}", shout, recentCheckin));
                        Console.WriteLine("{0}.{1}.{2} {3}:{4}:{5} => {6}",
                                          DateTime.Now.Day, DateTime.Now.Date.Month, DateTime.Now.Date.Year,
                                          DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
                                          recentCheckin);
                    }
                    _network.WriteLog("threadlog.txt", "Sleep for 10 minutes\n\n");

                    Thread.Sleep(TimeSpan.FromMinutes(10));
                    _network.WriteLog("threadlog.txt", "Awake");
                }
            }
            catch (Exception e)
            {
                _network.WriteLog("log.txt", e.Message);
                Console.WriteLine("{0}.{1}.{2} {3}:{4}:{5} => {6}",
                                      DateTime.Now.Day, DateTime.Now.Date.Month, DateTime.Now.Date.Year,
                                      DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, e.Message);
                MainAfterExeption();
            }
        }
    }
}
