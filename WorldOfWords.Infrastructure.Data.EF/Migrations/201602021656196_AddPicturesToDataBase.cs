namespace WorldOfWords.Infrastructure.Data.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPicturesToDataBase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.Binary(nullable: false),
                        WordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Words", t => t.WordId)
                .Index(t => t.WordId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pictures", "WordId", "dbo.Words");
            DropIndex("dbo.Pictures", new[] { "WordId" });
            DropTable("dbo.Pictures");
        }
    }
}
