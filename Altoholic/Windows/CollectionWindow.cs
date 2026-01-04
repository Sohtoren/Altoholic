using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using Emote = Altoholic.Models.Emote;
using Glasses = Altoholic.Models.Glasses;
using Mount = Altoholic.Models.Mount;
using Ornament = Altoholic.Models.Ornament;
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

        private bool _obtainedBardingsOnly;
        private bool _obtainedEmotesOnly;
        private bool _obtainedOrnamentsOnly;
        private bool _obtainedFacepaintsOnly;
        private bool _obtainedFramerKitsOnly;
        private bool _obtainedGlassesOnly;
        private bool _obtainedHairstylesOnly;
        private bool _obtainedMinionsOnly;
        private bool _obtainedMountsOnly;
        private bool _obtainedOrchestrionRollsOnly;
        private bool _obtainedSecretRecipeBookOnly;
        private bool _obtainedTripleTriadCardsOnly;
        private bool _obtainedVistaOnly;

        private string _searchOrchestrionRolls = string.Empty;
        private string _searchMinions = string.Empty;
        private string _searchMounts = string.Empty;
        private string _searchTripleTriadCards = string.Empty;
        private string _searchEmotes = string.Empty;
        private string _searchBardings = string.Empty;
        private string _searchFramerKits = string.Empty;
        private string _searchOrnaments = string.Empty;
        private string _searchGlasses = string.Empty;
        private string _searchHairstyles = string.Empty;
        private string _searchFacepaints = string.Empty;
        private string _searchSecretRecipeBooks = string.Empty;
        private string _searchVistas = string.Empty;

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
            _obtainedBardingsOnly = false;
            _obtainedEmotesOnly = false;
            _obtainedOrnamentsOnly = false;
            _obtainedFacepaintsOnly = false;
            _obtainedFramerKitsOnly = false;
            _obtainedGlassesOnly = false;
            _obtainedHairstylesOnly = false;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
            _obtainedOrchestrionRollsOnly = false;
            _obtainedSecretRecipeBookOnly = false;
            _obtainedTripleTriadCardsOnly = false;
            _obtainedVistaOnly = false;
        }

        public void Clear()
        {
            Plugin.Log.Info("CollectionWindow, Clear() called");
            _currentCharacter = null;
            _obtainedBardingsOnly = false;
            _obtainedEmotesOnly = false;
            _obtainedOrnamentsOnly = false;
            _obtainedFacepaintsOnly = false;
            _obtainedFramerKitsOnly = false;
            _obtainedGlassesOnly = false;
            _obtainedHairstylesOnly = false;
            _obtainedMinionsOnly = false;
            _obtainedMountsOnly = false;
            _obtainedOrchestrionRollsOnly = false;
            _obtainedSecretRecipeBookOnly = false;
            _obtainedTripleTriadCardsOnly = false;
            _obtainedVistaOnly = false;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
            _obtainedBardingsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedEmotesOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedOrnamentsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedFacepaintsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedFramerKitsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedGlassesOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedHairstylesOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedMinionsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedMountsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedOrchestrionRollsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedSecretRecipeBookOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedTripleTriadCardsOnly = _plugin.Configuration.ObtainedOnly;
            _obtainedVistaOnly = _plugin.Configuration.ObtainedOnly;
            
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
                        if (chars.Count > 0)
                        {
                            /*if (ImGui.Selectable($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersCollectionTable#CharactersListBox#All", _currentCharacter == null))
                            {
                                _currentCharacter = null;
                            }*/

#if DEBUG
                            for (int i = 0; i < 15; i++)
                            {
                                chars.Add(new Character()
                                {
                                    FirstName = $"Dummy {i}",
                                    LastName = $"LN {i}",
                                    HomeWorld = $"Homeworld {i}",
                                });
                            }
#endif

                            foreach (Character currChar in chars.Where(currChar => ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == _currentCharacter)))
                            {
                                _currentCharacter = currChar;
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
            using var tabBar = ImRaii.TabBar("###CollectionWindow#Tabs");
            if (!tabBar.Success) return;

            using (var bardingsTab =
                   ImRaii.TabItem(
                       $"{Loc.Localize("Barding", "Barding")}###CollectionWindow#Tabs#Bardings")) // Harnisch Barding Barde バード
            {
                if (bardingsTab.Success)
                {
                    DrawBardings(currentCharacter);
                }
            }

            using (var emotesTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 780)}###CollectionWindow#Tabs#Emotes"))
            {
                if (emotesTab.Success)
                {
                    DrawEmotes(currentCharacter);
                }
            }

            using (var fashionAccessoriesTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 13671)}###CollectionWindow#Tabs#Fashion"))
            {
                if (fashionAccessoriesTab.Success)
                {
                    DrawOrnaments(currentCharacter);
                }
            }

            using (var facepaintTab =
                   ImRaii.TabItem($"{Loc.Localize("Facepaint", "Facepaint")}###CollectionWindow#Tabs#Facepaints"))
            {
                if (facepaintTab.Success)
                {
                    DrawFacepaints(currentCharacter);
                }
            }

            using (var framerKitTab =
                   ImRaii.TabItem(
                       $"{Loc.Localize("FramerKit", "Framer's kit")}###CollectionWindow#Tabs#Framerkits")) // Portraitmaterial[p] / Framer's kit / Portrait / ポートレート
            {
                if (framerKitTab.Success)
                {
                    DrawFramerKits(currentCharacter);
                }
            }

            using (var glassesTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16051)}###CollectionWindow#Tabs#Glasses"))
            {
                if (glassesTab.Success)
                {
                    DrawGlasses(currentCharacter);
                }
            }

            using (var hairsTab =
                   ImRaii.TabItem(
                       $"{Loc.Localize("Hairstyle", "Hairstyle")}###CollectionWindow#Tabs#Hairstyles")) // Frisur Hairstyle Coupe de cheveux 髪型
            {
                if (hairsTab.Success)
                {
                    DrawHairstyles(currentCharacter);
                }
            }

            using (var minionsTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8303)}###CollectionWindow#Tabs#Minions")) //Minions // 7595 for Katakana
            {
                if (minionsTab.Success)
                {
                    DrawMinions(currentCharacter);
                }
            }

            using (var mountsTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8302)}###CollectionWindow#Tabs#Mounts")) // Mounts // 13971 for Katakana
            {
                if (mountsTab.Success)
                {
                    DrawMounts(currentCharacter);
                }
            }

            using (var orchestrionsTab =
                   ImRaii.TabItem("Orchestrions###CollectionWindow#Tabs#Orchestrion")) // same name in all languages
            {
                if (orchestrionsTab.Success)
                {
                    DrawOrchestrionRolls(currentCharacter);
                }
            }

            using (var tomesTab = ImRaii.TabItem("Tomes###CollectionWindow#Tabs#Tomes"))
            {
                if (tomesTab.Success)
                {
                    DrawSecretRecipeBooks(currentCharacter);
                }
            }

            using (var tripleTriadTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 9529)}###CollectionWindow#Tabs#TTC")) //9991 for katakana
            {
                if (tripleTriadTab.Success)
                {
                    DrawTripleTriadCards(currentCharacter);
                }
            }

            using (var vistaTab =
                   ImRaii.TabItem("Vista###CollectionWindow#Tabs#Vista"))
            {
                if (vistaTab.Success)
                {
                    DrawVistas(currentCharacter);
                }
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8616)}");
            }
        }

        /**************************************************/
        /**********************Minions*********************/
        /**************************************************/
        private void DrawMinions(Character currentCharacter)
        {
            using var minionsTabTable = ImRaii.Table("###MinionsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!minionsTabTable) return;
            ImGui.TableSetupColumn($"###MinionsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###MinionsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchMinions, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

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
            ImGui.TableSetupColumn($"###MinionsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###MinionsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
            List<uint> minions = (_obtainedMinionsOnly) ? [..currentCharacter.Minions] : _globalCache.MinionStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchMinions))
            {
                minions = Utils.FilterBySearch(
                    minions,
                    _searchMinions,
                    _currentLocale,
                    _globalCache.MinionStorage.GetMinion,
                    minion => _currentLocale switch
                    {
                        ClientLanguage.German => minion.GermanName,
                        ClientLanguage.English => minion.EnglishName,
                        ClientLanguage.French => minion.FrenchName,
                        ClientLanguage.Japanese => minion.JapaneseName,
                        _ => minion.EnglishName
                    }
                );
            }
            int minionsCount = minions.Count;
            if (minionsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchMinions))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(minionsCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###MinionsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(575, height));
            if (!table) return;

            ImGui.TableSetupColumn("###MinionsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MinionsTable#Col10",
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
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
            ImGui.TableSetupColumn($"###MountsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###MountsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchMounts, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

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
            ImGui.TableSetupColumn($"###MountsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###MountsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
            List<uint> mounts = (_obtainedMountsOnly) ? [..currentCharacter.Mounts] : _globalCache.MountStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchMounts))
            {
                mounts = Utils.FilterBySearch(
                    mounts,
                    _searchMounts,
                    _currentLocale,
                    _globalCache.MountStorage.GetMount,
                    mount => _currentLocale switch
                    {
                        ClientLanguage.German => mount.GermanName,
                        ClientLanguage.English => mount.EnglishName,
                        ClientLanguage.French => mount.FrenchName,
                        ClientLanguage.Japanese => mount.JapaneseName,
                        _ => mount.EnglishName
                    }
                );
            }
            int mountsCount = mounts.Count;
            if (mountsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchMounts))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows =  (int)Math.Ceiling(mountsCount / (double)10);
            int height = rows * 48 + rows;

            using var mountTable = ImRaii.Table("###MountsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(573, height));
            if (!mountTable) return;

            ImGui.TableSetupColumn("###MountsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###MountsTable#Col10",
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
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
            ImGui.TableSetupColumn($"###TripleTriadCardsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###TripleTriadCardsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchTripleTriadCards, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

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
            ImGui.TableSetupColumn($"###TripleTriadCardsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 480);
            ImGui.TableSetupColumn($"###TripleTriadCardsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.TripleTriadCards.Count}/{_globalCache.TripleTriadCardStorage.Count()}");
        }

        private void DrawTripleTriadCardsCollection(Character currentCharacter)
        {
            List<uint> tripleTriadCards = (_obtainedTripleTriadCardsOnly) ? [..currentCharacter.TripleTriadCards] : _globalCache.TripleTriadCardStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchTripleTriadCards))
            {
                tripleTriadCards = Utils.FilterBySearch(
                    tripleTriadCards,
                    _searchTripleTriadCards,
                    _currentLocale,
                    _globalCache.TripleTriadCardStorage.GetTripleTriadCard,
                    card => _currentLocale switch
                    {
                        ClientLanguage.German => card.GermanName,
                        ClientLanguage.English => card.EnglishName,
                        ClientLanguage.French => card.FrenchName,
                        ClientLanguage.Japanese => card.JapaneseName,
                        _ => card.EnglishName
                    }
                );
            }
            int tripleTriadCardsCount = tripleTriadCards.Count;
            if (tripleTriadCardsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchTripleTriadCards))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(tripleTriadCardsCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###TripleTriadCardsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(575, height));
            if (!table) return;

            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###TripleTriadCardsTable#Col10",
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(ttc.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(ttc.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(060028), new Vector2(48, 48));
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
            ImGui.TableSetupColumn($"###EmotesTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###EmotesTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchEmotes, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

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
            ImGui.TableSetupColumn($"###EmotesTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 480);
            ImGui.TableSetupColumn($"###EmotesTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Emotes.Count}/{_globalCache.EmoteStorage.Count()}");
        }

        private void DrawEmotesCollection(Character currentCharacter)
        {
            List<uint> emotes = (_obtainedEmotesOnly) ? [..currentCharacter.Emotes] : _globalCache.EmoteStorage.Get();

            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchEmotes))
            {
                emotes = Utils.FilterBySearch(
                    emotes,
                    _searchEmotes,
                    _currentLocale,
                    _globalCache.EmoteStorage.GetEmote,
                    emote => _currentLocale switch
                    {
                        ClientLanguage.German => emote.GermanName,
                        ClientLanguage.English => emote.EnglishName,
                        ClientLanguage.French => emote.FrenchName,
                        ClientLanguage.Japanese => emote.JapaneseName,
                        _ => emote.EnglishName
                    }
                );
            }

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
            if (emoteCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchEmotes))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(emoteCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###EmotesTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(571, height));
            if (!table) return;

            ImGui.TableSetupColumn("###EmotesTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###EmotesTable#Col10",
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(emote.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(emote.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
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
            ImGui.TableSetupColumn($"###BardingsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###BardingsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchBardings, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

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
            ImGui.TableSetupColumn($"###BardingsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###BardingsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
            List<uint> bardings = (_obtainedBardingsOnly) ? [..currentCharacter.Bardings] : _globalCache.BardingStorage.Get();
            if (!string.IsNullOrWhiteSpace(_searchBardings))
            {
                bardings = Utils.FilterBySearch(
                    bardings,
                    _searchBardings,
                    _currentLocale,
                    _globalCache.BardingStorage.GetBarding,
                    barding => _currentLocale switch
                    {
                        ClientLanguage.German => barding.GermanName,
                        ClientLanguage.English => barding.EnglishName,
                        ClientLanguage.French => barding.FrenchName,
                        ClientLanguage.Japanese => barding.JapaneseName,
                        _ => barding.EnglishName
                    }
                );
            }
            int bardingsCount = bardings.Count;
            if (bardingsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchBardings))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(bardingsCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###BardingsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, height));
            if (!table) return;

            ImGui.TableSetupColumn("###BardingsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###BardingsTable#Col10",
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(b.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(b.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawBardingTooltip(_currentLocale, ref _globalCache, b);
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
            ImGui.TableSetupColumn($"###FramerKitsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###FramerKitsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchFramerKits, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

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
            ImGui.TableSetupColumn($"###FramerKitsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###FramerKitsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
            List<uint> framerKits = (_obtainedFramerKitsOnly) ? [..currentCharacter.FramerKits] : _globalCache.FramerKitStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchFramerKits))
            {
                framerKits = Utils.FilterBySearch(
                    framerKits,
                    _searchFramerKits,
                    _currentLocale,
                    _globalCache.FramerKitStorage.LoadItem,
                    kit => _currentLocale switch
                    {
                        ClientLanguage.German => kit.GermanName,
                        ClientLanguage.English => kit.EnglishName,
                        ClientLanguage.French => kit.FrenchName,
                        ClientLanguage.Japanese => kit.JapaneseName,
                        _ => kit.EnglishName
                    }
                );
            }
            int framerKitsCount = framerKits.Count;
            if (framerKitsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchFramerKits))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(framerKitsCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###FramerKitsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, height));
            if (!table) return;

            ImGui.TableSetupColumn("###FramerKitsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FramerKitsTable#Col10",
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, f);
                }

                i++;
            }
        }

        /**************************************************/
        /*****************OrchestrionRoll******************/
        /**************************************************/
        private void DrawOrchestrionRolls(Character currentCharacter)
        {
            using var orchestrionRollsTabTable = ImRaii.Table("###OrchestrionRollsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!orchestrionRollsTabTable) return;
            ImGui.TableSetupColumn($"###OrchestrionRollsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###OrchestrionRollsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchOrchestrionRolls, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedOrchestrionRollsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedOrchestrionRollsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawOrchestrionRollsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var orchestrionRollsTableAmount = ImRaii.Table("###OrchestrionRollsTableAmount", 2);
            if (!orchestrionRollsTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###OrchestrionRollsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###OrchestrionRollsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
                endStr += $"/{_globalCache.OrchestrionRollStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.OrchestrionRolls.Count}{endStr}");
        }

        private void DrawOrchestrionRollsCollection(Character currentCharacter)
        {
            List<uint> orchestrionRolls = (_obtainedOrchestrionRollsOnly) ? [..currentCharacter.OrchestrionRolls] : _globalCache.OrchestrionRollStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchOrchestrionRolls))
            {
                orchestrionRolls = Utils.FilterBySearch(
                    orchestrionRolls,
                    _searchOrchestrionRolls,
                    _currentLocale,
                    _globalCache.OrchestrionRollStorage.GetOrchestrionRoll,
                    roll => _currentLocale switch
                    {
                        ClientLanguage.German => roll.GermanName,
                        ClientLanguage.English => roll.EnglishName,
                        ClientLanguage.French => roll.FrenchName,
                        ClientLanguage.Japanese => roll.JapaneseName,
                        _ => roll.EnglishName
                    }
                );
            }
            
            int orchestrionRollsCount = orchestrionRolls.Count;
            if (orchestrionRollsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchOrchestrionRolls))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(orchestrionRollsCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###OrchestrionRollsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, height));
            if (!table) return;

            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrchestrionRollsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint fkId in orchestrionRolls)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                OrchestrionRoll? f = _globalCache.OrchestrionRollStorage.GetOrchestrionRoll(_currentLocale, fkId);
                if (f == null)
                {
                    continue;
                }

                if (currentCharacter.HasOrchestrionRoll(fkId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrchestrionRollTooltip(_currentLocale, ref _globalCache, f);
                }

                i++;
            }
        }

        /**************************************************/
        /**********************Ornaments**********************/
        /**************************************************/
        private void DrawOrnaments(Character currentCharacter)
        {
            using var ornamentsTabTable = ImRaii.Table("###OrnamentsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!ornamentsTabTable) return;
            ImGui.TableSetupColumn($"###OrnamentsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###OrnamentsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchOrnaments, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedOrnamentsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedOrnamentsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawOrnamentsCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var ornamentsTableOrnament = ImRaii.Table("###OrnamentsTableOrnament", 2);
            if (!ornamentsTableOrnament) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###OrnamentsTableOrnament#{currentCharacter.CharacterId}#Ornament#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###OrnamentsTableOrnament#{currentCharacter.CharacterId}#Ornament#Col2",
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
                endStr += $"/{_globalCache.OrnamentStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Ornaments.Count}{endStr}");
        }

        private void DrawOrnamentsCollection(Character currentCharacter)
        {
            List<uint> ornaments = (_obtainedOrnamentsOnly) ? [..currentCharacter.Ornaments] : _globalCache.OrnamentStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchOrnaments))
            {
                ornaments = Utils.FilterBySearch(
                    ornaments,
                    _searchOrnaments,
                    _currentLocale,
                    _globalCache.OrnamentStorage.GetOrnament,
                    ornament => _currentLocale switch
                    {
                        ClientLanguage.German => ornament.GermanName,
                        ClientLanguage.English => ornament.EnglishName,
                        ClientLanguage.French => ornament.FrenchName,
                        ClientLanguage.Japanese => ornament.JapaneseName,
                        _ => ornament.EnglishName
                    }
                );
            }
            int ornamentsCount = ornaments.Count;
            if (ornamentsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchOrnaments))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(ornamentsCount / (double)10);
            int height = rows * 48 + rows;

            using var ornamentTable = ImRaii.Table("###OrnamentsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(573, height));
            if (!ornamentTable) return;

            ImGui.TableSetupColumn("###OrnamentsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###OrnamentsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint ornamentId in ornaments)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Ornament? o = _globalCache.OrnamentStorage.GetOrnament(_currentLocale, ornamentId);
                if (o == null)
                {
                    continue;
                }

                if (currentCharacter.HasOrnament(ornamentId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(48, 48));
                }
                else
                {
                    //Plugin.Log.Debug($"Ornament {ornamentId} not found");
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrnamentTooltip(_currentLocale, ref _globalCache, o);
                }

                i++;
            }
        }

        /**************************************************/
        /*********************Glasses**********************/
        /**************************************************/
        private void DrawGlasses(Character currentCharacter)
        {
            using var glassesTabTable = ImRaii.Table("###GlassesTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!glassesTabTable) return;
            ImGui.TableSetupColumn($"###GlassesTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###GlassesTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchGlasses, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedGlassesOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedGlassesOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGlassesCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var glassesTableGlasses= ImRaii.Table("###GlassesTableAglasse", 2);
            if (!glassesTableGlasses) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###GlassesTableAglasse#{currentCharacter.CharacterId}#Aglasse#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###GlassesTableAglasse#{currentCharacter.CharacterId}#Aglasse#Col2",
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
                endStr += $"/{_globalCache.GlassesStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Glasses.Count}{endStr}");
        }

        private void DrawGlassesCollection(Character currentCharacter)
        {
            List<uint> glasses = (_obtainedGlassesOnly) ? [..currentCharacter.Glasses] : _globalCache.GlassesStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchGlasses))
            {
                glasses = Utils.FilterBySearch(
                    glasses,
                    _searchGlasses,
                    _currentLocale,
                    _globalCache.GlassesStorage.GetGlasses,
                    glass => _currentLocale switch
                    {
                        ClientLanguage.German => glass.GermanName,
                        ClientLanguage.English => glass.EnglishName,
                        ClientLanguage.French => glass.FrenchName,
                        ClientLanguage.Japanese => glass.JapaneseName,
                        _ => glass.EnglishName
                    }
                );
            }
            int glassesCount = glasses.Count;
            if (glassesCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchGlasses))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(glassesCount / (double)10);
            int height = rows * 48 + rows;

            using var glasseTable = ImRaii.Table("###GlassesTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(573, height));
            if (!glasseTable) return;

            ImGui.TableSetupColumn("###GlassesTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###GlassesTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint glasseId in glasses)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Glasses? g = _globalCache.GlassesStorage.GetGlasses(_currentLocale, glasseId);
                if (g == null)
                {
                    continue;
                }

                if (currentCharacter.HasGlasses(glasseId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(g.Icon), new Vector2(48, 48));
                }
                else
                {
                    //Plugin.Log.Debug($"Glasses {glasseId} not found");
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(g.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawGlassesTooltip(_currentLocale, ref _globalCache, g);
                }

                i++;
            }
        }

        /**************************************************/
        /********************Hairstyles********************/
        /**************************************************/
        private void DrawHairstyles(Character currentCharacter)
        {
            if (currentCharacter.Profile == null)
            {
                return;
            }

            List<uint> availableHairstyles =
                _globalCache.HairstyleStorage.GetAllHairstylesForTribeGender(currentCharacter.Profile.Tribe,
                    currentCharacter.Profile.Gender);

            using var hairstylesTabTable = ImRaii.Table("###HairstylesTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!hairstylesTabTable) return;
            ImGui.TableSetupColumn($"###HairstylesTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###HairstylesTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchHairstyles, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedHairstylesOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedHairstylesOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (currentCharacter.Hairstyles.Count == 0 && !_isSpoilerEnabled)
            {
                ImGui.TextUnformatted($"{Loc.Localize("NoPurchasableHairstylesUnlocked", "No purchasable hairstyle unlocked")}");
            }
            else
            {
                    DrawHairstylesCollection(currentCharacter, availableHairstyles);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var hairstylesTableAmount = ImRaii.Table("###HairstylesTableAmount", 2);
            if (!hairstylesTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###HairstylesTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###HairstylesTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
                endStr += $"/{availableHairstyles.Count}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Hairstyles.Count}{endStr}");
        }

        private void DrawHairstylesCollection(Character currentCharacter, List<uint> availableHairstyles)
        {
            //int startIndex = Utils.GetHairstyleIndexStart(currentCharacter);
            //List<uint> hairstyles = (_obtainedHairstylesOnly) ? [.. currentCharacter.Hairstyles.Where(i => i >= startIndex)] : _globalCache.HairstyleStorage.GetIdsFromStartIndex(startIndex);
            //List<uint> hairstyles = (_obtainedHairstylesOnly) ? [.. currentCharacter.Hairstyles] : _globalCache.HairstyleStorage.Get();
            if (currentCharacter.Profile == null)
            {
                return;
            }

            List<uint> hairstyles = (_obtainedHairstylesOnly) ? [.. currentCharacter.Hairstyles] : availableHairstyles;
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchHairstyles))
            {
                hairstyles = Utils.FilterBySearch(
                    hairstyles,
                    _searchHairstyles,
                    _currentLocale,
                    _globalCache.HairstyleStorage.GetHairstyle,
                    style => _currentLocale switch
                    {
                        ClientLanguage.German => style.GermanName,
                        ClientLanguage.English => style.EnglishName,
                        ClientLanguage.French => style.FrenchName,
                        ClientLanguage.Japanese => style.JapaneseName,
                        _ => style.EnglishName
                    }
                );
            }
            int hairstylesCount = hairstyles.Count;
            if (hairstylesCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchHairstyles))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(hairstylesCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###HairstylesTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(575, height));
            if (!table) return;

            ImGui.TableSetupColumn("###HairstylesTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###HairstylesTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint hairstyleId in hairstyles)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Hairstyle? h = _globalCache.HairstyleStorage.GetHairstyle(_currentLocale, hairstyleId);
                if (h is not { IsPurchasable: true })
                {
                    continue;
                }

                Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, h.ItemId);
                if (itm == null)
                {
                    continue;
                }

                if (currentCharacter.HasHairstyle(hairstyleId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(h.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(h.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(026178), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawHairstyleFacepaintTooltip(_currentLocale, ref _globalCache, h);
                }

                i++;
            }
        }

        /**************************************************/
        /********************Facepaints********************/
        /**************************************************/
        private void DrawFacepaints(Character currentCharacter)
        {
            if (currentCharacter.Profile == null)
            {
                return;
            }

            List<uint> availableFacepaints =
                _globalCache.HairstyleStorage.GetAllFacepaintsForTribeGender(currentCharacter.Profile.Tribe,
                    currentCharacter.Profile.Gender);

            using var facepaintsTabTable = ImRaii.Table("###facepaintsTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!facepaintsTabTable) return;
            ImGui.TableSetupColumn($"###facepaintsTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###FacepaintsTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchFacepaints, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedFacepaintsOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedFacepaintsOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (currentCharacter.Facepaints.Count == 0 && !_isSpoilerEnabled)
            {
                ImGui.TextUnformatted($"{Loc.Localize("NoPurchasableFacepaintsUnlocked", "No purchasable facepaint unlocked")}");
            }
            else
            {
                DrawFacepaintsCollection(currentCharacter, availableFacepaints);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var facepaintsTableAmount = ImRaii.Table("###FacepaintsTableAmount", 2);
            if (!facepaintsTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###FacepaintsTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###FacepaintsTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
                endStr += $"/{availableFacepaints.Count}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Facepaints.Count}{endStr}");
        }

        private void DrawFacepaintsCollection(Character currentCharacter, List<uint> availableFacepaints)
        {
            if (currentCharacter.Profile == null)
            {
                return;
            }

            List<uint> facepaints = (_obtainedFacepaintsOnly) ? [.. currentCharacter.Facepaints] : availableFacepaints;
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchFacepaints))
            {
                facepaints = Utils.FilterBySearch(
                    facepaints,
                    _searchFacepaints,
                    _currentLocale,
                    _globalCache.HairstyleStorage.GetHairstyle,
                    paint => _currentLocale switch
                    {
                        ClientLanguage.German => paint.GermanName,
                        ClientLanguage.English => paint.EnglishName,
                        ClientLanguage.French => paint.FrenchName,
                        ClientLanguage.Japanese => paint.JapaneseName,
                        _ => paint.EnglishName
                    }
                );
            }
            int facepaintsCount = facepaints.Count;
            if (facepaintsCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchFacepaints))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(facepaintsCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###FacepaintsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(575, height));
            if (!table) return;

            ImGui.TableSetupColumn("###FacepaintsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###FacepaintsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint facepaintId in facepaints)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Hairstyle? h = _globalCache.HairstyleStorage.GetHairstyle(_currentLocale, facepaintId);
                if (h is not { IsPurchasable: true })
                {
                    continue;
                }

                Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, h.ItemId);
                if (itm == null)
                {
                    continue;
                }

                if (currentCharacter.HasFacepaint(facepaintId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(h.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(h.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(026178), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawHairstyleFacepaintTooltip(_currentLocale, ref _globalCache, h);
                }

                i++;
            }
        }

        /**************************************************/
        /*****************SecretRecipeBook******************/
        /**************************************************/
        private void DrawSecretRecipeBooks(Character currentCharacter)
        {
            using var SecretRecipeBooksTabTable = ImRaii.Table("###SecretRecipeBooksTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!SecretRecipeBooksTabTable) return;
            ImGui.TableSetupColumn($"###SecretRecipeBooksTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###SecretRecipeBooksTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchSecretRecipeBooks, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedSecretRecipeBookOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedSecretRecipeBookOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawSecretRecipeBooksCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var secretRecipeBooksTableAmount = ImRaii.Table("###SecretRecipeBooksTableAmount", 2);
            if (!secretRecipeBooksTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###SecretRecipeBooksTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###SecretRecipeBooksTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
                endStr += $"/{_globalCache.SecretRecipeBookStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.SecretRecipeBooks.Count}{endStr}");
        }

        private void DrawSecretRecipeBooksCollection(Character currentCharacter)
        {
            List<uint> secretRecipeBooks = (_obtainedSecretRecipeBookOnly) ? [.. currentCharacter.SecretRecipeBooks] : _globalCache.SecretRecipeBookStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchSecretRecipeBooks))
            {
                secretRecipeBooks = Utils.FilterBySearch(
                    secretRecipeBooks,
                    _searchSecretRecipeBooks,
                    _currentLocale,
                    _globalCache.SecretRecipeBookStorage.GetSecretRecipeBook,
                    book => _currentLocale switch
                    {
                        ClientLanguage.German => book.GermanName,
                        ClientLanguage.English => book.EnglishName,
                        ClientLanguage.French => book.FrenchName,
                        ClientLanguage.Japanese => book.JapaneseName,
                        _ => book.EnglishName
                    }
                );
            }
            int secretRecipeBooksCount = secretRecipeBooks.Count;
            if (secretRecipeBooksCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchSecretRecipeBooks))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(secretRecipeBooksCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###SecretRecipeBooksTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, height));
            if (!table) return;

            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###SecretRecipeBooksTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint fkId in secretRecipeBooks)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Models.SecretRecipeBook? srb = _globalCache.SecretRecipeBookStorage.GetSecretRecipeBook(_currentLocale, fkId);
                if (srb == null)
                {
                    continue;
                }

                if (currentCharacter.HasSecretRecipeBook(fkId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(srb.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(srb.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawSecretRecipeBookTooltip(_currentLocale, ref _globalCache, srb);
                }

                i++;
            }
        }

        /**************************************************/
        /**********************Vista***********************/
        /**************************************************/
        private void DrawVistas(Character currentCharacter)
        {
            switch (currentCharacter.SightseeingLogUnlockState)
            {
                case null:
                    ImGui.TextUnformatted($"{Loc.Localize("CharacterNotScannedYet", "This character was not scanned yet")}");
                    return;
                case 0:
                    ImGui.TextUnformatted($"{Loc.Localize("VistaNotUnlocked", "Sightseeing Log not unlocked")}");
                    return;
            }

            using var vistasTabTable = ImRaii.Table("###VistasTabTable", 1, ImGuiTableFlags.ScrollY);
            if (!vistasTabTable) return;
            ImGui.TableSetupColumn($"###VistasTabTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            // Set width for search input
            ImGui.SetNextItemWidth(300);
            ImGui.InputTextWithHint("###VistasTab#Search", _globalCache.AddonStorage.LoadAddonString(_currentLocale, 3386), ref _searchVistas, 100);
            // Set spacing between input and checkbox
            ImGui.SameLine(350);

            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedVistaOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedVistaOnly;
                _plugin.Configuration.Save();
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawVistasCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var vistasTableAmount = ImRaii.Table("###VistasTableAmount", 2);
            if (!vistasTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }
            ImGui.TableSetupColumn($"###VistasTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###VistasTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
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
                endStr += $"/{_globalCache.VistaStorage.Count()}";
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Vistas.Count}{endStr}");
        }

        private void DrawVistasCollection(Character currentCharacter)
        {
            List<uint> vistas = (_obtainedVistaOnly) ? [.. currentCharacter.Vistas] : _globalCache.VistaStorage.Get();
            // Filter by search string if provided
            if (!string.IsNullOrWhiteSpace(_searchVistas))
            {
                vistas = Utils.FilterBySearch(
                    vistas,
                    _searchVistas,
                    _currentLocale,
                    _globalCache.VistaStorage.GetVista,
                    vista => _currentLocale switch
                    {
                        ClientLanguage.German => vista.GermanName,
                        ClientLanguage.English => vista.EnglishName,
                        ClientLanguage.French => vista.FrenchName,
                        ClientLanguage.Japanese => vista.JapaneseName,
                        _ => vista.EnglishName
                    }
                );
            }
            int vistasCount = vistas.Count;
            if (vistasCount == 0)
            {
                if (!string.IsNullOrWhiteSpace(_searchVistas))
                {
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5494));
                }
                return;
            }
            int rows = (int)Math.Ceiling(vistasCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###VistasTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(572, height));
            if (!table) return;

            ImGui.TableSetupColumn("###VistasTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###VistasTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint fkId in vistas)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Vista? v = _globalCache.VistaStorage.GetVista(_currentLocale, fkId);
                if (v == null)
                {
                    continue;
                }

                bool hasVista = currentCharacter.HasVista(fkId);
                if (hasVista)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon((uint)v.IconDiscovered), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon((uint)v.IconList), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon((uint)v.IconUndiscovered), new Vector2(48, 48));
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawVistaTooltip(_currentLocale, ref _globalCache, v, hasVista, _isSpoilerEnabled);
                }

                i++;
            }
        }
    }
}