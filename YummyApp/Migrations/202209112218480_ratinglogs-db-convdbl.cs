namespace YummyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratinglogsdbconvdbl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RatingLogs", "Timestamp", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RatingLogs", "Timestamp", c => c.Long(nullable: false));
        }
    }
}
