using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GctgsWeb.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Crsid { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public bool Admin { get; set; }
        public string FcmToken { get; set; }

        public string Email => Crsid + "@cam.ac.uk";
    }
}
