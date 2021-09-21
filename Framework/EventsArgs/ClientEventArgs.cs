using Average.Server.Framework.Model;
using System;

namespace Average.Server.Framework.EventsArgs
{
    internal class ClientEventArgs : EventArgs
    {
        public Client Client { get; private set; }

        public ClientEventArgs(Client client)
        {
            Client = client;
        }
    }
}
