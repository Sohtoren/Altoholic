﻿using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;
using ClassJob = Lumina.Excel.GeneratedSheets.ClassJob;
using Stain = Lumina.Excel.GeneratedSheets.Stain;

namespace Altoholic
{
    internal class Utils
    {
        // ReSharper disable once InconsistentNaming
        private const uint FALLBACK_ICON = 055396;

        /*public static void DrawItemIcon(Vector2 iconSize, bool hq, uint itemId)
        {
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(GetLocale());

            Item? lumina = ditm?.GetRow(itemId);
            //Plugin.Log.Debug($"lumina : ${lumina}");
            if (lumina == null)
            {
                return;
            }

            //Plugin.Log.Debug($"icon path : {lumina.Icon}");
            uint iconId = (lumina.Icon == 0) ? FALLBACK_ICON : lumina.Icon;
            IDalamudTextureWrap? icon = Plugin.TextureProvider.GetIcon(iconId, hq ? IconFlags.ItemHighQuality : IconFlags.None);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, iconSize);
            }
        }*/
        /*public static void DrawEventItemIcon(Vector2 iconSize, bool hq, uint itemId)
        {
            ExcelSheet<EventItem>? deitm = Plugin.DataManager.GetExcelSheet<EventItem>(GetLocale());
            EventItem? lumina = deitm?.GetRow(itemId);
            //Plugin.Log.Debug($"lumina : ${lumina}");
            if (lumina == null)
            {
                return;
            }

            //Plugin.Log.Debug($"icon path : {lumina.Icon}");
            uint iconId = (lumina.Icon == 0) ? FALLBACK_ICON : lumina.Icon;
            IDalamudTextureWrap? icon = Plugin.TextureProvider.GetIcon(iconId, hq ? IconFlags.ItemHighQuality : IconFlags.None);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, iconSize);
            }
        }*/

        /*public static void DrawIcon(Vector2 iconSize, bool hq, uint iconId)
        {
            if (iconId == 0) iconId = FALLBACK_ICON;
            //Plugin.Log.Debug($"DrawIcon icon_id : {icon_id}");
            IDalamudTextureWrap? icon = Plugin.TextureProvider.GetIcon(iconId, hq ? IconFlags.ItemHighQuality : IconFlags.None);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, iconSize);
            }
        }
        public static void DrawIcon(Vector2 iconSize, bool hq, uint iconId, Vector2 alpha)
        {
            if (iconId == 0) iconId = FALLBACK_ICON;
            //Plugin.Log.Debug($"DrawIcon icon_id : {icon_id}");
            IDalamudTextureWrap? icon = Plugin.TextureProvider.GetIcon(iconId, hq ? IconFlags.ItemHighQuality : IconFlags.None);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, iconSize, new Vector2(0, 0), alpha);
            }
        }*/

        //public static void DrawIconFromTexture(Vector2 size, Vector4 area)

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
        /*public static string GetRace(int gender, uint race)
        {
            ExcelSheet<Race>? dr = Plugin.DataManager.GetExcelSheet<Race>(GetLocale());
            Race? lumina = dr?.GetRow(race);
            if (lumina != null)
                return gender == 0 ? lumina.Masculine : lumina.Feminine;
            return string.Empty;
            /*if (gender == 0)
                return Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Race>(GetLocale()).GetRow((uint)race)!.Masculine;
            else
                return Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Race>(GetLocale()).GetRow((uint)race)!.Feminine;*/
            /*return race switch
            {
                1 => "Hyur",
                2 => "Elezen",
                3 => "Lalafell",
                4 => "Miqo'te",
                5 => "Roegadyn",
                6 => "Au Ra",
                7 => "Hrothgar",
                8 => "Viera",
                _ => string.Empty,
            };*/
        //}

        public static string GetTribe(ClientLanguage currentLocale, int gender, uint tribe)
        {
            ExcelSheet<Tribe>? dt = Plugin.DataManager.GetExcelSheet<Tribe>(currentLocale);
            Tribe? lumina = dt?.GetRow(tribe);
            if (lumina != null)
                return gender == 0 ? lumina.Masculine : lumina.Feminine;
            return string.Empty;
        }

        /*public static string GetTribe(int gender, uint tribe)
        {
            ExcelSheet<Tribe>? dt = Plugin.DataManager.GetExcelSheet<Tribe>(GetLocale());
            Tribe? lumina = dt?.GetRow(tribe);
            if (lumina != null)
                return gender == 0 ? lumina.Masculine : lumina.Feminine;
            return string.Empty;
            /*if (gender == 0)
                 return Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Tribe>(GetLocale()).GetRow((uint)tribe)!.Masculine;
             else
                 return Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Tribe>(GetLocale()).GetRow((uint)tribe)!.Feminine;*/
            /*return tribe switch
            {
                1 => "Midlander",
                2 => "Highlander",
                3 => "Wildwood",
                4 => "Duskwight",
                5 => "Plainsfolk",
                6 => "Dunesfolk",
                7 => "Seeker of the Sun",
                8 => "Keeper of the Moon",
                9 => "Sea Wolf",
                10 => "Hellsguard",
                11 => "Raen",
                12 => "Xaela",
                13 => "Helions",
                14 => "The Lost",
                15 => "Rava",
                16 => "Veena",
                _ => string.Empty,
            };*/
        //}

        public static string GetTown(ClientLanguage currentLocale, int town)
        {
            ExcelSheet<Town>? dt = Plugin.DataManager.GetExcelSheet<Town>(currentLocale);
            Town? lumina = dt?.GetRow((uint)town);
            return lumina != null ? lumina.Name : string.Empty;
        }
        /*public static string GetTown(int town)
        {
            ExcelSheet<Town>? dt = Plugin.DataManager.GetExcelSheet<Town>(GetLocale());
            Town? lumina = dt?.GetRow((uint)town);
            return lumina != null ? lumina.Name : string.Empty;
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Town>(GetLocale()).GetRow((uint)town)!;
            return lumina.Name;*/
            /*return town switch
            {
                1 => "Limsa Lominsa",
                2 => "Gridania",
                3 => "Ul'dah",
                _ => string.Empty,
            };*/
        //}

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
        /*public static string GetGuardian(int guardian)
        {
            ExcelSheet<GuardianDeity>? dg = Plugin.DataManager.GetExcelSheet<GuardianDeity>(GetLocale());
            GuardianDeity? lumina = dg?.GetRow((uint)guardian);
            return lumina != null ? lumina.Name : string.Empty;
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GuardianDeity>(GetLocale()).GetRow((uint)guardian)!;
            return lumina.Name;*/
            /*return guardian switch
            {
                1 => "Halone, the Fury",
                2 => "Menphina, the Lover",
                3 => "Thaliak, the Scholar",
                4 => "Nymeia, the Spinner",
                5 => "Llymlaen, the Navigator",
                6 => "Oschon, the Wanderer",
                7 => "Byregot, the Builder",
                8 => "Rhalgr, the Destroyer",
                9 => "Azeyma, the Warden",
                10 => "Nald'thal, the Traders",
                11 => "Nophica, the Matron",
                12 => "Althyk, the Keeper",
                _ => string.Empty,
            };*/
        //}

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

        /*public static string GetGrandCompany(int id)
        {
            ExcelSheet<GrandCompany>? dgc = Plugin.DataManager.GetExcelSheet<GrandCompany>(GetLocale());
            GrandCompany? lumina = dgc?.GetRow((uint)id);
            return lumina != null ? lumina.Name : string.Empty;
        }*/
        public static string GetGrandCompany(ClientLanguage currentLocale, int id)
        {
            ExcelSheet<GrandCompany>? dgc = Plugin.DataManager.GetExcelSheet<GrandCompany>(currentLocale);
            GrandCompany? lumina = dgc?.GetRow((uint)id);
            return lumina != null ? lumina.Name : string.Empty;
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompany>(GetLocale()).GetRow((uint)id);
            return lumina.Name;*/
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
                            ExcelSheet<GCRankLimsaMaleText>? dgcrlmt = Plugin.DataManager.GetExcelSheet<GCRankLimsaMaleText>(currentLocale);
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
                            ExcelSheet<GCRankLimsaFemaleText>? dgcrlft = Plugin.DataManager.GetExcelSheet<GCRankLimsaFemaleText>(currentLocale);
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
                            ExcelSheet<GCRankUldahMaleText>? dgcrlmt = Plugin.DataManager.GetExcelSheet<GCRankUldahMaleText>(currentLocale);
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
                            ExcelSheet<GCRankUldahFemaleText>? dgcrlft = Plugin.DataManager.GetExcelSheet<GCRankUldahFemaleText>(currentLocale);
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
                            ExcelSheet<GCRankGridaniaMaleText>? dgcrlmt = Plugin.DataManager.GetExcelSheet<GCRankGridaniaMaleText>(currentLocale);
                            GCRankGridaniaMaleText? lumina = dgcrlmt?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.NameRank;
                        }
                        else
                        {
                            ExcelSheet<GCRankGridaniaFemaleText>? dgcrlft = Plugin.DataManager.GetExcelSheet<GCRankGridaniaFemaleText>(currentLocale);
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
        /*public static string GetItemNameFromId(ClientLanguage currentLocale, uint id)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            Item? lumina = ditm?.GetRow(id);
            return lumina != null ? lumina.Name : string.Empty;
        }*/

        public static IEnumerable<Item>? GetItemsFromName(ClientLanguage currentLocale, string name)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            IEnumerable<Item>? items = ditm?.Where(i => i.Name.RawString.Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return items;
        }

        public static ItemLevel? GetItemLevelFromId(uint id)
        {
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            ExcelSheet<ItemLevel>? dilvl = Plugin.DataManager.GetExcelSheet<ItemLevel>(ClientLanguage.English);
            ItemLevel? lumina = dilvl?.GetRow(id);
            return lumina;
        }
        /*public static Stain? GetStainFromId(uint id)
        {
            Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<Stain>? ds = Plugin.DataManager.GetExcelSheet<Stain>(GetLocale());
            Stain? lumina = ds?.GetRow(id);
            return lumina;
        }*/
        public static Stain? GetStainFromId(uint id, ClientLanguage clientLanguage)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<Stain>? ds = Plugin.DataManager.GetExcelSheet<Stain>(clientLanguage);
            Stain? lumina = ds?.GetRow(id);
            return lumina;
        }

        /*public static string GetJobNameFromId(uint id, bool abbreviation = false)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");

            Lumina.Excel.GeneratedSheets.ClassJob? lumina = GetClassJobFromId(id);
            if (lumina != null)
            {
                return (abbreviation) ? lumina.Abbreviation : Capitalize(lumina.Name);
            }

            return string.Empty;
        }*/

        /*public static string GetJobShortName(string fullname)
        {
            return fullname switch
            {
                "Adventurer" => "ADV",
                "Gladiator" => "GLA",
                "Pugilist" => "PGL",
                "Marauder" => "MRD",
                "Lancer" => "LNC",
                "Archer" => "ARC",
                "Conjurer" => "CNJ",
                "Thaumaturge" => "THM",
                "Carpenter" => "CRP",
                "Blacksmith" => "BSM",
                "Armorer" => "ARM",
                "Goldsmith" => "GSM",
                "Leatherworker" => "LTW",
                "Weaver" => "WVR",
                "Alchemist" => "ALC",
                "Culinarian" => "CUL",
                "Miner" => "MIN",
                "Botanist" => "BTN",
                "Fisher" => "FSH",
                "Paladin" => "PLD",
                "Monk" => "MNK",
                "Warrior" => "WAR",
                "Dragoon" => "DRG",
                "Bard" => "BRD",
                "WhiteMage" => "WHM",
                "BlackMage" => "BLM",
                "Arcanist" => "ACN",
                "Summoner" => "SMN",
                "Scholar" => "SCH",
                "Rogue" => "ROG",
                "Ninja" => "NIN",
                "Machinist" => "MCH",
                "DarkKnight" => "DRK",
                "Astrologian" => "AST",
                "Samurai" => "SAM",
                "RedMage" => "RDM",
                "BlueMage" => "BLU",
                "Gunbreaker" => "GNB",
                "Dancer" => "DNC",
                "Reaper" => "RPR",
                "Sage" => "SGE",
                _ => string.Empty,
            };
        }      */

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
        /*public static string GetRoleName(ClientLanguage GetLocale(), int role_id)
        {
            return role_id switch
            {
                0 => GetLocale() switch
                {
                    ClientLanguage.German => "Verteidiger",
                    ClientLanguage.English => "Tank",
                    ClientLanguage.French => "Tank",
                    ClientLanguage.Japanese => "Tank",
                    _ => string.Empty,
                },
                1 => GetLocale() switch
                {
                    ClientLanguage.German => "Heiler",
                    ClientLanguage.English => "Healer",
                    ClientLanguage.French => "Soigneur",
                    ClientLanguage.Japanese => "Healer",
                    _ => string.Empty,
                },
                2 => GetLocale() switch
                {
                    ClientLanguage.German => "Nahkampfangreifer",
                    ClientLanguage.English => "Melee DPS",
                    ClientLanguage.French => "DPS de mêlée",
                    ClientLanguage.Japanese => "Melee DPS",
                    _ => string.Empty,
                },
                3 => GetLocale() switch
                {
                    ClientLanguage.German => "Phys. Fernkämpfer",
                    ClientLanguage.English => "Physical Ranged DPS",
                    ClientLanguage.French => "DPS physique à distance",
                    ClientLanguage.Japanese => "Physical Ranged DPS",
                    _ => string.Empty,
                },
                4 => GetLocale() switch
                {
                    ClientLanguage.German => "Mag. Fernkämpfer",
                    ClientLanguage.English => "Magical Ranged DPS",
                    ClientLanguage.French => "DPS magique à distance",
                    ClientLanguage.Japanese => "Magical Ranged DPS",
                    _ => string.Empty,
                },
                5 => GetLocale() switch
                {
                    ClientLanguage.German => "Disziplinen der Handwerker",
                    ClientLanguage.English => "Disciplines of the Hand",
                    ClientLanguage.French => "Disciple de la main",
                    ClientLanguage.Japanese => "クラフター",
                    _ => string.Empty,
                },
                6 => GetLocale() switch
                {
                    ClientLanguage.German => "Disziplinen der Sammler",
                    ClientLanguage.English => "Disciplines of the Land",
                    ClientLanguage.French => "Disciple de la terre",
                    ClientLanguage.Japanese => "ギャザラー",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }*/

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

        //public static void DrawGear(List<Gear> gears, uint job, int jobLevel, int middleWidth, int middleHeigth, bool retainer = false, int maxLevel = 0)
        public static void DrawGear(ClientLanguage currentLocale, ref GlobalCache globalCache, ref Dictionary<GearSlot, IDalamudTextureWrap?> defaultTextures, List<Gear> gears, uint job, int jobLevel, int middleWidth, int middleHeigth, bool retainer = false, int maxLevel = 0)
        {
            if (gears.Count == 0) return;
            if (ImGui.BeginTable("###GearTableHeader", 3))
            {
                ImGui.TableSetupColumn("###GearTableHeader#MHColumn", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn("###GearTableHeader#RoleIconNameColumn", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("###GearTableHeader#RoleIconNameColumn", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawGearPiece(ref _globalCache, gears, GearSlot.MH, GetAddonString(11524), new Vector2(40, 40), 13775);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.MH, globalCache.AddonStorage.LoadAddonString(currentLocale, 11524), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.MH], defaultTextures[GearSlot.EMPTY]);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 335)} {jobLevel}");
                if (ImGui.BeginTable("###GearTable#RoleIconNameTable", 2))
                {
                    ImGui.TableSetupColumn("###GearTable#RoleColumn#RoleIcon", ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn("###GearTable#RoleColumn#RoleName", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawIcon(new Vector2(40, 40), false, GetJobIcon(job));
                    DrawIcon_test(globalCache.IconStorage.LoadIcon(GetJobIcon(job)), new Vector2(40, 40));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{globalCache.JobStorage.GetName(currentLocale, job)}");

                    ImGui.EndTable();
                }
                ImGui.TableSetColumnIndex(2);
                if(retainer)
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 2325).Replace("[", "").Replace("]", "")}{maxLevel}");

                ImGui.EndTable();
            }

            if (ImGui.BeginTable("###GearTable", 3))
            {
                ImGui.TableSetupColumn("###GearTable#LeftGearColumnHeader", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn("###GearTable#CentralColumnHeader", ImGuiTableColumnFlags.WidthFixed, middleWidth);
                ImGui.TableSetupColumn("###GearTable#RightGearColumnHeader", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginTable("###GearTable#LeftGearColumn", 1))
                {
                    ImGui.TableSetupColumn("###GearTable#LeftGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.HEAD, GetAddonString(11525), new Vector2(40, 40), 10032);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.HEAD, globalCache.AddonStorage.LoadAddonString(currentLocale, 11525), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.HEAD], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.BODY, GetAddonString(11526), new Vector2(40, 40), 10033);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.BODY, globalCache.AddonStorage.LoadAddonString(currentLocale, 11526), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.BODY], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.HANDS, GetAddonString(11527), new Vector2(40, 40), 10034);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.HANDS, globalCache.AddonStorage.LoadAddonString(currentLocale, 11527), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.HANDS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.LEGS, GetAddonString(11528), new Vector2(40, 40), 10035);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.LEGS, globalCache.AddonStorage.LoadAddonString(currentLocale, 11528), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.LEGS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.FEET, GetAddonString(11529), new Vector2(40, 40), 10035);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.FEET, globalCache.AddonStorage.LoadAddonString(currentLocale, 11529), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.FEET], defaultTextures[GearSlot.EMPTY]);
                    ImGui.EndTable();
                }

                ImGui.TableSetColumnIndex(1);
                //DrawIcon(new Vector2(middleWidth, middleHeigth), false, 055396);
                DrawIcon_test(globalCache.IconStorage.LoadIcon(055396), new Vector2(middleWidth, middleHeigth));

                ImGui.TableSetColumnIndex(2);
                if (ImGui.BeginTable("###GearTable#RightGearColumn", 1))
                {
                    ImGui.TableSetupColumn("###GearTable#RightGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.OH, GetAddonString(12227), new Vector2(40, 40), 30067);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.OH, globalCache.AddonStorage.LoadAddonString(currentLocale, 12227), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.OH], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.EARS, GetAddonString(11530), new Vector2(40, 40), 9293);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.EARS, globalCache.AddonStorage.LoadAddonString(currentLocale, 11530), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.EARS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.NECK, GetAddonString(11531), new Vector2(40, 40), 9292);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.NECK, globalCache.AddonStorage.LoadAddonString(currentLocale, 11531), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.NECK], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.WRISTS, GetAddonString(11532), new Vector2(40, 40), 9294);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.WRISTS, globalCache.AddonStorage.LoadAddonString(currentLocale, 11532), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.WRISTS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.RIGHT_RING, GetAddonString(11533), new Vector2(40, 40), 9295);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.RIGHT_RING, globalCache.AddonStorage.LoadAddonString(currentLocale, 11533), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.RIGHT_RING], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.LEFT_RING, GetAddonString(11534), new Vector2(40, 40), 9295);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.LEFT_RING, globalCache.AddonStorage.LoadAddonString(currentLocale, 11534), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.LEFT_RING], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.SOUL_CRYSTAL, GetAddonString(12238), new Vector2(40, 40), 55396);//Todo: Find Soul Crystal empty icon
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.SOUL_CRYSTAL, globalCache.AddonStorage.LoadAddonString(currentLocale, 12238), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.SOUL_CRYSTAL], defaultTextures[GearSlot.EMPTY]);
                    ImGui.EndTable();
                }
                ImGui.EndTable();
            }
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
        /*public static void DrawdefaultTextures[GearSlot.EMPTY](ref IDalamudTextureWrap texture, GearSlot part)
        {
            Vector2 size = new(64, 64);
            var (uv0, uv1) = part switch
            {
                GearSlot.MH => GetTextureCoordinate(texture.Size, 0, 144, 64, 64),
                GearSlot.HEAD => GetTextureCoordinate(texture.Size, 128, 144, 64, 64),
                GearSlot.BODY => GetTextureCoordinate(texture.Size, 192, 144, 64, 64),
                GearSlot.HANDS => GetTextureCoordinate(texture.Size, 256, 144, 64, 64),
                GearSlot.BELT => GetTextureCoordinate(texture.Size, 320, 144, 64, 64),
                GearSlot.LEGS => GetTextureCoordinate(texture.Size, 384, 144, 64, 64),
                GearSlot.FEET => GetTextureCoordinate(texture.Size, 0, 208, 64, 64),
                GearSlot.OH => GetTextureCoordinate(texture.Size, 64, 144, 64, 64),
                GearSlot.EARS => GetTextureCoordinate(texture.Size, 64, 208, 64, 64),
                GearSlot.NECK => GetTextureCoordinate(texture.Size, 128, 208, 64, 64),
                GearSlot.WRISTS => GetTextureCoordinate(texture.Size, 192, 208, 64, 64),
                GearSlot.LEFT_RING => GetTextureCoordinate(texture.Size, 256, 208, 64, 64),
                GearSlot.RIGHT_RING => GetTextureCoordinate(texture.Size, 256, 208, 64, 64),
                GearSlot.SOUL_CRYSTAL => GetTextureCoordinate(texture.Size, 384, 208, 64, 64),
                _ => GetTextureCoordinate(texture.Size, 0,0,0,0)
            };
            ImGui.Image(texture.ImGuiHandle, size, uv0, uv1, Vector4.One with { W = 0.33f });
        }
        */
        public static void DrawGearPiece(ClientLanguage currentLocale, ref GlobalCache globalCache, List<Gear> gear, GearSlot slot, string tooltip, Vector2 icon_size, /*uint fallback_icon*/ /*ref*/ IDalamudTextureWrap? fallbackTexture, IDalamudTextureWrap? emptySlot)
        {
            if (fallbackTexture is null || emptySlot is null) return;
            Gear? foundGear = gear.FirstOrDefault(g => g.Slot == (short)slot);
            //Plugin.Log.Debug($"{slot}, {GEAR.ItemId}");
            if (foundGear == null || foundGear.ItemId == 0)
            {
                //DrawItemIcon(icon_size, false, fallback_icon);
                /*var i = _itemStorage.LoadItem(fallback_icon);
                if (i == null) return;
                DrawIcon_test(_globalCache.IconStorage.LoadIcon(i.Icon), icon_size);*/
                //ImGui.Image(fallbackTexture.ImGuiHandle, icon_size, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.33f });
                System.Numerics.Vector2 p = ImGui.GetCursorPos();
                ImGui.Image(emptySlot.ImGuiHandle, new Vector2(42, 42));
                ImGui.SetCursorPos(new Vector2(p.X + 1, p.Y+1));
                ImGui.Image(fallbackTexture.ImGuiHandle, new Vector2(40, 40));
                //DrawdefaultTextures[GearSlot.EMPTY](ref fallbackTexture, slot);
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(tooltip);
                    ImGui.EndTooltip();
                }
            }
            else
            {
                //DrawItemIcon(icon_size, GEAR.HQ, GEAR.ItemId);
                ItemItemLevel? i = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, foundGear.ItemId);
                if(i == null) return;
                DrawIcon_test(globalCache.IconStorage.LoadIcon(i.Item.Icon, foundGear.HQ), icon_size);
                if (ImGui.IsItemHovered())
                {
                    DrawGearTooltip(currentLocale, ref globalCache, foundGear, i);
                }
            }
        }
        public static void DrawGearTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Gear item, ItemItemLevel itm)
        {
            //Lumina.Excel.GeneratedSheets.Item? dbItem = GetItemFromId(item.ItemId);
            Item? dbItem = itm.Item;
            if (dbItem == null) return;
            //ItemLevel? ilvl = GetItemLevelFromId(dbItem.LevelItem.Row);
            ItemLevel? ilvl = itm.ItemLevel;
            if (ilvl == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Unique", 3))
                {
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUntradable", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsBinding", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted("");
                    ImGui.TableSetColumnIndex(1);
                    if (dbItem.IsUnique)
                    {
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}");// Unique
                    }
                    if (dbItem.IsUntradable)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 495)}");// Untradable
                    }
                    /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(496)}");// Binding
                    }*/
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted("");
                    ImGui.EndTable();
                }
            }
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}", 3))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Icon", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Name", ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawItemIcon(new Vector2(40, 40), item.HQ, item.ItemId);
                DrawIcon_test(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40,40));
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
                ImGui.EndTable();
            }
            if(ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Defense", 3))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Defense#Empty", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Defense#Icon", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Defense#Name", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3244)}");// Defense
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3246)}");// Magic Defense
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.DefensePhys}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.DefenseMag}");
                ImGui.EndTable();
            }
            ImGui.Separator();
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13775)} {ilvl.RowId}");// Item Level
            ImGui.Separator();
            ImGui.TextUnformatted($"{GetClassJobCategoryFromId(currentLocale, dbItem.ClassJobCategory.Value?.RowId)}");
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1034)} {dbItem.LevelEquip}");
            ImGui.Separator();
            if (!dbItem.IsAdvancedMeldingPermitted)
            { 
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 4655)}"); // Advanced Melding Forbidden
            }
            if (item.Stain > 0)
            {
                //Stain? dye = GetStainFromId(item.Stain);
                string dye = globalCache.StainStorage.LoadStain(currentLocale, item.Stain);
                if (!string.IsNullOrEmpty(dye))
                {
                    ImGui.TextUnformatted($"{dye}");
                }
            }
            ImGui.Separator();
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Bonuses", 3))
            {
                //Todo: Conditional attributes since not every item will have the same
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Bonuses#StrengthCrit", ImGuiTableColumnFlags.WidthFixed, 40);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Bonuses#Empty", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Bonuses#VitSkS", ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3226)} +");// Defense
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3227)} +");// Vitality
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3241)} +");// Critical Hit
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 3249)} +");// Skill Speed
                ImGui.EndTable();
            }
            if (dbItem.MateriaSlotCount > 0) {
                ImGui.Separator();
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 491)}");// Materia
                for (int i = 0;i < dbItem.MateriaSlotCount;i++)
                {
                    ImGui.ColorButton($"##Item_{item.ItemId}#Materia#{i}", new Vector4(34, 169, 34, 1), ImGuiColorEditFlags.None, new Vector2(16,16));
                }
                //Plugin.Log.Debug($"Item materia: {item.Materia}");
            }
            ImGui.Separator();
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}");// Crafting & Repairs
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 498)} : {(item.Condition / 300f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 499)} : {(item.Spiritbond / 100f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 500)} : {globalCache.JobStorage.GetName(currentLocale, dbItem.ClassJobRepair.Row)}");//Repair Level
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 518)} : {GetItemRepairResource(currentLocale, dbItem.ItemRepair.Row)}");//Materials
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 995)} : ");//Quick Repairs
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 993)} : ");//Materia Melding
            ImGui.TextUnformatted($"{GetExtractableString(currentLocale, globalCache, dbItem)}");
            ImGui.TextUnformatted($"{GetSellableString(currentLocale, globalCache, dbItem, item)}");//Materia Melding
            if(item.CrafterContentID > 0)
                ImGui.TextUnformatted("Crafted");

            ImGui.EndTooltip();
        }
        
        public static void DrawItemTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Inventory item)
        {
            ItemItemLevel? itm = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, item.ItemId);
            Item? dbItem = itm?.Item;
            if (dbItem == null) return;
            //Lumina.Excel.GeneratedSheets.ItemLevel? ilvl = GetItemLevelFromId(dbItem.LevelItem.Row);
            ItemLevel? ilvl = itm?.ItemLevel;
            if (ilvl == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Unique", 3))
                {
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUntradable", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsBinding", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted("");
                    ImGui.TableSetColumnIndex(1);
                    if (dbItem.IsUnique)
                    {
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}");// Unique
                    }
                    if (dbItem.IsUntradable)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 495)}");// Untradable
                    }
                    /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(496)}");// Binding
                    }*/
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted("");
                    ImGui.EndTable();
                }
            }
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon", 2))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon#Icon", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon#Name", ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawItemIcon(new Vector2(40, 40), item.HQ, item.ItemId);
                DrawIcon_test(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
                ImGui.EndTable();
            }
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Category", 3))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Category#Icon", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Category#Name", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Category#Name", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.ItemUICategory.Value?.Name}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{item.Quantity}/{dbItem.StackSize} (Total: {item.Quantity})");
                ImGui.TableSetColumnIndex(2);
                ImGui.EndTable();
            }

            ImGui.Separator();
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}");// Crafting & Repairs

            ImGui.EndTooltip();
        }
        
        public static void DrawEventItemTooltip(ClientLanguage currentLocale, GlobalCache globalCache, Inventory item)
        {
            //Lumina.Excel.GeneratedSheets.EventItem? dbItem = GetEventItemFromId(item.ItemId);
            EventItem? dbItem = globalCache.ItemStorage.LoadEventItem(currentLocale, item.ItemId);
            if (dbItem == null) return;

            ImGui.BeginTooltip();

            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon", 2))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon#Icon", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon#Name", ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawItemIcon(new Vector2(40, 40), item.HQ, item.ItemId);
                DrawIcon_test(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40,40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
                ImGui.EndTable();
            }
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Category", 3))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Category#Icon", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Category#Name", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Category#Name", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TableSetColumnIndex(1);
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{item.Quantity}/99 (Total: {item.Quantity})");
                ImGui.EndTable();
            }

            ImGui.Separator();
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}");// Crafting & Repairs

            ImGui.EndTooltip();
        }
        
        public static void DrawCrystalTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, uint itemId, int amount)
        {
            //Lumina.Excel.GeneratedSheets.Item? dbItem = GetItemFromId(itemId);
            Item? dbItem = globalCache.ItemStorage.LoadItem(currentLocale, itemId);
            if (dbItem == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                if (ImGui.BeginTable($"##DrawItemTooltip#Item_{dbItem.RowId}#Unique", 3))
                {
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Unique#IsUnique", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Unique#IsUntradable", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Unique#IsBinding", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted("");
                    ImGui.TableSetColumnIndex(1);
                    if (dbItem.IsUnique)
                    {
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}");// Unique
                    }
                    if (dbItem.IsUntradable)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 495)}");// Untradable
                    }
                    /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(496)}");// Binding
                    }*/
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted("");
                    ImGui.EndTable();
                }
            }
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{dbItem.RowId}", 2))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Icon", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Name", ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //DrawItemIcon(new Vector2(40, 40), false, itemId);
                DrawIcon_test(globalCache.IconStorage.LoadIcon(dbItem.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name}");
                ImGui.EndTable();
            }
            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{dbItem.RowId}", 3))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Icon", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Name", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{dbItem.RowId}#Name", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.ItemUICategory.Value?.Name}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{amount}/99 (Total: {amount})");
                ImGui.EndTable();
            }

            ImGui.Separator();
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}");// Crafting & Repairs

            ImGui.EndTooltip();
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

        /*public static string GetAddonString(int id)
        {
            ExcelSheet<Addon>? da = Plugin.DataManager.GetExcelSheet<Addon>(GetLocale());
            if (da == null)
            {
                return string.Empty;
            }

            Addon? lumina = da.GetRow((uint)id);
            return lumina != null ? lumina.Text : string.Empty;
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

        /*public static string GetCurrencyTitle(ClientLanguage GetLocale(), int id)
        {
            return id switch
            {
                0 => GetLocale() switch
                {
                    ClientLanguage.German => "Allgemein",
                    ClientLanguage.English => "Common",
                    ClientLanguage.French => "Commun",
                    ClientLanguage.Japanese => "共通",
                    _ => string.Empty,
                },
                1 => GetLocale() switch
                {
                    ClientLanguage.German => "Kampf",
                    ClientLanguage.English => "Battle",
                    ClientLanguage.French => "Combats",
                    ClientLanguage.Japanese => "戦闘",
                    _ => string.Empty,
                },
                2 => GetLocale() switch
                {
                    ClientLanguage.German => "Sonstiges",
                    ClientLanguage.English => "Other",
                    ClientLanguage.French => "Autres",
                    ClientLanguage.Japanese => "非戦闘",
                    _ => string.Empty,
                },
                3 => GetLocale() switch
                {
                    ClientLanguage.German => "Stammesvölker",
                    ClientLanguage.English => "Tribal",
                    ClientLanguage.French => "Tribus",
                    ClientLanguage.Japanese => "友好部族",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }*/

        /*public static string GetCommonTitle(ClientLanguage GetLocale(), int id)
        {
            return id switch
            {
                0 => GetLocale() switch
                {
                    ClientLanguage.German => "Gil",
                    ClientLanguage.English => "Gil",
                    ClientLanguage.French => "Gils",
                    ClientLanguage.Japanese => "Gil",
                    _ => string.Empty,
                },
                1 => GetLocale() switch
                {
                    ClientLanguage.German => "Staatstaler",
                    ClientLanguage.English => "Company Seals",
                    ClientLanguage.French => "Sceaux de compagnie",
                    ClientLanguage.Japanese => "軍票",
                    _ => string.Empty,
                },
                2 => GetLocale() switch
                {
                    ClientLanguage.German => "Wertmarken",
                    ClientLanguage.English => "Ventures",
                    ClientLanguage.French => "Jetons de tâche",
                    ClientLanguage.Japanese => "Venture Scrip",
                    _ => string.Empty,
                },
                3 => GetLocale() switch
                {
                    ClientLanguage.German => "Manderville Gold Saucer-Punkte",
                    ClientLanguage.English => "Manderville Gold Saucer Points",
                    ClientLanguage.French => "Points du Gold Saucer (PGS)",
                    ClientLanguage.Japanese => "Manderville Gold Saucer Points",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }*/

        /*public static string GetBattleTitle(ClientLanguage GetLocale(), int id)
        {
            return id switch
            {
                0 => GetLocale() switch
                {
                    ClientLanguage.German => "Allagische Steine",
                    ClientLanguage.English => "Allagan Tomestones",
                    ClientLanguage.French => "Mémoquartz allagois",
                    ClientLanguage.Japanese => "Allagan Tomestones",
                    _ => string.Empty,
                },
                1 => GetLocale() switch
                {
                    ClientLanguage.German => "Zurücksetzung",
                    ClientLanguage.English => "Reset in",
                    ClientLanguage.French => "Remise à zéro",
                    ClientLanguage.Japanese => "リセット日時",
                    _ => string.Empty,
                },
                2 => GetLocale() switch
                {
                    ClientLanguage.German => "Gesamt",
                    ClientLanguage.English => "Total",
                    ClientLanguage.French => "Total",
                    ClientLanguage.Japanese => "Total",
                    _ => string.Empty,
                },
                3 => GetLocale() switch
                {
                    ClientLanguage.German => "Woche",
                    ClientLanguage.English => "This Week",
                    ClientLanguage.French => "Hebdomadaire",
                    ClientLanguage.Japanese => "Weekly",
                    _ => string.Empty,
                },
                4 => GetLocale() switch
                {
                    ClientLanguage.German => "Eingestellt",
                    ClientLanguage.English => "Discontinued",
                    ClientLanguage.French => "Devise(s) désuette(s)",
                    ClientLanguage.Japanese => "Discontinued",
                    _ => string.Empty,
                },
                5 => GetLocale() switch
                {
                    ClientLanguage.German => "PvP",
                    ClientLanguage.English => "PvP",
                    ClientLanguage.French => "Récompenses JcJ",
                    ClientLanguage.Japanese => "Honor",
                    _ => string.Empty,
                },
                6 => GetLocale() switch
                {
                    ClientLanguage.German => "Wertmarken",
                    ClientLanguage.English => "Ventures",
                    ClientLanguage.French => "Jetons de tâche",
                    ClientLanguage.Japanese => "Venture Scrip",
                    _ => string.Empty,
                },
                7 => GetLocale() switch
                {
                    ClientLanguage.German => "FATE",
                    ClientLanguage.English => "FATE",
                    ClientLanguage.French => "ALÉA",
                    ClientLanguage.Japanese => "F.A.T.E.",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }
        public static string GetTribalTitle(ClientLanguage GetLocale())
        {
            return GetLocale() switch
            {
                ClientLanguage.German => "Währungen",
                ClientLanguage.English => "Tribal Currency",
                ClientLanguage.French => "Monnaies tribales",
                ClientLanguage.Japanese => "Tribal Currency",
                _ => string.Empty,
            };
        }*/


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
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>(GetLocale()).GetRow(id)!;
            return lumina.CurrencyItem.Value.Name;*/
        }
        public static string Capitalize(string str)
        {
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
        public static ClassJob? GetClassJobFromId(ClientLanguage currentLocale, uint? id)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<ClassJob>? djc = Plugin.DataManager.GetExcelSheet<ClassJob>(currentLocale);
            if (id == null)
            {
                return null;
            }

            ClassJob? lumina = djc?.GetRow(id.Value);
            return lumina;
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



        ////////////////////////Test
        public static IDalamudTextureWrap? LoadIcon(uint iconId, bool hq = false)
        {
            if (iconId == 0) iconId = FALLBACK_ICON;
            //Plugin.Log.Debug($"DrawIcon icon_id : {icon_id}");
            return Plugin.TextureProvider.GetIcon(iconId, hq ? IconFlags.ItemHighQuality : IconFlags.None);
        }
        public static void DrawIcon_test(IDalamudTextureWrap? icon, Vector2 iconSize)
        {
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, iconSize);
            }
        }
        public static void DrawIcon_test(IDalamudTextureWrap? icon, Vector2 iconSize, Vector4 alpha)
        {
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, alpha);
            }
        }

        /*public static void DrawIcon_test(Vector2 icon_size, bool hq, uint icon_id)
        {
            var icon = LoadIcon(icon_id, hq);
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, icon_size);
            }
        }*/

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