using Microsoft.AspNetCore.Mvc;

namespace URL_Shorter.Views
{
    [Route("/")]
    public class IndexController : Controller
    {
        private static HttpContext Context;

        public IndexController(IHttpContextAccessor accessor)
        {
            Context = accessor.HttpContext;
        }

        [HttpGet]
        public object Index()
        {
            return View("Views/Index.cshtml");
        }
    }
}
