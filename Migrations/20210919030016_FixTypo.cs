﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Average.Server.Migrations
{
    public partial class FixTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AcneId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AgeingId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BeardstabbleId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BlushId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ComplexId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_DiscId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyebrowsId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyelinersId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FoundationId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FrecklesId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_GrimeId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_HairId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_LipsticksId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_MolesId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_PaintedmasksId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ScarsId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ShadowsId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_SpotsId",
                table: "FaceOverlayData");

            migrationBuilder.DropColumn(
                name: "Licence",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "Users",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OverlayData",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JobData",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "SpotsId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ShadowsId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ScarsId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "PaintedmasksId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "MolesId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "LipsticksId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "HairId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "GrimeId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "FrecklesId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "FoundationId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "EyelinersId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "EyebrowsId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "DiscId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ComplexId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "BlushId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "BeardstabbleId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AgeingId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AcneId",
                table: "FaceOverlayData",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "License",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Lastname",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Firstname",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DateOfBirth",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CityOfBirth",
                table: "Characters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AcneId",
                table: "FaceOverlayData",
                column: "AcneId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AgeingId",
                table: "FaceOverlayData",
                column: "AgeingId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BeardstabbleId",
                table: "FaceOverlayData",
                column: "BeardstabbleId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BlushId",
                table: "FaceOverlayData",
                column: "BlushId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ComplexId",
                table: "FaceOverlayData",
                column: "ComplexId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_DiscId",
                table: "FaceOverlayData",
                column: "DiscId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyebrowsId",
                table: "FaceOverlayData",
                column: "EyebrowsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyelinersId",
                table: "FaceOverlayData",
                column: "EyelinersId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FoundationId",
                table: "FaceOverlayData",
                column: "FoundationId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FrecklesId",
                table: "FaceOverlayData",
                column: "FrecklesId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_GrimeId",
                table: "FaceOverlayData",
                column: "GrimeId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_HairId",
                table: "FaceOverlayData",
                column: "HairId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_LipsticksId",
                table: "FaceOverlayData",
                column: "LipsticksId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_MolesId",
                table: "FaceOverlayData",
                column: "MolesId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_PaintedmasksId",
                table: "FaceOverlayData",
                column: "PaintedmasksId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ScarsId",
                table: "FaceOverlayData",
                column: "ScarsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ShadowsId",
                table: "FaceOverlayData",
                column: "ShadowsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_SpotsId",
                table: "FaceOverlayData",
                column: "SpotsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AcneId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AgeingId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BeardstabbleId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BlushId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ComplexId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_DiscId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyebrowsId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyelinersId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FoundationId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FrecklesId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_GrimeId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_HairId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_LipsticksId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_MolesId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_PaintedmasksId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ScarsId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ShadowsId",
                table: "FaceOverlayData");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceOverlayData_OverlayData_SpotsId",
                table: "FaceOverlayData");

            migrationBuilder.DropColumn(
                name: "License",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Licence",
                table: "Users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OverlayData",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "JobData",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SpotsId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ShadowsId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ScarsId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PaintedmasksId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MolesId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "LipsticksId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "HairId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GrimeId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FrecklesId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FoundationId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EyelinersId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EyebrowsId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DiscId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ComplexId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BlushId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BeardstabbleId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AgeingId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AcneId",
                table: "FaceOverlayData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "License",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lastname",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Firstname",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DateOfBirth",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CityOfBirth",
                table: "Characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AcneId",
                table: "FaceOverlayData",
                column: "AcneId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_AgeingId",
                table: "FaceOverlayData",
                column: "AgeingId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BeardstabbleId",
                table: "FaceOverlayData",
                column: "BeardstabbleId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_BlushId",
                table: "FaceOverlayData",
                column: "BlushId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ComplexId",
                table: "FaceOverlayData",
                column: "ComplexId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_DiscId",
                table: "FaceOverlayData",
                column: "DiscId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyebrowsId",
                table: "FaceOverlayData",
                column: "EyebrowsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_EyelinersId",
                table: "FaceOverlayData",
                column: "EyelinersId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FoundationId",
                table: "FaceOverlayData",
                column: "FoundationId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_FrecklesId",
                table: "FaceOverlayData",
                column: "FrecklesId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_GrimeId",
                table: "FaceOverlayData",
                column: "GrimeId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_HairId",
                table: "FaceOverlayData",
                column: "HairId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_LipsticksId",
                table: "FaceOverlayData",
                column: "LipsticksId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_MolesId",
                table: "FaceOverlayData",
                column: "MolesId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_PaintedmasksId",
                table: "FaceOverlayData",
                column: "PaintedmasksId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ScarsId",
                table: "FaceOverlayData",
                column: "ScarsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_ShadowsId",
                table: "FaceOverlayData",
                column: "ShadowsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceOverlayData_OverlayData_SpotsId",
                table: "FaceOverlayData",
                column: "SpotsId",
                principalTable: "OverlayData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}