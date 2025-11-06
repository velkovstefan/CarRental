namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactions1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "UserIdFrom", c => c.Int(nullable: false));
            AddColumn("dbo.Transactions", "UserIdTo", c => c.Int(nullable: false));
            DropColumn("dbo.Transactions", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.Transactions", "UserIdTo");
            DropColumn("dbo.Transactions", "UserIdFrom");
        }
    }
}
