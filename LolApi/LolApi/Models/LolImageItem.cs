using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LolApi.Models
{
    public class LolImageItem
    {
        public string ChampionName { get; set; }
        public string Tags { get; set; }
        public IFormFile Image { get; set; }
    }
}
