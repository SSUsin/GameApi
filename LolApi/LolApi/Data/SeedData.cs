using LolApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LolApi.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LolApiContext(
                serviceProvider.GetRequiredService<DbContextOptions<LolApiContext>>()))
            {
                // Look for any champions.
                if (context.LolItem.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.LolItem.AddRange(
                    new LolItem
                    {
                        ChampionName = "Aatrox",
                        Description = "the Darkin Blade",
                        Health = "580 (+80 per level)",
                        HealthRegen = "5 (+0.25 per level)",
                        AttackDamage = "60 (+5 per level)",
                        AttackSpeed = "NaN (+2.5% per level)",
                        MovementSpeed = "345",
                        Armor = "44 (+3.25 per level)",
                        MagicResist = "32.1 (+1.25 per level)",
                        Abilities = "Deathbringer Stance, The Darkin Blade, Infernal Chains, Umbral Dash, World Ender",
                        Url = "https://ddragon.leagueoflegends.com/cdn/img/champion/splash/Aatrox_0.jpg",
                        Tags = "aatrox",
                        Uploaded = "20-11-18 4:20T18:25:43.511Z"
                    }


                );
                context.SaveChanges();
            }
        }
    }
}
