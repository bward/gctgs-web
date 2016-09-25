using System;

namespace GctgsWeb.Models
{
    public class Request
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }

        public int RequesterId { get; set; }
        public User Requester { get; set; }

        public int BoardGameId { get; set; }
        public BoardGame BoardGame { get; set; }
    }
}
