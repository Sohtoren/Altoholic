using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Helpers
{
    public enum CharacterCollectible {
        Barding,
        Emote,
        FramerKit,
        Glass,
        Minion,
        Mount,
        Orchestrion,
        Ornament,
        TripleTriadCard
    }
    public abstract class Reward
    {
        public static (string, string, IDalamudTextureWrap?) GetCollectibleNameTransientDescriptionIcon(ClientLanguage currentLocale, GlobalCache globalCache, CharacterCollectible collectible, uint id)
        {
            string name = string.Empty;
            string transientOrDescription = string.Empty;
            IDalamudTextureWrap? icon = null;
            switch (collectible)
            {
                case CharacterCollectible.Barding:
                    {
                        Models.Barding? barding = globalCache.BardingStorage.GetBarding(currentLocale, id);
                        if (barding is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(barding.GermanName),
                            ClientLanguage.English => Utils.Capitalize(barding.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(barding.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(barding.JapaneseName),
                            _ => Utils.Capitalize(barding.EnglishName)
                        };
                        icon = globalCache.IconStorage.LoadIcon(barding.Icon);
                        break;
                    }                
                case CharacterCollectible.Emote:
                    {
                        Models.Emote? emote = globalCache.EmoteStorage.GetEmote(currentLocale, id);
                        if (emote is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => emote.GermanName,
                            ClientLanguage.English => Utils.CapitalizeSentence(emote.EnglishName),
                            ClientLanguage.French => Utils.CapitalizeSentence(emote.FrenchName),
                            ClientLanguage.Japanese => emote.JapaneseName,
                            _ => Utils.CapitalizeSentence(emote.EnglishName)
                        };
                        icon = globalCache.IconStorage.LoadIcon(emote.Icon);
                        break;
                    }
                case CharacterCollectible.FramerKit:
                    {
                        Models.FramerKit? fk = globalCache.FramerKitStorage.LoadItem(currentLocale, id);
                        if (fk is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(fk.GermanName),
                            ClientLanguage.English => Utils.Capitalize(fk.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(fk.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(fk.JapaneseName),
                            _ => Utils.Capitalize(fk.EnglishName)
                        };
                        icon = globalCache.IconStorage.LoadIcon(fk.Icon);
                        break;
                    }
                case CharacterCollectible.Glass:
                    {
                        Models.Glasses? glasses = globalCache.GlassesStorage.GetGlasses(currentLocale, id);
                        if(glasses is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(glasses.GermanName),
                            ClientLanguage.English => Utils.Capitalize(glasses.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(glasses.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(glasses.JapaneseName),
                            _ => Utils.Capitalize(glasses.EnglishName)
                        };
                        transientOrDescription = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(glasses.GermanDescription),
                            ClientLanguage.English => Utils.Capitalize(glasses.EnglishDescription),
                            ClientLanguage.French => Utils.Capitalize(glasses.FrenchDescription),
                            ClientLanguage.Japanese => Utils.Capitalize(glasses.JapaneseDescription),
                            _ => Utils.Capitalize(glasses.EnglishDescription)
                        };
                        icon = globalCache.IconStorage.LoadIcon(glasses.Icon);
                        break;
                    }
                case CharacterCollectible.Minion:
                    {
                        Models.Minion? minion = globalCache.MinionStorage.GetMinion(currentLocale, id);
                        if (minion is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(minion.GermanName),
                            ClientLanguage.English => Utils.Capitalize(minion.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(minion.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(minion.JapaneseName),
                            _ => Utils.Capitalize(minion.EnglishName)
                        };
                        if (minion.Transient is not null)
                        {
                            transientOrDescription = currentLocale switch
                            {
                                ClientLanguage.German => Utils.Capitalize(minion.Transient.GermanTooltip),
                                ClientLanguage.English => Utils.Capitalize(minion.Transient.EnglishTooltip),
                                ClientLanguage.French => Utils.Capitalize(minion.Transient.FrenchTooltip),
                                ClientLanguage.Japanese => Utils.Capitalize(minion.Transient.JapaneseTooltip),
                                _ => Utils.Capitalize(minion.Transient.EnglishTooltip)
                            };
                        }
                        icon = globalCache.IconStorage.LoadIcon(minion.Icon);
                        break;
                    }
                case CharacterCollectible.Mount:
                    {
                        Models.Mount? mount = globalCache.MountStorage.GetMount(currentLocale, id);
                        if (mount is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(mount.GermanName),
                            ClientLanguage.English => Utils.Capitalize(mount.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(mount.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(mount.JapaneseName),
                            _ => Utils.Capitalize(mount.EnglishName)
                        };
                        if (mount.Transient is not null)
                        {
                            transientOrDescription = currentLocale switch
                            {
                                ClientLanguage.German => Utils.Capitalize(mount.Transient.GermanTooltip),
                                ClientLanguage.English => Utils.Capitalize(mount.Transient.EnglishTooltip),
                                ClientLanguage.French => Utils.Capitalize(mount.Transient.FrenchTooltip),
                                ClientLanguage.Japanese => Utils.Capitalize(mount.Transient.JapaneseTooltip),
                                _ => Utils.Capitalize(mount.Transient.EnglishTooltip)
                            };
                        }
                        icon = globalCache.IconStorage.LoadIcon(mount.Icon);
                        break;
                    }
                case CharacterCollectible.Orchestrion:
                    {
                        Models.OrchestrionRoll? orchestrion = globalCache.OrchestrionRollStorage.GetOrchestrionRoll(currentLocale, id);
                        if (orchestrion is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(orchestrion.GermanName),
                            ClientLanguage.English => Utils.Capitalize(orchestrion.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(orchestrion.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(orchestrion.JapaneseName),
                            _ => Utils.Capitalize(orchestrion.EnglishName)
                        };
                        icon = globalCache.IconStorage.LoadIcon(orchestrion.Icon);
                        break; 
                    }
                case CharacterCollectible.Ornament:
                    {
                        Models.Ornament? ornament = globalCache.OrnamentStorage.GetOrnament(currentLocale, id);
                        if (ornament is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(ornament.GermanName),
                            ClientLanguage.English => Utils.Capitalize(ornament.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(ornament.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(ornament.JapaneseName),
                            _ => Utils.Capitalize(ornament.EnglishName)
                        };
                        if (ornament.Transient is not null)
                        {
                            transientOrDescription = currentLocale switch
                            {
                                ClientLanguage.German => Utils.Capitalize(ornament.Transient.GermanTooltip),
                                ClientLanguage.English => Utils.Capitalize(ornament.Transient.EnglishTooltip),
                                ClientLanguage.French => Utils.Capitalize(ornament.Transient.FrenchTooltip),
                                ClientLanguage.Japanese => Utils.Capitalize(ornament.Transient.JapaneseTooltip),
                                _ => Utils.Capitalize(ornament.Transient.EnglishTooltip)
                            };
                        }
                        icon = globalCache.IconStorage.LoadIcon(ornament.Icon);
                        break;
                    }
                case CharacterCollectible.TripleTriadCard:
                    {
                        Models.TripleTriadCard? ttc = globalCache.TripleTriadCardStorage.GetTripleTriadCard(currentLocale, id);
                        if (ttc is null) break;
                        name = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(ttc.GermanName),
                            ClientLanguage.English => Utils.Capitalize(ttc.EnglishName),
                            ClientLanguage.French => Utils.Capitalize(ttc.FrenchName),
                            ClientLanguage.Japanese => Utils.Capitalize(ttc.JapaneseName),
                            _ => Utils.Capitalize(ttc.EnglishName)
                        };
                        transientOrDescription = currentLocale switch
                        {
                            ClientLanguage.German => Utils.Capitalize(ttc.GermanDescription),
                            ClientLanguage.English => Utils.Capitalize(ttc.EnglishDescription),
                            ClientLanguage.French => Utils.Capitalize(ttc.FrenchDescription),
                            ClientLanguage.Japanese => Utils.Capitalize(ttc.JapaneseDescription),
                            _ => Utils.Capitalize(ttc.EnglishDescription)
                        };
                        icon = globalCache.IconStorage.LoadIcon(027672);
                        break;
                    }
                default:
                    break;
            }

            return (name, transientOrDescription, icon);
        }
        public static void DrawCollectibleTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, uint id, IDalamudTextureWrap? icon, string name, string transientOrDescription)
        {
            using var drawTooltip = ImRaii.Tooltip();

            using (var drawTooltipItem = ImRaii.Table($"###DrawCollectibleTooltip#Collectible_{id}", 2))
            {
                if (!drawTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawCollectibleTooltip#Collectible_{id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawCollectibleTooltip#Collectible_{id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(icon, new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
            }

            ImGui.Separator();
            if (string.IsNullOrEmpty(transientOrDescription)) return;
            ImGui.TextUnformatted($"{Utils.Capitalize(transientOrDescription)}");
        }
        public static void DrawAllCharsCollectible(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, CharacterCollectible collectible, uint id, int cost, Dictionary<ulong, int>? characterNeededTomestone = null)
        {
            (string name, string transientOrDescription, IDalamudTextureWrap? icon) = GetCollectibleNameTransientDescriptionIcon(currentLocale, globalCache, collectible, id);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(icon, new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                DrawCollectibleTooltip(currentLocale, ref globalCache, id, icon, name, transientOrDescription);
            }

            ImGui.SameLine();
            ImGui.TextUnformatted(name);

            if (cost > 0)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{cost}");
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                if (currChar.HasCollectible(collectible, id))
                {
                    characterNeededTomestone?[currChar.CharacterId] = characterNeededTomestone[currChar.CharacterId] - cost;
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
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

        public static void DrawAllCharsFramerKit(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, uint id, int cost, Dictionary<ulong, int>? characterNeededTomestone = null)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            uint? fkId = globalCache.FramerKitStorage.GetFramerKitIdFromItemId(id);
            if (fkId != null)
            {
                FramerKit? fk = globalCache.FramerKitStorage.LoadItem(currentLocale, fkId.Value);
                if (fk != null)
                {
                    (string name, string transientOrDescription, IDalamudTextureWrap? icon) = GetCollectibleNameTransientDescriptionIcon(currentLocale, globalCache, Helpers.CharacterCollectible.FramerKit, fk.Id);
                    Utils.DrawIcon(icon, new Vector2(32, 32));
                    if (ImGui.IsItemHovered())
                    {
                        DrawCollectibleTooltip(currentLocale, ref globalCache, fk.Id, icon, name, transientOrDescription);
                    }

                    ImGui.SameLine();
                    ImGui.TextUnformatted(name);

                    if (cost > 0)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{cost}");
                    }

                    foreach (Character currChar in chars)
                    {
                        ImGui.TableNextColumn();
                        if (currChar.HasFramerKit(fk.Id))
                        {
                            ImGui.PushFont(UiBuilder.IconFont);
                            characterNeededTomestone?[currChar.CharacterId] = characterNeededTomestone[currChar.CharacterId] - cost;
                            ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());

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
        }
        public static void DrawAllCharsHairstyle(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, uint itemId, int cost, Dictionary<ulong, int>? characterNeededTomestone = null)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, itemId);
            if (itm == null)
            {
                return;
            }
            Item i = itm.Value;
            ushort unlockLink = i.ItemAction.Value.Data[0];
            List<uint> ids = globalCache.HairstyleStorage.GetIdsFromUnlockLink(unlockLink);
            Utils.DrawIcon(globalCache.IconStorage.LoadIcon(i.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                Utils.DrawItemTooltip(currentLocale, ref globalCache, i);
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
                if (currChar.HasHairstyleFromIds(ids))
                {
                    ImGui.PushFont(UiBuilder.IconFont);
                    characterNeededTomestone?[currChar.CharacterId] = characterNeededTomestone[currChar.CharacterId] - cost;
                    ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
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
        }
        public static void DrawAllCharsTotal(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int total, Dictionary<ulong, int>? characterNeededTomestone = null)
        {
            string totalStr = currentLocale switch
            {
                ClientLanguage.German => "Gesamtbedarf",
                ClientLanguage.English => "Total required",
                ClientLanguage.French => "Total requis",
                ClientLanguage.Japanese => "必要合計",
                _ => "Total required"
            };
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{totalStr}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{total}");
            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{(characterNeededTomestone is not null ? characterNeededTomestone[currChar.CharacterId] : "")}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.TextUnformatted($"{currChar.Currencies?.Irregular_Tomestone_Of_Aphorism}{(characterNeededTomestone is not null ? "/" + characterNeededTomestone[currChar.CharacterId] : "")}");
                    ImGui.EndTooltip();
                }
            }
        }

        /*private static Vector2 GetPosVector(CharacterCollectible collectible, Vector2 p)
        {
            return collectible switch
            {
                CharacterCollectible.Minion => new Vector2(p.X + 26, p.Y + 20),
                CharacterCollectible.Mount => new Vector2(p.X + 26, p.Y + 20),
                CharacterCollectible.TripleTriadCard => new Vector2(p.X + 26, p.Y + 20),
            };                
        }*/

        public static void DrawCollectible(ClientLanguage currentLocale, GlobalCache globalCache, CharacterCollectible collectible, bool isSpoilerEnabled, uint id, bool hasCollectible)
        {
            Vector2 p = ImGui.GetCursorPos();
            (string name, string transientOrDescription, IDalamudTextureWrap? icon) = GetCollectibleNameTransientDescriptionIcon(currentLocale, globalCache, collectible, id);

            if (!hasCollectible)
            {
                if (isSpoilerEnabled)
                {
                    Utils.DrawIcon(icon, new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        DrawCollectibleTooltip(currentLocale, ref globalCache, id, icon, name, transientOrDescription);
                    }
                }
                else
                {
                    Utils.DrawIcon(globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(icon, new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    DrawCollectibleTooltip(currentLocale, ref globalCache, id, icon, name, transientOrDescription);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }
    }
}
