namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class savedrecipeimpl2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SavedRecipeUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Recipe_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Recipes", t => t.Recipe_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Recipe_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavedRecipeUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SavedRecipeUsers", "Recipe_Id", "dbo.Recipes");
            DropIndex("dbo.SavedRecipeUsers", new[] { "User_Id" });
            DropIndex("dbo.SavedRecipeUsers", new[] { "Recipe_Id" });
            DropTable("dbo.SavedRecipeUsers");
        }
    }
}
