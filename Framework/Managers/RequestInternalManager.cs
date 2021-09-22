using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Request;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Framework.Managers
{
    internal class RequestInternalManager
    {
        private readonly EventManager _event;

        private readonly Dictionary<int, Dictionary<string, dynamic>> _responseDictionary = new Dictionary<int, Dictionary<string, dynamic>>();

        public RequestInternalManager(EventManager @event)
        {
            _event = @event;

            #region Events

            _event.HttpResponse += HttpResponse;

            #endregion

            Logger.Write("RequestInternalManager", "Initialized successfully");
        }

        public void HttpResponse(object sender, HttpResponseEventArgs e)
        {
            Logger.Debug($"Receive http response: {e.Token}, {e.Status}, {e.Text}, {e.Header}");
            Response(e.Token, e.Status, e.Text, e.Header);
        }

        public void Response(int token, int status, string text, dynamic header)
        {
            var response = new Dictionary<string, dynamic>();

            response["headers"] = header;
            response["status"] = status;
            response["content"] = text;

            _responseDictionary[token] = response;
        }

        public async Task<Dictionary<string, dynamic>> Http(string url, string method, string data, dynamic headers)
        {
            var requestData = new RequestDataInternal();

            requestData.url = url;
            requestData.method = method;
            requestData.data = data;
            requestData.headers = headers;

            var json = JsonConvert.SerializeObject(requestData);
            var token = API.PerformHttpRequestInternal(json, json.Length);

            while (!_responseDictionary.ContainsKey(token)) await BaseScript.Delay(0);

            var res = _responseDictionary[token];
            _responseDictionary.Remove(token);

            return res;
        }
    }
}
