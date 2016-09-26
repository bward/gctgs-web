namespace GctgsWeb.Models
{
    public class BggDetails
    {
        public string ThumbnailUrl;
        public string Description;
        public float Rating;

        public static BggDetails Empty
            => new BggDetails {ThumbnailUrl = "/images/empty.jpg", Description = "", Rating = 0};
    }
}
