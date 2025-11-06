namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveingUnnecessaryCarAtributes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cars", "UserRentedId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cars", "UserRentedId", c => c.Int(nullable: false));
        }
    }
}
