using Average.Server.Framework.Areas;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Extensions;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Managers;
using Average.Server.Framework.Model;
using System.Collections.Generic;

namespace Average.Server.Services
{
    internal class AreaService : IService
    {
        private readonly EventManager _eventManager;

        public readonly List<BlipArea> _blipAreas = new List<BlipArea>();
        public readonly List<InteractionArea> _interactionAreas = new List<InteractionArea>();
        public readonly List<NpcArea> _npcAreas = new List<NpcArea>();

        public AreaService(EventManager eventManager)
        {
            _eventManager = eventManager;

            Logger.Write("AreaService", "Initialized successfully");
        }

        public AreaService AddBlip(BlipArea blip)
        {
            _blipAreas.Add(blip);
            return this;
        }

        public AreaService AddInteractionArea(InteractionArea interaction)
        {
            _interactionAreas.Add(interaction);
            return this;
        }

        public AreaService AddNpcArea(NpcArea npc)
        {
            _npcAreas.Add(npc);
            return this;
        }

        public void EmitClient(Client client)
        {
            var blips = _blipAreas.ToJson();
            var interactions = _interactionAreas.ToJson();
            var npcs = _npcAreas.ToJson();
            _eventManager.EmitClient(client.Player, "area:create_area", blips, interactions, npcs);
        }
    }
}
