using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Altoholic.Models
{
    public class Character
    {
        public ulong CharacterId { get; init; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string HomeWorld { get; set; } = string.Empty;
        public string CurrentWorld { get; set; } = string.Empty;
        public string Datacenter {  get; set; } = string.Empty;
        public string CurrentDatacenter {  get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public string CurrentRegion {  get; set; } = string.Empty;
        public bool IsSprout { get; set; }
        public bool IsBattleMentor { get; set; }
        public bool IsTradeMentor { get; set; }
        public bool IsReturner { get; set; }
        public uint LastJob { get; set; }
        public int LastJobLevel { get; set; }
        // ReSharper disable once InconsistentNaming
        public string FCTag { get; set; } = string.Empty;
        public string FreeCompany { get; init; } = string.Empty;
        public long LastOnline { get; set; }
        public uint PlayTime { get; set; }
        public long LastPlayTimeUpdate {  get; set; }
        public bool HasPremiumSaddlebag { get; set; }
        public short PlayerCommendations { get; set; }
        public ushort[] CurrentFacewear { get; init; } = [0,0];
        public ushort CurrentOrnament { get; set; }
        public ushort UnreadLetters { get; set; }
        public bool IslandSanctuaryUnlocked { get; set; } = false;
        public byte IslandSanctuaryLevel { get; set; }
        public Attributes? Attributes { get; set; }
        public PlayerCurrencies? Currencies { get; set; }
        public Jobs? Jobs { get; set; }
        public Profile? Profile { get; set; }
        public HashSet<int> Quests { get; set; } = [];
        public List<Inventory> Inventory { get; set; } = [];
        public ArmoryGear? ArmoryInventory { get; set; }
        public List<Inventory> Saddle { get; set; } = [];
        public List<Gear> Gear { get; set; } = [];
        public List<Retainer> Retainers { get; set; } = [];
        public Dictionary<ulong, string> BlacklistedRetainers { get; init; } = [];
        public HashSet<uint> Minions { get; init; } = [];
        public HashSet<uint> Mounts { get; init; } = [];
        public HashSet<uint> TripleTriadCards { get; init; } = [];
        public HashSet<uint> Emotes { get; init; } = [];
        public HashSet<uint> Bardings { get; init; } = [];
        public HashSet<uint> FramerKits { get; init; } = [];
        public HashSet<uint> OrchestrionRolls { get; init; } = [];
        public HashSet<uint> Ornaments { get; init; } = [];
        public HashSet<uint> Glasses { get; init; } = [];
        public List<CurrenciesHistory> CurrenciesHistory { get; set; } = [];
        public List<BeastTribeRank> BeastReputations { get; init; } = [];
        public HashSet<uint> Duties { get; init; } = [];
        public HashSet<uint> DutiesUnlocked { get; init; } = [];
        public List<Housing> Houses { get; init; } = [];
        public HashSet<uint> Hairstyles { get; set; } = [];
        public HashSet<uint> Facepaints { get; init; } = [];
        public HashSet<uint> SecretRecipeBooks { get; init; } = [];
        public HashSet<uint> Vistas { get; init; } = [];
        public int? SightseeingLogUnlockState { get; set; }
        public int? SightseeingLogUnlockStateEx { get; set; }
        public HashSet<uint> Armoire { get; set; } = [];
        public GlamourItem[] GlamourDresser { get; init; } = new GlamourItem[8000];
        public PvPProfile? PvPProfile { get; set; }
        public Timers Timers { get; init; } = new();
        public GearSet? CurrentGearSet { get; set; }
        public Dictionary<int, GearSet> GearSets { get; init; } = new(100);
        public Dictionary<byte, GlamourPlate> GlamourPlates { get; init; } = new(20);
        public Dictionary<uint, DateTime> CompletedRoulettes { get; init; } = [];


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

        public bool HasAnyBeastReputationUnlocked()
        {
            return HasAnyPreviousBeastReputationUnlocked() ||
                   HasAnyCurrentBeastReputationUnlocked();
        }
        public bool HasAnyPreviousBeastReputationUnlocked()
        {
            return HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) ||
                   HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) ||
                   HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) ||
                   HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) ||
                   HasQuest((int)QuestIds.TRIBE_ARR_IXAL) ||
                   HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) ||
                   HasQuest((int)QuestIds.TRIBE_HW_VATH) ||
                   HasQuest((int)QuestIds.TRIBE_HW_MOOGLES) ||
                   HasQuest((int)QuestIds.TRIBE_SB_KOJIN) ||
                   HasQuest((int)QuestIds.TRIBE_SB_ANANTA) ||
                   HasQuest((int)QuestIds.TRIBE_SB_NAMAZU) ||
                   HasQuest((int)QuestIds.TRIBE_SHB_PIXIES) ||
                   HasQuest((int)QuestIds.TRIBE_SHB_QITARI) ||
                   HasQuest((int)QuestIds.TRIBE_SHB_DWARVES) ||
                   HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) ||
                   HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) ||
                   HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS);
        }
        public bool HasAnyCurrentBeastReputationUnlocked()
        {
            return HasQuest((int)QuestIds.TRIBE_DT_PELUPELU) ||
                   HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA) ||
                   HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY);
        }

        public bool HasAnyCustomDeliveryUnlocked()
        {
            return HasQuest((int)QuestIds.CUSTOM_DELIVERIES_ZHLOE_ALIAPOH) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_M_NAAGO) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_KURENAI) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_ADKIRAGH) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_KAI_SHIRR) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_EHLL_TOU) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_CHARLEMEND) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_AMELIANCE) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_ANDEN) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_MARGRAT) ||
                   HasQuest((int)QuestIds.CUSTOM_DELIVERIES_NITOWIKWE);
        }

        public bool IsDutyCompleted(uint id)
        {
            return Duties.Count > 0 && Duties.Contains(id);
        }
        public bool IsDutyUnlocked(uint id)
        {
            return DutiesUnlocked.Count > 0 && DutiesUnlocked.Contains(id);
        }

        public bool HasHairstyle(uint id)
        {
            return Hairstyles.Count > 0 && Hairstyles.Contains(id);
        }
        public bool HasHairstyleFromIds(List<uint> ids)
        {
            return Hairstyles.Count != 0 && ids.Any(id => Hairstyles.Contains(id));
        }

        public bool HasFacepaint(uint id)
        {
            return Facepaints.Count > 0 && Facepaints.Contains(id);
        }
        public bool HasFacepaintFromIds(List<uint> ids)
        {
            return Facepaints.Count != 0 && ids.Any(id => Facepaints.Contains(id));
        }
        
        public bool HasSecretRecipeBook(uint id)
        {
            return SecretRecipeBooks.Count > 0 && SecretRecipeBooks.Contains(id);
        }

        public bool HasVista(uint id)
        {
            return Vistas.Count > 0 && Vistas.Contains(id);
        }
        public bool HasArmoire(uint id)
        {
            return Armoire.Count > 0 && Armoire.Contains(id);
        }


        public void TryAddGearSet(int key, GearSet gs)
        {
            if (GearSets.Count >= 100 && !GearSets.ContainsKey(key))
                throw new InvalidOperationException("Dictionary size limit reached.");

            GearSets[key] = gs;
        }
        public void TryAddGlamourPlate(byte key, GlamourPlate gp)
        {
            if (GlamourPlates.Count >= 20 && !GlamourPlates.ContainsKey(key))
                throw new InvalidOperationException("Dictionary size limit reached.");

            GlamourPlates[key] = gp;
        }
    }
}
