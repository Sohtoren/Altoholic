using Altoholic.Cache;
using Dalamud.Game;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace Altoholic.Windows
{
    public class MainWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private CharactersWindow CharactersWindow { get; }
        private DetailsWindow DetailsWindow { get; }
        private JobsWindow JobsWindow { get; }
        private CurrenciesWindow CurrenciesWindow { get; }
        private InventoriesWindow InventoriesWindow { get; }
        private RetainersWindow RetainersWindow { get; }
        private CollectionWindow CollectionWindow { get; }
        private ProgressWindow ProgressWindow { get; }
        private ConfigWindow ConfigWindow { get; }

        private ClientLanguage _currentLocale;

        private readonly GlobalCache _globalCache;

        public MainWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache,
            CharactersWindow charactersWindow,
            DetailsWindow detailsWindow,
            JobsWindow jobsWindow,
            CurrenciesWindow currenciesWindow,
            InventoriesWindow inventoriesWindow,
            RetainersWindow retainersWindow,
            CollectionWindow collectionWindow,
            ProgressWindow progressWindow,
            ConfigWindow configWindow
        )
            : base(name, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1050, 565), MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;
            CharactersWindow = charactersWindow;
            DetailsWindow = detailsWindow;
            JobsWindow = jobsWindow;
            CurrenciesWindow = currenciesWindow;
            InventoriesWindow = inventoriesWindow;
            RetainersWindow = retainersWindow;
            CollectionWindow = collectionWindow;
            ProgressWindow = progressWindow;
            ConfigWindow = configWindow;
        }

        /*public override bool DrawConditions()
        {
            Plugin.Log.Debug("MainWindow DrawConditions");
            return true;
        }*/

        public override void OnClose()
        {
            Plugin.Log.Debug("MainWindow, OnClose() called");
            /*CharactersWindow.IsOpen = false;
            CurrenciesWindow.IsOpen = false;
            DetailsWindow.IsOpen = false;
            JobsWindow.IsOpen = false;
            InventoriesWindow.IsOpen = false;
            RetainersWindow.IsOpen = false;
            CollectionWindow.IsOpen = false;
            CollectionWindow.OnClose();
            ConfigWindow.IsOpen = false;*/
        }

        public void Clear()
        {
            CharactersWindow.IsOpen = false;
            CurrenciesWindow.IsOpen = false;
            DetailsWindow.IsOpen = false;
            JobsWindow.IsOpen = false;
            InventoriesWindow.IsOpen = false;
            RetainersWindow.IsOpen = false;
            CollectionWindow.IsOpen = false;
            ProgressWindow.IsOpen = false;
            ConfigWindow.IsOpen = false;
        }

        public void Dispose()
        {
            CharactersWindow.IsOpen = false;
            CurrenciesWindow.IsOpen = false;
            DetailsWindow.IsOpen = false;
            JobsWindow.IsOpen = false;
            InventoriesWindow.IsOpen = false;
            RetainersWindow.IsOpen = false;
            CollectionWindow.IsOpen = false;
            ProgressWindow.IsOpen = false;
            ConfigWindow.IsOpen = false;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            using var tabBar = ImRaii.TabBar("###MainWindow#Tabs");
            if (!tabBar.Success) return;
            using (var charactersTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 7543)}"))
            {
                if (charactersTab.Success)
                {
                    //if(charactersWindow.DrawConditions())
                    CharactersWindow.Draw();
                }
            }
        
            using (var detailsTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 6361)}"))
            {
                if (detailsTab.Success)
                {
                    DetailsWindow.Draw();
                }
            }


            using (var jobsTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 760)}"))
            {
                if (jobsTab.Success)
                {
                    JobsWindow.Draw();
                }
            }
        
            using (var currenciesTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 761)}"))
            {
                if (currenciesTab.Success)
                {
                    CurrenciesWindow.Draw();
                }
            }
        
            using (var inventoryTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}"))// Inventory
            {
                if (inventoryTab.Success)
                {
                    InventoriesWindow.Draw();
                }
            }
        
            using (var retainersTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 532)}"))
            {
                if (retainersTab.Success)
                {
                    RetainersWindow.Draw();
                }
            }

            using (var collectionTab = ImRaii.TabItem($"{((_currentLocale == ClientLanguage.French) ? _globalCache.AddonStorage.LoadAddonString(_currentLocale, 9515) : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 12790))}")) //Pet&Mount&Orchestrion
            {
                if (collectionTab.Success)
                {
                    CollectionWindow.Draw();
                }
            }

            /*using (var progressTab = ImRaii.TabItem("Progress"))
            {
                if (progressTab.Success)
                {
                    //Double list like retainers, second list is the progress list (msq, event, yokai, trials,etc)
                    ProgressWindow.Draw();
                }
            }*/

            using (var settingsTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 10119)}"))
            {
                if (settingsTab.Success)
                {
                    ConfigWindow.Draw();
                }
            }

            //Todo: anonymize
            //ImGui.EndTabBar();
            /*bool val = true;
            ImGui.Checkbox("Anonymize", ref val);*/
            //}
        }
    }
}
