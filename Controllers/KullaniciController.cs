using Microsoft.AspNetCore.Mvc;
using kuafor.Models; // Kullanici ve AppDbContext için gerekli
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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
            // Kullanıcı giriş başarılı olduğunda
            // Kullanıcının email bilgisi oturumda tutulabilir
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, kullanici.Email),
            new Claim(ClaimTypes.Name, kullanici.Isim)
        };

            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(principal);

            // Giriş başarılı, randevular sayfasına yönlendir
            return RedirectToAction("AldigimRandevular", "Kullanici");
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
        var kullaniciEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == kullaniciEmail);

        if (kullanici == null)
        {
            return RedirectToAction("Giris", "Kullanici");
        }

        var randevular = _context.Randevular
           .Include(r => r.Calisan)
           .Include(r => r.Hizmet)
           .Where(r => r.KullaniciId == kullanici.Id) // Kullanıcıya ait tüm randevuları getir
           .ToList();


        ViewBag.Calisanlar = _context.Calisanlar
            .Include(c => c.CalisanHizmetler)
            .ThenInclude(ch => ch.Hizmet)
            .ToList();

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

        // Geçmiş bir tarihe randevu alınamaz
        if (randevuTarihi < DateTime.Now)
        {
            TempData["HataMesaji"] = "Geçmiş bir tarihe randevu alınamaz.";
            return RedirectToAction("AldigimRandevular");
        }

        // Hizmet süresini dakika cinsine çek
        var hizmet = _context.Hizmetler.FirstOrDefault(h => h.HizmetId == hizmetId);
        if (hizmet == null)
        {
            TempData["HataMesaji"] = "Geçersiz hizmet seçimi.";
            return RedirectToAction("AldigimRandevular");
        }
        var hizmetSuresiDakika = hizmet.Sure.TotalMinutes;

        // Çalışanın randevu çakışmasını kontrol et
        var calisanRandevusuVarMi = _context.Randevular
            .Where(r => r.CalisanId == calisanId)
            .Any(r =>
                r.RandevuTarihi <= randevuTarihi &&
                r.RandevuTarihi.AddMinutes(hizmetSuresiDakika) > randevuTarihi
            );

        if (calisanRandevusuVarMi)
        {
            TempData["HataMesaji"] = "Seçilen çalışanın bu saat aralığında başka bir randevusu var.";
            return RedirectToAction("AldigimRandevular");
        }

        // Kullanıcının randevu çakışmasını kontrol et
        var kullaniciRandevusuVarMi = _context.Randevular
            .Where(r => r.KullaniciId == kullanici.Id)
            .Any(r =>
                r.RandevuTarihi <= randevuTarihi &&
                r.RandevuTarihi.AddMinutes(hizmetSuresiDakika) > randevuTarihi
            );

        if (kullaniciRandevusuVarMi)
        {
            TempData["HataMesaji"] = "Bu saat aralığında zaten başka bir randevunuz var.";
            return RedirectToAction("AldigimRandevular");
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
