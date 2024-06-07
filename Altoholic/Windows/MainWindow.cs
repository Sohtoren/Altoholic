using Dalamud;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System;
using System.Numerics;

namespace Altoholic.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;
    private readonly IPluginLog pluginLog;
    private readonly IDataManager dataManager;
    private CharactersWindow charactersWindow { get; init; }
    private DetailsWindow detailsWindow { get; init; }
    private JobsWindow jobsWindow { get; init; }
    private CurrenciesWindow currenciesWindow { get; init; }

    private readonly ClientLanguage currentLocale;

    public MainWindow(
        Plugin plugin,
        string name,
        IPluginLog pluginLog,
        IDataManager dataManager,
        CharactersWindow charactersWindow,
        DetailsWindow detailsWindow,
        JobsWindow jobsWindow,
        CurrenciesWindow currenciesWindow,
        ClientLanguage currentLocale
    )
        : base(name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(1050, 565),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.plugin = plugin;
        this.pluginLog = pluginLog;
        this.dataManager = dataManager;
        this.currentLocale = currentLocale;
        this.charactersWindow = charactersWindow;
        this.detailsWindow = detailsWindow;
        this.jobsWindow = jobsWindow;
        this.currenciesWindow = currenciesWindow;
        this.currentLocale = currentLocale;
    }

    public override void OnClose()
    {
        pluginLog.Debug("MainWindow, OnClose() called");
        detailsWindow.IsOpen = false;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        if (ImGui.BeginTabBar($"#tabs"))
        {
            if (ImGui.BeginTabItem($"Characters"))
            {
                charactersWindow.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Details"))
            {
                detailsWindow.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 760)}"))
            {
                jobsWindow.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Currencies"))
            {
                currenciesWindow.Draw();
                ImGui.EndTabItem();
            }

            /*if (ImGui.BeginTabItem($"Bags"))
            {
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Retainers"))
            {
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Reputations"))
            {
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Progress"))
            {
                ImGui.EndTabItem();
            }*/

            ImGui.EndTabBar();
            /*bool val = true;
            ImGui.Checkbox("Anonymize", ref val);*/
        }
    }
}
