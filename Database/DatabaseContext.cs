using Microsoft.EntityFrameworkCore;

namespace Average.Server.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionstring) => _connectionString = connectionstring;

        public DatabaseContext()
        {

        }

        //public DbSet<User> Accounts { get; set; }
        //public DbSet<Character> Characters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options
            //.UseSqlServer(_connectionString); // no mssql anymore
            .EnableSensitiveDataLogging()
            .UseMySql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ////User

            //modelBuilder.Entity<User>()
            // .HasMany(a => a.Characters)
            // .WithOne(b => b.User)
            // .HasForeignKey(b => b.UserId)
            // .OnDelete(DeleteBehavior.Cascade);

            //// Character
            //modelBuilder.Entity<Character>()
            //    .HasOne(a => a.BodyAppearance)
            //    .WithOne(b => b.Character)
            //    .HasForeignKey<CharacterBodyAppearance>(b => b.CharacterId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Character>()
            //    .HasOne(a => a.Skills)
            //    .WithOne(b => b.Character)
            //    .HasForeignKey<CharacterSkill>(b => b.CharacterId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Character>()
            //    .HasOne(a => a.Appearance)
            //    .WithOne(b => b.Character)
            //    .HasForeignKey<CharacterAppearance>(b => b.CharacterId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Character>()
            //    .HasOne(a => a.CharacterLicenses)
            //    .WithOne(b => b.Character)
            //    .HasForeignKey<CharacterLicenses>(c => c.CharacterId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Character>()
            //.HasOne(a => a.Inventory)
            //.WithOne(b => b.Character)
            //.HasForeignKey<Inventory>(c => c.CharacterId)
            //.OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}