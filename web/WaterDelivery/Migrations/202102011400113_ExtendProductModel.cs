namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "specification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "specification");
        }
    }
}
