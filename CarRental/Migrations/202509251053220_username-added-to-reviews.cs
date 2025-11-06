namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usernameaddedtoreviews : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reviews", "UserName");
        }
    }
}
