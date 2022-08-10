namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mainpicturerecipe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "picture_Id", c => c.Int());
            CreateIndex("dbo.Recipes", "picture_Id");
            AddForeignKey("dbo.Recipes", "picture_Id", "dbo.Pictures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "picture_Id", "dbo.Pictures");
            DropIndex("dbo.Recipes", new[] { "picture_Id" });
            DropColumn("dbo.Recipes", "picture_Id");
        }
    }
}
