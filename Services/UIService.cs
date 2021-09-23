using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;

namespace Average.Server.Services
{
    internal class UIService
    {
        private readonly EventManager _eventManager;

        public UIService(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        internal void LoadFrame(Client client, string frame)
        {
            _eventManager.EmitClient(client, "ui:load_frame", frame);
        }

        internal void DestroyFrame(Client client, string frame)
        {
            _eventManager.EmitClient(client, "ui:destroy_frame", frame);
        }

        internal void SendNui(Client client, string frame, string requestType, object message)
        {
            _eventManager.EmitClient(client, "ui:emit", frame, requestType, message);
        }
    }
}
