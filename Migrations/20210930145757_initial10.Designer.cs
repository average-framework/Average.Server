﻿// <auto-generated />
using System;
using Average.Server.Framework.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Average.Server.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20210930145757_initial10")]
    partial class initial10
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Average.Shared.DataModels.CharacterData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CityOfBirth")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("DateOfBirth")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("LastUsing")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("License")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Average.Shared.DataModels.CoreData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<int>("Hunger")
                        .HasColumnType("int");

                    b.Property<int>("Stamina")
                        .HasColumnType("int");

                    b.Property<int>("Thirst")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("CoreData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.EconomyData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<decimal>("Bank")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("BankCent")
                        .HasColumnType("decimal(65,30)");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Money")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("MoneyCent")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("EconomyData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.JobData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("JobData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.OutfitData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<uint>("Accessory")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Apron")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Armor")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Badge")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("BeardChop")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Belt")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Beltbuckle")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Boot")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Bracelt")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Chap")
                        .HasColumnType("int unsigned");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<uint>("Cloak")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Coat")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("CoatClosed")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Eye")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Eyewear")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("FemaleUnknow01")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Gauntlet")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Glove")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Goatee")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Gunbelt")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Hair")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Hat")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Head")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("HolsterCrossdraw")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("HolsterLeft")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("HolsterRight")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Leg")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("LegAttachement")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Loadout")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Mask")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("MaskLarge")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Mustache")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("MustacheMP")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Necktie")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Neckwear")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Pant")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Poncho")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("RingLeftHand")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("RingRightHand")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Satchel")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Sheath")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Shirt")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Skirt")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Spat")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Spur")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Suspender")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("TalismanBelt")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("TalismanHolster")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("TalismanSatchel")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("TalismanWrist")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Teeth")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Torso")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Vest")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("OutfitData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.OverlayData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<float>("Opacity")
                        .HasColumnType("float");

                    b.Property<uint>("Palette")
                        .HasColumnType("int unsigned");

                    b.Property<int>("PalettePrimaryColor")
                        .HasColumnType("int");

                    b.Property<int>("PaletteSecondaryColor")
                        .HasColumnType("int");

                    b.Property<int>("PaletteTertiaryColor")
                        .HasColumnType("int");

                    b.Property<long>("SkinId")
                        .HasColumnType("bigint");

                    b.Property<int>("TextureColorType")
                        .HasColumnType("int");

                    b.Property<uint>("TextureId")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("TextureMaterial")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("TextureNormal")
                        .HasColumnType("int unsigned");

                    b.Property<float>("TextureOpacity")
                        .HasColumnType("float");

                    b.Property<int>("TextureUnk")
                        .HasColumnType("int");

                    b.Property<bool>("TextureVisibility")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Variante")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SkinId");

                    b.ToTable("OverlayData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.PositionData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<float>("H")
                        .HasColumnType("float");

                    b.Property<float>("X")
                        .HasColumnType("float");

                    b.Property<float>("Y")
                        .HasColumnType("float");

                    b.Property<float>("Z")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("PositionData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.SkinData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("Albedo")
                        .HasColumnType("int");

                    b.Property<uint>("Body")
                        .HasColumnType("int unsigned");

                    b.Property<int>("BodyType")
                        .HasColumnType("int");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<float>("CheeckBonesDepth")
                        .HasColumnType("float");

                    b.Property<float>("CheeckBonesHeight")
                        .HasColumnType("float");

                    b.Property<float>("CheeckBonesWidth")
                        .HasColumnType("float");

                    b.Property<float>("ChinDepth")
                        .HasColumnType("float");

                    b.Property<float>("ChinHeight")
                        .HasColumnType("float");

                    b.Property<float>("ChinWidth")
                        .HasColumnType("float");

                    b.Property<float>("EarsAngle")
                        .HasColumnType("float");

                    b.Property<float>("EarsHeight")
                        .HasColumnType("float");

                    b.Property<float>("EarsLobeSize")
                        .HasColumnType("float");

                    b.Property<float>("EarsWidth")
                        .HasColumnType("float");

                    b.Property<float>("EyeLidHeight")
                        .HasColumnType("float");

                    b.Property<float>("EyeLidWidth")
                        .HasColumnType("float");

                    b.Property<float>("EyebrowDepth")
                        .HasColumnType("float");

                    b.Property<float>("EyebrowHeight")
                        .HasColumnType("float");

                    b.Property<float>("EyebrowWidth")
                        .HasColumnType("float");

                    b.Property<float>("EyesAngle")
                        .HasColumnType("float");

                    b.Property<float>("EyesDepth")
                        .HasColumnType("float");

                    b.Property<float>("EyesDistance")
                        .HasColumnType("float");

                    b.Property<float>("EyesHeight")
                        .HasColumnType("float");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<uint>("Head")
                        .HasColumnType("int unsigned");

                    b.Property<float>("HeadWidth")
                        .HasColumnType("float");

                    b.Property<float>("JawDepth")
                        .HasColumnType("float");

                    b.Property<float>("JawHeight")
                        .HasColumnType("float");

                    b.Property<float>("JawWidth")
                        .HasColumnType("float");

                    b.Property<uint>("Legs")
                        .HasColumnType("int unsigned");

                    b.Property<float>("LowerLipDepth")
                        .HasColumnType("float");

                    b.Property<float>("LowerLipHeight")
                        .HasColumnType("float");

                    b.Property<float>("LowerLipWidth")
                        .HasColumnType("float");

                    b.Property<int>("Material")
                        .HasColumnType("int");

                    b.Property<float>("MouthDepth")
                        .HasColumnType("float");

                    b.Property<float>("MouthWidth")
                        .HasColumnType("float");

                    b.Property<float>("MouthXPos")
                        .HasColumnType("float");

                    b.Property<float>("MouthYPos")
                        .HasColumnType("float");

                    b.Property<float>("NoStrilsDistance")
                        .HasColumnType("float");

                    b.Property<int>("Normal")
                        .HasColumnType("int");

                    b.Property<float>("NoseAngle")
                        .HasColumnType("float");

                    b.Property<float>("NoseCurvature")
                        .HasColumnType("float");

                    b.Property<float>("NoseHeight")
                        .HasColumnType("float");

                    b.Property<float>("NoseSize")
                        .HasColumnType("float");

                    b.Property<float>("NoseWidth")
                        .HasColumnType("float");

                    b.Property<float>("Scale")
                        .HasColumnType("float");

                    b.Property<float>("UpperLipDepth")
                        .HasColumnType("float");

                    b.Property<float>("UpperLipHeight")
                        .HasColumnType("float");

                    b.Property<float>("UpperLipWidth")
                        .HasColumnType("float");

                    b.Property<int>("WaistType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("SkinData");
                });

            modelBuilder.Entity("Average.Shared.DataModels.UserData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsWhitelisted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastConnection")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("License")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("PermissionLevel")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Average.Shared.DataModels.WorldData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time(6)");

                    b.Property<uint>("Weather")
                        .HasColumnType("int unsigned");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Worlds");
                });

            modelBuilder.Entity("Average.Shared.DataModels.CharacterData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.UserData", "User")
                        .WithMany("Characters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.CoreData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.CharacterData", "Character")
                        .WithOne("Core")
                        .HasForeignKey("Average.Shared.DataModels.CoreData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.EconomyData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.CharacterData", "Character")
                        .WithOne("Economy")
                        .HasForeignKey("Average.Shared.DataModels.EconomyData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.JobData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.CharacterData", "Character")
                        .WithOne("Job")
                        .HasForeignKey("Average.Shared.DataModels.JobData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.OutfitData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.CharacterData", "Character")
                        .WithOne("Outfit")
                        .HasForeignKey("Average.Shared.DataModels.OutfitData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.OverlayData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.SkinData", "Skin")
                        .WithMany("OverlaysData")
                        .HasForeignKey("SkinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.PositionData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.CharacterData", "Character")
                        .WithOne("Position")
                        .HasForeignKey("Average.Shared.DataModels.PositionData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Shared.DataModels.SkinData", b =>
                {
                    b.HasOne("Average.Shared.DataModels.CharacterData", "Character")
                        .WithOne("Skin")
                        .HasForeignKey("Average.Shared.DataModels.SkinData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
