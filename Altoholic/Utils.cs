using Altoholic.Models;
using Dalamud;
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop.Attributes;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace Altoholic
{
    internal class Utils
    {
        public static void DrawItemIcon(ITextureProvider textureProvider, IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, Vector2 icon_size, bool hq, uint item_id)
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(currentLocale).GetRow(item_id)!;
            //pluginLog.Debug($"lumina : ${lumina}");
            if (lumina != null)
            {
                //var icon = (hq) ? textureProvider.GetIcon(lumina.Icon) : textureProvider.GetIcon(lumina.Icon);
                //Todo: HQ
                //pluginLog.Debug($"icon path : {lumina.Icon}");
                var icon = textureProvider.GetIcon(lumina.Icon);
                if (icon != null)
                {
                    //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                    ImGui.Image(icon.ImGuiHandle, icon_size);
                }
            }
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        
        public static void DrawIcon(ITextureProvider textureProvider, IPluginLog pluginLog, Vector2 icon_size, bool hq, uint icon_id)
        {
            //pluginLog.Debug($"DrawIcon icon_id : {icon_id}");
            var icon = textureProvider.GetIcon(icon_id);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, icon_size);
            }
        }
        public static void DrawIcon(ITextureProvider textureProvider, IPluginLog pluginLog, Vector2 icon_size, bool hq, uint icon_id, Vector2 alpha)
        {
            //pluginLog.Debug($"DrawIcon icon_id : {icon_id}");
            var icon = textureProvider.GetIcon(icon_id);
            if (icon != null)
            {
                //ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
                ImGui.Image(icon.ImGuiHandle, icon_size, new Vector2(0,0), alpha);
            }
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

        public static string GetRace(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int gender, uint race)
        {
            if (gender == 0)
                return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Race>(currentLocale).GetRow((uint)race)!.Masculine;
            else
                return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Race>(currentLocale).GetRow((uint)race)!.Feminine;
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

        public static string GetTribe(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int gender, uint tribe)
        {
            if (gender == 0)
                return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Tribe>(currentLocale).GetRow((uint)tribe)!.Masculine;
            else
                return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Tribe>(currentLocale).GetRow((uint)tribe)!.Feminine;
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

        public static string GetTown(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int town)
        {
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Town>(currentLocale).GetRow((uint)town)!;
            return lumina.Name;
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
                _ => 055396,
            };
        }

        public static string GetGuardian(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int guardian)
        {
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GuardianDeity>(currentLocale).GetRow((uint)guardian)!;
            return lumina.Name;
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
                _ => 055396,
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

        public static string GetGrandCompany(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int id)
        {
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompany>(currentLocale).GetRow((uint)id)!;
            return lumina.Name;
        }
        
        public static uint GetGrandCompanyIcon(int company)
        {
            return company switch
            {
                1 => 060871,
                2 => 060872,
                3 => 060873,
                _ => 055396,
            };
        }

        public static string GetGrandCompanyRank(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int company, int rank, int gender)
        {
            if (company == 1)
            {
                if (gender == 0)
                    return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankLimsaMaleText>(currentLocale).GetRow((uint)rank)!.NameRank;
                else
                    return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankLimsaFemaleText>(currentLocale).GetRow((uint)rank)!.NameRank;
            }
            else if (company == 2)
            {
                if (gender == 0)
                    return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankUldahMaleText>(currentLocale).GetRow((uint)rank)!.NameRank;
                else
                    return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankUldahFemaleText>(currentLocale).GetRow((uint)rank)!.NameRank;
            }
            else if (company == 3)
            {
                if (gender == 0)
                    return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankGridaniaMaleText>(currentLocale).GetRow((uint)rank)!.NameRank;
                else
                    return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GCRankGridaniaFemaleText>(currentLocale).GetRow((uint)rank)!.NameRank;
            }
            else
            {
                return string.Empty;
            }
        }

        public static uint GetGrandCompanyRankMaxSeals(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int rank)
        {
            return dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.GrandCompanyRank>(currentLocale).GetRow((uint)rank)!.MaxSeals;
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
                    _ => 055396,
                },
                2 => company switch
                {
                    1 => 083602,
                    2 => 083702,
                    3 => 083652,
                    _ => 055396,
                },
                3 => company switch
                {
                    1 => 083603,
                    2 => 083703,
                    3 => 083653,
                    _ => 055396,
                },
                4 => company switch
                {
                    1 => 083604,
                    2 => 083704,
                    3 => 083654,
                    _ => 055396,
                },
                5 => company switch
                {
                    1 => 083605,
                    2 => 083705,
                    3 => 083655,
                    _ => 055396,
                },
                6 => company switch
                {
                    1 => 083606,
                    2 => 083706,
                    3 => 083656,
                    _ => 055396,
                },
                7 => company switch
                {
                    1 => 083607,
                    2 => 083707,
                    3 => 083657,
                    _ => 055396,
                },
                8 => company switch
                {
                    1 => 083608,
                    2 => 083708,
                    3 => 083658,
                    _ => 055396,
                },
                9 => company switch
                {
                    1 => 083609,
                    2 => 083709,
                    3 => 083659,
                    _ => 055396,
                },
                10 => company switch
                {
                    1 => 083610,
                    2 => 083710,
                    3 => 083660,
                    _ => 055396,
                },
                11 => company switch
                {
                    1 => 083611,
                    2 => 083711,
                    3 => 083661,
                    _ => 055396,
                },
                12 => company switch
                {
                    1 => 083612,
                    2 => 083712,
                    3 => 083662,
                    _ => 055396,
                },
                13 => company switch
                {
                    1 => 083613,
                    2 => 083713,
                    3 => 083663,
                    _ => 055396,
                },
                14 => company switch
                {
                    1 => 083614,
                    2 => 083714,
                    3 => 083664,
                    _ => 055396,
                },
                15 => company switch
                {
                    1 => 083615,
                    2 => 083715,
                    3 => 083665,
                    _ => 055396,
                },
                16 => company switch
                {
                    1 => 083616,
                    2 => 083716,
                    3 => 083666,
                    _ => 055396,
                },
                17 => company switch
                {
                    1 => 083617,
                    2 => 083717,
                    3 => 083667,
                    _ => 055396,
                },
                18 => company switch
                {
                    1 => 083618,
                    2 => 083718,
                    3 => 083668,
                    _ => 055396,
                },
                19 => company switch
                {
                    1 => 083619,
                    2 => 083719,
                    3 => 083669,
                    _ => 055396,
                },
                20 => company switch
                {
                    1 => 083620,
                    2 => 083720,
                    3 => 083670,
                    _ => 055396,
                },
                _ => 0,
            };
        }

        public static string GetItemNameFromId(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, uint id)
        {
            //pluginLog.Debug($"GetItemNameFromId : {id}");
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(currentLocale).GetRow(id)!;
            return lumina.Name;
        }
        
        public static string GetJobNameFromId(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, uint id)
        {
            //pluginLog.Debug($"GetItemNameFromId : {id}");
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>(currentLocale).GetRow(id)!;
            return lumina.NameEnglish;
        }
        public static string GetJobShortNameFromId(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, uint id)
        {
            //pluginLog.Debug($"GetItemNameFromId : {id}");
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ClassJob>(currentLocale).GetRow(id)!;
            return lumina.Abbreviation;
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
                _ => 0,
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
                _ => 0,
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
                _ => 0,
            };
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
                _ => 0,
            };
        }
        /*public static string GetRoleName(ClientLanguage currentLocale, int role_id)
        {
            return role_id switch
            {
                0 => currentLocale switch
                {
                    ClientLanguage.German => "Verteidiger",
                    ClientLanguage.English => "Tank",
                    ClientLanguage.French => "Tank",
                    ClientLanguage.Japanese => "Tank",
                    _ => string.Empty,
                },
                1 => currentLocale switch
                {
                    ClientLanguage.German => "Heiler",
                    ClientLanguage.English => "Healer",
                    ClientLanguage.French => "Soigneur",
                    ClientLanguage.Japanese => "Healer",
                    _ => string.Empty,
                },
                2 => currentLocale switch
                {
                    ClientLanguage.German => "Nahkampfangreifer",
                    ClientLanguage.English => "Melee DPS",
                    ClientLanguage.French => "DPS de mêlée",
                    ClientLanguage.Japanese => "Melee DPS",
                    _ => string.Empty,
                },
                3 => currentLocale switch
                {
                    ClientLanguage.German => "Phys. Fernkämpfer",
                    ClientLanguage.English => "Physical Ranged DPS",
                    ClientLanguage.French => "DPS physique à distance",
                    ClientLanguage.Japanese => "Physical Ranged DPS",
                    _ => string.Empty,
                },
                4 => currentLocale switch
                {
                    ClientLanguage.German => "Mag. Fernkämpfer",
                    ClientLanguage.English => "Magical Ranged DPS",
                    ClientLanguage.French => "DPS magique à distance",
                    ClientLanguage.Japanese => "Magical Ranged DPS",
                    _ => string.Empty,
                },
                5 => currentLocale switch
                {
                    ClientLanguage.German => "Disziplinen der Handwerker",
                    ClientLanguage.English => "Disciplines of the Hand",
                    ClientLanguage.French => "Disciple de la main",
                    ClientLanguage.Japanese => "クラフター",
                    _ => string.Empty,
                },
                6 => currentLocale switch
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

        public static string DrawItemTooltip(ITextureProvider textureProvider, IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, Gear item)
        {
            return GetItemNameFromId(dataManager, pluginLog, currentLocale, item.ItemId);
        }

        public static string GetAddonString(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, int id)
        {
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Addon>(currentLocale).GetRow((uint)id)!;
            return lumina.Text;
        }

        /*public static string GetCurrencyTitle(ClientLanguage currentLocale, int id)
        {
            return id switch
            {
                0 => currentLocale switch
                {
                    ClientLanguage.German => "Allgemein",
                    ClientLanguage.English => "Common",
                    ClientLanguage.French => "Commun",
                    ClientLanguage.Japanese => "共通",
                    _ => string.Empty,
                },
                1 => currentLocale switch
                {
                    ClientLanguage.German => "Kampf",
                    ClientLanguage.English => "Battle",
                    ClientLanguage.French => "Combats",
                    ClientLanguage.Japanese => "戦闘",
                    _ => string.Empty,
                },
                2 => currentLocale switch
                {
                    ClientLanguage.German => "Sonstiges",
                    ClientLanguage.English => "Other",
                    ClientLanguage.French => "Autres",
                    ClientLanguage.Japanese => "非戦闘",
                    _ => string.Empty,
                },
                3 => currentLocale switch
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

        /*public static string GetCommonTitle(ClientLanguage currentLocale, int id)
        {
            return id switch
            {
                0 => currentLocale switch
                {
                    ClientLanguage.German => "Gil",
                    ClientLanguage.English => "Gil",
                    ClientLanguage.French => "Gils",
                    ClientLanguage.Japanese => "Gil",
                    _ => string.Empty,
                },
                1 => currentLocale switch
                {
                    ClientLanguage.German => "Staatstaler",
                    ClientLanguage.English => "Company Seals",
                    ClientLanguage.French => "Sceaux de compagnie",
                    ClientLanguage.Japanese => "軍票",
                    _ => string.Empty,
                },
                2 => currentLocale switch
                {
                    ClientLanguage.German => "Wertmarken",
                    ClientLanguage.English => "Ventures",
                    ClientLanguage.French => "Jetons de tâche",
                    ClientLanguage.Japanese => "Venture Scrip",
                    _ => string.Empty,
                },
                3 => currentLocale switch
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

        /*public static string GetBattleTitle(ClientLanguage currentLocale, int id)
        {
            return id switch
            {
                0 => currentLocale switch
                {
                    ClientLanguage.German => "Allagische Steine",
                    ClientLanguage.English => "Allagan Tomestones",
                    ClientLanguage.French => "Mémoquartz allagois",
                    ClientLanguage.Japanese => "Allagan Tomestones",
                    _ => string.Empty,
                },
                1 => currentLocale switch
                {
                    ClientLanguage.German => "Zurücksetzung",
                    ClientLanguage.English => "Reset in",
                    ClientLanguage.French => "Remise à zéro",
                    ClientLanguage.Japanese => "リセット日時",
                    _ => string.Empty,
                },
                2 => currentLocale switch
                {
                    ClientLanguage.German => "Gesamt",
                    ClientLanguage.English => "Total",
                    ClientLanguage.French => "Total",
                    ClientLanguage.Japanese => "Total",
                    _ => string.Empty,
                },
                3 => currentLocale switch
                {
                    ClientLanguage.German => "Woche",
                    ClientLanguage.English => "This Week",
                    ClientLanguage.French => "Hebdomadaire",
                    ClientLanguage.Japanese => "Weekly",
                    _ => string.Empty,
                },
                4 => currentLocale switch
                {
                    ClientLanguage.German => "Eingestellt",
                    ClientLanguage.English => "Discontinued",
                    ClientLanguage.French => "Devise(s) désuette(s)",
                    ClientLanguage.Japanese => "Discontinued",
                    _ => string.Empty,
                },
                5 => currentLocale switch
                {
                    ClientLanguage.German => "PvP",
                    ClientLanguage.English => "PvP",
                    ClientLanguage.French => "Récompenses JcJ",
                    ClientLanguage.Japanese => "Honor",
                    _ => string.Empty,
                },
                6 => currentLocale switch
                {
                    ClientLanguage.German => "Wertmarken",
                    ClientLanguage.English => "Ventures",
                    ClientLanguage.French => "Jetons de tâche",
                    ClientLanguage.Japanese => "Venture Scrip",
                    _ => string.Empty,
                },
                7 => currentLocale switch
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
        public static string GetTribalTitle(ClientLanguage currentLocale)
        {
            return currentLocale switch
            {
                ClientLanguage.German => "Währungen",
                ClientLanguage.English => "Tribal Currency",
                ClientLanguage.French => "Monnaies tribales",
                ClientLanguage.Japanese => "Tribal Currency",
                _ => string.Empty,
            };
        }*/
        

        public static string GetTribalNameFromId(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, uint id)
        {
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>(currentLocale).GetRow(id)!;
            return lumina.Name;
        }
        public static string GetTribalCurrencyFromId(IDataManager dataManager, IPluginLog pluginLog, ClientLanguage currentLocale, uint id)
        {
            //pluginLog.Debug($"GetItemNameFromId : {id}");
            var lumina = dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.BeastTribe>(currentLocale).GetRow(id)!;
            return lumina.CurrencyItem.Value.Name;
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

        public static bool IsQuestCompleted(IPluginLog pluginLog, int questId)
        {
            //pluginLog.Debug($"IsQuestCompleted questId: {questId}");

            return FFXIVClientStructs.FFXIV.Client.Game.QuestManager.IsQuestComplete((uint)questId);
        }
    }
}
