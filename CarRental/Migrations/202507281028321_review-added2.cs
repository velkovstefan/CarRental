namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviewadded2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reviews", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "UserId", c => c.Int(nullable: false));
        }
    }
}
