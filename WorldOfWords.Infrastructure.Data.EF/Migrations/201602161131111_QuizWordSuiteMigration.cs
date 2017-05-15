namespace WorldOfWords.Infrastructure.Data.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizWordSuiteMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Quizzes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordSuiteQuiz",
                c => new
                    {
                        QuizId = c.Int(nullable: false),
                        WordSuiteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuizId, t.WordSuiteId })
                .ForeignKey("dbo.Quizzes", t => t.QuizId, cascadeDelete: true)
                .ForeignKey("dbo.WordSuites", t => t.WordSuiteId, cascadeDelete: true)
                .Index(t => t.QuizId)
                .Index(t => t.WordSuiteId);
            
            DropColumn("dbo.WordSuites", "IsPictureQuizAllowed");
            DropColumn("dbo.WordSuites", "IsSoundQuizAllowed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WordSuites", "IsSoundQuizAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.WordSuites", "IsPictureQuizAllowed", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.WordSuiteQuiz", "WordSuiteId", "dbo.WordSuites");
            DropForeignKey("dbo.WordSuiteQuiz", "QuizId", "dbo.Quizzes");
            DropIndex("dbo.WordSuiteQuiz", new[] { "WordSuiteId" });
            DropIndex("dbo.WordSuiteQuiz", new[] { "QuizId" });
            DropTable("dbo.WordSuiteQuiz");
            DropTable("dbo.Quizzes");
        }
    }
}
