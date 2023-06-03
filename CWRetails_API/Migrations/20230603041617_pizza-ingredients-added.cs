using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CWRetails_API.Migrations
{
    /// <inheritdoc />
    public partial class pizzaingredientsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientPizza_Ingredients_IngredientsId",
                table: "IngredientPizza");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientPizza_Pizzas_PizzasId",
                table: "IngredientPizza");

            migrationBuilder.DropTable(
                name: "PizzaTopping");

            migrationBuilder.RenameColumn(
                name: "PizzasId",
                table: "IngredientPizza",
                newName: "PizzaId");

            migrationBuilder.RenameColumn(
                name: "IngredientsId",
                table: "IngredientPizza",
                newName: "IngredientId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientPizza_PizzasId",
                table: "IngredientPizza",
                newName: "IX_IngredientPizza_PizzaId");

            migrationBuilder.CreateTable(
                name: "ToppingPizza",
                columns: table => new
                {
                    PizzaId = table.Column<int>(type: "int", nullable: false),
                    ToppingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToppingPizza", x => new { x.PizzaId, x.ToppingId });
                    table.ForeignKey(
                        name: "FK_ToppingPizza_Pizzas_PizzaId",
                        column: x => x.PizzaId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToppingPizza_Topping_ToppingId",
                        column: x => x.ToppingId,
                        principalTable: "Topping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IngredientPizza",
                columns: new[] { "IngredientId", "PizzaId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 2, 1 },
                    { 2, 4 },
                    { 3, 1 },
                    { 3, 4 },
                    { 3, 5 },
                    { 4, 1 },
                    { 4, 4 },
                    { 4, 5 },
                    { 5, 2 },
                    { 6, 2 },
                    { 6, 5 },
                    { 7, 2 },
                    { 8, 3 },
                    { 9, 3 },
                    { 10, 3 },
                    { 11, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToppingPizza_ToppingId",
                table: "ToppingPizza",
                column: "ToppingId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientPizza_Ingredients_IngredientId",
                table: "IngredientPizza",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientPizza_Pizzas_PizzaId",
                table: "IngredientPizza",
                column: "PizzaId",
                principalTable: "Pizzas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientPizza_Ingredients_IngredientId",
                table: "IngredientPizza");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientPizza_Pizzas_PizzaId",
                table: "IngredientPizza");

            migrationBuilder.DropTable(
                name: "ToppingPizza");

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 9, 3 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "IngredientPizza",
                keyColumns: new[] { "IngredientId", "PizzaId" },
                keyValues: new object[] { 11, 5 });

            migrationBuilder.RenameColumn(
                name: "PizzaId",
                table: "IngredientPizza",
                newName: "PizzasId");

            migrationBuilder.RenameColumn(
                name: "IngredientId",
                table: "IngredientPizza",
                newName: "IngredientsId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientPizza_PizzaId",
                table: "IngredientPizza",
                newName: "IX_IngredientPizza_PizzasId");

            migrationBuilder.CreateTable(
                name: "PizzaTopping",
                columns: table => new
                {
                    PizzasId = table.Column<int>(type: "int", nullable: false),
                    ToppingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaTopping", x => new { x.PizzasId, x.ToppingsId });
                    table.ForeignKey(
                        name: "FK_PizzaTopping_Pizzas_PizzasId",
                        column: x => x.PizzasId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PizzaTopping_Topping_ToppingsId",
                        column: x => x.ToppingsId,
                        principalTable: "Topping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PizzaTopping_ToppingsId",
                table: "PizzaTopping",
                column: "ToppingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientPizza_Ingredients_IngredientsId",
                table: "IngredientPizza",
                column: "IngredientsId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientPizza_Pizzas_PizzasId",
                table: "IngredientPizza",
                column: "PizzasId",
                principalTable: "Pizzas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
