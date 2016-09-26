using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GctgsWeb.Services;
using Microsoft.Extensions.Caching.Memory;

namespace GctgsWeb.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }

        [NotMapped]
        public BggDetails BggDetails { get; set; }

        public async Task<BggDetails> GetBggDetails(IMemoryCache memoryCache)
        {
            var client = new BggClient(memoryCache);
            return await client.GetDetails(Name);
        }
    }
}
