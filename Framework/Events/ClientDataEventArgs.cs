using Average.Server.Framework.Model;
using System;

namespace Average.Server.Framework.Events
{
    internal class ClientDataEventArgs : EventArgs
    {
        public Client Client { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }

        public ClientDataEventArgs(Client client, string key, object value)
        {
            Client = client;
            Key = key;
            Value = value;
        }
    }
}
