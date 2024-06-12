using System;
using System.Collections.Generic;
using System.Numerics;
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
    private Plugin plugin;
    private Configuration Configuration;

    public ConfigWindow(
        Plugin plugin,
        string name
    ) : base(
        name,
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 250);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
        this.plugin = plugin;
    }

    private ClientLanguage selected_language;

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (Configuration.IsConfigWindowMovable)
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
        selected_language = plugin.Configuration.Language;
        //ImGui.TextUnformatted("Settings is not implemented at the moment");
        using var langCombo = ImRaii.Combo($"{Utils.GetAddonString(338)}###LangCombo", selected_language.ToString());
        if (langCombo.Success)
        {
            if (ImGui.Selectable(ClientLanguage.German.ToString(), ClientLanguage.German.ToString() == selected_language.ToString()))
            {
                selected_language = ClientLanguage.German;
            }
            if (ImGui.Selectable(ClientLanguage.English.ToString(), ClientLanguage.English.ToString() == selected_language.ToString()))
            {
                selected_language = ClientLanguage.English;
            }
            if (ImGui.Selectable(ClientLanguage.French.ToString(), ClientLanguage.French.ToString() == selected_language.ToString()))
            {
                selected_language = ClientLanguage.French;
            }
            if (ImGui.Selectable(ClientLanguage.Japanese.ToString(), ClientLanguage.Japanese.ToString() == selected_language.ToString()))
            {
                selected_language = ClientLanguage.Japanese;
            }
            Configuration.Language = selected_language;
            plugin.ChangeLanguage(selected_language);
            try
            {
                Configuration.Save();
            }
            catch (Exception e) {
                Plugin.Log.Debug($"Config save error: {e}");
            }
        }

        var movable = Configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Config Window", ref movable))
        {
            Configuration.IsConfigWindowMovable = movable;
            Configuration.Save();
        }
    }
}