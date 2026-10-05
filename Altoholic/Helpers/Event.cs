using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Helpers
{
    public abstract class Event
    {
        private static List<List<bool>> GetCharactersEventsQuests(List<Character> characters)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                bool allsaintswake = (character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_GRIDANIA) ||
                                      character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_LIMSA) ||
                                      character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_ULDAH));
                List<bool> completedQuests =
                [
                    /*character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2010),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2011),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2011),
                    character.HasQuest((int)QuestIds.EVENT_FIREFALL_FAIRE_2011),
                    character.HasQuest((int)QuestIds.EVENT_HUNTER_S_MOON_2011),
                    character.HasQuest((int)QuestIds.EVENT_FOUNDATION_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2011),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2011),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2012),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2012),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2012),
                    character.HasQuest((int)QuestIds.EVENT_FOUNDATION_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2013),*/
                    allsaintswake,
                    character.HasQuest((int)QuestIds.EVENT_LIGHTNING_STRIKES_2013),
                    //character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2013),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2014),
                    character.HasQuest((int)QuestIds.EVENT_BURGEONING_DREAD_2014),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2014),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2014),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2014),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2014),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2014),
                    character.HasQuest((int)QuestIds.EVENT_THAT_OLD_BLACK_MAGIC_2014),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2014),
                    character.HasQuest((int)QuestIds.EVENT_LIGHTNING_RETURNS),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2_2014),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2014),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2014),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2015),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2015),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2015),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2015),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2015),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2015),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2015),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2015),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2015),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2016),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2016),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2016),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2016),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2016),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2016),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2016),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2016),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2016),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2016),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2017),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2017),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2017),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2017),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2017),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2017),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2017),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2017),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2017),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2018),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2018),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2018),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2018),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2018),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_HUNT_FOR_RATHALOS),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2018),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2019),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2019),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2019),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2019),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2019),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_PHILOSOPHY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2019),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2019),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2019),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_MYTHOLOGY),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2019),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2019),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2020),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_SOLDIERY),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2020),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2020),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2020),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_LAW),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2020),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2020),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2020),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2020),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2020),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2020),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2020),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2021),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_AND_LITTLE_LADIES_DAY_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_ESOTERICS),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_FESTIVAL_2021_THE_HUNT_FOR_PAGEANTRY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2021),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2021),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2021),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2021),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_LORE),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2021),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2022),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2021),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_SCRIPTURE),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2022),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_VERITY),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2022),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_CREATION),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2022),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2023),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2023),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2023),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2023),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_MENDACITY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2023),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2023),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2023),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_10TH_ANNIVERSARY_HUNT),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2023),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2023),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2023),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_FIRST_HUNT_FOR_GENESIS),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2024),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2024),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_HATCHING_TIDE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_PATH_INFERNAL_2024),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_SECOND_HUNT_FOR_GENESIS),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2024),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2024),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024_1),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_GOETIA),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2024),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024_2),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2024),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2025),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2025),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2025),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2025),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2025),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2025),
                    character.HasQuest((int)QuestIds.EVENT_RISING_2025),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2025),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_2025),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2026),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2026),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2026),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2026),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2026),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2026),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2026),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2026),
                    character.HasQuest((int)QuestIds.EVENT_RISING_2026),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2026),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2026),
                ];
                result.Add(completedQuests);
            }

            return result;
        }
        public static uint GetEventCurrencyFromEventId(int msqIndex)
        {
            return msqIndex switch
            {
                132 => 47863,
                133 => 50082,
                134 => 50089,
                136 or 138 or 139 => 1,
                _ => 0
            };
        }

        private static (uint, uint) GetEventCurrenciesFromEventId(int msqIndex)
        {
            return msqIndex switch
            {
                140 => (29,25007),
                _ => (0,0)
            };
        }

        private static void DrawRewardsModal(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int msqIndex)
        {
            if (ImGui.IsItemClicked())
            {
                ImGui.OpenPopup(
                    $"###CharactersProgress#All#Event#RewardModal#{msqIndex}");
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ClickToDisplayRewards", "Click to display rewards")}");
                ImGui.EndTooltip();
            }

            //ImGui.SetNextWindowSize(new Vector2(800, 400));
            ImGui.SetNextWindowSizeConstraints(new Vector2(800, 400), new Vector2(2000, 800));
            using var rewardModal = ImRaii.PopupModal($"###CharactersProgress#All#Event#RewardModal#{msqIndex}");
            if (!rewardModal)
            {
                return;
            }

            if (ImGui.Button(globalCache.AddonStorage.LoadAddonString(currentLocale, 1219), new Vector2(120, 0)))
            {
                ImGui.CloseCurrentPopup();
            }

            uint eventCurrencyId = GetEventCurrencyFromEventId(msqIndex);

            int columns = chars.Count + 1;
            if (eventCurrencyId > 0)
            {
                columns += 1;
            }

            float nameSize = GetEventRewardLongestName(currentLocale, globalCache, msqIndex);

            using var charactersEventTable = ImRaii.Table(
            $"###CharactersProgress#All#Event#RewardTable#{msqIndex}",
            columns,
            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY, new Vector2(-1, 330));
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#RewardTable#{msqIndex}#Name",
                ImGuiTableColumnFlags.WidthFixed, nameSize + 5 + 32); // text + margin + icon sizes
            if (eventCurrencyId > 0)
            {
                if (eventCurrencyId is 29)
                {
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#RewardTable#{msqIndex}#Currency",
                        ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("100000").X + 5);
                }
                else
                {
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#RewardTable#{msqIndex}#Currency",
                        ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
                }
            }
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#RewardTable#{msqIndex}#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            //ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
            ImGui.TableSetupScrollFreeze(1, 1);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

            if (eventCurrencyId > 0)
            {
                Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, eventCurrencyId);
                if (itm != null)
                {
                    ImGui.TableSetColumnIndex(1);
                    Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                    }
                }
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

            DrawEventReward(currentLocale, globalCache, chars, msqIndex);
        }

        private static void DrawMultiTabRewardsModal(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int msqIndex)
        {
            if (ImGui.IsItemClicked())
            {
                ImGui.OpenPopup(
                    $"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}");
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ClickToDisplayRewards", "Click to display rewards")}");
                ImGui.EndTooltip();
            }

            //ImGui.SetNextWindowSize(new Vector2(800, 400));
            ImGui.SetNextWindowSizeConstraints(new Vector2(800, 400), new Vector2(2000, 800));
            using var rewardModal = ImRaii.PopupModal($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}");
            if (!rewardModal)
            {
                return;
            }

            if (ImGui.Button(globalCache.AddonStorage.LoadAddonString(currentLocale, 1219), new Vector2(120, 0)))
            {
                ImGui.CloseCurrentPopup();
            }

            (uint, uint) eventCurrencyIds = GetEventCurrenciesFromEventId(msqIndex);

            int columns = chars.Count + 1;
            if (eventCurrencyIds.Item1 > 0)
            {
                columns += 1;
            }

            float nameSize = GetEventRewardLongestName(currentLocale, globalCache, msqIndex);

            using var tabBar = ImRaii.TabBar($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#TabBar");
            if (!tabBar.Success) return;

            using (var collectableTab =
                ImRaii.TabItem(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1456)}###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#1#Collectable"))
            {
                if (collectableTab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#1",
                        columns,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY, new Vector2(-1, 330));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#1#Name",
                        ImGuiTableColumnFlags.WidthFixed, nameSize + 5 + 32); // text + margin + icon sizes
                    if (eventCurrencyIds.Item1 > 0)
                    {
                        if (eventCurrencyIds.Item1 is 29)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#1#Currency",
                                ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("100000").X + 5);
                        }
                        else
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#1#Currency",
                                ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
                        }
                    }
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#1#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    //ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrollingTableSetupScrollFreeze(1, 1);
                    ImGui.TableSetupScrollFreeze(1, 1);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                    if (eventCurrencyIds.Item1 > 0)
                    {
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, eventCurrencyIds.Item1);
                        if (itm != null)
                        {
                            ImGui.TableSetColumnIndex(1);
                            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                            }
                        }
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
                    DrawEventReward(currentLocale, globalCache, chars, msqIndex);
                }
            }
            using (var OrchestrionsTab =
            ImRaii.TabItem(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 832)}###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#2#Orchestrions"))
            {
                if (OrchestrionsTab.Success)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#2",
                        columns,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY, new Vector2(-1, 330));
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#2#Name",
                        ImGuiTableColumnFlags.WidthFixed, nameSize + 5 + 32); // text + margin + icon sizes
                    if (eventCurrencyIds.Item2 > 0)
                    {
                        if (eventCurrencyIds.Item2 is 29)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#2#Currency",
                                ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("100000").X + 5);
                        }
                        else
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#2#Currency",
                                ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
                        }
                    }
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MultiRewardModal#{msqIndex}#2#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    //ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
                    ImGui.TableSetupScrollFreeze(1, 1);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                    if (eventCurrencyIds.Item2 > 0)
                    {
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, eventCurrencyIds.Item2);
                        if (itm != null)
                        {
                            ImGui.TableSetColumnIndex(1);
                            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                            }
                        }
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
                    DrawEventReward(currentLocale, globalCache, chars, -msqIndex);
                }
            }
        }

        public static void DrawQuest(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars)
        {
            if (chars.Count == 0) return;
            List<List<bool>> charactersQuests = Helpers.Event.GetCharactersEventsQuests(chars);
            ImGui.TextUnformatted($"* {Loc.Localize("ProgressRecurringEvent",
                "As certain event do not change when reoccuring, completing them once will mark all of them done.")}");

            ImGui.TextUnformatted($"** {Loc.Localize("ProgressEventBlundervilleMessage",
                "For the Blunderville event, the introduction quest is used for completion.")}");

            using var tabBar = ImRaii.TabBar("###progressEvent#Tabs", ImGuiTabBarFlags.Reorderable);
            if (!tabBar.Success) return;
            //Plugin.Log.Debug($"charactersEventQuests: {charactersQuests.Count}");
            /*DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2010)", 0);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2011)", 1);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2011)", 2);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2011)", 3);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2011)", 4);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_FirefallFaire", "Firefall Faire")} (2011)", 5);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_HuntersMoon", "Hunter's Moon")} (2011)", 6);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_FoundationDay", "Foundation Day")} (2011)", 7);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2011) *", 8);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2011)", 9);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2012)", 10);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2012)", 11);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2012)", 12);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2012)", 13);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2012)", 14);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_FoundationDay", "Foundation Day")} (2012)", 15);
            DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2013)", 16);*/

            using (var progressEvent2026Tab = ImRaii.TabItem("2026###progressEvent#Tabs#2026"))
            {
                if (progressEvent2026Tab.Success)
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2026)",
                        131);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2026)",
                        132);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2026)",
                        133);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2026)",
                        134);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2026)",
                        135);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2026)", 136);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2026) *", 137);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2026)",
                        138);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2026)",
                        139);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2026) *",
                        140);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        141);
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2025)",
                        122);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2025)",
                        123);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2025)",
                        124);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2025)",
                        125);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2025)",
                        126);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2025)",
                        127);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2025)",
                        128);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2025)",
                        129);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2025)",
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2024)", 107);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2024)", 108);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2024)",
                        109);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2024) *", 110);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesAndHatchingTideDay", "Little Ladies' Day & Hatching-tide")} (2024)",
                        111);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ThePathInterfal", "The Path Infernal")} (2024)",
                        112);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2024) *", 113);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2024", 114);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2024)", 115);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        116);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2024)",
                        117);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2024)",
                        118);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2024)",
                        119);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        120);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2024)",
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2023)", 97);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2023)",
                        98);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2023)", 99);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2023)",
                        100);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2023)", 101);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2023)",
                        102);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2023)",
                        103);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2023)",
                        104);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Blunderville", "Blunderville")} **",
                        105);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2022)", 86);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2021 delayed)",
                        87);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2022)",
                        88);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2022)", 89);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2022)",
                        90);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2022)", 91);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2022", 92);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2022)",
                        93);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2022)",
                        94);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2022)",
                        95);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2021)", 77);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneAndLittleLadiesDays", "Valentione's and Little Ladies' Day")} (2021)",
                        78);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2021)",
                        79);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2021", 80);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2021)",
                        81);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2021)",
                        82);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2021) *", 83);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2021)", 84);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2020)", 66);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2020)",
                        67);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2020)", 68);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2020)",
                        69);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2020)", 70);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2020)", 71);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2020)",
                        72);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2020) *", 73);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2020)",
                        74);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2020)", 75);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2019)", 56);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2019)",
                        57);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2019)", 58);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2019)",
                        59);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ANocturneforHeroes", "A Nocturne for Heroes")} (2019) *", 60);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2019)", 61);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2019)",
                        62);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2019)",
                        63);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2019)",
                        64);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2018)", 46);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2018)",
                        47);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2018)", 48);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2018)",
                        49);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2018)", 50);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2018)",
                        51);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2018)",
                        52);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2018)",
                        53);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheHuntForRathalos", "The Hunt For Rathalos")}",
                        54);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
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
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2017)", 34);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2017)",
                        35);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2017)", 36);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2017)",
                        37);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} (2017)", 38);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2017)",
                        39);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_TheRising", "The Rising")} (2017)",
                        40);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2017) *", 41);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2017)",
                        42);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2017)", 43);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2017)", 44);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2017)",
                        45);
                }
            }

            using (var progressEvent2013141516Tab = ImRaii.TabItem("2013-14-15-16###progressEvent#Tabs#2013141516"))
            {
                if (progressEvent2013141516Tab.Success)
                {
                    int columns = chars.Count + 1;
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#2013141516#Table",
                        columns,
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
                    //ImGui.TableSetupScrollFreeze(columns, 1);//Freeze header so it shows while scrolling
                    ImGui.TableSetupScrollFreeze(1, 1);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1898)} ({Loc.Localize("ClickToDisplayRewards", "Click to display rewards")})");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 665));
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

                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2013)",
                        0);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LightningStrikes", "Lightning Strikes")} (2013)", 1);
                    //DrawAllLine(currentLocale, globalCache, chars, charactersQuests, $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2013)", 2);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2014)", 2);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BurgeoningDread", "Burgeoning Dread")} (2014)",
                        3);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2014)",
                        4);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2014)",
                        5);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2014)", 6);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2014)",
                        7);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2014)",
                        8);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ThatOldBlackMagic", "That Old Black Magic")} (2014)", 9);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheRising", "The Rising")} (2014)",
                        10);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LightningReturns", "Lightning Returns")}",
                        11);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_BreakingBrickMountains", "Breaking Brick Mountains")} (2014)",
                        12);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2014)",
                        13);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2014)",
                        14);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2015)", 15);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2015)",
                        16);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2015)", 17);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2015)",
                        18);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2015)",
                        19);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheRising", "The Rising")} (2015)",
                        20);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2015)",
                        21);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MaidensRhapsody", "The Maiden's Rhapsody")} (2015)", 22);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2015)",
                        23);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Heavensturn", "Heavensturn")} (2016)", 24);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_ValentioneDay", "Valentione's Day")} (2016)",
                        25);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_LittleLadiesDay", "Little Ladies' Day")} (2016)", 26);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_HatchingTide", "Hatching-tide")} (2016)",
                        27);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheMakeItRainCampaign", "The Make It Rain Campaign")} 2016", 28);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_YoKai", "Yo-kai Watch: Gather One, Gather All!")} (2016) *", 29);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_MoonfireFaire", "Moonfire Faire")} (2016)",
                        30);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_TheRising", "The Rising")} (2016)",
                        31);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_AllSaintsWake", "All Saints' Wake")} (2016)",
                        32);
                    DrawAllLine(currentLocale, globalCache, chars, charactersQuests,
                        $"{Loc.Localize("Event_Starlight", "Starlight Celebration")} (2016)",
                        33);
                }
            }

            using (var blundervilleRewards =
                   ImRaii.TabItem(
                       $"{Loc.Localize("Event_Blunderville", "Blunderville")} {globalCache.AddonStorage.LoadAddonString(currentLocale, 1885)}"))
            {
                if (blundervilleRewards.Success)
                {
                    DrawBlundervilleRewards(currentLocale, globalCache, chars);
                }
            }

            string mogEventName = currentLocale switch
            {
                ClientLanguage.German => "Mog Mog-Kollektion",
                ClientLanguage.English => "Moogle Treasure Trove",
                ClientLanguage.French => "Collection Mog Mog",
                ClientLanguage.Japanese => "モグモグ★コレクション",
                _ => "Moogle Treasure Trove"
            };
            using var moogleRewards = ImRaii.TabItem($"{mogEventName}");
            if (moogleRewards.Success)
            {
                MoogleEvent.DrawRewards(currentLocale, globalCache, chars);
            }
        }

        private static void DrawAllLine(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, List<List<bool>> charactersQuests, string name,
            int eventIndex)
        {
            //Plugin.Log.Debug($"DrawAllLine: {chars.Count}, name: {name}, msqIndex: {msqIndex}");
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(name);
            if (eventIndex is 140)
            {
                DrawMultiTabRewardsModal(currentLocale, globalCache, chars, eventIndex);
            }
            if (eventIndex is not 140 && (GetEventCurrencyFromEventId(eventIndex) != 0 || eventIndex is 29 or 41 or 113 or 137))
            {
                DrawRewardsModal(currentLocale, globalCache, chars, eventIndex);
            }
            foreach ((List<bool> cq, int index) charactersQuest in charactersQuests.Select((cq, index) => (cq, index)))
            {
                ImGui.TableNextColumn();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(charactersQuest.cq[eventIndex] ? FontAwesomeIcon.Check.ToIconString() : "");
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

        private static void DrawBlundervilleRewards(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars)
        {
            using var tabBar = ImRaii.TabBar("###CharactersProgress#All#Event#Blunderville#TabBar");
            if (!tabBar.Success) return;
            using (var collectableTab =
               ImRaii.TabItem(
                   $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1456)}###CharactersProgress#All#Event#Blunderville#Collectable"))
            {
                if (collectableTab.Success)
                {
                    int columns = chars.Count + 2;
                    using (var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#Blunderville#Table#Collectable#Table",
                    columns,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
                    {
                        if (charactersEventTable)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#Collectable#Table#Name",
                                ImGuiTableColumnFlags.WidthFixed, 270);
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#Collectable#Table#Currency",
                                ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
                            foreach (Character c in chars)
                            {
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#Collectable#Table#{c.CharacterId}",
                                    ImGuiTableColumnFlags.WidthFixed, 25);
                            }

                            //ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
                            ImGui.TableSetupScrollFreeze(1, 1);

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                            ImGui.TableSetColumnIndex(1);
                            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                                (uint)Currencies.MGF);
                            if (itm == null) return;
                            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                            }

                            int neededMGF = 2340;
                            Dictionary<ulong, int> charactersTotalNeededMGF = [];
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

                                charactersTotalNeededMGF[currChar.CharacterId] = neededMGF;
                            }

                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 276, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 330, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 499, 350, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 500, 350, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 657, 220, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 41377, 200, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 41378, 200, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 41379, 200, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, Currencies.MGF, neededMGF, charactersTotalNeededMGF);
                        }
                    }
                }
            }
            using (var gearsTab =
            ImRaii.TabItem(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 852)}###CharactersProgress#All#Event#Blunderville#Gears"))
            {
                if (gearsTab.Success)
                {
                    int columns = chars.Count + 2;
                    using (var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#Blunderville#Table#Gears#Table",
                    columns,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
                    {
                        if (charactersEventTable)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#Gears#Table#Name",
                                ImGuiTableColumnFlags.WidthFixed, 270);
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#Gears#Table#Currency",
                                ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
                            foreach (Character c in chars)
                            {
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#Gears#Table#{c.CharacterId}",
                                    ImGuiTableColumnFlags.WidthFixed, 25);
                            }

                            //ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
                            ImGui.TableSetupScrollFreeze(1, 1);

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                            ImGui.TableSetColumnIndex(1);
                            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                                (uint)Currencies.MGF);
                            if (itm == null) return;
                            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                            }

                            int neededMGF = 2460;
                            Dictionary<ulong, int> charactersTotalNeededMGF = [];
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

                                charactersTotalNeededMGF[currChar.CharacterId] = neededMGF;
                            }
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41560, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41561, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41562, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41563, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41564, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41796, 410, charactersTotalNeededMGF);
                            Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, Currencies.MGF, neededMGF, charactersTotalNeededMGF);
                        }
                    }
                }
            }
            using var nonCollectableTab =
            ImRaii.TabItem(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 6315)}###CharactersProgress#All#Event#Blunderville#NonCollectable");
            if (nonCollectableTab.Success)
            {
                int columns = chars.Count + 2;
                using var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#Blunderville#Table#NonCollectable#Table",
                    columns,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                if (charactersEventTable)
                {
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#NonCollectable#Table#Name",
                        ImGuiTableColumnFlags.WidthFixed, 270);
                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#NonCollectable#Table#Currency",
                        ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#Event#Blunderville#NonCollectable#Table#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 25);
                    }

                    //ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
                    ImGui.TableSetupScrollFreeze(1, 1);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                    ImGui.TableSetColumnIndex(1);
                    Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                        (uint)Currencies.MGF);
                    if (itm == null) return;
                    Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                    }

                    int neededMGF = 1100;
                    Dictionary<ulong, int> charactersTotalNeededMGF = [];
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

                        charactersTotalNeededMGF[currChar.CharacterId] = neededMGF;
                    }
                    Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41437, 220, charactersTotalNeededMGF);
                    Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41438, 220, charactersTotalNeededMGF);
                    Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41439, 220, charactersTotalNeededMGF);
                    Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41440, 220, charactersTotalNeededMGF);
                    Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 41441, 220, charactersTotalNeededMGF);
                    Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, Currencies.MGF, neededMGF, charactersTotalNeededMGF);
                }
            }
        }

        private static void DrawEventReward(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int msqIndex)
         {
            switch (msqIndex)

            {
                case 0: /*All Saints' Wake (2013)*/
                    {
                        break;
                    }
                case 1:/*Lightning Strikes (2013)*/
                    {
                        break;
                    }
                /*case 2: //Starlight Celebration (2013)
                {
                    break;
                }*/
                case 2:/*Heavensturn (2014)*/
                    {
                        break;
                    }
                case 3:/*Burgeoning Dread (2014)*/
                    {
                        break;
                    }
                case 4:/*Breaking Brick Mountains (2014)*/
                    {
                        break;
                    }
                case 5:/*Valentione's Day (2014)*/
                    {
                        break;
                    }
                case 6:/*Little Ladies' Day (2014)*/
                    {
                        break;
                    }
                case 7:/*Hatching-tide (2014)*/
                    {
                        break;
                    }
                case 8:/*Moonfire Faire (2014)*/
                    {
                        break;
                    }
                case 9:/*That Old Black Magic (2014)*/
                    {
                        break;
                    }
                case 10:/*The Rising (2014)*/
                    {
                        break;
                    }
                case 12:/*Lightning Returns";11;       "Breaking Brick Mountains (2014)*/
                    {
                        break;
                    }
                case 13:/*All Saints' Wake (2014)*/
                    {
                        break;
                    }
                case 14:/*Starlight Celebration (2014)*/
                    {
                        break;
                    }
                case 15:/*Heavensturn (2015)*/
                    {
                        break;
                    }
                case 16:/*Valentione's Day (2015)*/
                    {
                        break;
                    }
                case 17:/*Little Ladies' Day (2015)*/
                    {
                        break;
                    }
                case 18:/*Hatching-tide (2015)*/
                    {
                        break;
                    }
                case 19:/*Moonfire Faire (2015)*/
                    {
                        break;
                    }
                case 20:/*The Rising (2015)*/
                    {
                        break;
                    }
                case 21:/*All Saints' Wake (2015)*/
                    {
                        break;
                    }
                case 22:/*The Maiden's Rhapsody (2015)*/
                    {
                        break;
                    }
                case 23:/*Starlight Celebration (2015)*/
                    {
                        break;
                    }
                case 24:/*Heavensturn (2016)*/
                    {
                        break;
                    }
                case 25:/*Valentione's Day (2016)*/
                    {
                        break;
                    }
                case 26:/*Little Ladies' Day (2016)*/
                    {
                        break;
                    }
                case 27:/*Hatching-tide (2016)*/
                    {
                        break;
                    }
                case 28:/*The Make It Rain Campaign" 2016*/
                    {
                        break;
                    }
                case 29:/*Yo-kai Watch: Gather One;Gather All! (2016) **/
                    {
                        break;
                    }
                case 30:/*Moonfire Faire (2016)*/
                    {
                        break;
                    }
                case 31:/*The Rising (2016)*/
                    {
                        break;
                    }
                case 32:/*All Saints' Wake (2016)*/
                    {
                        break;
                    }
                case 33:/*Starlight Celebration (2016)*/
                    {
                        break;
                    }
                case 34:/*Heavensturn (2017)*/
                    {
                        break;
                    }
                case 35:/*Valentione's Day (2017)*/
                    {
                        break;
                    }
                case 36:/*Little Ladies' Day (2017)*/
                    {
                        break;
                    }
                case 37:/*Hatching-tide (2017)*/
                    {
                        break;
                    }
                case 38:/*The Make It Rain Campaign (2017)*/
                    {
                        break;
                    }
                case 39:/*Moonfire Faire (2017)*/
                    {
                        break;
                    }
                case 40:/*The Rising (2017)*/
                    {
                        break;
                    }
                case 41:/*Yo-kai Watch: Gather One Gather All! (2017)*/
                    {
                        break;
                    }
                case 42:/*All Saints' Wake (2017)*/
                    {
                        break;
                    }
                case 43:/*The Maiden's Rhapsody (2017)*/
                    {
                        break;
                    }
                case 44:/*Breaking Brick Mountains (2017)*/
                    {
                        break;
                    }
                case 45:/*Starlight Celebration (2017)*/
                    {
                        break;
                    }
                case 46:/*Heavensturn (2018)*/
                    {
                        break;
                    }
                case 47:/*Valentione's Day (2018)*/
                    {
                        break;
                    }
                case 48:/*Little Ladies' Day (2018)*/
                    {
                        break;
                    }
                case 49:/*Hatching-tide (2018)*/
                    {
                        break;
                    }
                case 50:/*The Make It Rain Campaign (2018)*/
                    {
                        break;
                    }
                case 51:/*Moonfire Faire (2018)*/
                    {
                        break;
                    }
                case 52:/*The Rising (2018)*/
                    {
                        break;
                    }
                case 53:/*All Saints' Wake (2018)*/
                    {
                        break;
                    }
                case 55:/*The Hunt For Rathalos";54;       "Starlight Celebration (2018)*/
                    {
                        break;
                    }
                case 56:/*Heavensturn (2019)*/
                    {
                        break;
                    }
                case 57:/*Valentione's Day (2019)*/
                    {
                        break;
                    }
                case 58:/*Little Ladies' Day (2019)*/
                    {
                        break;
                    }
                case 59:/*Hatching-tide (2019)*/
                    {
                        break;
                    }
                case 60:/*A Nocturne for Heroes (2019) **/
                    {
                        break;
                    }
                case 61:/*The Make It Rain Campaign (2019)*/
                    {
                        break;
                    }
                case 62:/*Moonfire Faire (2019)*/
                    {
                        break;
                    }
                case 63:/*The Rising (2019)*/
                    {
                        break;
                    }
                case 64:/*All Saints' Wake (2019)*/
                    {
                        break;
                    }
                case 65:/*Starlight Celebration (2019)*/
                    {
                        break;
                    }
                case 66:/*Heavensturn (2020)*/
                    {
                        break;
                    }
                case 67:/*Valentione's Day (2020)*/
                    {
                        break;
                    }
                case 68:/*Little Ladies' Day (2020)*/
                    {
                        break;
                    }
                case 69:/*Hatching-tide (2020)*/
                    {
                        break;
                    }
                case 70:/*The Maiden's Rhapsody (2020)*/
                    {
                        break;
                    }
                case 71:/*Breaking Brick Mountains (2020)*/
                    {
                        break;
                    }
                case 72:/*Moonfire Faire (2020)*/
                    {
                        break;
                    }
                case 73:/*Yo-kai Watch: Gather One;Gather All! (2020) **/
                    {
                        break;
                    }
                case 74:/*The Rising (2020)*/
                    {
                        break;
                    }
                case 75:/*The Make It Rain Campaign (2020)*/
                    {
                        break;
                    }
                case 76:/*Starlight Celebration (2020)*/
                    {
                        break;
                    }
                case 77:/*Heavensturn (2021)*/
                    {
                        break;
                    }
                case 78:/*Valentione's and Little Ladies' Day (2021)*/
                    {
                        break;
                    }
                case 79:/*Hatching-tide (2021)*/
                    {
                        break;
                    }
                case 80:/*The Make It Rain Campaign" 2021*/
                    {
                        break;
                    }
                case 81:/*Moonfire Faire (2021)*/
                    {
                        break;
                    }
                case 82:/*The Rising (2021)*/
                    {
                        break;
                    }
                case 83:/*A Nocturne for Heroes (2021) **/
                    {
                        break;
                    }
                case 84:/*Breaking Brick Mountains (2021)*/
                    {
                        break;
                    }
                case 85:/*Starlight Celebration (2021)*/
                    {
                        break;
                    }
                case 86:/*Heavensturn (2022)*/
                    {
                        break;
                    }
                case 87:/*All Saints' Wake (2021 delayed)*/
                    {
                        break;
                    }
                case 88:/*Valentione's Day (2022)*/
                    {
                        break;
                    }
                case 89:/*Little Ladies' Day (2022)*/
                    {
                        break;
                    }
                case 90:/*Hatching-tide (2022)*/
                    {
                        break;
                    }
                case 91:/*The Maiden's Rhapsody (2022)*/
                    {
                        break;
                    }
                case 92:/*The Make It Rain Campaign" 2022*/
                    {
                        break;
                    }
                case 93:/*Moonfire Faire (2022)*/
                    {
                        break;
                    }
                case 94:/*The Rising (2022)*/
                    {
                        break;
                    }
                case 95:/*All Saints' Wake (2022)*/
                    {
                        break;
                    }
                case 96:/*Starlight Celebration (2022)*/
                    {
                        break;
                    }
                case 97:/*Heavensturn (2023)*/
                    {
                        break;
                    }
                case 98:/*Valentione's Day (2023)*/
                    {
                        break;
                    }
                case 99:/*Little Ladies' Day (2023)*/
                    {
                        break;
                    }
                case 100: /*Hatching-tide (2023)*/
                    {
                        break;
                    }
                case 101: /*The Make It Rain Campaign (2023)*/
                    {
                        break;
                    }
                case 102: /*Moonfire Faire (2023)*/
                    {
                        break;
                    }
                case 103: /*The Rising (2023)*/
                    {
                        break;
                    }
                case 104: /*All Saints' Wake (2023)*/
                    {
                        break;
                    }
                case 105: /*Blunderville" ***/
                    {
                        break;
                    }
                case 106: /*Starlight Celebration (2023)*/
                    {
                        break;
                    }
                case 107: /*Heavensturn (2024)*/
                    {
                        break;
                    }
                case 108: /*The Maiden's Rhapsody (2024)*/
                    {
                        break;
                    }
                case 109: /*Valentione's Day (2024)*/
                    {
                        break;
                    }
                case 110: /*A Nocturne for Heroes (2024) **/
                    {
                        break;
                    }
                case 111: /*Little Ladies' Day & Hatching-tide (2024)*/
                    {
                        break;
                    }
                case 112: /*The Path Infernal (2024)*/
                    {
                        break;
                    }
                case 113: /*Yo-kai Watch: Gather One;Gather All! (2024) **/
                    {
                        break;
                    }
                case 114: /*The Make It Rain Campaign" 2024*/
                    {
                        break;
                    }
                case 115: /*Breaking Brick Mountains (2024)*/
                    {
                        break;
                    }
                case 116: /*Blunderville" ***/
                    {
                        break;
                    }
                case 117: /*Moonfire Faire (2024)*/
                    {
                        break;
                    }
                case 118: /*The Rising (2024)*/
                    {
                        break;
                    }
                case 119: /*All Saints' Wake (2024)*/
                    {
                        break;
                    }
                case 120: /*Blunderville" ***/
                    {
                        break;
                    }
                case 121: /*Starlight Celebration (2024)*/
                    {
                        break;
                    }
                case 122: /*Heavensturn (2025)*/
                    {
                        break;
                    }
                case 123: /*Valentione's Day (2025)*/
                    {
                        break;
                    }
                case 124: /*Little Ladies' Day (2025)*/
                    {
                        break;
                    }
                case 125: /*Hatching-tide (2025)*/
                    {
                        break;
                    }
                case 126: /*The Make It Rain Campaign (2025)*/
                    {
                        break;
                    }
                case 127: /*Moonfire Faire (2025)*/
                    {

                        break;
                    }
                case 128: /*The Rising (2025)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 546, 0);
                        break;
                    }
                case 129: /*All Saints' Wake (2025)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 396, 0);
                        break;
                    }
                case 130: /*Starlight Celebration (2025)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 298, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 299, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 300, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 301, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 302, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 303, 0);
                        break;
                    }
                case 131: /*Heavensturn (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 529, 0);

                        break;
                    }
                case 132: /*Valentione's Day (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 825, 2);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 826, 2);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 827, 2);
                        break;
                    }
                case 133: /*Little Ladies' Day (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 322, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 324, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 805, 2);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50848, 2);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50849, 2);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50850, 2);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50851, 2);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 49862, 1);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 49863, 1);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 47352, 3);
                        break;
                    }
                case 134: /*Hatching-tide (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 481, 20);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 505, 20);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 517, 20);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 529, 20);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 541, 20);
                        break;
                    }
                case 135: /*The Make It Rain Campaign (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 579, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 52, 0);
                        break;
                    }
                case 136: /*Breaking Brick Mountains (2026)*/
                    {
                        uint? fkId = globalCache.FramerKitStorage.GetFramerKitIdFromItemId(51998);
                        if (fkId is not null)
                        {
                            Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.FramerKit, fkId.Value, 1282);
                        }
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 8576, 57);
                        break;
                    }
                case 137: /*Yo-kai Watch: Gather One, Gather All (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 200, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 201, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 202, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 203, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 204, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 205, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 206, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 207, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 208, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 209, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 210, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 211, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 212, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 390, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 391, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 392, 0);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 393, 0);
                        uint? fkId = globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41797);
                        if (fkId is not null)
                        {
                            Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.FramerKit, fkId.Value, 0);
                        }

                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15208);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15221);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15209);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15210);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15211);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15212);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15213);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15214);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15215);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15216);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15217);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15218);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15219);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 15220);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30807);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30808);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30809);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30810);
                        break;
                    }
                case 138: /*Moonfire Faire (2026)*/
                    {
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 49873, 5000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 49874, 5000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 49875, 5000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50083, 2000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50084, 3000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 50085, 2000);
                        break;
                    }
                case 139: /*The Rising (2026)*/
                    {
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 51300, 3000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 51299, 2000);
                        Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 51301, 2000);
                        break;
                    }
                case 140: /*A Nocturne for Heroes (2026)*/
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 151, 200000);
                        Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 24802, 20000);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.TripleTriadCard, 252, 10000);
                        break;
                    }
                case -140:
                    {
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 299, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 300, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 301, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 302, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 303, 1);
                        Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 304, 1);
                        break;
                    }
            }
        }

        private static float GetEventRewardLongestName(ClientLanguage currentLocale, GlobalCache globalCache, int msqIndex)
        {
            return msqIndex switch
            {
                138 => ImGui.CalcTextSize(globalCache.ItemStorage.LoadItem(currentLocale, 50084)?.Name.ToString()).X + 5,
                _ => 270
            };
        }
    }
}