using Microsoft.AspNetCore.Mvc;

public class AdminPanelController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
