using Microsoft.EntityFrameworkCore.Migrations;

public partial class UpdateProductImageUrl : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // حذف العمود
        migrationBuilder.DropColumn(
            name: "ImageUrl",
            table: "Products");

        // إضافة العمود مرة أخرى بنوع البيانات الجديد
        migrationBuilder.AddColumn<byte[]>(
            name: "ImageUrl",
            table: "Products",
            type: "varbinary(max)",
            nullable: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // لإعادة العمود لنوع البيانات السابق إذا لزم الأمر
        migrationBuilder.DropColumn(
            name: "ImageUrl",
            table: "Products");

        migrationBuilder.AddColumn<string>(
            name: "ImageUrl",
            table: "Products",
            type: "nvarchar(max)",
            nullable: false);
    }
}
