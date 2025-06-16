using Microsoft.AspNetCore.Mvc;

namespace QLTT.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
