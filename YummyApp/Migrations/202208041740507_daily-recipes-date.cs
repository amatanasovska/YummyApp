namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dailyrecipesdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyRecipes", "ValidityDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyRecipes", "ValidityDate");
        }
    }
}
