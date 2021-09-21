using System;

namespace Average.Server.Framework.EventsArgs
{
    public class HttpResponseEventArgs : EventArgs
    {
        public int Token { get; }
        public int Status { get; }
        public string Text { get; }
        public dynamic Header { get; }

        public HttpResponseEventArgs(int token, int status, string text, dynamic header)
        {
            Token = token;
            Status = status;
            Text = text;
            Header = header;
        }
    }
}
