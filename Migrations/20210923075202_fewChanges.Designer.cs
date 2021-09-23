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
    [Migration("20210923075202_fewChanges")]
    partial class fewChanges
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Average.Server.DataModels.CharacterData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<uint>("Body")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("BodyType")
                        .HasColumnType("int unsigned");

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

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<uint>("Head")
                        .HasColumnType("int unsigned");

                    b.Property<DateTime>("LastUsing")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<uint>("Legs")
                        .HasColumnType("int unsigned");

                    b.Property<string>("License")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<uint>("Origin")
                        .HasColumnType("int unsigned");

                    b.Property<float>("Scale")
                        .HasColumnType("float");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<uint>("WaistType")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Average.Server.DataModels.ClothesData", b =>
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

                    b.ToTable("ClothesData");
                });

            modelBuilder.Entity("Average.Server.DataModels.CoreData", b =>
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

            modelBuilder.Entity("Average.Server.DataModels.EconomyData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<decimal>("Bank")
                        .HasColumnType("decimal(65,30)");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Money")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("EconomyData");
                });

            modelBuilder.Entity("Average.Server.DataModels.FaceData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<int>("CheeckBonesDepth")
                        .HasColumnType("int");

                    b.Property<int>("CheeckBonesHeight")
                        .HasColumnType("int");

                    b.Property<int>("CheeckBonesWidth")
                        .HasColumnType("int");

                    b.Property<int>("ChinDepth")
                        .HasColumnType("int");

                    b.Property<int>("ChinHeight")
                        .HasColumnType("int");

                    b.Property<int>("ChinWidth")
                        .HasColumnType("int");

                    b.Property<int>("EarsAngle")
                        .HasColumnType("int");

                    b.Property<int>("EarsHeight")
                        .HasColumnType("int");

                    b.Property<int>("EarsLobeSize")
                        .HasColumnType("int");

                    b.Property<int>("EarsWidth")
                        .HasColumnType("int");

                    b.Property<int>("EyeLidHeight")
                        .HasColumnType("int");

                    b.Property<int>("EyeLidWidth")
                        .HasColumnType("int");

                    b.Property<int>("EyebrowDepth")
                        .HasColumnType("int");

                    b.Property<int>("EyebrowHeight")
                        .HasColumnType("int");

                    b.Property<int>("EyebrowWidth")
                        .HasColumnType("int");

                    b.Property<int>("EyesAngle")
                        .HasColumnType("int");

                    b.Property<int>("EyesDepth")
                        .HasColumnType("int");

                    b.Property<int>("EyesDistance")
                        .HasColumnType("int");

                    b.Property<int>("EyesHeight")
                        .HasColumnType("int");

                    b.Property<int>("HeadWidth")
                        .HasColumnType("int");

                    b.Property<int>("JawDepth")
                        .HasColumnType("int");

                    b.Property<int>("JawHeight")
                        .HasColumnType("int");

                    b.Property<int>("JawWidth")
                        .HasColumnType("int");

                    b.Property<int>("LowerLipDepth")
                        .HasColumnType("int");

                    b.Property<int>("LowerLipHeight")
                        .HasColumnType("int");

                    b.Property<int>("LowerLipWidth")
                        .HasColumnType("int");

                    b.Property<int>("MouthDepth")
                        .HasColumnType("int");

                    b.Property<int>("MouthWidth")
                        .HasColumnType("int");

                    b.Property<int>("MouthXPos")
                        .HasColumnType("int");

                    b.Property<int>("MouthYPos")
                        .HasColumnType("int");

                    b.Property<int>("NoStrilsDistance")
                        .HasColumnType("int");

                    b.Property<int>("NoseAngle")
                        .HasColumnType("int");

                    b.Property<int>("NoseCurvature")
                        .HasColumnType("int");

                    b.Property<int>("NoseHeight")
                        .HasColumnType("int");

                    b.Property<int>("NoseSize")
                        .HasColumnType("int");

                    b.Property<int>("NoseWidth")
                        .HasColumnType("int");

                    b.Property<int>("UpperLipDepth")
                        .HasColumnType("int");

                    b.Property<int>("UpperLipHeight")
                        .HasColumnType("int");

                    b.Property<int>("UpperLipWidth")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("FaceData");
                });

            modelBuilder.Entity("Average.Server.DataModels.FaceOverlayData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("AcneId")
                        .HasColumnType("bigint");

                    b.Property<long>("AgeingId")
                        .HasColumnType("bigint");

                    b.Property<long>("BeardstabbleId")
                        .HasColumnType("bigint");

                    b.Property<long>("BlushId")
                        .HasColumnType("bigint");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<long>("ComplexId")
                        .HasColumnType("bigint");

                    b.Property<long>("DiscId")
                        .HasColumnType("bigint");

                    b.Property<long>("EyebrowsId")
                        .HasColumnType("bigint");

                    b.Property<long>("EyelinersId")
                        .HasColumnType("bigint");

                    b.Property<long>("FoundationId")
                        .HasColumnType("bigint");

                    b.Property<long>("FrecklesId")
                        .HasColumnType("bigint");

                    b.Property<long>("GrimeId")
                        .HasColumnType("bigint");

                    b.Property<long>("HairId")
                        .HasColumnType("bigint");

                    b.Property<long>("LipsticksId")
                        .HasColumnType("bigint");

                    b.Property<long>("MolesId")
                        .HasColumnType("bigint");

                    b.Property<long>("PaintedmasksId")
                        .HasColumnType("bigint");

                    b.Property<long>("ScarsId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShadowsId")
                        .HasColumnType("bigint");

                    b.Property<long>("SpotsId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AcneId");

                    b.HasIndex("AgeingId");

                    b.HasIndex("BeardstabbleId");

                    b.HasIndex("BlushId");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.HasIndex("ComplexId");

                    b.HasIndex("DiscId");

                    b.HasIndex("EyebrowsId");

                    b.HasIndex("EyelinersId");

                    b.HasIndex("FoundationId");

                    b.HasIndex("FrecklesId");

                    b.HasIndex("GrimeId");

                    b.HasIndex("HairId");

                    b.HasIndex("LipsticksId");

                    b.HasIndex("MolesId");

                    b.HasIndex("PaintedmasksId");

                    b.HasIndex("ScarsId");

                    b.HasIndex("ShadowsId");

                    b.HasIndex("SpotsId");

                    b.ToTable("FaceOverlayData");
                });

            modelBuilder.Entity("Average.Server.DataModels.JobData", b =>
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

            modelBuilder.Entity("Average.Server.DataModels.OverlayData", b =>
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

                    b.ToTable("OverlayData");
                });

            modelBuilder.Entity("Average.Server.DataModels.PositionData", b =>
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

            modelBuilder.Entity("Average.Server.DataModels.TextureData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<uint>("Albedo")
                        .HasColumnType("int unsigned");

                    b.Property<long>("CharacterId")
                        .HasColumnType("bigint");

                    b.Property<uint>("Material")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Normal")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("TextureData");
                });

            modelBuilder.Entity("Average.Server.DataModels.UserData", b =>
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

            modelBuilder.Entity("Average.Server.DataModels.CharacterData", b =>
                {
                    b.HasOne("Average.Server.DataModels.UserData", "User")
                        .WithMany("Characters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.ClothesData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Clothes")
                        .HasForeignKey("Average.Server.DataModels.ClothesData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.CoreData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Core")
                        .HasForeignKey("Average.Server.DataModels.CoreData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.EconomyData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Economy")
                        .HasForeignKey("Average.Server.DataModels.EconomyData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.FaceData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Face")
                        .HasForeignKey("Average.Server.DataModels.FaceData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.FaceOverlayData", b =>
                {
                    b.HasOne("Average.Server.DataModels.OverlayData", "Acne")
                        .WithMany()
                        .HasForeignKey("AcneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Ageing")
                        .WithMany()
                        .HasForeignKey("AgeingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Beardstabble")
                        .WithMany()
                        .HasForeignKey("BeardstabbleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Blush")
                        .WithMany()
                        .HasForeignKey("BlushId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("FaceOverlays")
                        .HasForeignKey("Average.Server.DataModels.FaceOverlayData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Complex")
                        .WithMany()
                        .HasForeignKey("ComplexId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Disc")
                        .WithMany()
                        .HasForeignKey("DiscId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Eyebrows")
                        .WithMany()
                        .HasForeignKey("EyebrowsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Eyeliners")
                        .WithMany()
                        .HasForeignKey("EyelinersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Foundation")
                        .WithMany()
                        .HasForeignKey("FoundationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Freckles")
                        .WithMany()
                        .HasForeignKey("FrecklesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Grime")
                        .WithMany()
                        .HasForeignKey("GrimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Hair")
                        .WithMany()
                        .HasForeignKey("HairId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Lipsticks")
                        .WithMany()
                        .HasForeignKey("LipsticksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Moles")
                        .WithMany()
                        .HasForeignKey("MolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Paintedmasks")
                        .WithMany()
                        .HasForeignKey("PaintedmasksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Scars")
                        .WithMany()
                        .HasForeignKey("ScarsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Shadows")
                        .WithMany()
                        .HasForeignKey("ShadowsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Average.Server.DataModels.OverlayData", "Spots")
                        .WithMany()
                        .HasForeignKey("SpotsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.JobData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Job")
                        .HasForeignKey("Average.Server.DataModels.JobData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.PositionData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Position")
                        .HasForeignKey("Average.Server.DataModels.PositionData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Average.Server.DataModels.TextureData", b =>
                {
                    b.HasOne("Average.Server.DataModels.CharacterData", "Character")
                        .WithOne("Texture")
                        .HasForeignKey("Average.Server.DataModels.TextureData", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
