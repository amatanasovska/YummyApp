namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dailyrecipes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Posted", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Posted");
        }
    }
}
