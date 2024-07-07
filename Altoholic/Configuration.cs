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
        public bool IsSpoilersEnabled { get; set; } = true; // Todo: Change this to false for release
        public bool ObtainedOnly { get; set; } = true;
        public Dalamud.Game.ClientLanguage Language { get; set; } = Dalamud.Game.ClientLanguage.English;
        public Models.Sort CharacterWindowSort { get; set; } = Models.Sort.Auto;

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
