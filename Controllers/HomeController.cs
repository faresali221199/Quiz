using Microsoft.AspNetCore.Mvc;

namespace Quiz.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
