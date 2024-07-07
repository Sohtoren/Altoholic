namespace Altoholic.Models
{
    public class Job
    {
        public int Level { get; set; } = 0;
        public int Exp { get; set; } = 0;
    }

    public class Jobs
    {
        public Job Adventurer { get; set; } = new();
        //  Tank
        public Job Paladin { get; set; } = new();
        public Job Gladiator { get; set; } = new();
        public Job Warrior { get; set; } = new();
        public Job Marauder { get; set; } = new();
        public Job DarkKnight { get; set; } = new();
        public Job Gunbreaker { get; set; } = new();
        //  Healer
        public Job WhiteMage { get; set; } = new();
        public Job Conjurer { get; set; } = new();
        public Job Scholar { get; set; } = new();
        public Job Astrologian { get; set; } = new();
        public Job Sage { get; set; } = new();
        //  Damage
        // Melee
        public Job Monk { get; set; } = new();
        public Job Pugilist { get; set; } = new();
        public Job Dragoon { get; set; } = new();
        public Job Lancer { get; set; } = new();
        public Job Ninja { get; set; } = new();
        public Job Rogue { get; set; } = new();
        public Job Samurai { get; set; } = new();
        public Job Reaper { get; set; } = new();
        public Job Viper { get; set; } = new();
        // Physical Ranged
        public Job Bard { get; set; } = new();
        public Job Archer { get; set; } = new();
        public Job Machinist { get; set; } = new();
        public Job Dancer { get; set; } = new();
        // Magical Ranged
        public Job BlackMage { get; set; } = new();
        public Job Thaumaturge { get; set; } = new();
        public Job Summoner { get; set; } = new();
        public Job Arcanist { get; set; } = new();
        public Job RedMage { get; set; } = new();
        public Job Pictomancer { get; set; } = new();
        public Job BlueMage { get; set; } = new();
        //  Crafting
        public Job Carpenter { get; set; } = new();
        public Job Blacksmith { get; set; } = new();
        public Job Armorer { get; set; } = new();
        public Job Goldsmith { get; set; } = new();
        public Job Leatherworker { get; set; } = new();
        public Job Weaver { get; set; } = new();
        public Job Alchemist { get; set; } = new();
        public Job Culinarian { get; set; } = new();
        //  Gathering
        public Job Miner { get; set; } = new();
        public Job Botanist { get; set; } = new();
        public Job Fisher { get; set; } = new();
    }
}
