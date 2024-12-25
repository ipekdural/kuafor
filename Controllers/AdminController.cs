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


    [HttpGet]
    public IActionResult AdminGiris()
    {
        return View();
    }

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

    [Authorize(Policy = "Admin")]
    public IActionResult AdminPanel()
    {
        return View();
    }

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
            .ThenInclude(ch => ch.Hizmet) 
            .Include(c => c.UzmanlikHizmet) 
            .ToList();

        return View(calisanlar);
    }


    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult CalisanEkle()
    {
        ViewBag.Hizmetler = _context.Hizmetler.ToList(); 
        return View();
    }
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult CalisanEkle(Calisan calisan, List<int> hizmetIds, int uzmanlikHizmetId)
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

        
            _context.Calisanlar.Add(calisan);
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
            .Include(c => c.Randevular) 
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
        {
            return NotFound();
        }

        foreach (var randevu in calisan.Randevular)
        {
            randevu.SilindiMi = true;
        }

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


		var bagliCalisanlar = _context.Calisanlar.Where(c => c.UzmanlikHizmetId == id).ToList();
		foreach (var calisan in bagliCalisanlar)
		{
			calisan.UzmanlikHizmetId = null; 
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
            .Where(r => r.OnayliMi == null) 
            .ToList();
        var onaylanmisRandevular = _context.Randevular
         .Include(r => r.Calisan)
         .Include(r => r.Hizmet)
         .Include(r => r.Kullanici)
         .Where(r => r.OnayliMi == true) 
         .ToList();


        ViewBag.OnaylanmamisRandevular = onaylanmamisRandevular;
        return View(onaylanmisRandevular); 
    }



    [Authorize(Policy = "Admin")]
    public IActionResult HizmetListesi()
    {
        var hizmetler = _context.Hizmetler.ToList();
        return View(hizmetler);
    }


    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult HizmetEkle()
    {
        return View();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public IActionResult HizmetEkle(string HizmetAdi, int Sure, decimal Ucret)
    {
        if (ModelState.IsValid)
        {
            var hizmet = new Hizmet
            {
                HizmetAdi = HizmetAdi,
                Sure = TimeSpan.FromMinutes(Sure), 
                Ucret = Ucret
            };

            _context.Hizmetler.Add(hizmet);
            _context.SaveChanges();

            return RedirectToAction("HizmetListesi");
        }

        return View();
    }

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
            hizmet.Sure = TimeSpan.FromMinutes(Sure); 
            hizmet.Ucret = Ucret;

            _context.Hizmetler.Update(hizmet);
            _context.SaveChanges();

            return RedirectToAction("HizmetListesi");
        }

        return View();
    }



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
            randevu.OnayliMi = true; 
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
            randevu.OnayliMi = false; 
            _context.SaveChanges();
        }
        return RedirectToAction("RandevuListesi");
    }


}
