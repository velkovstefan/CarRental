namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactions2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transactions", "UserIdFrom", c => c.String());
            AlterColumn("dbo.Transactions", "UserIdTo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "UserIdTo", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "UserIdFrom", c => c.Int(nullable: false));
        }
    }
}
