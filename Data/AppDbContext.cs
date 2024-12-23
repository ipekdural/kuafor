using Microsoft.EntityFrameworkCore;
using kuafor.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tablo olarak tanımlanacak modeller
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

    // OnModelCreating metodu
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

        // Kullanıcılar tablosunun yapılandırılması
        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Isim).IsRequired().HasMaxLength(100);
            entity.Property(k => k.Soyisim).IsRequired().HasMaxLength(100);
            entity.Property(k => k.Email).IsRequired().HasMaxLength(150);
            entity.Property(k => k.Sifre).IsRequired().HasMaxLength(100);
        });

        // Hizmetler tablosunun yapılandırılması
        modelBuilder.Entity<Hizmet>(entity =>
        {
            entity.HasKey(h => h.HizmetId);
            entity.Property(h => h.HizmetAdi).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Sure).IsRequired();
            entity.Property(h => h.Ucret).IsRequired();
        });

        // Çalışanlar tablosunun yapılandırılması
        modelBuilder.Entity<Calisan>()
            .HasOne(c => c.UzmanlikHizmet)
            .WithMany()
            .HasForeignKey(c => c.UzmanlikHizmetId)
            .OnDelete(DeleteBehavior.SetNull);

        // Randevular tablosunun yapılandırılması
        modelBuilder.Entity<Randevu>(entity =>
        {
            entity.HasKey(r => r.RandevuId);

            entity.Property(r => r.RandevuTarihi).IsRequired();
            entity.Property(r => r.OnayliMi).HasDefaultValue(null);

            // Hizmet ile ilişki
            entity.HasOne(r => r.Hizmet)
                .WithMany()
                .HasForeignKey(r => r.HizmetId)
                .OnDelete(DeleteBehavior.Cascade); // Hizmet silinirse randevular silinsin

            //// Çalışan ile ilişki
            //entity.HasOne(r => r.Calisan)
            //    .WithMany()
            //    .HasForeignKey(r => r.CalisanId)
            //    .OnDelete(DeleteBehavior.Cascade); // Çalışan silinirse randevular silinsin

            // Kullanıcı ile ilişki
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