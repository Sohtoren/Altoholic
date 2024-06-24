using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Altoholic.Windows
{
    public class CollectionWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private bool _isSpoilerEnabled;
        private GlobalCache _globalCache;

        public CollectionWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        )
            : base(
                name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450), MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;

        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;

        private Character? _currentCharacter;
        private IEnumerable<Mount>? _currentItems;
        private uint? _currentItem;
        private string _searchedItem = string.Empty;
        private string _lastSearchedItem = string.Empty;

        private bool _obtainedMinionsOnly;
        private bool _obtainedMountsOnly;

        public override void OnClose()
        {
            Plugin.Log.Debug("CollectionWindow, OnClose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
        }

        public void Dispose()
        {
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
            _obtainedMinionsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedMountsOnly = _plugin.Configuration.ObtainedOnly;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using var table = ImRaii.Table("###CharactersCollectionTable", 2);
                if (!table) return;

                ImGui.TableSetupColumn("###CharactersCollectionTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn("###CharactersCollectionTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (var listBox = ImRaii.ListBox("###CharactersCollectionTable#CharactersListBox", new Vector2(200, -1)))
                {
                    if (listBox)
                    {
                        ImGui.SetScrollY(0);
                        if (chars.Count > 0)
                        {
                            if (ImGui.Selectable($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersCollectionTable#CharactersListBox#All", _currentCharacter == null))
                            {
                                _currentCharacter = null;
                            }

                            foreach (Character currChar in chars.Where(currChar => ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == _currentCharacter)))
                            {
                                _currentCharacter = currChar;
                                _currentItem = null;
                                _currentItems = null;
                                _searchedItem = string.Empty;
                                _lastSearchedItem = string.Empty;
                            }
                        }
                    }
                }
                ImGui.TableSetColumnIndex(1);
                if (_currentCharacter is not null)
                {
                    DrawTabs(_currentCharacter);
                }
                else
                {
                    DrawAll(chars);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic CharactersCollectionTable Exception : {0}", e);
            }
        }

        private void DrawAll(List<Character> chars)
        {

        }

        public void DrawTabs(Character currentCharacter)
        {
            using var tabBar = ImRaii.TabBar($"###CollectionWindow#Tabs");
            if (!tabBar.Success) return;
            using (var bardingsTab =
                   ImRaii.TabItem($"{Loc.Localize("Barding", "Barding")}")) // Harnisch Barding Barde バード
            {
                if (bardingsTab.Success)
                {
                    //DrawBardings(currentCharacter);
                }
            }
            using (var emotesTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 780)}"))
            {
                if (emotesTab.Success)
                {
                    //DrawEmotes(currentCharacter);
                }
            }
            using (var fashionAccessoriesTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 13671)}"))
            {
                if (fashionAccessoriesTab.Success)
                {
                    //DrawEmotes(currentCharacter);
                }
            }
            using (var framerKitTab =
                   ImRaii.TabItem($"{Loc.Localize("FramerKit", "Framer's kit")}")) // Portraitmaterial[p] / Framer's kit / Portrait / ポートレート
            {
                if (framerKitTab.Success)
                {
                    //DrawEmotes(currentCharacter);
                }
            }
            
            using (var hairsTab =
                   ImRaii.TabItem($"{Loc.Localize("Hairstyle", "Hairstyle")}")) // Frisur Hairstyle Coupe de cheveux 髪型
            {
                if (hairsTab.Success)
                {

                }
            }

            using (var minionsTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8303)}"))
            {
                if (minionsTab.Success)
                {
                    DrawMinions(currentCharacter);
                }
            }

            using (var mountsTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4901)}")) // Mounts
            {
                if (mountsTab.Success)
                {
                    DrawMounts(currentCharacter);
                }
            }

            using (var orchestrionsTab = ImRaii.TabItem("Orchestrion")) // same name in all languages
            {
                if (orchestrionsTab.Success)
                {

                }
            }
            
            using (var tomesTab = ImRaii.TabItem("Tomes")) 
            {
                if (tomesTab.Success)
                {

                }
            }
        }

        
        
        private void DrawMinions(Character currentCharacter)
        {
            using var minionsTabTable = ImRaii.Table("###MinionsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!minionsTabTable) return;
            ImGui.TableSetupColumn($"###MinionsTabTable#{currentCharacter.Id}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedMinionsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedMinionsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawMinionsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var minionsTableAmount = ImRaii.Table("###MinionsTableAmount", 2);
            if (!minionsTableAmount) return;
            ImGui.TableSetupColumn($"###MinionsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 505);
            ImGui.TableSetupColumn($"###MinionsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{currentCharacter.Minions.Count}");
            if (_isSpoilerEnabled)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"/ {_globalCache.MinionStorage.Count()}");
            }
        }

        private void DrawMinionsCollection(Character currentCharacter)
        {
            List<uint> minions = (_obtainedMinionsOnly) ? currentCharacter.Minions : _globalCache.MinionStorage.Get();
            int minionsCount = minions.Count;
            if (minionsCount == 0) return;
            int rows = (int)Math.Ceiling(minionsCount / (double)10);
            int heigth = rows * 48 + 48;

            using var table = ImRaii.Table($"###MinionsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(575, heigth));
            if (!table) return;

            ImGui.TableSetupColumn($"###MinionsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MinionsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint minionId in minions)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Minion? m = _globalCache.MinionStorage.GetMinion(_currentLocale, minionId);
                if (m == null)
                {
                    continue;
                }

                if (currentCharacter.HasMinion(minionId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(m.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(m.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMinionTooltip(_currentLocale, ref _globalCache, m);
                }

                i++;
            }
        }

        private void DrawMounts(Character currentCharacter)
        {
            using var mountsTabTable = ImRaii.Table("###MountsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!mountsTabTable) return;
            ImGui.TableSetupColumn($"###MountsTabTable#{currentCharacter.Id}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedMountsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedMountsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawMountsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var mountsTableAmount = ImRaii.Table("###MountsTableAmount", 2);
            if (!mountsTableAmount) return;
            ImGui.TableSetupColumn($"###MountsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 500);
            ImGui.TableSetupColumn($"###MountsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{currentCharacter.Mounts.Count}");
            if (_isSpoilerEnabled)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"/ {_globalCache.MountStorage.Count()}");
            }
        }

        private void DrawMountsCollection(Character currentCharacter)
        {
            List<uint> mounts = (_obtainedMountsOnly) ? currentCharacter.Mounts : _globalCache.MountStorage.Get();
            int mountsCount = mounts.Count;
            int rows =  (int)Math.Ceiling(mountsCount / (double)10);
            int heigth = rows * 48 + 30;

            using var mountTable = ImRaii.Table($"###MountsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(573, heigth));
            if (!mountTable) return;

            ImGui.TableSetupColumn($"###MountsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###MountsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint mountId in mounts)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Mount? m = _globalCache.MountStorage.GetMount(_currentLocale, mountId);
                if (m == null)
                {
                    continue;
                }

                if (currentCharacter.HasMount(mountId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(m.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(m.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMountTooltip(_currentLocale, ref _globalCache, m);
                }

                i++;
            }
        }
        
        
    }
}