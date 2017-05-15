namespace WorldOfWords.Infrastructure.Data.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WordSuiteMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WordSuites", "IsPictureQuizAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.WordSuites", "IsSoundQuizAllowed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WordSuites", "IsSoundQuizAllowed");
            DropColumn("dbo.WordSuites", "IsPictureQuizAllowed");
        }
    }
}
