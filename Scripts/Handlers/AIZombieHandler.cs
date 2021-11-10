using Average.Server.Framework.Attributes;
using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;

namespace Average.Server.Scripts.Handlers
{
    internal class AIZombieHandler : IHandler
    {
        private readonly AIZombieScript _aiZombieScript;

        public AIZombieHandler(AIZombieScript aiZombieScript)
        {
            _aiZombieScript = aiZombieScript;
        }

        [ServerEvent("aizombie:sync")]
        private void OnSync(Client client, int netId, bool isRusher)
        {
            _aiZombieScript.SyncEntityBetweenPlayers(client, netId, isRusher);
        }
    }
}
