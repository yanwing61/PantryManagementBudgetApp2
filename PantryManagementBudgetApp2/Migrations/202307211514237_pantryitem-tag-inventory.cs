namespace PantryManagementBudgetApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pantryitemtaginventory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        InventoryID = c.Int(nullable: false, identity: true),
                        InventoryQty = c.Int(nullable: false),
                        InventoryLogDate = c.DateTime(nullable: false),
                        PantryItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryID)
                .ForeignKey("dbo.PantryItems", t => t.PantryItemID, cascadeDelete: true)
                .Index(t => t.PantryItemID);
            
            CreateTable(
                "dbo.PantryItems",
                c => new
                    {
                        PantryItemID = c.Int(nullable: false, identity: true),
                        PantryItemName = c.String(),
                        PantryItemCurrentQty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PantryItemID);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagID = c.Int(nullable: false, identity: true),
                        TagName = c.String(),
                    })
                .PrimaryKey(t => t.TagID);
            
            CreateTable(
                "dbo.TagPantryItems",
                c => new
                    {
                        Tag_TagID = c.Int(nullable: false),
                        PantryItem_PantryItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_TagID, t.PantryItem_PantryItemID })
                .ForeignKey("dbo.Tags", t => t.Tag_TagID, cascadeDelete: true)
                .ForeignKey("dbo.PantryItems", t => t.PantryItem_PantryItemID, cascadeDelete: true)
                .Index(t => t.Tag_TagID)
                .Index(t => t.PantryItem_PantryItemID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "PantryItemID", "dbo.PantryItems");
            DropForeignKey("dbo.TagPantryItems", "PantryItem_PantryItemID", "dbo.PantryItems");
            DropForeignKey("dbo.TagPantryItems", "Tag_TagID", "dbo.Tags");
            DropIndex("dbo.TagPantryItems", new[] { "PantryItem_PantryItemID" });
            DropIndex("dbo.TagPantryItems", new[] { "Tag_TagID" });
            DropIndex("dbo.Inventories", new[] { "PantryItemID" });
            DropTable("dbo.TagPantryItems");
            DropTable("dbo.Tags");
            DropTable("dbo.PantryItems");
            DropTable("dbo.Inventories");
        }
    }
}
