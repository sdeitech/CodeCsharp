using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class AnswerValueConfiguration : IEntityTypeConfiguration<AnswerValue>
{
    public void Configure(EntityTypeBuilder<AnswerValue> builder)
    {
        builder.ToTable("AnswerValue");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("AnswerValueId")
            .HasColumnType("BIGINT");
        
        builder.Property(x => x.AnswerId)
            .IsRequired();
            
        builder.Property(x => x.SelectedOptionId);
        
        builder.Property(x => x.MatrixRowId);
        
        builder.Property(x => x.SelectedMatrixColumnId);
        
        builder.Property(x => x.TextValue)
            .HasColumnType("VARCHAR(MAX)");
            
        builder.Property(x => x.NumericValue)
            .HasPrecision(10, 2);
        
        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
        builder.Property(x => x.CreatedBy)
            .IsRequired();
            
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Answer)
            .WithMany(x => x.AnswerValues)
            .HasForeignKey(x => x.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.SelectedOption)
            .WithMany()
            .HasForeignKey(x => x.SelectedOptionId)
            .OnDelete(DeleteBehavior.NoAction);
            
        // Indexes
        builder.HasIndex(x => x.AnswerId);
    }
}
