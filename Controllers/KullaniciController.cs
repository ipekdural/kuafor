using Microsoft.AspNetCore.Mvc;
using kuafor.Models; 
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

public class KullaniciController : Controller
{
    private readonly AppDbContext _context;

    public KullaniciController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Giris()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Giris(string Email, string Sifre)
    {
      
        var kullanici = _context.Kullanicilar
            .FirstOrDefault(k => k.Email == Email && k.Sifre == Sifre);

        if (kullanici != null)
        {

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, kullanici.Email),
            new Claim(ClaimTypes.Name, kullanici.Isim)
        };

            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(principal);

            return RedirectToAction("AldigimRandevular", "Kullanici");
        }

        ViewBag.HataMesaji = "Geçersiz e-posta veya şifre.";
        return View();
    }


    [HttpGet]
    public IActionResult Kayit()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Kayit(string Isim, string Soyisim, string Email, string Sifre)
    {

        var mevcutKullanici = _context.Kullanicilar
            .FirstOrDefault(k => k.Email == Email);

        if (mevcutKullanici != null)
        {

            ViewBag.HataMesaji = "Bu e-posta adresi zaten kayıtlı.";
            return View();
        }


        var yeniKullanici = new Kullanici
        {
            Isim = Isim,
            Soyisim = Soyisim,
            Email = Email,
            Sifre = Sifre
        };

        _context.Kullanicilar.Add(yeniKullanici);
        _context.SaveChanges();

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
            .Where(r => r.KullaniciId == kullanici.Id)
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

        var kullaniciEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == kullaniciEmail);

        if (kullanici == null)
        {
            return RedirectToAction("Giris", "Kullanici");
        }

        if (randevuTarihi < DateTime.Now)
        {
            TempData["HataMesaji"] = "Geçmiş bir tarihe randevu alınamaz.";
            return RedirectToAction("AldigimRandevular");
        }

        var hizmet = _context.Hizmetler.FirstOrDefault(h => h.HizmetId == hizmetId);
        if (hizmet == null)
        {
            TempData["HataMesaji"] = "Geçersiz hizmet seçimi.";
            return RedirectToAction("AldigimRandevular");
        }
        var hizmetSuresiDakika = hizmet.Sure.TotalMinutes;

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

        var yeniRandevu = new Randevu
        {
            KullaniciId = kullanici.Id,
            HizmetId = hizmetId,
            CalisanId = calisanId,
            RandevuTarihi = randevuTarihi,
            OnayliMi = null 
        };

        _context.Randevular.Add(yeniRandevu);
        _context.SaveChanges();

        return RedirectToAction("AldigimRandevular");
    }

    [Authorize]
    [HttpPost]
    public IActionResult CikisYap()
    {
        HttpContext.SignOutAsync(); 
        return RedirectToAction("Giris", "Kullanici"); 
    }
    [HttpGet]
    public IActionResult GetHizmetlerByCalisan(int calisanId)
    {
        var hizmetler = _context.CalisanHizmetler
            .Where(ch => ch.CalisanId == calisanId)
            .Select(ch => ch.Hizmet)
            .Distinct()
            .ToList();

        return Json(hizmetler);
    }




}
