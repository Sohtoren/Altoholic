using Dalamud.Configuration;
using Dalamud.Game;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace Altoholic
{
    public enum TimersStatus
    {
        MiniCacpot,
        JumboCacpot,
        FashionReport,
        CustomDeliveries,
        DomanEnclave,
        MaskedCarnivale,
        Tribes
    }

    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }
        public bool IsSpoilersEnabled { get; set; }
        public bool ObtainedOnly { get; set; } = true;
        public ClientLanguage Language { get; set; } = ClientLanguage.English;
        public Models.Sort CharacterWindowSort { get; set; } = Models.Sort.Auto;
        public bool IsPlaytimeNotificationEnabled { get; set; } = true;
        public int PlaytimeNotificationDays { get; set; } = 7;
        public bool IsAutoSaveChatMessageEnabled { get; set; } = true;
        public int AutoSaveTimer { get; set; } = 5;
        public int DateFormat { get; set; }
        public HashSet<TimersStatus>? EnabledTimers { get; set; }
        public int TimerStandaloneIcon { get; set; } = 48;
        public float TimerStandaloneIconAlpha { get; set; } = 0.5f;
        public bool TimerStandaloneShowAtStartup { get; set; }
        public bool TimerCrossMarkForNotUnlocked { get; set; } = false;
        /*public float TimerStandaloneWindowPositionX { get; set; }
        public float TimerStandaloneWindowPositionY { get; set; }*/

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private IDalamudPluginInterface? _pluginInterface;

        public void Initialize(int version, IDalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
            Version = version;
        }

        public void Save()
        {
            _pluginInterface!.SavePluginConfig(this);
        }
        public void TrySave()
        {
            try
            {
                _pluginInterface!.SavePluginConfig(this);
            }
            catch(Exception e)
            {
                Plugin.Log.Debug($"Config save error: {e}");
            }
        }
    }
}
