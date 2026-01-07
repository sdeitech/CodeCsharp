using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.DynamicQuestionnaire;

public class MatrixColumnConfiguration : IEntityTypeConfiguration<MatrixColumn>
{
    public void Configure(EntityTypeBuilder<MatrixColumn> builder)
    {
        builder.ToTable("MatrixColumn");

        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("MatrixColumnId");
        
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.QuestionId)
            .IsRequired();

        builder.Property(e => e.ColumnLabel)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.DisplayOrder)
            .IsRequired();

        builder.Property(e => e.Score)
            .IsRequired();

        // Configure many-to-one relationship with Question
        builder.HasOne(mc => mc.Question)
            .WithMany(q => q.MatrixColumns)
            .HasForeignKey(mc => mc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.QuestionId)
            .HasDatabaseName("IX_MatrixColumns_QuestionId");

        builder.HasIndex(e => new { e.QuestionId, e.DisplayOrder })
            .IsUnique()
            .HasDatabaseName("IX_MatrixColumns_QuestionId_DisplayOrder");
    }
}
