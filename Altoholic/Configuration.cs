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
        public bool IsConfigWindowMovable { get; set; } = true;
        public ClientLanguage Language { get; set; } = ClientLanguage.English;
        public Models.Sort CharacterWindowSort { get; set; } = Models.Sort.Auto;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;
        }

        public void Save()
        {
            PluginInterface!.SavePluginConfig(this);
        }
    }
}
