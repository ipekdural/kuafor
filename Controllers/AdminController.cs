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
    [Authorize(Policy = "Admin")]
    public IActionResult CalisanListesi()
    {
        var calisanlar = _context.Calisanlar
            .Include(c => c.CalisanHizmetler)
            .ThenInclude(ch => ch.Hizmet) // Hizmetleri dahil et
            .Include(c => c.UzmanlikHizmet) // Uzmanlık alanını dahil et
            .ToList();

        return View(calisanlar);
    }


    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult CalisanEkle()
    {
        ViewBag.Hizmetler = _context.Hizmetler.ToList(); // Tüm hizmetleri al
        return View();
    }
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult CalisanEkle(Calisan calisan, List<int> hizmetIds, int uzmanlikHizmetId)
    {
        if (ModelState.IsValid)
        {
            // Uzmanlık alanı, seçilen hizmetlerden biri olmalı
            if (!hizmetIds.Contains(uzmanlikHizmetId))
            {
                ModelState.AddModelError("", "Uzmanlık alanı, verilen hizmetlerden biri olmalıdır.");
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(calisan);
            }

            // Uzmanlık alanı atanıyor
            calisan.UzmanlikHizmetId = uzmanlikHizmetId;

            // Çalışanı ekle
            _context.Calisanlar.Add(calisan);
            _context.SaveChanges();

            // Çalışan ve hizmet ilişkilerini ekle
            foreach (var hizmetId in hizmetIds)
            {
                _context.CalisanHizmetler.Add(new CalisanHizmet
                {
                    CalisanId = calisan.CalisanId,
                    HizmetId = hizmetId
                });
            }

            _context.SaveChanges();

            // Çalışan listesine yönlendir
            return RedirectToAction("CalisanListesi");
        }

        ViewBag.Hizmetler = _context.Hizmetler.ToList();
        return View(calisan);
    }



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
    public IActionResult CalisanGuncelle(Calisan calisan, List<int> hizmetIds, int uzmanlikHizmetId)
    {
        if (ModelState.IsValid)
        {
            if (!hizmetIds.Contains(uzmanlikHizmetId))
            {
                ModelState.AddModelError("", "Uzmanlık alanı, verilen hizmetlerden biri olmalıdır.");
                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                return View(calisan);
            }

            calisan.UzmanlikHizmetId = uzmanlikHizmetId;

            _context.Calisanlar.Update(calisan);
            _context.SaveChanges();

            var eskiHizmetler = _context.CalisanHizmetler.Where(ch => ch.CalisanId == calisan.CalisanId);
            _context.CalisanHizmetler.RemoveRange(eskiHizmetler);
            _context.SaveChanges();

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
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult CalisanSil(int id)
    {
        var calisan = _context.Calisanlar
            .Include(c => c.CalisanHizmetler)
            .Include(c => c.Randevular) // Randevular ilişkisini dahil ettik
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
        {
            return NotFound();
        }

        // Randevuları işaretle
        foreach (var randevu in calisan.Randevular)
        {
            randevu.SilindiMi = true;
        }

        // Çalışanı sil
        _context.CalisanHizmetler.RemoveRange(calisan.CalisanHizmetler);
        _context.Calisanlar.Remove(calisan);
        _context.SaveChanges();

        return RedirectToAction("CalisanListesi");
    }

	[Authorize(Policy = "Admin")]
	[HttpPost]
	public IActionResult HizmetSil(int id)
	{
		var hizmet = _context.Hizmetler.FirstOrDefault(h => h.HizmetId == id);
		if (hizmet == null)
		{
			return NotFound();
		}

		// Bu hizmete bağlı çalışanların UzmanlikHizmetId alanını güncelle
		var bagliCalisanlar = _context.Calisanlar.Where(c => c.UzmanlikHizmetId == id).ToList();
		foreach (var calisan in bagliCalisanlar)
		{
			calisan.UzmanlikHizmetId = null; // Veya başka bir hizmet ID'si atanabilir
		}

		_context.Hizmetler.Remove(hizmet);
		_context.SaveChanges();

		return RedirectToAction("HizmetListesi");
	}

	[Authorize(Policy = "Admin")]
    public IActionResult RandevuListesi()
    {
        var onaylanmamisRandevular = _context.Randevular
            .Include(r => r.Calisan)
            .Include(r => r.Hizmet)
            .Include(r => r.Kullanici)
            .Where(r => r.OnayliMi == null) // Onay bekleyenler
            .ToList();
        var onaylanmisRandevular = _context.Randevular
         .Include(r => r.Calisan)
         .Include(r => r.Hizmet)
         .Include(r => r.Kullanici)
         .Where(r => r.OnayliMi == true) // Onaylanmış randevular
         .ToList();


        ViewBag.OnaylanmamisRandevular = onaylanmamisRandevular;
        return View(onaylanmisRandevular); // Onaylanmış randevular Model olarak gönderilir
    }



    // Hizmet Listesini Görüntüle
    [Authorize(Policy = "Admin")]
    public IActionResult HizmetListesi()
    {
        var hizmetler = _context.Hizmetler.ToList();
        return View(hizmetler);
    }

    // Hizmet Ekle (GET)
    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult HizmetEkle()
    {
        return View();
    }

    // Hizmet Ekleme
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult HizmetEkle(string HizmetAdi, int Sure, decimal Ucret)
    {
        if (ModelState.IsValid)
        {
            var hizmet = new Hizmet
            {
                HizmetAdi = HizmetAdi,
                Sure = TimeSpan.FromMinutes(Sure), // Dakikayı TimeSpan'e çevir
                Ucret = Ucret
            };

            _context.Hizmetler.Add(hizmet);
            _context.SaveChanges();

            return RedirectToAction("HizmetListesi");
        }

        return View();
    }

    // Hizmet Güncelle (GET)
    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult HizmetGuncelle(int id)
    {
        var hizmet = _context.Hizmetler.Find(id);
        if (hizmet == null)
        {
            return NotFound();
        }
        return View(hizmet);
    }

    // Hizmet Güncelleme
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult HizmetGuncelle(int HizmetId, string HizmetAdi, int Sure, decimal Ucret)
    {
        if (ModelState.IsValid)
        {
            var hizmet = _context.Hizmetler.FirstOrDefault(h => h.HizmetId == HizmetId);
            if (hizmet == null)
            {
                return NotFound();
            }

            hizmet.HizmetAdi = HizmetAdi;
            hizmet.Sure = TimeSpan.FromMinutes(Sure); // Dakikayı TimeSpan'e çevir
            hizmet.Ucret = Ucret;

            _context.Hizmetler.Update(hizmet);
            _context.SaveChanges();

            return RedirectToAction("HizmetListesi");
        }

        return View();
    }


    // Hizmet Sil
    //[Authorize(Policy = "Admin")]
    //[HttpPost]
    //public IActionResult HizmetSil(int id)
    //{
    //    var hizmet = _context.Hizmetler.Find(id);
    //    if (hizmet != null)
    //    {
    //        _context.Hizmetler.Remove(hizmet);
    //        _context.SaveChanges();
    //    }
    //    return RedirectToAction("HizmetListesi");
    //}

    // Kullanıcı Listesini JSON formatında döndürme
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

 
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult RandevuOnayla(int id)
    {
        var randevu = _context.Randevular.FirstOrDefault(r => r.RandevuId == id);
        if (randevu != null)
        {
            randevu.OnayliMi = true; // Randevu onaylanır
            _context.SaveChanges();
        }
        return RedirectToAction("RandevuListesi");
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult RandevuRed(int id)
    {
        var randevu = _context.Randevular.FirstOrDefault(r => r.RandevuId == id);
        if (randevu != null)
        {
            randevu.OnayliMi = false; // Randevu reddedilir
            _context.SaveChanges();
        }
        return RedirectToAction("RandevuListesi");
    }


}
