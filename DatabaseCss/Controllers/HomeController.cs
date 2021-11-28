using Microsoft.AspNetCore.Mvc;

namespace DatabaseCss.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}