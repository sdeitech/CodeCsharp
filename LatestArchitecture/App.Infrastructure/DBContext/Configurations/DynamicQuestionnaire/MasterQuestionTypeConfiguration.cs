using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.DynamicQuestionnaire;

public class MasterQuestionTypeConfiguration : IEntityTypeConfiguration<MasterQuestionType>
{
    public void Configure(EntityTypeBuilder<MasterQuestionType> builder)
    {
        builder.ToTable("MasterQuestionType");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("QuestionTypeId");
        
        builder.Property(x => x.TypeName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
            
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => x.TypeName).IsUnique();
    }
}
