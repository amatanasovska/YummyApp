namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipeimageuploadchange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Recipes", "file", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Recipes", "file", c => c.String(nullable: false));
        }
    }
}
