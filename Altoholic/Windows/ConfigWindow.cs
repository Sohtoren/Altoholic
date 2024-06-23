using System;
using System.Collections.Generic;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;

namespace Altoholic.Windows;

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
        this._plugin = plugin;
        this._globalCache = globalCache;
    }

    private ClientLanguage _selectedLanguage;

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (_configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

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
            if (ImGui.Selectable(ClientLanguage.Japanese.ToString(), ClientLanguage.Japanese.ToString() == _selectedLanguage.ToString()))
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

        bool movable = _configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {
            _configuration.IsConfigWindowMovable = movable;
            _configuration.Save();
        }
    }
}