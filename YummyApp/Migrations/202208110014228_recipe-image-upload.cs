namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipeimageupload : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "file", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "file");
        }
    }
}
