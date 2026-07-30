using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Altoholic.Helpers
{
    internal class IshgardianRestoration
    {
        public static void DrawRewards(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, uint eventCurrencyId = 0)
        {
            int columns = chars.Count + 1;
            if (eventCurrencyId > 0)
            {
                columns += 1;
            }

            using var charactersIsgardianRestorationSkybuildersTable = ImRaii.Table(
            $"###CharactersProgress#All#IsgardianRestoration#RewardTable#{eventCurrencyId}",
            columns,
            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
            ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersIsgardianRestorationSkybuildersTable) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#IsgardianRestoration#RewardTable#Skybuilders#Name",
                ImGuiTableColumnFlags.WidthFixed, 270);
            if (eventCurrencyId > 0)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#IsgardianRestoration#RewardTable#{eventCurrencyId}#Currency",
                    ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("1000").X + 5);
            }
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#IsgardianRestoration#RewardTable#{eventCurrencyId}#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }

            ImGui.TableSetupScrollFreeze(columns, 1); //Freeze header so it shows while scrolling
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

            if (eventCurrencyId == 28063)
            {
                DrawSkybuildersScripRewards(currentLocale, globalCache, chars);
            }
            else
            {
                DrawFetePresentRewards(currentLocale, globalCache, chars);
            }
        }

        private static void DrawFetePresentRewards(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars)
        {
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 34, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 35, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 45, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 28, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 25, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 76, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 41, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 47, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 22, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 52, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 220, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 208, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 207, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 203, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 6, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 7, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 1, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 13, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 2, 0);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 30113, 0);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 28615, 0);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 31406, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 136, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 226, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 363, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 197, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 139, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 357, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 180, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 97, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 137, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 66, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 138, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 243, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 166, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 375, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 428, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 165, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 374, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 376, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 167, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 248, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 247, 0);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 441, 0);
        }

        private static void DrawSkybuildersScripRewards(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars)
        {
            // SpecialShopRowid = 1770041;
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 209, 8400);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 211, 8400);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 225, 8400);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 236, 8400);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 242, 8400);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 67, 4200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 203, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 207, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 208, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 213, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 220, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 223, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 206, 1800);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 28615, 1800);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 30113, 1800);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 31406, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 360, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 357, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 363, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 157, 800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 162, 800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 190, 800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 194, 800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Barding, 76, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 349, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 377, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 398, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 428, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 441, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 463, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 554, 1200);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 123, 600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 374, 600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 375, 600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 376, 600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 382, 600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 397, 600);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 1, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 7, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 13, 1800);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 2, 900);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 6, 900);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.TripleTriadCard, 286, 500);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.TripleTriadCard, 295, 500);
        }

        internal static void Draw(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            using var tab = ImRaii.TabBar("###CharactersProgressTable#All#Quests#TabBar");
            if (!tab) return;
            using (var skybuildersTab = ImRaii.TabItem($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 5758)}###CharactersProgressTable#All#TabBar#IshgardianRestoration#Skybuilders"))
            {
                if (skybuildersTab)
                {
                    DrawRewards(currentLocale, globalCache, chars, 28063);
                }
            }
            Item? item = globalCache.ItemStorage.LoadItem(currentLocale, 33441);
            if (item is null) return;
            using var fetePresentTab = ImRaii.TabItem($"{item.Value.Name}###CharactersProgressTable#All#TabBar#IshgardianRestoration#FetePresent");
            if (fetePresentTab)
            {

                ImGui.TextUnformatted($"{item.Value.Name}: {Loc.Localize("FetePresentPotentialRewards", "Potential items")}");
                DrawRewards(currentLocale, globalCache, chars);
            }
        } 
    }
}
