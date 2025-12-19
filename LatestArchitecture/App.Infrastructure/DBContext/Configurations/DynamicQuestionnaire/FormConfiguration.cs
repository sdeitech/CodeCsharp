using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.ToTable("Form");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("FormId");
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(x => x.Description)
            .HasMaxLength(1000);
            
        builder.Property(x => x.Instructions)
            .HasColumnType("varchar(max)");
            
        builder.Property(x => x.IsPublished)
            .HasDefaultValue(false);
            
        builder.Property(x => x.AllowResubmission)
            .HasDefaultValue(false);
            
        builder.Property(x => x.PublicKey)
            .HasMaxLength(50);
            
        builder.Property(x => x.PublicURL)
            .HasMaxLength(500);
            
        // Create unique index on PublicKey
        builder.HasIndex(x => x.PublicKey)
            .IsUnique()
            .HasFilter("[PublicKey] IS NOT NULL");
            
        // Audit Fields - these are inherited from BaseEntity
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(x => x.Pages)
            .WithOne(x => x.Form)
            .HasForeignKey(x => x.FormId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
