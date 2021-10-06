using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Enums;
using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Average.Server.Services
{
    internal class InputService : IService
    {
        private readonly EventService _eventService;
        private readonly ClientService _clientService;
        private readonly List<Input> _inputs = new List<Input>();

        public class Input
        {
            public string Id { get; }
            public Control Key { get; }
            public bool LastConditionState { get; set; }
            public Func<Client, bool> Condition { get; }
            public Action<Client, bool> OnStateChanged { get; }
            public Action<Client> OnKeyReleased { get; }

            public Input(Control key, Func<Client, bool> condition, Action<Client, bool> onStateChanged, Action<Client> onKeyReleased)
            {
                Id = Guid.NewGuid().ToString();
                Key = key;
                Condition = condition;
                OnStateChanged = onStateChanged;
                OnKeyReleased = onKeyReleased;
            }
        }

        public InputService(EventService eventService, ClientService clientService)
        {
            _eventService = eventService;
            _clientService = clientService;

            Logger.Write("InputService", "Initialized successfully");
        }

        [Thread]
        private async Task Update()
        {
            if (_inputs.Count > 0)
            {
                foreach (var client in _clientService.Clients)
                {
                    foreach (var input in _inputs)
                    {
                        var isValidate = input.Condition.Invoke(client);

                        if (input.LastConditionState != isValidate)
                        {
                            input.LastConditionState = isValidate;
                            input.OnStateChanged?.Invoke(client, isValidate);

                            _eventService.EmitClient(client, "input:set_state", input.Id, isValidate);
                        }
                    }
                }

                await BaseScript.Delay(250);
            }
            else
            {
                await BaseScript.Delay(1000);
            }
        }

        internal void OnRegisteringInputs(Client client)
        {
            var newInputs = new List<object>();

            _inputs.ForEach(x => newInputs.Add(new
            {
                x.Id,
                Key = (uint)x.Key
            }));

            _eventService.EmitClient(client, "input:register_inputs", newInputs.ToJson());
        }

        internal Input GetInput(string id)
        {
            return _inputs.Find(x => x.Id == id);
        }

        internal bool IsRegisteredKey(Input input)
        {
            return _inputs.Exists(x => x.Id == input.Id);
        }

        internal InputService RegisterKey(Input input)
        {
            _inputs.Add(input);
            return this;
        }

        internal InputService UnregisterKey(Input input)
        {
            _inputs.Remove(input);
            return this;
        }
    }
}
