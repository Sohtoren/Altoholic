using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altoholic.Helpers
{
    public class MoogleEvent
    {
        public static void DrawRewards(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int currentOldMoogleReward)
        {
            Dictionary<int, string> mooglesNames = [];
            mooglesNames[0] = Loc.Localize("PreviousMoogleEvent", "Previous Events");
            string astronomyName = currentLocale switch
            {
                ClientLanguage.German => "Astronomische Abenteuer - Teil 1",
                ClientLanguage.English => "The First Hunt for Astronomy",
                ClientLanguage.French => "Astronomie Kupo - Partie 1",
                ClientLanguage.Japanese => "～天文に至る路 Part1～",
                _ => "The Hunt for Aphorism"
            };
            if (ImGui.CollapsingHeader($"2026 - {astronomyName}"))
            {
                using var tabBar = ImRaii.TabBar("###CharactersDetailsTable#ProfileTable#ProfileCol#ProfileTabBar");
                if (!tabBar.Success) return;
                using (var collectableTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1456)}###CharactersProgress#All#Event#MogRewards#Event2026_2#Collectable"))
                {
                    if (collectableTab.Success)
                    {
                        using (var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#MogRewards#Table#Event2026_2#Collectable#Table",
                        chars.Count + 2,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
                        {
                            if (charactersEventTable)
                            {
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#Collectable#Table#Name",
                                    ImGuiTableColumnFlags.WidthFixed, 270);
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#Collectable#Table#Currency",
                                    ImGuiTableColumnFlags.WidthFixed, 20);
                                foreach (Character c in chars)
                                {
                                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#Collectable#Table#{c.CharacterId}",
                                        ImGuiTableColumnFlags.WidthFixed, 20);
                                }

                                ImGui.TableSetupScrollFreeze(-1, 1); //Freeze header so it shows while scrolling
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                                ImGui.TableSetColumnIndex(1);
                                Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                                    (uint)Currencies.IRREGULAR_TOMESTONE_OF_ASTRONOMY_I);
                                if (itm == null) return;
                                Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                                if (ImGui.IsItemHovered())
                                {
                                    Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                                }

                                int neededTomestone = 288;
                                Dictionary<ulong, int> charactersTotalNeededTomestone = [];
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

                                    charactersTotalNeededTomestone[currChar.CharacterId] = neededTomestone;
                                }

                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 219, 50, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 418, 50, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 182, 30, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 318, 30, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 31, 30, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 185, 30, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 441, 15, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 185, 10, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 187, 10, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 85, 7, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 16703, 7, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 359, 7, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 401, 7, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 78, 5, charactersTotalNeededTomestone);
                                Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, neededTomestone, charactersTotalNeededTomestone);
                            }
                        }
                    }
                }
                using (var gearsTab =
                ImRaii.TabItem(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 852)}###CharactersProgress#All#Event#MogRewards#Event2026_2#Gears"))
                {
                    if (gearsTab.Success)
                    {
                        using (var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#Event#MogRewards#Table#Event2026_2#Gears#Table",
                        chars.Count + 2,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
                        {
                            if (charactersEventTable)
                            {
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#Gears#Table#Name",
                                    ImGuiTableColumnFlags.WidthFixed, 270);
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#Gears#Table#Currency",
                                    ImGuiTableColumnFlags.WidthFixed, 20);
                                foreach (Character c in chars)
                                {
                                    ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#Gears#Table#{c.CharacterId}",
                                        ImGuiTableColumnFlags.WidthFixed, 20);
                                }

                                ImGui.TableSetupScrollFreeze(-1, 1); //Freeze header so it shows while scrolling
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                                ImGui.TableSetColumnIndex(1);
                                Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                                    (uint)Currencies.IRREGULAR_TOMESTONE_OF_ASTRONOMY_I);
                                if (itm == null) return;
                                Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                                if (ImGui.IsItemHovered())
                                {
                                    Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                                }

                                int neededTomestone = 215;
                                Dictionary<ulong, int> charactersTotalNeededTomestone = [];
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

                                    charactersTotalNeededTomestone[currChar.CharacterId] = neededTomestone;
                                }
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 24615, 50);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 32794, 30);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30052, 30);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 27936, 30);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 27937, 30);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 13162, 15);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 13174, 15);
                                Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 13186, 15);
                                Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, neededTomestone, charactersTotalNeededTomestone);
                            }
                        }
                    }
                }
                using var nonCollectableTab =
                  ImRaii.TabItem(
                      $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 832)}###CharactersProgress#All#Event#MogRewards#Event2026_2#NonCollectable");
                if (nonCollectableTab.Success)
                {
                    using (var charactersEventTable = ImRaii.Table(
                    $"###CharactersProgress#All#Event#MogRewards#Table#Event2026_2#NonCollectable#Table",
                    chars.Count + 2,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                    ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
                    {
                        if (charactersEventTable)
                        {
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#NonCollectable#Table#Name",
                                ImGuiTableColumnFlags.WidthFixed, 270);
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#NonCollectable#Table#Currency",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                            foreach (Character c in chars)
                            {
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_2#NonCollectable#Table#{c.CharacterId}",
                                    ImGuiTableColumnFlags.WidthFixed, 20);
                            }

                            ImGui.TableSetupScrollFreeze(-1, 1); //Freeze header so it shows while scrolling
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                            ImGui.TableSetColumnIndex(1);
                            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                                (uint)Currencies.IRREGULAR_TOMESTONE_OF_ASTRONOMY_I);
                            if (itm == null) return;
                            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                            }

                            int neededTomestone = 121;
                            Dictionary<ulong, int> charactersTotalNeededTomestone = [];
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

                                charactersTotalNeededTomestone[currChar.CharacterId] = neededTomestone;
                            }
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 16784, 30);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 38585, 20);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 38605, 20);
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 38622, 20);                          
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 39918, 15);
                            //Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, , 15);//Presumably Special Timeworn Map
                            Helpers.Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 25005, 1);
                            Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, neededTomestone, charactersTotalNeededTomestone);
                        }
                    }
                }
            }

            string aphorismName = currentLocale switch
            {
                ClientLanguage.German => "Allgegenwärtige Aphorismen",
                ClientLanguage.English => "The Hunt for Aphorism",
                ClientLanguage.French => "Aphorisme Kupo",
                ClientLanguage.Japanese => "～霧の中の経典～",
                _ => "The Hunt for Aphorism"
            };
            mooglesNames[2026_1] = $"2026 {aphorismName}";
            string revelationName = currentLocale switch
            {
                ClientLanguage.German => "Aufgezeigte Offenbarungen",
                ClientLanguage.English => "The Hunt for Revelation",
                ClientLanguage.French => "Révélation Kupo",
                ClientLanguage.Japanese => "\uff5e黙示への道標\uff5e",
                _ => "The Hunt for Revelation"
            };
            mooglesNames[2025_3] = $"2025 {revelationName}";
            string allegoryName = currentLocale switch
            {
                ClientLanguage.German => "Fantastische Fachsimpeleien",
                ClientLanguage.English => "The Hunt for Allegory",
                ClientLanguage.French => "L'allégorie perdue",
                ClientLanguage.Japanese => "\uff5e奇譚の探求者\uff5e",
                _ => "The Hunt for Allegory"
            };
            mooglesNames[2025_2] = $"2025 {allegoryName}";
            string phantasmagoriaName = currentLocale switch
            {
                ClientLanguage.German => "Phantasmagorische Freundschaften",
                ClientLanguage.English => "The Hunt for Phantasmagoria",
                ClientLanguage.French => "Fantasmagorie Kupo",
                ClientLanguage.Japanese => "\uff5e幻想との邂逅\uff5e",
                _ => "The Hunt for Phantasmagoria"
            };
            mooglesNames[2025_1] = $"2025 {phantasmagoriaName}";
            string goetiaName = currentLocale switch
            {
                ClientLanguage.German => "Goëtische Goldschätze",
                ClientLanguage.English => "The Hunt for Goetia",
                ClientLanguage.French => "L'aube de la goétie",
                ClientLanguage.Japanese => "～黄金の魔典～",
                _ => "The Hunt for Goetia"
            };
            mooglesNames[2024_3] = $"2024 - {goetiaName}";
            string genesis2Name = currentLocale switch
            {
                ClientLanguage.German => "Dimensionale Ursprünge - Teil 2",
                ClientLanguage.English => "The Second Hunt for Genesis",
                ClientLanguage.French => "Genèse d'une nouvelle dimension - Partie 2",
                ClientLanguage.Japanese => "～新次元の創世 Part2～",
                _ => "The Second Hunt for Genesis"
            };
            mooglesNames[2024_2] = $"2024 - {genesis2Name}";
            string genesis1Name = currentLocale switch
            {
                ClientLanguage.German => "Dimensionale Ursprünge - Teil 1",
                ClientLanguage.English => "The First Hunt for Genesis",
                ClientLanguage.French => "Genèse d'une nouvelle dimension - Partie 1",
                ClientLanguage.Japanese => "～新次元の創世 Part1～",
                _ => "The First Hunt for Genesis"
            };
            mooglesNames[2024_1] = $"2024 - {genesis1Name}";
            string tenthAnniversaryName = currentLocale switch
            {
                ClientLanguage.German => "10. Jubiläum",
                ClientLanguage.English => "The 10th Anniversary Hunt",
                ClientLanguage.French => "Spéciale 10e anniversaire",
                ClientLanguage.Japanese => "\uff5e新生10周年スペシャル\uff5e",
                _ => "The 10th Anniversary Hunt"
            };
            mooglesNames[2023_2] = $"2023 - {tenthAnniversaryName}";
            string mendacityName = currentLocale switch
            {
                ClientLanguage.German => "Tratsch aus Quatsch",
                ClientLanguage.English => "The Hunt for Mendacity",
                ClientLanguage.French => "Duplicité Kupo",
                ClientLanguage.Japanese => "\uff5e虚構の刻\uff5e",
                _ => "The Hunt for Mendacity"
            };
            mooglesNames[2023_1] = $"2023 - {mendacityName}";
            string theHuntForCreationName = currentLocale switch
            {
                ClientLanguage.German => "Allerlei Erinnerungen",
                ClientLanguage.English => "The Hunt for Creation",
                ClientLanguage.French => "Cosmogonie Kupo",
                ClientLanguage.Japanese => "\uff5e万物の記憶\uff5e",
                _ => "The Hunt for Creation"
            };
            mooglesNames[2022_3] = $"2022 - {theHuntForCreationName}";

            string theHuntForVerityName = currentLocale switch
            {
                ClientLanguage.German => "Die Stunde der Wahrheit",
                ClientLanguage.English => "The Hunt for Verity",
                ClientLanguage.French => "Véridicité Kupo",
                ClientLanguage.Japanese => "\uff5e帰ってきた真理\uff5e",
                _ => "The Hunt for Verity"
            };
            mooglesNames[2022_2] = $"2022 - {theHuntForVerityName}";

            string theHuntForScriptureName = currentLocale switch
            {
                ClientLanguage.German => "Theologisches Vermächtnis",
                ClientLanguage.English => "The Hunt for Scripture",
                ClientLanguage.French => "Théologie Kupo",
                ClientLanguage.Japanese => "～聖典を継ぐ者～",
                _ => "The Hunt for Scripture"
            };
            mooglesNames[2022_1] = $"2022 - {theHuntForScriptureName}";

            string theHuntForLoreName = currentLocale switch
            {
                ClientLanguage.German => "Sagenhafte Schätze",
                ClientLanguage.English => "The Hunt for Lore",
                ClientLanguage.French => "Tradition Kupo",
                ClientLanguage.Japanese => "\uff5e炎獄の伝承\uff5e",
                _ => "The Hunt for Lore"
            };
            mooglesNames[2021_3] = $"2021 - {theHuntForLoreName}";

            string theHuntForPageantryName = currentLocale switch
            {
                ClientLanguage.German => "Fan Festival",
                ClientLanguage.English => "The Hunt for Pageantry",
                ClientLanguage.French => "Festivités Kupo",
                ClientLanguage.Japanese => "\uff5eファンフェススペシャル2021\uff5e",
                _ => "The Hunt for Pageantry"
            };
            mooglesNames[2021_2] = $"2021 - {theHuntForPageantryName}";

            string theHuntForEsotericsName = currentLocale switch
            {
                ClientLanguage.German => "Esoterische Momente",
                ClientLanguage.English => "The Hunt for Esoterics",
                ClientLanguage.French => "Ésotérisme Kupo",
                ClientLanguage.Japanese => "\uff5eもうひとつの禁書\uff5e",
                _ => "The Hunt for Esoterics"
            };
            mooglesNames[2021_1] = $"2021 - {theHuntForEsotericsName}";

            string theHuntForLawName = currentLocale switch
            {
                ClientLanguage.German => "Emotionale Erinnerungen",
                ClientLanguage.English => "The Hunt for Law",
                ClientLanguage.French => "Réminiscence Kupo",
                ClientLanguage.Japanese => "\uff5e追憶の法典\uff5e",
                _ => "The Hunt for Law"
            };
            mooglesNames[2020_2] = $"2020 - {theHuntForLawName}";

            string theHuntForSoldieryName = currentLocale switch
            {
                ClientLanguage.German => "Strategische Schnitzeljag",
                ClientLanguage.English => "The Hunt for Soldiery",
                ClientLanguage.French => "Martialité Kupo",
                ClientLanguage.Japanese => "\uff5e戦記ューフォーエバー\uff5e",
                _ => "The Hunt for Soldiery"
            };
            mooglesNames[2020_1] = $"2020 - {theHuntForSoldieryName}";

            string theHuntForMythologyName = currentLocale switch
            {
                ClientLanguage.German => "Mythologische Mär",
                ClientLanguage.English => "The Hunt for Mythology",
                ClientLanguage.French => "Mythologie Kupo",
                ClientLanguage.Japanese => "\uff5eそして神話へ…\uff5e",
                _ => "The Hunt for Mythology"
            };
            mooglesNames[2019_2] = $"2019 - {theHuntForMythologyName}";

            string theHuntForPhilosophyName = currentLocale switch
            {
                ClientLanguage.German => "Philosophische Momente",
                ClientLanguage.English => "The Hunt for Philosophy",
                ClientLanguage.French => "Philosophie Kupo",
                ClientLanguage.Japanese => "\uff5e哲学ふたたび\uff5e",
                _ => "The Hunt for Philosophy"
            };
            mooglesNames[2019_1] = $"2019 - {theHuntForPhilosophyName}";

            string n = (currentOldMoogleReward == 0) ? mooglesNames[0] : mooglesNames[currentOldMoogleReward];
            using (var combo = ImRaii.Combo("###CharactersProgress#Reputations#Combo", n))
            {
                if (combo)
                {
                    foreach (KeyValuePair<int, string> name in mooglesNames.Where(name =>
                                 ImGui.Selectable(name.Value, name.Value == n)))
                    {
                        currentOldMoogleReward = name.Key;
                    }
                }
            }

            switch (currentOldMoogleReward)
            {
                case 2026_1:
                    {
                        if (ImGui.CollapsingHeader($"2026 - {aphorismName}"))
                        {
                            using var charactersEventTable = ImRaii.Table(
                                $"###CharactersProgress#All#Event#MogRewards#Table#Event2026_1",
                                chars.Count + 2,
                                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                            if (!charactersEventTable) return;
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_1#Name",
                                ImGuiTableColumnFlags.WidthFixed, 270);
                            ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_1#Currency",
                                ImGuiTableColumnFlags.WidthFixed, 20);
                            foreach (Character c in chars)
                            {
                                ImGui.TableSetupColumn($"###CharactersProgress#All#Event#MogRewards#Event2026_1#{c.CharacterId}",
                                    ImGuiTableColumnFlags.WidthFixed, 20);
                            }

                            ImGui.TableSetupScrollFreeze(-1, 1); //Freeze header so it shows while scrolling
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                            ImGui.TableSetColumnIndex(1);
                            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                                (uint)Currencies.IRREGULAR_TOMESTONE_OF_APHORISM);
                            if (itm == null) return;
                            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
                            }

                            int neededTomestone = 540;
                            Dictionary<ulong, int> charactersTotalNeededTomestone = [];
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

                                charactersTotalNeededTomestone[currChar.CharacterId] = neededTomestone;
                            }

                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 14, 50, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 238, 50, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 130, 50, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 24234, 50, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 169, 50, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 388, 50, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 351, 30, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 158, 30, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 172, 30, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 59, 15, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 188, 15, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 183, 10, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 184, 10, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 234, 7, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 87, 7, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 324, 7, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 82, 7, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 371, 7, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 48, 5, charactersTotalNeededTomestone);
                            Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, neededTomestone, charactersTotalNeededTomestone);
                        }
                        break;
                    }
                case 2025_3:
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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_REVELATION);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 226, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 180, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 23370, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 417, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 346, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 61, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 1, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 133, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 144, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 61, 15);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 158, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 160, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 244, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 306, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 291, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 144, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 362, 5);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 289, 3);
                        break;
                    }
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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_ALLEGORY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 365, 100);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 217, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 191, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 23369, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 385, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 76, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 195, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 13, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 116, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 115, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 60, 15);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 141, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 142, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 290, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 293, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 326, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 243, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 15, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 199, 1);
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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_PHANTASMAGORIA);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 205, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 112, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 193, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 79, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 374, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 364, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 24233, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 11, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 203, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 133, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 182, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 50, 15);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 107, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 121, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 279, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 304, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 315, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 340, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 353, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 345, 7);
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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_GOETIA);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 192, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 126, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 73, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 32835, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 363, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 215, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 189, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 158, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 172, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 58, 15);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 105, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 106, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 228, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 271, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 303, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 326, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 336, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 231, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_II);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 211, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 224, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 359, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 333, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 54, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 76, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 7, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 30, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 77, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 144, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 103, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 104, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 267, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 282, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 388, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 148, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 207, 7);


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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_I);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 242, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 69, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 28615, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 319, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 208, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 254, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 78, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 43, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 76, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 133, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 111, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 102, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 249, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 261, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 361, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 144, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 179, 7);


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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_TENFOLD_PAGEANTRY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 40501, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 14, 100);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 68, 100);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 33706, 100);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 66, 100);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 181, 80);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 180, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 213, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 223, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 214, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 189, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 236, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 150, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 45, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 386, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 507, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 24234, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 349, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 352, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 116, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 115, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 133, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 144, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 158, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 172, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 182, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 257, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 258, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 259, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 2, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 83, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 84, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 235, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 250, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 372, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 373, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 93, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 110, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 345, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 199, 1);



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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_MENDACITY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 121, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 130, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 225, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 324, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 23369, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 208, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 28, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 22, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 75, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 104, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 116, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 115, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 81, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 82, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 216, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 244, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 82, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 333, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 117, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 118, 7);


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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_CREATION);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 182, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 209, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 112, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 195, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 227, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 264, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 207, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 31, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 29, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 31, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 90, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 98, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 36, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 37, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 234, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 250, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 256, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 85, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 86, 7);


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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_VERITY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 158, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 211, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 314, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 31406, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 263, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 203, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 30, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 40, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 77, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 78, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 33, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 35, 10);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 224, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 229, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 243, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 330, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 231, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_SCRIPTURE);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 172, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 30113, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 262, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 28, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 43, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 75, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 76, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 148, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 254, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 213, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 215, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 209, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 179, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 207, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_LORE);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 144, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 90, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 24234, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 261, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 54, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 29, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 31, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 144, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 283, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 206, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 179, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 197, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 117, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 118, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_PAGEANTRY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 133, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 115, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 116, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 180, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 181, 80);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 23369, 50);
                        Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 24233, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 257, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 258, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 259, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 260, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 199, 1);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 270, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 305, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 197, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 201, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 190, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 191, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_ESOTERICS);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 115, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 150, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 112, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 121, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 580, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 30, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 40, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 22, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 143, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 272, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 85, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 177, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 182, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 85, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 86, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_LAW);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 130, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 104, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 116, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 43, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 92, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 30, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 28, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 43, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 178, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 179, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 116, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 137, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 64, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 231, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 30, 7);

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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_SOLDIERY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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


                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 121, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 78, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 45, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 78, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 76, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 77, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 29, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 31, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 281, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 141, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 43, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 55, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 98, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 168, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 179, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 207, 7);
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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_MYTHOLOGY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 112, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 77, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 98, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 45, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 78, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 76, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 77, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 30, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 40, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 232, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 259, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 52, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 219, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 110, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 99, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 117, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 118, 7);
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
                        ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

                        ImGui.TableSetColumnIndex(1);
                        Item? itm = globalCache.ItemStorage.LoadItem(currentLocale,
                            (uint)Currencies.IRREGULAR_TOMESTONE_OF_PHILOSOPHY);
                        if (itm == null) return;
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 67, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 75, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 76, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 45, 50);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 19, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 20, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 26, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 27, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 28, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 35, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 43, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 16, 30);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 188, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 194, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 215, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 299, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 136, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 152, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 163, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 229, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 85, 7);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 86, 7);
                        break;
                    }
            }
        }
    }
}
