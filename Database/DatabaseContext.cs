using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SDK.Shared.DataModels;

namespace Average.Server.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionstring) => _connectionString = connectionstring;

        public DbSet<UserData> Users { get; set; }
        public DbSet<CharacterData> Characters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options
            .EnableSensitiveDataLogging()
            .UseMySql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User

            modelBuilder.Entity<UserData>()
               .HasMany(x => x.Characters)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            // Character

            modelBuilder.Entity<CharacterData>()
               .HasOne(x => x.Economy)
               .WithOne(x => x.Character)
               .HasForeignKey<EconomyData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
               .HasOne(x => x.Position)
               .WithOne(x => x.Character)
               .HasForeignKey<PositionData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
               .HasOne(x => x.Core)
               .WithOne(x => x.Character)
               .HasForeignKey<CoreData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
               .HasOne(x => x.Job)
               .WithOne(x => x.Character)
               .HasForeignKey<JobData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
               .HasOne(x => x.Face)
               .WithOne(x => x.Character)
               .HasForeignKey<FaceData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
              .HasOne(x => x.Clothes)
              .WithOne(x => x.Character)
              .HasForeignKey<ClothesData>(x => x.CharacterId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
              .HasOne(x => x.Texture)
              .WithOne(x => x.Character)
              .HasForeignKey<TextureData>(x => x.CharacterId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
              .HasOne(x => x.FaceOverlays)
              .WithOne(x => x.Character)
              .HasForeignKey<FaceOverlayData>(x => x.CharacterId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
              .Property(x => x.Data)
              .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<KeyValueData>(x));

            base.OnModelCreating(modelBuilder);
        }
    }
}