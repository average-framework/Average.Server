using Average.Server.Framework.Extensions;
using Average.Shared.DataModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Average.Server.Framework.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionstring) => _connectionString = connectionstring;

        public DbSet<UserData> Users { get; set; }
        public DbSet<CharacterData> Characters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options
            //.UseSqlServer(_connectionString); // no mssql anymore
            .EnableSensitiveDataLogging()
            .UseMySql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User

            //modelBuilder.Entity<UserData>()
            //    .HasIndex(x => x.Id)
            //    .IsUnique();

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
               .HasOne(x => x.Skin)
               .WithOne(x => x.Character)
               .HasForeignKey<SkinData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CharacterData>()
               .HasOne(x => x.Outfit)
               .WithOne(x => x.Character)
               .HasForeignKey<OutfitData>(x => x.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<CharacterData>()
            //   .Property(x => x.Data)
            //   .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<KeyValueData>(x));

            modelBuilder.Entity<CharacterData>()
               .Property(x => x.Data)
               .HasConversion(x => x.ToJson(Formatting.None), x => x.Convert<Dictionary<string, object>>());

            // Skin

            modelBuilder.Entity<SkinData>()
               .HasMany(x => x.OverlaysData)
               .WithOne(x => x.Skin)
               .HasForeignKey(x => x.SkinId)
               .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}