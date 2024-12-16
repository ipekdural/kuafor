using Microsoft.EntityFrameworkCore;
using kuafor.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Kullanıcılar tablosu
    public DbSet<Kullanici> Kullanicilar
    {
        get; set;
    }
}
