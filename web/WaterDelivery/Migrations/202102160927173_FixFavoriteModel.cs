namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixFavoriteModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Favorites", "FkUserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Favorites", "FkUserId", c => c.String());
        }
    }
}
