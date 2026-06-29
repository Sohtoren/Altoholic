using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altoholic.Helpers
{
    public abstract class CustomDelivery
    {
        public static Models.CustomDelivery? GetNPC(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<SatisfactionNpc> dc = Plugin.DataManager.GetExcelSheet<SatisfactionNpc>(currentLocale);
            SatisfactionNpc? lumina = dc?.GetRow(id);
            if (lumina is null || !lumina.HasValue) return null;
            if (!lumina.Value.Npc.IsValid) return null;
            Models.CustomDelivery c = new() { Id = lumina.Value.RowId, Icon = (uint)lumina.Value.Icon, GlamourIndex = lumina.Value.GlamourIndex, LevelUnlock = lumina.Value.LevelUnlock, DeliveriesPerWeek = lumina.Value.DeliveriesPerWeek, QuestRequired = lumina.Value.QuestRequired.Value.RowId };
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    c.GermanName = lumina.Value.Npc.Value.Singular.ExtractText();
                    break;
                case ClientLanguage.English:
                    c.EnglishName = lumina.Value.Npc.Value.Singular.ExtractText();
                    break;
                case ClientLanguage.French:
                    c.FrenchName = lumina.Value.Npc.Value.Singular.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    c.JapaneseName = lumina.Value.Npc.Value.Singular.ExtractText();
                    break;
            }
            return c;
        }

        public static List<Models.CustomDelivery>? GetAllNPC(ClientLanguage currentLocale)
        {
            List<Models.CustomDelivery> returnedIds = [];
            ExcelSheet<SatisfactionNpc>? btm = Plugin.DataManager.GetExcelSheet<SatisfactionNpc>(currentLocale);
            using IEnumerator<SatisfactionNpc>? btEnumerator = btm?.GetEnumerator();
            if (btEnumerator is null) return null;

            while (btEnumerator.MoveNext())
            {
                SatisfactionNpc sNPC = btEnumerator.Current;
                if (!sNPC.Npc.IsValid) continue;
                if (sNPC.Npc.Value.Singular.IsEmpty) continue;
                //if (sNPC.Icon == 0) continue;
                Models.CustomDelivery c = new() { Id = sNPC.RowId, Icon = (uint)sNPC.Icon, GlamourIndex = sNPC.GlamourIndex, LevelUnlock = sNPC.LevelUnlock, DeliveriesPerWeek = sNPC.DeliveriesPerWeek, QuestRequired = sNPC.QuestRequired.Value.RowId };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        c.GermanName = sNPC.Npc.Value.Singular.ExtractText();
                        break;
                    case ClientLanguage.English:
                        c.EnglishName = sNPC.Npc.Value.Singular.ExtractText();
                        break;
                    case ClientLanguage.French:
                        c.FrenchName = sNPC.Npc.Value.Singular.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        c.JapaneseName = sNPC.Npc.Value.Singular.ExtractText();
                        break;
                }

                returnedIds.Add(c);
            }

            return returnedIds;
        }

        public static void Draw(IDalamudTextureWrap? satisfactionRanksTexture, Cache.GlobalCache globalCache, ClientLanguage currentLocale, bool isSpoilerEnabled, Character selectedCharacter)
        {
            if (!selectedCharacter.HasAnyCustomDeliveryUnlocked())
            {
                ImGui.TextUnformatted($"{Loc.Localize("NoCustomDeliveryUnlocked", "No Custom Deliveries has been unlocked")}");
                return;
            }

            using var charactersCustomDelivery = ImRaii.Table($"###CharactersProgress#{selectedCharacter.CharacterId}#CustomDelivery", 3,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersCustomDelivery) return;

            ImGui.TableSetupColumn($"###CharactersProgress#{selectedCharacter.CharacterId}#CustomDelivery#Icon", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableSetupColumn($"###CharactersProgress#{selectedCharacter.CharacterId}#CustomDelivery#NPC", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn($"###CharactersProgress#{selectedCharacter.CharacterId}#CustomDelivery#Satisfaction", ImGuiTableColumnFlags.WidthFixed, 250);

            for (int i = 1; i < globalCache.CustomDeliveryStorage.Count() + 1; i++)
            {
                Models.CustomDelivery? npc = globalCache.CustomDeliveryStorage.GetCustomDeliveryNPC(currentLocale, (uint)i);
                if (npc is null) continue;

                if (!selectedCharacter.HasQuest((int)npc.QuestRequired) && !isSpoilerEnabled)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 627)}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 627)}");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted($"{Loc.Localize("CustomDeliveryNpcNotUnlocked", "You have not unlocked this NPC")}");
                    continue;
                }

                string name = currentLocale switch
                {
                    ClientLanguage.German => npc.GermanName,
                    ClientLanguage.English => npc.EnglishName,
                    ClientLanguage.French => npc.FrenchName,
                    ClientLanguage.Japanese => npc.JapaneseName,
                    _ => npc.EnglishName
                };

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(globalCache.IconStorage.LoadIcon(npc.Icon), new Vector2(50, 50));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{name}");
                ImGui.TableSetColumnIndex(2);
                if (selectedCharacter.HasQuest((int)npc.QuestRequired))
                {
                    for (int j = 1; j <= 5; j++)
                    {
                        if (j > 1)
                        {
                            ImGui.SameLine();
                        }
                        if (satisfactionRanksTexture is not null)
                        {
                            selectedCharacter.CustomDeliveries.TryGetValue(i - 1, out CustomDeliveryRank? rank);

                            if (rank is not null)
                            {
                                if (rank.HeartCount == 0) continue;
                                if (j == 5 && i is 1 or 3 or 4 or 5 or 7 or 8 or 9 or 10 or 11)
                                {
                                    if (rank.HeartCount == 5)
                                    {
                                        Vector2 p = ImGui.GetCursorPos();
                                        DrawTexture(ref satisfactionRanksTexture, CustomDeliverySatisfactionIcon.GlamourEmptyGold, new Vector2(64, 30));
                                        ImGui.SetCursorPos(new Vector2(p.X + 1, p.Y - 1));
                                        DrawTexture(ref satisfactionRanksTexture, CustomDeliverySatisfactionIcon.FullHeart, new Vector2(30, 30));
                                        ImGui.SetCursorPos(p);
                                    }
                                    else
                                    {
                                        DrawTexture(ref satisfactionRanksTexture, CustomDeliverySatisfactionIcon.GlamourEmptySilver, new Vector2(64, 30));
                                    }
                                }
                                else
                                {
                                    if (j <= rank.HeartCount)
                                    {
                                        DrawTexture(ref satisfactionRanksTexture, CustomDeliverySatisfactionIcon.FullHeart, new Vector2(30, 30));
                                    }
                                    else
                                    {
                                        DrawTexture(ref satisfactionRanksTexture, CustomDeliverySatisfactionIcon.EmptyHeart, new Vector2(30, 30));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    ImGui.TextUnformatted($"{Loc.Localize("CustomDeliveryNpcNotUnlocked", "You have not unlocked this NPC")}");
                }

            }
        }
        public enum CustomDeliverySatisfactionIcon
        {
            EmptyHeart,
            FullHeart,
            GlamourEmptyGold,
            GlamourGold,
            GlamourEmptySilver,
            GlamourSilver,
            BonusDoH,
            BonusDoL,
            BonusFsh
        }
        public static void DrawTexture(ref IDalamudTextureWrap texture, CustomDeliverySatisfactionIcon icon, Vector2 size)
        {
            (Vector2 uv0, Vector2 uv1) = icon switch
            {
                CustomDeliverySatisfactionIcon.EmptyHeart => Utils.GetTextureCoordinate(texture.Size, 0, 0, 64, 56),
                CustomDeliverySatisfactionIcon.FullHeart => Utils.GetTextureCoordinate(texture.Size, 64, 0, 64, 56),
                CustomDeliverySatisfactionIcon.GlamourEmptyGold => Utils.GetTextureCoordinate(texture.Size, 0, 56, 128, 56),
                CustomDeliverySatisfactionIcon.GlamourGold => Utils.GetTextureCoordinate(texture.Size, 80, 56, 56, 56),
                CustomDeliverySatisfactionIcon.GlamourEmptySilver => Utils.GetTextureCoordinate(texture.Size, 0, 112, 128, 56),
                CustomDeliverySatisfactionIcon.GlamourSilver => Utils.GetTextureCoordinate(texture.Size, 80, 112, 56, 56),
                CustomDeliverySatisfactionIcon.BonusDoH => Utils.GetTextureCoordinate(texture.Size, 0, 168, 64, 56),
                CustomDeliverySatisfactionIcon.BonusDoL => Utils.GetTextureCoordinate(texture.Size, 64, 168, 64, 56),
                CustomDeliverySatisfactionIcon.BonusFsh => Utils.GetTextureCoordinate(texture.Size, 0, 224, 64, 56),
                _ => Utils.GetTextureCoordinate(texture.Size, 0, 0, 0, 0)
            };
            ImGui.Image(texture.Handle, size, uv0, uv1);
        }

        internal static void DrawAll(GlobalCache globalCache, ClientLanguage currentLocale, List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var charactersCustomDeliveryQuestAll = ImRaii.Table("###CharactersProgress#All#CustomDelivery", chars.Count + 1,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersCustomDeliveryQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersProgress#All#CustomDelivery#Name", ImGuiTableColumnFlags.WidthFixed, 250);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersProgress#All#CustomDelivery#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
            }
            ImGui.TableSetupScrollFreeze(chars.Count + 1, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 14055));
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

            List<List<bool>> charatersCustomDeliveries = GetCharactersCustomDeliveryRank(chars, globalCache.CustomDeliveryStorage.Count());
            for (int i = 1; i < globalCache.CustomDeliveryStorage.Count() + 1; i++)
            {
                Models.CustomDelivery? npc = globalCache.CustomDeliveryStorage.GetCustomDeliveryNPC(currentLocale, (uint)i);
                if (npc is null) continue;

                string name = currentLocale switch
                {
                    ClientLanguage.German => npc.GermanName,
                    ClientLanguage.English => npc.EnglishName,
                    ClientLanguage.French => npc.FrenchName,
                    ClientLanguage.Japanese => npc.JapaneseName,
                    _ => npc.EnglishName
                };

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{name}");
                foreach ((List<bool> cq, int index) charactersCustomDelivery in charatersCustomDeliveries.Select((cq, index) => (cq, index)))
                {
                    ImGui.TableNextColumn();
                    ImGui.PushFont(UiBuilder.IconFont);
                    if (!chars[charactersCustomDelivery.index].HasQuest((int)npc.QuestRequired))
                    {
                        ImGui.TextUnformatted($"{FontAwesomeIcon.Times.ToIconString()}");
                    }
                    else
                    {
                        ImGui.TextUnformatted(charactersCustomDelivery.cq[i - 1] ? FontAwesomeIcon.Check.ToIconString() : "");
                    }
                    ImGui.PopFont();
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(name);
                        ImGui.TextUnformatted(
                            $"{chars[charactersCustomDelivery.index].FirstName} {chars[charactersCustomDelivery.index].LastName}{(char)SeIconChar.CrossWorld}{chars[charactersCustomDelivery.index].HomeWorld}");
                        ImGui.EndTooltip();
                    }
                }

            }
        }

        static List<List<bool>> GetCharactersCustomDeliveryRank(List<Character> characters, int customDeliveriesCount)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                List<bool> completedCustomDelivery = [];
                for (int i = 0; i < customDeliveriesCount; i++)
                {
                    character.CustomDeliveries.TryGetValue(i, out CustomDeliveryRank? rank);
                    if (rank is null || rank.HeartCount < 5)
                    {
                        completedCustomDelivery.Add(false);
                        continue;
                    }
                    if (rank.HeartCount == 5)
                    {
                        completedCustomDelivery.Add(true);
                    }
                }
                result.Add(completedCustomDelivery);
            }
            return result;
        }
    }
}