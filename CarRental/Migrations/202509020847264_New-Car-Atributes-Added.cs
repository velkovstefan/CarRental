namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewCarAtributesAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "Brand", c => c.String());
            AddColumn("dbo.Cars", "Model", c => c.String());
            AddColumn("dbo.Cars", "Power", c => c.Int(nullable: false));
            AddColumn("dbo.Cars", "Consumption", c => c.Int(nullable: false));
            AddColumn("dbo.Cars", "FuelType", c => c.String());
            AddColumn("dbo.Cars", "NumberOfSeats", c => c.Int(nullable: false));
            AddColumn("dbo.Cars", "NumberOfDoors", c => c.Int(nullable: false));
            AddColumn("dbo.Cars", "CarType", c => c.String());
            AddColumn("dbo.Cars", "Capacity", c => c.Int(nullable: false));
            DropColumn("dbo.Cars", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cars", "Name", c => c.String());
            DropColumn("dbo.Cars", "Capacity");
            DropColumn("dbo.Cars", "CarType");
            DropColumn("dbo.Cars", "NumberOfDoors");
            DropColumn("dbo.Cars", "NumberOfSeats");
            DropColumn("dbo.Cars", "FuelType");
            DropColumn("dbo.Cars", "Consumption");
            DropColumn("dbo.Cars", "Power");
            DropColumn("dbo.Cars", "Model");
            DropColumn("dbo.Cars", "Brand");
        }
    }
}
