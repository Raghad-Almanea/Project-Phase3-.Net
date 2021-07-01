namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CatLang : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "name_en", c => c.String());
            AddColumn("dbo.SubCategories", "name_en", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubCategories", "name_en");
            DropColumn("dbo.Categories", "name_en");
        }
    }
}
