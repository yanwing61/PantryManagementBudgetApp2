namespace PantryManagementBudgetApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        PurchaseID = c.Int(nullable: false, identity: true),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Qty = c.Int(nullable: false),
                        PeriodId = c.Int(nullable: false),
                        PantryItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseID)
                .ForeignKey("dbo.PantryItems", t => t.PantryItemID, cascadeDelete: true)
                .ForeignKey("dbo.Periods", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId)
                .Index(t => t.PantryItemID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "PeriodId", "dbo.Periods");
            DropForeignKey("dbo.Purchases", "PantryItemID", "dbo.PantryItems");
            DropIndex("dbo.Purchases", new[] { "PantryItemID" });
            DropIndex("dbo.Purchases", new[] { "PeriodId" });
            DropTable("dbo.Purchases");
        }
    }
}
