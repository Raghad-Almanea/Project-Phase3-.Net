namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddtitleToSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "Title1", c => c.String());
            AddColumn("dbo.Settings", "Description1", c => c.String());
            AddColumn("dbo.Settings", "Title2", c => c.String());
            AddColumn("dbo.Settings", "Description2", c => c.String());
            AddColumn("dbo.Settings", "Title3", c => c.String());
            AddColumn("dbo.Settings", "Description3", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "Description3");
            DropColumn("dbo.Settings", "Title3");
            DropColumn("dbo.Settings", "Description2");
            DropColumn("dbo.Settings", "Title2");
            DropColumn("dbo.Settings", "Description1");
            DropColumn("dbo.Settings", "Title1");
        }
    }
}
