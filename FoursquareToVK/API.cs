using System;
using System.Linq;
using System.Xml;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace FoursquareToVK
{
    class API
    {
        private XmlDocument _result = new XmlDocument();
        private NameValueCollection _qs = new NameValueCollection();
        private const string AccessToken = "";
        private string _responseUri, _stream;
        private const string FoursquareToken = "";
        private static API _instance;
        private static object _syncLock = new object();
        private NetworkClass _network;

        private API()
        {
            _network = NetworkClass.GetNetworkClass();
        }

        //дас ист синглтооон!
        public static API GetApi()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new API();
                    }
                }
            }
            return _instance;
        }

        private XmlDocument ExecuteCommand(string name, NameValueCollection qs)
        {
            var url = String.Format("https://api.vkontakte.ru/method/{0}.xml?access_token={1}&{2}", name, AccessToken, String.Join("&", from item in qs.AllKeys select item + "=" + qs[item]));
            _network.GetRequestAndResponse(url, out _responseUri, out _stream);
            _result.LoadXml(_stream);

            return _result;
        }
        private XmlDocument Execute4sqCommand(string name)
        {
            var url = String.Format("https://api.foursquare.com/v2/{0}&oauth_token={1}&v=20120621", name, FoursquareToken);
            _network.GetRequestAndResponse(url, out _responseUri, out _stream);
            return JsonConvert.DeserializeXmlNode(_stream, "baseprop");
        }

        public XmlDocument Exit()
        {
            _result.Load("http://api.vk.com/oauth/logout?client_id=2947179");
            return _result;
        }
        public XmlDocument StatusSet(string text)
        {
            _qs["text"] = text;
            return ExecuteCommand("status.set", _qs);
        }
        public XmlDocument GetSelf()
        {
            return Execute4sqCommand("users/self/checkins?limit=1");
        }
        public string ParseRecentCheckinFromXML(XmlDocument user, out string shout)
        {
            shout = _network.GetDataFromXmlNode(user.SelectSingleNode("/baseprop/response/checkins/items/shout"));
            return _network.GetDataFromXmlNode(user.SelectSingleNode("/baseprop/response/checkins/items/venue/name"));
        }
    }
}
