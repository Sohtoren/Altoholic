using Dalamud;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System;
using System.Numerics;

namespace Altoholic.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;
    private CharactersWindow charactersWindow { get; init; }
    private DetailsWindow detailsWindow { get; init; }
    private JobsWindow jobsWindow { get; init; }
    private CurrenciesWindow currenciesWindow { get; init; }
    private InventoriesWindow inventoriesWindow { get; init; }
    private RetainersWindow retainersWindow { get; init; }
    private ConfigWindow configWindow { get; init; }

    private ClientLanguage currentLocale;

    public MainWindow(
        Plugin plugin,
        string name,
        CharactersWindow charactersWindow,
        DetailsWindow detailsWindow,
        JobsWindow jobsWindow,
        CurrenciesWindow currenciesWindow,
        InventoriesWindow inventoriesWindow,
        RetainersWindow retainersWindow,
        ConfigWindow configWindow/*,
        ClientLanguage currentLocale*/
    )
        : base(name, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(1050, 565),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.plugin = plugin;
        //this.currentLocale = currentLocale;
        this.charactersWindow = charactersWindow;
        this.detailsWindow = detailsWindow;
        this.jobsWindow = jobsWindow;
        this.currenciesWindow = currenciesWindow;
        this.inventoriesWindow = inventoriesWindow;
        this.retainersWindow = retainersWindow;
        this.configWindow = configWindow;
    }

    public override void OnClose()
    {
        Plugin.Log.Debug("MainWindow, OnClose() called");
        charactersWindow.IsOpen = false;
        currenciesWindow.IsOpen = false;
        detailsWindow.IsOpen = false;
        jobsWindow.IsOpen = false;
        inventoriesWindow.IsOpen = false;
        retainersWindow.IsOpen = false;
        configWindow.IsOpen = false;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        currentLocale = plugin.Configuration.Language;
        using var tabBar = ImRaii.TabBar($"###MainWindow#Tabs");
        if (!tabBar.Success) return;
        using (var charactersTab = ImRaii.TabItem($"{Utils.GetAddonString(7543)}"))
        {
            if (charactersTab.Success)
            {
                charactersWindow.Draw();
            }
        };
        
        using (var detailsTab = ImRaii.TabItem($"{Utils.GetAddonString(6361)}"))
        {
            if (detailsTab.Success)
            {
                detailsWindow.Draw();
            }
        };


        using (var jobsTab = ImRaii.TabItem($"{Utils.GetAddonString(760)}"))
        {
            if (jobsTab.Success)
            {
                jobsWindow.Draw();
            }
        };
        
        using (var currenciesTab = ImRaii.TabItem($"{Utils.GetAddonString(761)}"))
        {
            if (currenciesTab.Success)
            {
                currenciesWindow.Draw();
            }
        };
        
        //using (var bagsTab = ImRaii.TabItem($"{Utils.GetAddonString(358)}"))// Bags
        using (var inventoryTab = ImRaii.TabItem($"{Utils.GetAddonString(520)}"))// Inventory
        {
            if (inventoryTab.Success)
            {
                inventoriesWindow.Draw();
            }
        };
        
        using (var retainersTab = ImRaii.TabItem($"{Utils.GetAddonString(532)}"))
        {
            if (retainersTab.Success)
            {
                retainersWindow.Draw();
            }
        };
        
        using (var settingsTab = ImRaii.TabItem($"{Utils.GetAddonString(10119)}"))
        {
            if (settingsTab.Success)
            {
                configWindow.Draw();
            }
        };

        /*if (ImGui.BeginTabBar($"###MainWindow#Tabs"))
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

            if (ImGui.BeginTabItem($"{Utils.GetAddonString(760)}"))
            {
                jobsWindow.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Currencies"))
            {
                currenciesWindow.Draw();
                ImGui.EndTabItem();
            }

            /*if (ImGui.BeginTabItem($"Bags")) //358
            {
                ImGui.EndTabItem();
            }
        
            if (ImGui.BeginTabItem($"Collection")) //Pet&Mount
            {
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Retainers"))
            {
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Reputations")) // 102512
            {
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem($"Progress"))
            {
                ImGui.EndTabItem();
            }*/

        //ImGui.EndTabBar();
        /*bool val = true;
        ImGui.Checkbox("Anonymize", ref val);*/
        //}
    }
}
