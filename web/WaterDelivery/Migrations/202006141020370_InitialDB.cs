namespace WaterDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fk_userID = c.Int(nullable: false),
                        title = c.String(),
                        address = c.String(),
                        lat = c.String(),
                        lng = c.String(),
                        is_used = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.fk_userID, cascadeDelete: true)
                .Index(t => t.fk_userID);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fk_cityID = c.Int(nullable: false),
                        user_name = c.String(),
                        phone = c.String(),
                        password = c.String(),
                        code = c.String(),
                        active_code = c.Boolean(nullable: false),
                        lang = c.String(),
                        img = c.String(),
                        device_type = c.String(),
                        user_type = c.String(),
                        active = c.Boolean(nullable: false),
                        notification = c.Boolean(nullable: false),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Cities", t => t.fk_cityID, cascadeDelete: true)
                .Index(t => t.fk_cityID);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        date = c.String(),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fk_userID = c.Int(nullable: false),
                        fk_providerID = c.Int(),
                        type = c.Int(nullable: false),
                        total = c.Double(nullable: false),
                        delivary = c.Double(nullable: false),
                        delivary_time = c.DateTime(),
                        net_total = c.Double(nullable: false),
                        address = c.String(),
                        lat = c.String(),
                        lng = c.String(),
                        date = c.String(),
                        date_time = c.DateTime(nullable: false),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Providers", t => t.fk_providerID)
                .ForeignKey("dbo.Clients", t => t.fk_userID, cascadeDelete: true)
                .Index(t => t.fk_userID)
                .Index(t => t.fk_providerID);
            
            CreateTable(
                "dbo.Providers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fk_city = c.Int(nullable: false),
                        user_name = c.String(),
                        phone = c.String(),
                        address = c.String(),
                        lat = c.String(),
                        lng = c.String(),
                        password = c.String(),
                        code = c.String(),
                        active_code = c.Boolean(nullable: false),
                        lang = c.String(),
                        img = c.String(),
                        drive_licence_img = c.String(),
                        national_id_img = c.String(),
                        device_type = c.String(),
                        user_type = c.String(),
                        active = c.Boolean(nullable: false),
                        rate = c.Double(nullable: false),
                        notification = c.Boolean(nullable: false),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        fk_userID = c.Int(nullable: false),
                        fk_providerID = c.Int(nullable: false),
                        rate = c.Int(nullable: false),
                        comment = c.String(),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Providers", t => t.fk_providerID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.fk_userID, cascadeDelete: true)
                .Index(t => t.fk_userID)
                .Index(t => t.fk_providerID);
            
            CreateTable(
                "dbo.OrderInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fk_orderID = c.Int(nullable: false),
                        fk_product = c.Int(nullable: false),
                        qty = c.Int(nullable: false),
                        name = c.String(),
                        price = c.Double(nullable: false),
                        img = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.fk_orderID, cascadeDelete: true)
                .Index(t => t.fk_orderID);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        fk_userID = c.Int(nullable: false),
                        fk_productID = c.Int(nullable: false),
                        qty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Products", t => t.fk_productID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.fk_userID, cascadeDelete: true)
                .Index(t => t.fk_userID)
                .Index(t => t.fk_productID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        price = c.Double(nullable: false),
                        img = c.String(),
                        date = c.DateTime(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        fk_categoryID = c.Int(nullable: false),
                        all_qty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.fk_categoryID, cascadeDelete: true)
                .Index(t => t.fk_categoryID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        img = c.String(),
                        date = c.DateTime(nullable: false),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Complaints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        email = c.String(),
                        text = c.String(),
                        date = c.DateTime(nullable: false),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Device_Id",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        fk_user = c.Int(nullable: false),
                        is_client = c.Boolean(nullable: false),
                        device_id = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Notifies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fk_user = c.Int(nullable: false),
                        fk_provider = c.Int(nullable: false),
                        text = c.String(),
                        date = c.DateTime(nullable: false),
                        order_id = c.Int(),
                        order_type = c.Int(),
                        fk_user_show = c.Boolean(nullable: false),
                        fk_provider_show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Condtions = c.String(),
                        phone = c.String(),
                        facebook = c.String(),
                        whatapp = c.String(),
                        instgram = c.String(),
                        twitter = c.String(),
                        snapchat = c.String(),
                        aboutUs = c.String(),
                        delivery = c.Double(nullable: false),
                        delivery_default_distance_in_km = c.Double(nullable: false),
                        delivery_for_aditinal_km = c.Double(nullable: false),
                        Lat = c.String(),
                        Lng = c.String(),
                        GooglePlayUrl = c.String(),
                        AppleStoreUrl = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Sliders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        type = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Img = c.String(),
                        Type = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Carts", "fk_userID", "dbo.Clients");
            DropForeignKey("dbo.Products", "fk_categoryID", "dbo.Categories");
            DropForeignKey("dbo.Carts", "fk_productID", "dbo.Products");
            DropForeignKey("dbo.OrderInfoes", "fk_orderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "fk_userID", "dbo.Clients");
            DropForeignKey("dbo.Rates", "fk_userID", "dbo.Clients");
            DropForeignKey("dbo.Rates", "fk_providerID", "dbo.Providers");
            DropForeignKey("dbo.Orders", "fk_providerID", "dbo.Providers");
            DropForeignKey("dbo.Clients", "fk_cityID", "dbo.Cities");
            DropForeignKey("dbo.AddressUsers", "fk_userID", "dbo.Clients");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Products", new[] { "fk_categoryID" });
            DropIndex("dbo.Carts", new[] { "fk_productID" });
            DropIndex("dbo.Carts", new[] { "fk_userID" });
            DropIndex("dbo.OrderInfoes", new[] { "fk_orderID" });
            DropIndex("dbo.Rates", new[] { "fk_providerID" });
            DropIndex("dbo.Rates", new[] { "fk_userID" });
            DropIndex("dbo.Orders", new[] { "fk_providerID" });
            DropIndex("dbo.Orders", new[] { "fk_userID" });
            DropIndex("dbo.Clients", new[] { "fk_cityID" });
            DropIndex("dbo.AddressUsers", new[] { "fk_userID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Sliders");
            DropTable("dbo.Settings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Notifies");
            DropTable("dbo.Device_Id");
            DropTable("dbo.Complaints");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
            DropTable("dbo.OrderInfoes");
            DropTable("dbo.Rates");
            DropTable("dbo.Providers");
            DropTable("dbo.Orders");
            DropTable("dbo.Cities");
            DropTable("dbo.Clients");
            DropTable("dbo.AddressUsers");
        }
    }
}
