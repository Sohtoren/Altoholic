using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Bindings.ImGui;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClassJob = Lumina.Excel.Sheets.ClassJob;
using Mount = Lumina.Excel.Sheets.Mount;
using Stain = Lumina.Excel.Sheets.Stain;
using TripleTriadCard = Lumina.Excel.Sheets.TripleTriadCard;
using Emote = Lumina.Excel.Sheets.Emote;
using TextCommand = Lumina.Excel.Sheets.TextCommand;
using Ornament = Lumina.Excel.Sheets.Ornament;
using Glasses = Lumina.Excel.Sheets.Glasses;
using Quest = Lumina.Excel.Sheets.Quest;
using Cabinet = Lumina.Excel.Sheets.Cabinet;
using System.Runtime.CompilerServices;
using BaseParam = Lumina.Excel.Sheets.BaseParam;
using Materia = Lumina.Excel.Sheets.Materia;
using PvPRankTransient = Altoholic.Models.PvPRankTransient;
using Vector2 = FFXIVClientStructs.FFXIV.Common.Math.Vector2;
using Vector4 = FFXIVClientStructs.FFXIV.Common.Math.Vector4;
using Lumina.Text.ReadOnly;


namespace Altoholic
{
    internal class Utils
    {
        // ReSharper disable once InconsistentNaming
        private const uint FALLBACK_ICON = 055396;

        // ReSharper disable once InconsistentNaming
        public static string GetDCString()
        {
            return "DC";
        }

        public static void ChatMessage(string message)
        {
            Plugin.ChatGui.Print($"[Altoholic] {message}");
        }
        public static void ChatMessage(ReadOnlySeString message)
        {
            Plugin.ChatGui.Print($"[Altoholic] {message}");
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
                return gender == 0 ? lumina.Value.Masculine.ExtractText() : lumina.Value.Feminine.ExtractText();
            return string.Empty;
        }

        public static string GetTribe(ClientLanguage currentLocale, int gender, uint tribe)
        {
            ExcelSheet<Tribe>? dt = Plugin.DataManager.GetExcelSheet<Tribe>(currentLocale);
            Tribe? lumina = dt?.GetRow(tribe);
            if (lumina != null)
                return gender == 0 ? lumina.Value.Masculine.ExtractText() : lumina.Value.Feminine.ExtractText();
            return string.Empty;
        }

        public static string GetTown(ClientLanguage currentLocale, int town)
        {
            ExcelSheet<Town>? dt = Plugin.DataManager.GetExcelSheet<Town>(currentLocale);
            Town? lumina = dt?.GetRow((uint)town);
            return lumina != null ? lumina.Value.Name.ExtractText() : string.Empty;
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
            return lumina != null ? lumina.Value.Name.ExtractText() : string.Empty;
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

        public static string GetNameday(ClientLanguage currentLocale, int day, int month)
        {
            string nameday = string.Empty;
            string namedayMonth = string.Empty;
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    {
                        namedayMonth = month switch
                        {
                            1 => "1. Licht",
                            2 => "1. Schatten",
                            3 => "2. Licht",
                            4 => "2. Schatten",
                            5 => "3. Licht",
                            6 => "3. Schatten",
                            7 => "4. Licht",
                            8 => "4. Schatten",
                            9 => "5. Licht",
                            10 => "5. Schatten",
                            11 => "6. Licht",
                            12 => "6. Schatten",
                            _ => "",
                        };

                        nameday = $"{day}. Sonne im {namedayMonth}mond";
                    }
                    break;
                case ClientLanguage.English:
                    {
                        string namedayDay = day switch
                        {
                            1 => "1st",
                            2 => "2nd",
                            3 => "3rd",
                            4 => "4th",
                            5 => "5th",
                            6 => "6th",
                            7 => "7th",
                            8 => "8th",
                            9 => "9th",
                            10 => "10th",
                            11 => "11th",
                            12 => "12th",
                            13 => "13th",
                            14 => "14th",
                            15 => "15th",
                            16 => "16th",
                            17 => "17th",
                            18 => "18th",
                            19 => "19th",
                            20 => "20th",
                            21 => "21st",
                            22 => "22nd",
                            23 => "23rd",
                            24 => "24th",
                            25 => "25th",
                            26 => "26th",
                            27 => "27th",
                            28 => "28th",
                            29 => "29th",
                            30 => "30th",
                            31 => "31st",
                            32 => "32nd",
                            _ => "",
                        };

                        namedayMonth = month switch
                        {
                            1 => "1st Astral",
                            2 => "1st Umbral",
                            3 => "2nd Astral",
                            4 => "2nd Umbral",
                            5 => "3rd Astral",
                            6 => "3rd Umbral",
                            7 => "4th Astral",
                            8 => "4th Umbral",
                            9 => "5th Astral",
                            10 => "5th Umbral",
                            11 => "6th Astral",
                            12 => "6th Umbral",
                            _ => "",
                        };
                        nameday = $"{namedayDay} Sun of the {namedayMonth} Moon";
                        break;
                    }
                case ClientLanguage.French:
                    {

                        namedayMonth = month switch
                        {
                            1 => "1re lune astrale",
                            2 => "1re lune ombrale",
                            3 => "2e lune astrale",
                            4 => "2e lune ombrale",
                            5 => "3e lune astrale",
                            6 => "3e lune ombrale",
                            7 => "4e lune astrale",
                            8 => "4e lune ombrale",
                            9 => "5e lune astrale",
                            10 => "5e lune ombrale",
                            11 => "6e lune astrale",
                            12 => "6e lune ombrale",
                            _ => "",
                        };
                        nameday = $"{((day == 1) ? "er" : "e")} soleil de la {namedayMonth}";
                    }
                    break;

                case ClientLanguage.Japanese:
                    {
                        namedayMonth = month switch
                        {
                            1 => "星1月",
                            2 => "霊1月",
                            3 => "星2月",
                            4 => "霊2月",
                            5 => "星3月",
                            6 => "霊3月",
                            7 => "星4月",
                            8 => "霊4月",
                            9 => "星5月",
                            10 => "霊5月",
                            11 => "星6月",
                            12 => "霊6月",
                            _ => "",
                        };

                        nameday = $"{namedayMonth} {day}日";
                    }
                    break;
            }

            return nameday;
        }

        public static string GetGrandCompany(ClientLanguage currentLocale, int id)
        {
            ExcelSheet<GrandCompany>? dgc = Plugin.DataManager.GetExcelSheet<GrandCompany>(currentLocale);
            GrandCompany? lumina = dgc?.GetRow((uint)id);
            return lumina != null ? lumina.Value.Name.ExtractText() : string.Empty;
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
                            GCRankLimsaMaleText? lumina = dgcrlmt?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Value.Singular.ExtractText();
                        }
                        else
                        {
                            ExcelSheet<GCRankLimsaFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankLimsaFemaleText>(currentLocale);
                            GCRankLimsaFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Value.Singular.ExtractText();
                        }

                        return string.Empty;
                    }
                case 2:
                    {
                        if (gender == 0)
                        {
                            ExcelSheet<GCRankUldahMaleText>? dgcrlmt =
                                Plugin.DataManager.GetExcelSheet<GCRankUldahMaleText>(currentLocale);
                            GCRankUldahMaleText? lumina = dgcrlmt?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Value.Singular.ExtractText();
                        }
                        else
                        {
                            ExcelSheet<GCRankUldahFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankUldahFemaleText>(currentLocale);
                            GCRankUldahFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Value.Singular.ExtractText();
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
                                return lumina.Value.Singular.ExtractText();
                        }
                        else
                        {
                            ExcelSheet<GCRankGridaniaFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankGridaniaFemaleText>(currentLocale);
                            GCRankGridaniaFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Value.Singular.ExtractText();
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
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            Item? lumina = ditm?.GetRow(id);
            return lumina;
        }

        public static EventItem? GetEventItemFromId(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<EventItem>? deitm = Plugin.DataManager.GetExcelSheet<EventItem>(currentLocale);
            EventItem? lumina = deitm?.GetRow(id);
            return lumina;
        }

        public static IEnumerable<Item>? GetItemsFromName(ClientLanguage currentLocale, string name)
        {
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            IEnumerable<Item>? items = ditm?.Where(i =>
                i.Name.ExtractText().Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return items;
        }

        public static IEnumerable<Item>? GetItemsFromItemAction(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            IEnumerable<Item>? items = ditm?.Where(i =>
                i.ItemAction.RowId == id);
            return items;
        }

        public static Item? GetItemFromName(ClientLanguage currentLocale, string name)
        {
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            Item? item = ditm?.FirstOrNull(i =>
                i.Name.ExtractText().Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return item;
        }

        public static ItemLevel? GetItemLevelFromId(uint id)
        {
            ExcelSheet<ItemLevel> dilvl = Plugin.DataManager.GetExcelSheet<ItemLevel>(ClientLanguage.English);
            ItemLevel? lumina = dilvl.GetRow(id);
            return lumina;
        }

        public static Stain? GetStainFromId(uint id, ClientLanguage clientLanguage)
        {
            ExcelSheet<Stain>? ds = Plugin.DataManager.GetExcelSheet<Stain>(clientLanguage);
            Stain? lumina = ds?.GetRow(id);
            return lumina;
        }

        private static uint GetJobIcon(uint jobId)
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
                41 => 062041,
                42 => 062042,
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
                41 => 062141,
                42 => 062142,
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
                41 => 062266,
                42 => 062267,
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

        public static (Dictionary<uint,PvPSeries>, uint) GetPvPSeries(ClientLanguage currentLocale)
        {
            Dictionary<uint, PvPSeries>? series = new();
            uint lastId = 0;
            ExcelSheet<PvPSeries>? dct = Plugin.DataManager.GetExcelSheet<PvPSeries>(currentLocale);
            using IEnumerator<PvPSeries>? seriesEnumerator = dct?.GetEnumerator();
            if (seriesEnumerator is null) return (series, 0);
            while (seriesEnumerator.MoveNext())
            {
                PvPSeries s = seriesEnumerator.Current;
                series.Add(s.RowId, s);
                lastId = s.RowId;
            }

            return (series, lastId);
        }
        public static Dictionary<uint,PvPSeriesLevel> GetPvPSeriesLevel(ClientLanguage currentLocale)
        {
            Dictionary<uint, PvPSeriesLevel>? seriesLevel = new();
            ExcelSheet<PvPSeriesLevel>? dct = Plugin.DataManager.GetExcelSheet<PvPSeriesLevel>(currentLocale);
            using IEnumerator<PvPSeriesLevel>? seriesLevelEnumerator = dct?.GetEnumerator();
            if (seriesLevelEnumerator is null) return seriesLevel;
            while (seriesLevelEnumerator.MoveNext())
            {
                PvPSeriesLevel s = seriesLevelEnumerator.Current;
                seriesLevel.Add(s.RowId, s);
            }

            return seriesLevel;
        }
        private static Lumina.Excel.Sheets.PvPRankTransient? GetPvPRankTransient(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Lumina.Excel.Sheets.PvPRankTransient>? dct = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.PvPRankTransient>(currentLocale);
            Lumina.Excel.Sheets.PvPRankTransient? lumina = dct?.GetRow(id);
            return lumina;
        }
        public static List<Models.PvPRank>? GetPvPRanks(ClientLanguage currentLocale)
        {
            List<Models.PvPRank> returnedModelsPvPRanks = [];
            ExcelSheet<Lumina.Excel.Sheets.PvPRank>? dm = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.PvPRank>(currentLocale);
            using IEnumerator<Lumina.Excel.Sheets.PvPRank>? pvPRankEnumerator = dm?.GetEnumerator();
            if (pvPRankEnumerator is null) return null;
            while (pvPRankEnumerator.MoveNext())
            {
                Lumina.Excel.Sheets.PvPRank rank = pvPRankEnumerator.Current;
                Models.PvPRank p = new() { Id = rank.RowId, ExpRequired = rank.ExpRequired, Transients = new PvPRankTransient() };
                Lumina.Excel.Sheets.PvPRankTransient? pt = GetPvPRankTransient(currentLocale, rank.RowId);
                if (pt is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        p.Transients.GermanTransients.Add(0,pt.Value.Unknown0.ExtractText());
                        p.Transients.GermanTransients.Add(1,pt.Value.Unknown1.ExtractText());
                        p.Transients.GermanTransients.Add(2,pt.Value.Unknown2.ExtractText());
                        p.Transients.GermanTransients.Add(3,pt.Value.Unknown3.ExtractText());
                        p.Transients.GermanTransients.Add(4,pt.Value.Unknown4.ExtractText());
                        p.Transients.GermanTransients.Add(5,pt.Value.Unknown5.ExtractText());
                        break;
                    case ClientLanguage.English:
                        p.Transients.Id = pt.Value.RowId;
                        p.Transients.EnglishTransients.Add(0, pt.Value.Unknown0.ExtractText());
                        p.Transients.EnglishTransients.Add(1, pt.Value.Unknown1.ExtractText());
                        p.Transients.EnglishTransients.Add(2, pt.Value.Unknown2.ExtractText());
                        p.Transients.EnglishTransients.Add(3, pt.Value.Unknown3.ExtractText());
                        p.Transients.EnglishTransients.Add(4, pt.Value.Unknown4.ExtractText());
                        p.Transients.EnglishTransients.Add(5, pt.Value.Unknown5.ExtractText());
                        break;
                    case ClientLanguage.French:
                        p.Transients.FrenchTransients.Add(0, pt.Value.Unknown0.ExtractText());
                        p.Transients.FrenchTransients.Add(1, pt.Value.Unknown1.ExtractText());
                        p.Transients.FrenchTransients.Add(2, pt.Value.Unknown2.ExtractText());
                        p.Transients.FrenchTransients.Add(3, pt.Value.Unknown3.ExtractText());
                        p.Transients.FrenchTransients.Add(4, pt.Value.Unknown4.ExtractText());
                        p.Transients.FrenchTransients.Add(5, pt.Value.Unknown5.ExtractText());
                        break;
                    case ClientLanguage.Japanese:
                        p.Transients.JapaneseTransients.Add(0, pt.Value.Unknown0.ExtractText());
                        p.Transients.JapaneseTransients.Add(1, pt.Value.Unknown1.ExtractText());
                        p.Transients.JapaneseTransients.Add(2, pt.Value.Unknown2.ExtractText());
                        p.Transients.JapaneseTransients.Add(3, pt.Value.Unknown3.ExtractText());
                        p.Transients.JapaneseTransients.Add(4, pt.Value.Unknown4.ExtractText());
                        p.Transients.JapaneseTransients.Add(5, pt.Value.Unknown5.ExtractText());
                        break;
                }
                returnedModelsPvPRanks.Add(p);
            }

            return returnedModelsPvPRanks;
        }

        public static void DrawPvPRankBar(uint left, uint right, int width, Vector4 fgColor, Vector4 bgColor)
        {
            float progress = (float)left /right;

            ImGui.PushStyleColor(ImGuiCol.FrameBg, bgColor);
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, fgColor);
            ImGui.ProgressBar(progress, new Vector2(width, 10), "");
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
        }

        public static BeastReputationRank? GetBeastReputationRank(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BeastReputationRank> btm = Plugin.DataManager.GetExcelSheet<BeastReputationRank>(currentLocale);
            BeastReputationRank? lumina = btm.GetRow(id);
            return lumina;
        }

        public static List<BeastReputationRank>? GetBeastReputationRanks(ClientLanguage currentLocale)
        {
            List<BeastReputationRank> returnedbtsIds = [];
            ExcelSheet<BeastReputationRank>? btm = Plugin.DataManager.GetExcelSheet<BeastReputationRank>(currentLocale);
            using IEnumerator<BeastReputationRank>? btEnumerator = btm?.GetEnumerator();
            if (btEnumerator is null) return null;
            while (btEnumerator.MoveNext())
            {
                BeastReputationRank bt = btEnumerator.Current;
                returnedbtsIds.Add(bt);
            }
            return returnedbtsIds;
        }

        public static BeastTribe? GetBeastTribe(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BeastTribe>? dc = Plugin.DataManager.GetExcelSheet<BeastTribe>(currentLocale);
            BeastTribe? lumina = dc?.GetRow(id);
            return lumina;
        }

        public static List<BeastTribes>? GetAllBeastTribes(ClientLanguage currentLocale)
        {
            List<BeastTribes> returnedbtsIds = [];
            ExcelSheet<BeastTribe>? btm = Plugin.DataManager.GetExcelSheet<BeastTribe>(currentLocale);
            using IEnumerator<BeastTribe>? btEnumerator = btm?.GetEnumerator();
            if (btEnumerator is null) return null;
            while (btEnumerator.MoveNext())
            {
                BeastTribe bt = btEnumerator.Current;
                if (bt.Name.IsEmpty) continue;
                if (bt.Icon == 0) continue;
                BeastTribes b = new() { Id = bt.RowId, Icon = bt.Icon, MaxRank = bt.MaxRank, DisplayOrder = bt.DisplayOrder };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        b.GermanName = bt.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        b.EnglishName = bt.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        b.FrenchName = bt.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        b.JapaneseName = bt.Name.ExtractText();
                        break;
                }

                returnedbtsIds.Add(b);
            }

            return returnedbtsIds;
        }

        public static string GetTribalNameFromId(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BeastTribe>? dbt = Plugin.DataManager.GetExcelSheet<BeastTribe>(currentLocale);
            BeastTribe? lumina = dbt?.GetRow(id);
            return lumina != null ? lumina.Value.Name.ExtractText() : string.Empty;
        }
        public static string GetTribalCurrencyFromId(ClientLanguage currentLocale, uint id)
        {
            //Plugin.Log.Debug($"GetItemNameFromId : {id}");
            ExcelSheet<BeastTribe>? dbt = Plugin.DataManager.GetExcelSheet<BeastTribe>(currentLocale);
            BeastTribe? lumina = dbt?.GetRow(id);
            return lumina != null ? lumina.Value.Name.ExtractText() : string.Empty;
        }

        public static Materia? GetMateria(uint id)
        {
            ExcelSheet<Materia>? dc = Plugin.DataManager.GetExcelSheet<Materia>(ClientLanguage.English);
            Materia? lumina = dc?.GetRow(id);
            return lumina;
        }

        public static List<Models.Materia>? GetAllMaterias()
        {
            List<Models.Materia> returnedMaterias = [];
            ExcelSheet<Materia>? dm = Plugin.DataManager.GetExcelSheet<Materia>(ClientLanguage.English);
            using IEnumerator<Materia>? mEnumerator = dm?.GetEnumerator();
            if (mEnumerator is null) return null;
            while (mEnumerator.MoveNext())
            {
                Materia m = mEnumerator.Current;
                if (!m.Item[0].IsValid) continue;
                Models.Materia materia = new() { Id = m.RowId, BaseParamId = m.BaseParam.RowId, Grades = new uint[16], Values = new short[16] };
                for (int i = 0; i < 16;i++)
                {
                    materia.Grades[i] = m.Item[i].RowId;
                    materia.Values[i] = m.Value[i];
                }

                returnedMaterias.Add(materia);
            }

            return returnedMaterias;
        }

        private static uint ConvertColorToAbgr(uint rgbColor)
        {
            byte red = (byte)((rgbColor >> 16) & 0xFF);
            byte green = (byte)((rgbColor >> 8) & 0xFF);
            byte blue = (byte)(rgbColor & 0xFF);

            // Adding alpha channel (fully opaque)
            return (uint)((0xFF << 24) | (blue << 16) | (green << 8) | red);
        }

        private static Vector4 ConvertColorToVector4(uint color)
        {
            byte r = (byte)(color >> 24);
            byte g = (byte)(color >> 16);
            byte b = (byte)(color >> 8);
            byte a = (byte)color;

            return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        private static Vector4 StainToVector4(uint stainColor)
        {
            const float s = 1.0f / 255.0f;

            return new Vector4()
            {
                X = ((stainColor >> 16) & 0xFF) * s,
                Y = ((stainColor >> 8) & 0xFF) * s,
                Z = ((stainColor >> 0) & 0xFF) * s,
                W = ((stainColor >> 24) & 0xFF) * s
            };
        }

        public static void DrawReputationProgressBar(ClientLanguage currentLocale, GlobalCache globalCache, uint exp, bool isMax, uint reputationLevel, bool isAllied)
        {
            BeastReputationRank? brr = globalCache.BeastTribesStorage.GetRank(currentLocale, reputationLevel);
            if (brr == null) return;

            float progress = (float)exp / brr.Value.RequiredReputation;
            ImGui.ProgressBar(progress, new Vector2(550, 10), "");

            using var charactersJobsJobLine = ImRaii.Table("###DrawReputationProgressBar#ReputationLine", 3);
            if (!charactersJobsJobLine) return;
            ImGui.TableSetupColumn("###DrawReputationProgressBar#ReputationLine#Level", ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###DrawReputationProgressBar#ReputationLine#Empty", ImGuiTableColumnFlags.WidthFixed,
                brr.Value.RequiredReputation == 0 ? 190 : 150);
            ImGui.TableSetupColumn("###DrawReputationProgressBar#ReputationLine#Exp", ImGuiTableColumnFlags.WidthFixed,
                brr.Value.RequiredReputation == 0 ? 100 : 150);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            UIColor? c = brr.Value.Color.ValueNullable;
            if (c is not null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ConvertColorToVector4(c.Value.Dark));
                ImGui.TextUnformatted(isAllied ? $"{reputationLevel + 1}. {brr.Value.Name}" : $"{ reputationLevel}. { brr.Value.AlliedNames}");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.TextUnformatted($"{reputationLevel}. {brr.Value.Name}");
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted($"{exp}/{brr.Value.RequiredReputation}");
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

        private static string GetSlotName(ClientLanguage currentLocale, GlobalCache globalCache, short id)
        {
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
            int middleWidth, int middleHeigth, bool retainer = false, int maxLevel = 0, ushort[]? currentFacewear = null)
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
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.HEAD,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11525), new Vector2(40, 40),
                    defaultTextures[GearSlot.HEAD], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.BODY,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11526), new Vector2(40, 40),
                    defaultTextures[GearSlot.BODY], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.HANDS,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11527), new Vector2(40, 40),
                    defaultTextures[GearSlot.HANDS], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.LEGS,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11528), new Vector2(40, 40),
                    defaultTextures[GearSlot.LEGS], defaultTextures[GearSlot.EMPTY]);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.FEET,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 11529), new Vector2(40, 40),
                    defaultTextures[GearSlot.FEET], defaultTextures[GearSlot.EMPTY]);
                if (!retainer && currentFacewear is not null)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawFacewearPiece(currentLocale, ref globalCache, currentFacewear[0],
                        globalCache.AddonStorage.LoadAddonString(currentLocale, 16050), new Vector2(40, 40),
                        defaultTextures[GearSlot.FACEWEAR], defaultTextures[GearSlot.EMPTY]);
                }
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

            if (!retainer)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.SOUL_CRYSTAL,
                    globalCache.AddonStorage.LoadAddonString(currentLocale, 12238), new Vector2(40, 40),
                    defaultTextures[GearSlot.SOUL_CRYSTAL], defaultTextures[GearSlot.EMPTY]);
            }
            else
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawRetainerJob(currentLocale, ref globalCache, job);
            }
        }

        private static void DrawRetainerJob(ClientLanguage currentLocale, ref GlobalCache globalCache, uint job)
        {
            IDalamudTextureWrap? iconTexture = globalCache.IconStorage.LoadRetainerJobIconTexture();
            if (iconTexture == null)
            {
                Plugin.Log.Debug($"{iconTexture is null}");
                return;
            }
            DrawRetainerJobIconFromTexture(ref iconTexture, (Models.ClassJob)job, new Vector2(44, 44));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                string str = globalCache.AddonStorage.LoadAddonString(currentLocale, 379).Split(':')[0];
                ImGui.TextUnformatted($"{str}: {globalCache.JobStorage.GetName(currentLocale, job)}");
                ImGui.EndTooltip();
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
            ImGui.Image(texture.Handle, size, uv0, uv1);
        }

        private static void DrawRetainerJobIconFromTexture(ref IDalamudTextureWrap texture, Models.ClassJob job,
            Vector2 size)
        {
            (Vector2 uv0, Vector2 uv1) = job switch
            {
                Models.ClassJob.PLD => GetTextureCoordinate(texture.Size, 0, 150, 86, 90),
                Models.ClassJob.MNK => GetTextureCoordinate(texture.Size, 90, 150, 86, 90),
                Models.ClassJob.WAR => GetTextureCoordinate(texture.Size, 180, 150, 86, 86),
                Models.ClassJob.DRG => GetTextureCoordinate(texture.Size, 0, 236, 86, 86),
                Models.ClassJob.BRD => GetTextureCoordinate(texture.Size, 90, 236, 86, 86),
                Models.ClassJob.WHM => GetTextureCoordinate(texture.Size, 180, 236, 86, 86),
                Models.ClassJob.BLM => GetTextureCoordinate(texture.Size, 0, 322, 86, 86),
                Models.ClassJob.SMN => GetTextureCoordinate(texture.Size, 90, 322, 86, 86),
                Models.ClassJob.SCH => GetTextureCoordinate(texture.Size, 180, 322, 86, 86),
                Models.ClassJob.NIN => GetTextureCoordinate(texture.Size, 0, 408, 86, 86),
                Models.ClassJob.MCH => GetTextureCoordinate(texture.Size, 90, 408, 86, 86),
                Models.ClassJob.DRK => GetTextureCoordinate(texture.Size, 180, 408, 86, 86),
                Models.ClassJob.AST => GetTextureCoordinate(texture.Size, 0, 494, 86, 86),
                Models.ClassJob.SAM => GetTextureCoordinate(texture.Size, 90, 494, 86, 86),
                Models.ClassJob.RDM => GetTextureCoordinate(texture.Size, 180, 494, 86, 86),
                Models.ClassJob.BLU => GetTextureCoordinate(texture.Size, 0, 580, 86, 86),
                Models.ClassJob.GNB => GetTextureCoordinate(texture.Size, 90, 580, 86, 86),
                Models.ClassJob.DNC => GetTextureCoordinate(texture.Size, 180, 580, 86, 86),
                Models.ClassJob.SGE => GetTextureCoordinate(texture.Size, 0, 666, 86, 90),
                Models.ClassJob.RPR => GetTextureCoordinate(texture.Size, 90, 666, 86, 90),
                Models.ClassJob.VPR => GetTextureCoordinate(texture.Size, 180, 666, 86, 90),
                Models.ClassJob.PCT => GetTextureCoordinate(texture.Size, 0, 752, 86, 86),
                _ => GetTextureCoordinate(texture.Size, 0, 0, 0, 0)
            };
            ImGui.Image(texture.Handle, size, uv0, uv1);
        }

         private static void DrawGearPiece(ClientLanguage currentLocale, ref GlobalCache globalCache, List<Gear> gear,
            GearSlot slot, string tooltip, Vector2 iconSize,
            IDalamudTextureWrap? fallbackTexture, IDalamudTextureWrap? emptySlot)
        {
            if (fallbackTexture is null || emptySlot is null) return;
            Gear? foundGear = gear.FirstOrDefault(g => g.Slot == (short)slot);
            if (foundGear == null || foundGear.ItemId == 0)
            {
                System.Numerics.Vector2 p = ImGui.GetCursorPos();
                ImGui.Image(emptySlot.Handle, new Vector2(42, 42));
                ImGui.SetCursorPos(new Vector2(p.X + 1, p.Y + 1));
                ImGui.Image(fallbackTexture.Handle, new Vector2(40, 40));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(tooltip);
                    ImGui.EndTooltip();
                }
            }
            else
            {
                ItemItemLevel? itl = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, foundGear.ItemId);
                if (itl == null) return;
                Item? i = itl.Item;
                if(i == null) return;
                DrawIcon(globalCache.IconStorage.LoadIcon(i.Value.Icon, foundGear.HQ), iconSize);
                if (ImGui.IsItemHovered())
                {
                    DrawGearTooltip(currentLocale, ref globalCache, foundGear, itl);
                }
            }
        }

        private static void DrawFacewearPiece(ClientLanguage currentLocale, ref GlobalCache globalCache, ushort id, string tooltip, Vector2 iconSize,
            IDalamudTextureWrap? fallbackTexture, IDalamudTextureWrap? emptySlot)
        {
            if (fallbackTexture is null || emptySlot is null) return;
            if (id == 0)
            {
                System.Numerics.Vector2 p = ImGui.GetCursorPos();
                ImGui.Image(emptySlot.Handle, new Vector2(42, 42));
                ImGui.SetCursorPos(new Vector2(p.X + 1, p.Y + 1));
                ImGui.Image(fallbackTexture.Handle, new Vector2(40, 40));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(tooltip);
                    ImGui.EndTooltip();
                }
            }
            else
            {
                Models.Glasses? g = globalCache.GlassesStorage.GetGlasses(currentLocale, id);
                if (g == null) return;
                DrawIcon(globalCache.IconStorage.LoadIcon(g.Icon), iconSize);
                if (ImGui.IsItemHovered())
                {
                    DrawGlassesTooltip(currentLocale, ref globalCache, g);
                }
            }
        }

        public static void DrawGearTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Gear item,
            ItemItemLevel itm)
        {
            Item? dbItem = itm.Item;
            if (dbItem == null) return;
            ItemLevel? ilvl = itm.ItemLevel;
            if (ilvl == null) return;

            ImGui.BeginTooltip();

            if (dbItem.Value.IsUnique || dbItem.Value.IsUntradable)
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
                if (dbItem.Value.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (dbItem.Value.IsUntradable)
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
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Value.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Value.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
                if (dbItem.Value.IsGlamorous)
                {
                    Item? glamour = globalCache.ItemStorage.LoadItem(currentLocale, item.GlamourID);
                    if (glamour != null)
                    {
                        ImGui.TextUnformatted($"{(char)SeIconChar.Glamoured} {glamour.Value.Name}");
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
                ImGui.TextUnformatted($"{dbItem.Value.DefensePhys}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Value.DefenseMag}");
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13775)} {ilvl.Value.RowId}"); // Item Level
            ImGui.Separator();
            ImGui.TextUnformatted($"{GetClassJobCategoryFromId(currentLocale, dbItem.Value.ClassJobCategory.ValueNullable?.RowId)}");
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1034)} {dbItem.Value.LevelEquip}");
            ImGui.Separator();
            if (!dbItem.Value.IsAdvancedMeldingPermitted)
            {
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 4655)}"); // Advanced Melding Forbidden
            }

            if (item.Stain > 0)
            {
                ImGui.Separator();
                (string, uint) dye = globalCache.StainStorage.LoadStainWithColor(currentLocale, item.Stain);
                if (!string.IsNullOrEmpty(dye.Item1))
                {
                    ImGui.ColorButton($"##Gear_{item.ItemId}#Dye#1", StainToVector4(dye.Item2),
                        ImGuiColorEditFlags.None, new Vector2(16, 16));
                    ImGui.SameLine();
                    /*Vector4 color = item.Stain == 102 ? new Vector4(1,1,1,1) : ConvertColorToVector4(ConvertColorToAbgr(dye.Item2));
                    ImGui.PushStyleColor(ImGuiCol.Text, color);*/
                    ImGui.TextUnformatted(dye.Item1);
                    //ImGui.PopStyleColor();
                }
            }
            if (item.Stain2 > 0)
            {
                (string, uint) dye2 = globalCache.StainStorage.LoadStainWithColor(currentLocale, item.Stain2);
                if (!string.IsNullOrEmpty(dye2.Item1))
                {
                    ImGui.ColorButton($"##Gear_{item.ItemId}#Dye#2", StainToVector4(dye2.Item2),
                        ImGuiColorEditFlags.None, new Vector2(16, 16));
                    ImGui.SameLine();
                    /*Vector4 color = item.Stain == 102 ? new Vector4(1, 1, 1, 1) : ConvertColorToVector4(ConvertColorToAbgr(dye2.Item2));
                    ImGui.PushStyleColor(ImGuiCol.Text, color);*/
                    ImGui.TextUnformatted(dye2.Item1);
                    //ImGui.PopStyleColor();
                }
            }

            ImGui.Separator();
            using (var drawItemTooltipItemBonuses = ImRaii.Table($"###DrawItemTooltip#Item_{item.ItemId}#Bonuses", 3))
            {
                if (!drawItemTooltipItemBonuses) return;
                //Todo: Conditional attributes since not every item will have the same
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Bonuses#StrengthCrit",
                    ImGuiTableColumnFlags.WidthFixed, 150);
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

            if (dbItem.Value.MateriaSlotCount > 0)
            {
                ImGui.Separator();
                ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 491)}"); // Materia

                for (int i = 0; i < 5; i++)
                {
                    bool isOver = i+1 > dbItem.Value.MateriaSlotCount;
                    if (item.Materia[i] == 0)
                    {
                        if (isOver) continue;
                        DrawMateriaEmptyIcon(globalCache);
                    }
                    else
                    {
                        Models.Materia? materia = globalCache.MateriaStorage.GetMateria(item.Materia[i]);
                        if (materia is null) continue;
                        uint? materiaItemId = materia.Grades[item.MateriaGrade[i]];
                        if (materiaItemId is 0) continue;
                        Item? mItem = globalCache.ItemStorage.LoadItem(currentLocale, materiaItemId.Value);
                        if (mItem is null) continue;
                        short value = materia.Values[item.MateriaGrade[i]];
                        if (value is 0) continue;
                        Models.BaseParam? param =
                            globalCache.BaseParamStorage.GetBaseParam(currentLocale, materia.BaseParamId);
                        if (param is null) continue;
                        string paramName = currentLocale switch
                        {
                            ClientLanguage.German => param?.GermanName,
                            ClientLanguage.English => param?.EnglishName,
                            ClientLanguage.French => param?.FrenchName,
                            ClientLanguage.Japanese => param?.JapaneseName,
                            _ => param?.EnglishName
                        } ?? string.Empty;
                        DrawMateriaIcon(globalCache, item.MateriaGrade[i], isOver);
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{mItem.Value.Name}");
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{paramName} {value}");
                    }
                }

            }

            ImGui.Separator();
            uint jobId = dbItem.Value.ClassJobRepair.RowId;
            DrawIcon(globalCache.IconStorage.LoadIcon(GetJobIconWithCornerSmall(jobId)), new Vector2(24,24));
            ImGui.SameLine();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}"); // Crafting & Repairs
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 498)} : {(item.Condition / 300f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 499)} : {(item.Spiritbond / 100f).ToString(false ? "F2" : "0.##").Truncate(2) + "%"}");
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 500)} : {globalCache.JobStorage.GetName(currentLocale, jobId)}"); //Repair Level
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 518)} : {GetItemRepairResource(currentLocale, dbItem.Value.ItemRepair.RowId)}"); //Materials
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 995)} : "); //Quick Repairs
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 993)} : "); //Materia Melding
            ImGui.TextUnformatted($"{GetExtractableString(currentLocale, globalCache, dbItem.Value)}");
            ImGui.TextUnformatted($"{GetSellableString(currentLocale, globalCache, dbItem.Value, item)}"); //Materia Melding
            if (item.CrafterContentID > 0)
                ImGui.TextUnformatted("Crafted");

            ImGui.EndTooltip();
        }

        private static void DrawMateriaEmptyIcon(GlobalCache globalCache)
        {
            IDalamudTextureWrap? itemDetailsTexture = globalCache.IconStorage.LoadItemDetailsTexture(2);
            if (itemDetailsTexture == null)
            {
                Plugin.Log.Debug("itemDetailsTexture is null");
                return;
            }

            DrawIcon(itemDetailsTexture, new Vector2(20, 20));
        }
        private static void DrawMateriaIcon(GlobalCache globalCache, byte grade, bool isOver = false)
        {
            int index;
            if (!isOver)
            {
                index = grade switch
                {
                    1 or 2 or 3 or 4 => 3,
                    5 => 21,
                    6 => 23,
                    7 => 25,
                    8 => 27,
                    9 => 29,
                    10 => 31,
                    11 => 33,
                    12 => 35,
                    _ => 2
                };
            }
            else
            {
                index = grade switch
                {
                    1 or 2 or 3 or 4 => 17,
                    5 => 22,
                    6 => 24,
                    7 => 26,
                    8 => 28,
                    9 => 30,
                    10 => 32,
                    11 => 34,
                    12 => 36,
                    _ => 2
                };
            }

            IDalamudTextureWrap? itemDetailsTexture = globalCache.IconStorage.LoadItemDetailsTexture(index);
            if (itemDetailsTexture == null)
            {
                Plugin.Log.Debug("itemDetailsTexture is null");
                return;
            }

            DrawIcon(itemDetailsTexture, new Vector2(30, 30));
        }

        public static void DrawItemTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Item item)
        {
            ImGui.BeginTooltip();

            if (item.IsUnique || item.IsUntradable)
            {
                using var drawItemTooltipItemUnique = ImRaii.Table($"##DrawItemTooltip#Item_{item.RowId}#Unique", 3);
                if (!drawItemTooltipItemUnique) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.RowId}#Unique#IsUnique",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.RowId}#Unique#IsUntradable",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.RowId}#Unique#IsBinding",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(1);
                if (item.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (item.IsUntradable)
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

            using (var drawItemTooltipItemNameIcon = ImRaii.Table($"##DrawItemTooltip#Item_{item.RowId}#NameIcon", 2))
            {
                if (!drawItemTooltipItemNameIcon) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.RowId}#NameIcon#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.RowId}#NameIcon#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
  
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{item.Name}");
            }

            ImGui.TextUnformatted($"{item.ItemUICategory.ValueNullable?.Name}");

            ImGui.EndTooltip();
        }

        public static void DrawInventoryItemTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Inventory item, bool armoire = false)
        {
            ItemItemLevel? itm = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, item.ItemId);
            Item? dbItem = itm?.Item;
            if (dbItem == null) return;
            /*ItemLevel? ilvl = itm?.ItemLevel;
            Plugin.Log.Debug("ilvl is not null");*/

            ImGui.BeginTooltip();

            if (dbItem.Value.IsUnique || dbItem.Value.IsUntradable)
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
                if (dbItem.Value.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (dbItem.Value.IsUntradable)
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

            int col = (armoire) ? 4 : 2;
            using (var drawItemTooltipItemNameIcon = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}#NameIcon", col))
            {
                if (!drawItemTooltipItemNameIcon) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#NameIcon#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#NameIcon#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                if (armoire)
                {
                    ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#NameIcon#Empty",
                        ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#NameIcon#Armoire",
                        ImGuiTableColumnFlags.WidthStretch);
                }

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Value.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Value.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
                if (armoire)
                {
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TableSetColumnIndex(3);
                    DrawIcon(globalCache.IconStorage.LoadIcon(066460), new Vector2(16, 16));
                    ImGui.SameLine();
                    ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 11991));
                }
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
                ImGui.TextUnformatted($"{dbItem.Value.ItemUICategory.ValueNullable?.Name}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{item.Quantity}/{dbItem.Value.StackSize} (Total: {item.Quantity})");
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
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Value.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Value.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
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

            if (dbItem.Value.IsUnique || dbItem.Value.IsUntradable)
            {
                using var drawCrystalTooltipItemUnique =
                    ImRaii.Table($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Unique", 3);
                if (!drawCrystalTooltipItemUnique) return;
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Unique#IsUnique",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Unique#IsUntradable",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"##DrawItem#DrawCrystalTooltipTooltip#Item_{dbItem.Value.RowId}#Unique#IsBinding",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("");
                ImGui.TableSetColumnIndex(1);
                if (dbItem.Value.IsUnique)
                {
                    ImGui.TextUnformatted(
                        $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 494)}"); // Unique
                }

                if (dbItem.Value.IsUntradable)
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

            using (var drawCrystalTooltipItem = ImRaii.Table($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}", 2))
            {
                if (!drawCrystalTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Value.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Value.Name}");
            }

            using (var drawCrystalTooltipItemCategory =
                   ImRaii.Table($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Category", 3))
            {
                if (!drawCrystalTooltipItemCategory) return;
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Category#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Category#Name",
                    ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn($"###DrawCrystalTooltip#Item_{dbItem.Value.RowId}#Category#Empty",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{dbItem.Value.ItemUICategory.ValueNullable?.Name}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{amount}/99 (Total: {amount})");
            }

            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 497)}"); // Crafting & Repairs

            ImGui.EndTooltip();
        }

        public static void DrawGlamourDresserTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, GlamourItem item,
            ItemItemLevel itm, bool isInASet, IDalamudTextureWrap? miragePrismIcon, Vector2 miragePrismBoxSetIconUv0, Vector2 miragePrismBoxSetIconUv1)
        {
            Item? dbItem = itm.Item;
            if (dbItem == null) return;
            ItemLevel? ilvl = itm.ItemLevel;
            if (ilvl == null) return;

            bool hq = item.Flags.HasFlag(InventoryItem.ItemFlags.HighQuality);

            ImGui.BeginTooltip();

            using (var drawItemTooltipItem = ImRaii.Table($"##DrawItemTooltip#Item_{item.ItemId}", 3))
            {
                if (!drawItemTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Icon", ImGuiTableColumnFlags.WidthFixed,
                    55);
                ImGui.TableSetupColumn($"###DrawItemTooltip#Item_{item.ItemId}#Name", ImGuiTableColumnFlags.WidthFixed,
                    305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Vector2 p = ImGui.GetCursorPos();
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Value.Icon, hq), new Vector2(40, 40));
                if (isInASet && miragePrismIcon is not null)
                {
                    ImGui.SetCursorPos(p with { X = p.X + 25 });
                    ImGui.Image(miragePrismIcon.Handle, new Vector2(16, 16), miragePrismBoxSetIconUv0, miragePrismBoxSetIconUv1);
                    ImGui.SetCursorPos(p);
                }
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Value.Name} {(hq ? (char)SeIconChar.HighQuality : "")}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(isInASet
                    ? $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 15624)}"
                    : $"{GetSlotName(currentLocale, globalCache, item.Slot)}");
            }

            if (!isInASet)
            {
                ImGui.Separator();
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 13775)} {ilvl.Value.RowId}"); // Item Level
                ImGui.Separator();
                ImGui.TextUnformatted(
                    $"{GetClassJobCategoryFromId(currentLocale, dbItem.Value.ClassJobCategory.ValueNullable?.RowId)}");
                ImGui.TextUnformatted(
                    $"{globalCache.AddonStorage.LoadAddonString(currentLocale, 1034)} {dbItem.Value.LevelEquip}");
                ImGui.Separator();

                if (item.Stain0 > 0)
                {
                    ImGui.Separator();
                    (string, uint) dye = globalCache.StainStorage.LoadStainWithColor(currentLocale, item.Stain0);
                    if (!string.IsNullOrEmpty(dye.Item1))
                    {
                        ImGui.ColorButton($"##Gear_{item.ItemId}#Dye#1", StainToVector4(dye.Item2),
                            ImGuiColorEditFlags.None, new Vector2(16, 16));
                        ImGui.SameLine();
                        //ImGui.PushStyleColor(ImGuiCol.Text, ConvertColorToVector4(ConvertColorToAbgr(dye.Item2)));
                        ImGui.TextUnformatted(dye.Item1);
                        //ImGui.PopStyleColor();
                    }
                }

                if (item.Stain1 > 0)
                {
                    (string, uint) dye2 = globalCache.StainStorage.LoadStainWithColor(currentLocale, item.Stain1);
                    if (!string.IsNullOrEmpty(dye2.Item1))
                    {
                        ImGui.ColorButton($"##Gear_{item.ItemId}#Dye#2", StainToVector4(dye2.Item2),
                            ImGuiColorEditFlags.None, new Vector2(16, 16));
                        ImGui.SameLine();
                        //ImGui.PushStyleColor(ImGuiCol.Text, ConvertColorToVector4(ConvertColorToAbgr(dye2.Item2)));
                        ImGui.TextUnformatted(dye2.Item1);
                        //ImGui.PopStyleColor();
                    }
                }

                ImGui.Separator();
                if (isInASet)
                {
                    if (miragePrismIcon is not null)
                    {
                        ImGui.Image(miragePrismIcon.Handle, new Vector2(16, 16), miragePrismBoxSetIconUv0,
                            miragePrismBoxSetIconUv1);
                    }
                    ImGui.SameLine();
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 15643)}");
                }

                ImGui.TextUnformatted($"{dbItem.Value.Description}");
            }
            else
            {
                ImGui.Separator();
                HashSet<uint>? sets = globalCache.MirageSetStorage.GetMirageSetLookup(item.ItemId);
                if (sets is not null)
                {
                    foreach (uint u in sets)
                    {
                        Item? i = globalCache.ItemStorage.LoadItem(currentLocale, u);
                        if (i == null) continue;
                        ImGui.TextUnformatted(i.Value.Name.ExtractText());
                    }
                }
            }
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
            Barding barding)
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
            DrawIcon(globalCache.IconStorage.LoadIcon(barding.Icon), new Vector2(40, 40));
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

        public static void DrawOrnamentTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Models.Ornament ornament)
        {
            using var drawOrnamentTooltip = ImRaii.Tooltip();
            if (!drawOrnamentTooltip) return;
            using (var drawOrnamentTooltipItem = ImRaii.Table($"###DrawOrnamentTooltipItem#Ornament_{ornament.Id}", 2))
            {
                if (!drawOrnamentTooltipItem) return;
                ImGui.TableSetupColumn($"###DrawOrnamentTooltipItem#Ornament_{ornament.Id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawOrnamentTooltipItem#Ornament_{ornament.Id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(ornament.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(ornament.GermanName)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(ornament.EnglishName)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(ornament.FrenchName)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(ornament.JapaneseName)}");
                        break;
                }
            }

            ImGui.Separator();
            if (ornament.Transient is null) return;
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(ornament.Transient.GermanTooltip)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(ornament.Transient.EnglishTooltip)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(ornament.Transient.FrenchTooltip)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(ornament.Transient.JapaneseTooltip)}");
                    break;
            }
        }

        public static void DrawGlassesTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Models.Glasses glasses)
        {
            using var drawGlassesTooltip = ImRaii.Tooltip();
            if (!drawGlassesTooltip) return;
            using (var drawGlassesDescriptionItem = ImRaii.Table($"###DrawGlassesDescriptionItem#Glasses_{glasses.Id}", 2))
            {
                if (!drawGlassesDescriptionItem) return;
                ImGui.TableSetupColumn($"###DrawGlassesDescriptionItem#Glasses_{glasses.Id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawGlassesDescriptionItem#Glasses_{glasses.Id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawIcon(globalCache.IconStorage.LoadIcon(glasses.Icon), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(glasses.GermanName)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(glasses.EnglishName)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(glasses.FrenchName)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(glasses.JapaneseName)}");
                        break;
                }
            }

            ImGui.Separator();
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(glasses.GermanDescription)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(glasses.EnglishDescription)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(glasses.FrenchDescription)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(glasses.JapaneseDescription)}");
                    break;
            }
        }

        public static void DrawHairstyleFacepaintTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Models.Hairstyle hairstyle)
        {
            using var drawhairstyleTooltip = ImRaii.Tooltip();
            if (!drawhairstyleTooltip) return;
            using var drawhairstyleDescriptionItem = ImRaii.Table($"###DrawhairstyleDescriptionItem#hairstyle_{hairstyle.Id}", 2);
            if (!drawhairstyleDescriptionItem) return;
            ImGui.TableSetupColumn($"###DrawhairstyleDescriptionItem#hairstyle_{hairstyle.Id}#Icon",
                ImGuiTableColumnFlags.WidthFixed, 55);
            ImGui.TableSetupColumn($"###DrawhairstyleDescriptionItem#hairstyle_{hairstyle.Id}#Name",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawIcon(globalCache.IconStorage.LoadIcon(hairstyle.Icon), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(hairstyle.GermanName)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(hairstyle.EnglishName)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(hairstyle.FrenchName)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(hairstyle.JapaneseName)}");
                    break;
            }
        }

        public static void DrawSecretRecipeBookTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Models.SecretRecipeBook secretRecipeBook)
        {
            using var drawSecretRecipeBookTooltip = ImRaii.Tooltip();
            if (!drawSecretRecipeBookTooltip) return;
            using var drawsecretRecipeBookDescriptionItem = ImRaii.Table($"###DrawsecretRecipeBookDescriptionItem#secretRecipeBook_{secretRecipeBook.Id}", 2);
            if (!drawsecretRecipeBookDescriptionItem) return;
            ImGui.TableSetupColumn($"###DrawsecretRecipeBookDescriptionItem#secretRecipeBook_{secretRecipeBook.Id}#Icon",
                ImGuiTableColumnFlags.WidthFixed, 55);
            ImGui.TableSetupColumn($"###DrawsecretRecipeBookDescriptionItem#secretRecipeBook_{secretRecipeBook.Id}#Name",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawIcon(globalCache.IconStorage.LoadIcon(secretRecipeBook.Icon), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted($"{Capitalize(secretRecipeBook.GermanName)}");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted($"{Capitalize(secretRecipeBook.EnglishName)}");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted($"{Capitalize(secretRecipeBook.FrenchName)}");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted($"{Capitalize(secretRecipeBook.JapaneseName)}");
                    break;
            }
        }

        public static void DrawVistaTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Vista vista, bool hasVista, bool isSpoilerEnabled)
        {
            using var drawVistaTooltip = ImRaii.Tooltip();
            if (!drawVistaTooltip) return;
            using (var drawVistaDescriptionItem = ImRaii.Table($"###DrawVistaDescriptionItem#Vista_{vista.Id}", 2))
            {
                if (!drawVistaDescriptionItem) return;
                ImGui.TableSetupColumn($"###DrawVistaDescriptionItem#Vista_{vista.Id}#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 55);
                ImGui.TableSetupColumn($"###DrawVistaDescriptionItem#Vista_{vista.Id}#Name",
                    ImGuiTableColumnFlags.WidthFixed, 305);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (hasVista)
                {
                    DrawIcon(globalCache.IconStorage.LoadIcon((uint)vista.IconDiscovered), new Vector2(40, 40));
                }
                else
                {
                    if (isSpoilerEnabled)
                    {
                        DrawIcon(globalCache.IconStorage.LoadIcon((uint)vista.IconList), new Vector2(40, 40),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        DrawIcon(globalCache.IconStorage.LoadIcon((uint)vista.IconUndiscovered), new Vector2(40, 40));
                    }
                }
                ImGui.TableSetColumnIndex(1);
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(vista.GermanName)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(vista.EnglishName)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(vista.FrenchName)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(vista.JapaneseName)}");
                        break;
                }
            }

            Models.PlaceName? pn = globalCache.PlaceNameStorage.GetPlaceName(currentLocale, vista.PlaceNameId);
            string locationName = currentLocale switch
            {
                ClientLanguage.German => pn?.GermanName,
                ClientLanguage.English => pn?.EnglishName,
                ClientLanguage.French => pn?.FrenchName,
                ClientLanguage.Japanese => pn?.JapaneseName,
                _ => pn?.EnglishName
            } ?? string.Empty;
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 296)}: {locationName}");
            Models.Emote? e = globalCache.EmoteStorage.GetEmote(currentLocale, vista.Emote);
            string emoteName = currentLocale switch
            {
                ClientLanguage.German => e?.GermanName,
                ClientLanguage.English => e?.EnglishName,
                ClientLanguage.French => e?.FrenchName,
                ClientLanguage.Japanese => e?.JapaneseName,
                _ => e?.EnglishName
            } ?? string.Empty;
            ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 635)}: {emoteName}");
            ImGui.Separator();
            if (hasVista || isSpoilerEnabled)
            {
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(vista.GermanDescription)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(vista.EnglishDescription)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(vista.FrenchDescription)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(vista.JapaneseDescription)}");
                        break;
                }
            }
            else
            {
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ImGui.TextUnformatted($"{Capitalize(vista.GermanImpression)}");
                        break;
                    case ClientLanguage.English:
                        ImGui.TextUnformatted($"{Capitalize(vista.EnglishImpression)}");
                        break;
                    case ClientLanguage.French:
                        ImGui.TextUnformatted($"{Capitalize(vista.FrenchImpression)}");
                        break;
                    case ClientLanguage.Japanese:
                        ImGui.TextUnformatted($"{Capitalize(vista.JapaneseImpression)}");
                        break;
                }
            }
        }

        private static string GetExtractableString(ClientLanguage currentLocale, GlobalCache globalCache, Item item)
        {
            string str = globalCache.AddonStorage.LoadAddonString(currentLocale, 1361);
            Plugin.Log.Debug($"extract str: {str} => item desynth {item.Desynth}");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(1),0))>Y<Else/>N</If>", (item.AdditionalData) ? "Y" : "N");
            //str = str.Replace("<If(GreaterThan(IntegerParameter(2),0))>Y<Else/>N</If>", (item.IsGlamourous) ? "Y" : "N");
            str = str.Replace("Extractable: YN", "Extractable: ");
            str = str.Replace("Projectable: YN", (item.IsGlamorous) ? "Projectable: Y" : "Projectable: N");
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
        /*public static void Materia(ushort id, byte grade, Lumina.Excel.Sheets.ItemLevel itemLevel)
        {
            var baseParams = new Dictionary<uint, Lumina.Excel.Sheets.BaseParam>();
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
            return lumina != null ? lumina.Value.Text.ExtractText() : string.Empty;
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
                if (minion.Singular.IsEmpty) continue;
                if (minion.Icon == 0) continue;
                Minion m = new() { Id = minion.RowId, Icon = minion.Icon, Transient = new Transient() };
                CompanionTransient? ct = GetCompanionTransient(currentLocale, minion.RowId);
                if (ct is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = minion.Singular.ExtractText();
                        m.Transient.GermanDescription = ct.Value.Description.ExtractText();
                        m.Transient.GermanDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.GermanTooltip = ct.Value.Tooltip.ExtractText();
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = minion.Singular.ExtractText();
                        m.Transient.EnglishDescription = ct.Value.Description.ExtractText();
                        m.Transient.EnglishDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.EnglishTooltip = ct.Value.Tooltip.ExtractText();
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = minion.Singular.ExtractText();
                        m.Transient.FrenchDescription = ct.Value.Description.ExtractText();
                        m.Transient.FrenchDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.FrenchTooltip = ct.Value.Tooltip.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = minion.Singular.ExtractText();
                        m.Transient.JapaneseDescription = ct.Value.Description.ExtractText();
                        m.Transient.JapaneseDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.JapaneseTooltip = ct.Value.Tooltip.ExtractText();
                        break;
                }

                returnedMinionsIds.Add(m);
            }

            return returnedMinionsIds;
        }

        public static BaseParam? GetBaseParam(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BaseParam>? dct = Plugin.DataManager.GetExcelSheet<BaseParam>(currentLocale);
            BaseParam? lumina = dct?.GetRow(id);
            return lumina;
        }
        public static List<Models.BaseParam>? GetAllBaseParams(ClientLanguage currentLocale)
        {
            List<Models.BaseParam> returnedBaseParamsIds = [];
            ExcelSheet<BaseParam>? dm = Plugin.DataManager.GetExcelSheet<BaseParam>(currentLocale);
            using IEnumerator<BaseParam>? baseParamEnumerator = dm?.GetEnumerator();
            if (baseParamEnumerator is null) return null;
            while (baseParamEnumerator.MoveNext())
            {
                BaseParam baseParam = baseParamEnumerator.Current;
                if (baseParam.Name.IsEmpty) continue;
                Models.BaseParam m = new()
                {
                    Id = baseParam.RowId,
                    OneHandWeaponPercent = baseParam.OneHandWeaponPercent,
                    OffHandPercent = baseParam.OffHandPercent,
                    HeadPercent = baseParam.HeadPercent,
                    ChestPercent = baseParam.ChestPercent,
                    HandsPercent = baseParam.HandsPercent,
                    WaistPercent = baseParam.WaistPercent,
                    LegsPercent = baseParam.LegsPercent,
                    FeetPercent = baseParam.FeetPercent,
                    EarringPercent = baseParam.EarringPercent,
                    NecklacePercent = baseParam.NecklacePercent,
                    BraceletPercent = baseParam.BraceletPercent,
                    RingPercent = baseParam.RingPercent,
                    TwoHandWeaponPercent = baseParam.TwoHandWeaponPercent,
                    UnderArmorPercent = baseParam.UnderArmorPercent,
                    ChestHeadPercent = baseParam.ChestHeadPercent,
                    ChestHeadLegsFeetPercent = baseParam.ChestHeadLegsFeetPercent,
                    Unknown0 = baseParam.Unknown0,
                    LegsFeetPercent = baseParam.LegsFeetPercent,
                    HeadChestHandsLegsFeetPercent = baseParam.HeadChestHandsLegsFeetPercent,
                    ChestLegsGlovesPercent = baseParam.ChestLegsGlovesPercent,
                    ChestLegsFeetPercent = baseParam.ChestLegsFeetPercent,
                    Unknown1 = baseParam.Unknown1,
                    OrderPriority = baseParam.OrderPriority,
                    MeldParam = [..baseParam.MeldParam],
                    PacketIndex = baseParam.PacketIndex,
                    Unknown2 = baseParam.Unknown2
                };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = baseParam.Name.ExtractText();
                        m.GermanDescription = baseParam.Description.ExtractText();
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = baseParam.Name.ExtractText();
                        m.EnglishDescription = baseParam.Description.ExtractText();
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = baseParam.Name.ExtractText();
                        m.FrenchDescription = baseParam.Description.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = baseParam.Name.ExtractText();
                        m.JapaneseDescription = baseParam.Description.ExtractText();
                        break;
                }

                returnedBaseParamsIds.Add(m);
            }

            return returnedBaseParamsIds;
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
                if (mount.Singular.IsEmpty) continue;
                if (mount.Icon == 0) continue;
                Models.Mount m = new() { Id = mount.RowId, Icon = mount.Icon, Transient = new Transient() };
                MountTransient? mt = GetMountTransient(currentLocale, mount.RowId);
                if (mt is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = mount.Singular.ExtractText();
                        m.Transient.GermanDescription = mt.Value.Description.ExtractText();
                        m.Transient.GermanDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.GermanTooltip = mt.Value.Tooltip.ExtractText();
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = mount.Singular.ExtractText();
                        m.Transient.EnglishDescription = mt.Value.Description.ExtractText();
                        m.Transient.EnglishDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.EnglishTooltip = mt.Value.Tooltip.ExtractText();
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = mount.Singular.ExtractText();
                        m.Transient.FrenchDescription = mt.Value.Description.ExtractText();
                        m.Transient.FrenchDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.FrenchTooltip = mt.Value.Tooltip.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = mount.Singular.ExtractText();
                        m.Transient.JapaneseDescription = mt.Value.Description.ExtractText();
                        m.Transient.JapaneseDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                        m.Transient.JapaneseTooltip = mt.Value.Tooltip.ExtractText();
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
                if (tripletriadcard.Name.IsEmpty || tripletriadcard.Name == "0") continue;
                Models.TripleTriadCard ttc = new() { Id = tripletriadcard.RowId };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        ttc.GermanName = tripletriadcard.Name.ExtractText();
                        ttc.GermanDescription = tripletriadcard.Description.ExtractText();
                        break;
                    case ClientLanguage.English:
                        ttc.EnglishName = tripletriadcard.Name.ExtractText();
                        ttc.EnglishDescription = tripletriadcard.Description.ExtractText();
                        break;
                    case ClientLanguage.French:
                        ttc.FrenchName = tripletriadcard.Name.ExtractText();
                        ttc.FrenchDescription = tripletriadcard.Description.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        ttc.JapaneseName = tripletriadcard.Name.ExtractText();
                        ttc.JapaneseDescription = tripletriadcard.Description.ExtractText();
                        break;
                }

                ttc.Icon = tripletriadcard.RowId + 88000;

                returnedTripletriadcardsIds.Add(ttc);
            }

            return returnedTripletriadcardsIds;
        }

        public static Emote? GetEmote(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Emote> dttc = Plugin.DataManager.GetExcelSheet<Emote>(currentLocale);
            Emote? lumina = dttc.GetRow(id);
            return lumina;
        }
        public static TextCommand? GetTextCommand(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<TextCommand> dtc = Plugin.DataManager.GetExcelSheet<TextCommand>(currentLocale);
            TextCommand? lumina = dtc.GetRow(id);
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
                if (emote.Name.IsEmpty || emote.Name == "0") continue;
                Models.Emote e = new() { Id = emote.RowId, TextCommand = new Models.TextCommand() };
                TextCommand? tc = emote.TextCommand.ValueNullable;
                if (tc is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        e.GermanName = emote.Name.ExtractText();
                        e.TextCommand.GermanCommand = tc.Value.Command.ExtractText();
                        e.TextCommand.GermanShortCommand = tc.Value.ShortCommand.ExtractText();
                        e.TextCommand.GermanDescription= tc.Value.Description.ExtractText();
                        e.TextCommand.GermanAlias = tc.Value.Alias.ExtractText();
                        e.TextCommand.GermanShortAlias = tc.Value.ShortAlias.ExtractText();
                        break;
                    case ClientLanguage.English:
                        e.EnglishName = emote.Name.ExtractText();
                        e.TextCommand.EnglishCommand = tc.Value.Command.ExtractText();
                        e.TextCommand.EnglishShortCommand = tc.Value.ShortCommand.ExtractText();
                        e.TextCommand.EnglishDescription = tc.Value.Description.ExtractText();
                        e.TextCommand.EnglishAlias = tc.Value.Alias.ExtractText();
                        e.TextCommand.EnglishShortAlias = tc.Value.ShortAlias.ExtractText();
                        break;
                    case ClientLanguage.French:
                        e.FrenchName = emote.Name.ExtractText();
                        e.TextCommand.FrenchCommand = tc.Value.Command.ExtractText();
                        e.TextCommand.FrenchShortCommand = tc.Value.ShortCommand.ExtractText();
                        e.TextCommand.FrenchDescription = tc.Value.Description.ExtractText();
                        e.TextCommand.FrenchAlias = tc.Value.Alias.ExtractText();
                        e.TextCommand.FrenchShortAlias = tc.Value.ShortAlias.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        e.JapaneseName = emote.Name.ExtractText();
                        e.TextCommand.JapaneseCommand = tc.Value.Command.ExtractText();
                        e.TextCommand.JapaneseShortCommand = tc.Value.ShortCommand.ExtractText();
                        e.TextCommand.JapaneseDescription = tc.Value.Description.ExtractText();
                        e.TextCommand.JapaneseAlias = tc.Value.Alias.ExtractText();
                        e.TextCommand.JapaneseShortAlias = tc.Value.ShortAlias.ExtractText();
                        break;
                }

                e.Icon = emote.Icon;
                e.UnlockLink = emote.UnlockLink;
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
            using IEnumerator<BuddyEquip>? bardingEnumerator = dbe.GetEnumerator();
            while (bardingEnumerator.MoveNext())
            {
                BuddyEquip barding = bardingEnumerator.Current;
                if (barding.Name.IsEmpty || barding.RowId == 0) continue;
                Barding b = new() { Id = barding.RowId };
                Item? item = GetItemFromName(currentLocale, barding.Name.ExtractText());
                b.Icon = (item is null || item.Value.RowId == 0) ? barding.IconHead: item.Value.Icon;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        b.GermanName = barding.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        b.EnglishName = barding.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        b.FrenchName = barding.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        b.JapaneseName = barding.Name.ExtractText();
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
                if (orchestrionRoll.Name.IsEmpty || orchestrionRoll.RowId == 0) continue;
                OrchestrionRoll orchestrion = new() { Id = orchestrionRoll.RowId };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        orchestrion.GermanName = orchestrionRoll.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        orchestrion.EnglishName = orchestrionRoll.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        orchestrion.FrenchName = orchestrionRoll.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        orchestrion.JapaneseName = orchestrionRoll.Name.ExtractText();
                        break;
                }

                returnedOrchestrionRollIds.Add(orchestrion);
            }

            return returnedOrchestrionRollIds;
        }

        public static Ornament? GetOrnament(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Ornament>? dm = Plugin.DataManager.GetExcelSheet<Ornament>(currentLocale);
            Ornament? lumina = dm?.GetRow(id);
            return lumina;
        }
        public static OrnamentTransient? GetOrnamentTransient(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<OrnamentTransient>? dmt = Plugin.DataManager.GetExcelSheet<OrnamentTransient>(currentLocale);
            OrnamentTransient? lumina = dmt?.GetRow(id);
            return lumina;
        }
        public static List<Models.Ornament>? GetAllOrnaments(ClientLanguage currentLocale)
        {
            List<Models.Ornament> returnedOrnamentsIds = [];
            ExcelSheet<Ornament>? dor = Plugin.DataManager.GetExcelSheet<Ornament>(currentLocale);
            using IEnumerator<Ornament>? ornamentEnumerator = dor?.GetEnumerator();
            if (ornamentEnumerator is null) return null;
            while (ornamentEnumerator.MoveNext())
            {
                Ornament ornament = ornamentEnumerator.Current;
                if (ornament.Singular.IsEmpty) continue;
                if (ornament.Icon == 0) continue;
                Models.Ornament m = new() { Id = ornament.RowId, Icon = ornament.Icon, Transient = new Transient() };
                OrnamentTransient? ct = GetOrnamentTransient(currentLocale, ornament.RowId);
                if (ct is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = ornament.Singular.ExtractText();
                        m.Transient.GermanDescription = ct.Value.Text.ExtractText();
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = ornament.Singular.ExtractText();
                        m.Transient.EnglishDescription = ct.Value.Text.ExtractText();
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = ornament.Singular.ExtractText();
                        m.Transient.FrenchDescription = ct.Value.Text.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = ornament.Singular.ExtractText();
                        m.Transient.JapaneseDescription = ct.Value.Text.ExtractText();
                        break;
                }

                returnedOrnamentsIds.Add(m);
            }

            return returnedOrnamentsIds;
        }

        public static Glasses? GetGlasses(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Glasses>? dm = Plugin.DataManager.GetExcelSheet<Glasses>(currentLocale);
            Glasses? lumina = dm?.GetRow(id);
            return lumina;
        }
        public static List<Models.Glasses>? GetAllGlasses(ClientLanguage currentLocale)
        {
            List<Models.Glasses> returnedGlassessIds = [];
            ExcelSheet<Glasses>? dor = Plugin.DataManager.GetExcelSheet<Glasses>(currentLocale);
            using IEnumerator<Glasses>? glassesEnumerator = dor?.GetEnumerator();
            if (glassesEnumerator is null) return null;
            while (glassesEnumerator.MoveNext())
            {
                Glasses glasses = glassesEnumerator.Current;
                if (glasses.Singular.IsEmpty) continue;
                if (glasses.Icon == 0) continue;
                Models.Glasses m = new() { Id = glasses.RowId, Icon = (uint)glasses.Icon };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = glasses.Singular.ExtractText();
                        m.GermanDescription = glasses.Description.ExtractText();
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = glasses.Singular.ExtractText();
                        m.EnglishDescription = glasses.Description.ExtractText();
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = glasses.Singular.ExtractText();
                        m.FrenchDescription = glasses.Description.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = glasses.Singular.ExtractText();
                        m.JapaneseDescription = glasses.Description.ExtractText();
                        break;
                }

                returnedGlassessIds.Add(m);
            }

            return returnedGlassessIds;
        }

        public static Lumina.Excel.Sheets.SecretRecipeBook? GetSecretRecipeBook(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Lumina.Excel.Sheets.SecretRecipeBook>? dsbr = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.SecretRecipeBook>(currentLocale);
            Lumina.Excel.Sheets.SecretRecipeBook? lumina = dsbr?.GetRow(id);
            return lumina;
        }
        public static List<Models.SecretRecipeBook>? GetAllSecretRecipeBook(ClientLanguage currentLocale)
        {
            List<Models.SecretRecipeBook> returnedSecretRecipeBookIds = [];
            ExcelSheet<Lumina.Excel.Sheets.SecretRecipeBook>? dor = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.SecretRecipeBook>(currentLocale);
            using IEnumerator<Lumina.Excel.Sheets.SecretRecipeBook>? secretRecipeBookEnumerator = dor?.GetEnumerator();
            if (secretRecipeBookEnumerator is null) return null;
            while (secretRecipeBookEnumerator.MoveNext())
            {
                Lumina.Excel.Sheets.SecretRecipeBook secretRecipeBook = secretRecipeBookEnumerator.Current;
                if (secretRecipeBook.Name.IsEmpty || secretRecipeBook.RowId == 0 || secretRecipeBook.RowId == 16) continue;
                Models.SecretRecipeBook srb = new() { Id = secretRecipeBook.RowId, ItemId = secretRecipeBook.Item.RowId, Icon = secretRecipeBook.Item.Value.Icon };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        srb.GermanName = secretRecipeBook.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        srb.EnglishName = secretRecipeBook.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        srb.FrenchName = secretRecipeBook.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        srb.JapaneseName = secretRecipeBook.Name.ExtractText();
                        break;
                }

                returnedSecretRecipeBookIds.Add(srb);
            }

            return returnedSecretRecipeBookIds;
        }

        public static Adventure? GetVista(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Adventure>? dv = Plugin.DataManager.GetExcelSheet<Adventure>(currentLocale);
            Adventure? lumina = dv?.GetRow(id);
            return lumina;
        }
        public static List<Vista>? GetAllVista(ClientLanguage currentLocale)
        {
            List<Vista> returnedVistasIds = [];
            ExcelSheet<Adventure>? dv = Plugin.DataManager.GetExcelSheet<Adventure>(currentLocale);
            using IEnumerator<Adventure>? vistaEnumerator = dv?.GetEnumerator();
            if (vistaEnumerator is null) return null;
            while (vistaEnumerator.MoveNext())
            {
                Adventure vista = vistaEnumerator.Current;
                if (vista.Name.IsEmpty || vista.RowId == 0) continue;
                Vista v = new()
                {
                    Id = vista.RowId,
                    LevelId = vista.Level.RowId,
                    MinLevel = vista.MinLevel,
                    MaxLevel = vista.MaxLevel,
                    Emote = vista.Emote.RowId,
                    PlaceNameId = vista.PlaceName.RowId,
                    IconList = vista.IconList,
                    IconDiscovered = vista.IconDiscovered,
                    IconUndiscovered = vista.IconUndiscovered,
                    IsInitial = vista.IsInitial,
                };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        v.GermanName = vista.Name.ExtractText();
                        v.GermanImpression = vista.Impression.ExtractText();
                        v.GermanDescription = vista.Description.ExtractText();
                        break;
                    case ClientLanguage.English:
                        v.EnglishName = vista.Name.ExtractText();
                        v.EnglishImpression = vista.Impression.ExtractText();
                        v.EnglishDescription = vista.Description.ExtractText();
                        break;
                    case ClientLanguage.French:
                        v.FrenchName = vista.Name.ExtractText();
                        v.FrenchImpression = vista.Impression.ExtractText();
                        v.FrenchDescription = vista.Description.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        v.JapaneseName = vista.Name.ExtractText();
                        v.JapaneseImpression = vista.Impression.ExtractText();
                        v.JapaneseDescription = vista.Description.ExtractText();
                        break;
                }

                returnedVistasIds.Add(v);
            }

            return returnedVistasIds;
        }

        public static ExcelSheet<MirageStoreSetItem> GetMirageStoreSetItems()
        {
            ExcelSheet<MirageStoreSetItem> dor = Plugin.DataManager.GetExcelSheet<MirageStoreSetItem>(ClientLanguage.English);
            return dor;
        }

        public static string Capitalize(string str)
        {
            if(str.Length == 0) return str;
            return char.ToUpper(str[0]) + str[1..];
        }

        public static string CapitalizeSentence(string str)
        {
            if (str.Length == 0) return str;
            string[] splits = str.Split(" ");
            for (int i = 0; i < splits.Length; i++)
            {
                splits[i] = Capitalize(splits[i]);
            }

            return string.Join(" ", splits);
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

        public static bool IsDutyCompleted(uint dutyId)
        {
            return UIState.IsInstanceContentCompleted(dutyId);
        }
        public static bool IsDutyUnlocked(uint dutyId)
        {
            return UIState.IsInstanceContentUnlocked(dutyId);
        }

        public static Models.Quest? GetQuest(uint id)
        {
            Models.Quest q = new();
            List<ClientLanguage> langs =
                [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
            foreach (ClientLanguage l in langs)
            {
                ExcelSheet<Quest> dbe = Plugin.DataManager.GetExcelSheet<Quest>(l);
                Quest lumina = dbe.GetRow(id);
                if (lumina.RowId == 0) return null;
                switch (l)
                {
                    case ClientLanguage.German:
                        {
                            q.GermanName = lumina.Name.ExtractText();
                            break;
                        }
                    case ClientLanguage.English:
                        {
                            q.Id = lumina.RowId;
                            q.Icon = lumina.Icon;
                            q.EnglishName = lumina.Name.ExtractText();
                            break;
                        }
                    case ClientLanguage.French:
                        {
                            q.FrenchName = lumina.Name.ExtractText();
                            break;
                        }
                    case ClientLanguage.Japanese:
                        {
                            q.JapaneseName = lumina.Name.ExtractText();
                            break;
                        }
                }
            }

            return q;
        }

        public static Duty? GetDuty(uint id)
        {
            Duty d = new();
            List<ClientLanguage> langs =
                [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
            foreach (ClientLanguage l in langs)
            {
                ExcelSheet<ContentFinderCondition>? cfc =
                    Plugin.DataManager.GetExcelSheet<ContentFinderCondition>(l);
                ContentFinderCondition? duty = cfc?.GetRow(id);
                if (duty == null || duty.Value.Name.IsEmpty) continue;

                ExcelSheet<ContentFinderConditionTransient>? dbt =
                    Plugin.DataManager.GetExcelSheet<ContentFinderConditionTransient>(l);
                ContentFinderConditionTransient? cfct = dbt?.GetRow(duty.Value.RowId);

                switch (l)
                {
                    case ClientLanguage.German:
                        {
                            d.GermanName = duty.Value.Name.ExtractText();
                            d.GermanTransient = cfct?.Description.ExtractText() ?? string.Empty;
                            break;
                        }
                    case ClientLanguage.English:
                        {
                            d.Id = duty.Value.RowId;
                            d.Icon = duty.Value.Icon;
                            d.Image = duty.Value.Image;
                            if (duty.Value.ContentType.ValueNullable != null)
                            {
                                d.ContentTypeId = duty.Value.ContentType.Value.RowId;
                            }

                            d.Content = duty.Value.Content.RowId;
                            d.SortKey = duty.Value.SortKey;
                            d.TransientKey = duty.Value.Transient.RowId;
                            d.EnglishName = duty.Value.Name.ExtractText();
                            d.EnglishTransient = cfct?.Description.ExtractText() ?? string.Empty;
                            d.ExVersion = duty.Value.TerritoryType.ValueNullable?.ExVersion.ValueNullable?.RowId;
                            d.HighEndDuty = duty.Value.HighEndDuty;
                            d.AllowUndersized = duty.Value.AllowUndersized;
                            d.ContentMemberType = duty.Value.ContentMemberType.RowId;
                            d.TerritoryType = duty.Value.TerritoryType.RowId;
                            break;
                        }
                    case ClientLanguage.French:
                        {
                            d.FrenchName = duty.Value.Name.ExtractText();
                            d.FrenchTransient = cfct?.Description.ExtractText() ?? string.Empty;
                            break;
                        }
                    case ClientLanguage.Japanese:
                        {
                            d.JapaneseName = duty.Value.Name.ExtractText();
                            d.JapaneseTransient = cfct?.Description.ExtractText() ?? string.Empty;
                            break;
                        }
                }
            }

            return d;
        }

        public static List<Duty>? GetDutyList()
        {
            List<Duty> returnedDuties = [];

            ExcelSheet<ContentFinderCondition>? dor =
                Plugin.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English);
            using IEnumerator<ContentFinderCondition>? duties = dor?.GetEnumerator();
            if (duties is null) return null;
            while (duties.MoveNext())
            {
                ContentFinderCondition duty = duties.Current;
                if (duty.Name.IsEmpty) continue;

                Duty? d = GetDuty(duty.RowId);
                if(d == null) continue;
                returnedDuties.Add(d);
            }

            return returnedDuties;
        }

        private static DutyType GetDutyType(Duty duty, ContentFinderCondition cfc)
        {
            return duty switch
            {
                { ContentTypeId: 5 } when duty.EnglishName.Contains("Savage") => DutyType.Savage,
                { ContentTypeId: 28 } => DutyType.Ultimate,
                { ContentTypeId: 4, HighEndDuty: false } => DutyType.Extreme,
                { ContentTypeId: 4, HighEndDuty: true } => DutyType.Unreal,
                { ContentTypeId: 30, AllowUndersized: false } => DutyType.Criterion,
                { ContentTypeId: 5, ContentMemberType: 4 } => DutyType.Alliance,
                _ => DutyType.Unknown,
            };
        }

        public static List<List<bool>> GetCharactersMainScenarioQuests(List<Character> characters)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                List<bool> completedQuests =
                [
                    character.HasQuest((int)QuestIds.MSQ_A_REALM_REBORN),
                    character.HasQuest((int)QuestIds.MSQ_A_REALM_AWOKEN),
                    character.HasQuest((int)QuestIds.MSQ_THROUGH_THE_MAELSTROM),
                    character.HasQuest((int)QuestIds.MSQ_DEFENDERS_OF_EORZEA),
                    character.HasQuest((int)QuestIds.MSQ_DREAMS_OF_ICE),
                    character.HasQuest((int)QuestIds.MSQ_BEFORE_THE_FALL_PART_1),
                    character.HasQuest((int)QuestIds.MSQ_BEFORE_THE_FALL_PART_2),
                    character.HasQuest((int)QuestIds.MSQ_HEAVENSWARD),
                    character.HasQuest((int)QuestIds.MSQ_AS_GOES_LIGHT_SO_GOES_DARKNESS),
                    character.HasQuest((int)QuestIds.MSQ_THE_GEARS_OF_CHANGE),
                    character.HasQuest((int)QuestIds.MSQ_REVENGE_OF_THE_HORDE),
                    character.HasQuest((int)QuestIds.MSQ_SOUL_SURRENDER),
                    character.HasQuest((int)QuestIds.MSQ_THE_FAR_EDGE_OF_FATE_PART_1),
                    character.HasQuest((int)QuestIds.MSQ_THE_FAR_EDGE_OF_FATE_PART_2),
                    character.HasQuest((int)QuestIds.MSQ_STORMBLOOD),
                    character.HasQuest((int)QuestIds.MSQ_THE_LEGEND_RETURNS),
                    character.HasQuest((int)QuestIds.MSQ_RISE_OF_A_NEW_SUN),
                    character.HasQuest((int)QuestIds.MSQ_UNDER_THE_MOONLIGHT),
                    character.HasQuest((int)QuestIds.MSQ_PRELUDE_IN_VIOLET),
                    character.HasQuest((int)QuestIds.MSQ_A_REQUIEM_FOR_HEROES_PART_1),
                    character.HasQuest((int)QuestIds.MSQ_A_REQUIEM_FOR_HEROES_PART_2),
                    character.HasQuest((int)QuestIds.MSQ_SHADOWBRINGER),
                    character.HasQuest((int)QuestIds.MSQ_VOWS_OF_VIRTUE_DEEDS_OF_CRUELTY),
                    character.HasQuest((int)QuestIds.MSQ_ECHOES_OF_A_FALLEN_STAR),
                    character.HasQuest((int)QuestIds.MSQ_REFLECTIONS_IN_CRYSTAL),
                    character.HasQuest((int)QuestIds.MSQ_FUTURES_REWRITTEN),
                    character.HasQuest((int)QuestIds.MSQ_DEATH_UNTO_DAWN_PART_1),
                    character.HasQuest((int)QuestIds.MSQ_DEATH_UNTO_DAWN_PART_2),
                    character.HasQuest((int)QuestIds.MSQ_ENDWALKER),
                    character.HasQuest((int)QuestIds.MSQ_NEWFOUND_ADVENTURE),
                    character.HasQuest((int)QuestIds.MSQ_BURIED_MEMORY),
                    character.HasQuest((int)QuestIds.MSQ_GODS_REVEL_LANDS_TREMBLE),
                    character.HasQuest((int)QuestIds.MSQ_THE_DARK_THRONE),
                    character.HasQuest((int)QuestIds.MSQ_GROWING_LIGHT_PART_1),
                    character.HasQuest((int)QuestIds.MSQ_GROWING_LIGHT_PART_2),
                    character.HasQuest((int)QuestIds.MSQ_DAWNTRAIL),
                    character.HasQuest((int)QuestIds.MSQ_CROSSROADS),
                    character.HasQuest((int)QuestIds.MSQ_SEEKERS_OF_ETERNITY),
                    character.HasQuest((int)QuestIds.MSQ_THE_PROMISE_OF_TOMORROW),
                    character.HasQuest((int)QuestIds.MSQ_THE_MIST)
                ];
                result.Add(completedQuests);
            }

            return result;
        }

        public static List<List<bool>> GetCharactersEventsQuests(List<Character> characters)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                bool allsaintswake = (character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_GRIDANIA) ||
                                      character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_LIMSA) ||
                                      character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_ULDAH));
                List<bool> completedQuests =
                [
                    /*character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2010),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2011),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2011),
                    character.HasQuest((int)QuestIds.EVENT_FIREFALL_FAIRE_2011),
                    character.HasQuest((int)QuestIds.EVENT_HUNTER_S_MOON_2011),
                    character.HasQuest((int)QuestIds.EVENT_FOUNDATION_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2011),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2011),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2012),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2012),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2012),
                    character.HasQuest((int)QuestIds.EVENT_FOUNDATION_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2013),*/
                    allsaintswake,
                    character.HasQuest((int)QuestIds.EVENT_LIGHTNING_STRIKES_2013),
                    //character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2013),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2014),
                    character.HasQuest((int)QuestIds.EVENT_BURGEONING_DREAD_2014),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2014),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2014),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2014),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2014),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2014),
                    character.HasQuest((int)QuestIds.EVENT_THAT_OLD_BLACK_MAGIC_2014),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2014),
                    character.HasQuest((int)QuestIds.EVENT_LIGHTNING_RETURNS),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2_2014),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2014),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2014),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2015),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2015),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2015),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2015),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2015),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2015),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2015),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2015),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2015),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2016),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2016),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2016),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2016),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2016),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2016),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2016),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2016),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2016),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2016),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2017),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2017),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2017),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2017),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2017),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2017),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2017),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2017),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2017),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2018),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2018),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2018),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2018),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2018),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_HUNT_FOR_RATHALOS),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2018),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2019),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2019),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2019),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2019),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2019),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_PHILOSOPHY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2019),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2019),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2019),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_MYTHOLOGY),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2019),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2019),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2020),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_SOLDIERY),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2020),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2020),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2020),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_LAW),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2020),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2020),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2020),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2020),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2020),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2020),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2020),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2021),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_AND_LITTLE_LADIES_DAY_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_ESOTERICS),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_FESTIVAL_2021_THE_HUNT_FOR_PAGEANTRY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2021),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2021),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2021),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2021),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_LORE),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2021),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2022),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2021),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_SCRIPTURE),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2022),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_VERITY),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2022),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_CREATION),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2022),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2023),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2023),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2023),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2023),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_MENDACITY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2023),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2023),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2023),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_10TH_ANNIVERSARY_HUNT),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2023),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2023),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2023),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_FIRST_HUNT_FOR_GENESIS),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2024),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2024),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_HATCHING_TIDE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_PATH_INFERNAL_2024),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_SECOND_HUNT_FOR_GENESIS),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2024),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2024),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024_1),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_GOETIA),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2024),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024_2),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2024),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2025),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2025),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2025),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2025),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2025),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2025),
                    character.HasQuest((int)QuestIds.EVENT_RISING_2025),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2025),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_2025),
                ];
                result.Add(completedQuests);
            }

            return result;
        }

        public static List<List<bool>> GetCharactersHildibrandQuests(List<Character> characters)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                List<bool> completedQuests =
                [
                    character.HasQuest((int)QuestIds.HILDIBRAND_ARR_THE_IMMACULATE_DECEPTION),
                    character.HasQuest((int)QuestIds.HILDIBRAND_ARR_THE_THREE_COLLECTORS),
                    character.HasQuest((int)QuestIds.HILDIBRAND_ARR_A_CASE_OF_INDECENCY),
                    character.HasQuest((int)QuestIds.HILDIBRAND_ARR_THE_COLISEUM_CONUNDRUM),
                    character.HasQuest((int)QuestIds.HILDIBRAND_ARR_HER_LAST_VOW),
                    character.HasQuest((int)QuestIds.HILDIBRAND_HW_DONT_CALL_IT_A_COMEBACK),
                    character.HasQuest((int)QuestIds.HILDIBRAND_HW_THE_MEASURE_OF_A_MAMMET),
                    character.HasQuest((int)QuestIds.HILDIBRAND_HW_DONT_TRUST_ANYONE_OVER_SIXTY),
                    character.HasQuest((int)QuestIds.HILDIBRAND_HW_IF_I_COULD_TURN_BACK_TIME),
                    character.HasQuest((int)QuestIds.HILDIBRAND_SB_A_HINGAN_TALE_NASHU_GOES_EAST),
                    character.HasQuest((int)QuestIds.HILDIBRAND_SB_OF_WOLVES_AND_GENTLEMEN),
                    character.HasQuest((int)QuestIds.HILDIBRAND_SB_THE_BLADE_MISLAID),
                    character.HasQuest((int)QuestIds.HILDIBRAND_SB_GOOD_SWORDS_GOOD_DOGS),
                    character.HasQuest((int)QuestIds.HILDIBRAND_SB_DONT_DO_THE_DEWPRISM),
                    character.HasQuest((int)QuestIds.HILDIBRAND_EW_A_SOULFUL_REUNION),
                    character.HasQuest((int)QuestIds.HILDIBRAND_EW_THE_IMPERFECT_GENTLEMAN),
                    character.HasQuest((int)QuestIds.HILDIBRAND_EW_GENERATIONAL_BONDING),
                    character.HasQuest((int)QuestIds.HILDIBRAND_EW_NOT_FROM_AROUND_HERE),
                    character.HasQuest((int)QuestIds.HILDIBRAND_EW_GENTLEMEN_AT_HEART),
                    character.HasQuest((int)QuestIds.HILDIBRAND_DT_THE_CASE_OF_THE_DISPLACED_INSPECTOR),
                    character.HasQuest((int)QuestIds.HILDIBRAND_DT_THE_CASE_OF_THE_FIENDISH_FUGITIVES),
                    character.HasQuest((int)QuestIds.HILDIBRAND_DT_ON_THE_TRAIL_OF_DESTRUCTION),
                ];
                result.Add(completedQuests);
            }

            return result;
        }

        public static List<List<bool>> GetCharactersRoleQuestQuests(List<Character> characters)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                List<bool> completedQuests =
                [
                    character.HasQuest((int)QuestIds.ROLEQUEST_SHB_TANK_TO_HAVE_LOVED_AND_LOST),
                    character.HasQuest((int)QuestIds.ROLEQUEST_SHB_PHYSICAL_COURAGE_BORN_OF_FEAR),
                    character.HasQuest((int)QuestIds.ROLEQUEST_SHB_MRDPS_A_TEARFUL_REUNION),
                    character.HasQuest((int)QuestIds.ROLEQUEST_SHB_HEALER_THE_SOUL_OF_TEMPERANCE),
                    character.HasQuest((int)QuestIds.ROLEQUEST_SHB_MASTER_SAFEKEEPING),
                    character.HasQuest((int)QuestIds.ROLEQUEST_EW_TANK_A_PATH_UNVEILED),
                    character.HasQuest((int)QuestIds.ROLEQUEST_EW_MELEE_TO_CALMER_SEAS),
                    character.HasQuest((int)QuestIds.ROLEQUEST_EW_PRDPS_LAID_TO_REST),
                    character.HasQuest((int)QuestIds.ROLEQUEST_EW_MRDPS_EVER_MARCH_HEAVENSWARD),
                    character.HasQuest((int)QuestIds.ROLEQUEST_EW_HEALER_THE_GIFT_OF_MERCY),
                    character.HasQuest((int)QuestIds.ROLEQUEST_EW_MASTER_FORLORN_GLORY),
                    character.HasQuest((int)QuestIds.ROLEQUEST_DT_TANK_DREAMS_OF_A_NEW_DAY),
                    character.HasQuest((int)QuestIds.ROLEQUEST_DT_MELEE_A_HUNTER_TRUE),
                    character.HasQuest((int)QuestIds.ROLEQUEST_DT_PRDPS_THE_MIGHTIEST_SHIELD),
                    character.HasQuest((int)QuestIds.ROLEQUEST_DT_MRDPS_HEROES_AND_PRETENDERS),
                    character.HasQuest((int)QuestIds.ROLEQUEST_DT_HEALER_AN_ANTIDOTE_FOR_ANARCHY),
                    character.HasQuest((int)QuestIds.ROLEQUEST_DT_MASTER_BAR_THE_PASSAGE)
                ];
                result.Add(completedQuests);
            }

            return result;
        }

        public static List<List<bool>> GetCharactersTribeQuests(List<Character> characters, GlobalCache globalCache, ClientLanguage currentLocale)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                List<bool> completedQuests = [];

                for (uint i = 1; i <= 20; i++)
                {
                    BeastTribeRank? rep = character.GetBeastReputation(i);
                    BeastTribes? beastTribe = globalCache.BeastTribesStorage.GetBeastTribe(currentLocale, i);
                    if (beastTribe == null || rep == null)
                    {
                        completedQuests.Add(false);
                        continue;
                    }

                    uint maxRank = (i is 12 or 13 or 14 or 18) ? beastTribe.MaxRank : 8;
                    bool done = maxRank == rep.Rank;
                    /*Plugin.Log.Debug(
                        $"{i}, cname: {character.FirstName}, name: {beastTribe?.EnglishName}, rep.Rank: {rep?.Rank}, maxrank: {beastTribe?.MaxRank}");*/

                    completedQuests.Add(done);
                }

                result.Add(completedQuests);
            }

            return result;
        }

        private static string GetClassJobCategoryFromId(ClientLanguage currentLocale, uint? id)
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
            return lumina != null ? lumina.Value.Name.ExtractText() : string.Empty;
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

        private static string GetItemRepairResource(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<ItemRepairResource>? dirr = Plugin.DataManager.GetExcelSheet<ItemRepairResource>(currentLocale);
            if (dirr == null)
            {
                return string.Empty;
            }

            ItemRepairResource? lumina = dirr.GetRow(id);
            RowRef<Item>? itm = lumina?.Item;
            Item? v = itm?.Value;
            return v != null ? v.Value.Name.ExtractText() : string.Empty;
        }

        // ReSharper disable once InconsistentNaming
        public static string GetFCTag(ClientLanguage currentLocale, GlobalCache globalCache, Character localPlayer)
        {
            // ReSharper disable once InconsistentNaming
            string FCTag = string.Empty;
            if (string.IsNullOrEmpty(localPlayer.CurrentWorld) || string.IsNullOrEmpty(localPlayer.CurrentDatacenter) || string.IsNullOrEmpty(localPlayer.CurrentWorld) || (localPlayer.CurrentWorld == localPlayer.HomeWorld && localPlayer.CurrentRegion == localPlayer.Region))
            {
                FCTag = localPlayer.FCTag;
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion == localPlayer.Region)
            {
                //FCTag = globalCache.AddonStorage.LoadAddonString(currentLocale, 12541);
                FCTag = currentLocale switch
                {
                    ClientLanguage.German => (localPlayer.Profile?.Gender == 0) ? "Wanderer" : "Wanderin",
                    ClientLanguage.English => globalCache.AddonStorage.LoadAddonString(currentLocale, 12541),
                    ClientLanguage.French => (localPlayer.Profile?.Gender == 0) ? "Baroudeur" : "Baroudeuse",
                    ClientLanguage.Japanese => globalCache.AddonStorage.LoadAddonString(currentLocale, 12541),
                    _ => globalCache.AddonStorage.LoadAddonString(currentLocale, 12541),
                };
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion != localPlayer.Region)
            {
                //FCTag = globalCache.AddonStorage.LoadAddonString(currentLocale, 12625);
                FCTag = currentLocale switch
                {
                    ClientLanguage.German => (localPlayer.Profile?.Gender == 0) ? "Reisender" : "Reisende",
                    ClientLanguage.English => globalCache.AddonStorage.LoadAddonString(currentLocale, 12625),
                    ClientLanguage.French => (localPlayer.Profile?.Gender == 0) ? "Explorateur" : "Exploratrice",
                    ClientLanguage.Japanese => globalCache.AddonStorage.LoadAddonString(currentLocale, 12625),
                    _ => globalCache.AddonStorage.LoadAddonString(currentLocale, 12625),
                };
            }
            else if (localPlayer.CurrentWorld != localPlayer.HomeWorld && localPlayer.CurrentRegion != localPlayer.Region)
            {
                //FCTag = globalCache.AddonStorage.LoadAddonString(currentLocale, 12627);
                FCTag = currentLocale switch
                {
                    ClientLanguage.German => (localPlayer.Profile?.Gender == 0) ? "Voyageur" : "Voyageurin",
                    ClientLanguage.English => globalCache.AddonStorage.LoadAddonString(currentLocale, 12627),
                    ClientLanguage.French => (localPlayer.Profile?.Gender == 0) ? "Voyageur" : "Voyageuse",
                    ClientLanguage.Japanese => globalCache.AddonStorage.LoadAddonString(currentLocale, 12627),
                    _ => globalCache.AddonStorage.LoadAddonString(currentLocale, 12627),
                };
            }
            //Plugin.Log.Debug($"localPlayerRegion : {localPlayerRegion}");
            //Plugin.Log.Debug($"localPlayer.CurrentRegion : {localPlayer.CurrentRegion}");
            return FCTag;
        }
        public static string GetCrystalName(ClientLanguage currentLocale, GlobalCache globalCache, uint i)
        {
            uint crystalId = i + 2;
            Item? itm = globalCache.ItemStorage.LoadItem(currentLocale, crystalId);
            return (itm is null) ? string.Empty : itm.Value.Name.ExtractText();
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

        public static Lumina.Excel.Sheets.PlaceName? GetPlaceName(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Lumina.Excel.Sheets.PlaceName>? dpn = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.PlaceName>(currentLocale);
            Lumina.Excel.Sheets.PlaceName? lumina = dpn?.GetRow(id);
            return lumina;
        }

        public static CharaMakeCustomize? GetHairstlyle(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<CharaMakeCustomize>? dh =
                Plugin.DataManager.GetExcelSheet<CharaMakeCustomize>(currentLocale);
            CharaMakeCustomize? lumina = dh?.GetRow(id);
            return lumina;
        }

        public static List<Models.PlaceName>? GetAllPlaceName()
        {
            List<Models.PlaceName> returnedPlaceNamesIds = [];
            ExcelSheet<Lumina.Excel.Sheets.PlaceName>? dpn = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.PlaceName>(ClientLanguage.English);
            using IEnumerator<Lumina.Excel.Sheets.PlaceName>? placeEnumerator = dpn?.GetEnumerator();
            if (placeEnumerator is null) return null;
            while (placeEnumerator.MoveNext())
            {
                Lumina.Excel.Sheets.PlaceName place = placeEnumerator.Current;
                if (place.Name.IsEmpty) continue;
                uint id = place.RowId;
                Models.PlaceName pn = new(){Id = id};
                List<ClientLanguage> langs =
                    [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
                foreach (ClientLanguage l in langs)
                {
                    if (l == ClientLanguage.English)
                    {
                        pn.EnglishName = place.Name.ExtractText();
                        pn.EnglishNoParticle = place.NameNoArticle.ExtractText();
                    }
                    Lumina.Excel.Sheets.PlaceName? cpn = GetPlaceName(l, id);
                    if (cpn == null) continue;
                    switch (l)
                    {
                        case ClientLanguage.German:
                            pn.GermanName = cpn.Value.Name.ExtractText();
                            pn.GermanNoParticle = cpn.Value.NameNoArticle.ExtractText();
                            break;
                        case ClientLanguage.French:
                            pn.FrenchName = cpn.Value.Name.ExtractText();
                            pn.FrenchNoParticle = cpn.Value.NameNoArticle.ExtractText();
                            break;
                        case ClientLanguage.Japanese:
                            pn.JapaneseName = cpn.Value.Name.ExtractText();
                            pn.JapaneseNoParticle = cpn.Value.NameNoArticle.ExtractText();
                            break;
                    }
                }

                returnedPlaceNamesIds.Add(pn);
            }

            return returnedPlaceNamesIds;
        }

        public static List<Hairstyle>? GetAllHairstlyles()
        {
            List<Hairstyle> hairstyles = [];
            ExcelSheet<CharaMakeCustomize>? dh =
                Plugin.DataManager.GetExcelSheet<CharaMakeCustomize>(ClientLanguage.English);
            using IEnumerator<CharaMakeCustomize>? hairstylesEnumerator = dh?.GetEnumerator();
            if (hairstylesEnumerator is null) return null;
            while (hairstylesEnumerator.MoveNext())
            {
                CharaMakeCustomize hairstyle = hairstylesEnumerator.Current;
                if (hairstyle.HintItem.Value.Name.IsEmpty) continue;

                Hairstyle h = new()
                {
                    Id = hairstyle.RowId,
                    Icon = hairstyle.Icon,
                    IsPurchasable = hairstyle.IsPurchasable,
                    SortKey = hairstyle.UnlockLink,
                    UnlockLink = hairstyle.UnlockLink,
                    ItemId = hairstyle.HintItem.Value.RowId,
                    FeatureId = hairstyle.FeatureID
                };
                List<ClientLanguage> langs =
                    [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
                foreach (ClientLanguage l in langs)
                {
                    if (hairstyle.UnlockLink == 228)
                    {
                        switch (l)
                        {
                            case ClientLanguage.German:
                                h.GermanName = "Ewigen Bundes";
                                break;
                            case ClientLanguage.English:
                                h.EnglishName = "Eternal Bonding";
                                break;
                            case ClientLanguage.French:
                                h.FrenchName = "Lien Éternel";
                                break;
                            case ClientLanguage.Japanese:
                                h.JapaneseName = "エターナルバンド";
                                break;
                        }
                    }
                    else
                    {
                        Item? i = GetItemFromId(l, hairstyle.HintItem.Value.RowId);
                        if (i == null) continue;
                        switch (l)
                        {
                            case ClientLanguage.German:
                                h.GermanName = i.Value.Name.ExtractText();
                                break;
                            case ClientLanguage.English:
                                h.EnglishName = i.Value.Name.ExtractText();
                                break;
                            case ClientLanguage.French:
                                h.FrenchName = i.Value.Name.ExtractText();
                                break;
                            case ClientLanguage.Japanese:
                                h.JapaneseName = i.Value.Name.ExtractText();
                                break;
                        }
                    }
                }
                hairstyles.Add(h);
            }

            return hairstyles;
        }

        public static int GetHairstyleIndexStart(Character character)
        {
            if (character.Profile == null) return 0;
            byte tribeId = character.Profile.Tribe;
            bool isMale = character.Profile.Gender == 0;

            const int numHair = 130;
            return tribeId switch
            {
                1 => isMale ? 0 : 1 * numHair, // Midlander
                2 => isMale ? 2 * numHair : 3 * numHair, // Highlander
                _ => (tribeId + 2) * numHair
            };

        }

        public static List<Armoire>? GetAllArmoire(ClientLanguage currentLocale)
        {
            List<Armoire> returnedIds = [];
            ExcelSheet<Cabinet>? cm = Plugin.DataManager.GetExcelSheet<Cabinet>(currentLocale);
            using IEnumerator<Cabinet>? cEnumerator = cm?.GetEnumerator();
            if (cEnumerator is null) return null;
            while (cEnumerator.MoveNext())
            {
                Cabinet c = cEnumerator.Current;
                if (c.Item.Value.Name.IsEmpty) continue;
                Armoire a = new()
                {
                    Id = c.RowId,
                    ItemId = c.Item.RowId,
                    Order = c.Order,
                    ArmoireCategory = c.Category.RowId,
                    ArmoireSubcategory = c.SubCategory.RowId
                };

                returnedIds.Add(a);
            }

            return returnedIds;
        }
        /*public static List<ArmoireSubCategory>? GetAllArmoireSubCategories(ClientLanguage currentLocale)
        {
            List<ArmoireSubCategory> returnedIds = [];
            ExcelSheet<CabinetSubCategory>? cm = Plugin.DataManager.GetExcelSheet<CabinetSubCategory>(currentLocale);
            using IEnumerator<CabinetSubCategory>? cEnumerator = cm?.GetEnumerator();
            if (cEnumerator is null) return null;
            while (cEnumerator.MoveNext())
            {
                CabinetSubCategory csc = cEnumerator.Current;
                if (csc.RowId == 0) continue;
                ArmoireSubCategory asc = new()
                {
                    Id = csc.RowId,
                    MenuOrder= csc.MenuOrder,
                };

                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        {
                            asc.GermanName = csc.Name.ExtractText();
                            break;
                        }
                    case ClientLanguage.English:
                        {
                            asc.EnglishName = csc.Name.ExtractText();
                            break;
                        }
                    case ClientLanguage.French:
                        {
                            asc.FrenchName = csc.Name.ExtractText();
                            break;
                        }
                    case ClientLanguage.Japanese:
                        {
                            asc.JapaneseName = csc.Name.ExtractText();
                            break;
                        }
                }

                returnedIds.Add(asc);
            }

            return returnedIds;
        }
        */


        public static List<TKey> FilterBySearch<TKey, TItem>(List<TKey> items, string searchText, ClientLanguage lang, Func<ClientLanguage, TKey, TItem?> getItemFunc, Func<TItem, string> getNameFunc)
            where TItem : class
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return items;

            string search = searchText.ToLowerInvariant();
            return items.Where(id => {
                TItem? item = getItemFunc(lang, id);
                if (item == null)
                    return false;

                string name = getNameFunc(item).ToLowerInvariant();
                return name.Contains(search);
            }).ToList();

            // This could eventually be made to also filter descriptions if we wanted the expand on this functionality
        }

        public static void DrawIcon(IDalamudTextureWrap? icon, Vector2 iconSize)
        {
            if (icon != null)
            {
                ImGui.Image(icon.Handle, iconSize);
            }
        }
        public static void DrawIcon(IDalamudTextureWrap? icon, Vector2 iconSize, Vector4 alpha)
        {
            if (icon != null)
            {
                ImGui.Image(icon.Handle, iconSize, Vector2.Zero, Vector2.One, alpha);
            }
        }

        public static string UnixTimeStampToDateTime(long lastOnline)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(lastOnline).ToLocalTime();
            return dateTime.ToString(CultureInfo.InvariantCulture);
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

        public enum TimeOptions
        {
            Normal = 0,
            Seconds = 1,
            Minutes = 2,
            Hours = 4,
            Days = 8,
        }

        private static TimeOptions TimeOption { get; set; } = TimeOptions.Normal;
        public static string GeneratePlaytime(TimeSpan time, bool withSeconds = false)
        {
            return TimeOption switch
            {
                TimeOptions.Normal => GeneratePlaytimeString(time, withSeconds),
                TimeOptions.Seconds => $"{time.TotalSeconds:n0} Seconds",
                TimeOptions.Minutes => $"{time.TotalMinutes:n0} Minutes",
                TimeOptions.Hours => $"{time.TotalHours:n2} Hours",
                TimeOptions.Days => $"{time.TotalDays:n2} Days",
                _ => GeneratePlaytimeString(time, withSeconds)
            };
        }

        private static string GeneratePlaytimeString(TimeSpan time, bool withSeconds = false)
        {
            if (time == TimeSpan.Zero)
            {
                return Loc.Localize("NoPlaytimeFound", "No playtime found, use /playtime");
            }
            string formatted =
                $"{(time.Days > 0 ? $"{time.Days:n0} {(time.Days == 1 ? "Day" : "Days")}, " : string.Empty)}" +
                $"{(time.Hours > 0 ? $"{time.Hours:n0} {(time.Hours == 1 ? "Hour" : "Hours")}, " : string.Empty)}" +
                $"{(time.Minutes > 0 ? $"{time.Minutes:n0} {(time.Minutes == 1 ? "Minute" : "Minutes")}, " : string.Empty)}";

            if (withSeconds)
                formatted += $"{time.Seconds:n0} {(time.Seconds == 1 ? "Second" : "Seconds")}";

            if (formatted.EndsWith(", "))
                formatted = formatted[..^2];

            return formatted;
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

public static class ListExt
{
    /// <summary> Return the first object fulfilling the predicate or null for structs. </summary>
    /// <param name="values"> The enumerable. </param>
    /// <param name="predicate"> The predicate. </param>
    /// <returns> The first object fulfilling the predicate, or a null-optional. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T? FirstOrNull<T>(this IEnumerable<T> values, Func<T, bool> predicate) where T : struct
    {
        foreach (T val in values)
            if (predicate(val))
                return val;

        return null;
    }

    public static bool TryGetFirst<T>(this IEnumerable<T> values, out T result) where T : struct
    {
        using IEnumerator<T> e = values.GetEnumerator();
        if (e.MoveNext())
        {
            result = e.Current;
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryGetFirst<T>(this IEnumerable<T> values, Predicate<T> predicate, out T result) where T : struct
    {
        using IEnumerator<T> e = values.GetEnumerator();
        while (e.MoveNext())
        {
            if (!predicate(e.Current))
            {
                continue;
            }

            result = e.Current;
            return true;
        }
        result = default;
        return false;
    }
}