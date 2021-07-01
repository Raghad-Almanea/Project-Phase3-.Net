namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPhone2andAAddressToSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "phone2", c => c.String());
            AddColumn("dbo.Settings", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Settings", "Address");
            DropColumn("dbo.Settings", "phone2");
        }
    }
}
