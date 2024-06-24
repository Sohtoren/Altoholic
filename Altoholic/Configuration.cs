using Dalamud;
using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace Altoholic
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool IsSpoilersEnabled { get; set; } = true; // Todo: Change this to false for release
        public bool ObtainedOnly { get; set; } = true;
        public ClientLanguage Language { get; set; } = ClientLanguage.English;
        public Models.Sort CharacterWindowSort { get; set; } = Models.Sort.Auto;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? _pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
        }

        public void Save()
        {
            _pluginInterface!.SavePluginConfig(this);
        }
    }
}
