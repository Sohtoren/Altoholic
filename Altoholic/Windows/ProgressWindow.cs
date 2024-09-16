using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

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
                MinimumSize = new Vector2(1000, 450), MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;

            _commendationIcon = Plugin.TextureProvider.GetFromGame("ui/uld/Character_hr1.tex").RentAsync().Result;
            _chevronTexture = Plugin.TextureProvider.GetFromGame("ui/uld/fourth/ListItemB_hr1.tex").RentAsync().Result;

            _currentLocale = _plugin.Configuration.Language;
            _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752);
        }

        public Func<Character> GetPlayer { get; set; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;

        private string _selectedExpansion;

        private readonly IDalamudTextureWrap? _commendationIcon;
        private readonly IDalamudTextureWrap? _chevronTexture;

        private bool _rightChevron = true;
        private bool _downChevron;
        private bool _hasValueBeenSelected;

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
                       $"{(_currentLocale == ClientLanguage.Japanese ? "ヒルディブランド": "Hildibrand")}###CharactersProgressTable#All#TabBar#Hildibrand"))
            {
                if (hildibrandTab)
                {
                    DrawHildibrandQuest(chars);
                }
            }

            using (var msqTab = ImRaii.TabItem("MSQ###CharactersProgressTable#All#TabBar#MSQ"))
            {
                if (msqTab)
                {
                    DrawMainScenarioQuest(chars);
                }
            }
        }

        private void DrawMainScenarioQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var characterswMainScenarioQuestAll = ImRaii.Table("###CharactersProgress#All#MSQ", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!characterswMainScenarioQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#MSQ#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#MSQ#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 15);
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
        }

        private void DrawEventQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            List<List<bool>> charactersQuests = Utils.GetCharactersEventsQuests(chars);
            ImGui.TextUnformatted($"{Loc.Localize("RecurringEvent",
                "* As certain event do not change when reoccuring, completing them once will mark all of them done.")}");

            ImGui.TextUnformatted($"{Loc.Localize("Blunderville",
                "** For the Blunderville event, the introduction quest is used for completion.")
            }");

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

            using (var progressEvent2024Tab = ImRaii.TabItem("2024###progressEvent#Tabs#2024"))
            {
                if (progressEvent2024Tab.Success)
                {
                    using (var charactersEventTable = ImRaii.Table(
                               $"###CharactersProgress#All#Event#2024#Table",
                               chars.Count + 1,
                               ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                               ImGuiTableFlags.ScrollX, new Vector2(-1, 315)))
                    {
                        if (!charactersEventTable) return;

                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2024#Name",
                            ImGuiTableColumnFlags.WidthFixed, 260);
                        foreach (Character c in chars)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#2024#{c.CharacterId}",
                                ImGuiTableColumnFlags.WidthFixed, 15);
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
                        /*DrawAllLine(chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2024)",
                            120);*/
                    }
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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
                            ImGuiTableColumnFlags.WidthFixed, 15);
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


            watch.Stop();
            Plugin.Log.Debug($"watch.Elapsed.Microseconds: {watch.Elapsed.Microseconds}");
        }

        private void DrawHildibrandQuest(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var characterswHildibrandQuestAll = ImRaii.Table("###CharactersProgress#All#Hildibrand", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!characterswHildibrandQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#Hildibrand#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#Hildibrand#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 15);
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

            List<List<bool>> charactersQuests = Utils.GetCharactersHildibrandQuests(chars);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_THE_IMMACULATE_DECEPTION)}", 0);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_THE_THREE_COLLECTORS)}", 1);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_A_CASE_OF_INDECENCY)}", 2);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_THE_COLISEUM_CONUNDRUM)}", 3);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_ARR_HER_LAST_VOW)}", 4);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_DONT_CALL_IT_A_COMEBACK)}", 5);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_THE_MEASURE_OF_A_MAMMET)}", 6);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_DONT_TRUST_ANYONE_OVER_SIXTY)}", 7);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_HW_IF_I_COULD_TURN_BACK_TIME)}", 8);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_A_HINGAN_TALE_NASHU_GOES_EAST)}", 9);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_OF_WOLVES_AND_GENTLEMEN)}", 10);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_THE_BLADE_MISLAID)}", 11);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_GOOD_SWORDS_GOOD_DOGS)}", 12);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_SB_DONT_DO_THE_DEWPRISM)}", 13);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_A_SOULFUL_REUNION)}", 14);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_THE_IMPERFECT_GENTLEMAN)}", 15);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_GENERATIONAL_BONDING)}", 16);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_NOT_FROM_AROUND_HERE)}", 17);
            DrawAllLine(chars, charactersQuests, $"{_globalCache.QuestStorage.GetQuestName(_currentLocale, (int)QuestIds.HILDIBRAND_EW_GENTLEMEN_AT_HEART)}", 18);

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
                ImGui.TextUnformatted(charactersQuest.cq[msqIndex] ? "\u2713" : "");
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

        public void DrawTabs(Character selectedCharacter)
        {
            using var tab =
                ImRaii.TabBar($"###CharactersProgressTable#ProgressTabs#{selectedCharacter.CharacterId}#TabBar");
            if (!tab) return;

            using (ImRaii.IEndObject reputationTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102512)}###CharactersProgressTable#ProgressTabs#{selectedCharacter.CharacterId}#TabBar#Tabs#Reputation"))
            {
                if (reputationTab.Success)
                {
                    DrawReputations(selectedCharacter);
                }
            }
        }

        private void DrawReputations(Character selectedCharacter)
        {
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102513)}");
            ImGui.Separator();
            if (_commendationIcon != null)
            {
                (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_commendationIcon.Size, 320, 208, 64, 64);
                ImGui.Image(_commendationIcon.ImGuiHandle, new Vector2(32, 32), uv0, uv1);
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
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS))
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
                    ImGui.Image(_chevronTexture.ImGuiHandle, new Vector2(24, 24), uv0, uv1);
                }

                if (_downChevron)
                {
                    _rightChevron = false;
                    (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_chevronTexture.Size, 48, 0, 48, 48);
                    ImGui.Image(_chevronTexture.ImGuiHandle, new Vector2(24, 24), uv0, uv1);
                }
            }

            ImGui.SameLine();
            bool arrUnlocked = false;
            bool hwUnlocked = false;
            bool sbUnlocked = false;
            bool shbUnlocked = false;
            bool ewUnlocked = false;
            List<string> names = [];
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752));
                arrUnlocked = true;
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753));
                hwUnlocked = true;

                if (!_hasValueBeenSelected && !arrUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753);
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754));
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
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156));
                if (!_hasValueBeenSelected && !sbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156);
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160));
                ewUnlocked = true;
                if (!_hasValueBeenSelected && !shbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160);
            }


            /*
             if (selectedCharacter.HasQuest() &&
                   selectedCharacter.HasQuest() &&
                   selectedCharacter.HasQuest())
               {
                   names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175));//DT
                    dtUnlocked = true;
                    if (!ewUnlocked)
                        _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)
             */

            using (var combo = ImRaii.Combo("###CharactersProgress#Reputations#Combo", _selectedExpansion))
            {
                if (combo)
                {
                    foreach (string name in names.Where(name => ImGui.Selectable(name, name == _selectedExpansion)))
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

            // Todo: if spoiler enabled, add a unique reward list (orchestrions, pets, framerkits) here and check it if brought/unlocked.
            // If spoiler is disabled, unveil each reward by reputation rank
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
                ImGui.TextUnformatted("\u2713");
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
                ImGui.TextUnformatted("\u2713");
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
                ImGui.TextUnformatted("\u2713");
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
                ImGui.TextUnformatted("\u2713");
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
                ImGui.TextUnformatted("\u2713");
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32),
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
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, t);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
                ImGui.SetCursorPos(p);
            }
        }
    }
}
