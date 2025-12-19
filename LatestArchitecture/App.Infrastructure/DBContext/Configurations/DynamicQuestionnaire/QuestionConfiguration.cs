using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Question");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("QuestionId");
        
        builder.Property(x => x.QuestionText)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(x => x.IsRequired)
            .HasDefaultValue(false);
            
        builder.Property(x => x.QuestionOrder)
            .IsRequired();
            
        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Page)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.PageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.QuestionType)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.QuestionTypeId);
            
        builder.HasMany(x => x.Options)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
