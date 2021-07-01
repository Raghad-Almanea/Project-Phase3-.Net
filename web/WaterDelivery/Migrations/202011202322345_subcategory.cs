namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subcategory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "fk_categoryID", "dbo.Categories");
            DropIndex("dbo.Products", new[] { "fk_categoryID" });
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fk_cat = c.Int(nullable: false),
                        name = c.String(),
                        img = c.String(),
                        date = c.DateTime(nullable: false),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SubCategories");
            CreateIndex("dbo.Products", "fk_categoryID");
            AddForeignKey("dbo.Products", "fk_categoryID", "dbo.Categories", "Id", cascadeDelete: true);
        }
    }
}
