namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCarBrandDbView : DbMigration
    {
        public override void Up()
        {
            //DropTable("dbo.CarBrands");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CarBrands",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
