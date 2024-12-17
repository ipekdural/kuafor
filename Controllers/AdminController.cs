using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kuafor.Models;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // Admin Giriş Sayfası
    [HttpGet]
    public IActionResult AdminGiris()
    {
        return View();
    }

    // Admin Giriş İşlemi
    [HttpPost]
    public IActionResult AdminGiris(string Email, string Sifre)
    {
        if (Email == "g211210026@sakarya.edu.tr" && Sifre == "sau")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email),
                new Claim("Admin", "true")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("AdminPanel");
        }

        ViewBag.HataMesaji = "Geçersiz kullanıcı adı veya şifre.";
        return View();
    }

    // Admin Paneli - Admin Girişi sonrası yönlendirilir
    [Authorize(Policy = "Admin")]
    public IActionResult AdminPanel()
    {
        return View();
    }

    // Admin Çıkışı
    [HttpPost]
    public IActionResult CikisYap()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("AdminGiris");
    }

    // Admin Paneli: Çalışan Listesini Görüntüle
    [Authorize(Policy = "Admin")]
    public IActionResult CalisanListesi()
    {
        var calisanlar = _context.Calisanlar
            .Include(c => c.CalisanHizmetler) // Çalışanların sunduğu hizmetler
            .ThenInclude(ch => ch.Hizmet) // Hizmet detaylarını al
            .ToList();

        return View(calisanlar);
    }

    // Admin Paneli: Çalışan Ekle
    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult CalisanEkle()
    {
        ViewBag.Hizmetler = _context.Hizmetler.ToList();  // Hizmetleri ekranda listele
        return View();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult CalisanEkle(Calisan calisan, List<int> hizmetIds)
    {
        if (ModelState.IsValid)
        {
            _context.Calisanlar.Add(calisan);
            _context.SaveChanges();

            // Çalışana hizmetleri ekle
            foreach (var hizmetId in hizmetIds)
            {
                _context.CalisanHizmetler.Add(new CalisanHizmet
                {
                    CalisanId = calisan.CalisanId,
                    HizmetId = hizmetId
                });
            }
            _context.SaveChanges();
            return RedirectToAction("CalisanListesi");
        }
        ViewBag.Hizmetler = _context.Hizmetler.ToList();
        return View(calisan);
    }

    // Admin Paneli: Çalışan Güncelle
    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult CalisanGuncelle(int id)
    {
        var calisan = _context.Calisanlar
            .Include(c => c.CalisanHizmetler)
            .ThenInclude(ch => ch.Hizmet)
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
        {
            return NotFound();
        }

        ViewBag.Hizmetler = _context.Hizmetler.ToList();
        return View(calisan);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult CalisanGuncelle(Calisan calisan, List<int> hizmetIds)
    {
        if (ModelState.IsValid)
        {
            _context.Calisanlar.Update(calisan);
            _context.SaveChanges();

            // Eski hizmetleri sil
            var eskiHizmetler = _context.CalisanHizmetler.Where(ch => ch.CalisanId == calisan.CalisanId);
            _context.CalisanHizmetler.RemoveRange(eskiHizmetler);
            _context.SaveChanges();

            // Yeni hizmetleri ekle
            foreach (var hizmetId in hizmetIds)
            {
                _context.CalisanHizmetler.Add(new CalisanHizmet
                {
                    CalisanId = calisan.CalisanId,
                    HizmetId = hizmetId
                });
            }

            _context.SaveChanges();
            return RedirectToAction("CalisanListesi");
        }
        ViewBag.Hizmetler = _context.Hizmetler.ToList();
        return View(calisan);
    }

    // Admin Paneli: Çalışan Sil
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult CalisanSil(int id)
    {
        var calisan = _context.Calisanlar
            .Include(c => c.CalisanHizmetler)
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan != null)
        {
            _context.CalisanHizmetler.RemoveRange(calisan.CalisanHizmetler); // Çalışanın hizmetlerini sil
            _context.Calisanlar.Remove(calisan); // Çalışanı sil
            _context.SaveChanges();
        }

        return RedirectToAction("CalisanListesi");
    }

    // Admin Paneli: Randevuları Görüntüle
    [Authorize(Policy = "Admin")]
    public IActionResult RandevuListesi()
    {
        var randevular = _context.Randevular
            .Include(r => r.Calisan)
            .Include(r => r.Hizmet)
            .ToList();

        return View(randevular);
    }

    // Admin Paneli: Hizmetleri Görüntüle
    [Authorize(Policy = "Admin")]
    public IActionResult HizmetListesi()
    {
        var hizmetler = _context.Hizmetler.ToList();
        return View(hizmetler);
    }

    // Kullanıcı Listesini JSON formatında döndürme (Örnek)
    [Authorize(Policy = "Admin")]
    public IActionResult KullaniciListesi()
    {
        var kullanicilar = _context.Kullanicilar
            .Select(k => new
            {
                k.Isim,
                k.Soyisim,
                k.Email
            })
            .ToList();

        return Json(kullanicilar);
    }
}
