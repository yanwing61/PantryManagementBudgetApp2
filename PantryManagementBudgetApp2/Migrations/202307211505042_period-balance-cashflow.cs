namespace PantryManagementBudgetApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class periodbalancecashflow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Balances",
                c => new
                    {
                        BalanceId = c.Int(nullable: false, identity: true),
                        PeriodId = c.Int(nullable: false),
                        ChequingAcct = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SavingsAcct = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevolvingCrdt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InstalmentCrdt = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BalanceId)
                .ForeignKey("dbo.Periods", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId);
            
            CreateTable(
                "dbo.Periods",
                c => new
                    {
                        PeriodId = c.Int(nullable: false, identity: true),
                        PeriodYear = c.Int(nullable: false),
                        PeriodMonth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PeriodId);
            
            CreateTable(
                "dbo.Cashflows",
                c => new
                    {
                        CashflowId = c.Int(nullable: false, identity: true),
                        PeriodId = c.Int(nullable: false),
                        Budget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expense = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CashflowId)
                .ForeignKey("dbo.Periods", t => t.PeriodId, cascadeDelete: true)
                .Index(t => t.PeriodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cashflows", "PeriodId", "dbo.Periods");
            DropForeignKey("dbo.Balances", "PeriodId", "dbo.Periods");
            DropIndex("dbo.Cashflows", new[] { "PeriodId" });
            DropIndex("dbo.Balances", new[] { "PeriodId" });
            DropTable("dbo.Cashflows");
            DropTable("dbo.Periods");
            DropTable("dbo.Balances");
        }
    }
}
