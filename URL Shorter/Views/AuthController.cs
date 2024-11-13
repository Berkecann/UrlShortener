using Microsoft.AspNetCore.Mvc;

namespace URL_Shorter.Views
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private static HttpContext Context;
        public AuthController(IHttpContextAccessor accessor)
        {
            Context = accessor.HttpContext;
        }

        [HttpGet]
        public object Index()
        {
            return View("Views/Auth.cshtml");
        }

        [Route("login/{code}")]
        [HttpGet]
        public object Login(string code)
        {
            if (code != Globals.APIKEY)
                return BadRequest(new { statuscode = 400, message = "Invalid key" });
            else
            {
                Context.Response.Cookies.Append("token", Globals.APIKEY);
                return Ok("Valid Key");
            }
        }
    }
}
