namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class propertyvisibilityrecipe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "IsPublic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "IsPublic");
        }
    }
}
