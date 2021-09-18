using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Average.Server.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    License = table.Column<string>(nullable: false),
                    Firstname = table.Column<string>(nullable: false),
                    Lastname = table.Column<string>(nullable: false),
                    Nationality = table.Column<string>(nullable: false),
                    CityOfBirth = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<string>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Culture = table.Column<int>(nullable: false),
                    Head = table.Column<int>(nullable: false),
                    Body = table.Column<int>(nullable: false),
                    Legs = table.Column<int>(nullable: false),
                    BodyType = table.Column<int>(nullable: false),
                    WaistType = table.Column<int>(nullable: false),
                    Scale = table.Column<float>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    LastUsing = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OverlayData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    TextureVisibility = table.Column<bool>(nullable: false),
                    TextureColorType = table.Column<int>(nullable: false),
                    TextureOpacity = table.Column<float>(nullable: false),
                    TextureUnk = table.Column<int>(nullable: false),
                    PalettePrimaryColor = table.Column<int>(nullable: false),
                    PaletteSecondaryColor = table.Column<int>(nullable: false),
                    PaletteTertiaryColor = table.Column<int>(nullable: false),
                    Variante = table.Column<int>(nullable: false),
                    Opacity = table.Column<float>(nullable: false),
                    TextureId = table.Column<uint>(nullable: false),
                    TextureNormal = table.Column<uint>(nullable: false),
                    TextureMaterial = table.Column<uint>(nullable: false),
                    Palette = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverlayData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Licence = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastConnection = table.Column<DateTime>(nullable: false),
                    PermissionLevel = table.Column<int>(nullable: false),
                    IsBanned = table.Column<int>(nullable: false),
                    IsWhitelisted = table.Column<int>(nullable: false),
                    IsConnected = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClothesData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Accessory = table.Column<uint>(nullable: false),
                    Apron = table.Column<uint>(nullable: false),
                    Armor = table.Column<uint>(nullable: false),
                    Badge = table.Column<uint>(nullable: false),
                    Beltbuckle = table.Column<uint>(nullable: false),
                    Belt = table.Column<uint>(nullable: false),
                    Boot = table.Column<uint>(nullable: false),
                    Bracelt = table.Column<uint>(nullable: false),
                    Chap = table.Column<uint>(nullable: false),
                    Cloak = table.Column<uint>(nullable: false),
                    Coat = table.Column<uint>(nullable: false),
                    CoatClosed = table.Column<uint>(nullable: false),
                    Eye = table.Column<uint>(nullable: false),
                    Eyewear = table.Column<uint>(nullable: false),
                    Gauntlet = table.Column<uint>(nullable: false),
                    Glove = table.Column<uint>(nullable: false),
                    Goatee = table.Column<uint>(nullable: false),
                    Gunbelt = table.Column<uint>(nullable: false),
                    Hair = table.Column<uint>(nullable: false),
                    Hat = table.Column<uint>(nullable: false),
                    Head = table.Column<uint>(nullable: false),
                    Leg = table.Column<uint>(nullable: false),
                    Loadout = table.Column<uint>(nullable: false),
                    Mask = table.Column<uint>(nullable: false),
                    Mustache = table.Column<uint>(nullable: false),
                    MustacheMP = table.Column<uint>(nullable: false),
                    Necktie = table.Column<uint>(nullable: false),
                    Neckwear = table.Column<uint>(nullable: false),
                    Pant = table.Column<uint>(nullable: false),
                    Poncho = table.Column<uint>(nullable: false),
                    Satchel = table.Column<uint>(nullable: false),
                    Sheath = table.Column<uint>(nullable: false),
                    Shirt = table.Column<uint>(nullable: false),
                    Skirt = table.Column<uint>(nullable: false),
                    Spat = table.Column<uint>(nullable: false),
                    Spur = table.Column<uint>(nullable: false),
                    Suspender = table.Column<uint>(nullable: false),
                    Teeth = table.Column<uint>(nullable: false),
                    Torso = table.Column<uint>(nullable: false),
                    Vest = table.Column<uint>(nullable: false),
                    BeardChop = table.Column<uint>(nullable: false),
                    FemaleUnknow01 = table.Column<uint>(nullable: false),
                    HolsterCrossdraw = table.Column<uint>(nullable: false),
                    HolsterLeft = table.Column<uint>(nullable: false),
                    HolsterRight = table.Column<uint>(nullable: false),
                    LegAttachement = table.Column<uint>(nullable: false),
                    MaskLarge = table.Column<uint>(nullable: false),
                    TalismanBelt = table.Column<uint>(nullable: false),
                    TalismanHolster = table.Column<uint>(nullable: false),
                    TalismanSatchel = table.Column<uint>(nullable: false),
                    TalismanWrist = table.Column<uint>(nullable: false),
                    RingLeftHand = table.Column<uint>(nullable: false),
                    RingRightHand = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothesData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClothesData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoreData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Health = table.Column<int>(nullable: false),
                    Hunger = table.Column<int>(nullable: false),
                    Thirst = table.Column<int>(nullable: false),
                    Stamina = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EconomyData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Money = table.Column<decimal>(nullable: false),
                    Bank = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EconomyData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomyData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaceData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    HeadWidth = table.Column<int>(nullable: false),
                    EyebrowHeight = table.Column<int>(nullable: false),
                    EyebrowWidth = table.Column<int>(nullable: false),
                    EyebrowDepth = table.Column<int>(nullable: false),
                    EarsWidth = table.Column<int>(nullable: false),
                    EarsAngle = table.Column<int>(nullable: false),
                    EarsHeight = table.Column<int>(nullable: false),
                    EarsLobeSize = table.Column<int>(nullable: false),
                    CheeckBonesHeight = table.Column<int>(nullable: false),
                    CheeckBonesWidth = table.Column<int>(nullable: false),
                    CheeckBonesDepth = table.Column<int>(nullable: false),
                    JawHeight = table.Column<int>(nullable: false),
                    JawWidth = table.Column<int>(nullable: false),
                    JawDepth = table.Column<int>(nullable: false),
                    ChinHeight = table.Column<int>(nullable: false),
                    ChinWidth = table.Column<int>(nullable: false),
                    ChinDepth = table.Column<int>(nullable: false),
                    EyeLidHeight = table.Column<int>(nullable: false),
                    EyeLidWidth = table.Column<int>(nullable: false),
                    EyesDepth = table.Column<int>(nullable: false),
                    EyesAngle = table.Column<int>(nullable: false),
                    EyesDistance = table.Column<int>(nullable: false),
                    EyesHeight = table.Column<int>(nullable: false),
                    NoseWidth = table.Column<int>(nullable: false),
                    NoseSize = table.Column<int>(nullable: false),
                    NoseHeight = table.Column<int>(nullable: false),
                    NoseAngle = table.Column<int>(nullable: false),
                    NoseCurvature = table.Column<int>(nullable: false),
                    NoStrilsDistance = table.Column<int>(nullable: false),
                    MouthWidth = table.Column<int>(nullable: false),
                    MouthDepth = table.Column<int>(nullable: false),
                    MouthXPos = table.Column<int>(nullable: false),
                    MouthYPos = table.Column<int>(nullable: false),
                    UpperLipHeight = table.Column<int>(nullable: false),
                    UpperLipWidth = table.Column<int>(nullable: false),
                    UpperLipDepth = table.Column<int>(nullable: false),
                    LowerLipHeight = table.Column<int>(nullable: false),
                    LowerLipWidth = table.Column<int>(nullable: false),
                    LowerLipDepth = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    H = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextureData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Albedo = table.Column<uint>(nullable: false),
                    Normal = table.Column<uint>(nullable: false),
                    Material = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextureData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextureData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaceOverlayData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    EyebrowsId = table.Column<long>(nullable: false),
                    ScarsId = table.Column<long>(nullable: false),
                    EyelinersId = table.Column<long>(nullable: false),
                    LipsticksId = table.Column<long>(nullable: false),
                    AcneId = table.Column<long>(nullable: false),
                    ShadowsId = table.Column<long>(nullable: false),
                    BeardstabbleId = table.Column<long>(nullable: false),
                    PaintedmasksId = table.Column<long>(nullable: false),
                    AgeingId = table.Column<long>(nullable: false),
                    BlushId = table.Column<long>(nullable: false),
                    ComplexId = table.Column<long>(nullable: false),
                    DiscId = table.Column<long>(nullable: false),
                    FoundationId = table.Column<long>(nullable: false),
                    FrecklesId = table.Column<long>(nullable: false),
                    GrimeId = table.Column<long>(nullable: false),
                    HairId = table.Column<long>(nullable: false),
                    MolesId = table.Column<long>(nullable: false),
                    SpotsId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceOverlayData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_AcneId",
                        column: x => x.AcneId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_AgeingId",
                        column: x => x.AgeingId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_BeardstabbleId",
                        column: x => x.BeardstabbleId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_BlushId",
                        column: x => x.BlushId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_ComplexId",
                        column: x => x.ComplexId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_DiscId",
                        column: x => x.DiscId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_EyebrowsId",
                        column: x => x.EyebrowsId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_EyelinersId",
                        column: x => x.EyelinersId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_FoundationId",
                        column: x => x.FoundationId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_FrecklesId",
                        column: x => x.FrecklesId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_GrimeId",
                        column: x => x.GrimeId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_HairId",
                        column: x => x.HairId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_LipsticksId",
                        column: x => x.LipsticksId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_MolesId",
                        column: x => x.MolesId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_PaintedmasksId",
                        column: x => x.PaintedmasksId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_ScarsId",
                        column: x => x.ScarsId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_ShadowsId",
                        column: x => x.ShadowsId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceOverlayData_OverlayData_SpotsId",
                        column: x => x.SpotsId,
                        principalTable: "OverlayData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClothesData_CharacterId",
                table: "ClothesData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoreData_CharacterId",
                table: "CoreData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EconomyData_CharacterId",
                table: "EconomyData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaceData_CharacterId",
                table: "FaceData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_AcneId",
                table: "FaceOverlayData",
                column: "AcneId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_AgeingId",
                table: "FaceOverlayData",
                column: "AgeingId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_BeardstabbleId",
                table: "FaceOverlayData",
                column: "BeardstabbleId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_BlushId",
                table: "FaceOverlayData",
                column: "BlushId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_CharacterId",
                table: "FaceOverlayData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_ComplexId",
                table: "FaceOverlayData",
                column: "ComplexId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_DiscId",
                table: "FaceOverlayData",
                column: "DiscId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_EyebrowsId",
                table: "FaceOverlayData",
                column: "EyebrowsId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_EyelinersId",
                table: "FaceOverlayData",
                column: "EyelinersId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_FoundationId",
                table: "FaceOverlayData",
                column: "FoundationId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_FrecklesId",
                table: "FaceOverlayData",
                column: "FrecklesId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_GrimeId",
                table: "FaceOverlayData",
                column: "GrimeId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_HairId",
                table: "FaceOverlayData",
                column: "HairId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_LipsticksId",
                table: "FaceOverlayData",
                column: "LipsticksId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_MolesId",
                table: "FaceOverlayData",
                column: "MolesId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_PaintedmasksId",
                table: "FaceOverlayData",
                column: "PaintedmasksId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_ScarsId",
                table: "FaceOverlayData",
                column: "ScarsId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_ShadowsId",
                table: "FaceOverlayData",
                column: "ShadowsId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceOverlayData_SpotsId",
                table: "FaceOverlayData",
                column: "SpotsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobData_CharacterId",
                table: "JobData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionData_CharacterId",
                table: "PositionData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TextureData_CharacterId",
                table: "TextureData",
                column: "CharacterId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothesData");

            migrationBuilder.DropTable(
                name: "CoreData");

            migrationBuilder.DropTable(
                name: "EconomyData");

            migrationBuilder.DropTable(
                name: "FaceData");

            migrationBuilder.DropTable(
                name: "FaceOverlayData");

            migrationBuilder.DropTable(
                name: "JobData");

            migrationBuilder.DropTable(
                name: "PositionData");

            migrationBuilder.DropTable(
                name: "TextureData");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "OverlayData");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
