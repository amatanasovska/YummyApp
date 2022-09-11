namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dellogs : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RatingLogs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RatingLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        RecipeId = c.Int(nullable: false),
                        Timestamp = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
