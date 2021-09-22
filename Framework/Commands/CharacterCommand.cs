using Average.Server.Framework.Attributes;
using Average.Server.Framework.Diagnostics;
using Average.Server.Framework.Model;

namespace Average.Server.Framework.Commands
{
    internal interface ICommand
    {

    }

    internal class CharacterCommand : ICommand
    {
        public CharacterCommand()
        {

        }

        //[ClientCommand("character:set_money")]
        //private void OnCharacterSetMoney(Client client, string amount)
        //{
        //    Logger.Debug("Set money for: " + client.Name + ", " + amount);
        //}

        //[ClientCommand("character:set_bank")]
        //private void OnCharacterSetBank(Client client, string amount, string amount1, int amount2)
        //{
        //    Logger.Debug("Set money for: " + client.Name + ", " + amount);
        //}
    }
}
