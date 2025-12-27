namespace Altoholic.Models
{
    public class DatabaseCharacter
    {
        public ulong CharacterId { get; init; } = 0;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string HomeWorld { get; init; } = string.Empty;
        public string CurrentWorld { get; init; } = string.Empty;
        public string Datacenter { get; init; } = string.Empty;
        public string CurrentDatacenter { get; init; } = string.Empty;
        public string Region { get; init; } = string.Empty;
        public string CurrentRegion { get; init; } = string.Empty;
        public bool IsSprout { get; init; } = false;
        public bool IsBattleMentor { get; init; } = false;
        public bool IsTradeMentor { get; init; } = false;
        public bool IsReturner { get; init; } = false;
        public uint LastJob { get; init; } = 0;
        public int LastJobLevel { get; init; } = 0;

        // ReSharper disable once InconsistentNaming
        public string FCTag { get; init; } = string.Empty;
        public string FreeCompany { get; init; } = string.Empty;
        public long LastOnline { get; init; } = 0;
        public uint PlayTime { get; init; } = 0;
        public long LastPlayTimeUpdate { get; init; } = 0;
        public bool HasPremiumSaddlebag { get; init; } = false;
        public short PlayerCommendations { get; init; } = 0;
        public string CurrentFacewear { get; init; } = string.Empty;
        public ushort CurrentOrnament { get; init; } = 0;
        public ushort UnreadLetters { get; init; } = 0;
        public bool IslandSanctuaryUnlocked { get; init; } = false;
        public byte IslandSanctuaryLevel { get; init; } = 0;
        public string Attributes { get; init; } = string.Empty;
        public string Currencies { get; init; } = string.Empty;
        public string Jobs { get; init; } = string.Empty;
        public string Profile { get; init; } = string.Empty;
        public string Quests { get; init; } = string.Empty;
        public string Inventory { get; init; } = string.Empty;
        public string ArmoryInventory { get; init; } = string.Empty;
        public string Saddle { get; init; } = string.Empty;
        public string Gear { get; init; } = string.Empty;
        public string Retainers { get; init; } = string.Empty;
        public string BlacklistedRetainers { get; init; } = string.Empty;
        public string Minions { get; init; } = string.Empty;
        public string Mounts { get; init; } = string.Empty;
        public string TripleTriadCards { get; init; } = string.Empty;
        public string Emotes { get; init; } = string.Empty;
        public string Bardings { get; init; } = string.Empty;
        public string FramerKits { get; init; } = string.Empty;
        public string OrchestrionRolls { get; init; } = string.Empty;
        public string Ornaments { get; init; } = string.Empty;
        public string Glasses { get; init; } = string.Empty;
        public string BeastReputations { get; init; } = string.Empty;
        public string Duties { get; init; } = string.Empty;
        public string DutiesUnlocked { get; init; } = string.Empty;
        public string Houses { get; init; } = string.Empty;
        public string Hairstyles { get; init; } = string.Empty;
        public string Facepaints { get; init; } = string.Empty;
        public string SecretRecipeBooks { get; init; } = string.Empty;
        public string Vistas { get; init; } = string.Empty;
        public int? SightseeingLogUnlockState { get; init; }
        public int? SightseeingLogUnlockStateEx { get; init; }
        public string Armoire { get; init; } = string.Empty;
        public string GlamourDresser { get; init; } = string.Empty;
        public string PvPProfile { get; init; } = string.Empty;
    }
}