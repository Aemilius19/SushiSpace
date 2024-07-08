using Microsoft.AspNetCore.Mvc;

namespace SushiSpace.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
