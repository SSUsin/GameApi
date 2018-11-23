using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LolApi.Models
{
    public class LolItem
    {
        public int Id { get; set; }
        public string ChampionName { get; set; }
        public string Description { get; set; }
        public string Health { get; set; }
        public string HealthRegen { get; set; }
        public string AttackDamage { get; set; }
        public string AttackSpeed { get; set; }
        public string MovementSpeed { get; set; }
        public string Armor { get; set; }
        public string MagicResist { get; set; }
        public string Abilities { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }
        public string Uploaded { get; set; }
    }
}
