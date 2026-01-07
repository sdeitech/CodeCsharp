using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submission");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("SubmissionId");
        
        builder.Property(x => x.FormId)
            .IsRequired();
            
        builder.Property(x => x.RespondentEmail)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(x => x.RespondentName)
            .HasMaxLength(255);
            
        builder.Property(x => x.SubmittedDate)
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(x => x.TotalScore)
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
        builder.HasOne(x => x.Form)
            .WithMany()
            .HasForeignKey(x => x.FormId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Answers)
            .WithOne(x => x.Submission)
            .HasForeignKey(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(x => x.FormId);
        builder.HasIndex(x => x.RespondentEmail);
        builder.HasIndex(x => new { x.FormId, x.SubmittedDate });
    }
}
