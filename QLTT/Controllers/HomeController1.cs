using Microsoft.AspNetCore.Mvc;

namespace QLTT.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
