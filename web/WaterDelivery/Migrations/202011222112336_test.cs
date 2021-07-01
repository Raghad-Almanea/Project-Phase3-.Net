namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AddressUsers", "is_active", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Orders", "delivary_time", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "delivary_time", c => c.DateTime());
            DropColumn("dbo.AddressUsers", "is_active");
        }
    }
}
