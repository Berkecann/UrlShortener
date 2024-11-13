global using MongoDB.Driver;

namespace URL_Shorter
{
    public static class Globals
    {
        public const string APIKEY = "KEY";

        private const string DbConnectionString = "MongodbConnectionString";

        public static MongoClient db = new MongoClient(DbConnectionString);

        public const string DOMAIN = "http://localhost:5000";
    }
}
