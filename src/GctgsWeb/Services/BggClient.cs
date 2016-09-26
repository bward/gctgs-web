using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GctgsWeb.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GctgsWeb.Services
{
    public class BggClient
    {
        private readonly string _baseUrl = "http://www.boardgamegeek.com/xmlapi2/";
        private readonly IMemoryCache _memoryCache;

        public BggClient(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<BggDetails> GetDetails(string name)
        {
            BggDetails result;
            if (!_memoryCache.TryGetValue(name, out result))
            {
                var id = await GetId(name);
                if (id == 0) result = BggDetails.Empty;
                else
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(_baseUrl + "thing?stats=1&id=" + id);
                        var document =
                            XDocument.Parse(Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
                        var item = document.Root.Element("item");

                        var thumbnailUrl = item.Element("thumbnail").Value;
                        var description = item.Element("description").Value;
                        var rating = item.Element("statistics")
                            .Element("ratings")
                            .Element("bayesaverage")
                            .Attribute("value");

                        result = new BggDetails
                        {
                            ThumbnailUrl = thumbnailUrl,
                            Description = Uri.UnescapeDataString(description),
                            Rating = (float) rating
                        };
                    }
                _memoryCache.Set(name, result,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(365)));
            }
            else
            {
                Debug.WriteLine("from cache!");
            }

            return result;
        }

        private async Task<int> GetId(string name)
        {
            using (var client = new HttpClient())
            {
                Debug.WriteLine("search?exact=1&type=boardgame&query=" + Uri.EscapeDataString(name));
                var response = await client.GetAsync(_baseUrl + "search?exact=1&type=boardgame&query=" + Uri.EscapeDataString(name));
                var document = XDocument.Parse(Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
                var item =
                    document.Root.Elements("item").OrderByDescending(i => (int) i.Attribute("id")).FirstOrDefault();
                return item != null ? (int) item.Attribute("id") : 0;
            }
        }
    }
}
