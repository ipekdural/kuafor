﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace kuafor.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("kuafor.Models.Calisan", b =>
                {
                    b.Property<int>("CalisanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CalisanId"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UzmanlikAlanlari")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("CalisanId");

                    b.ToTable("Calisanlar");
                });

            modelBuilder.Entity("kuafor.Models.CalisanHizmet", b =>
                {
                    b.Property<int>("CalisanId")
                        .HasColumnType("int");

                    b.Property<int>("HizmetId")
                        .HasColumnType("int");

                    b.HasKey("CalisanId", "HizmetId");

                    b.HasIndex("HizmetId");

                    b.ToTable("CalisanHizmetler");
                });

            modelBuilder.Entity("kuafor.Models.Hizmet", b =>
                {
                    b.Property<int>("HizmetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HizmetId"));

                    b.Property<string>("HizmetAdi")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<TimeSpan>("Sure")
                        .HasColumnType("time");

                    b.Property<decimal>("Ucret")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("HizmetId");

                    b.ToTable("Hizmetler");
                });

            modelBuilder.Entity("kuafor.Models.Kullanici", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Isim")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Sifre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Soyisim")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Kullanicilar");
                });

            modelBuilder.Entity("kuafor.Models.Randevu", b =>
                {
                    b.Property<int>("RandevuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RandevuId"));

                    b.Property<int>("CalisanId")
                        .HasColumnType("int");

                    b.Property<int>("HizmetId")
                        .HasColumnType("int");

                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<bool>("OnayliMi")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RandevuTarihi")
                        .HasColumnType("datetime2");

                    b.HasKey("RandevuId");

                    b.HasIndex("CalisanId");

                    b.HasIndex("HizmetId");

                    b.HasIndex("KullaniciId");

                    b.ToTable("Randevular");
                });

            modelBuilder.Entity("kuafor.Models.CalisanHizmet", b =>
                {
                    b.HasOne("kuafor.Models.Calisan", "Calisan")
                        .WithMany("CalisanHizmetler")
                        .HasForeignKey("CalisanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kuafor.Models.Hizmet", "Hizmet")
                        .WithMany()
                        .HasForeignKey("HizmetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Calisan");

                    b.Navigation("Hizmet");
                });

            modelBuilder.Entity("kuafor.Models.Randevu", b =>
                {
                    b.HasOne("kuafor.Models.Calisan", "Calisan")
                        .WithMany()
                        .HasForeignKey("CalisanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kuafor.Models.Hizmet", "Hizmet")
                        .WithMany()
                        .HasForeignKey("HizmetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("kuafor.Models.Kullanici", "Kullanici")
                        .WithMany()
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Calisan");

                    b.Navigation("Hizmet");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("kuafor.Models.Calisan", b =>
                {
                    b.Navigation("CalisanHizmetler");
                });
#pragma warning restore 612, 618
        }
    }
}
