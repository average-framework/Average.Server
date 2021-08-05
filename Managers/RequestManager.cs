using SDK.Server;
using SDK.Server.Interfaces;
using SDK.Shared.Request;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Average.Server.Managers
{
    public class RequestManager : IRequestManager
    {
        Framework framework;

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>
        {
            { "Content-Type", "application/json"}
        };

        public RequestManager(Framework framework)
        {
            this.framework = framework;
        }

        public async Task<RequestResponse> Http(string url, string method = "GET", string data = "", Dictionary<string, string> headers = null)
        {
            await framework.IsReadyAsync();

            headers = (headers == null) ? new Dictionary<string, string>() : headers;
            //var request = await framework.Internal.GetPluginInstance<RequestInternal>("Request.Server.RequestInternal");
            var response = await framework.RequestInternal.Http(url, method, data, headers);
            return ParseRequestResponseInternal(response);
        }

        public WebHeaderCollection ParseHeadersInternal(dynamic headerDyn)
        {
            var headers = new WebHeaderCollection();
            var headerDict = (IDictionary<string, object>)headerDyn;

            foreach (KeyValuePair<string, object> entry in headerDict)
            {
                headers.Add(entry.Key, entry.Value.ToString());
            }

            return headers;
        }

        public HttpStatusCode ParseStatusInternal(int status) => (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), status);

        public RequestResponse ParseRequestResponseInternal(IDictionary<string, dynamic> rr)
        {
            var result = new RequestResponse();
            result.status = ParseStatusInternal(rr["status"]);
            result.headers = ParseHeadersInternal(rr["headers"]);
            result.content = rr["content"];
            return result;
        }
    }
}
