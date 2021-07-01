namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnToSettingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "Condtions_en", c => c.String());
            AddColumn("dbo.Settings", "aboutUs_en", c => c.String());
            AddColumn("dbo.Settings", "FooterDescription_en", c => c.String());
            AddColumn("dbo.Settings", "Title1_en", c => c.String());
            AddColumn("dbo.Settings", "Description1_en", c => c.String());
            AddColumn("dbo.Settings", "Title2_en", c => c.String());
            AddColumn("dbo.Settings", "Description2_en", c => c.String());
            AddColumn("dbo.Settings", "Title3_en", c => c.String());
            AddColumn("dbo.Settings", "Description3_en", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "Description3_en");
            DropColumn("dbo.Settings", "Title3_en");
            DropColumn("dbo.Settings", "Description2_en");
            DropColumn("dbo.Settings", "Title2_en");
            DropColumn("dbo.Settings", "Description1_en");
            DropColumn("dbo.Settings", "Title1_en");
            DropColumn("dbo.Settings", "FooterDescription_en");
            DropColumn("dbo.Settings", "aboutUs_en");
            DropColumn("dbo.Settings", "Condtions_en");
        }
    }
}
