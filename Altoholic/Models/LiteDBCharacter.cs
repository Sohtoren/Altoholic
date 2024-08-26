using System.Collections.Generic;

namespace Altoholic.Models
{
    public class Quest
    {
        public int Id { get; set; }
        public bool Completed { get; set; }
    }
    // ReSharper disable once InconsistentNaming
    public class LiteDBCharacter
    {
        public ulong Id { get; init; } = 0;
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
        public Attributes? Attributes { get; set; }
        public PlayerCurrencies? Currencies { get; set; }
        public Jobs? Jobs { get; set; }
        public Profile? Profile { get; set; }
        public List<Quest> Quests { get; set; } = [];
        public List<Inventory> Inventory { get; set; } = [];
        public ArmoryGear? ArmoryInventory { get; set; } = null;
        public List<Inventory> Saddle { get; set; } = [];
        public List<Gear> Gear { get; set; } = [];
        public List<Retainer> Retainers { get; set; } = [];
        public List<uint> Minions { get; set; } = [];
        public List<uint> Mounts { get; set; } = [];
        public List<uint> TripleTriadCards { get; set; } = [];
        public List<uint> Emotes { get; set; } = [];
        public List<uint> Bardings { get; set; } = [];
        public List<uint> FramerKits { get; set; } = [];
        public List<uint> OrchestrionRolls { get; set; } = [];
        public List<uint> Ornaments { get; set; } = [];
        public List<uint> Glasses { get; set; } = [];
        public List<CurrenciesHistory> CurrenciesHistory { get; set; } = [];
        public List<BeastTribeRank> BeastReputations { get; set; } = [];
    }
}
