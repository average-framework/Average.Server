using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using SDK.Server.Diagnostics;
using SDK.Server.Events;
using SDK.Server.Interfaces;
using SDK.Shared.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class RequestInternalManager : IRequestInternalManager
    {
        private Dictionary<int, Dictionary<string, dynamic>> _responseDictionary = new Dictionary<int, Dictionary<string, dynamic>>();

        public RequestInternalManager()
        {
            Main.eventManager.HttpResponse += HttpResponse;
        }

        public void HttpResponse(object sender, HttpResponseEventArgs e)
        {
            Log.Debug($"Receive http response: {e.Token}, {e.Status}, {e.Text}, {e.Header}");
            Response(e.Token, e.Status, e.Text, e.Header);
        }

        public void Response(int token, int status, string text, dynamic header)
        {
            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
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

            var json = await JsonConvert.SerializeObjectAsync(requestData);
            var token = API.PerformHttpRequestInternal(json, json.Length);

            while (!_responseDictionary.ContainsKey(token)) await BaseScript.Delay(0);

            var res = _responseDictionary[token];
            _responseDictionary.Remove(token);
            return res;
        }
    }
}
