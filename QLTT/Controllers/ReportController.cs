using Microsoft.AspNetCore.Mvc;

namespace QLTT.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
