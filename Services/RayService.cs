using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Events;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Server.Ray;
using Average.Shared.Enums;
using Average.Shared.Models;
using System;
using System.Collections.Generic;
using static Average.Server.Services.InputService;

namespace Average.Server.Services
{
    internal class RayService : IService
    {
        private readonly UIService _uiService;
        private readonly InputService _inputService;
        private readonly ClientService _clientService;

        public Action<Client, RaycastHit, bool> CrossairCondition { get; set; }

        private readonly List<RayGroup> _menus = new();

        public const float MaxTargetRange = 6f;
        private const int CrossairTransitionDuration = 150;

        private readonly RayGroup _mainGroup = new RayGroup("main");

        public RayService(UIService uiService, InputService inputService, ClientService clientService)
        {
            _uiService = uiService;
            _inputService = inputService;
            _clientService = clientService;

            var testGroup = new RayGroup("testGroup");

            _mainGroup.AddItem(new RayItem("Debug 1", "", false,
                action: (client, raycast) =>
                {
                    Logger.Debug("Debug 1");
                    Open(client, testGroup);
                },
                condition: async (client, raycast) =>
                {
                    return client.Name == "bub.bl";
                }));

            testGroup.AddItem(new RayItem("Debug 2", "", true,
                action: (client, raycast) =>
                {
                    Logger.Debug("Debug 2");
                },
                condition: async (client, raycast) =>
                {
                    return true;
                }));

            AddGroup(_mainGroup);
            AddGroup(testGroup);

            // Inputs
            _inputService.RegisterKey(new Input((Control)0x8CC9CD42,
            condition: (client) =>
            {
                return _menus.Count > 0;
            },
            onStateChanged: (client, state) =>
            {
                Logger.Debug($"Client {client.Name} can open/close N3 menu");
            },
            onKeyReleased: async (client) =>
            {
                ShowMenu(client);
                SetVisibility(client, true);
                ShowGroup(client);

                _uiService.FocusFrame(client, "ray");
                _uiService.Focus(client);

                SetCrossairVisibility(client, true, CrossairTransitionDuration);

                Logger.Debug($"Client {client.Name} open N3 menu");
            }));

            Logger.Write("RayService", "Initialized successfully");
        }

        internal void OnClientWindowInitialized(Client client)
        {
            _uiService.LoadFrame(client, "ray");
            _uiService.SetZIndex(client, "ray", 60000);
        }

        internal void OnClientInitialized(Client client)
        {
            _clientService.OnShareData(client, "Ray:Histories", new List<RayGroup>());
            _clientService.OnShareData(client, "Ray:IsOpen", false);
            _clientService.OnShareData(client, "Ray:CurrentGroup", _mainGroup);

            AddHistory(client, _mainGroup);
        }

        internal void OnClick(Client client, string itemId)
        {
            var raycast = _clientService.GetData<RaycastHit>(client, "Character:CurrentRaycast") ?? new RaycastHit();
            var currentGroup = _clientService.GetData<RayGroup>(client, "Ray:CurrentGroup");

            var item = _menus.Find(x => x.Name == currentGroup.Name)[itemId];
            if (item == null) return;

            item.Action?.Invoke(client, raycast);

            if (item.CloseMenuOnAction)
            {
                OnPrevious(client);
                CloseMenu(client);
                _uiService.Unfocus(client);
            }
        }

        private void SetVisibility(Client client, bool isVisible, int fadeDuration = 100) => _uiService.SendMessage(client, "ray", "visibility", new
        {
            isVisible,
            fade = fadeDuration
        });

        internal void AddGroup(RayGroup group)
        {
            if (!_menus.Exists(x => x.Name == group.Name))
            {
                _menus.Add(group);
            }
        }

        private async void ShowGroup(Client client)
        {
            var raycast = _clientService.GetData<RaycastHit>(client, "Character:CurrentRaycast") ?? new RaycastHit();
            var items = _menus.Find(x => x.Name == _mainGroup.Name);

            foreach (var item in items)
            {
                var result = await item.Condition(client, raycast);
                item.IsVisible = result;
            }

            OnRender(client, items);
        }

        internal async void Open(Client client, RayGroup group)
        {
            var raycast = _clientService.GetData<RaycastHit>(client, "Character:CurrentRaycast") ?? new RaycastHit();

            _clientService.OnShareData(client, "Ray:CurrentGroup", group);

            foreach (var item in group)
            {
                var result = await item.Condition(client, raycast);
                item.IsVisible = result;
            }

            AddHistory(client, group);
            OnRender(client, group);
            SetCrossairVisibility(client, true, CrossairTransitionDuration);
            ShowMenu(client);

            if (!_clientService.GetData<bool>(client, "Ray:IsOpen"))
            {
                _clientService.OnShareData(client, "Ray:IsOpen", true);
            }
        }

        internal void OnPrevious(Client client)
        {
            if (_clientService.GetData<bool>(client, "Ray:IsOpen"))
            {
                _clientService.OnShareData(client, "Ray:IsOpen", false);
                _clientService.OnShareData(client, "Ray:CurrentGroup", _mainGroup);

                ClearHistory(client);
                AddHistory(client, _mainGroup);

                var raycast = _clientService.GetData<RaycastHit>(client, "Character:CurrentRaycast") ?? new RaycastHit();

                if (raycast.Hit && raycast.EntityType != (int)EntityType.Map)
                {
                    SetVisibility(client, false);
                    SetCrossairVisibility(client, true, CrossairTransitionDuration);
                }
                else
                {
                    CloseMenu(client);
                    SetCrossairVisibility(client, false, CrossairTransitionDuration);
                }
            }
        }

        private void OnRender(Client client, RayGroup group) => _uiService.SendMessage(client, "ray", "render", new
        {
            items = group.Items
        });

        internal void ShowMenu(Client client)
        {
            _uiService.SendMessage(client, "ray", "open");
            _clientService.OnShareData(client, "Ray:IsOpen", true);
        }

        internal void CloseMenu(Client client)
        {
            _uiService.SendMessage(client, "ray", "close");
            _clientService.OnShareData(client, "Ray:IsOpen", false);
        }

        internal void SetCrossairVisibility(Client client, bool isVisible, int fadeDuration = 100) => _uiService.SendMessage(client, "ray", "crossair", new
        {
            isVisible,
            fade = fadeDuration
        });

        internal void AddHistory(Client client, RayGroup group)
        {
            var histories = _clientService.GetData<List<RayGroup>>(client, "Ray:Histories");

            if (!histories.Exists(x => x.Name == group.Name))
            {
                histories.Add(group);
            }
        }

        internal void RemoveHistory(Client client, RayGroup group)
        {
            var histories = _clientService.GetData<List<RayGroup>>(client, "Ray:Histories");

            if (histories.Exists(x => x.Name == group.Name))
            {
                histories.Remove(group);
            }
        }

        internal void ClearHistory(Client client)
        {
            var histories = _clientService.GetData<List<RayGroup>>(client, "Ray:Histories");
            histories.Clear();
        }
    }
}
