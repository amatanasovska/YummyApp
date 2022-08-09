namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviewchange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reviews", "Recipe_Id", "dbo.Recipes");
            DropIndex("dbo.Reviews", new[] { "Recipe_Id" });
            RenameColumn(table: "dbo.Reviews", name: "Recipe_Id", newName: "RecipeId");
            AlterColumn("dbo.Reviews", "RecipeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reviews", "RecipeId");
            AddForeignKey("dbo.Reviews", "RecipeId", "dbo.Recipes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "RecipeId", "dbo.Recipes");
            DropIndex("dbo.Reviews", new[] { "RecipeId" });
            AlterColumn("dbo.Reviews", "RecipeId", c => c.Int());
            RenameColumn(table: "dbo.Reviews", name: "RecipeId", newName: "Recipe_Id");
            CreateIndex("dbo.Reviews", "Recipe_Id");
            AddForeignKey("dbo.Reviews", "Recipe_Id", "dbo.Recipes", "Id");
        }
    }
}
