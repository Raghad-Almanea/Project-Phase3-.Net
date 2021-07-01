namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWalletToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "wallet", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "payment_type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "payment_type");
            DropColumn("dbo.Clients", "wallet");
        }
    }
}
