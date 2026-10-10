using Altoholic.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using System.Collections.Generic;

namespace Altoholic.Helpers
{
    public static class IslandSanctuary
    {
        public static void DrawRewards(Cache.GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
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
                    ImGuiTableColumnFlags.WidthFixed, 25);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1885));

            ImGui.TableSetColumnIndex(1);
            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, (uint)Currencies.SEAFARERS_COWRIE);
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

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 14252).Replace(":", ""));
            ImGui.TableSetColumnIndex(1);
            int neededCowries = 381000;
            Dictionary<ulong, int> charactersTotalNeededCowries = [];
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{(currChar.IslandSanctuaryUnlocked ? currChar.IslandSanctuaryLevel : "")}");

                charactersTotalNeededCowries[currChar.CharacterId] = neededCowries;
            }

            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 277, 24000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 286, 35000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 282, 50000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 335, 100000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 255, 12000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 256, 12000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 257, 12000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 258, 18000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 259, 18000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Mount, 260, 18000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 456, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 468, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 481, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Minion, 496, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 30, 6000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 34, 6000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 38, 6000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Ornament, 28, 6000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 38442, 6000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsHairstyle(currentLocale, globalCache, chars, 38443, 6000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 544, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 545, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Orchestrion, 593, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.TripleTriadCard, 370, 1000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 89, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, Helpers.CharacterCollectible.Barding, 93, 4000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 39578, 3000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 39579, 3000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsFramerKit(currentLocale, globalCache, chars, 46822, 3000, charactersTotalNeededCowries);
            Helpers.Reward.DrawAllCharsTotal(currentLocale, globalCache, chars, Currencies.SEAFARERS_COWRIE,  neededCowries, charactersTotalNeededCowries);
        }
    }
}
