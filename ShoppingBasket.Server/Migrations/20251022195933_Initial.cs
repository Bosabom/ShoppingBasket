using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoppingBasket.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    item_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item", x => x.item_id);
                });

            migrationBuilder.CreateTable(
                name: "item_ordered",
                columns: table => new
                {
                    item_ordered_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    is_discounted = table.Column<bool>(type: "boolean", nullable: false),
                    discounted_price = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_ordered", x => x.item_ordered_id);
                });

            migrationBuilder.CreateTable(
                name: "receipt",
                columns: table => new
                {
                    receipt_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_ordered_ids = table.Column<long[]>(type: "bigint[]", nullable: false),
                    sub_total_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    total_discount = table.Column<decimal>(type: "numeric", nullable: true),
                    total_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt", x => x.receipt_id);
                });

            migrationBuilder.InsertData(
                table: "item",
                columns: new[] { "item_id", "description", "item_type", "price" },
                values: new object[,]
                {
                    { 1L, "Tomato Soup (400g)", 0, 0.65m },
                    { 2L, "Wholemeal Bread (800g)", 1, 0.80m },
                    { 3L, "Semi-skimmed Milk (1L)", 2, 1.30m },
                    { 4L, "Apples bag", 3, 1m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "item_ordered");

            migrationBuilder.DropTable(
                name: "receipt");
        }
    }
}
