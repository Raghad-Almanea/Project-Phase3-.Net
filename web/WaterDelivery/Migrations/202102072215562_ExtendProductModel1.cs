namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendProductModel1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ByUser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ByUser");
        }
    }
}
