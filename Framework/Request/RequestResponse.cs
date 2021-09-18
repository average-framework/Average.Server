using System.Net;

namespace Average.Server.Framework.Request
{
    public struct RequestResponse
    {
        public HttpStatusCode status;
        public WebHeaderCollection headers;
        public string content;
    }
}
