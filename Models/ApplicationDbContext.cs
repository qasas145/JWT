using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    // public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=JWT;User Id= SA;Password=Hamada1020;TrustServerCertificate=True");
        base.OnConfiguring(optionsBuilder);
    }
}