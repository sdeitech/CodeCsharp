using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable("Option");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("OptionId");
        
        builder.Property(x => x.OptionText)
            .HasMaxLength(500);
            
        builder.Property(x => x.ImageUrl)
            .HasMaxLength(2048);
            
        builder.Property(x => x.DisplayOrder)
            .IsRequired();
            
        builder.Property(x => x.Score)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);
            
        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Question)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
