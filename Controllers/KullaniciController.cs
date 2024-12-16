using Microsoft.AspNetCore.Mvc;
using kuafor.Models; // Kullanici ve AppDbContext için gerekli
using System.Linq;

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
}
