using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
