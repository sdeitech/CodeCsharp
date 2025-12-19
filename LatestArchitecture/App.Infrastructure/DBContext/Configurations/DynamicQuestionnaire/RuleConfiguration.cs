using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class RuleConfiguration : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> builder)
    {
        // Table mapping
        builder.ToTable("Rule");

        // Primary key - use the inherited Id property and map to RuleId column
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("RuleId");

        // Properties
        builder.Property(r => r.FormId)
            .HasColumnName("FormId")
            .IsRequired();

        builder.Property(r => r.SourceQuestionId)
            .HasColumnName("SourceQuestionId")
            .IsRequired();

        builder.Property(r => r.TriggerOptionId)
            .HasColumnName("TriggerOptionId");

        builder.Property(r => r.Condition)
            .HasColumnName("Condition")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.MinValue)
            .HasColumnName("MinValue")
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.MaxValue)
            .HasColumnName("MaxValue")
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.ActionType)
            .HasColumnName("ActionType")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.TargetQuestionId)
            .HasColumnName("TargetQuestionId");

        builder.Property(r => r.TargetPageId)
            .HasColumnName("TargetPageId");

        builder.Property(r => r.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(false);

        // Audit fields - these are inherited from BaseEntity and map to the correct column names
        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(r => r.CreatedBy)
            .HasColumnName("CreatedBy");

        builder.Property(r => r.UpdatedBy)
            .HasColumnName("UpdatedBy");

        builder.Property(r => r.DeletedAt)
            .HasColumnName("DeletedAt");

        builder.Property(r => r.DeletedBy)
            .HasColumnName("DeletedBy");

        // Relationships
        builder.HasOne(r => r.Form)
            .WithMany(f => f.Rules)
            .HasForeignKey(r => r.FormId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Rule_Form");

        builder.HasOne(r => r.SourceQuestion)
            .WithMany()
            .HasForeignKey(r => r.SourceQuestionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Rule_SourceQuestion");

        builder.HasOne(r => r.TriggerOption)
            .WithMany()
            .HasForeignKey(r => r.TriggerOptionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Rule_TriggerOption");

        builder.HasOne(r => r.TargetQuestion)
            .WithMany()
            .HasForeignKey(r => r.TargetQuestionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Rule_TargetQuestion");

        builder.HasOne(r => r.TargetPage)
            .WithMany()
            .HasForeignKey(r => r.TargetPageId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Rule_TargetPage");

        // Indexes
        builder.HasIndex(r => r.FormId)
            .HasDatabaseName("IX_Rule_FormId");

        builder.HasIndex(r => r.SourceQuestionId)
            .HasDatabaseName("IX_Rule_SourceQuestionId");

        builder.HasIndex(r => new { r.FormId, r.IsDeleted })
            .HasDatabaseName("IX_Rule_FormId_IsDeleted");
    }
}
