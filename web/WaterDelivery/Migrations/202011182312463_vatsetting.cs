namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vatsetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "description", c => c.String());
            AddColumn("dbo.Settings", "vat", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "vat");
            DropColumn("dbo.Products", "description");
        }
    }
}
