namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class author_name_per_recipe_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Author", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Author");
        }
    }
}
