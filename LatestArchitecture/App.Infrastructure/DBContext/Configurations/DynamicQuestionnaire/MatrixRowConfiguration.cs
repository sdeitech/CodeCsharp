using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.DynamicQuestionnaire;

public class MatrixRowConfiguration : IEntityTypeConfiguration<MatrixRow>
{
    public void Configure(EntityTypeBuilder<MatrixRow> builder)
    {
        builder.ToTable("MatrixRow");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("MatrixRowId");

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.QuestionId)
            .IsRequired();

        builder.Property(e => e.RowLabel)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.DisplayOrder)
            .IsRequired();

        // Configure many-to-one relationship with Question
        builder.HasOne(mr => mr.Question)
            .WithMany(q => q.MatrixRows)
            .HasForeignKey(mr => mr.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.QuestionId)
            .HasDatabaseName("IX_MatrixRow_QuestionId");

        builder.HasIndex(e => new { e.QuestionId, e.DisplayOrder })
            .IsUnique()
            .HasDatabaseName("IX_MatrixRow_QuestionId_DisplayOrder");
    }
}
