namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userreviews : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Reviews", "ApplicationUser_Id");
            AddForeignKey("dbo.Reviews", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Reviews", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Reviews", "ApplicationUser_Id");
        }
    }
}
