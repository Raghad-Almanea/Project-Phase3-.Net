namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendSettingAndClientModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Points", c => c.Int(nullable: false));
            AddColumn("dbo.Settings", "PointsPerOrder", c => c.Int(nullable: false));
            AddColumn("dbo.Settings", "PointsPerRiyal", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "PointsPerRiyal");
            DropColumn("dbo.Settings", "PointsPerOrder");
            DropColumn("dbo.Clients", "Points");
        }
    }
}
