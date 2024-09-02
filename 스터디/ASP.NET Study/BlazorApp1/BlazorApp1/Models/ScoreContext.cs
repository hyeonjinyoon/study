using Microsoft.EntityFrameworkCore;
namespace BlazorApp1.Models;

public class ScoreContext : DbContext
{
    public DbSet<ScoreItem> Scores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("server=localhost;database=study_asp;user=test;password=test1234!");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ScoreItem>(entity =>
        {
            entity.HasKey(e => e.user);
            entity.Property(e => e.id).IsRequired();
        });
    }
}
