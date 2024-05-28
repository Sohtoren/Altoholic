using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altoholic.Models
{
    public class Job
    {
        public int Level { get; set; } = 0;
        public int Exp { get; set; } = 0;
    }

    public class Jobs
    {
        public Job Adventurer { get; set; } = null!;
        //  Tank
        public Job Paladin { get; set; } = null!;
        public Job Gladiator { get; set; } = null!;
        public Job Warrior { get; set; } = null!;
        public Job Marauder { get; set; } = null!;
        public Job DarkKnight { get; set; } = null!;
        public Job Gunbreaker { get; set; } = null!;
        //  Healer
        public Job WhiteMage { get; set; } = null!;
        public Job Conjurer { get; set; } = null!;
        public Job Scholar { get; set; } = null!;
        public Job Astrologian { get; set; } = null!;
        public Job Sage { get; set; } = null!;
        //  Damage
        // Melee
        public Job Monk { get; set; } = null!;
        public Job Pugilist { get; set; } = null!;
        public Job Dragoon { get; set; } = null!;
        public Job Lancer { get; set; } = null!;
        public Job Ninja { get; set; } = null!;
        public Job Rogue { get; set; } = null!;
        public Job Samurai { get; set; } = null!;
        public Job Reaper { get; set; } = null!;
        // Physical Ranged
        public Job Bard { get; set; } = null!;
        public Job Archer { get; set; } = null!;
        public Job Machinist { get; set; } = null!;
        public Job Dancer { get; set; } = null!;
        // Magical Ranged
        public Job BlackMage { get; set; } = null!;
        public Job Thaumaturge { get; set; } = null!;
        public Job Summoner { get; set; } = null!;
        public Job Arcanist { get; set; } = null!;
        public Job RedMage { get; set; } = null!;
        public Job BlueMage { get; set; } = null!;
        //  Crafting
        public Job Carpenter { get; set; } = null!;
        public Job Blacksmith { get; set; } = null!;
        public Job Armorer { get; set; } = null!;
        public Job Goldsmith { get; set; } = null!;
        public Job Leatherworker { get; set; } = null!;
        public Job Weaver { get; set; } = null!;
        public Job Alchemist { get; set; } = null!;
        public Job Culinarian { get; set; } = null!;
        //  Gathering
        public Job Miner { get; set; } = null!;
        public Job Botanist { get; set; } = null!;
        public Job Fisher { get; set; } = null!;
    }
}
