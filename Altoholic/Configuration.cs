using Dalamud.Configuration;
using Dalamud.Game;
using Dalamud.Plugin;
using System;

namespace Altoholic
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool IsSpoilersEnabled { get; set; } = false;
        public bool ObtainedOnly { get; set; } = true;
        public ClientLanguage Language { get; set; } = ClientLanguage.English;
        public Models.Sort CharacterWindowSort { get; set; } = Models.Sort.Auto;
        public bool IsAutoSaveChatMessageEnabled { get; set; } = true;
        public int AutoSaveTimer { get; set; } = 5;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private IDalamudPluginInterface? _pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
        }

        public void Save()
        {
            _pluginInterface!.SavePluginConfig(this);
        }
    }
}
