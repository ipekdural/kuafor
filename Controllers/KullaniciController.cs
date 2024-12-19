using Microsoft.AspNetCore.Mvc;
using kuafor.Models; // Kullanici ve AppDbContext için gerekli
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class KullaniciController : Controller
{
    private readonly AppDbContext _context;

    // Dependency Injection ile AppDbContext alınıyor
    public KullaniciController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Kullanici/Giris
    [HttpGet]
    public IActionResult Giris()
    {
        return View();
    }

    // POST: Kullanici/Giris
    [HttpPost]
    public IActionResult Giris(string Email, string Sifre)
    {
        // Veritabanında kullanıcı kontrolü
        var kullanici = _context.Kullanicilar
            .FirstOrDefault(k => k.Email == Email && k.Sifre == Sifre);

        if (kullanici != null)
        {
            // Giriş başarılı, anasayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }

        // Giriş başarısızsa hata mesajı gönder
        ViewBag.HataMesaji = "Geçersiz e-posta veya şifre.";
        return View();
    }

    // GET: Kullanici/Kayit
    [HttpGet]
    public IActionResult Kayit()
    {
        return View();
    }

    // POST: Kullanici/Kayit
    [HttpPost]
    public IActionResult Kayit(string Isim, string Soyisim, string Email, string Sifre)
    {
        // Kullanıcının daha önce kayıtlı olup olmadığını kontrol et
        var mevcutKullanici = _context.Kullanicilar
            .FirstOrDefault(k => k.Email == Email);

        if (mevcutKullanici != null)
        {
            // E-posta zaten kayıtlıysa hata mesajı göster
            ViewBag.HataMesaji = "Bu e-posta adresi zaten kayıtlı.";
            return View();
        }

        // Yeni kullanıcı ekle
        var yeniKullanici = new Kullanici
        {
            Isim = Isim,
            Soyisim = Soyisim,
            Email = Email,
            Sifre = Sifre
        };

        _context.Kullanicilar.Add(yeniKullanici);
        _context.SaveChanges();

        // Kayıt başarılıysa giriş sayfasına yönlendir
        return RedirectToAction("Giris", "Kullanici");
    }
    [Authorize]
    public IActionResult AldigimRandevular()
    {
        // Oturumdaki kullanıcıyı al
        var kullaniciEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == kullaniciEmail);

        if (kullanici == null)
        {
            return RedirectToAction("Giris", "Kullanici");
        }

        // Kullanıcının aldığı randevular
        var randevular = _context.Randevular
            .Include(r => r.Calisan)
            .Include(r => r.Hizmet)
            .Where(r => r.KullaniciId == kullanici.Id)
            .ToList();

        ViewBag.Calisanlar = _context.Calisanlar.Include(c => c.CalisanHizmetler).ThenInclude(ch => ch.Hizmet).ToList();
        return View(randevular);
    }

    [Authorize]
    [HttpPost]
    public IActionResult RandevuAl(int hizmetId, int calisanId, DateTime randevuTarihi)
    {
        // Oturumdaki kullanıcıyı al
        var kullaniciEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == kullaniciEmail);

        if (kullanici == null)
        {
            return RedirectToAction("Giris", "Kullanici");
        }

        // Yeni randevu ekleme
        var yeniRandevu = new Randevu
        {
            KullaniciId = kullanici.Id,
            HizmetId = hizmetId,
            CalisanId = calisanId,
            RandevuTarihi = randevuTarihi,
            OnayliMi = false // Başlangıçta onaysız
        };

        _context.Randevular.Add(yeniRandevu);
        _context.SaveChanges();

        return RedirectToAction("AldigimRandevular");
    }


}
