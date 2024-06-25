using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClassJob = Lumina.Excel.GeneratedSheets.ClassJob;
using Mount = Lumina.Excel.GeneratedSheets.Mount;
using Stain = Lumina.Excel.GeneratedSheets.Stain;
using TripleTriadCard = Lumina.Excel.GeneratedSheets.TripleTriadCard;
using Emote = Lumina.Excel.GeneratedSheets.Emote;
using TextCommand = Lumina.Excel.GeneratedSheets.TextCommand;
using System.Xml.Linq;

namespace Altoholic
{
    internal class Utils
    {
        // ReSharper disable once InconsistentNaming
        private const uint FALLBACK_ICON = 055396;

        public static string GetDCString()
        {
            return "DC";
        }

        public static string GetDatacenterFromWorld(string name)
        {
            if (Array.Exists(Datacenter.Aether, w => w == name))
            {
                return "Aether";
            }

            if (Array.Exists(Datacenter.Chaos, w => w == name))
            {
                return "Chaos";
            }

            if (Array.Exists(Datacenter.Crystal, w => w == name))
            {
                return "Crystal";
            }

            if (Array.Exists(Datacenter.Dynamis, w => w == name))
            {
                return "Dynamis";
            }

            if (Array.Exists(Datacenter.Elemental, w => w == name))
            {
                return "Elemental";
            }

            if (Array.Exists(Datacenter.Gaia, w => w == name))
            {
                return "Gaia";
            }

            if (Array.Exists(Datacenter.Light, w => w == name))
            {
                return "Light";
            }

            if (Array.Exists(Datacenter.Mana, w => w == name))
            {
                return "Mana";
            }

            if (Array.Exists(Datacenter.Materia, w => w == name))
            {
                return "Materia";
            }

            if (Array.Exists(Datacenter.Meteor, w => w == name))
            {
                return "Meteor";
            }

            if (Array.Exists(Datacenter.Primal, w => w == name))
            {
                return "Primal";
            }

            return Array.Exists(Datacenter.Shadow, w => w == name) ? "Shadow" : string.Empty;
        }

        public static string GetRegionFromWorld(string name)
        {
            if (Array.Exists(Region.EU, w => w == name))
            {
                return "EU";
            }

            if (Array.Exists(Region.JP, w => w == name))
            {
                return "JP";
            }

            if (Array.Exists(Region.NA, w => w == name))
            {
                return "NA";
            }

            return Array.Exists(Region.OCE, w => w == name) ? "OCE" : string.Empty;
        }

        public static string GetGender(int gender)
        {
            return gender switch
            {
                0 => "♂",
                1 => "♀",
                _ => string.Empty,
            };
        }

        public static string GetRace(ClientLanguage currentLocale, int gender, uint race)
        {
            ExcelSheet<Race>? dr = Plugin.DataManager.GetExcelSheet<Race>(currentLocale);
            Race? lumina = dr?.GetRow(race);
            if (lumina != null)
                return gender == 0 ? lumina.Masculine : lumina.Feminine;
            return string.Empty;
        }

        public static string GetTribe(ClientLanguage currentLocale, int gender, uint tribe)
        {
            ExcelSheet<Tribe>? dt = Plugin.DataManager.GetExcelSheet<Tribe>(currentLocale);
            Tribe? lumina = dt?.GetRow(tribe);
            if (lumina != null)
                return gender == 0 ? lumina.Masculine : lumina.Feminine;
            return string.Empty;
        }

        public static string GetTown(ClientLanguage currentLocale, int town)
        {
            ExcelSheet<Town>? dt = Plugin.DataManager.GetExcelSheet<Town>(currentLocale);
            Town? lumina = dt?.GetRow((uint)town);
            return lumina != null ? lumina.Name : string.Empty;
        }

        public static uint GetTownIcon(int town)
        {
            return town switch
            {
                1 => 060881,
                2 => 060882,
                3 => 060883,
                _ => FALLBACK_ICON,
            };
        }

        public static string GetGuardian(ClientLanguage currentLocale, int guardian)
        {
            ExcelSheet<GuardianDeity>? dg = Plugin.DataManager.GetExcelSheet<GuardianDeity>(currentLocale);
            GuardianDeity? lumina = dg?.GetRow((uint)guardian);
            return lumina != null ? lumina.Name : string.Empty;
        }

        public static uint GetGuardianIcon(int guardian)
        {
            return guardian switch
            {
                1 => 061601,
                2 => 061602,
                3 => 061603,
                4 => 061604,
                5 => 061605,
                6 => 061606,
                7 => 061607,
                8 => 061608,
                9 => 061609,
                10 => 061610,
                11 => 061611,
                12 => 061612,
                _ => FALLBACK_ICON,
            };
        }

        public static string GetNameday(int day, int month)
        {
            string nameday = day switch
            {
                1 or 21 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th",
            };
            string namedaymonth = month switch
            {
                1 or 21 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th",
            };
            return $"{day}{nameday} Sun of the {month}{namedaymonth} Astral Moon";
        }

        public static string GetGrandCompany(ClientLanguage currentLocale, int id)
        {
            ExcelSheet<GrandCompany>? dgc = Plugin.DataManager.GetExcelSheet<GrandCompany>(currentLocale);
            GrandCompany? lumina = dgc?.GetRow((uint)id);
            return lumina != null ? lumina.Name : string.Empty;
        }

        public static uint GetGrandCompanyIcon(int company)
        {
            return company switch
            {
                1 => 060871,
                2 => 060872,
                3 => 060873,
                _ => FALLBACK_ICON,
            };
        }

        public static string GetGrandCompanyRank(ClientLanguage currentLocale, int company, int rank, int gender)
        {
            switch (company)
            {
                case 1:
                    {
                        if (gender == 0)
                        {
                            ExcelSheet<GCRankLimsaMaleText>? dgcrlmt =
                                Plugin.DataManager.GetExcelSheet<GCRankLimsaMaleText>(currentLocale);
                            if (dgcrlmt == null)
                            {
                                return string.Empty;
                            }

                            GCRankLimsaMaleText? lumina = dgcrlmt.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }
                        else
                        {
                            ExcelSheet<GCRankLimsaFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankLimsaFemaleText>(currentLocale);
                            if (dgcrlft == null)
                            {
                                return string.Empty;
                            }

                            GCRankLimsaFemaleText? lumina = dgcrlft.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }

                        return string.Empty;
                    }
                case 2:
                    {
                        if (gender == 0)
                        {
                            ExcelSheet<GCRankUldahMaleText>? dgcrlmt =
                                Plugin.DataManager.GetExcelSheet<GCRankUldahMaleText>(currentLocale);
                            if (dgcrlmt == null)
                            {
                                return string.Empty;
                            }

                            GCRankUldahMaleText? lumina = dgcrlmt.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }
                        else
                        {
                            ExcelSheet<GCRankUldahFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankUldahFemaleText>(currentLocale);
                            GCRankUldahFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }

                        return string.Empty;
                    }
                case 3:
                    {
                        if (gender == 0)
                        {
                            ExcelSheet<GCRankGridaniaMaleText>? dgcrlmt =
                                Plugin.DataManager.GetExcelSheet<GCRankGridaniaMaleText>(currentLocale);
                            GCRankGridaniaMaleText? lumina = dgcrlmt?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }
                        else
                        {
                            ExcelSheet<GCRankGridaniaFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankGridaniaFemaleText>(currentLocale);
                            GCRankGridaniaFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }

                        return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

        public static uint GetGrandCompanyRankMaxSeals(ClientLanguage currentLocale, int rank)
        {
            ExcelSheet<GrandCompanyRank>? dgcr = Plugin.DataManager.GetExcelSheet<GrandCompanyRank>(currentLocale);
            GrandCompanyRank? lumina = dgcr?.GetRow((uint)rank);
            return lumina?.MaxSeals ?? 0;
        }

        public static uint GetGrandCompanyRankIcon(int company, int rank)
        {
            return rank switch
            {
                1 => company switch
                {
                    1 => 083601,
                    2 => 083701,
                    3 => 083651,
                    _ => FALLBACK_ICON,
                },
                2 => company switch
                {
                    1 => 083602,
                    2 => 083702,
                    3 => 083652,
                    _ => FALLBACK_ICON,
                },
                3 => company switch
                {
                    1 => 083603,
                    2 => 083703,
                    3 => 083653,
                    _ => FALLBACK_ICON,
                },
                4 => company switch
                {
                    1 => 083604,
                    2 => 083704,
                    3 => 083654,
                    _ => FALLBACK_ICON,
                },
                5 => company switch
                {
                    1 => 083605,
                    2 => 083705,
                    3 => 083655,
                    _ => FALLBACK_ICON,
                },
                6 => company switch
                {
                    1 => 083606,
                    2 => 083706,
                    3 => 083656,
                    _ => FALLBACK_ICON,
                },
                7 => company switch
                {
                    1 => 083607,
                    2 => 083707,
                    3 => 083657,
                    _ => FALLBACK_ICON,
                },
                8 => company switch
                {
                    1 => 083608,
                    2 => 083708,
                    3 => 083658,
                    _ => FALLBACK_ICON,
                },
                9 => company switch
                {
                    1 => 083609,
                    2 => 083709,
                    3 => 083659,
                    _ => FALLBACK_ICON,
                },
                10 => company switch
                {
                    1 => 083610,
                    2 => 083710,
                    3 => 083660,
                    _ => FALLBACK_ICON,
                },
                11 => company switch
                {
                    1 => 083611,
                    2 => 083711,
                    3 => 083661,
                    _ => FALLBACK_ICON,
                },
                12 => company switch
                {
                    1 => 083612,
                    2 => 083712,
                    3 => 083662,
                    _ => FALLBACK_ICON,
                },
                13 => company switch
                {
                    1 => 083613,
                    2 => 083713,
                    3 => 083663,
                    _ => FALLBACK_ICON,
                },
                14 => company switch
                {
                    1 => 083614,
                    2 => 083714,
                    3 => 083664,
                    _ => FALLBACK_ICON,
                },
                15 => company switch
                {
                    1 => 083615,
                    2 => 083715,
                    3 => 083665,
                    _ => FALLBACK_ICON,
                },
                16 => company switch
                {
                    1 => 083616,
                    2 => 083716,
                    3 => 083666,
                    _ => FALLBACK_ICON,
                },
                17 => company switch
                {
                    1 => 083617,
                    2 => 083717,
                    3 => 083667,
                    _ => FALLBACK_ICON,
                },
                18 => company switch
                {
                    1 => 083618,
                    2 => 083718,
                    3 => 083668,
                    _ => FALLBACK_ICON,
                },
                19 => company switch
                {
                    1 => 083619,
                    2 => 083719,
                    3 => 083669,
                    _ => FALLBACK_ICON,
                },
                20 => company switch
                {
                    1 => 083620,
                    2 => 083720,
                    3 => 083670,
                    _ => FALLBACK_ICON,
                },
                _ => FALLBACK_ICON,
            };
        }

        public static Item? GetItemFromId(ClientLanguage currentLocale, uint id)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            Item? lumina = ditm?.GetRow(id);
            return lumina;
        }

        public static EventItem? GetEventItemFromId(ClientLanguage currentLocale, uint id)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<EventItem>? deitm = Plugin.DataManager.GetExcelSheet<EventItem>(currentLocale);
            EventItem? lumina = deitm?.GetRow(id);
            return lumina;
        }

        public static IEnumerable<Item>? GetItemsFromName(ClientLanguage currentLocale, string name)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            IEnumerable<Item>? items = ditm?.Where(i =>
                i.Name.RawString.Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return items;
        }

        public static Item? GetItemFromName(ClientLanguage currentLocale, string name)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            Item? item = ditm?.FirstOrDefault(i =>
                i.Name.RawString.Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return item;
        }

        public static ItemLevel? GetItemLevelFromId(uint id)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<ItemLevel>? dilvl = Plugin.DataManager.GetExcelSheet<ItemLevel>(ClientLanguage.English);
            ItemLevel? lumina = dilvl?.GetRow(id);
            return lumina;
        }

        public static Stain? GetStainFromId(uint id, ClientLanguage clientLanguage)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<Stain>? ds = Plugin.DataManager.GetExcelSheet<Stain>(clientLanguage);
            Stain? lumina = ds?.GetRow(id);
            return lumina;
        }

        public static uint GetJobIcon(uint jobId)
        {
            return jobId switch
            {
                1 => 062001,
                2 => 062002,
                3 => 062003,
                4 => 062004,
                5 => 062005,
                6 => 062006,
                7 => 062007,
                8 => 062008,
                9 => 062009,
                10 => 062010,
                11 => 062011,
                12 => 062012,
                13 => 062013,
                14 => 062014,
                15 => 062015,
                16 => 062016,
                17 => 062017,
                18 => 062018,
                19 => 062019,
                20 => 062020,
                21 => 062021,
                22 => 062022,
                23 => 062023,
                24 => 062024,
                25 => 062025,
                26 => 062026,
                27 => 062027,
                28 => 062028,
                29 => 062029,
                30 => 062030,
                31 => 062031,
                32 => 062032,
                33 => 062033,
                34 => 062034,
                35 => 062035,
                36 => 062036,
                37 => 062037,
                38 => 062038,
                39 => 062039,
                40 => 062040,
                _ => FALLBACK_ICON,
            };
        }

        public static uint GetJobIconWithCorner(uint jobId)
        {
            return jobId switch
            {
                1 => 062101,
                2 => 062102,
                3 => 062103,
                4 => 062104,
                5 => 062105,
                6 => 062106,
                7 => 062107,
                8 => 062108,
                9 => 062109,
                10 => 062110,
                11 => 062111,
                12 => 062112,
                13 => 062113,
                14 => 062114,
                15 => 062115,
                16 => 062116,
                17 => 062117,
                18 => 062118,
                19 => 062119,
                20 => 062120,
                21 => 062121,
                22 => 062122,
                23 => 062123,
                24 => 062124,
                25 => 062125,
                26 => 062126,
                27 => 062127,
                28 => 062128,
                29 => 062129,
                30 => 062130,
                31 => 062131,
                32 => 062132,
                33 => 062133,
                34 => 062134,
                35 => 062135,
                36 => 062136,
                37 => 062137,
                38 => 062138,
                39 => 062139,
                40 => 062140,
                _ => FALLBACK_ICON,
            };
        }

        public static uint GetJobIconWithCornerSmall(uint jobId)
        {
            return jobId switch
            {
                1 => 062226,
                2 => 062227,
                3 => 062228,
                4 => 062229,
                5 => 062230,
                6 => 062231,
                7 => 062232,
                8 => 062233,
                9 => 062234,
                10 => 062235,
                11 => 062236,
                12 => 062237,
                13 => 062238,
                14 => 062239,
                15 => 062240,
                16 => 062241,
                17 => 062242,
                18 => 062243,
                19 => 062244,
                20 => 062245,
                21 => 062246,
                22 => 062247,
                23 => 062248,
                24 => 062249,
                25 => 062250,
                26 => 062251,
                27 => 062252,
                28 => 062253,
                29 => 062254,
                30 => 062255,
                31 => 062256,
                32 => 062257,
                33 => 062258,
                34 => 062259,
                35 => 062260,
                36 => 062261,
                37 => 062262,
                38 => 062263,
                39 => 062264,
                40 => 062265,
                _ => FALLBACK_ICON,
            };
        }

        public static int GetJobNextLevelExp(int level)
        {
            ExcelSheet<ParamGrow>? dbt = Plugin.DataManager.GetExcelSheet<ParamGrow>(ClientLanguage.English);
            ParamGrow? lumina = dbt?.GetRow((uint)level);
            return lumina?.ExpToNext ?? 0;
        }

        public static int GetRetainerJobMaxLevel(uint job, Character character)
        {
            if (character.Jobs == null) return 0;
            return job switch
            {
                0 => character.Jobs.Adventurer.Level,
                1 => character.Jobs.Gladiator.Level,
                2 => character.Jobs.Pugilist.Level,
                3 => character.Jobs.Marauder.Level,
                4 => character.Jobs.Lancer.Level,
                5 => character.Jobs.Archer.Level,
                6 => character.Jobs.Conjurer.Level,
                7 => character.Jobs.Thaumaturge.Level,
                8 => character.Jobs.Carpenter.Level,
                9 => character.Jobs.Blacksmith.Level,
                10 => character.Jobs.Armorer.Level,
                11 => character.Jobs.Goldsmith.Level,
                12 => character.Jobs.Leatherworker.Level,
                13 => character.Jobs.Weaver.Level,
                14 => character.Jobs.Alchemist.Level,
                15 => character.Jobs.Culinarian.Level,
                16 => character.Jobs.Miner.Level,
                17 => character.Jobs.Botanist.Level,
                18 => character.Jobs.Fisher.Level,
                19 => character.Jobs.Paladin.Level,
                20 => character.Jobs.Monk.Level,
                21 => character.Jobs.Warrior.Level,
                22 => character.Jobs.Dragoon.Level,
                23 => character.Jobs.Bard.Level,
                24 => character.Jobs.WhiteMage.Level,
                25 => character.Jobs.BlackMage.Level,
                26 => character.Jobs.Arcanist.Level,
                27 => character.Jobs.Summoner.Level,
                28 => character.Jobs.Scholar.Level,
                29 => character.Jobs.Rogue.Level,
                30 => character.Jobs.Ninja.Level,
                31 => character.Jobs.Machinist.Level,
                32 => character.Jobs.DarkKnight.Level,
                33 => character.Jobs.Astrologian.Level,
                34 => character.Jobs.Samurai.Level,
                35 => character.Jobs.RedMage.Level,
                36 => character.Jobs.BlueMage.Level,
                37 => character.Jobs.Gunbreaker.Level,
                38 => character.Jobs.Dancer.Level,
                39 => character.Jobs.Reaper.Level,
                40 => character.Jobs.Sage.Level,
                _ => 0
            };
        }

        public static void DrawLevelProgressBar(int exp, int nextExp, string jobFullname, bool active, bool isMax)
        {
            float progress = (float)exp / nextExp;
            if (active)
            {
                ImGui.ProgressBar(progress, new Vector2(150, 10), "");
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                ImGui.ProgressBar(progress, new Vector2(150, 10), "");
                ImGui.PopStyleVar();
            }

            if (ImGui.IsItemHovered())
            {
                if (active && !isMax)
                    ImGui.TextUnformatted($"{exp}/{nextExp}");
            }
        }

        public static uint GetRoleIcon(uint roleId)
        {
            return roleId switch
            {
                0 => 062581,
                1 => 062582,
                2 => 062584,
                3 => 062586,
                4 => 062587,
                5 => 062008,
                6 => 062016,
                _ => FALLBACK_ICON,
            };
        }

        public static string GetSlotName(ClientLanguage currentLocale, GlobalCache globalCache, short id)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            return id switch
            {
                0 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11524),
                1 => globalCache.AddonStorage.LoadAddonString(currentLocale, 12227),
                2 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11525),
                3 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11526),
                4 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11527),
                6 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11528),
                7 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11529),
                8 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11530),
                9 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11531),
                10 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11532),
                11 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11533),
                12 => globalCache.AddonStorage.LoadAddonString(currentLocale, 11534),
                13 => globalCache.AddonStorage.LoadAddonString(currentLocale, 12238),
                _ => string.Empty,
            };
        }

        public static void DrawGear(ClientLanguage currentLocale, ref GlobalCache globalCache,
            ref Dictionary<GearSlot, IDalamudTextureWrap?> defaultTextures, List<Gear> gears, uint job, int jobLevel,
            int middleWidth, int middleHeigth, bool retainer = false, int maxLevel = 0)
        {
            if (gears.Count == 0) return;
            using (var gearTableHeader = ImRaii.Table("###GearTableHeader", 3))
            {
                if (!gearTableHeader) return;
                ImGui.TableSetupColumn("###GearTableHeader#MHColumn", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn("###GearTableHeader#RoleIconNameColumn", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("###GearTableHeader#RoleIconNameColumn", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.MH,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11524), new Vector2(40, 40),
                    defaultTextures[GearSlot.MH], defaultTextures[GearSlot.EMPTY]);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 335)} {jobLevel}");
                using (var gearTableRoleIconNameTable = ImRaii.Table("###GearTable#RoleIconNameTable", 2))
                {
                    if (!gearTableRoleIconNameTable) return;
                    ImGui.TableSetupColumn("###GearTable#RoleColumn#RoleIcon", ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn("###GearTable#RoleColumn#RoleName", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawIcon(globalCache.IconStorage.LoadIcon(GetJobIcon(job)), new Vector2(40, 40));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{globalCache.JobStorage.GetName(currentLocale, job)}");
                }

                ImGui.TableSetColumnIndex(2);
                if (retainer)
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 2325).Replace("[", "").Replace("]", "")}{maxLevel}");
            }

            using var gearTable = ImRaii.Table("###GearTable", 3);
            if (!gearTable) return;
            ImGui.TableSetupColumn("###GearTable#LeftGearColumnHeader", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("###GearTable#CentralColumnHeader", ImGuiTableColumnFlags.WidthFixed, middleWidth);
            ImGui.TableSetupColumn("###GearTable#RightGearColumnHeader", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var gearTableLeftGearColumn = ImRaii.Table("###GearTable#LeftGearColumn", 1))
            {
                if (!gearTableLeftGearColumn) return;
                ImGui.TableSetupColumn("###GearTable#LeftGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawGearPiece(ref _globalCache, gears, GearSlot.HEAD, GetAddonString(11525), new Vector2(40, 40), 10032);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.HEAD,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11525), new Vector2(40, 40),
                    defaultTextures[GearSlot.HEAD], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawGearPiece(ref _globalCache, gears, GearSlot.BODY, GetAddonString(11526), new Vector2(40, 40), 10033);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.BODY,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11526), new Vector2(40, 40),
                    defaultTextures[GearSlot.BODY], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawGearPiece(ref _globalCache, gears, GearSlot.HANDS, GetAddonString(11527), new Vector2(40, 40), 10034);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.HANDS,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11527), new Vector2(40, 40),
                    defaultTextures[GearSlot.HANDS], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawGearPiece(ref _globalCache, gears, GearSlot.LEGS, GetAddonString(11528), new Vector2(40, 40), 10035);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.LEGS,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11528), new Vector2(40, 40),
                    defaultTextures[GearSlot.LEGS], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawGearPiece(ref _globalCache, gears, GearSlot.FEET, GetAddonString(11529), new Vector2(40, 40), 10035);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.FEET,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11529), new Vector2(40, 40),
                    defaultTextures[GearSlot.FEET], defaultTextures[GearSlot.EMPTY]);
            }

            ImGui.TableSetColumnIndex(1);
            DrawIcon(globalCache.IconStorage.LoadIcon(055396), new Vector2(middleWidth, middleHeigth));

            ImGui.TableSetColumnIndex(2);
            using var gearTableRightGearColumn = ImRaii.Table("###GearTable#RightGearColumn", 1);
            if (!gearTableRightGearColumn) return;
            ImGui.TableSetupColumn("###GearTable#RightGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.OH,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 12227), new Vector2(40, 40),
                defaultTextures[GearSlot.OH], defaultTextures[GearSlot.EMPTY]);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.EARS,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 11530), new Vector2(40, 40),
                defaultTextures[GearSlot.EARS], defaultTextures[GearSlot.EMPTY]);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.NECK,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 11531), new Vector2(40, 40),
                defaultTextures[GearSlot.NECK], defaultTextures[GearSlot.EMPTY]);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.WRISTS,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 11532), new Vector2(40, 40),
                defaultTextures[GearSlot.WRISTS], defaultTextures[GearSlot.EMPTY]);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.RIGHT_RING,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 11533), new Vector2(40, 40),
                defaultTextures[GearSlot.RIGHT_RING], defaultTextures[GearSlot.EMPTY]);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.LEFT_RING,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 11534), new Vector2(40, 40),
                defaultTextures[GearSlot.LEFT_RING], defaultTextures[GearSlot.EMPTY]);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.SOUL_CRYSTAL,
                globalCache.AddonStorage.LoadAddonString(currentLocale, 12238), new Vector2(40, 40),
                defaultTextures[GearSlot.SOUL_CRYSTAL], defaultTextures[GearSlot.EMPTY]);
        }

        public static (Vector2 uv0, Vector2 uv1) GetTextureCoordinate(Vector2 textureSize, int u, int v, int w, int h)
        {
            float u1 = (u + w) / textureSize.X;
            float v1 = (v + h) / textureSize.Y;
            Vector2 uv0 = new(u / textureSize.X, v / textureSize.Y);
            Vector2 uv1 = new(u1, v1);
            return (uv0, uv1);
        }

        public static void DrawRoleTexture(ref IDalamudTextureWrap texture, RoleIcon role, Vector2 size)
        {
            (Vector2 uv0, Vector2 uv1) = role switch
            {
                RoleIcon.Tank => GetTextureCoordinate(texture.Size, 100, 0, 40, 40),
                RoleIcon.Heal => GetTextureCoordinate(texture.Size, 140, 0, 40, 40),
                RoleIcon.Melee => GetTextureCoordinate(texture.Size, 180, 0, 40, 40),
                RoleIcon.Ranged => GetTextureCoordinate(texture.Size, 220, 0, 40, 40),
                RoleIcon.Caster => GetTextureCoordinate(texture.Size, 100, 40, 40, 40),
                RoleIcon.Crafter => GetTextureCoordinate(texture.Size, 140, 40, 40, 40),
                RoleIcon.Gatherer => GetTextureCoordinate(texture.Size, 180, 40, 40, 40),
                RoleIcon.DoWDoM => GetTextureCoordinate(texture.Size, 136, 80, 36, 36),
                RoleIcon.DoHDoL => GetTextureCoordinate(texture.Size, 208, 80, 36, 36),
                _ => GetTextureCoordinate(texture.Size, 0, 0, 0, 0)
            };
            ImGui.Image(texture.ImGuiHandle, size, uv0, uv1);
        }

        public static void DrawGearPiece(ClientLanguage currentLocale, ref GlobalCache globalCache, List<Gear> gear,
            GearSlot slot, string tooltip, Vector2 icon_size, /*uint fallback_icon*/
            IDalamudTextureWrap? fallbackTexture, IDalamudTextureWrap? emptySlot)
        {
            if (fallbackTexture is null || emptySlot is null) return;
            Gear? foundGear = gear.FirstOrDefault(g => g.Slot == (short)slot);
            //Plugin.Log.Debug($"{slot}, {GEAR.ItemId}");
            if (foundGear == null || foundGear.ItemId == 0)
            {
                System.Numerics.Vector2 p = ImGui.GetCursorPos();
                ImGui.Image(emptySlot.ImGuiHandle, new Vector2(42, 42));
                ImGui.SetCursorPos(new Vector2(p.X + 1, p.Y + 1));
                ImGui.Image(fallbackTexture.ImGuiHandle, new Vector2(40, 40));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(tooltip);
                    ImGui.EndTooltip();
                }
            }
            else
            {
                ItemItemLevel? i = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, foundGear.ItemId);
                if (i == null) return;
                DrawIcon(globalCache.IconStorage.LoadIcon(i.Item.Icon, foundGear.HQ), icon_size);
                if (ImGui.IsItemHovered())
                {
                    DrawGearTooltip(currentLocale, ref globalCache, foundGear, i);
                }
            }
        }

        public static void DrawGearTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Gear item,
            ItemItemLevel itm)
        {
            Item dbItem = itm.Item;
            ItemLevel? ilvl = itm.ItemLevel;
            if (ilvl == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                using var drawItemTooltipItemUnique = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}#Unique", 3);
                if (!drawItemTooltipItemUnique) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Unique#IsUntradable",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Unique#IsBinding",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(1);
                if (dbItem.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (dbItem.IsUntradable)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 495)}"); // Untradable
                }

                /*if (i.Is) No Binding value???
                        {
                            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(496)}");// Binding
                        }*/
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted("");
            }

            using (var drawItemTooltipItem = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}", 3))
            {
                if (!drawItemTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Icon", ImGuiTableColumnFlags.WidthFixed,
                    55);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Name", ImGuiTableColumnFlags.WidthFixed,
                    305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
                if (dbItem.IsGlamourous)
                {
                    Item? glamour = globalCache.ItemStorage.LoadItem(currentLocale, item.GlamourID);
                    if (glamour != null)
                    {
                        ImGui.TextUnformatted($"{(char)SeIconChar.Glamoured} {glamour.Name}");
                    }
                }

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{GetSlotName(currentLocale, globalCache, item.Slot)}");
            }

            using (var drawItemTooltipItemDefense = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}#Defense", 3))
            {
                if (!drawItemTooltipItemDefense) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Defense#Empty",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Defense#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Defense#Name",
                    ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3244)}"); // Defense
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3246)}"); // Magic Defense
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.DefensePhys}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.DefenseMag}");
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13775)} {ilvl.RowId}"); // Item Level
            ImGui.Separator();
            ImGui.TextUnformatted($"{GetClassJobCategoryFromId(currentLocale, dbItem.ClassJobCategory.Value?.RowId)}");
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1034)} {dbItem.LevelEquip}");
            ImGui.Separator();
            if (!dbItem.IsAdvancedMeldingPermitted)
            {
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 4655)}"); // Advanced Melding Forbidden
            }

            if (item.Stain > 0)
            {
                string dye = globalCache.StainStorage.LoadStain(currentLocale, item.Stain);
                if (!string.IsNullOrEmpty(dye))
                {
                    ImGui.TextUnformatted($"{dye}");
                }
            }

            ImGui.Separator();
            using (var drawItemTooltipItemBonuses = ImRaii.Table($"###DrawItemTooltip#Item_{item.ItemId}#Bonuses", 3))
            {
                if (!drawItemTooltipItemBonuses) return;
                //Todo: Conditional attributes since not every item will have the same
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Bonuses#StrengthCrit",
                    ImGuiTableColumnFlags.WidthFixed, 40);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Bonuses#Empty",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Bonuses#VitSkS",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3226)} +"); // Defense
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3227)} +"); // Vitality
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3241)} +"); // Critical Hit
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3249)} +"); // Skill Speed
            }

            if (dbItem.MateriaSlotCount > 0)
            {
                ImGui.Separator();
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 491)}"); // Materia
                for (int i = 0; i < dbItem.MateriaSlotCount; i++)
                {
                    ImGui.ColorButton($"##Item_{item.ItemId}#Materia#{i}", new Vector4(34, 169, 34, 1),
                        ImGuiColorEditFlags.None, new Vector2(16, 16));
                }
                //Plugin.Log.Debug($"Item materia: {item.Materia}");
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}"); // Crafting & Repairs
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 498)} : {(item.Condition / 300f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 499)} : {(item.Spiritbond / 100f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 500)} : {globalCache.JobStorage.GetName(currentLocale, dbItem.ClassJobRepair.Row)}"); //Repair Level
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 518)} : {GetItemRepairResource(currentLocale, dbItem.ItemRepair.Row)}"); //Materials
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 995)} : "); //Quick Repairs
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 993)} : "); //Materia Melding
            ImGui.TextUnformatted($"{GetExtractableString(currentLocale, globalCache, dbItem)}");
            ImGui.TextUnformatted($"{GetSellableString(currentLocale, globalCache, dbItem, item)}"); //Materia Melding
            if (item.CrafterContentID > 0)
                ImGui.TextUnformatted("Crafted");

            ImGui.EndTooltip();
        }

        public static void DrawItemTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Inventory item)
        {
            ItemItemLevel? itm = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, item.ItemId);
            Item? dbItem = itm?.Item;
            if (dbItem == null) return;
            ItemLevel? ilvl = itm?.ItemLevel;
            if (ilvl == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                using var drawItemTooltipItemUnique = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}#Unique", 3);
                if (!drawItemTooltipItemUnique) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Unique#IsUntradable",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Unique#IsBinding",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(1);
                if (dbItem.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (dbItem.IsUntradable)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 495)}"); // Untradable
                }

                /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(496)}");// Binding
                    }*/
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted("");
            }

            using (var drawItemTooltipItemNameIcon = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon", 2))
            {
                if (!drawItemTooltipItemNameIcon) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#NameIcon#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#NameIcon#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
            }

            using (var drawItemTooltipItemCategory =
                   ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}#Category", 3))
            {
                if (!drawItemTooltipItemCategory) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Category#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Category#Name",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Category#Name",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.ItemUICategory.Value?.Name}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{item.Quantity}/{dbItem.StackSize} (Total: {item.Quantity})");
                ImGui.TableSetColumnIndex(2);
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}"); // Crafting & Repairs

            ImGui.EndTooltip();
        }

        public static void DrawEventItemTooltip(ClientLanguage currentLocale, GlobalCache globalCache, Inventory item)
        {
            EventItem? dbItem = globalCache.ItemStorage.LoadEventItem(currentLocale, item.ItemId);
            if (dbItem == null) return;

            ImGui.BeginTooltip();

            using (var drawEventItemTooltipItemNameIcon =
                   ImRaii.Table($"###DrawEventItemTooltip#Item_{item.ItemId}#NameIcon", 2))
            {
                if (!drawEventItemTooltipItemNameIcon) return;
                ImGui.TableSetupColumn($"###DrawEventItemTooltip#Item_{item.ItemId}#NameIcon#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawEventItemTooltip#Item_{item.ItemId}#NameIcon#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
            }

            using (var drawEventItemTooltipItemCategory =
                   ImRaii.Table($"###DrawEventItemTooltip#Item_{item.ItemId}#Category", 3))
            {
                if (!drawEventItemTooltipItemCategory) return;
                ImGui.TableSetupColumn($"###DrawEventItemTooltip#Item_{item.ItemId}#Category#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###DrawEventItemTooltip#Item_{item.ItemId}#Category#Name",
                    ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###DrawEventItemTooltip#Item_{item.ItemId}#Category#Name",
                    ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TableSetColumnIndex(1);
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{item.Quantity}/99 (Total: {item.Quantity})");
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}"); // Crafting & Repairs

            ImGui.EndTooltip();
        }

        public static void DrawCrystalTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, uint itemId,
            int amount)
        {
            Item? dbItem = globalCache.ItemStorage.LoadItem(currentLocale, itemId);
            if (dbItem == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                using var drawCrystalTooltipItemUnique =
                    ImRaii.Table($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Unique", 3);
                if (!drawCrystalTooltipItemUnique) return;
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Unique#IsUnique",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Unique#IsUntradable",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"##DrawItem#DrawCrystalTooltipTooltip#Item_{dbItem.RowId}#Unique#IsBinding",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(1);
                if (dbItem.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (dbItem.IsUntradable)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 495)}"); // Untradable
                }

                /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(496)}");// Binding
                    }*/
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted("");
            }

            using (var drawCrystalTooltipItem = ImRaii.Table($"###DrawCrystalTooltip#Item_{dbItem.RowId}", 2))
            {
                if (!drawCrystalTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name}");
            }

            using (var drawCrystalTooltipItemCategory =
                   ImRaii.Table($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Category", 3))
            {
                if (!drawCrystalTooltipItemCategory) return;
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Category#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Category#Name",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.RowId}#Category#Empty",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.ItemUICategory.Value?.Name}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{amount}/99 (Total: {amount})");
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}"); // Crafting & Repairs

            ImGui.EndTooltip();
        }

        public static void DrawMountTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache,
            Models.Mount mount)
        {
            using var drawMountTooltip = ImRaii.Tooltip();
            if (!drawMountTooltip) return;
            using (var drawMountTooltipItem = ImRaii.Table($"###DrawMountTooltipItem#Mount_{mount.Id}", 2))
            {
                if (!drawMountTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawMountTooltipItem#Mount_{mount.Id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawMountTooltipItem#Mount_{mount.Id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(mount.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(mount.GermanName)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(mount.EnglishName)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(mount.FrenchName)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(mount.JapaneseName)}");
                        break;
                }
            }

            ImGui.Separator();
            if (mount.Transient is null) return;
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(mount.Transient.GermanTooltip)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(mount.Transient.EnglishTooltip)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(mount.Transient.FrenchTooltip)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(mount.Transient.JapaneseTooltip)}");
                    break;
            }
        }

        public static void DrawMinionTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Minion minion)
        {
            using var drawMinionTooltip = ImRaii.Tooltip();
            if (!drawMinionTooltip) return;
            using (var drawMinionTooltipTable = ImRaii.Table($"###DrawMinionTooltip#Minion_{minion.Id}", 2))
            {
                if (!drawMinionTooltipTable) return;
                ImGui.TableSetupColumn($"###DrawMinionTooltip#Minion_{minion.Id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawMinionTooltip#Minion_{minion.Id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(minion.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(minion.GermanName)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(minion.EnglishName)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(minion.FrenchName)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(minion.JapaneseName)}");
                        break;
                }
            }

            ImGui.Separator();
            if (minion.Transient is null) return;
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(minion.Transient.GermanTooltip)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(minion.Transient.EnglishTooltip)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(minion.Transient.FrenchTooltip)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(minion.Transient.JapaneseTooltip)}");
                    break;
            }
        }

        public static void DrawTTCTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache,
            Models.TripleTriadCard tripleTriadCard)
        {
            using var drawTripleTriadCardTooltip = ImRaii.Tooltip();
            if (!drawTripleTriadCardTooltip) return;
            using (var drawTripleTriadCardTooltipTable =
                   ImRaii.Table($"###DrawTripleTriadCardTooltip#TripleTriadCard_{tripleTriadCard.Id}", 2))
            {
                if (!drawTripleTriadCardTooltipTable) return;
                ImGui.TableSetupColumn($"###DrawTripleTriadCardTooltip#TripleTriadCard_{tripleTriadCard.Id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawTripleTriadCardTooltip#TripleTriadCard_{tripleTriadCard.Id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(tripleTriadCard.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.GermanName)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.EnglishName)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.FrenchName)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.JapaneseName)}");
                        break;
                }
            }

            ImGui.Separator();
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.GermanDescription)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.EnglishDescription)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.FrenchDescription)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(tripleTriadCard.JapaneseDescription)}");
                    break;
            }
        }

        public static void DrawEmoteTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache,
            Models.Emote emote)
        {
            using var drawTripleTriadCardTooltip = ImRaii.Tooltip();
            if (!drawTripleTriadCardTooltip) return;
            using var drawTripleTriadCardTooltipTable =
                ImRaii.Table($"###DrawEmoteTooltip#Emote_{emote.Id}", 2);
            if (!drawTripleTriadCardTooltipTable) return;
            ImGui.TableSetupColumn($"###DrawEmoteTooltip#Emote_{emote.Id}#Icon",
                ImGuiTableColumnFlags.WidthFixed, 55);
            ImGui.TableSetupColumn($"###DrawEmoteTooltip#Emote_{emote.Id}#Name",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawIcon(globalCache.IconStorage.LoadIcon(emote.Icon), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(emote.GermanName)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(emote.EnglishName)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(emote.FrenchName)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(emote.JapaneseName)}");
                    break;
            }

            if (emote.TextCommand is null) return;
            ImGui.Separator();
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(emote.TextCommand.GermanDescription)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(emote.TextCommand.EnglishDescription)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(emote.TextCommand.FrenchDescription)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(emote.TextCommand.JapaneseDescription)}");
                    break;
            }
        }

        public static void DrawBardingTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache,
            Models.Barding barding, uint Icon)
        {
            using var drawTripleTriadCardTooltip = ImRaii.Tooltip();
            if (!drawTripleTriadCardTooltip) return;
            using var drawTripleTriadCardTooltipTable =
                ImRaii.Table($"###DrawBardingTooltip#Barding_{barding.Id}", 2);
            if (!drawTripleTriadCardTooltipTable) return;
            ImGui.TableSetupColumn($"###DrawBardingTooltip#Barding_{barding.Id}#Icon",
                ImGuiTableColumnFlags.WidthFixed, 55);
            ImGui.TableSetupColumn($"###DrawBardingTooltip#Barding_{barding.Id}#Name",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawIcon(globalCache.IconStorage.LoadIcon(Icon), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(barding.GermanName)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(barding.EnglishName)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(barding.FrenchName)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(barding.JapaneseName)}");
                    break;
            }
        }

        public static void DrawFramerKitTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache,
            FramerKit framerKit)
        {
            using var drawTripleTriadCardTooltip = ImRaii.Tooltip();
            if (!drawTripleTriadCardTooltip) return;
            using var drawTripleTriadCardTooltipTable =
                ImRaii.Table($"###DrawFramerKitTooltip#FramerKit_{framerKit.Id}", 2);
            if (!drawTripleTriadCardTooltipTable) return;
            ImGui.TableSetupColumn($"###DrawFramerKitTooltip#FramerKit_{framerKit.Id}#Icon",
                ImGuiTableColumnFlags.WidthFixed, 55);
            ImGui.TableSetupColumn($"###DrawFramerKitTooltip#FramerKit_{framerKit.Id}#Name",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawIcon(globalCache.IconStorage.LoadIcon(framerKit.Icon), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(framerKit.GermanName)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(framerKit.EnglishName)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(framerKit.FrenchName)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(framerKit.JapaneseName)}");
                    break;
            }
        }


        public static void DrawOrchestrionRollTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, OrchestrionRoll orchestrionRoll)
        {
            using var drawTripleTriadCardTooltip = ImRaii.Tooltip();
            if (!drawTripleTriadCardTooltip) return;
            using var drawTripleTriadCardTooltipTable =
                ImRaii.Table($"###DrawOrchestrionRollTooltip#OrchestrionRoll_{orchestrionRoll.Id}", 2);
            if (!drawTripleTriadCardTooltipTable) return;
            ImGui.TableSetupColumn($"###DrawOrchestrionRollTooltip#OrchestrionRoll_{orchestrionRoll.Id}#Icon",
                ImGuiTableColumnFlags.WidthFixed, 55);
            ImGui.TableSetupColumn($"###DrawOrchestrionRollTooltip#OrchestrionRoll_{orchestrionRoll.Id}#Name",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawIcon(globalCache.IconStorage.LoadIcon(orchestrionRoll.Icon), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(orchestrionRoll.GermanName)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(orchestrionRoll.EnglishName)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(orchestrionRoll.FrenchName)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(orchestrionRoll.JapaneseName)}");
                    break;
            }
        }

    private static string GetExtractableString(ClientLanguage currentLocale, GlobalCache globalCache, Item item)
        {
            string str = globalCache.AddonStorage.LoadAddonString(currentLocale, 1361);
            Plugin.Log.Debug($"extract str: {str} => item desynth {item.Desynth}");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(1),0))>Y<Else/>N</If>", (item.AdditionalData) ? "Y" : "N");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(2),0))>Y<Else/>N</If>", (item.IsGlamourous) ? "Y" : "N");
            str = str.Replace("Extractable: YN", "Extractable: ");
            str = str.Replace("Projectable: YN", (item.IsGlamourous) ? "Projectable: Y" : "Projectable: N");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(3),0))><Value>IntegerParameter(4)</Value>.00<Else/>N</If>", (item.Desynth == 0)? "N" : "Y");
            str = str.Replace(".00N", (item.Desynth == 0)? "N" : "Y");
            return str;
        }
        private static string GetSellableString(ClientLanguage currentLocale, GlobalCache globalCache, Item item, Gear gear)
        {
            double price = item.PriceLow * (gear.HQ ? 1.1f : 1.0);
            //Plugin.Log.Debug($"PriceLow : {item.PriceLow}, PriceMid: {item.PriceMid}, stackValue {price}");
            string str = globalCache.AddonStorage.LoadAddonString(currentLocale, 484);
            //Plugin.Log.Debug($"price str: {str}");
            str = item.PriceLow == 0 ? globalCache.AddonStorage.LoadAddonString(currentLocale, 503).Replace(" <If(IntegerParameter(1))><Else/> ", "") :
                //str = str.Replace("<Format(IntegerParameter(1),FF022C)/>", price.ToString());
                str.Replace(",", Math.Ceiling(price).ToString("N0"));
            //str = str.Replace("<If(IntegerParameter(2))><Else/>　Market Prohibited</If>", "");
            //str = str.Replace("　Market Prohibited", "");
            return str;
        }

        // ./SimpleTweaksPlugin/Tweaks/Tooltips/MateriaStats.cs
        /*public static void Materia(ushort id, byte grade, Lumina.Excel.GeneratedSheets.ItemLevel itemLevel)
        {
            var baseParams = new Dictionary<uint, Lumina.Excel.GeneratedSheets.BaseParam>();
            var baseParamDeltas = new Dictionary<uint, int>();
            var baseParamOriginal = new Dictionary<uint, int>();
            var baseParamLimits = new Dictionary<uint, int>();
            for (var i = 0; i < 5; i++, id++, grade++)
            {
                if (*level >= 10) continue;
                var materia = GetMateria(id);
                if (materia == null) continue;
                if (materia.BaseParam.Row == 0) continue;
                if (materia.BaseParam.Value == null) continue;
                if (!baseParamDeltas.ContainsKey(materia.BaseParam.Row))
                {
                    var bp = GetBaseParam(materia.BaseParam.Row);
                    if (bp == null) continue;
                    baseParams.Add(materia.BaseParam.Row, bp);
                    baseParamDeltas.Add(materia.BaseParam.Row, materia.Value[grade]);
                    baseParamOriginal.Add(materia.BaseParam.Row, 0);
                    baseParamLimits.Add(materia.BaseParam.Row, (int)Math.Round(itemLevel.BaseParam[materia.BaseParam.Row] * (bp.EquipSlotCategoryPct[item.EquipSlotCategory.Row] / 1000f), MidpointRounding.AwayFromZero));
                    continue;
                }
                baseParamDeltas[materia.BaseParam.Row] += materia.Value[grade];
            }
        }*/

        public static string GetAddonString(ClientLanguage currentLocale, int id)
        {
            ExcelSheet<Addon>? da = Plugin.DataManager.GetExcelSheet<Addon>(currentLocale);
            if (da == null)
            {
                return string.Empty;
            }

            Addon? lumina = da.GetRow((uint)id);
            return lumina != null ? lumina.Text : string.Empty;
        }
        
        public static Companion? GetMinion(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Companion>? dc = Plugin.DataManager.GetExcelSheet<Companion>(currentLocale);
            Companion? lumina = dc?.GetRow(id);
            return lumina;
        }
        public static CompanionTransient? GetCompanionTransient(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<CompanionTransient>? dct = Plugin.DataManager.GetExcelSheet<CompanionTransient>(currentLocale);
            CompanionTransient? lumina = dct?.GetRow(id);
            return lumina;
        }
        public static List<Minion>? GetAllMinions(ClientLanguage currentLocale)
        {
            List<Minion> returnedMinionsIds = [];
            ExcelSheet<Companion>? dm = Plugin.DataManager.GetExcelSheet<Companion>(currentLocale);
            using IEnumerator<Companion>? minionEnumerator = dm?.GetEnumerator();
            if (minionEnumerator is null) return null;
            while (minionEnumerator.MoveNext())
            {
                Companion minion = minionEnumerator.Current;
                if (string.IsNullOrEmpty(minion.Singular)) continue;
                if (minion.Icon == 0) continue;
                Minion m = new() { Id = minion.RowId, Icon = minion.Icon, Transient = new Transient() };
                CompanionTransient? ct = GetCompanionTransient(currentLocale, minion.RowId);
                if (ct is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = minion.Singular;
                        m.Transient.GermanDescription = ct.Description;
                        m.Transient.GermanDescriptionEnhanced = ct.DescriptionEnhanced;
                        m.Transient.GermanTooltip = ct.Tooltip;
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = minion.Singular;
                        m.Transient.EnglishDescription = ct.Description;
                        m.Transient.EnglishDescriptionEnhanced = ct.DescriptionEnhanced;
                        m.Transient.EnglishTooltip = ct.Tooltip;
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = minion.Singular;
                        m.Transient.FrenchDescription = ct.Description;
                        m.Transient.FrenchDescriptionEnhanced = ct.DescriptionEnhanced;
                        m.Transient.FrenchTooltip = ct.Tooltip;
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = minion.Singular;
                        m.Transient.JapaneseDescription = ct.Description;
                        m.Transient.JapaneseDescriptionEnhanced = ct.DescriptionEnhanced;
                        m.Transient.JapaneseTooltip = ct.Tooltip;
                        break;
                }

                returnedMinionsIds.Add(m);
            }

            return returnedMinionsIds;
        }

        public static Mount? GetMount(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Mount>? dm = Plugin.DataManager.GetExcelSheet<Mount>(currentLocale);
            Mount? lumina = dm?.GetRow(id);
            return lumina;
        }
        public static MountTransient? GetMountTransient(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<MountTransient>? dmt = Plugin.DataManager.GetExcelSheet<MountTransient>(currentLocale);
            MountTransient? lumina = dmt?.GetRow(id);
            return lumina;
        }
        public static List<Models.Mount>? GetAllMounts(ClientLanguage currentLocale)
        {
            List<Models.Mount> returnedMountsIds = [];
            ExcelSheet<Mount>? dm = Plugin.DataManager.GetExcelSheet<Mount>(currentLocale);
            using IEnumerator<Mount>? mountEnumerator = dm?.GetEnumerator();
            if (mountEnumerator is null) return null;
            while (mountEnumerator.MoveNext())
            {
                Mount mount = mountEnumerator.Current;
                if (string.IsNullOrEmpty(mount.Singular)) continue;
                if (mount.Icon == 0) continue;
                Models.Mount m = new() { Id = mount.RowId, Icon = mount.Icon, Transient = new Transient() };
                MountTransient? mt = GetMountTransient(currentLocale, mount.RowId);
                if (mt is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = mount.Singular;
                        m.Transient.GermanDescription = mt.Description;
                        m.Transient.GermanDescriptionEnhanced = mt.DescriptionEnhanced;
                        m.Transient.GermanTooltip = mt.Tooltip;
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = mount.Singular;
                        m.Transient.EnglishDescription = mt.Description;
                        m.Transient.EnglishDescriptionEnhanced = mt.DescriptionEnhanced;
                        m.Transient.EnglishTooltip = mt.Tooltip;
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = mount.Singular;
                        m.Transient.FrenchDescription = mt.Description;
                        m.Transient.FrenchDescriptionEnhanced = mt.DescriptionEnhanced;
                        m.Transient.FrenchTooltip = mt.Tooltip;
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = mount.Singular;
                        m.Transient.JapaneseDescription = mt.Description;
                        m.Transient.JapaneseDescriptionEnhanced = mt.DescriptionEnhanced;
                        m.Transient.JapaneseTooltip = mt.Tooltip;
                        break;
                }

                returnedMountsIds.Add(m);
            }

            return returnedMountsIds;
        }

        public static TripleTriadCard? GetTripleTriadCard(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<TripleTriadCard>? dttc = Plugin.DataManager.GetExcelSheet<TripleTriadCard>(currentLocale);
            TripleTriadCard? lumina = dttc?.GetRow(id);
            return lumina;
        }
        public static List<Models.TripleTriadCard>? GetAllTripletriadcards(ClientLanguage currentLocale)
        {
            List<Models.TripleTriadCard> returnedTripletriadcardsIds = [];
            ExcelSheet<TripleTriadCard>? dm = Plugin.DataManager.GetExcelSheet<TripleTriadCard>(currentLocale);
            using IEnumerator<TripleTriadCard>? tripletriadcardEnumerator = dm?.GetEnumerator();
            if (tripletriadcardEnumerator is null) return null;
            while (tripletriadcardEnumerator.MoveNext())
            {
                TripleTriadCard tripletriadcard = tripletriadcardEnumerator.Current;
                if (string.IsNullOrEmpty(tripletriadcard.Name) || tripletriadcard.Name == "0") continue;
                Models.TripleTriadCard ttc = new() { Id = tripletriadcard.RowId };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ttc.GermanName = tripletriadcard.Name;
                        ttc.GermanDescription = tripletriadcard.Description;
                        break;
                    case ClientLanguage.English:
                        ttc.EnglishName = tripletriadcard.Name;
                        ttc.EnglishDescription = tripletriadcard.Description;
                        break;
                    case ClientLanguage.French:
                        ttc.FrenchName = tripletriadcard.Name;
                        ttc.FrenchDescription = tripletriadcard.Description;
                        break;
                    case ClientLanguage.Japanese:
                        ttc.JapaneseName = tripletriadcard.Name;
                        ttc.JapaneseDescription = tripletriadcard.Description;
                        break;
                }

                ttc.Icon = tripletriadcard.RowId + 88000;

                returnedTripletriadcardsIds.Add(ttc);
            }

            return returnedTripletriadcardsIds;
        }

        public static Emote? GetEmote(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Emote>? dttc = Plugin.DataManager.GetExcelSheet<Emote>(currentLocale);
            Emote? lumina = dttc?.GetRow(id);
            return lumina;
        }
        public static TextCommand? GetTextCommand(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<TextCommand>? dtc = Plugin.DataManager.GetExcelSheet<TextCommand>(currentLocale);
            TextCommand? lumina = dtc?.GetRow(id);
            return lumina;
        }
        public static List<Models.Emote>? GetAllEmotes(ClientLanguage currentLocale)
        {
            List<Models.Emote> returnedEmotesIds = [];
            ExcelSheet<Emote>? dm = Plugin.DataManager.GetExcelSheet<Emote>(currentLocale);
            using IEnumerator<Emote>? emoteEnumerator = dm?.GetEnumerator();
            if (emoteEnumerator is null) return null;
            while (emoteEnumerator.MoveNext())
            {
                Emote emote = emoteEnumerator.Current;
                if (string.IsNullOrEmpty(emote.Name) || emote.Name == "0") continue;
                Models.Emote e = new() { Id = emote.RowId, TextCommand = new Models.TextCommand() };
                TextCommand? tc = GetTextCommand(currentLocale, emote.TextCommand.Row);
                if (tc is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        e.GermanName = emote.Name;
                        e.TextCommand.GermanCommand = tc.Command;
                        e.TextCommand.GermanShortCommand = tc.ShortCommand;
                        e.TextCommand.GermanDescription= tc.Description;
                        e.TextCommand.GermanAlias = tc.Alias;
                        e.TextCommand.GermanShortAlias = tc.ShortAlias;
                        break;
                    case ClientLanguage.English:
                        e.EnglishName = emote.Name;
                        e.TextCommand.EnglishCommand = tc.Command;
                        e.TextCommand.EnglishShortCommand = tc.ShortCommand;
                        e.TextCommand.EnglishDescription = tc.Description;
                        e.TextCommand.EnglishAlias = tc.Alias;
                        e.TextCommand.EnglishShortAlias = tc.ShortAlias;
                        break;
                    case ClientLanguage.French:
                        e.FrenchName = emote.Name;
                        e.TextCommand.FrenchCommand = tc.Command;
                        e.TextCommand.FrenchShortCommand = tc.ShortCommand;
                        e.TextCommand.FrenchDescription = tc.Description;
                        e.TextCommand.FrenchAlias = tc.Alias;
                        e.TextCommand.FrenchShortAlias = tc.ShortAlias;
                        break;
                    case ClientLanguage.Japanese:
                        e.JapaneseName = emote.Name;
                        e.TextCommand.JapaneseCommand = tc.Command;
                        e.TextCommand.JapaneseShortCommand = tc.ShortCommand;
                        e.TextCommand.JapaneseDescription = tc.Description;
                        e.TextCommand.JapaneseAlias = tc.Alias;
                        e.TextCommand.JapaneseShortAlias = tc.ShortAlias;
                        break;
                }

                e.Icon = emote.Icon;

                returnedEmotesIds.Add(e);
            }

            return returnedEmotesIds;
        }

        public static BuddyEquip? GetBarding(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BuddyEquip>? dbe = Plugin.DataManager.GetExcelSheet<BuddyEquip>(currentLocale);
            BuddyEquip? lumina = dbe?.GetRow(id);
            return lumina;
        }
        public static List<Barding>? GetAllBardings(ClientLanguage currentLocale)
        {
            List<Barding> returnedBardingsIds = [];
            ExcelSheet<BuddyEquip>? dbe = Plugin.DataManager.GetExcelSheet<BuddyEquip>(currentLocale);
            using IEnumerator<BuddyEquip>? bardingEnumerator = dbe?.GetEnumerator();
            if (bardingEnumerator is null) return null;
            while (bardingEnumerator.MoveNext())
            {
                BuddyEquip barding = bardingEnumerator.Current;
                if (string.IsNullOrEmpty(barding.Name) || barding.RowId == 0) continue;
                Barding b = new() { Id = barding.RowId };
                Item? item = GetItemFromName(currentLocale, barding.Name);

                b.Icon = item?.Icon ?? barding.IconHead;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        b.GermanName = barding.Name;
                        break;
                    case ClientLanguage.English:
                        b.EnglishName = barding.Name;
                        break;
                    case ClientLanguage.French:
                        b.FrenchName = barding.Name;
                        break;
                    case ClientLanguage.Japanese:
                        b.JapaneseName = barding.Name;
                        break;
                }

                b.IconHead = barding.IconHead;
                b.IconBody = barding.IconBody;
                b.IconLegs = barding.IconLegs;

                returnedBardingsIds.Add(b);
            }

            return returnedBardingsIds;
        }

        public static Orchestrion? GetOrchestrionRoll(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Orchestrion>? dor = Plugin.DataManager.GetExcelSheet<Orchestrion>(currentLocale);
            Orchestrion? lumina = dor?.GetRow(id);
            return lumina;
        }
        public static List<OrchestrionRoll>? GetAllOrchestrionRolls(ClientLanguage currentLocale)
        {
            List<OrchestrionRoll> returnedOrchestrionRollIds = [];
            ExcelSheet<Orchestrion>? dor = Plugin.DataManager.GetExcelSheet<Orchestrion>(currentLocale);
            using IEnumerator<Orchestrion>? orchestrionRollEnumerator = dor?.GetEnumerator();
            if (orchestrionRollEnumerator is null) return null;
            while (orchestrionRollEnumerator.MoveNext())
            {
                Orchestrion orchestrionRoll = orchestrionRollEnumerator.Current;
                if (string.IsNullOrEmpty(orchestrionRoll.Name) || orchestrionRoll.RowId == 0) continue;
                OrchestrionRoll orchestrion = new() { Id = orchestrionRoll.RowId };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        orchestrion.GermanName = orchestrionRoll.Name;
                        break;
                    case ClientLanguage.English:
                        orchestrion.EnglishName = orchestrionRoll.Name;
                        break;
                    case ClientLanguage.French:
                        orchestrion.FrenchName = orchestrionRoll.Name;
                        break;
                    case ClientLanguage.Japanese:
                        orchestrion.JapaneseName = orchestrionRoll.Name;
                        break;
                }

                returnedOrchestrionRollIds.Add(orchestrion);
            }

            return returnedOrchestrionRollIds;
        }


        public static string GetTribalNameFromId(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BeastTribe>? dbt = Plugin.DataManager.GetExcelSheet<BeastTribe>(currentLocale);
            BeastTribe? lumina = dbt?.GetRow(id);
            return lumina != null ? lumina.Name : string.Empty;
        }
        public static string GetTribalCurrencyFromId(ClientLanguage currentLocale, uint id)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<BeastTribe>? dbt = Plugin.DataManager.GetExcelSheet<BeastTribe>(currentLocale);
            BeastTribe? lumina = dbt?.GetRow(id);
            return lumina != null ? lumina.Name : string.Empty;
        }
        public static string Capitalize(string str)
        {
            if(str.Length == 0) return str;;
            return char.ToUpper(str[0]) + str[1..];
        }

        public static string CapitalizeSnakeCase(string str)
        {
            List<string> items = [];
            items.AddRange(str.Split("_").Select(Capitalize));
            return string.Join("_", items);
        }
        
        public static string CapitalizeCurrency(string str)
        {
            if (str is "MGP" or "MGF")
            {
                return str;
            }
            List<string> items = [];
            items.AddRange(str.Split("_").Select(item => Capitalize(item.ToLower())));
            return string.Join("_", items);
        }

        public static bool IsQuestCompleted(int questId)
        {
            //Plugin.Log.Debug($"IsQuestCompleted questId: {questId}");

            return QuestManager.IsQuestComplete((uint)questId);
        }

        public static string GetClassJobCategoryFromId(ClientLanguage currentLocale, uint? id)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<ClassJobCategory>? djc = Plugin.DataManager.GetExcelSheet<ClassJobCategory>(currentLocale);
            if (djc == null)
            {
                return string.Empty;
            }

            if (id == null)
            {
                return string.Empty;
            }

            ClassJobCategory? lumina = djc.GetRow(id.Value);
            return lumina != null ? lumina.Name : string.Empty;
        }
        
        public static ClassJob? GetClassJobFromId(uint? id, ClientLanguage clientLanguage)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<ClassJob>? djc = Plugin.DataManager.GetExcelSheet<ClassJob>(clientLanguage);

            if (id == null)
            {
                return null;
            }

            ClassJob? lumina = djc?.GetRow(id.Value);
            return lumina;
        }

        public static string GetItemRepairResource(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<ItemRepairResource>? dirr = Plugin.DataManager.GetExcelSheet<ItemRepairResource>(currentLocale);
            if (dirr == null)
            {
                return string.Empty;
            }

            ItemRepairResource? lumina = dirr.GetRow(id);
            LazyRow<Item>? itm = lumina?.Item;
            Item? v = itm?.Value;
            return v != null ? v.Name : string.Empty;
        }

        // ReSharper disable once InconsistentNaming
        public static string GetFCTag(ClientLanguage currentLocale, GlobalCache globalCache, Character localPlayer)
        {
            // ReSharper disable once InconsistentNaming
            string FCTag = string.Empty;
            if ((string.IsNullOrEmpty(localPlayer.CurrentWorld) || string.IsNullOrEmpty(localPlayer.CurrentDatacenter) || string.IsNullOrEmpty(localPlayer.CurrentWorld)) || (localPlayer.CurrentWorld == localPlayer.HomeWorld && localPlayer.CurrentRegion == localPlayer.Region))
            {
                FCTag = localPlayer.FCTag;
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion == localPlayer.Region)
            {
                FCTag = globalCache.AddonStorage.LoadAddonString(currentLocale, 12541);
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion != localPlayer.Region)
            {
                FCTag = globalCache.AddonStorage.LoadAddonString(currentLocale, 12625);
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion != localPlayer.Region)
            {
                FCTag = globalCache.AddonStorage.LoadAddonString(currentLocale, 12627);
            }
            //Plugin.Log.Debug($"localPlayerRegion : {localPlayerRegion}");
            //Plugin.Log.Debug($"localPlayer.CurrentRegion : {localPlayer.CurrentRegion}");
            return FCTag;
        }
        public static string GetCrystalName(ClientLanguage currentLocale, GlobalCache globalCache, uint i)
        {
            uint crystalId = i + 2;
            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, crystalId);
            return (itm is null) ? string.Empty : itm.Name;
        }

        public static RetainerTask? GetRetainerTask(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<RetainerTask>? drt = Plugin.DataManager.GetExcelSheet<RetainerTask>(currentLocale);
            RetainerTask? lumina = drt?.GetRow(id);
            return lumina;
        }
        public static RetainerTaskNormal? GetRetainerTaskNormal(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<RetainerTaskNormal>? drtn = Plugin.DataManager.GetExcelSheet<RetainerTaskNormal>(currentLocale);
            RetainerTaskNormal? lumina = drtn?.GetRow(id);
            return lumina;
        }
        public static RetainerTaskRandom? GetRetainerTaskRandom(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<RetainerTaskRandom>? drtr = Plugin.DataManager.GetExcelSheet<RetainerTaskRandom>(currentLocale);
            RetainerTaskRandom? lumina = drtr?.GetRow(id);
            return lumina;
        }

        public static string UnixTimeStampToDateTime(long lastOnline)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(lastOnline).ToLocalTime();
            return dateTime.ToString(CultureInfo.InvariantCulture);
        }
        public static void DrawIcon(IDalamudTextureWrap? icon, Vector2 iconSize)
        {
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, iconSize);
            }
        }
        public static void DrawIcon(IDalamudTextureWrap? icon, Vector2 iconSize, Vector4 alpha)
        {
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, alpha);
            }
        }

        public static long GetLastPlayTimeUpdateDiff(long lastOnline)
           {
               long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
               long diff = now - lastOnline;
               long diffDays = Math.Abs(diff / 86400);
               return diffDays;
           }

        public static string GetLastOnlineFormatted(long lastOnline /*, string firstname*/)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long diff = now - lastOnline;
            long diffDays = Math.Abs(diff / 86400);
            long diffHours = Math.Abs(diff / 3600);
            long diffMins = Math.Abs(diff / 60);

            string? time;

            //Plugin.Log.Debug($"{firstname} diffDays: {diffDays}, diffHours: {diffHours}, diffMins: {diffMins}");

            if (lastOnline == 0)
            {
                time = "now";
            }
            else
                switch (diffDays)
                {
                    case > 365:
                        time = "Over a year ago";
                        break;
                    case < 365 and > 30:
                        {
                            double tdiff = diffDays / 30.0;
                            time = $"{Math.Floor(tdiff)} months ago";
                            break;
                        }
                    case < 30 and > 1:
                        time = $"{diffDays} days ago";
                        break;
                    case 1:
                        time = "A day ago";
                        break;
                    case 0 when diffHours is < 24 and > 1:
                        time = $"{diffHours} hours ago";
                        break;
                    /*else if (diffDays == 0 && diffHours == 1 && diffMins > 0)
                {
                    time = string.Format("An hour and {diffMins} mins")
                }*/
                    //else if (diffDays == 0 && diffHours == 1 && diffMins == 0)
                    case 0 when diffHours == 1:
                        time = "One hour ago";
                        break;
                    case 0 when diffHours == 0 && diffMins is < 60 and > 1:
                        time = $"{diffMins} minutes ago";
                        break;
                    case 0 when diffHours == 0 && diffMins == 1 && diff == 0:
                        time = "One minutes ago";
                        break;
                    case 0 when diffHours == 0 && diffMins <= 1:
                        time = "A few seconds ago";
                        break;
                    default:
                        time = "Unknown";
                        break;
                }

            return time;
        }
    }
}

public static class StringExt
{
    public static string? Truncate(this string? value, int maxLength)
    {
        return value?.Length > maxLength
            ? value[..maxLength]
            : value;
    }
}