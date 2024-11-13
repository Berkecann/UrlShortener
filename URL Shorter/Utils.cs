using System.Text;
using URL_Shorter.Models;

namespace URL_Shorter
{
    public static class Utils
    {
        public static bool IsCodeAvailable(string code)
        {
            var count = Globals.db.GetDatabase("Short").GetCollection<ShortenerModel>("data")
                .Find(x => x.Code == code);
            if (count.CountDocuments() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GenerateID(int Length)
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(Length)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString();
            return id;
        }

        /// <summary>
        /// Increases `+1` click count for code
        /// </summary>
        /// <param name="code">short url code to increase</param>
        public static Task IncreaseClick(string code)
        {
            try
            {
                var documents = Globals.db.GetDatabase("Short").GetCollection<dynamic>("data")
                    .Find(Builders<dynamic>.Filter.Eq("Code", code));

                if (documents.CountDocuments() == 0)
                {
                    return Task.CompletedTask;
                }

                var data = documents.First();
                int clicks;
                if (((IDictionary<string, object>)data).ContainsKey("Clicks"))
                {
                    clicks = data.Clicks;
                }
                else
                {
                    clicks = 0;
                }

                var ub = Builders<dynamic>.Update
                    .Set("Clicks", ++clicks);
                Globals.db.GetDatabase("Short").GetCollection<dynamic>("data")
                    .UpdateMany(Builders<dynamic>.Filter.Eq("Code", code), ub);
            }
            catch (Exception)
            {
            }

            return Task.CompletedTask;
        }
    }
}
