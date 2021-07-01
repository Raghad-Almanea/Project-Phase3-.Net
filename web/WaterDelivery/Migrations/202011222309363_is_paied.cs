namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class is_paied : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "is_paied", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "is_paied");
        }
    }
}
