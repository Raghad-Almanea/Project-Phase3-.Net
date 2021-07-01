namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavoriteModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favorites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FkUserId = c.String(),
                        FkProductId = c.Int(nullable: false),
                        Product_Id = c.Int(),
                        User_id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.Clients", t => t.User_id)
                .Index(t => t.Product_Id)
                .Index(t => t.User_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favorites", "User_id", "dbo.Clients");
            DropForeignKey("dbo.Favorites", "Product_Id", "dbo.Products");
            DropIndex("dbo.Favorites", new[] { "User_id" });
            DropIndex("dbo.Favorites", new[] { "Product_Id" });
            DropTable("dbo.Favorites");
        }
    }
}
