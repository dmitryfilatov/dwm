﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeltaWorkMonitoring.Migrations.AppIdentityDb
{
    public partial class Customproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "City",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Qualifications",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Qualifications",
                table: "AspNetUsers");
        }
    }
}
