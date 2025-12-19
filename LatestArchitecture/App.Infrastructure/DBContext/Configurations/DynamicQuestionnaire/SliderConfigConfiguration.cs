using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.DynamicQuestionnaire;

public class SliderConfigConfiguration : IEntityTypeConfiguration<SliderConfig>
{
    public void Configure(EntityTypeBuilder<SliderConfig> builder)
    {
        builder.ToTable("SliderConfig");

        
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("SliderConfigId");

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.QuestionId)
            .IsRequired();

        builder.Property(e => e.MinValue)
            .IsRequired();

        builder.Property(e => e.MaxValue)
            .IsRequired();

        builder.Property(e => e.StepValue)
            .IsRequired();

        builder.Property(e => e.MinLabel)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.MaxLabel)
            .HasMaxLength(100)
            .IsRequired(false);

        // Configure one-to-one relationship with Question
        builder.HasOne(sc => sc.Question)
            .WithOne(q => q.SliderConfig)
            .HasForeignKey<SliderConfig>(sc => sc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.QuestionId)
            .IsUnique()
            .HasDatabaseName("IX_SliderConfigs_QuestionId");
    }
}
