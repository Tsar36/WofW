using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Infrastructure.Data.EF.Configurations
{
    public class TicketConfiguration : EntityTypeConfiguration<Ticket>
    {
        public TicketConfiguration()
        {
            //Primary Key
            HasKey<int>(t => t.TicketId);

            //Properties
            ToTable("Tickets");
            Property(t => t.TicketId).HasColumnName("Id");
            Property(t => t.Subject).HasColumnName("Subject");
            Property(t => t.ReviewStatus).HasColumnName("ReviewStatus");
            Property(t => t.OwnerId).HasColumnName("OwnerId");
            Property(t => t.OpenDate).HasColumnName("OpenDate");
            Property(t => t.CloseDate).HasColumnName("CloseDate");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.GroupId).HasColumnName("GroupId");

            //Table & Column Mappings
            HasRequired(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey<int>(t => t.OwnerId);
                //.WillCascadeOnDelete(true);
            HasOptional(t => t.Group)
                .WithMany(u => u.Tickets)
                .HasForeignKey<int?>(t => t.GroupId)
                .WillCascadeOnDelete(true);
        }
    }
}
