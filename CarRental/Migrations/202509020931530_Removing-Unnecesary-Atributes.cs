namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingUnnecesaryAtributes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Cars", "AvalibleAtDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cars", "AvalibleAtDate", c => c.DateTime(nullable: false));
        }
    }
}
