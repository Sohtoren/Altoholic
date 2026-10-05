using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Altoholic.Helpers
{
    public class FieldOperations
    {
        public static void Draw(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#FieldOperations");
            if (!tab) return;
            using (var bozjaTab =
                   ImRaii.TabItem(
                       $"Bozja###CharactersProgressTable#All#FieldOperations#Bozja"))
            {
                if (bozjaTab)
                {
                    DrawBozja(globalCache, currentLocale, chars);
                }
            }
            using (var cosmicExplorationTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3849)}###CharactersProgressTable#All#FieldOperations#CosmicExploration"))
            {
                if (cosmicExplorationTab)
                {
                    DrawCosmicExploration(globalCache, currentLocale, chars);
                }
            }
            using var eurekaTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 2305)}###CharactersProgressTable#All#FieldOperations#Eureka");
            if (eurekaTab)
            {
                DrawEureka(globalCache, currentLocale, chars);
            }
        }

        private static void DrawEureka(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using (var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#FieldOperations#Eureka#Table",
                chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY, new Vector2(-1, 40)))
            {
                if (!charactersEventTable) return;
                ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Eureka#Name",
                    ImGuiTableColumnFlags.WidthFixed, 260);
                foreach (Character c in chars)
                {
                    ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Eureka#{c.CharacterId}",
                        ImGuiTableColumnFlags.WidthFixed, 20);
                }

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("");

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
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 11602)} {(char)SeIconChar.EurekaLevel}");
                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    if (currChar.Eureka != null)
                    {
                        int currentLevel = currChar.Eureka.GetCurrentLevel() + 1;
                        ImGui.TextUnformatted($"{currentLevel}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted($"{Loc.Localize("CurrentLevel", "Current level")}");
                            ImGui.EndTooltip();
                        }
                        if (currentLevel < currChar.Eureka.MaxLevel)
                        {
                            ImGui.SameLine();
                            ImGui.PushStyleColor(ImGuiCol.Text, KnownColor.Gray.Vector());
                            ImGui.TextUnformatted($"({currChar.Eureka.MaxLevel})");
                            ImGui.PopStyleColor();
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted($"{Loc.Localize("MaxLevelAttained", "Max level attained")}");
                                ImGui.EndTooltip();
                            }
                        }
                    }
                }
            }
            DrawEurekaRewards(globalCache, currentLocale, chars);
        }
        private static void DrawEurekaRewards(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#FieldOperations#Eureka#Rewards#Table",
                chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Eureka#Rewards#Name",
                ImGuiTableColumnFlags.WidthFixed, 260);

            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Eureka#Rewards#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 61, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 66, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 181, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 189, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 195, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 132, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 285, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 286, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 287, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 315, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 319, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 150, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 208, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 209, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 289, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 288, 0);
        }

        private static void DrawBozja(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#FieldOperations#Bozja#TabBar");
            if (!tab) return;
            using (var bozjaMainTab =
                   ImRaii.TabItem(
                       $"Progression###CharactersProgressTable#All#FieldOperations#Bozja#Progression"))
            {
                if (bozjaMainTab)
                {
                    DrawBozjaMain(globalCache, currentLocale, chars);
                }
            }

            using (var bozjaDropsTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1918)}###CharactersProgressTable#All#FieldOperations#Bozja#Rewards"))
            {
                if (bozjaDropsTab)
                {
                    DrawBozjaDrops(globalCache, currentLocale, chars);
                }
            }

            using (var bozjaRewardsTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 8951)}###CharactersProgressTable#All#FieldOperations#Bozja#Exchange"))
            {
                if (bozjaRewardsTab)
                {
                    DrawBozjaExchange(globalCache, currentLocale, chars);
                }
            }
        }
        private static void DrawBozjaMain(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#FieldOperations#Bozja#Rewards#Table",
                chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Bozja#Rewards#Name",
                ImGuiTableColumnFlags.WidthFixed, 160);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Bozja#Rewards#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("10,000,000").X + 5);
            }
            ImGui.TableSetupScrollFreeze(1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("");

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
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13817)}");
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                if (currChar.Bozja != null)
                {
                    int currentRank = currChar.Bozja.GetCurrentRank();
                    ImGui.TextUnformatted($"{currentRank}");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 2792)}");
                        ImGui.EndTooltip();
                    }
                    if (currentRank < currChar.Bozja.MaxLevel)
                    {
                        ImGui.SameLine();
                        ImGui.PushStyleColor(ImGuiCol.Text, KnownColor.Gray.Vector());
                        ImGui.TextUnformatted($"({currChar.Bozja.MaxLevel})");
                        ImGui.PopStyleColor();
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted($"{Loc.Localize("MaxRankAttained", "Max rank attained")}");
                            ImGui.EndTooltip();
                        }
                    }
                }
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13818)}");
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                if (currChar.Bozja != null)
                {
                    ImGui.TextUnformatted($"{currChar.Bozja.CurrentExperience:N0}");
                }
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13819)}");
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                if (currChar.Bozja != null)
                {
                    ImGui.TextUnformatted($"{currChar.Bozja.NeededExperience:N0}");
                }
            }
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13822)}");
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                if (currChar.Bozja != null)
                {
                    ImGui.TextUnformatted($"{currChar.Currencies.GetValueOrDefault((uint)Currencies.BOZJAN_CLUSTER)}/200");
                }
            }

        }
        private static void DrawBozjaDrops(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#FieldOperations#Bozja#Rewards#Table",
                chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Bozja#Rewards#Name",
                ImGuiTableColumnFlags.WidthFixed, 260);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#Bozja#Rewards#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            ImGui.TableSetupScrollFreeze(1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1918));

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
            
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 34, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 35, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 41, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 45, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 47, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 52, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 53, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 55, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 59, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 62, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 64, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 65, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 67, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 80, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 222, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 8, 0);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 33706, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 265, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 267, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 268, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 271, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 272, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 273, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 275, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 278, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 279, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 283, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 284, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 290, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 303, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 312, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 321, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 327, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 329, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 334, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 348, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 389, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 224, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 387, 0);
        }

        private static void DrawBozjaExchange(GlobalCache globalCache, ClientLanguage currentLocale,
            List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#FieldOperations#Bozja#Exchange#TabBar");
            if (!tab) return;

            for (int i = 1; i <= 8; i++)
            {
                string name = i switch
                {
                    1 => currentLocale switch
                    {
                        ClientLanguage.German => "Verteidigung",
                        ClientLanguage.English => "Fending",
                        ClientLanguage.French => "Protecteur",
                        ClientLanguage.Japanese => "ディフェンダー",
                        _ => "Fending",
                    },
                    2 => currentLocale switch
                    {
                        ClientLanguage.German => "Verstümmelns",
                        ClientLanguage.English => "Maiming",
                        ClientLanguage.French => "Abatteur",
                        ClientLanguage.Japanese => "スレイヤー",
                        _ => "Maiming",
                    },
                    3 => currentLocale switch
                    {
                        ClientLanguage.German => "Schlagens",
                        ClientLanguage.English => "Striking",
                        ClientLanguage.French => "Agresseur",
                        ClientLanguage.Japanese => "ストライカー",
                        _ => "Striking",
                    },
                    4 => currentLocale switch
                    {
                        ClientLanguage.German => "Zielens",
                        ClientLanguage.English => "Aiming",
                        ClientLanguage.French => "Pisteur",
                        ClientLanguage.Japanese => "レンジャー",
                        _ => "Aiming",
                    },
                    5 => currentLocale switch
                    {
                        ClientLanguage.German => "Spähens",
                        ClientLanguage.English => "Scouting",
                        ClientLanguage.French => "Éclaireur",
                        ClientLanguage.Japanese => "スカウトゴー",
                        _ => "Scouting",
                    },
                    6 => currentLocale switch
                    {
                        ClientLanguage.German => "Heilung",
                        ClientLanguage.English => "Healing",
                        ClientLanguage.French => "Soigneur",
                        ClientLanguage.Japanese => "ヒーラー",
                        _ => "Healing",
                    },
                    7 => currentLocale switch
                    {
                        ClientLanguage.German => "Magie",
                        ClientLanguage.English => "Casting",
                        ClientLanguage.French => "Incantateur",
                        ClientLanguage.Japanese => "キャスターリング",
                        _ => "Casting",
                    },
                    8 => currentLocale switch
                    {
                        ClientLanguage.German => "Bozja-Leichtmetall-Kit",
                        ClientLanguage.English => "Bozjan runner's secrets",
                        ClientLanguage.French => "Matériaux ultralégers pour équipement bozjien",
                        ClientLanguage.Japanese => "ボズヤ軽量補材",
                        _ => "Bozjan runner's secrets",
                    }
                };

                using var bozjaRewardsTab =
                    ImRaii.TabItem(
                        $"{name}###CharactersProgressTable#All#FieldOperations#Bozja#Exchange#{i}");
                if (bozjaRewardsTab)
                {
                    int columns = chars.Count + 2;
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#FieldOperations#Bozja#Exchange#{i}#Table",
                        columns,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    int mainWidth = (i == 8) ? 300 : 260;
                    ImGui.TableSetupColumn(
                        $"###CharactersProgress#All#FieldOperations#Bozja#Exchange#{i}#Name",
                        ImGuiTableColumnFlags.WidthFixed, mainWidth);
                    ImGui.TableSetupColumn(
                        $"###CharactersProgress#All#FieldOperations#Bozja#Exchange#{i}#Currency",
                        ImGuiTableColumnFlags.WidthFixed, 25);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn(
                            $"###CharactersProgress#All#FieldOperations#Bozja#Exchange#{i}#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1918));
                    ImGui.TableSetColumnIndex(1);
                    Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, 31351);
                    if (itm != null)
                    {
                        ImGui.TableSetColumnIndex(1);
                        Utils.DrawIcon(globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawItemTooltip(currentLocale, ref globalCache, itm.Value);
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

                    switch (i)
                    {
                        case 1:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30715, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30716, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30717, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30718, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30719, 14);

                                break;
                            }
                        case 2:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30720, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30721, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30722, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30723, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30724, 14);

                                break;
                            }
                        case 3:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30725, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30726, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30727, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30728, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30729, 14);
                                break;
                            }
                        case 4:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30730, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30731, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30732, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30733, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30734, 14);
                                break;
                            }
                        case 5:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30735, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30736, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30737, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30738, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30739, 14);

                                break;
                            }
                        case 6:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30740, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30741, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30742, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30743, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30744, 14);
                                break;
                            }
                        case 7:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30745, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30746, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30747, 14);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30748, 28);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 30749, 14);
                                break;
                            }
                        case 8:
                            {

                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 31352, 500);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 31353, 999);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 31354, 500);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 31355, 999);
                                Reward.DrawAllCharsItemAcquired(currentLocale, globalCache, chars, 31356, 500);
                                break;
                            }
                    }
                }
            }
        }

        private static void DrawCosmicExploration(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#TabBar#CosmicExploration");
            if (!tab) return;

            using (var cosmicExplorationTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3460)}###CharactersProgressTable#All#TabBar#CosmicExploration#Vendor"))
            {
                if (cosmicExplorationTab)
                {
                    DrawCosmicExplorationVendor(globalCache, currentLocale, chars);
                }
            }
            using (var cosmicExplorationShuffleTab =
                   ImRaii.TabItem(
                       $"Cosmic Fortunes###CharactersProgressTable#All#TabBar#CosmicExploration#Shuffle"))
            {
                if (cosmicExplorationShuffleTab)
                {
                    DrawCosmicExplorationShuffle(globalCache, currentLocale, chars);
                }
            }
        }
        private static void DrawCosmicExplorationVendor(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Table",
                chars.Count + 2,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersEventTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Name",
                ImGuiTableColumnFlags.WidthFixed, 260);
            ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Currency",
                ImGuiTableColumnFlags.WidthFixed, 33);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

            ImGui.TableSetColumnIndex(1);
            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, (uint)Currencies.COSMOCREDIT);
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

            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 401, 29000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 425, 20000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 426, 20000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 445, 20000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 446, 20000);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 48091, 6000);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 46768, 6000);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 50019, 6000);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 51996, 6000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 449, 4000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 450, 6000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 458, 4000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 474, 4000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 294, 9600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Glass, 289, 6000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Glass, 373, 3000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Glass, 385, 3000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 737, 6000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 738, 6000);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 777, 6000);
        }
        private static void DrawCosmicExplorationShuffle(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#TabBar#CosmicExplorationShuffle");
            if (!tab) return;

            using (var cosmicExplorationShuffleSinusArdorumTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 16780)}###CharactersProgressTable#All#TabBar#CosmicExploration#Shuffle#SinusArdorum"))
            {
                if (cosmicExplorationShuffleSinusArdorumTab)
                {
                    using var charactersEventTable = ImRaii.Table(
                $"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Table",
                chars.Count + 2,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Currency",
                        ImGuiTableColumnFlags.WidthFixed, 25);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

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

                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Glass, 301, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 47, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 364, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 547, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 286, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 745, 0);
                }
            }
            using (var cosmicExplorationShufflePhaennaTab =
                   ImRaii.TabItem(
                       $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 16904)}###CharactersProgressTable#All#TabBar#CosmicExploration#Shuffle#Phaenna"))
            {
                if (cosmicExplorationShufflePhaennaTab)
                {
                    using var charactersEventTable = ImRaii.Table(
                        $"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Table",
                        chars.Count + 2,
                        ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                        ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
                    if (!charactersEventTable) return;
                    ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Name",
                        ImGuiTableColumnFlags.WidthFixed, 260);
                    ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#Currency",
                        ImGuiTableColumnFlags.WidthFixed, 25);
                    foreach (Character c in chars)
                    {
                        ImGui.TableSetupColumn($"###CharactersProgress#All#FieldOperations#CosmicExplorationRewards#{c.CharacterId}",
                            ImGuiTableColumnFlags.WidthFixed, 20);
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

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

                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 386, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 553, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 776, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Emote, 304, 0);
                    Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Glass, 397, 0);
                }
            }
        }
    }
}
