using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Services;

namespace Average.Server.Handlers
{
    internal class InputHandler : IHandler
    {
        private readonly InputService _inputService;

        public InputHandler(InputService inputService)
        {
            _inputService = inputService;
        }

        [ServerEvent("input:triggered")]
        private void OnInputTriggered(Client client, string id)
        {
            var input = _inputService.GetInput(id);
            input?.OnKeyReleased?.Invoke(client);
        }
    }
}
