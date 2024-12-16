using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    // GET: Admin/Giris
    [HttpGet]
    public IActionResult Giris()
    {
        return View();
    }

    // POST: Admin/Giris
    [HttpPost]
    public IActionResult Giris(string Email, string Sifre)
    {
        // Örnek doğrulama: Sabit e-posta ve şifre
        if (Email == "admin@ornek.com" && Sifre == "admin123")
        {
            // Giriş başarılı, admin paneline yönlendir
            return RedirectToAction("Index", "AdminPanel");
        }

        // Giriş başarısızsa hata mesajı göster
        ViewBag.HataMesaji = "Geçersiz e-posta veya şifre.";
        return View();
    }
}
