namespace CarRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transmissionandnoTimeRentedadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "Transmission", c => c.String());
            AddColumn("dbo.Cars", "NumderOfTimesRented", c => c.Int(nullable: false));
            Sql(@"CREATE VIEW CarBrands AS
                    SELECT 
                        CAST(ROW_NUMBER() OVER (ORDER BY Brand) AS int) AS Id,
                        Brand
                    FROM (
                        SELECT DISTINCT Brand
                        FROM Cars
                    ) AS Brand;");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "NumderOfTimesRented");
            DropColumn("dbo.Cars", "Transmission");
            Sql("DROP VIEW CarBrands");
        }
    }
}
