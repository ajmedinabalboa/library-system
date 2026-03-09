
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Infrastructure.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(a => a.Name);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasMany(a => a.BookAuthors)
            .WithOne(ba => ba.Author)
            .HasForeignKey(ba => ba.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
