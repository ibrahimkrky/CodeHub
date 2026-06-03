using CodeHub.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Snippet> Snippets { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<SnippetTag> SnippetTags { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionSnippet> CollectionSnippets { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Many-to-Many: Snippet <-> Tag
            builder.Entity<SnippetTag>().HasKey(st => new { st.SnippetId, st.TagId });
            builder.Entity<SnippetTag>()
                .HasOne(st => st.Snippet).WithMany(s => s.SnippetTags)
                .HasForeignKey(st => st.SnippetId);
            builder.Entity<SnippetTag>()
                .HasOne(st => st.Tag).WithMany(t => t.SnippetTags)
                .HasForeignKey(st => st.TagId);

            // Many-to-Many: Collection <-> Snippet
            builder.Entity<CollectionSnippet>().HasKey(cs => new { cs.CollectionId, cs.SnippetId });
            builder.Entity<CollectionSnippet>()
                .HasOne(cs => cs.Collection).WithMany(c => c.CollectionSnippets)
                .HasForeignKey(cs => cs.CollectionId);
            builder.Entity<CollectionSnippet>()
                .HasOne(cs => cs.Snippet).WithMany(s => s.CollectionSnippets)
                .HasForeignKey(cs => cs.SnippetId);

            // Snippet -> Author
            builder.Entity<Snippet>()
                .HasOne(s => s.Author).WithMany(u => u.Snippets)
                .HasForeignKey(s => s.AuthorId).OnDelete(DeleteBehavior.Restrict);

            // Collection -> Owner
            builder.Entity<Collection>()
                .HasOne(c => c.Owner).WithMany(u => u.Collections)
                .HasForeignKey(c => c.OwnerId).OnDelete(DeleteBehavior.Restrict);

            // Star
            builder.Entity<Star>()
                .HasOne(s => s.User).WithMany(u => u.Stars)
                .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Star>()
                .HasOne(s => s.Snippet).WithMany(sn => sn.Stars)
                .HasForeignKey(s => s.SnippetId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Star>().HasIndex(s => new { s.UserId, s.SnippetId }).IsUnique();

            // Review -> Author
            builder.Entity<Review>()
                .HasOne(r => r.Author).WithMany(u => u.Reviews)
                .HasForeignKey(r => r.AuthorId).OnDelete(DeleteBehavior.Restrict);

            // Comment
            builder.Entity<Comment>()
                .HasOne(c => c.Review).WithMany(r => r.Comments)
                .HasForeignKey(c => c.ReviewId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Comment>()
                .HasOne(c => c.Author).WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.Restrict);

            // Vote
            builder.Entity<Vote>()
                .HasOne(v => v.Comment).WithMany(c => c.Votes)
                .HasForeignKey(v => v.CommentId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Vote>()
                .HasOne(v => v.User).WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Vote>().HasIndex(v => new { v.UserId, v.CommentId }).IsUnique();
        }
    }
}
