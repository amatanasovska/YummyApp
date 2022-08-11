namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class savedrecipeimpl3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Recipes", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Recipes", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Recipes", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Recipes", "ApplicationUser_Id");
            AddForeignKey("dbo.Recipes", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
