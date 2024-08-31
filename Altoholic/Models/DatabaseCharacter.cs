namespace TestNewDB.Models
{
    public class DatabaseCharacter
    {
        public ulong CharacterId { get; init; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string HomeWorld { get; set; } = string.Empty;
        public string CurrentWorld { get; set; } = string.Empty;
        public string Datacenter { get; set; } = string.Empty;
        public string CurrentDatacenter { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string CurrentRegion { get; set; } = string.Empty;
        public bool IsSprout { get; set; } = false;
        public bool IsBattleMentor { get; set; } = false;
        public bool IsTradeMentor { get; set; } = false;
        public bool IsReturner { get; set; } = false;
        public uint LastJob { get; set; } = 0;
        public int LastJobLevel { get; set; } = 0;

        // ReSharper disable once InconsistentNaming
        public string FCTag { get; set; } = string.Empty;
        public string FreeCompany { get; set; } = string.Empty;
        public long LastOnline { get; set; } = 0;
        public uint PlayTime { get; set; } = 0;
        public long LastPlayTimeUpdate { get; set; } = 0;
        public bool HasPremiumSaddlebag { get; set; } = false;
        public short PlayerCommendations { get; set; } = 0;
        public string Attributes { get; set; } = string.Empty;
        public string Currencies { get; set; } = string.Empty;
        public string Jobs { get; set; } = string.Empty;
        public string Profile { get; set; } = string.Empty;
        public string Quests { get; set; } = string.Empty;
        public string Inventory { get; set; } = string.Empty;
        public string ArmoryInventory { get; set; } = string.Empty;
        public string Saddle { get; set; } = string.Empty;
        public string Gear { get; set; } = string.Empty;
        public string Retainers { get; set; } = string.Empty;
        public string Minions { get; set; } = string.Empty;
        public string Mounts { get; set; } = string.Empty;
        public string TripleTriadCards { get; set; } = string.Empty;
        public string Emotes { get; set; } = string.Empty;
        public string Bardings { get; set; } = string.Empty;
        public string FramerKits { get; set; } = string.Empty;
        public string OrchestrionRolls { get; set; } = string.Empty;
        public string Ornaments { get; set; } = string.Empty;
        public string Glasses { get; set; } = string.Empty;
        public string BeastReputations { get; set; } = string.Empty;
    }
}