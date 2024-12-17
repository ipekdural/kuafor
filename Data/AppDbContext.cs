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

        // Kullanıcılar tablosunun yapılandırılması
        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.HasKey(k => k.Id); // Birincil anahtar
            entity.Property(k => k.Isim)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(k => k.Soyisim)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(k => k.Email)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(k => k.Sifre)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Hizmetler tablosunun yapılandırılması
        modelBuilder.Entity<Hizmet>(entity =>
        {
            entity.HasKey(h => h.HizmetId); // Birincil anahtar
            entity.Property(h => h.HizmetAdi)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(h => h.Sure)
                .IsRequired();
            entity.Property(h => h.Ucret)
                .IsRequired();
        });

        // Çalışanlar tablosunun yapılandırılması
        modelBuilder.Entity<Calisan>(entity =>
        {
            entity.HasKey(c => c.CalisanId); // Birincil anahtar
            entity.Property(c => c.Ad)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(c => c.Soyad)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(c => c.UzmanlikAlanlari)
                .HasMaxLength(255);
        });

        // Çalışan ve Hizmet ilişkisini tanımlamak
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

        // Randevular tablosunun yapılandırılması
        modelBuilder.Entity<Randevu>(entity =>
        {
            entity.HasKey(r => r.RandevuId);
            entity.Property(r => r.RandevuTarihi)
                .IsRequired();
            entity.Property(r => r.OnayliMi)
                .IsRequired();
        });
    }
}
