namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFkUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FkUser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FkUser");
        }
    }
}
