namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class savedrecipesimpl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Recipes", "ApplicationUser_Id");
            AddForeignKey("dbo.Recipes", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Recipes", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Recipes", "ApplicationUser_Id");
        }
    }
}
