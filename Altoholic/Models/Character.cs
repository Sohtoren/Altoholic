using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Altoholic.Models
{
    public class Character
    {
        public ulong Id { get; init; } = 0;
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
            return Quests.Exists(q => q.Id == id);
        }
        public bool IsQuestCompleted(int id)
        {
            return HasQuest(id) && Quests.First(q => q.Id == id).Completed;
        }

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
    }
}
