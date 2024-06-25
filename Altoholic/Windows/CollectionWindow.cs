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
using Lumina.Excel.GeneratedSheets;
using System.Diagnostics;
using Emote = Altoholic.Models.Emote;
using Mount = Altoholic.Models.Mount;
using TripleTriadCard = Altoholic.Models.TripleTriadCard;

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
        private bool _obtainedTripleTriadCardsOnly;
        private bool _obtainedEmotesOnly;
        private bool _obtainedBardingsOnly;
        private bool _obtainedFramerKitsOnly;

        /*public override void OnClose()
        {
            Plugin.Log.Debug("CollectionWindow, OnClose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
        }*/

        public void Dispose()
        {
            Plugin.Log.Info("CollectionWindow, Dispose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
            _obtainedTripleTriadCardsOnly = false;
            _obtainedEmotesOnly = false;
            _obtainedBardingsOnly = false;
            _obtainedFramerKitsOnly = false;
        }

        public void Clear()
        {
            Plugin.Log.Info("CollectionWindow, Clear() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
            _obtainedTripleTriadCardsOnly = false;
            _obtainedEmotesOnly = false;
            _obtainedBardingsOnly = false;
            _obtainedFramerKitsOnly = false;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
            _obtainedMinionsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedMountsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedTripleTriadCardsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedEmotesOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedBardingsOnly = _plugin.Configuration.ObtainedOnly;
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
            ImGui.TextUnformatted("");
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
                    DrawBardings(currentCharacter);
                }
            }
            using (var emotesTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 780)}"))
            {
                if (emotesTab.Success)
                {
                    DrawEmotes(currentCharacter);
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
                    DrawFramerKits(currentCharacter);
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
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4902)}"))
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
            
            using (var tripleTriadTab = ImRaii.TabItem("Triple Triad")) 
            {
                if (tripleTriadTab.Success)
                {
                    DrawTripleTriadCards(currentCharacter);
                }
            }
        }

        /**************************************************/
        /**********************Minions*********************/
        /**************************************************/
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
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###MinionsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###MinionsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, widthCol2);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            string endStr = string.Empty;
            if (!_isSpoilerEnabled)
            {
                endStr += $"{Loc.Localize("ObtainedLowercase", " obtained")}";
            }
            else
            {
                endStr += $"/{_globalCache.MinionStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Minions.Count}{endStr}");
        }

        private void DrawMinionsCollection(Character currentCharacter)
        {
            List<uint> minions = (_obtainedMinionsOnly) ? currentCharacter.Minions : _globalCache.MinionStorage.Get();
            int minionsCount = minions.Count;
            if (minionsCount == 0) return;
            int rows = (int)Math.Ceiling(minionsCount / (double)10);
            int heigth = rows * 48 + 0;

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

        /**************************************************/
        /**********************Mounts**********************/
        /**************************************************/
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
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###MountsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###MountsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, widthCol2);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            string endStr = string.Empty;
            if (!_isSpoilerEnabled)
            {
                endStr += $"{Loc.Localize("ObtainedLowercase", " obtained")}";
            }
            else
            {
                endStr += $"/{_globalCache.MountStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Mounts.Count}{endStr}");
        }

        private void DrawMountsCollection(Character currentCharacter)
        {
            List<uint> mounts = (_obtainedMountsOnly) ? currentCharacter.Mounts : _globalCache.MountStorage.Get();
            int mountsCount = mounts.Count;
            int rows =  (int)Math.Ceiling(mountsCount / (double)10);
            int heigth = rows * 48 + rows;

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

        /**************************************************/
        /***********************TTC************************/
        /**************************************************/
        private void DrawTripleTriadCards(Character currentCharacter)
        {
            using var tripleTriadCardsTabTable = ImRaii.Table("###TripleTriadCardsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!tripleTriadCardsTabTable) return;
            ImGui.TableSetupColumn($"###TripleTriadCardsTabTable#{currentCharacter.Id}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedTripleTriadCardsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedTripleTriadCardsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawTripleTriadCardsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var tripleTriadCardsTableAmount = ImRaii.Table("###TripleTriadCardsTableAmount", 2);
            if (!tripleTriadCardsTableAmount) return;
            ImGui.TableSetupColumn($"###TripleTriadCardsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 480);
            ImGui.TableSetupColumn($"###TripleTriadCardsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.TripleTriadCards.Count}/{_globalCache.TripleTriadCardStorage.Count()}");
        }

        private void DrawTripleTriadCardsCollection(Character currentCharacter)
        {
            List<uint> tripleTriadCards = (_obtainedTripleTriadCardsOnly) ? currentCharacter.TripleTriadCards : _globalCache.TripleTriadCardStorage.Get();
            int tripleTriadCardsCount = tripleTriadCards.Count;
            if (tripleTriadCardsCount == 0) return;
            int rows = (int)Math.Ceiling(tripleTriadCardsCount / (double)10);
            int heigth = rows * 48 + 0;

            using var table = ImRaii.Table($"###TripleTriadCardsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(575, heigth));
            if (!table) return;

            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###TripleTriadCardsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint ttcId in tripleTriadCards)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                TripleTriadCard? ttc = _globalCache.TripleTriadCardStorage.GetTripleTriadCard(_currentLocale, ttcId);
                if (ttc == null)
                {
                    continue;
                }


                if (currentCharacter.HasTTC(ttcId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(ttc.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(ttc.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(060028), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, ttc);
                }

                i++;
            }
        }

        /**************************************************/
        /***********************Emote**********************/
        /**************************************************/
        private void DrawEmotes(Character currentCharacter)
        {
            using var emoteTabTable = ImRaii.Table("###EmotesTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!emoteTabTable) return;
            ImGui.TableSetupColumn($"###EmotesTabTable#{currentCharacter.Id}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedEmotesOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedEmotesOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawEmotesCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var emoteTableAmount = ImRaii.Table("###EmotesTableAmount", 2);
            if (!emoteTableAmount) return;
            ImGui.TableSetupColumn($"###EmotesTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 480);
            ImGui.TableSetupColumn($"###EmotesTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Emotes.Count}/{_globalCache.EmoteStorage.Count()}");
        }

        private void DrawEmotesCollection(Character currentCharacter)
        {
            List<uint> emotes = (_obtainedEmotesOnly) ? currentCharacter.Emotes : _globalCache.EmoteStorage.Get();

            switch (currentCharacter.Profile?.GrandCompany)
            {
                case 1:
                    {
                        emotes.Remove(56);
                        emotes.Remove(57);
                        break;
                    }
                case 2:
                    {
                        emotes.Remove(55);
                        emotes.Remove(57);
                        break;
                    }
                case 3:
                    {
                        emotes.Remove(55);
                        emotes.Remove(56);
                        break;
                    }
            }

            int emoteCount = emotes.Count;
            if (emoteCount == 0) return;
            int rows = (int)Math.Ceiling(emoteCount / (double)10);
            int heigth = rows * 48 + 0;

            using var table = ImRaii.Table($"###EmotesTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(571, heigth));
            if (!table) return;

            ImGui.TableSetupColumn($"###EmotesTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###EmotesTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint emoteId in emotes)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Emote? emote = _globalCache.EmoteStorage.GetEmote(_currentLocale, emoteId);
                if (emote == null)
                {
                    continue;
                }

                if (currentCharacter.HasEmote(emoteId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(emote.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(emote.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawEmoteTooltip(_currentLocale, ref _globalCache, emote);
                }

                i++;
            }
        }

        /**************************************************/
        /*********************Barding**********************/
        /**************************************************/
        private void DrawBardings(Character currentCharacter)
        {
            using var bardingsTabTable = ImRaii.Table("###BardingsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!bardingsTabTable) return;
            ImGui.TableSetupColumn($"###BardingsTabTable#{currentCharacter.Id}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedBardingsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedBardingsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawBardingsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var bardingsTableAmount = ImRaii.Table("###BardingsTableAmount", 2);
            if (!bardingsTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###BardingsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###BardingsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, widthCol2);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            string endStr = string.Empty;
            if (!_isSpoilerEnabled)
            {
                endStr += $"{Loc.Localize("ObtainedLowercase", " obtained")}";
            }
            else
            {
                endStr += $"/{_globalCache.BardingStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Bardings.Count}{endStr}");
        }

        private void DrawBardingsCollection(Character currentCharacter)
        {
            List<uint> bardings = (_obtainedBardingsOnly) ? currentCharacter.Bardings : _globalCache.BardingStorage.Get();
            int bardingsCount = bardings.Count;
            if (bardingsCount == 0) return;
            int rows = (int)Math.Ceiling(bardingsCount / (double)10);
            int heigth = rows * 48 + 0;

            using var table = ImRaii.Table($"###BardingsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, heigth));
            if (!table) return;

            ImGui.TableSetupColumn($"###BardingsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###BardingsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint bId in bardings)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Barding? b = _globalCache.BardingStorage.GetBarding(_currentLocale, bId);
                if (b == null)
                {
                    continue;
                }

                if (currentCharacter.HasBarding(bId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(b.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(b.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawBardingTooltip(_currentLocale, ref _globalCache, b, b.Icon);
                }

                i++;
            }
        }
        /**************************************************/
        /*********************FramerKit**********************/
        /**************************************************/
        private void DrawFramerKits(Character currentCharacter)
        {
            using var framerKitsTabTable = ImRaii.Table("###FramerKitsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!framerKitsTabTable) return;
            ImGui.TableSetupColumn($"###FramerKitsTabTable#{currentCharacter.Id}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedFramerKitsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedFramerKitsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawFramerKitsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var framerKitsTableAmount = ImRaii.Table("###FramerKitsTableAmount", 2);
            if (!framerKitsTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###FramerKitsTableAmount#{currentCharacter.Id}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###FramerKitsTableAmount#{currentCharacter.Id}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, widthCol2);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            string endStr = string.Empty;
            if (!_isSpoilerEnabled)
            {
                endStr += $"{Loc.Localize("ObtainedLowercase", " obtained")}";
            }
            else
            {
                endStr += $"/{_globalCache.FramerKitStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.FramerKits.Count}{endStr}");
        }

        private void DrawFramerKitsCollection(Character currentCharacter)
        {
            List<uint> framerKits = (_obtainedFramerKitsOnly) ? currentCharacter.FramerKits : _globalCache.FramerKitStorage.Get();
            int framerKitsCount = framerKits.Count;
            if (framerKitsCount == 0) return;
            int rows = (int)Math.Ceiling(framerKitsCount / (double)10);
            int heigth = rows * 48 + 0;

            using var table = ImRaii.Table($"###FramerKitsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, heigth));
            if (!table) return;

            ImGui.TableSetupColumn($"###FramerKitsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn($"###FramerKitsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint fkId in framerKits)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                FramerKit? f = _globalCache.FramerKitStorage.LoadItem(_currentLocale, fkId);
                if (f == null)
                {
                    continue;
                }

                if (currentCharacter.HasFramerKit(fkId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(f.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(f.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, f);
                }

                i++;
            }
        }
    }
}