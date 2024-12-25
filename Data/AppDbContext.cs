using Microsoft.EntityFrameworkCore;
using kuafor.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Kullanici> Kullanicilar
    {
        get; set;
    }
    public DbSet<Hizmet> Hizmetler
    {
        get; set;
    }
    public DbSet<Calisan> Calisanlar
    {
        get; set;
    }
    public DbSet<CalisanHizmet> CalisanHizmetler
    {
        get; set;
    }
    public DbSet<Randevu> Randevular
    {
        get; set;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CalisanHizmet>()
            .HasKey(ch => new { ch.CalisanId, ch.HizmetId });

        modelBuilder.Entity<CalisanHizmet>()
            .HasOne(ch => ch.Calisan)
            .WithMany(c => c.CalisanHizmetler)
            .HasForeignKey(ch => ch.CalisanId);

        modelBuilder.Entity<CalisanHizmet>()
            .HasOne(ch => ch.Hizmet)
            .WithMany()
            .HasForeignKey(ch => ch.HizmetId);

        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Isim).IsRequired().HasMaxLength(100);
            entity.Property(k => k.Soyisim).IsRequired().HasMaxLength(100);
            entity.Property(k => k.Email).IsRequired().HasMaxLength(150);
            entity.Property(k => k.Sifre).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Hizmet>(entity =>
        {
            entity.HasKey(h => h.HizmetId);
            entity.Property(h => h.HizmetAdi).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Sure).IsRequired();
            entity.Property(h => h.Ucret).IsRequired();
        });

        modelBuilder.Entity<Calisan>()
            .HasOne(c => c.UzmanlikHizmet)
            .WithMany()
            .HasForeignKey(c => c.UzmanlikHizmetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Randevu>(entity =>
        {
            entity.HasKey(r => r.RandevuId);

            entity.Property(r => r.RandevuTarihi).IsRequired();
            entity.Property(r => r.OnayliMi).HasDefaultValue(null);

            entity.HasOne(r => r.Hizmet)
                .WithMany()
                .HasForeignKey(r => r.HizmetId)
                .OnDelete(DeleteBehavior.Cascade); 

            entity.HasOne(r => r.Kullanici)
                .WithMany()
                .HasForeignKey(r => r.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Calisan)
        .WithMany(c => c.Randevular)
        .HasForeignKey(r => r.CalisanId)
        .OnDelete(DeleteBehavior.Cascade);
        });
    }
}