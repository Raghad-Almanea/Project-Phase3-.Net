namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFkCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FkCity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FkCity");
        }
    }
}
