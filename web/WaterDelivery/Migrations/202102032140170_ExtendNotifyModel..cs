namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendNotifyModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifies", "type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifies", "type");
        }
    }
}
