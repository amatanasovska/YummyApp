namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dailyrecipes2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyRecipes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipeId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DailyRecipes");
        }
    }
}
