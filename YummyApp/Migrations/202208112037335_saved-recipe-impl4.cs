namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class savedrecipeimpl4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SavedRecipeUsers", "Recipe_Id", "dbo.Recipes");
            DropForeignKey("dbo.SavedRecipeUsers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.SavedRecipeUsers", new[] { "Recipe_Id" });
            DropIndex("dbo.SavedRecipeUsers", new[] { "User_Id" });
            AddColumn("dbo.SavedRecipeUsers", "UserId", c => c.String());
            AddColumn("dbo.SavedRecipeUsers", "RecipeId", c => c.Int(nullable: false));
            DropColumn("dbo.SavedRecipeUsers", "Recipe_Id");
            DropColumn("dbo.SavedRecipeUsers", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SavedRecipeUsers", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.SavedRecipeUsers", "Recipe_Id", c => c.Int());
            DropColumn("dbo.SavedRecipeUsers", "RecipeId");
            DropColumn("dbo.SavedRecipeUsers", "UserId");
            CreateIndex("dbo.SavedRecipeUsers", "User_Id");
            CreateIndex("dbo.SavedRecipeUsers", "Recipe_Id");
            AddForeignKey("dbo.SavedRecipeUsers", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.SavedRecipeUsers", "Recipe_Id", "dbo.Recipes", "Id");
        }
    }
}
