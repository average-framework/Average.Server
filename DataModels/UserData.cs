using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Average.Server.DataModels
{
    public class UserData : EntityBase, IDbEntity
    {
        public ICollection<CharacterData> Characters { get; set; } = new List<CharacterData>();

        /// <summary>
        /// Rockstar license of the player
        /// </summary>
        public string License { get; set; }
        /// <summary>
        /// Creation date time of user account
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Last connection date time of user
        /// </summary>
        public DateTime LastConnection { get; set; }
        /// <summary>
        /// Represent the user account permission level
        /// </summary>
        public int PermissionLevel { get; set; }
        /// <summary>
        /// Ban state of the user
        /// </summary>
        public int IsBanned { get; set; }
        /// <summary>
        /// Whitelist state of the user
        /// </summary>
        public int IsWhitelisted { get; set; }
        /// <summary>
        /// Dertermine if the player is connected
        /// </summary>
        public int IsConnected { get; set; }

        [NotMapped]
        public int CharacterCount => Characters.Count;
    }
}
