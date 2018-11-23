using Microsoft.EntityFrameworkCore.Migrations;

namespace LolApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LolItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChampionName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Health = table.Column<string>(nullable: true),
                    HealthRegen = table.Column<string>(nullable: true),
                    AttackDamage = table.Column<string>(nullable: true),
                    AttackSpeed = table.Column<string>(nullable: true),
                    MovementSpeed = table.Column<string>(nullable: true),
                    Armor = table.Column<string>(nullable: true),
                    MagicResist = table.Column<string>(nullable: true),
                    Abilities = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Uploaded = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LolItem", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LolItem");
        }
    }
}
