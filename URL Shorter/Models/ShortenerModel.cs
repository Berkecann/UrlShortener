using MongoDB.Bson;

namespace URL_Shorter.Models
{
    public class ShortenerModel
    {
        public ObjectId _id { get; set; }
        public string Code { get; set; }
        public string Destination_URL { get; set; }
        public bool IsLimitedUse { get; set; }
        public int LimitedUse { get; set; }
        public bool IsExpires { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
