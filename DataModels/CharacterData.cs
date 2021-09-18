using Average.Server.Framework.Interfaces;
using Average.Server.Framework.Model;
using Average.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Average.Server.DataModels
{
    public class CharacterData : EntityBase, IDbEntity
    {
        public virtual UserData User { get; set; }
        public long UserId { get; set; }

        [NotMapped]
        public int PlayerId { get; set; }

        public string License { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Nationality { get; set; }
        public string CityOfBirth { get; set; }
        public string DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public int Culture { get; set; }
        public int Head { get; set; }
        public int Body { get; set; }
        public int Legs { get; set; }
        public int BodyType { get; set; }
        public int WaistType { get; set; }

        public float Scale { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUsing { get; set; }

        public EconomyData Economy { get; set; } = new EconomyData();
        public PositionData Position { get; set; } = new PositionData();
        public CoreData Core { get; set; } = new CoreData();
        public JobData Job { get; set; } = new JobData();

        public FaceData Face { get; set; }
        public ClothesData Clothes { get; set; }
        public TextureData Texture { get; set; }
        public FaceOverlayData FaceOverlays { get; set; }
        public KeyValueData Data { get; set; }
    }
}
