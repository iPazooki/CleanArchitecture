namespace Ca.Services.Caching
{
    public class CacheOptions
    {
        public const string Cache = "Cache";

        public int SlidingExpirationSec { get; set; }

        public int AbsoluteExpirationSec { get; set; }
    }
}