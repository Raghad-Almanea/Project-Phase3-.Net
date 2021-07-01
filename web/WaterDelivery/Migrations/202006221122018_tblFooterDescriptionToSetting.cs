namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblFooterDescriptionToSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "FooterDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "FooterDescription");
        }
    }
}
