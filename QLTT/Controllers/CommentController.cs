using Microsoft.AspNetCore.Mvc;

namespace QLTT.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
