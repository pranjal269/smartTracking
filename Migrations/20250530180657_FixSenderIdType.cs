using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTracking.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixSenderIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "UserEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "UserEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "UserEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Parcels",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Parcels",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId1",
                table: "Parcels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlerId1",
                table: "HandlerLocations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_SenderId1",
                table: "Parcels",
                column: "SenderId1");

            migrationBuilder.CreateIndex(
                name: "IX_HandlerLocations_HandlerId1",
                table: "HandlerLocations",
                column: "HandlerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_HandlerLocations_UserEntries_HandlerId1",
                table: "HandlerLocations",
                column: "HandlerId1",
                principalTable: "UserEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_UserEntries_SenderId1",
                table: "Parcels",
                column: "SenderId1",
                principalTable: "UserEntries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandlerLocations_UserEntries_HandlerId1",
                table: "HandlerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_UserEntries_SenderId1",
                table: "Parcels");

            migrationBuilder.DropIndex(
                name: "IX_Parcels_SenderId1",
                table: "Parcels");

            migrationBuilder.DropIndex(
                name: "IX_HandlerLocations_HandlerId1",
                table: "HandlerLocations");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "HandlerId1",
                table: "HandlerLocations");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "UserEntries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "UserEntries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "UserEntries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserEntries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Parcels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Parcels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
