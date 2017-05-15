namespace WorldOfWords.Infrastructure.Data.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentToWordMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Words", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Words", "Comment");
        }
    }
}
