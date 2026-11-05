using Microsoft.AspNetCore.Mvc;

namespace ProjektAWiMDDD.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
