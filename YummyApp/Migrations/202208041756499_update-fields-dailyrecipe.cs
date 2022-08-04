namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefieldsdailyrecipe : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DailyRecipes", "RecipeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DailyRecipes", "RecipeId", c => c.String());
        }
    }
}
