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
            migrationBuilder.CreateSequence(
                name: "receipt_number_seq");

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
                name: "receipt",
                columns: table => new
                {
                    receipt_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    receipt_number = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('receipt_number_seq')"),
                    total_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt", x => x.receipt_id);
                });

            migrationBuilder.CreateTable(
                name: "discount",
                columns: table => new
                {
                    discount_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    discount_type = table.Column<int>(type: "integer", nullable: false),
                    percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discount", x => x.discount_id);
                    table.ForeignKey(
                        name: "FK_discount_item_item_id",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_ordered",
                columns: table => new
                {
                    item_ordered_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    receipt_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    sub_total_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    is_discounted = table.Column<bool>(type: "boolean", nullable: false),
                    discount_id = table.Column<long>(type: "bigint", nullable: true),
                    discounted_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    total_cost = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_ordered", x => x.item_ordered_id);
                    table.ForeignKey(
                        name: "FK_item_ordered_discount_discount_id",
                        column: x => x.discount_id,
                        principalTable: "discount",
                        principalColumn: "discount_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_item_ordered_item_item_id",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_ordered_receipt_receipt_id",
                        column: x => x.receipt_id,
                        principalTable: "receipt",
                        principalColumn: "receipt_id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.InsertData(
                table: "discount",
                columns: new[] { "discount_id", "discount_type", "end_date", "is_active", "item_id", "name", "percentage", "start_date" },
                values: new object[,]
                {
                    { 1L, 0, null, true, 4L, "Apples 10% off", 10m, null },
                    { 2L, 1, null, true, 2L, "Buy 2 soups get bread half price", 50m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_discount_item_id",
                table: "discount",
                column: "item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_ordered_discount_id",
                table: "item_ordered",
                column: "discount_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_ordered_item_id",
                table: "item_ordered",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_ordered_receipt_id",
                table: "item_ordered",
                column: "receipt_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_ordered");

            migrationBuilder.DropTable(
                name: "discount");

            migrationBuilder.DropTable(
                name: "receipt");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropSequence(
                name: "receipt_number_seq");
        }
    }
}
