namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratinglogsdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RatingLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        RecipeId = c.Int(nullable: false),
                        Timestamp = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RatingLogs");
        }
    }
}
