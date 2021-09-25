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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    License = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastConnection = table.Column<DateTime>(nullable: false),
                    PermissionLevel = table.Column<int>(nullable: false),
                    IsBanned = table.Column<bool>(nullable: false),
                    IsWhitelisted = table.Column<bool>(nullable: false),
                    IsConnected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    License = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    LastUsing = table.Column<DateTime>(nullable: false),
                    Firstname = table.Column<string>(nullable: false),
                    Lastname = table.Column<string>(nullable: false),
                    Nationality = table.Column<string>(nullable: false),
                    CityOfBirth = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                name: "OutfitData",
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
                    table.PrimaryKey("PK_OutfitData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutfitData_Characters_CharacterId",
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
                name: "SkinData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Head = table.Column<uint>(nullable: false),
                    Body = table.Column<uint>(nullable: false),
                    Legs = table.Column<uint>(nullable: false),
                    BodyType = table.Column<int>(nullable: false),
                    WaistType = table.Column<int>(nullable: false),
                    Scale = table.Column<float>(nullable: false),
                    HeadWidth = table.Column<float>(nullable: false),
                    EyebrowHeight = table.Column<float>(nullable: false),
                    EyebrowWidth = table.Column<float>(nullable: false),
                    EyebrowDepth = table.Column<float>(nullable: false),
                    EarsWidth = table.Column<float>(nullable: false),
                    EarsAngle = table.Column<float>(nullable: false),
                    EarsHeight = table.Column<float>(nullable: false),
                    EarsLobeSize = table.Column<float>(nullable: false),
                    CheeckBonesHeight = table.Column<float>(nullable: false),
                    CheeckBonesWidth = table.Column<float>(nullable: false),
                    CheeckBonesDepth = table.Column<float>(nullable: false),
                    JawHeight = table.Column<float>(nullable: false),
                    JawWidth = table.Column<float>(nullable: false),
                    JawDepth = table.Column<float>(nullable: false),
                    ChinHeight = table.Column<float>(nullable: false),
                    ChinWidth = table.Column<float>(nullable: false),
                    ChinDepth = table.Column<float>(nullable: false),
                    EyeLidHeight = table.Column<float>(nullable: false),
                    EyeLidWidth = table.Column<float>(nullable: false),
                    EyesDepth = table.Column<float>(nullable: false),
                    EyesAngle = table.Column<float>(nullable: false),
                    EyesDistance = table.Column<float>(nullable: false),
                    EyesHeight = table.Column<float>(nullable: false),
                    NoseWidth = table.Column<float>(nullable: false),
                    NoseSize = table.Column<float>(nullable: false),
                    NoseHeight = table.Column<float>(nullable: false),
                    NoseAngle = table.Column<float>(nullable: false),
                    NoseCurvature = table.Column<float>(nullable: false),
                    NoStrilsDistance = table.Column<float>(nullable: false),
                    MouthWidth = table.Column<float>(nullable: false),
                    MouthDepth = table.Column<float>(nullable: false),
                    MouthXPos = table.Column<float>(nullable: false),
                    MouthYPos = table.Column<float>(nullable: false),
                    UpperLipHeight = table.Column<float>(nullable: false),
                    UpperLipWidth = table.Column<float>(nullable: false),
                    UpperLipDepth = table.Column<float>(nullable: false),
                    LowerLipHeight = table.Column<float>(nullable: false),
                    LowerLipWidth = table.Column<float>(nullable: false),
                    LowerLipDepth = table.Column<float>(nullable: false),
                    Albedo = table.Column<int>(nullable: false),
                    Normal = table.Column<int>(nullable: false),
                    Material = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkinData_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverlayData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SkinId = table.Column<long>(nullable: false),
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
                    table.ForeignKey(
                        name: "FK_OverlayData_SkinData_SkinId",
                        column: x => x.SkinId,
                        principalTable: "SkinData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

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
                name: "IX_JobData_CharacterId",
                table: "JobData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutfitData_CharacterId",
                table: "OutfitData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OverlayData_SkinId",
                table: "OverlayData",
                column: "SkinId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionData_CharacterId",
                table: "PositionData",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkinData_CharacterId",
                table: "SkinData",
                column: "CharacterId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoreData");

            migrationBuilder.DropTable(
                name: "EconomyData");

            migrationBuilder.DropTable(
                name: "JobData");

            migrationBuilder.DropTable(
                name: "OutfitData");

            migrationBuilder.DropTable(
                name: "OverlayData");

            migrationBuilder.DropTable(
                name: "PositionData");

            migrationBuilder.DropTable(
                name: "SkinData");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
