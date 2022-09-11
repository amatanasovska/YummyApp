namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class category_recipe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Type");
        }
    }
}
