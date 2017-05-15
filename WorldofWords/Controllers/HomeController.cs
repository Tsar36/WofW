using System.Web.Mvc;

namespace WorldofWords.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return RedirectToRoute("AngularRoute");
        }
    }
}