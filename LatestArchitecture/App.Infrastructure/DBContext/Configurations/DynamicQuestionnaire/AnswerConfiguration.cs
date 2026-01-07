using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answer");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("AnswerId");
        
        builder.Property(x => x.SubmissionId)
            .IsRequired();
            
        builder.Property(x => x.QuestionId)
            .IsRequired();
            
        builder.Property(x => x.Score)
            .HasPrecision(10, 2)
            .HasDefaultValue(0);
        
        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
        builder.Property(x => x.CreatedBy)
            .IsRequired();
            
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Submission)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.HasMany(x => x.AnswerValues)
            .WithOne(x => x.Answer)
            .HasForeignKey(x => x.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(x => x.SubmissionId);
        builder.HasIndex(x => x.QuestionId);
    }
}
