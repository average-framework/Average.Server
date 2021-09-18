using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Average.Server.Framework.Managers
{
    internal class RequestManager
    {
        private readonly RequestInternalManager _request;

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>
        {
            { "Content-Type", "application/json"}
        };

        public RequestManager(RequestInternalManager request)
        {
            _request = request;

            Logger.Write("RequestManager", "Initialized successfully");
        }

        public async Task<RequestResponse> Http(string url, string method = "GET", string data = "", Dictionary<string, string> headers = null)
        {
            headers = (headers == null) ? new Dictionary<string, string>() : headers;
            var response = await _request.Http(url, method, data, headers);
            return ParseRequestResponseInternal(response);
        }

        internal WebHeaderCollection ParseHeadersInternal(dynamic headerDyn)
        {
            var headers = new WebHeaderCollection();
            var headerDict = (IDictionary<string, object>)headerDyn;

            headerDict.ToList().ForEach(x => headers.Add(x.Key, x.Value.ToString()));

            return headers;
        }

        internal HttpStatusCode ParseStatusInternal(int status) => (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), status);

        internal RequestResponse ParseRequestResponseInternal(IDictionary<string, dynamic> rr)
        {
            var result = new RequestResponse();

            result.status = ParseStatusInternal(rr["status"]);
            result.headers = ParseHeadersInternal(rr["headers"]);
            result.content = rr["content"];

            return result;
        }
    }
}
