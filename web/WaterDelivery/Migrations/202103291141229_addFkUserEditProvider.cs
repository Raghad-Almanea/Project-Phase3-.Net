namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFkUserEditProvider : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Providers", "FkUser", c => c.String());
            DropColumn("dbo.AspNetUsers", "FkUser");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "FkUser", c => c.String());
            DropColumn("dbo.Providers", "FkUser");
        }
    }
}
