namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LimitDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Recipes", "Description", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Recipes", "Description", c => c.String(nullable: false));
        }
    }
}
