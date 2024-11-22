using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Altoholic.Models
{
    public class Character
    {
        public ulong CharacterId { get; init; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string HomeWorld { get; set; } = string.Empty;
        public string CurrentWorld { get; set; } = string.Empty;
        public string Datacenter {  get; set; } = string.Empty;
        public string CurrentDatacenter {  get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public string CurrentRegion {  get; set; } = string.Empty;
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
        public long LastPlayTimeUpdate {  get; set; } = 0;
        public bool HasPremiumSaddlebag { get; set; } = false;
        public short PlayerCommendations { get; set; } = 0;
        public ushort[] CurrentFacewear { get; set; } = [0,0];
        public ushort CurrentOrnament { get; set; } = 0;
        public Attributes? Attributes { get; set; }
        public PlayerCurrencies? Currencies { get; set; }
        public Jobs? Jobs { get; set; }
        public Profile? Profile { get; set; }
        public HashSet<int> Quests { get; set; } = [];
        public List<Inventory> Inventory { get; set; } = [];
        public ArmoryGear? ArmoryInventory { get; set; } = null;
        public List<Inventory> Saddle { get; set; } = [];
        public List<Gear> Gear { get; set; } = [];
        public List<Retainer> Retainers { get; set; } = [];
        public HashSet<uint> Minions { get; set; } = [];
        public HashSet<uint> Mounts { get; set; } = [];
        public HashSet<uint> TripleTriadCards { get; set; } = [];
        public HashSet<uint> Emotes { get; set; } = [];
        public HashSet<uint> Bardings { get; set; } = [];
        public HashSet<uint> FramerKits { get; set; } = [];
        public HashSet<uint> OrchestrionRolls { get; set; } = [];
        public HashSet<uint> Ornaments { get; set; } = [];
        public HashSet<uint> Glasses { get; set; } = [];
        public List<CurrenciesHistory> CurrenciesHistory { get; set; } = [];
        public List<BeastTribeRank> BeastReputations { get; set; } = [];
        public HashSet<uint> Duties { get; set; } = [];
        public HashSet<uint> DutiesUnlocked { get; set; } = [];
        public List<Housing> Houses { get; set; } = [];

        public bool HasAnyLevelJob(int level)
        {
            if (Jobs is null) return false;
            foreach (PropertyInfo prop in Jobs.GetType().GetProperties())
            {
                object? value = prop.GetValue(Jobs);
                if (value is not Job job)
                {
                    continue;
                }

                int jobLevel = job.Level;
                if (jobLevel >= level)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasQuest(int id)
        {
            //return Quests.Exists(q => q.Id == id);
            return Quests.Count > 0 && Quests.Contains(id);
        }
        /*public bool IsQuestCompleted(int id)
        {
            return HasQuest(id) && Quests.First(q => q.Id == id).Completed;
        }*/

        public bool HasMinion(uint id)
        {
            return Minions.Count > 0 && Minions.Contains(id);
        }
        public bool HasMount(uint id)
        {
            return Mounts.Count > 0 && Mounts.Contains(id);
        }
        // ReSharper disable once InconsistentNaming
        public bool HasTTC(uint id)
        {
            return TripleTriadCards.Count > 0 && TripleTriadCards.Contains(id);
        }
        public bool HasEmote(uint id)
        {
            return Emotes.Count > 0 && Emotes.Contains(id);
        }
        public bool HasBarding(uint id)
        {
            return Bardings.Count > 0 && Bardings.Contains(id);
        }
        public bool HasFramerKit(uint id)
        {
            return FramerKits.Count > 0 && FramerKits.Contains(id);
        }
        public bool HasOrchestrionRoll(uint id)
        {
            return OrchestrionRolls.Count > 0 && OrchestrionRolls.Contains(id);
        }
        public bool HasOrnament(uint id)
        {
            return Ornaments.Count > 0 && Ornaments.Contains(id);
        }
        public bool HasGlasses(uint id)
        {
            return Glasses.Count > 0 && Glasses.Contains(id);
        }

        public BeastTribeRank? GetBeastReputation(uint id)
        {
            return BeastReputations.Find(br => br.Id == id);
        }

        public bool HasBeastReputation(uint id)
        {
            return BeastReputations.Count > 0 && BeastReputations.Exists(br => br.Id == id);
        }

        public bool IsDutyCompleted(uint id)
        {
            return Duties.Count > 0 && Duties.Contains(id);
        }
        public bool IsDutyUnlocked(uint id)
        {
            return DutiesUnlocked.Count > 0 && DutiesUnlocked.Contains(id);
        }
    }
}
