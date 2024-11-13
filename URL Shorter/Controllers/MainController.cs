using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URL_Shorter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System.Dynamic;

namespace URL_Shorter.Controllers
{
    [ApiController]
    public class MainController : Controller
    {
        private HttpContext Context;
        public MainController(IHttpContextAccessor accessor)
        {
            Context = accessor.HttpContext;
        }

        [Route("/{code}")]
        public object Main(string code)
        {
            var findURL = Globals.db.GetDatabase("Short").GetCollection<dynamic>("data")
                .Find(Builders<dynamic>.Filter.Eq("Code", code));
            if (findURL.CountDocuments() == 0)
            {
                Context.Response.StatusCode = 404;
                return View("Views/CodeNotFound.cshtml");
            }
            dynamic URL = findURL.First();
            Utils.IncreaseClick(code);

            if (URL.IsLimitedUse)
            {
                URL.LimitedUse--;
                var ub = Builders<ShortenerModel>.Update
                    .Set("LimitedUse", URL.LimitedUse);
                Globals.db.GetDatabase("Short").GetCollection<ShortenerModel>("data")
                    .UpdateOne(Builders<dynamic>.Filter.Eq("Code", URL.Code), ub);
                if (URL.LimitedUse <= 0)
                {
                    Globals.db.GetDatabase("Short").GetCollection<ShortenerModel>("data")
                        .DeleteOne(Builders<dynamic>.Filter.Eq("Code", URL.Code));
                }
            }

            return Redirect(URL.Destination_URL);
        }

        [Route("api/shorten")]
        [HttpPost]
        public object Shorten([FromHeader]string URL, [FromHeader]bool IsLimitedUse = false, [FromHeader]bool IsExpires = false, [FromHeader]int LimitedUse = 0, [FromHeader]string ExpiresAt = null)
        {
            if (Context.Request.Cookies["token"] != Globals.APIKEY)
            {
                return StatusCode(401, "Unauthorized");
            }

            Uri test;
            if (!Uri.TryCreate(URL, UriKind.Absolute, out test))
            {
                return StatusCode(400, "Invalid Url");
            }

            var code = Utils.GenerateID(6);
            var IsCodeAvailable = false;

            while (IsCodeAvailable == false)
            {
                if (Utils.IsCodeAvailable(code))
                {
                    IsCodeAvailable = true;
                    break;
                }
                code = Utils.GenerateID(6);
            }

            dynamic data = new ExpandoObject();
            data._id = ObjectId.GenerateNewId();
            data.Code = code;
            data.Destination_URL = URL;
            data.IsExpires = IsExpires;
            data.IsLimitedUse = IsLimitedUse;

            if (IsLimitedUse)
            {
                data.LimitedUse = LimitedUse;
            }
            if (IsExpires && ExpiresAt != null)
            {
                data.ExpiresAt = DateTime.Parse(ExpiresAt);
            }

            Globals.db.GetDatabase("Short").GetCollection<dynamic>("data")
                .InsertOne(data);

            return StatusCode(200, $"Url shortened, {Globals.DOMAIN}/{code}");
        }
    }
}
