using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Emote = Altoholic.Models.Emote;
using Mount = Altoholic.Models.Mount;
using Ornament = Altoholic.Models.Ornament;
using TripleTriadCard = Altoholic.Models.TripleTriadCard;

namespace Altoholic.Windows
{
    public class ProgressWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;
        private bool _isSpoilerEnabled;

        public ProgressWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        )
            : base(
                name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;

            _commendationIcon = Plugin.TextureProvider.GetFromGame("ui/uld/Character_hr1.tex").RentAsync().Result;
            _chevronTexture = Plugin.TextureProvider.GetFromGame("ui/uld/img03/ListItemB_hr1.tex").RentAsync().Result;

            _currentLocale = _plugin.Configuration.Language;
            _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752);
            _rolesTextureWrap = _globalCache.IconStorage.LoadRoleIconTexture();
        }

        public Func<Character> GetPlayer { get; set; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;

        private string _selectedExpansion;

        private readonly IDalamudTextureWrap? _commendationIcon;
        private readonly IDalamudTextureWrap? _chevronTexture;
        private IDalamudTextureWrap? _rolesTextureWrap;

        private bool _rightChevron = true;
        private bool _downChevron;
        private bool _hasValueBeenSelected;
        private int _currentOldMoogleReward;

        public override void OnClose()
        {
        }

        public void Dispose()
        {
            _currentCharacter = null;
            _selectedExpansion = string.Empty;
            _commendationIcon?.Dispose();
            _chevronTexture?.Dispose();
        }

        public void Clear()
        {
            Plugin.Log.Debug("ProgressWindow Clear() called");
            _currentCharacter = null;
            _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752);
            _currentOldMoogleReward = 0;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using ImRaii.IEndObject table = ImRaii.Table("###CharactersProgressTable", 2);
                if (!table) return;

                ImGui.TableSetupColumn("###CharactersProgressTable#CharactersListHeader",
                    ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn("###CharactersProgressTable#ProgressTabs", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (ImRaii.IEndObject listBox =
                       ImRaii.ListBox("###CharactersProgressTable#CharactersListBox", new Vector2(200, -1)))
                {
                    if (listBox)
                    {
                        if (ImGui.Selectable(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersCurrenciesTable#CharactersListBox#All",
                                _currentCharacter == null))
                        {
                            _currentCharacter = null;
                        }
#if DEBUG
                        for (int i = 0; i < 15; i++)
                        {
                            chars.Add(new Character()
                            {
                                FirstName = $"Dummy {i}",
                                LastName = $"LN {i}",
                                HomeWorld = $"Homeworld {i}",
                                Datacenter = $"EU",
                                FCTag = $"FC {i}",
                                Currencies = new PlayerCurrencies() { Gil = i },
                            });
                        }
#endif

                        foreach (Character currChar in chars.Where(currChar =>
                                     ImGui.Selectable(
                                         $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                         currChar == _currentCharacter)))
                        {
                            _currentCharacter = currChar;
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
                Plugin.Log.Debug("Altoholic CharactersProgressTable Exception : {0}", e);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#TabBar");
            if (!tab) return;

            using (var cosmicExplorationTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3849)}###CharactersProgressTable#All#TabBar#CosmicExploration"))
            {
                if (cosmicExplorationTab)
                {
                    DrawCosmicExploration(chars);
                }
            }
            using (var dutyTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2225)}###CharactersProgressTable#All#TabBar#Duty"))
            {
                if (dutyTab)
                {
                    DrawDuties(chars);
                }
            }
            using (var eventTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 665)}###CharactersProgressTable#All#TabBar#Events"))
            {
                if (eventTab)
                {
                    DrawEventQuest(chars);
                }
            }

            using (var hildibrandTab =
                   ImRaii.TabItem(
                       $"{(_currentLocale == ClientLanguage.Japanese ? "ヒルディブランド" : "Hildibrand")}###CharactersProgressTable#All#TabBar#Hildibrand"))
            {
                if (hildibrandTab)
                {
                    DrawHildibrandQuest(chars);
                }
            }

            using (var islandSanctuaryTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 15101)}###CharactersProgressTable#All#TabBar#IslandSanctuary"))
            {
                if (islandSanctuaryTab)
                {
                    DrawIslandSanctuaryRewards(chars);
                }
            }

            using (var msqTab = ImRaii.TabItem("MSQ###CharactersProgressTable#All#TabBar#MSQ"))
            {
                if (msqTab)
                {
                    DrawMainScenarioQuest(chars);
                }
            }

            using (var tribeTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102512)}###CharactersProgressTable#All#TabBar#Tribes"))
            {
                if (tribeTab)
                {
                    DrawTribes(chars);
                }
            }

            string rqText = _currentLocale switch
            {
                ClientLanguage.German => "Rollenauftrag",
                ClientLanguage.English => "Role quest",
                ClientLanguage.French => "Quête de rôle",
                ClientLanguage.Japanese => "ロールクエスト",
                _ => "Role quest",
            };
            using (var rolequestTab =
                   ImRaii.TabItem(
                       $"{rqText}###CharactersProgressTable#All#TabBar#RoleQuest"))
            {
                if (rolequestTab)
                {
                    DrawRoleQuestQuest(chars);
                }
            }
        }

        private void DrawIslandSanctuaryRewards(List<Character> chars)
        {
            using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#Event#IslandSanctuaryRewards#Table",
                chars.Count + 2,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#IslandSanctuaryRewards#Name",
                ImGuiTableColumnFlags.WidthFixed, 260);
            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#IslandSanctuaryRewards#Currency",
                ImGuiTableColumnFlags.WidthFixed, 40);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#IslandSanctuaryRewards#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

            ImGui.TableSetColumnIndex(1);
            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)Currencies.SEAFARERS_COWRIE);
            if (itm == null) return;
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
            if (ImGui.IsItemHovered())
            {
                Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14252).Replace(":", ""));
            ImGui.TableSetColumnIndex(1);
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{(currChar.IslandSanctuaryUnlocked ? currChar.IslandSanctuaryLevel : "")}");
            }


            DrawAllCharsMount(chars, 277, 24000);
            DrawAllCharsMount(chars, 286, 35000);
            DrawAllCharsMount(chars, 282, 50000);
            DrawAllCharsMount(chars, 335, 100000);
            DrawAllCharsMount(chars, 255, 12000);
            DrawAllCharsMount(chars, 256, 12000);
            DrawAllCharsMount(chars, 257, 12000);
            DrawAllCharsMount(chars, 258, 18000);
            DrawAllCharsMount(chars, 259, 18000);
            DrawAllCharsMount(chars, 260, 18000);
            DrawAllCharsMinion(chars, 456, 4000);
            DrawAllCharsMinion(chars, 468, 4000);
            DrawAllCharsMinion(chars, 481, 4000);
            DrawAllCharsMinion(chars, 496, 4000);
            DrawAllCharsOrnament(chars, 30, 6000);
            DrawAllCharsOrnament(chars, 34, 6000);
            DrawAllCharsOrnament(chars, 38, 6000);
            DrawAllCharsOrnament(chars, 28, 6000);
            DrawAllCharsHairstyle(chars, 38442, 6000);
            DrawAllCharsHairstyle(chars, 38443, 6000);
            DrawAllCharsOrchestrion(chars, 544, 4000);
            DrawAllCharsOrchestrion(chars, 545, 4000);
            DrawAllCharsOrchestrion(chars, 593, 4000);
            DrawAllCharsTripleTriadCard(chars, 370, 1000);
            DrawAllCharsBarding(chars, 89, 4000);
            DrawAllCharsBarding(chars, 93, 4000);
            DrawAllCharsFramerKit(chars, 39578, 3000);
            DrawAllCharsFramerKit(chars, 39579, 3000);
            DrawAllCharsFramerKit(chars, 46822, 3000);
        }

        private void DrawCosmicExploration(List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#TabBar#CosmicExploration");
            if (!tab) return;

            using (var cosmicExplorationTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3460)}###CharactersProgressTable#All#TabBar#CosmicExploration#Vendor"))
            {
                if (cosmicExplorationTab)
                {
                    DrawCosmicExplorationVendor(chars);
                }
            }
            using (var cosmicExplorationShuffleTab =
                   ImRaii.TabItem(
                       $"Cosmic Fortunes###CharactersProgressTable#All#TabBar#CosmicExploration#Shuffle"))
            {
                if (cosmicExplorationShuffleTab)
                {
                    DrawCosmicExplorationShuffle(chars);
                }
            }
        }
        private void DrawCosmicExplorationVendor(List<Character> chars)
        {
            using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#Event#CosmicExplorationRewards#Table",
                chars.Count + 2,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#Name",
                ImGuiTableColumnFlags.WidthFixed, 260);
            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#Currency",
                ImGuiTableColumnFlags.WidthFixed, 33);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

            ImGui.TableSetColumnIndex(1);
            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)Currencies.COSMOCREDIT);
            if (itm == null) return;
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
            if (ImGui.IsItemHovered())
            {
                Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            DrawAllCharsMount(chars, 401, 29000);
            DrawAllCharsFramerKit(chars, 48091, 6000);
            DrawAllCharsFramerKit(chars, 46768, 6000);
            DrawAllCharsTripleTriadCard(chars, 449, 4000);
            DrawAllCharsTripleTriadCard(chars, 450, 6000);
            DrawAllCharsEmote(chars, 294, 9600);
            DrawAllCharsFacewear(chars, 289, 6000);
            DrawAllCharsFacewear(chars, 373, 3000);
            DrawAllCharsFacewear(chars, 385, 3000);
            DrawAllCharsOrchestrion(chars, 737, 6000);
            DrawAllCharsOrchestrion(chars, 738, 6000);
            DrawAllCharsOrchestrion(chars, 777, 6000);
        }
        private void DrawCosmicExplorationShuffle(List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#TabBar#CosmicExplorationShuffle");
            if (!tab) return;

            using (var cosmicExplorationShuffleSinusArdorumTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16780)}###CharactersProgressTable#All#TabBar#CosmicExploration#Shuffle#SinusArdorum"))
            {
                if (cosmicExplorationShuffleSinusArdorumTab)
                {
                    using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#Event#CosmicExplorationRewards#Table",
                chars.Count + 2,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#Currency",
                        ImGuiTableColumnFlags.WidthFixed, 25);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                    /*ImGui.TableSetColumnIndex(1);
                    Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)Currencies.COSMOCREDIT);
                    if (itm == null) return;
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                    }*/

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllCharsFacewear(chars, 301, 0);
                    DrawAllCharsOrnament(chars, 47, 0);
                    DrawAllCharsMount(chars, 364, 0);
                    DrawAllCharsMinion(chars, 547, 0);
                    DrawAllCharsEmote(chars, 286, 0);
                    DrawAllCharsOrchestrion(chars, 745, 0);
                }
            }
            using (var cosmicExplorationShufflePhaennaTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16904)}###CharactersProgressTable#All#TabBar#CosmicExploration#Shuffle#Phaenna"))
            {
                if (cosmicExplorationShufflePhaennaTab)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#CosmicExplorationRewards#Table",
                        chars.Count + 2,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#Currency",
                        ImGuiTableColumnFlags.WidthFixed, 25);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#CosmicExplorationRewards#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                    /*ImGui.TableSetColumnIndex(1);
                    Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)Currencies.COSMOCREDIT);
                    if (itm == null) return;
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                    }*/

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllCharsMount(chars, 386, 0);
                    DrawAllCharsMinion(chars, 553, 0);
                    DrawAllCharsOrchestrion(chars, 776, 0);
                    DrawAllCharsEmote(chars, 304, 0);
                    DrawAllCharsFacewear(chars, 397, 0);
                }
            }
        }

        private void DrawDuties(List<Character> chars)
        {
            //List<KeyValuePair<int, bool>> characterDuties = Utils.GetCharacterDuties(chars);
            List<Duty> duties = _globalCache.DutyStorage.GetAll();

            string savage = _currentLocale switch
            {
                ClientLanguage.German => "episch",
                ClientLanguage.English => "Savage",
                ClientLanguage.French => "sadique",
                ClientLanguage.Japanese => "零式",
                _ => "Savage"
            };
            List<string> dutyNames =
            [
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 15403), //Criterion
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 15402), //Variant
                "Ultimate", //Ultimate
                savage //Savage
            ];


            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#TabBar#Duty#TabBar");
            if (!tab) return;

            using (var dungeonTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8335)}###CharactersProgressTable#All#TabBar#Duty#TabBar#Dungeons"))
            {
                if (dungeonTab)
                {
                    DrawDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8335),
                        duties.FindAll(d => d.ContentTypeId == 2), chars); //Dungeon
                }
            }

            using (var trialTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8608)}###CharactersProgressTable#All#TabBar#Duty#TabBar#Trials"))
            {
                if (trialTab)
                {
                    DrawDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8608),
                        duties.FindAll(d => d.ContentTypeId == 4), chars); ////Trials
                }
            }

            using (var raidTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8609)}###CharactersProgressTable#All#TabBar#Duty#TabBar#Raids"))
            {
                if (raidTab)
                {
                    DrawDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8609),
                        duties.FindAll(d => d.ContentTypeId == 5), chars); //Raids
                }
            }

            using (var deepDungeonsTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2304)}###CharactersProgressTable#All#TabBar#Duty#TabBar#DeepDungeon"))
            {
                if (deepDungeonsTab)
                {
                    DrawDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2304),
                        duties.FindAll(d => d.ContentTypeId == 21), chars); //Deep Dungeons
                }
            }

            using (var guildhestsTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3165)}###CharactersProgressTable#All#TabBar#Duty#TabBar#Guildhests"))
            {
                if (guildhestsTab)
                {
                    DrawSpecialDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3165),
                        duties.FindAll(d => d.ContentTypeId == 3), chars); //Guildhests
                }
            }

            using (var thTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2276)}###CharactersProgressTable#All#TabBar#Duty#TabBar#TreasureHunts"))
            {
                if (thTab)
                {
                    DrawSpecialDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2276),
                        duties.FindAll(d => d.ContentTypeId == 9), chars); //Treasure Hunts
                }
            }

            using (var ultimateTab =
                   ImRaii.TabItem(
                       $"Ultimate###CharactersProgressTable#All#TabBar#Duty#TabBar#Ultimate"))
            {
                if (ultimateTab)
                {
                    DrawSpecialDuty("Ultimate",
                        duties.FindAll(d => d.ContentTypeId == 28), chars); //Ultimate
                }
            }

            using (var vcTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 15400)}###CharactersProgressTable#All#TabBar#Duty#TabBar#VC"))
            {
                if (vcTab)
                {
                    DrawSpecialDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 15400),
                        duties.FindAll(d => d.ContentTypeId == 30), chars); //V&C Dungeon Finder
                }
            }

            string chaotic = _currentLocale switch
            {
                ClientLanguage.German => "Chaotisch",
                ClientLanguage.English => "Chaotic",
                ClientLanguage.French => "Chaotique",
                ClientLanguage.Japanese => "滅",
                _ => "Chaotic"
            };
            using (var chaoticTab =
                   ImRaii.TabItem(
                       $"{chaotic}###CharactersProgressTable#All#TabBar#Duty#TabBar#Chaotic"))
            {
                if (chaoticTab)
                {
                    DrawSpecialDuty(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 15400),
                        duties.FindAll(d => d.ContentTypeId == 37), chars); //Chaotic
                }
            }
        }

        private void DrawDuty(string duName, List<Duty> duties, List<Character> chars)
        {
            string[] expNames =
            [
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)
            ];
            foreach (var ex in expNames.Select((value, i) => new { i, value }))
            {
                List<Duty> expDuties = duties.FindAll(d => d.ExVersion == ex.i).OrderBy(d => d.SortKey).ToList();
                if (expDuties.Count == 0) continue;
                if (ImGui.CollapsingHeader($"{ex.value}###Exp{ex.i}"))
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Duty#{duName}_{ex.i}#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Duty#{duName}_{ex.i}#Table#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn(
                            $"###CharactersProgress#All#Duty#{duName}_{ex.i}#Table#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }
                    ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2225));
                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    foreach (Duty expDuty in expDuties)
                    {
                        if (expDuty.Id is 70 or 71) continue;
                        DrawAllDutyLine(chars, expDuty);
                    }
                    //FindByExp => FindByDutyType => OrderBySortKey
                }

            }
        }

        private void DrawSpecialDuty(string duName, List<Duty> duties, List<Character> chars)
        {
            string[] expNames =
            [
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160),
                _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)
            ];
            foreach (var ex in expNames.Select((value, i) => new { i, value }))
            {
                List<Duty> expDuties = duties.OrderBy(d => d.SortKey).ToList();
                if (expDuties.Count == 0) continue;

                using var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Duty#{duName}_{ex.i}#Table",
                    chars.Count + 1,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                if (!charactersEventTable) return;
                ImGui.TableSetupColumn($"###CharactersProgress#All#Duty#{duName}_{ex.i}#Table#Name",
                    ImGuiTableColumnFlags.WidthFixed, 260);
                foreach (Character c in chars)
                {
                    ImGui.TableSetupColumn(
                        $"###CharactersProgress#All#Duty#{duName}_{ex.i}#Table#{c.CharacterId}",
                        ImGuiTableColumnFlags.WidthFixed, 20);
                }

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2225));
                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }

                foreach (Duty expDuty in expDuties)
                {
                    if (expDuty.Id is 70 or 71) continue;
                    DrawAllDutyLine(chars, expDuty);
                }
                //FindByExp => FindByDutyType => OrderBySortKey
            }
        }

        private void DrawAllDutyLine(List<Character> chars, Duty expDuty)
        {
            string name = _currentLocale switch
            {
                ClientLanguage.German => expDuty.GermanName,
                ClientLanguage.English => expDuty.EnglishName,
                ClientLanguage.French => expDuty.FrenchName,
                ClientLanguage.Japanese => expDuty.JapaneseName,
                _ => expDuty.EnglishName
            };
            //Plugin.Log.Debug($"DrawAllDutyLine: {chars.Count}, name: {expDuty.EnglishName}, msqIndex: {expDuty.Id}");
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(Utils.Capitalize(name));

            foreach (Character character in chars)
            {
                bool completed = character.IsDutyCompleted(expDuty.Id);
                bool unlocked = character.IsDutyUnlocked(expDuty.Id);
                string completedStr = _currentLocale switch
                {
                    ClientLanguage.German => "Vollendet",
                    ClientLanguage.English => "Completed",
                    ClientLanguage.French => "Complété",
                    ClientLanguage.Japanese => "終了",
                    _ => "Completed"
                };
                string unlockedStr = _currentLocale switch
                {
                    ClientLanguage.German => "Entsperrt",
                    ClientLanguage.English => "Unlocked",
                    ClientLanguage.French => "Débloqué",
                    ClientLanguage.Japanese => "開放",
                    _ => "Unlocked"
                };
                ImGui.TableNextColumn();
                //ImGui.TextUnformatted(completed ? "\u2713" : unlocked ? "\u25ef" : "");
                // Todo: Use line below on weird non default font
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(completed ? FontAwesomeIcon.Check.ToIconString() : unlocked ? FontAwesomeIcon.Circle.ToIconString() : "");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(Utils.Capitalize(name));
                    if (completed || unlocked)
                    {
                        ImGui.TextUnformatted(completed ? completedStr :
                            unlocked ? unlockedStr : "");
                    }

                    ImGui.TextUnformatted(
                        $"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }

        private void DrawMainScenarioQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var charactersMainScenarioQuestAll = ImRaii.Table("###CharactersProgress#All#MSQ", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersMainScenarioQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#MSQ#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#MSQ#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }
            ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                ImGui.EndTooltip();
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            List<List<bool>> charactersQuests = Utils.GetCharactersMainScenarioQuests(chars);
            DrawAllLine(chars, charactersQuests,
                $"2.0 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_A_REALM_REBORN)}",
                0);
            DrawAllLine(chars, charactersQuests,
                $"2.1 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_A_REALM_AWOKEN)}",
                1);
            DrawAllLine(chars, charactersQuests,
                $"2.2 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THROUGH_THE_MAELSTROM)}",
                2);
            DrawAllLine(chars, charactersQuests,
                $"2.3 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_DEFENDERS_OF_EORZEA)}",
                3);
            DrawAllLine(chars, charactersQuests,
                $"2.4 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_DREAMS_OF_ICE)}",
                4);
            DrawAllLine(chars, charactersQuests,
                $"2.5 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_BEFORE_THE_FALL_PART_1)}",
                5);
            DrawAllLine(chars, charactersQuests,
                $"2.55 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_BEFORE_THE_FALL_PART_2)}",
                6);
            DrawAllLine(chars, charactersQuests,
                $"3.0 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_HEAVENSWARD)}",
                7);
            DrawAllLine(chars, charactersQuests,
                $"3.1 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_AS_GOES_LIGHT_SO_GOES_DARKNESS)}",
                8);
            DrawAllLine(chars, charactersQuests,
                $"3.2 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_GEARS_OF_CHANGE)}",
                9);
            DrawAllLine(chars, charactersQuests,
                $"3.3 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_REVENGE_OF_THE_HORDE)}",
                10);
            DrawAllLine(chars, charactersQuests,
                $"3.4 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_SOUL_SURRENDER)}",
                11);
            DrawAllLine(chars, charactersQuests,
                $"3.5 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_FAR_EDGE_OF_FATE_PART_1)}",
                12);
            DrawAllLine(chars, charactersQuests,
                $"3.56 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_FAR_EDGE_OF_FATE_PART_2)}",
                13);
            DrawAllLine(chars, charactersQuests,
                $"4.0 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_STORMBLOOD)}",
                14);
            DrawAllLine(chars, charactersQuests,
                $"4.1 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_LEGEND_RETURNS)}",
                15);
            DrawAllLine(chars, charactersQuests,
                $"4.2 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_RISE_OF_A_NEW_SUN)}",
                16);
            DrawAllLine(chars, charactersQuests,
                $"4.3 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_UNDER_THE_MOONLIGHT)}",
                17);
            DrawAllLine(chars, charactersQuests,
                $"4.4 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_PRELUDE_IN_VIOLET)}",
                18);
            DrawAllLine(chars, charactersQuests,
                $"4.5 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_A_REQUIEM_FOR_HEROES_PART_1)}",
                19);
            DrawAllLine(chars, charactersQuests,
                $"4.56 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_A_REQUIEM_FOR_HEROES_PART_2)}",
                20);
            DrawAllLine(chars, charactersQuests,
                $"5.0 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_SHADOWBRINGER)}",
                21);
            DrawAllLine(chars, charactersQuests,
                $"5.1 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_VOWS_OF_VIRTUE_DEEDS_OF_CRUELTY)}",
                22);
            DrawAllLine(chars, charactersQuests,
                $"5.2 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_ECHOES_OF_A_FALLEN_STAR)}",
                23);
            DrawAllLine(chars, charactersQuests,
                $"5.3 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_REFLECTIONS_IN_CRYSTAL)}",
                24);
            DrawAllLine(chars, charactersQuests,
                $"5.4 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_FUTURES_REWRITTEN)}",
                25);
            DrawAllLine(chars, charactersQuests,
                $"5.5 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_DEATH_UNTO_DAWN_PART_1)}",
                26);
            DrawAllLine(chars, charactersQuests,
                $"5.55 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_DEATH_UNTO_DAWN_PART_2)}",
                27);
            DrawAllLine(chars, charactersQuests,
                $"6.0 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_ENDWALKER)}",
                28);
            DrawAllLine(chars, charactersQuests,
                $"6.1 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_NEWFOUND_ADVENTURE)}",
                29);
            DrawAllLine(chars, charactersQuests,
                $"6.2 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_BURIED_MEMORY)}",
                30);
            DrawAllLine(chars, charactersQuests,
                $"6.3 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_GODS_REVEL_LANDS_TREMBLE)}",
                31);
            DrawAllLine(chars, charactersQuests,
                $"6.4 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_DARK_THRONE)}",
                32);
            DrawAllLine(chars, charactersQuests,
                $"6.5 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_GROWING_LIGHT_PART_1)}",
                33);
            DrawAllLine(chars, charactersQuests,
                $"6.55 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_GROWING_LIGHT_PART_2)}",
                34);
            DrawAllLine(chars, charactersQuests,
                $"7.0 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_DAWNTRAIL)}",
                35);
            DrawAllLine(chars, charactersQuests,
                $"7.1 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_CROSSROADS)}",
                36);
            DrawAllLine(chars, charactersQuests,
                $"7.2 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_SEEKERS_OF_ETERNITY)}",
                37);
            DrawAllLine(chars, charactersQuests,
                $"7.3 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_PROMISE_OF_TOMORROW)}",
                38);
            DrawAllLine(chars, charactersQuests,
                $"7.4 - {_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.MSQ_THE_MIST)}",
                39);
        }

        private void DrawEventQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            List<List<bool>> charactersQuests = Utils.GetCharactersEventsQuests(chars);
            ImGui.TextUnformatted($"{Loc.Localize("RecurringEvent",
                "* As certain event do not change when reoccuring, completing them once will mark all of them done.")}");

            ImGui.TextUnformatted($"{Loc.Localize("Blunderville",
                "** For the Blunderville event, the introduction quest is used for completion.")}");

            using var tabBar = ImRaii.TabBar("###progressEvent#Tabs", ImGuiTabBarFlags.Reorderable);
            if (!tabBar.Success) return;
            //Plugin.Log.Debug($"charactersEventQuests: {charactersQuests.Count}");
            /*DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2010)", 0);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2011)", 1);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2011)", 2);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2011)", 3);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2011)", 4);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_FirefallFaire", "Firefall Faire")} (2011)", 5);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_HuntersMoon", "Hunter's Moon")} (2011)", 6);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_FoundationDay", "Foundation Day")} (2011)", 7);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2011) *", 8);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2011)", 9);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2012)", 10);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2012)", 11);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2012)", 12);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2012)", 13);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2012)", 14);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_FoundationDay", "Foundation Day")} (2012)", 15);
            DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2013)", 16);*/

            using (var progressEvent2025Tab = ImRaii.TabItem("2026###progressEvent#Tabs#2026"))
            {
                if (progressEvent2025Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2026#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 335));
                    if (!charactersEventTable) return;

                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2026#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2026#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2026)",
                        131);
                }
            }

            using (var progressEvent2025Tab = ImRaii.TabItem("2025###progressEvent#Tabs#2025"))
            {
                if (progressEvent2025Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2025#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 335));
                    if (!charactersEventTable) return;

                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2025#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2025#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2025)",
                        122);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2025)",
                        123);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2025)",
                        124);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2025)",
                        125);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2025)",
                        126);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2025)",
                        127);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2025)",
                        128);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2025)",
                        129);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2025)",
                        130);
                }
            }

            using (var progressEvent2024Tab = ImRaii.TabItem("2024###progressEvent#Tabs#2024"))
            {
                if (progressEvent2024Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2024#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 335));
                    if (!charactersEventTable) return;

                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2024#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2024#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2024)", 107);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2024)", 108);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2024)",
                        109);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2024) *", 110);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesAndHatchingTideDay", "Little Ladies' Day & Hatching-tide")} (2024)",
                        111);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ThePathInterfal", "The Path Infernal")} (2024)",
                        112);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2024) *", 113);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2024", 114);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2024)", 115);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        116);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2024)",
                        117);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2024)",
                        118);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2024)",
                        119);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        120);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2024)",
                        121);
                }
            }

            using (var progressEvent2023Tab = ImRaii.TabItem("2023###progressEvent#Tabs#2023"))
            {
                if (progressEvent2023Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        "###CharactersProgress#All#Event#2023#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 250));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn("###CharactersProgress#All#Event#2023#Name",
                        ImGuiTableColumnFlags.WidthFixed, 200);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2023#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2023)", 97);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2023)",
                        98);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2023)", 99);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2023)",
                        100);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign 2023")}", 101);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2023)",
                        102);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2023)",
                        103);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2023)",
                        104);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        105);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2023)",
                        106);
                }
            }

            using (var progressEvent2022Tab = ImRaii.TabItem("2022###progressEvent#Tabs#2022"))
            {
                if (progressEvent2022Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2022#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 270));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2022#Name",
                        ImGuiTableColumnFlags.WidthFixed, 200);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2022#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2022)", 86);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2021 delayed)",
                        87);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2022)",
                        88);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2022)", 89);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2022)",
                        90);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2022)", 91);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2022", 92);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2022)",
                        93);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2022)",
                        94);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2022)",
                        95);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2022)",
                        96);
                }
            }

            using (var progressEvent2021Tab = ImRaii.TabItem("2021###progressEvent#Tabs#2021"))
            {
                if (progressEvent2021Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2021#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 230));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2021#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2021#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2021)", 77);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneAndLittleLadiesDays", "Valentione's and Little Ladies' Day")} (2021)",
                        78);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2021)",
                        79);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2021", 80);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2021)",
                        81);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2021)",
                        82);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2021) *", 83);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2021)", 84);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2021)",
                        85);
                }
            }

            using (var progressEvent2020Tab = ImRaii.TabItem("2020###progressEvent#Tabs#2020"))
            {
                if (progressEvent2020Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2020#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 270));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2020#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2020#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2020)", 66);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2020)",
                        67);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2020)", 68);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2020)",
                        69);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2020)", 70);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2020)", 71);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2020)",
                        72);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2020) *", 73);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2020)",
                        74);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2020)", 75);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2020)",
                        76);
                }
            }

            using (var progressEvent2019Tab = ImRaii.TabItem("2019##progressEvent#Tabs#2019"))
            {
                if (progressEvent2019Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2019#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 250));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2019#Name",
                        ImGuiTableColumnFlags.WidthFixed, 200);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2019#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2019)", 56);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2019)",
                        57);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2019)", 58);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2019)",
                        59);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2019) *", 60);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2019)", 61);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2019)",
                        62);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2019)",
                        63);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2019)",
                        64);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2019)",
                        65);
                }
            }

            using (var progressEvent2018Tab = ImRaii.TabItem("2018###progressEvent#Tabs#2018"))
            {
                if (progressEvent2018Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2018#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 250));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2018#Name",
                        ImGuiTableColumnFlags.WidthFixed, 200);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2018#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2018)", 46);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2018)",
                        47);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2018)", 48);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2018)",
                        49);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2018)", 50);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2018)",
                        51);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2018)",
                        52);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2018)",
                        53);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheHuntForRathalos", "The Hunt For Rathalos")}",
                        54);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2018)",
                        55);
                }
            }

            using (var progressEvent2017Tab = ImRaii.TabItem("2017###progressEvent#Tabs#2017"))
            {
                if (progressEvent2017Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2017#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX, new Vector2(-1, 290));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2017#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2017#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2017)", 34);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2017)",
                        35);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2017)", 36);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2017)",
                        37);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2017)", 38);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2017)",
                        39);
                    DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2017)",
                        40);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2017) *", 41);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2017)",
                        42);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2017)", 43);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2017)", 44);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2017)",
                        45);
                }
            }

            using (var progressEvent2013141516Tab = ImRaii.TabItem("2013-14-15-16###progressEvent#Tabs#2013141516"))
            {
                if (progressEvent2013141516Tab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2013141516#Table",
                        chars.Count + 1,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2013141516#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2013141516#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }
                    ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                        ImGui.EndTooltip();
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }

                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2013)",
                        0);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LightningStrikes", "Lightning Strikes")} (2013)", 1);
                    //DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2013)", 2);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2014)", 2);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BurgeoningDread", "Burgeoning Dread")} (2014)",
                        3);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2014)",
                        4);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2014)",
                        5);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2014)", 6);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2014)",
                        7);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2014)",
                        8);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ThatOldBlackMagic", "That Old Black Magic")} (2014)", 9);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheRising", "The Rising")} (2014)",
                        10);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LightningReturns", "Lightning Returns")}",
                        11);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2014)",
                        12);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2014)",
                        13);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2014)",
                        14);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2015)", 15);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2015)",
                        16);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2015)", 17);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2015)",
                        18);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2015)",
                        19);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheRising", "The Rising")} (2015)",
                        20);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2015)",
                        21);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2015)", 22);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2015)",
                        23);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2016)", 24);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2016)",
                        25);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2016)", 26);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2016)",
                        27);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2016", 28);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2016) *", 29);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2016)",
                        30);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_TheRising", "The Rising")} (2016)",
                        31);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2016)",
                        32);
                    DrawAllLine(chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2016)",
                        33);
                }
            }

            using (var blundervilleRewards =
                   ImRaii.TabItem(
                       $"{Loc.Localize("Event_Blunderville", "Blunderville")} {_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885)}"))
            {
                if (blundervilleRewards.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#blundervilleRewards#Table",
                        chars.Count + 2,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#blundervilleRewards#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#blundervilleRewards#Currency",
                        ImGuiTableColumnFlags.WidthFixed, 25);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#blundervilleRewards#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }
                    ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                    ImGui.TableSetColumnIndex(1);
                    Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)Currencies.MGF);
                    if (itm == null) return;
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }


                    DrawBlundervilleRewards(chars);
                }
            }

            string mogEventName = _currentLocale switch
            {
                ClientLanguage.German => "Mog Mog-Kollektion",
                ClientLanguage.English => "Moogle Treasure Trove",
                ClientLanguage.French => "Collection Mog Mog",
                ClientLanguage.Japanese => "モグモグ★コレクション",
                _ => "Moogle Treasure Trove"
            };
            using (var moogleRewards = ImRaii.TabItem($"{mogEventName}"))
            {
                if (moogleRewards.Success)
                {
                    DrawMoogleRewards(chars);
                }
            }
        }

        private void DrawMoogleRewards(List<Character> chars)
        {
            Dictionary<int, string> mooglesNames = [];
            mooglesNames[0] = Loc.Localize("OldMoogleEvent", "Old Events");
            string revelationName = _currentLocale switch
            {
                ClientLanguage.German => "Aufgezeigte Offenbarungen",
                ClientLanguage.English => "The Hunt for Revelation",
                ClientLanguage.French => "Révélation Kupo",
                ClientLanguage.Japanese => "\uff5e黙示への道標\uff5e",
                _ => "The Hunt for Revelation"
            };
            if (ImGui.CollapsingHeader($"2025 - {revelationName}"))
            {
                using var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#MogRewards#Table#Event2025_3",
                    chars.Count + 2,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                if (!charactersEventTable) return;
                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_3#Name",
                    ImGuiTableColumnFlags.WidthFixed, 270);
                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_3#Currency",
                    ImGuiTableColumnFlags.WidthFixed, 20);
                foreach (Character c in chars)
                {
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_3#{c.CharacterId}",
                        ImGuiTableColumnFlags.WidthFixed, 20);
                }

                ImGui.TableSetupScrollFreeze(-1, 1); //Freeze header so it shows while scrolling
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                ImGui.TableSetColumnIndex(1);
                Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                    (uint)Currencies.IRREGULAR_TOMESTONE_OF_REVELATION);
                if (itm == null) return;
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }

                DrawAllCharsMount(chars, 226, 50);
                DrawAllCharsEmote(chars, 180, 50);
                DrawAllCharsHairstyle(chars, 23370, 50);
                DrawAllCharsOrchestrion(chars, 417, 50);
                DrawAllCharsMinion(chars, 346, 30);
                DrawAllCharsBarding(chars, 61, 30);
                DrawAllCharsOrnament(chars, 1, 30);
                DrawAllCharsMount(chars, 20, 30);
                DrawAllCharsMount(chars, 26, 30);
                DrawAllCharsMount(chars, 133, 30);
                DrawAllCharsMount(chars, 144, 30);
                DrawAllCharsMinion(chars, 61, 15);
                DrawAllCharsTripleTriadCard(chars, 158, 10);
                DrawAllCharsTripleTriadCard(chars, 160, 10);
                DrawAllCharsTripleTriadCard(chars, 244, 7);
                DrawAllCharsTripleTriadCard(chars, 306, 7);
                DrawAllCharsTripleTriadCard(chars, 291, 7);
                DrawAllCharsMinion(chars, 144, 7);
                DrawAllCharsMinion(chars, 362, 5);
                DrawAllCharsOrchestrion(chars, 289, 3);
                DrawAllCharsTotal(chars, 481);
            }

            string allegoryName = _currentLocale switch
            {
                ClientLanguage.German => "Fantastische Fachsimpeleien",
                ClientLanguage.English => "The Hunt for Allegory",
                ClientLanguage.French => "L'allégorie perdue",
                ClientLanguage.Japanese => "\uff5e奇譚の探求者\uff5e",
                _ => "The Hunt for Allegory"
            };
            mooglesNames[2025_2] = $"2025 {allegoryName}";
            string phantasmagoriaName = _currentLocale switch
            {
                ClientLanguage.German => "Phantasmagorische Freundschaften",
                ClientLanguage.English => "The Hunt for Phantasmagoria",
                ClientLanguage.French => "Fantasmagorie Kupo",
                ClientLanguage.Japanese => "\uff5e幻想との邂逅\uff5e",
                _ => "The Hunt for Phantasmagoria"
            };
            mooglesNames[2025_1] = $"2025 {phantasmagoriaName}";
            string goetiaName = _currentLocale switch
            {
                ClientLanguage.German => "Goëtische Goldschätze",
                ClientLanguage.English => "The Hunt for Goetia",
                ClientLanguage.French => "L'aube de la goétie",
                ClientLanguage.Japanese => "～黄金の魔典～",
                _ => "The Hunt for Goetia"
            };
            mooglesNames[2024_3] = $"2024 - {goetiaName}";
            string genesis2Name = _currentLocale switch
            {
                ClientLanguage.German => "Dimensionale Ursprünge - Teil 2",
                ClientLanguage.English => "The Second Hunt for Genesis",
                ClientLanguage.French => "Genèse d'une nouvelle dimension - Partie 2",
                ClientLanguage.Japanese => "～新次元の創世 Part2～",
                _ => "The Second Hunt for Genesis"
            };
            mooglesNames[2024_2] = $"2024 - {genesis2Name}";
            string genesis1Name = _currentLocale switch
            {
                ClientLanguage.German => "Dimensionale Ursprünge - Teil 1",
                ClientLanguage.English => "The First Hunt for Genesis",
                ClientLanguage.French => "Genèse d'une nouvelle dimension - Partie 1",
                ClientLanguage.Japanese => "～新次元の創世 Part1～",
                _ => "The First Hunt for Genesis"
            };
            mooglesNames[2024_1] = $"2024 - {genesis1Name}";
            string tenthAnniversaryName = _currentLocale switch
            {
                ClientLanguage.German => "10. Jubiläum",
                ClientLanguage.English => "The 10th Anniversary Hunt",
                ClientLanguage.French => "Spéciale 10e anniversaire",
                ClientLanguage.Japanese => "\uff5e新生10周年スペシャル\uff5e",
                _ => "The 10th Anniversary Hunt"
            };
            mooglesNames[2023_2] = $"2023 - {tenthAnniversaryName}";
            string mendacityName = _currentLocale switch
            {
                ClientLanguage.German => "Tratsch aus Quatsch",
                ClientLanguage.English => "The Hunt for Mendacity",
                ClientLanguage.French => "Duplicité Kupo",
                ClientLanguage.Japanese => "\uff5e虚構の刻\uff5e",
                _ => "The Hunt for Mendacity"
            };
            mooglesNames[2023_1] = $"2023 - {mendacityName}";
            string theHuntForCreationName = _currentLocale switch
            {
                ClientLanguage.German => "Allerlei Erinnerungen",
                ClientLanguage.English => "The Hunt for Creation",
                ClientLanguage.French => "Cosmogonie Kupo",
                ClientLanguage.Japanese => "\uff5e万物の記憶\uff5e",
                _ => "The Hunt for Creation"
            };
            mooglesNames[2022_3] = $"2022 - {theHuntForCreationName}";

            string theHuntForVerityName = _currentLocale switch
            {
                ClientLanguage.German => "Die Stunde der Wahrheit",
                ClientLanguage.English => "The Hunt for Verity",
                ClientLanguage.French => "Véridicité Kupo",
                ClientLanguage.Japanese => "\uff5e帰ってきた真理\uff5e",
                _ => "The Hunt for Verity"
            };
            mooglesNames[2022_2] = $"2022 - {theHuntForVerityName}";

            string theHuntForScriptureName = _currentLocale switch
            {
                ClientLanguage.German => "Theologisches Vermächtnis",
                ClientLanguage.English => "The Hunt for Scripture",
                ClientLanguage.French => "Théologie Kupo",
                ClientLanguage.Japanese => "～聖典を継ぐ者～",
                _ => "The Hunt for Scripture"
            };
            mooglesNames[2022_1] = $"2022 - {theHuntForScriptureName}";

            string theHuntForLoreName = _currentLocale switch
            {
                ClientLanguage.German => "Sagenhafte Schätze",
                ClientLanguage.English => "The Hunt for Lore",
                ClientLanguage.French => "Tradition Kupo",
                ClientLanguage.Japanese => "\uff5e炎獄の伝承\uff5e",
                _ => "The Hunt for Lore"
            };
            mooglesNames[2021_3] = $"2021 - {theHuntForLoreName}";

            string theHuntForPageantryName = _currentLocale switch
            {
                ClientLanguage.German => "Fan Festival",
                ClientLanguage.English => "The Hunt for Pageantry",
                ClientLanguage.French => "Festivités Kupo",
                ClientLanguage.Japanese => "\uff5eファンフェススペシャル2021\uff5e",
                _ => "The Hunt for Pageantry"
            };
            mooglesNames[2021_2] = $"2021 - {theHuntForPageantryName}";

            string theHuntForEsotericsName = _currentLocale switch
            {
                ClientLanguage.German => "Esoterische Momente",
                ClientLanguage.English => "The Hunt for Esoterics",
                ClientLanguage.French => "Ésotérisme Kupo",
                ClientLanguage.Japanese => "\uff5eもうひとつの禁書\uff5e",
                _ => "The Hunt for Esoterics"
            };
            mooglesNames[2021_1] = $"2021 - {theHuntForEsotericsName}";

            string theHuntForLawName = _currentLocale switch
            {
                ClientLanguage.German => "Emotionale Erinnerungen",
                ClientLanguage.English => "The Hunt for Law",
                ClientLanguage.French => "Réminiscence Kupo",
                ClientLanguage.Japanese => "\uff5e追憶の法典\uff5e",
                _ => "The Hunt for Law"
            };
            mooglesNames[2020_2] = $"2020 - {theHuntForLawName}";

            string theHuntForSoldieryName = _currentLocale switch
            {
                ClientLanguage.German => "Strategische Schnitzeljag",
                ClientLanguage.English => "The Hunt for Soldiery",
                ClientLanguage.French => "Martialité Kupo",
                ClientLanguage.Japanese => "\uff5e戦記ューフォーエバー\uff5e",
                _ => "The Hunt for Soldiery"
            };
            mooglesNames[2020_1] = $"2020 - {theHuntForSoldieryName}";

            string theHuntForMythologyName = _currentLocale switch
            {
                ClientLanguage.German => "Mythologische Mär",
                ClientLanguage.English => "The Hunt for Mythology",
                ClientLanguage.French => "Mythologie Kupo",
                ClientLanguage.Japanese => "\uff5eそして神話へ…\uff5e",
                _ => "The Hunt for Mythology"
            };
            mooglesNames[2019_2] = $"2019 - {theHuntForMythologyName}";

            string theHuntForPhilosophyName = _currentLocale switch
            {
                ClientLanguage.German => "Philosophische Momente",
                ClientLanguage.English => "The Hunt for Philosophy",
                ClientLanguage.French => "Philosophie Kupo",
                ClientLanguage.Japanese => "\uff5e哲学ふたたび\uff5e",
                _ => "The Hunt for Philosophy"
            };
            mooglesNames[2019_1] = $"2019 - {theHuntForPhilosophyName}";

            string n = (_currentOldMoogleReward == 0) ? mooglesNames[0] : mooglesNames[_currentOldMoogleReward];
            using (var combo = ImRaii.Combo("###CharactersProgress#Reputations#Combo", n))
            {
                if (combo)
                {
                    foreach (KeyValuePair<int, string> name in mooglesNames.Where(name =>
                                 ImGui.Selectable(name.Value, name.Value == n)))
                    {
                        _currentOldMoogleReward = name.Key;
                    }
                }
            }

            switch (_currentOldMoogleReward)
            {
                case 2025_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#MogRewards#Table#Event2025_2",
                    chars.Count + 2,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }
                        ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_ALLEGORY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsOrchestrion(chars, 365, 100);
                        DrawAllCharsMount(chars, 217, 50);
                        DrawAllCharsMount(chars, 191, 50);
                        DrawAllCharsHairstyle(chars, 23369, 50);
                        DrawAllCharsMinion(chars, 385, 50);
                        DrawAllCharsBarding(chars, 76, 30);
                        DrawAllCharsEmote(chars, 195, 30);
                        DrawAllCharsOrnament(chars, 13, 30);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 116, 30);
                        DrawAllCharsMount(chars, 115, 30);
                        DrawAllCharsMinion(chars, 60, 15);
                        DrawAllCharsTripleTriadCard(chars, 141, 10);
                        DrawAllCharsTripleTriadCard(chars, 142, 10);
                        DrawAllCharsTripleTriadCard(chars, 290, 7);
                        DrawAllCharsTripleTriadCard(chars, 293, 7);
                        DrawAllCharsTripleTriadCard(chars, 326, 7);
                        DrawAllCharsMinion(chars, 243, 7);
                        DrawAllCharsOrchestrion(chars, 15, 7);
                        DrawAllCharsMinion(chars, 199, 1);
                        break;
                    }
                case 2025_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2025_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2025_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2025_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_PHANTASMAGORIA);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 205, 50);
                        DrawAllCharsMount(chars, 112, 50);
                        DrawAllCharsMount(chars, 193, 50);
                        DrawAllCharsBarding(chars, 79, 50);
                        DrawAllCharsMinion(chars, 374, 50);
                        DrawAllCharsOrchestrion(chars, 364, 50);
                        DrawAllCharsHairstyle(chars, 24233, 30);
                        DrawAllCharsOrnament(chars, 11, 30);
                        DrawAllCharsEmote(chars, 203, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 133, 30);
                        DrawAllCharsMount(chars, 182, 30);
                        DrawAllCharsMinion(chars, 50, 15);
                        DrawAllCharsTripleTriadCard(chars, 107, 10);
                        DrawAllCharsTripleTriadCard(chars, 121, 10);
                        DrawAllCharsTripleTriadCard(chars, 279, 7);
                        DrawAllCharsTripleTriadCard(chars, 304, 7);
                        DrawAllCharsTripleTriadCard(chars, 315, 7);
                        DrawAllCharsMinion(chars, 340, 7);
                        DrawAllCharsMinion(chars, 353, 7);
                        DrawAllCharsOrchestrion(chars, 345, 7);
                        break;
                    }
                case 2024_3:
                    {
                        using var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#MogRewards#Table#Event2024_3",
                    chars.Count + 2,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_3#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_3#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_3#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_GOETIA);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 192, 50);
                        DrawAllCharsMount(chars, 126, 50);
                        DrawAllCharsBarding(chars, 73, 50);
                        DrawAllCharsHairstyle(chars, 32835, 50);
                        DrawAllCharsOrchestrion(chars, 363, 50);
                        DrawAllCharsEmote(chars, 215, 50);
                        DrawAllCharsEmote(chars, 189, 30);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 158, 30);
                        DrawAllCharsMount(chars, 172, 30);
                        DrawAllCharsMinion(chars, 58, 15);
                        DrawAllCharsTripleTriadCard(chars, 105, 10);
                        DrawAllCharsTripleTriadCard(chars, 106, 10);
                        DrawAllCharsTripleTriadCard(chars, 228, 7);
                        DrawAllCharsTripleTriadCard(chars, 271, 7);
                        DrawAllCharsTripleTriadCard(chars, 303, 7);
                        DrawAllCharsMinion(chars, 326, 7);
                        DrawAllCharsMinion(chars, 336, 7);
                        DrawAllCharsOrchestrion(chars, 231, 7);

                        break;
                    }
                case 2024_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2024_2",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 25);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2024_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_II);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 211, 50);
                        DrawAllCharsMount(chars, 224, 50);
                        DrawAllCharsMinion(chars, 359, 50);
                        DrawAllCharsOrchestrion(chars, 333, 50);
                        DrawAllCharsBarding(chars, 54, 30);
                        DrawAllCharsOrchestrion(chars, 76, 30);
                        DrawAllCharsOrnament(chars, 7, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 30, 30);
                        DrawAllCharsMount(chars, 77, 30);
                        DrawAllCharsMount(chars, 144, 30);
                        DrawAllCharsTripleTriadCard(chars, 103, 10);
                        DrawAllCharsTripleTriadCard(chars, 104, 10);
                        DrawAllCharsTripleTriadCard(chars, 267, 10);
                        DrawAllCharsTripleTriadCard(chars, 282, 10);
                        DrawAllCharsMinion(chars, 388, 7);
                        DrawAllCharsMinion(chars, 148, 7);
                        DrawAllCharsOrchestrion(chars, 207, 7);


                        break;
                    }
                case 2024_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2024_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2024_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 25);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2024_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_I);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 242, 50);
                        DrawAllCharsBarding(chars, 69, 50);
                        DrawAllCharsHairstyle(chars, 28615, 50);
                        DrawAllCharsMinion(chars, 319, 50);
                        DrawAllCharsMount(chars, 208, 30);
                        DrawAllCharsOrchestrion(chars, 254, 30);
                        DrawAllCharsOrchestrion(chars, 78, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 43, 30);
                        DrawAllCharsMount(chars, 76, 30);
                        DrawAllCharsMount(chars, 133, 30);
                        DrawAllCharsTripleTriadCard(chars, 111, 10);
                        DrawAllCharsTripleTriadCard(chars, 102, 10);
                        DrawAllCharsTripleTriadCard(chars, 249, 7);
                        DrawAllCharsTripleTriadCard(chars, 261, 7);
                        DrawAllCharsMinion(chars, 361, 7);
                        DrawAllCharsMinion(chars, 144, 7);
                        DrawAllCharsOrchestrion(chars, 179, 7);


                        break;
                    }
                case 2023_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2023_2",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2023_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2023_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 25);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2023_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_TENFOLD_PAGEANTRY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsFramerKit(chars, 40501, 10);
                        DrawAllCharsOrnament(chars, 14, 100);
                        DrawAllCharsTripleTriadCard(chars, 68, 100);
                        DrawAllCharsHairstyle(chars, 33706, 100);
                        DrawAllCharsBarding(chars, 66, 100);
                        DrawAllCharsEmote(chars, 181, 80);
                        DrawAllCharsEmote(chars, 180, 50);
                        DrawAllCharsEmote(chars, 213, 50);
                        DrawAllCharsEmote(chars, 223, 50);
                        DrawAllCharsEmote(chars, 214, 50);
                        DrawAllCharsMount(chars, 189, 50);
                        DrawAllCharsMount(chars, 236, 50);
                        DrawAllCharsMount(chars, 150, 50);
                        DrawAllCharsOrchestrion(chars, 45, 50);
                        DrawAllCharsOrchestrion(chars, 386, 50);
                        DrawAllCharsOrchestrion(chars, 507, 50);
                        DrawAllCharsHairstyle(chars, 24234, 50);
                        DrawAllCharsMinion(chars, 349, 50);
                        DrawAllCharsMinion(chars, 352, 50);
                        DrawAllCharsMount(chars, 116, 30);
                        DrawAllCharsMount(chars, 115, 30);
                        DrawAllCharsMount(chars, 133, 30);
                        DrawAllCharsMount(chars, 144, 30);
                        DrawAllCharsMount(chars, 158, 30);
                        DrawAllCharsMount(chars, 172, 30);
                        DrawAllCharsMount(chars, 182, 30);
                        DrawAllCharsOrchestrion(chars, 257, 30);
                        DrawAllCharsOrchestrion(chars, 258, 30);
                        DrawAllCharsOrchestrion(chars, 259, 30);
                        DrawAllCharsOrnament(chars, 2, 30);
                        DrawAllCharsTripleTriadCard(chars, 83, 10);
                        DrawAllCharsTripleTriadCard(chars, 84, 10);
                        DrawAllCharsTripleTriadCard(chars, 235, 7);
                        DrawAllCharsTripleTriadCard(chars, 250, 7);
                        DrawAllCharsMinion(chars, 372, 7);
                        DrawAllCharsMinion(chars, 373, 7);
                        DrawAllCharsMinion(chars, 93, 7);
                        DrawAllCharsMinion(chars, 110, 7);
                        DrawAllCharsOrchestrion(chars, 345, 7);
                        DrawAllCharsMinion(chars, 199, 1);



                        break;
                    }
                case 2023_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2023_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2023_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2023_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2023_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_MENDACITY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 121, 50);
                        DrawAllCharsMount(chars, 130, 50);
                        DrawAllCharsMount(chars, 225, 50);
                        DrawAllCharsOrchestrion(chars, 324, 50);
                        DrawAllCharsHairstyle(chars, 23369, 50);
                        DrawAllCharsEmote(chars, 208, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 28, 30);
                        DrawAllCharsMount(chars, 22, 30);
                        DrawAllCharsMount(chars, 75, 30);
                        DrawAllCharsMount(chars, 104, 30);
                        DrawAllCharsMount(chars, 116, 30);
                        DrawAllCharsMount(chars, 115, 30);
                        DrawAllCharsTripleTriadCard(chars, 81, 10);
                        DrawAllCharsTripleTriadCard(chars, 82, 10);
                        DrawAllCharsTripleTriadCard(chars, 216, 7);
                        DrawAllCharsTripleTriadCard(chars, 244, 7);
                        DrawAllCharsMinion(chars, 82, 7);
                        DrawAllCharsMinion(chars, 333, 7);
                        DrawAllCharsOrchestrion(chars, 117, 7);
                        DrawAllCharsOrchestrion(chars, 118, 7);


                        break;
                    }
                case 2022_3:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2022_3",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2022_3#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2022_3#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2022_3#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_CREATION);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 182, 50);
                        DrawAllCharsMount(chars, 209, 50);
                        DrawAllCharsMount(chars, 112, 50);
                        DrawAllCharsEmote(chars, 195, 50);
                        DrawAllCharsOrchestrion(chars, 227, 50);
                        DrawAllCharsOrchestrion(chars, 264, 50);
                        DrawAllCharsEmote(chars, 207, 30);
                        DrawAllCharsBarding(chars, 31, 30);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 29, 30);
                        DrawAllCharsMount(chars, 31, 30);
                        DrawAllCharsMount(chars, 90, 30);
                        DrawAllCharsMount(chars, 98, 30);
                        DrawAllCharsOrchestrion(chars, 36, 10);
                        DrawAllCharsOrchestrion(chars, 37, 10);
                        DrawAllCharsOrchestrion(chars, 234, 7);
                        DrawAllCharsOrchestrion(chars, 250, 7);
                        DrawAllCharsMinion(chars, 256, 7);
                        DrawAllCharsOrchestrion(chars, 85, 7);
                        DrawAllCharsOrchestrion(chars, 86, 7);


                        break;
                    }
                case 2022_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2022_2",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2022_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2022_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2022_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_VERITY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 158, 50);
                        DrawAllCharsMount(chars, 211, 50);
                        DrawAllCharsMinion(chars, 314, 50);
                        DrawAllCharsHairstyle(chars, 31406, 50);
                        DrawAllCharsOrchestrion(chars, 263, 50);
                        DrawAllCharsEmote(chars, 203, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 30, 30);
                        DrawAllCharsMount(chars, 40, 30);
                        DrawAllCharsMount(chars, 77, 30);
                        DrawAllCharsMount(chars, 78, 30);
                        DrawAllCharsTripleTriadCard(chars, 33, 10);
                        DrawAllCharsTripleTriadCard(chars, 35, 10);
                        DrawAllCharsTripleTriadCard(chars, 224, 7);
                        DrawAllCharsTripleTriadCard(chars, 229, 7);
                        DrawAllCharsMinion(chars, 243, 7);
                        DrawAllCharsMinion(chars, 330, 7);
                        DrawAllCharsOrchestrion(chars, 231, 7);

                        break;
                    }
                case 2022_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2022_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2022_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2022_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2022_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_SCRIPTURE);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 172, 50);
                        DrawAllCharsHairstyle(chars, 30113, 50);
                        DrawAllCharsOrchestrion(chars, 262, 50);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 28, 30);
                        DrawAllCharsMount(chars, 43, 30);
                        DrawAllCharsMount(chars, 75, 30);
                        DrawAllCharsMount(chars, 76, 30);
                        DrawAllCharsMinion(chars, 148, 7);
                        DrawAllCharsMinion(chars, 254, 7);
                        DrawAllCharsTripleTriadCard(chars, 213, 7);
                        DrawAllCharsTripleTriadCard(chars, 215, 7);
                        DrawAllCharsTripleTriadCard(chars, 209, 7);
                        DrawAllCharsOrchestrion(chars, 179, 7);
                        DrawAllCharsOrchestrion(chars, 207, 7);

                        break;
                    }
                case 2021_3:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2021_3",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2021_3#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2021_3#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2021_3#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_LORE);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 144, 50);
                        DrawAllCharsMount(chars, 90, 50);
                        DrawAllCharsHairstyle(chars, 24234, 50);
                        DrawAllCharsOrchestrion(chars, 261, 50);
                        DrawAllCharsBarding(chars, 54, 30);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 29, 30);
                        DrawAllCharsMount(chars, 31, 30);
                        DrawAllCharsMinion(chars, 144, 7);
                        DrawAllCharsMinion(chars, 283, 7);
                        DrawAllCharsTripleTriadCard(chars, 206, 7);
                        DrawAllCharsTripleTriadCard(chars, 179, 7);
                        DrawAllCharsTripleTriadCard(chars, 197, 7);
                        DrawAllCharsOrchestrion(chars, 117, 7);
                        DrawAllCharsOrchestrion(chars, 118, 7);

                        break;
                    }
                case 2021_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2021_2",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2021_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2021_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2021_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_PAGEANTRY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 133, 50);
                        DrawAllCharsMount(chars, 115, 50);
                        DrawAllCharsMount(chars, 116, 50);
                        DrawAllCharsEmote(chars, 180, 50);
                        DrawAllCharsEmote(chars, 181, 80);
                        DrawAllCharsHairstyle(chars, 23369, 50);
                        DrawAllCharsHairstyle(chars, 24233, 50);
                        DrawAllCharsOrchestrion(chars, 257, 50);
                        DrawAllCharsOrchestrion(chars, 258, 50);
                        DrawAllCharsOrchestrion(chars, 259, 50);
                        DrawAllCharsOrchestrion(chars, 260, 30);
                        DrawAllCharsMinion(chars, 199, 1);
                        DrawAllCharsMinion(chars, 270, 7);
                        DrawAllCharsMinion(chars, 305, 7);
                        DrawAllCharsMinion(chars, 197, 7);
                        DrawAllCharsTripleTriadCard(chars, 201, 7);
                        DrawAllCharsTripleTriadCard(chars, 190, 7);
                        DrawAllCharsTripleTriadCard(chars, 191, 7);

                        break;
                    }
                case 2021_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2021_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2021_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2021_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2021_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_ESOTERICS);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 115, 50);
                        DrawAllCharsMount(chars, 150, 50);
                        DrawAllCharsMount(chars, 112, 50);
                        DrawAllCharsMount(chars, 121, 50);
                        DrawAllCharsOrchestrion(chars, 580, 50);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 30, 30);
                        DrawAllCharsMount(chars, 40, 30);
                        DrawAllCharsMount(chars, 22, 30);
                        DrawAllCharsMinion(chars, 143, 7);
                        DrawAllCharsMinion(chars, 272, 7);
                        DrawAllCharsTripleTriadCard(chars, 85, 7);
                        DrawAllCharsTripleTriadCard(chars, 177, 7);
                        DrawAllCharsTripleTriadCard(chars, 182, 7);
                        DrawAllCharsOrchestrion(chars, 85, 7);
                        DrawAllCharsOrchestrion(chars, 86, 7);

                        break;
                    }
                case 2020_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2020_2",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2020_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2020_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2020_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_LAW);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 130, 50);
                        DrawAllCharsMount(chars, 104, 50);
                        DrawAllCharsMount(chars, 116, 50);
                        DrawAllCharsOrchestrion(chars, 43, 50);
                        DrawAllCharsOrchestrion(chars, 92, 50);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 30, 30);
                        DrawAllCharsMount(chars, 28, 30);
                        DrawAllCharsMount(chars, 43, 30);
                        DrawAllCharsMinion(chars, 178, 7);
                        DrawAllCharsMinion(chars, 179, 7);
                        DrawAllCharsTripleTriadCard(chars, 116, 7);
                        DrawAllCharsTripleTriadCard(chars, 137, 7);
                        DrawAllCharsTripleTriadCard(chars, 64, 7);
                        DrawAllCharsOrchestrion(chars, 231, 7);
                        DrawAllCharsOrchestrion(chars, 30, 7);

                        break;
                    }
                case 2020_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2020_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2020_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2020_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2020_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_SOLDIERY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }


                        DrawAllCharsMount(chars, 121, 50);
                        DrawAllCharsMount(chars, 78, 50);
                        DrawAllCharsOrchestrion(chars, 45, 50);
                        DrawAllCharsOrchestrion(chars, 78, 50);
                        DrawAllCharsOrchestrion(chars, 76, 50);
                        DrawAllCharsOrchestrion(chars, 77, 50);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 29, 30);
                        DrawAllCharsMount(chars, 31, 30);
                        DrawAllCharsMinion(chars, 281, 7);
                        DrawAllCharsMinion(chars, 141, 7);
                        DrawAllCharsTripleTriadCard(chars, 43, 7);
                        DrawAllCharsTripleTriadCard(chars, 55, 7);
                        DrawAllCharsTripleTriadCard(chars, 98, 7);
                        DrawAllCharsTripleTriadCard(chars, 168, 7);
                        DrawAllCharsOrchestrion(chars, 179, 7);
                        DrawAllCharsOrchestrion(chars, 207, 7);
                        break;
                    }
                case 2019_2:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2019_2",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2019_2#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2019_2#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2019_2#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_MYTHOLOGY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 112, 50);
                        DrawAllCharsMount(chars, 77, 50);
                        DrawAllCharsMount(chars, 98, 50);
                        DrawAllCharsOrchestrion(chars, 45, 50);
                        DrawAllCharsOrchestrion(chars, 78, 50);
                        DrawAllCharsOrchestrion(chars, 76, 50);
                        DrawAllCharsOrchestrion(chars, 77, 50);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 30, 30);
                        DrawAllCharsMount(chars, 40, 30);
                        DrawAllCharsMinion(chars, 232, 7);
                        DrawAllCharsMinion(chars, 259, 7);
                        DrawAllCharsTripleTriadCard(chars, 52, 7);
                        DrawAllCharsTripleTriadCard(chars, 219, 7);
                        DrawAllCharsTripleTriadCard(chars, 110, 7);
                        DrawAllCharsTripleTriadCard(chars, 99, 7);
                        DrawAllCharsOrchestrion(chars, 117, 7);
                        DrawAllCharsOrchestrion(chars, 118, 7);
                        break;
                    }
                case 2019_1:
                    {
                        using var charactersEventTable = ImRaii.Table(
                            $"###CharactersProgress#All#Event#MogRewards#Table#Event2019_1",
                            chars.Count + 2,
                            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                        if (!charactersEventTable) return;
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2019_1#Name",
                            ImGuiTableColumnFlags.WidthFixed, 270);
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2019_1#Currency",
                            ImGuiTableColumnFlags.WidthFixed, 50);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn(
                                $"###CharactersProgress#All#Event#MogRewards#Event2019_1#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_PHILOSOPHY);
                        if (itm == null) return;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(_currentLocale, ref _globalCache, itm.Value);
                        }

                        foreach (Character currChar in chars)
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                                ImGui.EndTooltip();
                            }
                        }

                        DrawAllCharsMount(chars, 67, 50);
                        DrawAllCharsMount(chars, 75, 50);
                        DrawAllCharsMount(chars, 76, 50);
                        DrawAllCharsOrchestrion(chars, 45, 50);
                        DrawAllCharsMount(chars, 19, 30);
                        DrawAllCharsMount(chars, 20, 30);
                        DrawAllCharsMount(chars, 26, 30);
                        DrawAllCharsMount(chars, 27, 30);
                        DrawAllCharsMount(chars, 28, 30);
                        DrawAllCharsMount(chars, 35, 30);
                        DrawAllCharsMount(chars, 43, 30);
                        DrawAllCharsBarding(chars, 16, 30);
                        DrawAllCharsMinion(chars, 188, 7);
                        DrawAllCharsMinion(chars, 194, 7);
                        DrawAllCharsMinion(chars, 215, 7);
                        DrawAllCharsMinion(chars, 299, 7);
                        DrawAllCharsTripleTriadCard(chars, 136, 7);
                        DrawAllCharsTripleTriadCard(chars, 152, 7);
                        DrawAllCharsTripleTriadCard(chars, 163, 7);
                        DrawAllCharsTripleTriadCard(chars, 229, 7);
                        DrawAllCharsOrchestrion(chars, 85, 7);
                        DrawAllCharsOrchestrion(chars, 86, 7);
                        break;
                    }
            }
        }

        private void DrawAllCharsEmote(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Emote? e = _globalCache.EmoteStorage.GetEmote(_currentLocale, id);
            if (e != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(e.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawEmoteTooltip(_currentLocale, ref _globalCache, e);
                }
                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => e.GermanName,
                    ClientLanguage.English => e.EnglishName,
                    ClientLanguage.French => e.FrenchName,
                    ClientLanguage.Japanese => e.JapaneseName,
                    _ => e.EnglishName
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasEmote(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasEmote(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }
        private void DrawAllCharsMount(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Mount? mount = _globalCache.MountStorage.GetMount(_currentLocale, id);
            if (mount != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(mount.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMountTooltip(_currentLocale, ref _globalCache, mount);
                }

                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => mount.GermanName,
                    ClientLanguage.English => Utils.CapitalizeSentence(mount.EnglishName),
                    ClientLanguage.French => Utils.CapitalizeSentence(mount.FrenchName),
                    ClientLanguage.Japanese => mount.JapaneseName,
                    _ => Utils.CapitalizeSentence(mount.EnglishName)
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasMount(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasMount(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }
        private void DrawAllCharsMinion(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Minion? minion = _globalCache.MinionStorage.GetMinion(_currentLocale, id);
            if (minion != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(minion.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMinionTooltip(_currentLocale, ref _globalCache, minion);
                }

                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => minion.GermanName,
                    ClientLanguage.English => Utils.CapitalizeSentence(minion.EnglishName),
                    ClientLanguage.French => Utils.CapitalizeSentence(minion.FrenchName),
                    ClientLanguage.Japanese => minion.JapaneseName,
                    _ => Utils.CapitalizeSentence(minion.EnglishName)
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasMinion(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasMinion(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }
        private void DrawAllCharsOrchestrion(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            OrchestrionRoll? o = _globalCache.OrchestrionRollStorage.GetOrchestrionRoll(_currentLocale, id);
            if (o != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrchestrionRollTooltip(_currentLocale, ref _globalCache, o);
                }
                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => o.GermanName,
                    ClientLanguage.English => o.EnglishName,
                    ClientLanguage.French => o.FrenchName,
                    ClientLanguage.Japanese => o.JapaneseName,
                    _ => o.EnglishName
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasOrchestrionRoll(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasOrchestrionRoll(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }
        private void DrawAllCharsFramerKit(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(id);
            if (fkId != null)
            {
                FramerKit? fk = _globalCache.FramerKitStorage.LoadItem(_currentLocale, fkId.Value);
                if (fk != null)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(fk.Icon), new Vector2(32, 32));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, fk);
                    }

                    ImGui.SameLine();
                    string name = _currentLocale switch
                    {
                        ClientLanguage.German => fk.GermanName,
                        ClientLanguage.English => fk.EnglishName,
                        ClientLanguage.French => fk.FrenchName,
                        ClientLanguage.Japanese => fk.JapaneseName,
                        _ => fk.EnglishName
                    };
                    ImGui.TextUnformatted(name);

                    if (cost > 0)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{cost}");
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        //ImGui.TextUnformatted(currChar.HasFramerKit(fkId.Value) ? "\u2713" : "");
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.TextUnformatted(currChar.HasFramerKit(fk.Id) ? FontAwesomeIcon.Check.ToIconString() : "");
                        ImGui.PopFont();
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted(name);
                            ImGui.TextUnformatted(
                                $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                            ImGui.EndTooltip();
                        }
                    }
                }
            }
        }
        private void DrawAllCharsBarding(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Barding? b = _globalCache.BardingStorage.GetBarding(_currentLocale, id);
            if (b != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(b.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawBardingTooltip(_currentLocale, ref _globalCache, b);
                }

                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => b.GermanName,
                    ClientLanguage.English => b.EnglishName,
                    ClientLanguage.French => b.FrenchName,
                    ClientLanguage.Japanese => b.JapaneseName,
                    _ => b.EnglishName
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasBarding(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasBarding(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }
        private void DrawAllCharsTripleTriadCard(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            TripleTriadCard? ttc = _globalCache.TripleTriadCardStorage.GetTripleTriadCard(_currentLocale, id);
            if (ttc != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(027672), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, ttc);
                }

                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => ttc.GermanName,
                    ClientLanguage.English => ttc.EnglishName,
                    ClientLanguage.French => ttc.FrenchName,
                    ClientLanguage.Japanese => ttc.JapaneseName,
                    _ => ttc.EnglishName
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasTTC(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasTTC(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }

        private void DrawAllCharsOrnament(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Ornament? o = _globalCache.OrnamentStorage.GetOrnament(_currentLocale, id);
            if (o != null)
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrnamentTooltip(_currentLocale, ref _globalCache, o);
                }

                ImGui.SameLine();
                string name = _currentLocale switch
                {
                    ClientLanguage.German => o.GermanName,
                    ClientLanguage.English => o.EnglishName,
                    ClientLanguage.French => o.FrenchName,
                    ClientLanguage.Japanese => o.JapaneseName,
                    _ => o.EnglishName
                };
                ImGui.TextUnformatted(name);

                if (cost > 0)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{cost}");
                }

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    //ImGui.TextUnformatted(currChar.HasOrnament(id) ? "\u2713" : "");
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(currChar.HasOrnament(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }
            }
        }

        private void DrawAllCharsHairstyle(List<Character> chars, uint itemId, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, itemId);
            if (itm == null)
            {
                return;
            }
            Item i = itm.Value;
            ushort unlockLink = i.ItemAction.Value.Data[0];
            List<uint> ids = _globalCache.HairstyleStorage.GetIdsFromUnlockLink(unlockLink);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(i.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                Utils.DrawItemTooltip(_currentLocale, ref _globalCache, i);
            }

            ImGui.SameLine();
            ImGui.TextUnformatted(i.Name.ExtractText());

            if (cost > 0)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{cost}");
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(currChar.HasHairstyleFromIds(ids) ? FontAwesomeIcon.Check.ToIconString() : "");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(i.Name.ExtractText());
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }
        private void DrawAllCharsFacewear(List<Character> chars, uint id, int cost)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Models.Glasses? g = _globalCache.GlassesStorage.GetGlasses(_currentLocale, id);
            if (g == null)
            {
                return;
            }

            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(g.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                Utils.DrawGlassesTooltip(_currentLocale, ref _globalCache, g);
            }
            ImGui.SameLine();
            string name = _currentLocale switch
            {
                ClientLanguage.German => g.GermanName,
                ClientLanguage.English => g.EnglishName,
                ClientLanguage.French => g.FrenchName,
                ClientLanguage.Japanese => g.JapaneseName,
                _ => g.EnglishName
            };
            ImGui.TextUnformatted(Utils.CapitalizeSentence(name));

            if (cost > 0)
            {
                ImGui.TableNextColumn(); ImGui.TextUnformatted($"{cost}");
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(currChar.HasGlasses(id) ? FontAwesomeIcon.Check.ToIconString() : "");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(name);
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }

        private void DrawAllCharsTotal(List<Character> chars, int total)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 935)}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{total}");
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{(currChar.Currencies != null ? currChar.Currencies.Irregular_Tomestone_Of_Revelation : "")}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }

        private void DrawBlundervilleRewards(List<Character> chars)
        {
            DrawAllCharsEmote(chars, 276, 410);
            DrawAllCharsMount(chars, 330, 410);
            DrawAllCharsMinion(chars, 499, 350);
            DrawAllCharsMinion(chars, 500, 350);
            DrawAllCharsOrchestrion(chars, 657, 220);
            DrawAllCharsFramerKit(chars, 41377, 200);
            DrawAllCharsFramerKit(chars, 41378, 200);
            DrawAllCharsFramerKit(chars, 41379, 200);
        }

        private void DrawHildibrandQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var charactersHildibrandQuestAll = ImRaii.Table("###CharactersProgress#All#Hildibrand", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersHildibrandQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#Hildibrand#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#Hildibrand#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }
            ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                ImGui.EndTooltip();
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            List<List<bool>> charactersQuests = Utils.GetCharactersHildibrandQuests(chars);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752)}");
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_THE_IMMACULATE_DECEPTION)}", 0);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_THE_THREE_COLLECTORS)}", 1);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_A_CASE_OF_INDECENCY)}", 2);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_THE_COLISEUM_CONUNDRUM)}", 3);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_HER_LAST_VOW)}", 4);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753)}");
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_DONT_CALL_IT_A_COMEBACK)}", 5);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_THE_MEASURE_OF_A_MAMMET)}", 6);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_DONT_TRUST_ANYONE_OVER_SIXTY)}", 7);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_IF_I_COULD_TURN_BACK_TIME)}", 8);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754)}");
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_A_HINGAN_TALE_NASHU_GOES_EAST)}", 9);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_OF_WOLVES_AND_GENTLEMEN)}", 10);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_THE_BLADE_MISLAID)}", 11);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_GOOD_SWORDS_GOOD_DOGS)}", 12);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_DONT_DO_THE_DEWPRISM)}", 13);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160)}");
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_A_SOULFUL_REUNION)}", 14);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_THE_IMPERFECT_GENTLEMAN)}", 15);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_GENERATIONAL_BONDING)}", 16);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_NOT_FROM_AROUND_HERE)}", 17);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_GENTLEMEN_AT_HEART)}", 18);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)}");
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_DT_THE_CASE_OF_THE_DISPLACED_INSPECTOR)}", 19);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_DT_THE_CASE_OF_THE_FIENDISH_FUGITIVES)}", 20);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_DT_ON_THE_TRAIL_OF_DESTRUCTION)}", 21);
        }

        private void DrawRoleQuestQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var charactersRoleQuestQuestAll = ImRaii.Table("###CharactersProgress#All#RoleQuest", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersRoleQuestQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#RoleQuest#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#RoleQuest#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }
            ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                ImGui.EndTooltip();
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            List<List<bool>> charactersQuests = Utils.GetCharactersRoleQuestQuests(chars);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156)}");

            DrawAllRoleQuestLine(RoleIcon.Tank, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_SHB_TANK_TO_HAVE_LOVED_AND_LOST)}",
                0);
            DrawAllRoleQuestLine(RoleIcon.Melee, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_SHB_PHYSICAL_COURAGE_BORN_OF_FEAR)}",
                1, true);
            DrawAllRoleQuestLine(RoleIcon.Caster, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_SHB_MRDPS_A_TEARFUL_REUNION)}",
                2);
            DrawAllRoleQuestLine(RoleIcon.Heal, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_SHB_HEALER_THE_SOUL_OF_TEMPERANCE)}",
                3);
            DrawAllRoleQuestLine(null, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_SHB_MASTER_SAFEKEEPING)}", 4);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160)}");

            DrawAllRoleQuestLine(RoleIcon.Tank, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_EW_TANK_A_PATH_UNVEILED)}", 5);
            DrawAllRoleQuestLine(RoleIcon.Melee, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_EW_MELEE_TO_CALMER_SEAS)}", 6);
            DrawAllRoleQuestLine(RoleIcon.Ranged, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_EW_PRDPS_LAID_TO_REST)}", 7);
            DrawAllRoleQuestLine(RoleIcon.Caster, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_EW_MRDPS_EVER_MARCH_HEAVENSWARD)}",
                8);
            DrawAllRoleQuestLine(RoleIcon.Heal, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_EW_HEALER_THE_GIFT_OF_MERCY)}",
                9);
            DrawAllRoleQuestLine(null, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_EW_MASTER_FORLORN_GLORY)}", 10);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)}");

            DrawAllRoleQuestLine(RoleIcon.Tank, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_DT_TANK_DREAMS_OF_A_NEW_DAY)}",
                11);
            DrawAllRoleQuestLine(RoleIcon.Melee, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_DT_MELEE_A_HUNTER_TRUE)}", 12);
            DrawAllRoleQuestLine(RoleIcon.Ranged, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_DT_PRDPS_THE_MIGHTIEST_SHIELD)}",
                13);
            DrawAllRoleQuestLine(RoleIcon.Caster, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_DT_MRDPS_HEROES_AND_PRETENDERS)}",
                14);
            DrawAllRoleQuestLine(RoleIcon.Heal, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_DT_HEALER_AN_ANTIDOTE_FOR_ANARCHY)}",
                15);
            DrawAllRoleQuestLine(null, chars, charactersQuests,
                $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.ROLEQUEST_DT_MASTER_BAR_THE_PASSAGE)}",
                16);
        }

        private void DrawTribes(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var charactersTribeQuestAll = ImRaii.Table("###CharactersProgress#All#Tribe", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersTribeQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#Tribe#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#Tribe#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }
            ImGui.TableSetupScrollFreeze(-1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                ImGui.EndTooltip();
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            List<List<bool>> charactersQuests = Utils.GetCharactersTribeQuests(chars, _globalCache, _currentLocale);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752));
            DrawAllTribes(chars, charactersQuests, 1);
            DrawAllTribes(chars, charactersQuests, 2);
            DrawAllTribes(chars, charactersQuests, 3);
            DrawAllTribes(chars, charactersQuests, 4);
            DrawAllTribes(chars, charactersQuests, 5);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753));
            DrawAllTribes(chars, charactersQuests, 6);
            DrawAllTribes(chars, charactersQuests, 7);
            DrawAllTribes(chars, charactersQuests, 8);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754));
            DrawAllTribes(chars, charactersQuests, 9);
            DrawAllTribes(chars, charactersQuests, 10);
            DrawAllTribes(chars, charactersQuests, 11);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156)}");
            DrawAllTribes(chars, charactersQuests, 12);
            DrawAllTribes(chars, charactersQuests, 13);
            DrawAllTribes(chars, charactersQuests, 14);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160)}");
            DrawAllTribes(chars, charactersQuests, 15);
            DrawAllTribes(chars, charactersQuests, 16);
            DrawAllTribes(chars, charactersQuests, 17);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)}");
            DrawAllTribes(chars, charactersQuests, 18);
            DrawAllTribes(chars, charactersQuests, 19);
            DrawAllTribes(chars, charactersQuests, 20);
        }

        private static void DrawAllLine(List<Character> chars, List<List<bool>> charactersQuests, string name,
            int msqIndex)
        {
            //Plugin.Log.Debug($"DrawAllLine: {chars.Count}, name: {name}, msqIndex: {msqIndex}");
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(name);
            foreach ((List<bool> cq, int index) charactersQuest in charactersQuests.Select((cq, index) => (cq, index)))
            {
                ImGui.TableNextColumn();
                //ImGui.TextUnformatted(charactersQuest.cq[msqIndex] ? "\u2713" : "");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(charactersQuest.cq[msqIndex] ? FontAwesomeIcon.Check.ToIconString() : "");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(name);
                    ImGui.TextUnformatted(
                        $"{chars[charactersQuest.index].FirstName} {chars[charactersQuest.index].LastName}{(char)SeIconChar.CrossWorld}{chars[charactersQuest.index].HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }

        private void DrawAllTribes(List<Character> chars, List<List<bool>> charactersQuests, int tribeIndex)
        {
            //Plugin.Log.Debug($"DrawAlDrawAllTribeslLine: {chars.Count}, msqIndex: {tribeIndex}");
            BeastTribes? beastTribe = _globalCache.BeastTribesStorage.GetBeastTribe(_currentLocale, (uint)tribeIndex);
            if (beastTribe == null) return;
            string name = _currentLocale switch
            {
                ClientLanguage.German => beastTribe.GermanName,
                ClientLanguage.English => beastTribe.EnglishName,
                ClientLanguage.French => beastTribe.FrenchName,
                ClientLanguage.Japanese => beastTribe.JapaneseName,
                _ => beastTribe.EnglishName
            };
            BeastReputationRank? allied = _globalCache.BeastTribesStorage.GetRank(_currentLocale, 8);
            string alliedName = (allied == null) ? string.Empty : allied.Value.Name.ExtractText();
            name = $"{Utils.Capitalize(name)}";
            if (alliedName != string.Empty && tribeIndex != 12 && tribeIndex != 13 && tribeIndex != 14 && tribeIndex != 18)
                name = $"{name} ({alliedName})";

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(name);
            foreach ((List<bool> cq, int index) charactersQuest in charactersQuests.Select((cq, index) => (cq, index)))
            {
                ImGui.TableNextColumn();
                //ImGui.TextUnformatted(charactersQuest.cq[tribeIndex-1] ? "\u2713" : "");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(charactersQuest.cq[tribeIndex - 1] ? FontAwesomeIcon.Check.ToIconString() : "");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(name);
                    ImGui.TextUnformatted(
                        $"{chars[charactersQuest.index].FirstName} {chars[charactersQuest.index].LastName}{(char)SeIconChar.CrossWorld}{chars[charactersQuest.index].HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }

        private void DrawAllRoleQuestLine(RoleIcon? icon, List<Character> chars, List<List<bool>> charactersQuests, string name,
            int msqIndex, bool shadowbringer = false)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawRoleQuestIcons(icon, shadowbringer);
            ImGui.SameLine();
            ImGui.TextUnformatted(name);
            foreach ((List<bool> cq, int index) charactersQuest in charactersQuests.Select((cq, index) => (cq, index)))
            {
                ImGui.TableNextColumn();
                //ImGui.TextUnformatted(charactersQuest.cq[msqIndex] ? "\u2713" : "");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(charactersQuest.cq[msqIndex] ? FontAwesomeIcon.Check.ToIconString() : "");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(name);
                    ImGui.TextUnformatted(
                        $"{chars[charactersQuest.index].FirstName} {chars[charactersQuest.index].LastName}{(char)SeIconChar.CrossWorld}{chars[charactersQuest.index].HomeWorld}");
                    ImGui.EndTooltip();
                }
            }
        }

        private void DrawRoleQuestIcons(RoleIcon? icon, bool shadowbringer = false)
        {
            if (_rolesTextureWrap is null) return;
            switch (icon)
            {
                case null:
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(062576), new Vector2(22, 22));
                    break;
                case RoleIcon.Tank:
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Tank, new Vector2(20, 20));
                    break;
                case RoleIcon.Melee:
                    if (shadowbringer)
                    {
                        Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Melee, new Vector2(20, 20));
                        ImGui.SameLine();
                        Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Ranged, new Vector2(20, 20));
                    }
                    else
                    {
                        Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Melee, new Vector2(20, 20));
                    }

                    break;
                case RoleIcon.Ranged:
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Ranged, new Vector2(20, 20));
                    break;
                case RoleIcon.Caster:
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Caster, new Vector2(20, 20));
                    break;
                case RoleIcon.Heal:
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Heal, new Vector2(20, 20));
                    break;
            }
        }

        public void DrawTabs(Character selectedCharacter)
        {
            using var tab =
                ImRaii.TabBar($"###CharactersProgressTable#ProgressTabs#{selectedCharacter.CharacterId}#TabBar");
            if (!tab) return;

            using ImRaii.IEndObject reputationTab =
                ImRaii.TabItem(
                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102512)}###CharactersProgressTable#ProgressTabs#{selectedCharacter.CharacterId}#TabBar#Tabs#Reputation");
            if (reputationTab.Success)
            {
                DrawReputations(selectedCharacter);
            }
        }

        private void DrawReputations(Character selectedCharacter)
        {
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102513)}");
            ImGui.Separator();
            if (_commendationIcon != null)
            {
                (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_commendationIcon.Size, 320, 208, 64, 64);
                ImGui.Image(_commendationIcon.Handle, new Vector2(32, 32), uv0, uv1);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102520)}");
                ImGui.EndTooltip();
            }

            ImGui.SameLine();
            ImGui.TextUnformatted($"{selectedCharacter.PlayerCommendations}");

            if (!selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_PIXIES) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_QITARI) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_DWARVES) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_PELUPELU) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY))
            {
                return;
            }

            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102515)}");
            ImGui.Separator();
            if (_chevronTexture != null)
            {
                if (_rightChevron)
                {
                    _downChevron = false;
                    (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_chevronTexture.Size, 0, 0, 48, 48);
                    ImGui.Image(_chevronTexture.Handle, new Vector2(24, 24), uv0, uv1);
                }

                if (_downChevron)
                {
                    _rightChevron = false;
                    (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_chevronTexture.Size, 48, 0, 48, 48);
                    ImGui.Image(_chevronTexture.Handle, new Vector2(24, 24), uv0, uv1);
                }
            }

            ImGui.SameLine();
            bool arrUnlocked = false;
            bool hwUnlocked = false;
            bool sbUnlocked = false;
            bool shbUnlocked = false;
            bool ewUnlocked = false;
            bool dtUnlocked = false;
            List<string> expansionNames = [];
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL)
               )
            {
                expansionNames.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752));
                arrUnlocked = true;
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES)
               )
            {
                expansionNames.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753));
                hwUnlocked = true;

                if (!_hasValueBeenSelected && !arrUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753);
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU)
               )
            {
                expansionNames.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754));
                sbUnlocked = true;

                if (!_hasValueBeenSelected && !hwUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754);
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_PIXIES) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_QITARI) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_DWARVES)
               )
            {
                shbUnlocked = true;
                expansionNames.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156));
                if (!_hasValueBeenSelected && !sbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156);
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS)
               )
            {
                expansionNames.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160));
                ewUnlocked = true;
                if (!_hasValueBeenSelected && !shbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160);
            }
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_PELUPELU) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY)
                  )
            {
                expansionNames.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175));
                dtUnlocked = true;
                if (!_hasValueBeenSelected && !ewUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175);
            }

            using (var combo = ImRaii.Combo("###CharactersProgress#Reputations#Combo", _selectedExpansion))
            {
                if (combo)
                {
                    foreach (string name in expansionNames.Where(name => ImGui.Selectable(name, name == _selectedExpansion)))
                    {
                        _selectedExpansion = name;
                        _rightChevron = true;
                        _downChevron = false;
                        _hasValueBeenSelected = true;
                    }
                }
            }
            if (ImGui.IsItemClicked())
            {
                _rightChevron = false;
                _downChevron = true;
            }

            using ImRaii.IEndObject charactersReputationTable = ImRaii.Table("###CharactersProgress#Reputations##Reputation", 1, ImGuiTableFlags.ScrollY);
            if (!charactersReputationTable) return;
            ImGui.TableSetupColumn("###CharactersProgress#ReputationsTable", ImGuiTableColumnFlags.WidthFixed, 560);
            switch (_selectedExpansion)
            {
                case "A Realm Reborn":
                case "新生エオルゼア":
                    {
                        bool arrAllied = selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_ALLIED);
                        for (uint i = 1; i <= 5; i++)
                        {
                            if (
                                i == 1 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) ||
                                i == 2 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) ||
                                i == 3 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) ||
                                i == 4 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) ||
                                i == 5 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, arrAllied);
                            DrawReward(selectedCharacter, i, b.Rank, arrAllied);
                        }
                    }
                    break;
                case "Heavensward":
                case "蒼天のイシュガルド":
                    {
                        bool hwAllied = selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_ALLIED);
                        for (uint i = 6; i <= 8; i++)
                        {
                            if (
                                i == 6 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) ||
                                i == 7 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH) ||
                                i == 8 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, hwAllied);
                            DrawReward(selectedCharacter, i, b.Rank, hwAllied);
                        }
                    }
                    break;
                case "Stormblood":
                case "紅蓮のリベレーター":
                    {
                        bool sbAllied = selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ALLIED);
                        for (uint i = 9; i <= 11; i++)
                        {
                            if (
                                i == 9 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN) ||
                                i == 10 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA) ||
                                i == 11 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, sbAllied);
                            DrawReward(selectedCharacter, i, b.Rank, sbAllied);
                        }
                    }
                    break;
                case "Shadowbringers":
                case "漆黒編":
                    {
                        for (uint i = 12; i <= 14; i++)
                        {
                            if (
                                i == 12 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_PIXIES) ||
                                i == 13 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_QITARI) ||
                                i == 14 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_DWARVES)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, false);
                            DrawReward(selectedCharacter, i, b.Rank, false);
                        }
                    }
                    break;
                case "Endwalker":
                case "暁月編":
                    {
                        bool ewAllied = selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ALLIED);
                        for (uint i = 15; i <= 17; i++)
                        {
                            if (
                                i == 15 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) ||
                                i == 16 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) ||
                                i == 17 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, ewAllied);
                            DrawReward(selectedCharacter, i, b.Rank, ewAllied);
                        }
                    }
                    break;
                case "Dawntrail":
                case "黄金編":
                    {
                        //bool dtAllied = selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_ALLIED);
                        bool dtAllied = false;
                        for (uint i = 18; i <= 20; i++)
                        {
                            if (
                                i == 18 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_PELUPELU) ||
                                i == 19 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA) ||
                                i == 20 && !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, dtAllied);
                            DrawReward(selectedCharacter, i, b.Rank, dtAllied);
                        }
                    }
                    break;
            }
        }

        private void DrawReputationLine(uint id, uint currentExp, uint rank, bool isAllied)
        {
            //Plugin.Log.Debug($"id: {id}, currentExp: {currentExp}, rank: {rank}, isAllied: {isAllied}");
            BeastTribes? beastTribe = _globalCache.BeastTribesStorage.GetBeastTribe(_currentLocale, id);
            if (beastTribe == null) return;

            using (var charactersReputationsReputationLine = ImRaii.Table($"###CharactersProgress#Reputations##Reputation#ReputationLine#{id}", 2))
            {
                if (!charactersReputationsReputationLine) return;
                ImGui.TableSetupColumn($"###CharactersProgress#Reputations##Reputation#ReputationLine#{id}Icon", ImGuiTableColumnFlags.WidthFixed, 36);
                ImGui.TableSetupColumn($"###CharactersProgress#Reputations##Reputation#ReputationLine#{id}#RankNameExp", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(beastTribe.Icon), new Vector2(36, 36));
                ImGui.TableSetColumnIndex(1);
                string name = _currentLocale switch
                {
                    ClientLanguage.German => beastTribe.GermanName,
                    ClientLanguage.English => beastTribe.EnglishName,
                    ClientLanguage.French => beastTribe.FrenchName,
                    ClientLanguage.Japanese => beastTribe.JapaneseName,
                    _ => beastTribe.EnglishName
                };
                ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
            }

            Utils.DrawReputationProgressBar(_currentLocale, _globalCache, currentExp, beastTribe.MaxRank == rank, rank, isAllied);
        }

        private void DrawReward(Character currentCharacter, uint id, uint rank, bool isAllied)
        {
            if (rank < 3) return;
            //Plugin.Log.Debug($"DrawReward: id={id}, rank={rank}, isAllied={isAllied}");
            switch (id)
            {
                case 1: //Amaal'ja
                    {
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(85, currentCharacter.HasOrchestrionRoll(85));
                            ImGui.TableSetColumnIndex(1);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43957);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));

                            if (rank >= 4)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(58, currentCharacter.HasMinion(58));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(19, currentCharacter.HasMount(19));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMinion(124, currentCharacter.HasMinion(124));
                            }
                        }

                        break;
                    }
                case 2: //Sylph
                    {
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(117, currentCharacter.HasOrchestrionRoll(117));
                            ImGui.TableSetColumnIndex(1);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43956);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));

                            if (rank >= 4)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(50, currentCharacter.HasMinion(50));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(20, currentCharacter.HasMount(20));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMinion(123, currentCharacter.HasMinion(123));
                            }
                        }

                        break;
                    }
                case 3: //Kobold
                    {
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43955);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            if (rank >= 4)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMinion(60, currentCharacter.HasMinion(60));
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(27, currentCharacter.HasMount(27));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(126, currentCharacter.HasMinion(126));
                            }
                        }

                        break;
                    }
                case 4: //Sahagin
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(61, currentCharacter.HasMinion(61));
                            ImGui.TableSetColumnIndex(1);
                            DrawMount(26, currentCharacter.HasMount(26));
                            ImGui.TableSetColumnIndex(2);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43958);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(127, currentCharacter.HasMinion(127));
                            }
                        }

                        break;
                    }
                case 5: //Ixal
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(59, currentCharacter.HasMinion(59));
                            ImGui.TableSetColumnIndex(1);
                            DrawMount(35, currentCharacter.HasMount(35));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(2);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43959);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(125, currentCharacter.HasMinion(125));
                            }
                        }

                        break;
                    }
                case 6: //Vanu Vanu
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(86, currentCharacter.HasOrchestrionRoll(86));
                            ImGui.TableSetColumnIndex(1);
                            DrawMinion(135, currentCharacter.HasMinion(135));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawEmote(120, currentCharacter.HasEmote(120));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(172, currentCharacter.HasMinion(172));
                                ImGui.TableSetColumnIndex(4);
                                DrawMount(53, currentCharacter.HasMount(53));
                            }
                            if (rank == 8)
                            {
                                ImGui.TableSetColumnIndex(5);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41371);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 7: //Vath
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(118, currentCharacter.HasOrchestrionRoll(118));
                            ImGui.TableSetColumnIndex(1);
                            DrawMinion(175, currentCharacter.HasMinion(175));

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(156, currentCharacter.HasMinion(156));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(72, currentCharacter.HasMount(72));
                            }
                            if (rank == 8)
                            {
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41372);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 8: //Moogle
                    {
                        if (rank < 7) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawEmote(126, currentCharacter.HasEmote(126));
                            ImGui.TableSetColumnIndex(1);
                            DrawMinion(184, currentCharacter.HasMinion(184));
                            ImGui.TableSetColumnIndex(2);
                            DrawMount(86, currentCharacter.HasMount(86));

                            if (rank == 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41373);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMinion(235, currentCharacter.HasMinion(235));
                            }
                        }

                        break;
                    }
                case 9: //Kojin
                    {
                        if (rank < 5) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 7);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion3",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(266, currentCharacter.HasMinion(266));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(179, currentCharacter.HasOrchestrionRoll(179));
                            }


                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawEmote(167, currentCharacter.HasEmote(167));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(136, currentCharacter.HasMount(136));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40502);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(5);
                                DrawMinion(323, currentCharacter.HasMinion(323));
                                ImGui.TableSetColumnIndex(6);
                                DrawMinion(328, currentCharacter.HasMinion(328));
                            }
                        }

                        break;
                    }
                case 10: //Ananta
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 7);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(277, currentCharacter.HasMinion(277));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawEmote(64, currentCharacter.HasEmote(64));
                                ImGui.TableSetColumnIndex(2);
                                DrawOrchestrion(207, currentCharacter.HasOrchestrionRoll(207));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(146, currentCharacter.HasMount(146));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMount(148, currentCharacter.HasMount(148));
                                ImGui.TableSetColumnIndex(5);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40503);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(6);
                                DrawMinion(322, currentCharacter.HasMinion(322));
                            }
                        }

                        break;
                    }
                case 11: //Namazu
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(302, currentCharacter.HasMinion(302));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawEmote(176, currentCharacter.HasEmote(176));
                                ImGui.TableSetColumnIndex(2);
                                DrawOrchestrion(231, currentCharacter.HasOrchestrionRoll(231));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(164, currentCharacter.HasMount(164));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40504);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 12: //Pixie
                    {
                        if (rank < 6) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(354, currentCharacter.HasMinion(354));

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMount(201, currentCharacter.HasMount(201));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawOrchestrion(345, currentCharacter.HasOrchestrionRoll(345));
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39572);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 13: //Qitari
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(369, currentCharacter.HasMinion(369));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(371, currentCharacter.HasOrchestrionRoll(371));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(215, currentCharacter.HasMount(215));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(370, currentCharacter.HasMinion(370));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39573);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 14: //Dwarf
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(380, currentCharacter.HasMinion(380));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(383, currentCharacter.HasOrchestrionRoll(383));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(223, currentCharacter.HasMount(223));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawEmote(199, currentCharacter.HasEmote(199));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39574);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 15: //Arkasodara
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#TTCard1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(444, currentCharacter.HasMinion(444));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawTripleTriadCard(349, currentCharacter.HasTTC(349));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(287, currentCharacter.HasMount(287));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawOrchestrion(514, currentCharacter.HasOrchestrionRoll(514));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39575);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 16: //Omicron
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#TTCard1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawTripleTriadCard(357, currentCharacter.HasTTC(357));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMinion(457, currentCharacter.HasMinion(457));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(298, currentCharacter.HasMount(298));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(38466);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(4);
                                DrawOrchestrion(514, currentCharacter.HasOrchestrionRoll(514));
                            }
                        }

                        break;
                    }
                case 17: //Loporrit
                    {
                        if (rank < 5) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(472, currentCharacter.HasMinion(472));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(582, currentCharacter.HasOrchestrionRoll(582));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(285, currentCharacter.HasMount(285));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawEmote(252, currentCharacter.HasEmote(252));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39576);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(5);
                                DrawOrchestrion(566, currentCharacter.HasOrchestrionRoll(566));
                            }
                        }

                        break;
                    }
                case 18: //Pelupelu
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Accessory",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(534, currentCharacter.HasMinion(534));
                            ImGui.TableSetColumnIndex(1);
                            DrawOrchestrion(708, currentCharacter.HasOrchestrionRoll(708));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(358, currentCharacter.HasMount(358));
                            }

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawOrnament(44, currentCharacter.HasOrnament(44));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(44944);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 19: //Mamool Ja
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Accessory",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMount(381, currentCharacter.HasMount(381));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(728, currentCharacter.HasOrchestrionRoll(728));
                            }

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(545, currentCharacter.HasMinion(545));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(48085);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(4);
                                DrawTripleTriadCard(441, currentCharacter.HasTTC(441));
                            }
                        }

                        break;
                    }
                case 20: //Yok Huy
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Accessory",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(770, currentCharacter.HasOrchestrionRoll(770));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMinion(554, currentCharacter.HasMinion(554));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(393, currentCharacter.HasMount(393));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(46767);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(4);
                                DrawTripleTriadCard(453, currentCharacter.HasTTC(453));
                            }
                        }

                        break;
                    }
            }
        }

        private void DrawMinion(uint id, bool hasMinion)
        {
            Vector2 p = ImGui.GetCursorPos();
            Minion? m = _globalCache.MinionStorage.GetMinion(_currentLocale, id);
            if (m == null)
            {
                return;
            }

            if (!hasMinion)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawMinionTooltip(_currentLocale, ref _globalCache, m);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMinionTooltip(_currentLocale, ref _globalCache, m);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawMount(uint id, bool hasMount)
        {
            Vector2 p = ImGui.GetCursorPos();
            Mount? m = _globalCache.MountStorage.GetMount(_currentLocale, id);
            if (m == null)
            {
                return;
            }

            if (!hasMount)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawMountTooltip(_currentLocale, ref _globalCache, m);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMountTooltip(_currentLocale, ref _globalCache, m);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawEmote(uint id, bool hasEmote)
        {
            Vector2 p = ImGui.GetCursorPos();
            Emote? e = _globalCache.EmoteStorage.GetEmote(_currentLocale, id);
            if (e == null)
            {
                return;
            }

            if (!hasEmote)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(e.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawEmoteTooltip(_currentLocale, ref _globalCache, e);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(e.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawEmoteTooltip(_currentLocale, ref _globalCache, e);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawOrchestrion(uint id, bool hasOrchestrion)
        {
            Vector2 p = ImGui.GetCursorPos();
            OrchestrionRoll? o = _globalCache.OrchestrionRollStorage.GetOrchestrionRoll(_currentLocale, id);
            if (o == null)
            {
                return;
            }

            if (!hasOrchestrion)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawOrchestrionRollTooltip(_currentLocale, ref _globalCache, o);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrchestrionRollTooltip(_currentLocale, ref _globalCache, o);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawFramerKit(uint id, bool hasFramerKit)
        {
            Vector2 p = ImGui.GetCursorPos();
            FramerKit? f = _globalCache.FramerKitStorage.LoadItem(_currentLocale, id);
            if (f == null)
            {
                return;
            }
            //Plugin.Log.Debug($"fk: {f.Id} name:{f.EnglishName}, icon:{f.Icon} itemId:{f.ItemId}");
            if (!hasFramerKit)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, f);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, f);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawTripleTriadCard(uint id, bool hasTripleTriadCard)
        {
            Vector2 p = ImGui.GetCursorPos();
            TripleTriadCard? t = _globalCache.TripleTriadCardStorage.GetTripleTriadCard(_currentLocale, id);
            if (t == null)
            {
                return;
            }
            //Plugin.Log.Debug($"fk: {f.Id} name:{f.EnglishName}, icon:{f.Icon} itemId:{f.ItemId}");
            if (!hasTripleTriadCard)
            {
                if (_isSpoilerEnabled)
                {
                    //Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32),
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(027672), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, t);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                //Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32));
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(027672), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, t);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawOrnament(uint id, bool hasOrnament)
        {
            Vector2 p = ImGui.GetCursorPos();
            Ornament? o = _globalCache.OrnamentStorage.GetOrnament(_currentLocale, id);
            if (o == null)
            {
                return;
            }

            if (!hasOrnament)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawOrnamentTooltip(_currentLocale, ref _globalCache, o);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrnamentTooltip(_currentLocale, ref _globalCache, o);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }
    }
}
