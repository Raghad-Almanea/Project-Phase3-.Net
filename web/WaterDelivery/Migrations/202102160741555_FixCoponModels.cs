namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCoponModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Copons",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        count = c.Int(nullable: false),
                        count_used = c.Int(nullable: false),
                        expirdate = c.DateTime(nullable: false),
                        copon_code = c.String(),
                        discount = c.Double(nullable: false),
                        limt_discount = c.Double(nullable: false),
                        isActive = c.Boolean(nullable: false),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.CoponUseds",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fk_copon = c.Int(nullable: false),
                        fk_order = c.Int(nullable: false),
                        fk_user = c.Int(nullable: false),
                        Copon_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Copons", t => t.Copon_id)
                .Index(t => t.Copon_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CoponUseds", "Copon_id", "dbo.Copons");
            DropIndex("dbo.CoponUseds", new[] { "Copon_id" });
            DropTable("dbo.CoponUseds");
            DropTable("dbo.Copons");
        }
    }
}
