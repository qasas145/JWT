using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    // public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=server name ;Database=database name;User Id= SA;Password=your password;TrustServerCertificate=True");
        base.OnConfiguring(optionsBuilder);
    }
}