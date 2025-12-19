using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Page");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("PageId");
        
        builder.Property(x => x.Title)
            .HasMaxLength(255);
            
        builder.Property(x => x.Description)
            .HasMaxLength(1000);
            
        builder.Property(x => x.PageOrder)
            .IsRequired();
            
        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Form)
            .WithMany(x => x.Pages)
            .HasForeignKey(x => x.FormId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Questions)
            .WithOne(x => x.Page)
            .HasForeignKey(x => x.PageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
