namespace WorldOfWords.Infrastructure.Data.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class partOfSpeechProd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PartsOfSpeech",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShortName = c.String(maxLength: 10),
                        Name = c.String(),
                        LanguageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId)
                .Index(t => t.LanguageId);
            
            AddColumn("dbo.Words", "PartOfSpeechId", c => c.Int());
            CreateIndex("dbo.Words", "PartOfSpeechId");
            AddForeignKey("dbo.Words", "PartOfSpeechId", "dbo.PartsOfSpeech", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PartsOfSpeech", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.Words", "PartOfSpeechId", "dbo.PartsOfSpeech");
            DropIndex("dbo.Words", new[] { "PartOfSpeechId" });
            DropIndex("dbo.PartsOfSpeech", new[] { "LanguageId" });
            DropColumn("dbo.Words", "PartOfSpeechId");
            DropTable("dbo.PartsOfSpeech");
        }
    }
}
