using CitizenFX.Core;

namespace Average.Server.Framework.Extensions
{
    public static class PlayerExtensions
    {
        public static string Steam(this Player player) => player.Identifiers["steam"];
        public static string License(this Player player) => player.Identifiers["license"];
        public static string DiscordId(this Player player) => player.Identifiers["discord"];
    }
}