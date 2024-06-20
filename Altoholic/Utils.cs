using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace Altoholic
{
    internal class Utils
    {
        private const uint FALLBACK_ICON = 055396;

        public static void DrawItemIcon(Vector2 icon_size, bool hq, uint item_id)
        {
            if (Plugin.TextureProvider is null || Plugin.DataManager is null || Plugin.Log is null) return;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Item>? ditm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(GetLocale());
            if (ditm != null)
            {
                Lumina.Excel.GeneratedSheets.Item? lumina = ditm.GetRow(item_id);
                //Plugin.Log.Debug($"lumina : ${lumina}");
                if (lumina != null)
                {
                    //Plugin.Log.Debug($"icon path : {lumina.Icon}");
                    uint icon_id = (lumina.Icon == 0) ? (uint)FALLBACK_ICON : lumina.Icon;
                    var icon = Plugin.TextureProvider.GetIcon(icon_id, hq ? IconFlags.ItemHighQuality : IconFlags.None);
                    if (icon != null)
                    {
                        //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                        ImGui.Image(icon.ImGuiHandle, icon_size);
                    }
                }
            }
        }
        public static void DrawEventItemIcon(Vector2 icon_size, bool hq, uint item_id)
        {
            if (Plugin.TextureProvider is null || Plugin.DataManager is null || Plugin.Log is null) return;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.EventItem>? deitm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.EventItem>(GetLocale());
            if (deitm != null)
            {
                Lumina.Excel.GeneratedSheets.EventItem? lumina = deitm.GetRow(item_id);
                //Plugin.Log.Debug($"lumina : ${lumina}");
                if (lumina != null)
                {
                    //Plugin.Log.Debug($"icon path : {lumina.Icon}");
                    uint icon_id = (lumina.Icon == 0) ? (uint)FALLBACK_ICON : lumina.Icon;
                    var icon = Plugin.TextureProvider.GetIcon(icon_id, hq ? IconFlags.ItemHighQuality : IconFlags.None);
                    if (icon != null)
                    {
                        //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                        ImGui.Image(icon.ImGuiHandle, icon_size);
                    }
                }
            }
        }

        public static void DrawIcon(Vector2 icon_size, bool hq, uint icon_id)
        {
            if (Plugin.TextureProvider is null  /*||Plugin.Log is null*/) return;
            if (icon_id == 0) icon_id = FALLBACK_ICON;
            //Plugin.Log.Debug($"DrawIcon icon_id : {icon_id}");
            var icon = Plugin.TextureProvider.GetIcon(icon_id, hq ? IconFlags.ItemHighQuality : IconFlags.None);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, icon_size);
            }
        }
        public static void DrawIcon(Vector2 icon_size, bool hq, uint icon_id, Vector2 alpha)
        {
            if (Plugin.TextureProvider is null  /*||Plugin.Log is null*/) return;
            if (icon_id == 0) icon_id = FALLBACK_ICON;
            //Plugin.Log.Debug($"DrawIcon icon_id : {icon_id}");
            var icon = Plugin.TextureProvider.GetIcon(icon_id, hq ? IconFlags.ItemHighQuality : IconFlags.None);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, icon_size, new Vector2(0, 0), alpha);
            }
        }

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
            else if (Array.Exists(Datacenter.Chaos, w => w == name))
            {
                return "Chaos";
            }
            else if (Array.Exists(Datacenter.Crystal, w => w == name))
            {
                return "Crystal";
            }
            else if (Array.Exists(Datacenter.Dynamis, w => w == name))
            {
                return "Dynamis";
            }
            else if (Array.Exists(Datacenter.Elemental, w => w == name))
            {
                return "Elemental";
            }
            else if (Array.Exists(Datacenter.Gaia, w => w == name))
            {
                return "Gaia";
            }
            else if (Array.Exists(Datacenter.Light, w => w == name))
            {
                return "Light";
            }
            else if (Array.Exists(Datacenter.Mana, w => w == name))
            {
                return "Mana";
            }
            else if (Array.Exists(Datacenter.Materia, w => w == name))
            {
                return "Materia";
            }
            else if (Array.Exists(Datacenter.Meteor, w => w == name))
            {
                return "Meteor";
            }
            else if (Array.Exists(Datacenter.Primal, w => w == name))
            {
                return "Primal";
            }
            else if (Array.Exists(Datacenter.Shadow, w => w == name))
            {
                return "Shadow";
            }

            return string.Empty;
        }

        public static string GetRegionFromWorld(string name)
        {
            if (Array.Exists(Region.EU, w => w == name))
            {
                return "EU";
            }
            else if (Array.Exists(Region.JP, w => w == name))
            {
                return "JP";
            }
            else if (Array.Exists(Region.NA, w => w == name))
            {
                return "NA";
            }
            else if (Array.Exists(Region.OCE, w => w == name))
            {
                return "OCE";
            }
            return string.Empty;
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

        public static string GetRace(int gender, uint race)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Race>? dr = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Race>(GetLocale());
            if (dr != null)
            {
                Lumina.Excel.GeneratedSheets.Race? lumina = dr.GetRow((uint)race);
                if (lumina != null)
                    return (gender == 0) ? lumina.Masculine : lumina.Feminine;
            }
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
        }

        public static ClientLanguage GetLocale()
        {
            var config = Plugin.PluginInterface.GetPluginConfig() as Configuration;
            return (config is not null) ? config.Language : ClientLanguage.English;
        }
        public static string GetTribe(int gender, uint tribe)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Tribe>? dt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Tribe>(GetLocale());
            if (dt != null)
            {
                Lumina.Excel.GeneratedSheets.Tribe? lumina = dt.GetRow((uint)tribe);
                if (lumina != null)
                    return (gender == 0) ? lumina.Masculine : lumina.Feminine;
            }
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
        }

        public static string GetTown(int town)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Town>? dt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Town>(GetLocale());
            if (dt != null)
            {
                Lumina.Excel.GeneratedSheets.Town? lumina = dt.GetRow((uint)town);
                if (lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Town>(GetLocale()).GetRow((uint)town)!;
            return lumina.Name;*/
            /*return town switch
            {
                1 => "Limsa Lominsa",
                2 => "Gridania",
                3 => "Ul'dah",
                _ => string.Empty,
            };*/
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

        public static string GetGuardian(int guardian)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GuardianDeity>? dg = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GuardianDeity>(GetLocale());
            if (dg != null)
            {
                Lumina.Excel.GeneratedSheets.GuardianDeity? lumina = dg.GetRow((uint)guardian);
                if (lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
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

        public static string GetGrandCompany(int id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompany>? dgc = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompany>(GetLocale());
            if (dgc != null)
            {
                Lumina.Excel.GeneratedSheets.GrandCompany? lumina = dgc.GetRow((uint)id);
                if (lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompany>(GetLocale()).GetRow((uint)id)!;
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

        public static string GetGrandCompanyRank(int company, int rank, int gender)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            if (company == 1)
            {
                if (gender == 0)
                {
                    Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GCRankLimsaMaleText>? dgcrlmt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankLimsaMaleText>(GetLocale());
                    if (dgcrlmt != null)
                    {
                        Lumina.Excel.GeneratedSheets.GCRankLimsaMaleText? lumina = dgcrlmt.GetRow((uint)rank);
                        if (lumina != null)
                            return lumina.NameRank;
                    }
                }
                else
                {
                    Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GCRankLimsaFemaleText>? dgcrlft = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankLimsaFemaleText>(GetLocale());
                    if (dgcrlft != null)
                    {
                        Lumina.Excel.GeneratedSheets.GCRankLimsaFemaleText? lumina = dgcrlft.GetRow((uint)rank);
                        if (lumina != null)
                            return lumina.NameRank;
                    }
                }
                return string.Empty;
            }
            else if (company == 2)
            {
                if (gender == 0)
                {
                    Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GCRankUldahMaleText>? dgcrlmt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankUldahMaleText>(GetLocale());
                    if (dgcrlmt != null)
                    {
                        Lumina.Excel.GeneratedSheets.GCRankUldahMaleText? lumina = dgcrlmt.GetRow((uint)rank);
                        if (lumina != null)
                            return lumina.NameRank;
                    }
                }
                else
                {
                    Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GCRankUldahFemaleText>? dgcrlft = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankUldahFemaleText>(GetLocale());
                    if (dgcrlft != null)
                    {
                        Lumina.Excel.GeneratedSheets.GCRankUldahFemaleText? lumina = dgcrlft.GetRow((uint)rank);
                        if (lumina != null)
                            return lumina.NameRank;
                    }
                }
                return string.Empty;
            }
            else if (company == 3)
            {
                if (gender == 0)
                {
                    Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GCRankGridaniaMaleText>? dgcrlmt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankGridaniaMaleText>(GetLocale());
                    if (dgcrlmt != null)
                    {
                        Lumina.Excel.GeneratedSheets.GCRankGridaniaMaleText? lumina = dgcrlmt.GetRow((uint)rank);
                        if (lumina != null)
                            return lumina.NameRank;
                    }
                }
                else
                {
                    Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GCRankGridaniaFemaleText>? dgcrlft = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankGridaniaFemaleText>(GetLocale());
                    if (dgcrlft != null)
                    {
                        Lumina.Excel.GeneratedSheets.GCRankGridaniaFemaleText? lumina = dgcrlft.GetRow((uint)rank);
                        if (lumina != null)
                            return lumina.NameRank;
                    }
                }
                return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static uint GetGrandCompanyRankMaxSeals(int rank)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return 0;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompanyRank>? dgcr = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompanyRank>(GetLocale());
            if (dgcr != null)
            {
                Lumina.Excel.GeneratedSheets.GrandCompanyRank? lumina = dgcr.GetRow((uint)rank);
                if (lumina != null)
                    return lumina.MaxSeals;
            }
            return 0;
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

        public static Lumina.Excel.GeneratedSheets.Item? GetItemFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Item>? ditm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(GetLocale());
            if (ditm != null)
            {
                Lumina.Excel.GeneratedSheets.Item? lumina = ditm.GetRow(id);
                return lumina;
            }
            return null;
        }

        public static Lumina.Excel.GeneratedSheets.EventItem? GetEventItemFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.EventItem>? deitm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.EventItem>(GetLocale());
            if (deitm != null)
            {
                Lumina.Excel.GeneratedSheets.EventItem? lumina = deitm.GetRow(id);
                return lumina;
            }
            return null;
        }
        public static string GetItemNameFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Item>? ditm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(GetLocale());
            if (ditm != null)
            {
                Lumina.Excel.GeneratedSheets.Item? lumina = ditm.GetRow(id);
                if (lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
        }

        public static IEnumerable<Lumina.Excel.GeneratedSheets.Item>? GetItemsFromName(string name)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Item>? ditm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(GetLocale());
            if (ditm != null)
            {
                IEnumerable<Lumina.Excel.GeneratedSheets.Item>? items = ditm.Where(i => i.Name.RawString.Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
                return items;
            }
            return null;
        }

        public static Lumina.Excel.GeneratedSheets.ItemLevel? GetItemLevelFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            //Plugin.Log.Debug($"GetItemFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.ItemLevel>? dilvl = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ItemLevel>(GetLocale());
            if (dilvl != null)
            {
                Lumina.Excel.GeneratedSheets.ItemLevel? lumina = dilvl.GetRow(id);
                return lumina;
            }
            return null;
        }
        public static Lumina.Excel.GeneratedSheets.Stain? GetStainFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Stain>? ds = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Stain>(GetLocale());
            if (ds != null)
            {
                Lumina.Excel.GeneratedSheets.Stain? lumina = ds.GetRow(id);
                return lumina;
            }
            return null;
        }

        public static string GetJobNameFromId(uint id, bool abbreviation = false)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");

            Lumina.Excel.GeneratedSheets.ClassJob? lumina = GetClassJobFromId(id);
            if (lumina != null)
            {
                return (abbreviation) ? lumina.Abbreviation : Capitalize(lumina.Name);
            }

            return string.Empty;
        }

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
            if (Plugin.DataManager is null || Plugin.Log is null) return 0;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.ParamGrow>? dbt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ParamGrow>(GetLocale());
            if (dbt != null)
            {
                Lumina.Excel.GeneratedSheets.ParamGrow? lumina = dbt.GetRow((uint)level);
                if (lumina != null)
                    return lumina.ExpToNext;
            }
            return 0;
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

        public static string GetSlotName(short id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            return id switch
            {
                0 => GetAddonString(11524),
                1 => GetAddonString(12227),
                2 => GetAddonString(11525),
                3 => GetAddonString(11526),
                4 => GetAddonString(11527),
                6 => GetAddonString(11528),
                7 => GetAddonString(11529),
                8 => GetAddonString(11530),
                9 => GetAddonString(11531),
                10 => GetAddonString(11532),
                11 => GetAddonString(11533),
                12 => GetAddonString(11534),
                13 => GetAddonString(12238),
                _ => string.Empty,
            };
        }

        //public static void DrawGear(List<Gear> gears, uint job, int jobLevel, int middleWidth, int middleHeigth, bool retainer = false, int maxLevel = 0)
        public static void DrawGear(ref GlobalCache _globalCache, ref Dictionary<GearSlot, IDalamudTextureWrap?> defaultTextures, List<Gear> gears, uint job, int jobLevel, int middleWidth, int middleHeigth, bool retainer = false, int maxLevel = 0)
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
                DrawGearPiece(ref _globalCache, gears, GearSlot.MH, GetAddonString(11524), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.MH], defaultTextures[GearSlot.EMPTY]);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{GetAddonString(335)} {jobLevel}");
                if (ImGui.BeginTable("###GearTable#RoleIconNameTable", 2))
                {
                    ImGui.TableSetupColumn("###GearTable#RoleColumn#RoleIcon", ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn("###GearTable#RoleColumn#RoleName", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawIcon(new Vector2(40, 40), false, GetJobIcon(job));
                    DrawIcon_test(_globalCache.IconStorage.LoadIcon(GetJobIcon(job)), new Vector2(40, 40));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{GetJobNameFromId(job)}");

                    ImGui.EndTable();
                }
                ImGui.TableSetColumnIndex(2);
                if(retainer)
                    ImGui.TextUnformatted($"{GetAddonString(2325).Replace("[", "").Replace("]", "")}{maxLevel}");

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
                    DrawGearPiece(ref _globalCache, gears, GearSlot.HEAD, GetAddonString(11525), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.HEAD], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.BODY, GetAddonString(11526), new Vector2(40, 40), 10033);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.BODY, GetAddonString(11526), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.BODY], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.HANDS, GetAddonString(11527), new Vector2(40, 40), 10034);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.HANDS, GetAddonString(11527), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.HANDS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.LEGS, GetAddonString(11528), new Vector2(40, 40), 10035);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.LEGS, GetAddonString(11528), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.LEGS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.FEET, GetAddonString(11529), new Vector2(40, 40), 10035);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.FEET, GetAddonString(11529), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.FEET], defaultTextures[GearSlot.EMPTY]);
                    ImGui.EndTable();
                }

                ImGui.TableSetColumnIndex(1);
                //DrawIcon(new Vector2(middleWidth, middleHeigth), false, 055396);
                DrawIcon_test(_globalCache.IconStorage.LoadIcon(055396), new Vector2(middleWidth, middleHeigth));

                ImGui.TableSetColumnIndex(2);
                if (ImGui.BeginTable("###GearTable#RightGearColumn", 1))
                {
                    ImGui.TableSetupColumn("###GearTable#RightGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.OH, GetAddonString(12227), new Vector2(40, 40), 30067);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.OH, GetAddonString(12227), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.OH], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.EARS, GetAddonString(11530), new Vector2(40, 40), 9293);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.EARS, GetAddonString(11530), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.EARS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.NECK, GetAddonString(11531), new Vector2(40, 40), 9292);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.NECK, GetAddonString(11531), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.NECK], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.WRISTS, GetAddonString(11532), new Vector2(40, 40), 9294);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.WRISTS, GetAddonString(11532), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.WRISTS], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.RIGHT_RING, GetAddonString(11533), new Vector2(40, 40), 9295);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.RIGHT_RING, GetAddonString(11533), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.RIGHT_RING], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.LEFT_RING, GetAddonString(11534), new Vector2(40, 40), 9295);
                    DrawGearPiece(ref _globalCache, gears, GearSlot.LEFT_RING, GetAddonString(11534), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.LEFT_RING], defaultTextures[GearSlot.EMPTY]);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //DrawGearPiece(ref _globalCache, gears, GearSlot.SOUL_CRYSTAL, GetAddonString(12238), new Vector2(40, 40), 55396);//Todo: Find Soul Crystal empty icon
                    DrawGearPiece(ref _globalCache, gears, GearSlot.SOUL_CRYSTAL, GetAddonString(12238), new Vector2(40, 40), /*ref*/ defaultTextures[GearSlot.SOUL_CRYSTAL], defaultTextures[GearSlot.EMPTY]);
                    ImGui.EndTable();
                }
                ImGui.EndTable();
            }
        }

        public static (Vector2 uv0, Vector2 uv1) GetTextureCoordinate(Vector2 texture_size, int u, int v, int w, int h)
        {
            float u1 = (u + w) / texture_size.X;
            float v1 = (v + h) / texture_size.Y;
            Vector2 uv0 = new(u / texture_size.X, v / texture_size.Y);
            Vector2 uv1 = new(u1, v1);
            return (uv0, uv1);
        }

        public static void DrawRoleTexture(ref IDalamudTextureWrap texture, RoleIcon role, Vector2 size)
        {
            var (uv0, uv1) = role switch
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
        public static void DrawGearPiece(ref GlobalCache _globalCache, List<Gear> Gear, GearSlot slot, string tooltip, Vector2 icon_size, /*uint fallback_icon*/ /*ref*/ IDalamudTextureWrap? fallbackTexture, IDalamudTextureWrap? emptySlot)
        {
            if (fallbackTexture is null || emptySlot is null) return;
            var GEAR = Gear.First(g => g.Slot == (short)slot);
            //Plugin.Log.Debug($"{slot}, {GEAR.ItemId}");
            if (GEAR == null || GEAR.ItemId == 0)
            {
                //DrawItemIcon(icon_size, false, fallback_icon);
                /*var i = _itemStorage.LoadItem(fallback_icon);
                if (i == null) return;
                DrawIcon_test(_globalCache.IconStorage.LoadIcon(i.Icon), icon_size);*/
                //ImGui.Image(fallbackTexture.ImGuiHandle, icon_size, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.33f });
                var p = ImGui.GetCursorPos();
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
                var i = _globalCache.ItemStorage.LoadItem(GEAR.ItemId);
                if(i == null) return;
                DrawIcon_test(_globalCache.IconStorage.LoadIcon(i.Icon, GEAR.HQ), icon_size);
                if (ImGui.IsItemHovered())
                {
                    DrawGearTooltip(ref _globalCache, GEAR, i);
                }
            }
        }
        public static void DrawGearTooltip(ref GlobalCache _globalCache, Gear item, Item dbItem)
        {
            if (Plugin.TextureProvider == null || Plugin.DataManager is null || Plugin.Log is null) return;
            //Lumina.Excel.GeneratedSheets.Item? dbItem = GetItemFromId(item.ItemId);
            if (dbItem == null) return;
            ItemLevel? ilvl = GetItemLevelFromId(dbItem.LevelItem.Row);
            if (ilvl == null) return;

            ImGui.BeginTooltip();

            if (dbItem.IsUnique || dbItem.IsUntradable)
            {
                if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Unique", 3))
                {
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Unique#IsUnique", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted("");
                    ImGui.TableSetColumnIndex(1);
                    if (dbItem.IsUnique)
                    {
                        ImGui.TextUnformatted($"{GetAddonString(494)}");// Unique
                    }
                    if (dbItem.IsUntradable)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{GetAddonString(495)}");// Untradable
                    }
                    /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{GetAddonString(496)}");// Binding
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
                DrawIcon_test(_globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40,40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
                if (dbItem.IsGlamourous)
                {
                    ImGui.TextUnformatted($"{(char)SeIconChar.Glamoured} {GetItemNameFromId(item.GlamourID)}");
                }
                ImGui.TableNextRow(); 
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{GetSlotName(item.Slot)}");
                ImGui.EndTable();
            }
            if(ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#Defense", 3))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Defense#Empty", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Defense#Icon", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#Defense#Name", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{GetAddonString(3244)}");// Defense
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{GetAddonString(3246)}");// Magic Defense
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.DefensePhys}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.DefenseMag}");
                ImGui.EndTable();
            }
            ImGui.Separator();
            ImGui.TextUnformatted($"{GetAddonString(13775)} {ilvl.RowId}");// Item Level
            ImGui.Separator();
            ImGui.TextUnformatted($"{GetClassJobCategoryFromId(dbItem.ClassJobCategory.Value?.RowId)}");
            ImGui.TextUnformatted($"{GetAddonString(1034)} {dbItem.LevelEquip}");
            ImGui.Separator();
            if (!dbItem.IsAdvancedMeldingPermitted)
            { 
                ImGui.TextUnformatted($"{GetAddonString(4655)}"); // Advanced Melding Forbidden
            }
            if (item.Stain > 0)
            {
                Stain? dye = GetStainFromId(item.Stain);
                if (dye != null)
                {
                    ImGui.TextUnformatted($"{dye.Name}");
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
                ImGui.TextUnformatted($"{GetAddonString(3226)} +");// Defense
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{GetAddonString(3227)} +");// Vitality
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{GetAddonString(3241)} +");// Critical Hit
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{GetAddonString(3249)} +");// Skill Speed
                ImGui.EndTable();
            }
            if (dbItem.MateriaSlotCount > 0) {
                ImGui.Separator();
                ImGui.TextUnformatted($"{GetAddonString(491)}");// Materia
                for (int i = 0;i < dbItem.MateriaSlotCount;i++)
                {
                    ImGui.ColorButton($"##Item_{item.ItemId}#Materia#{i}", new Vector4(34, 169, 34, 1), ImGuiColorEditFlags.None, new Vector2(16,16));
                }
                //Plugin.Log.Debug($"Item materia: {item.Materia}");
            }
            ImGui.Separator();
            ImGui.TextUnformatted($"{GetAddonString(497)}");// Crafting & Repairs
            ImGui.TextUnformatted($"{GetAddonString(498)} : {(item.Condition / 300f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted($"{GetAddonString(499)} : {(item.Spiritbond / 100f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted($"{GetAddonString(500)} : {GetJobNameFromId(dbItem.ClassJobRepair.Row)}");//Repair Level
            ImGui.TextUnformatted($"{GetAddonString(518)} : {GetItemRepairResource(dbItem.ItemRepair.Row)}");//Materials
            ImGui.TextUnformatted($"{GetAddonString(995)} : ");//Quick Repairs
            ImGui.TextUnformatted($"{GetAddonString(993)} : ");//Materia Melding
            ImGui.TextUnformatted($"{GetExtractableString(dbItem)}");
            ImGui.TextUnformatted($"{GetSellableString(dbItem, item)}");//Materia Melding
            if((item.CrafterContentID > 0))
                ImGui.TextUnformatted($"Crafted");

            ImGui.EndTooltip();
        }
        
        public static void DrawItemTooltip(Inventory item)
        {
            if (Plugin.TextureProvider == null || Plugin.DataManager is null || Plugin.Log is null) return;
            Lumina.Excel.GeneratedSheets.Item? dbItem = GetItemFromId(item.ItemId);
            if (dbItem == null) return;
            Lumina.Excel.GeneratedSheets.ItemLevel? ilvl = GetItemLevelFromId(dbItem.LevelItem.Row);
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
                        ImGui.TextUnformatted($"{GetAddonString(494)}");// Unique
                    }
                    if (dbItem.IsUntradable)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{GetAddonString(495)}");// Untradable
                    }
                    /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{GetAddonString(496)}");// Binding
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
                DrawItemIcon(new Vector2(40, 40), item.HQ, item.ItemId);
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
                ImGui.TextUnformatted($"{item.Quantity}/99 (Total: {item.Quantity})");
                ImGui.TableSetColumnIndex(2);
                ImGui.EndTable();
            }

            ImGui.Separator();
            ImGui.TextUnformatted($"{GetAddonString(497)}");// Crafting & Repairs

            ImGui.EndTooltip();
        }
        
        public static void DrawEventItemTooltip(Inventory item)
        {
            if (Plugin.TextureProvider == null || Plugin.DataManager is null || Plugin.Log is null) return;
            Lumina.Excel.GeneratedSheets.EventItem? dbItem = GetEventItemFromId(item.ItemId);
            if (dbItem == null) return;

            ImGui.BeginTooltip();

            if (ImGui.BeginTable($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon", 2))
            {
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon#Icon", ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon#Name", ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawItemIcon(new Vector2(40, 40), item.HQ, item.ItemId);
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
            ImGui.TextUnformatted($"{GetAddonString(497)}");// Crafting & Repairs

            ImGui.EndTooltip();
        }
        
        public static void DrawCrystalTooltip(uint itemId, int amount)
        {
            if (Plugin.TextureProvider == null || Plugin.DataManager is null || Plugin.Log is null) return;
            Lumina.Excel.GeneratedSheets.Item? dbItem = GetItemFromId(itemId);
            if (dbItem == null) return;
            Lumina.Excel.GeneratedSheets.ItemLevel? ilvl = GetItemLevelFromId(dbItem.LevelItem.Row);
            if (ilvl == null) return;

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
                        ImGui.TextUnformatted($"{GetAddonString(494)}");// Unique
                    }
                    if (dbItem.IsUntradable)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{GetAddonString(495)}");// Untradable
                    }
                    /*if (i.Is) No Binding value???
                    {
                        ImGui.TextUnformatted($"{GetAddonString(496)}");// Binding
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
                DrawItemIcon(new Vector2(40, 40), false, itemId);
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
            ImGui.TextUnformatted($"{GetAddonString(497)}");// Crafting & Repairs

            ImGui.EndTooltip();
        }

        private static string GetExtractableString(Lumina.Excel.GeneratedSheets.Item item)
        {
            string str = GetAddonString(1361);
            Plugin.Log.Debug($"extract str: {str} => item desynth {item.Desynth}");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(1),0))>Y<Else/>N</If>", (item.AdditionalData) ? "Y" : "N");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(2),0))>Y<Else/>N</If>", (item.IsGlamourous) ? "Y" : "N");
            str = str.Replace("Extractable: YN", "Extractable: ");
            str = str.Replace("Projectable: YN", (item.IsGlamourous) ? "Projectable: Y" : "Projectable: N");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(3),0))><Value>IntegerParameter(4)</Value>.00<Else/>N</If>", (item.Desynth == 0)? "N" : "Y");
            str = str.Replace(".00N", (item.Desynth == 0)? "N" : "Y");
            return str;
        }
        private static string GetSellableString(Lumina.Excel.GeneratedSheets.Item item, Gear gear)
        {
            var price = item.PriceLow * (gear.HQ ? 1.1f : 1.0);
            //Plugin.Log.Debug($"PriceLow : {item.PriceLow}, PriceMid: {item.PriceMid}, stackValue {price}");
            string str = GetAddonString(484);
            //Plugin.Log.Debug($"price str: {str}");
            if (item.PriceLow == 0) {
                str = GetAddonString(503).Replace(" <If(IntegerParameter(1))><Else/> ", "");
            }
            else
            {
                //str = str.Replace("<Format(IntegerParameter(1),FF022C)/>", price.ToString());
                str = str.Replace(",", Math.Ceiling(price).ToString("N0"));
                //str = str.Replace("<If(IntegerParameter(2))><Else/>　Market Prohibited</If>", "");
                //str = str.Replace("　Market Prohibited", "");
            }
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

        public static string GetAddonString(int id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Addon>? da = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Addon>(GetLocale());
            if (da != null)
            {
                Lumina.Excel.GeneratedSheets.Addon? lumina = da.GetRow((uint)id)!;
                if (lumina != null)
                    return lumina.Text;
            }
            return string.Empty;
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
        

        public static string GetTribalNameFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>? dbt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>(GetLocale());
            if (dbt != null)
            {
                Lumina.Excel.GeneratedSheets.BeastTribe? lumina = dbt.GetRow(id);
                if (lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
        }
        public static string GetTribalCurrencyFromId(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>? dbt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>(GetLocale());
            if (dbt != null)
            {
                Lumina.Excel.GeneratedSheets.BeastTribe? lumina = dbt.GetRow(id);
                if (lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
            /*var lumina = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>(GetLocale()).GetRow(id)!;
            return lumina.CurrencyItem.Value.Name;*/
        }
        public static string Capitalize(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string CapitalizeSnakeCase(string str)
        {
            List<string> items = [];
            foreach (var item in str.Split("_"))
            {
                items.Add(Capitalize(item));
            }
            return string.Join("_", items);
        }
        
        public static string CapitalizeCurrency(string str)
        {
            if (str == "MGP" || str == "MGF")
            {
                return str;
            }
            List<string> items = [];
            foreach (var item in str.Split("_"))
            {
                items.Add(Capitalize(item.ToLower()));
            }
            return string.Join("_", items);
        }

        public static bool IsQuestCompleted(int questId)
        {
            //Plugin.Log.Debug($"IsQuestCompleted questId: {questId}");

            return FFXIVClientStructs.FFXIV.Client.Game.QuestManager.IsQuestComplete((uint)questId);
        }

        public static string GetClassJobCategoryFromId(uint? id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null || id is null) return string.Empty;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.ClassJobCategory>? djc = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJobCategory>(GetLocale());
            if (djc != null)
            {
                Lumina.Excel.GeneratedSheets.ClassJobCategory? lumina = djc.GetRow(id.Value);
                if(lumina != null)
                    return lumina.Name;
            }
            return string.Empty;
        }
        public static Lumina.Excel.GeneratedSheets.ClassJob? GetClassJobFromId(uint? id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null || id is null) return null;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>? djc = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>(GetLocale());
            if (djc != null)
            {
                Lumina.Excel.GeneratedSheets.ClassJob? lumina = djc.GetRow(id.Value);
                return lumina;
            }
            return null;
        }
        
        public static Lumina.Excel.GeneratedSheets.ClassJob? GetClassJobFromId(uint? id, Dalamud.ClientLanguage clientLanguage)
        {
            if (Plugin.DataManager is null || Plugin.Log is null || id is null) return null;
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>? djc = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>(clientLanguage);
            if (djc != null)
            {
                Lumina.Excel.GeneratedSheets.ClassJob? lumina = djc.GetRow(id.Value);
                return lumina;
            }
            return null;
        }

        public static string GetItemRepairResource(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return string.Empty;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.ItemRepairResource>? dirr = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ItemRepairResource>(GetLocale());
            if (dirr != null)
            {
                Lumina.Excel.GeneratedSheets.ItemRepairResource? lumina = dirr.GetRow(id)!;
                if (lumina != null)
                {
                    var itm = lumina.Item;
                    if (itm != null)
                    {
                        var v = itm.Value;
                        if (v != null)
                            return v.Name;
                    }
                }
            }
            return string.Empty;
        }
        
        public static string GetFCTag(Character localPlayer)
        {
            string FCTag = string.Empty;
            if ((string.IsNullOrEmpty(localPlayer.CurrentWorld) || string.IsNullOrEmpty(localPlayer.CurrentDatacenter) || string.IsNullOrEmpty(localPlayer.CurrentWorld)) || (localPlayer.CurrentWorld == localPlayer.HomeWorld && localPlayer.CurrentRegion == localPlayer.Region))
            {
                FCTag = localPlayer.FCTag;
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion == localPlayer.Region)
            {
                FCTag = GetAddonString(12541);
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion != localPlayer.Region)
            {
                FCTag = GetAddonString(12625);
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion != localPlayer.Region)
            {
                FCTag = GetAddonString(12627);
            }
            //Plugin.Log.Debug($"localPlayerRegion : {localPlayerRegion}");
            //Plugin.Log.Debug($"localPlayer.CurrentRegion : {localPlayer.CurrentRegion}");
            return FCTag;
        }
        public static string GetCrystalName(int i)
        {
            return i switch
            {
                0 => GetItemNameFromId(2),
                1 => GetItemNameFromId(3),
                2 => GetItemNameFromId(4),
                3 => GetItemNameFromId(5),
                4 => GetItemNameFromId(6),
                5 => GetItemNameFromId(7),
                6 => GetItemNameFromId(8),
                7 => GetItemNameFromId(9),
                8 => GetItemNameFromId(10),
                9 => GetItemNameFromId(11),
                10 => GetItemNameFromId(12),
                11 => GetItemNameFromId(13),
                12 => GetItemNameFromId(14),
                13 => GetItemNameFromId(15),
                14 => GetItemNameFromId(16),
                15 => GetItemNameFromId(17),
                16 => GetItemNameFromId(18),
                17 => GetItemNameFromId(19),
                _ => string.Empty
            };
        }

        public static Lumina.Excel.GeneratedSheets.RetainerTask? GetRetainerTask(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.RetainerTask>? drt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.RetainerTask>(GetLocale());
            if (drt != null)
            {
                Lumina.Excel.GeneratedSheets.RetainerTask? lumina = drt.GetRow(id);
                return lumina;
            }
            return null;
        }
        public static Lumina.Excel.GeneratedSheets.RetainerTaskNormal? GetRetainerTaskNormal(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.RetainerTaskNormal>? drtn = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.RetainerTaskNormal>(GetLocale());
            if (drtn != null)
            {
                Lumina.Excel.GeneratedSheets.RetainerTaskNormal? lumina = drtn.GetRow(id);
                return lumina;
            }
            return null;
        }
        public static Lumina.Excel.GeneratedSheets.RetainerTaskRandom? GetRetainerTaskRandom(uint id)
        {
            if (Plugin.DataManager is null || Plugin.Log is null) return null;
            Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.RetainerTaskRandom>? drtr = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.RetainerTaskRandom>(GetLocale());
            if (drtr != null)
            {
                Lumina.Excel.GeneratedSheets.RetainerTaskRandom? lumina = drtr.GetRow(id);
                return lumina;
            }
            return null;
        }

        public static string UnixTimeStampToDateTime(long lastOnline)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(lastOnline).ToLocalTime();
            return dateTime.ToString();
        }



        ////////////////////////Test
        public static IDalamudTextureWrap? LoadIcon(uint icon_id, bool hq = false)
        {
            if (Plugin.TextureProvider is null  /*||Plugin.Log is null*/) return null;
            if (icon_id == 0) icon_id = FALLBACK_ICON;
            //Plugin.Log.Debug($"DrawIcon icon_id : {icon_id}");
            return Plugin.TextureProvider.GetIcon(icon_id, hq ? IconFlags.ItemHighQuality : IconFlags.None);
        }
        public static void DrawIcon_test(IDalamudTextureWrap? icon, Vector2 icon_size)
        {
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, icon_size);
            }
        }
        public static void DrawIcon_test(IDalamudTextureWrap? icon, Vector2 icon_size, Vector4 alpha)
        {
            if (icon != null)
            {
                ImGui.Image(icon.ImGuiHandle, icon_size, Vector2.Zero, Vector2.One, alpha);
            }
        }// call this in the window constructor for all the images that won't change (gil, roles,classes,etc) and store them in global dictionary or something so it doesn't load them every draw

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