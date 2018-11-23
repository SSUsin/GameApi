using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LolApi.Models
{
    public class LolApiContext : DbContext
    {
        public LolApiContext (DbContextOptions<LolApiContext> options)
            : base(options)
        {
        }

        public DbSet<LolApi.Models.LolItem> LolItem { get; set; }
    }
}
