using System;
using System.Numerics;
using Altoholic.Cache;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Altoholic.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private readonly Configuration _configuration;
        private readonly GlobalCache _globalCache;

        public ConfigWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        ) : base(
            name,
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoScrollWithMouse)
        {
            Size = new Vector2(300, 250);
            SizeCondition = ImGuiCond.Always;

            _configuration = plugin.Configuration;
            _plugin = plugin;
            _globalCache = globalCache;
        }

        private ClientLanguage _selectedLanguage;

        public void Dispose() { }

        public override void Draw()
        {
            _selectedLanguage = _plugin.Configuration.Language;
            //ImGui.TextUnformatted("Settings is not implemented at the moment");
            using var langCombo = ImRaii.Combo($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 338)}###LangCombo", _selectedLanguage.ToString());
            if (langCombo.Success)
            {
                if (ImGui.Selectable(ClientLanguage.German.ToString(), ClientLanguage.German.ToString() == _selectedLanguage.ToString()))
                {
                    _selectedLanguage = ClientLanguage.German;
                }
                if (ImGui.Selectable(ClientLanguage.English.ToString(), ClientLanguage.English.ToString() == _selectedLanguage.ToString()))
                {
                    _selectedLanguage = ClientLanguage.English;
                }
                if (ImGui.Selectable(ClientLanguage.French.ToString(), ClientLanguage.French.ToString() == _selectedLanguage.ToString()))
                {
                    _selectedLanguage = ClientLanguage.French;
                }
                if (ImGui.Selectable("日本語", ClientLanguage.Japanese.ToString() == _selectedLanguage.ToString()))
                {
                    _selectedLanguage = ClientLanguage.Japanese;
                }
                _configuration.Language = _selectedLanguage;
                _plugin.ChangeLanguage(_selectedLanguage);
                try
                {
                    _configuration.Save();
                }
                catch (Exception e) {
                    Plugin.Log.Debug($"Config save error: {e}");
                }
            }

            bool isObtainedOnlyEnabled = _configuration.ObtainedOnly;
            if (ImGui.Checkbox("Obtained Only###ObtainedOnly", ref isObtainedOnlyEnabled))
            {
                _configuration.ObtainedOnly = isObtainedOnlyEnabled;
                _configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted($"{Loc.Localize("ObtainedOnly", "Display unobtained items, mounts, minions, etc with a non spoiler icon")}");
            }

            bool isEnabled = _configuration.IsSpoilersEnabled;
            if (ImGui.Checkbox("Enable Spoilers####EnableSpoilers", ref isEnabled))
            {
                _configuration.IsSpoilersEnabled = isEnabled;
                _configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted($"{Loc.Localize("Spoilers", "Display unobtained icons instead of placeholder")}");
            }
        }
    }
}