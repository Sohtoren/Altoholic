using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
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
using Ornament = Lumina.Excel.GeneratedSheets.Ornament;
using Glasses = Lumina.Excel.GeneratedSheets.Glasses;
using Quest = Lumina.Excel.GeneratedSheets.Quest;
using Altoholic.Database;

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
            Plugin.ChatGui.Print($"[Altoholic]: {message}");
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
                                return lumina.Singular;
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
                                return lumina.Singular;
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
                                return lumina.Singular;
                        }
                        else
                        {
                            ExcelSheet<GCRankUldahFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankUldahFemaleText>(currentLocale);
                            GCRankUldahFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Singular;
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
                                return lumina.Singular;
                        }
                        else
                        {
                            ExcelSheet<GCRankGridaniaFemaleText>? dgcrlft =
                                Plugin.DataManager.GetExcelSheet<GCRankGridaniaFemaleText>(currentLocale);
                            GCRankGridaniaFemaleText? lumina = dgcrlft?.GetRow((uint)rank);
                            if (lumina != null)
                                return lumina.Singular;
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
                i.Name.RawString.Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return items;
        }

        public static Item? GetItemFromName(ClientLanguage currentLocale, string name)
        {
            ExcelSheet<Item>? ditm = Plugin.DataManager.GetExcelSheet<Item>(currentLocale);
            Item? item = ditm?.FirstOrDefault(i =>
                i.Name.RawString.Contains(name.ToLower(), StringComparison.CurrentCultureIgnoreCase));
            return item;
        }

        public static ItemLevel? GetItemLevelFromId(uint id)
        {
            ExcelSheet<ItemLevel>? dilvl = Plugin.DataManager.GetExcelSheet<ItemLevel>(ClientLanguage.English);
            ItemLevel? lumina = dilvl?.GetRow(id);
            return lumina;
        }

        public static Stain? GetStainFromId(uint id, ClientLanguage clientLanguage)
        {
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

        public static BeastReputationRank? GetBeastReputationRank(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<BeastReputationRank>? btm = Plugin.DataManager.GetExcelSheet<BeastReputationRank>(currentLocale);
            BeastReputationRank? lumina = btm?.GetRow(id);
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
                if (string.IsNullOrEmpty(bt.Name)) continue;
                if (bt.Icon == 0) continue;
                BeastTribes b = new() { Id = bt.RowId, Icon = bt.Icon, MaxRank = bt.MaxRank, DisplayOrder = bt.DisplayOrder };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        b.GermanName = bt.Name;
                        break;
                    case ClientLanguage.English:
                        b.EnglishName = bt.Name;
                        break;
                    case ClientLanguage.French:
                        b.FrenchName = bt.Name;
                        break;
                    case ClientLanguage.Japanese:
                        b.JapaneseName = bt.Name;
                        break;
                }

                returnedbtsIds.Add(b);
            }

            return returnedbtsIds;
        }

        private static Vector4 ConvertColorToVector4(uint color)
        {
            byte r = (byte)(color >> 24);
            byte g = (byte)(color >> 16);
            byte b = (byte)(color >> 8);
            byte a = (byte)color;

            return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
        public static void DrawReputationProgressBar(ClientLanguage currentLocale, GlobalCache globalCache, uint exp, bool isMax, uint reputationLevel, bool isAllied)
        {
            BeastReputationRank? brr = globalCache.BeastTribesStorage.GetRank(currentLocale, reputationLevel);
            if (brr == null) return;

            float progress = (float)exp / brr.RequiredReputation;
            ImGui.ProgressBar(progress, new Vector2(550, 10), "");

            using var charactersJobsJobLine = ImRaii.Table("###DrawReputationProgressBar#ReputationLine", 3);
            if (!charactersJobsJobLine) return;
            ImGui.TableSetupColumn("###DrawReputationProgressBar#ReputationLine#Level", ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###DrawReputationProgressBar#ReputationLine#Empty", ImGuiTableColumnFlags.WidthFixed, 190);
            ImGui.TableSetupColumn("###DrawReputationProgressBar#ReputationLine#Exp", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            UIColor? c = brr.Color.Value;
            if (c is not null)
            {
                ImGui.TextColored(ConvertColorToVector4(c.UIForeground),
                    isAllied ? $"{reputationLevel+1}. {brr.Name}" : $"{reputationLevel}. {brr.AlliedNames}");
            }
            else
            {
                ImGui.TextUnformatted($"{reputationLevel}. {brr.Name}");
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted($"{exp}/{brr.RequiredReputation}");
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
                if (!retainer)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawGearPiece(currentLocale, ref globalCache, gears, GearSlot.FACEWEAR,
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

        public static void DrawRetainerJob(ClientLanguage currentLocale, ref GlobalCache globalCache, uint job)
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
                ImGui.TextUnformatted(globalCache.AddonStorage.LoadAddonString(currentLocale, 379).Replace("ClassJob", globalCache.JobStorage.GetName(currentLocale, job)));
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
            ImGui.Image(texture.ImGuiHandle, size, uv0, uv1);
        }

        public static void DrawRetainerJobIconFromTexture(ref IDalamudTextureWrap texture, Models.ClassJob job,
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
            ImGui.Image(texture.ImGuiHandle, size, uv0, uv1);
        }

        public static void DrawGearPiece(ClientLanguage currentLocale, ref GlobalCache globalCache, List<Gear> gear,
            GearSlot slot, string tooltip, Vector2 iconSize,
            IDalamudTextureWrap? fallbackTexture, IDalamudTextureWrap? emptySlot)
        {
            if (fallbackTexture is null || emptySlot is null) return;
            Gear? foundGear = gear.FirstOrDefault(g => g.Slot == (short)slot);
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
                DrawIcon(globalCache.IconStorage.LoadIcon(i.Item.Icon, foundGear.HQ), iconSize);
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

        public static void DrawItemTooltip(ClientLanguage currentLocale, ref GlobalCache globalCache, Inventory item, bool armoire = false)
        {
            ItemItemLevel? itm = globalCache.ItemStorage.LoadItemWithItemLevel(currentLocale, item.ItemId);
            Item? dbItem = itm?.Item;
            if (dbItem == null) return;
            /*ItemLevel? ilvl = itm?.ItemLevel;
            Plugin.Log.Debug("ilvl is not null");*/

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
                DrawIcon(globalCache.IconStorage.LoadIcon(dbItem.Icon, item.HQ), new Vector2(40, 40));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{dbItem.Name} {(item.HQ ? (char)SeIconChar.HighQuality : "")}");
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
            Barding barding, uint icon)
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
            DrawIcon(globalCache.IconStorage.LoadIcon(icon), new Vector2(40, 40));
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

        public static Ornament? GetOrnament(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Ornament>? dm = Plugin.DataManager.GetExcelSheet<Ornament>(currentLocale);
            Ornament? lumina = dm?.GetRow(id);
            return lumina;
        }
        public static Lumina.Excel.GeneratedSheets2.OrnamentTransient? GetOrnamentTransient(ClientLanguage currentLocale, uint id)
        {
            ExcelSheet<Lumina.Excel.GeneratedSheets2.OrnamentTransient>? dmt = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets2.OrnamentTransient>(currentLocale);
            Lumina.Excel.GeneratedSheets2.OrnamentTransient? lumina = dmt?.GetRow(id);
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
                if (string.IsNullOrEmpty(ornament.Singular)) continue;
                if (ornament.Icon == 0) continue;
                Models.Ornament m = new() { Id = ornament.RowId, Icon = ornament.Icon, Transient = new Transient() };
                Lumina.Excel.GeneratedSheets2.OrnamentTransient? ct = GetOrnamentTransient(currentLocale, ornament.RowId);
                if (ct is null) continue;
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = ornament.Singular;
                        m.Transient.GermanDescription = ct.Unknown0;
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = ornament.Singular;
                        m.Transient.EnglishDescription = ct.Unknown0;
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = ornament.Singular;
                        m.Transient.FrenchDescription = ct.Unknown0;
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = ornament.Singular;
                        m.Transient.JapaneseDescription = ct.Unknown0;
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
                if (string.IsNullOrEmpty(glasses.Singular)) continue;
                if (glasses.Icon == 0) continue;
                Models.Glasses m = new() { Id = glasses.RowId, Icon = (uint)glasses.Icon };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        m.GermanName = glasses.Singular;
                        m.GermanDescription = glasses.Description;
                        break;
                    case ClientLanguage.English:
                        m.EnglishName = glasses.Singular;
                        m.EnglishDescription = glasses.Description;
                        break;
                    case ClientLanguage.French:
                        m.FrenchName = glasses.Singular;
                        m.FrenchDescription = glasses.Description;
                        break;
                    case ClientLanguage.Japanese:
                        m.JapaneseName = glasses.Singular;
                        m.JapaneseDescription = glasses.Description;
                        break;
                }

                returnedGlassessIds.Add(m);
            }

            return returnedGlassessIds;
        }

        public static List<uint> GetArmoireIds()
        {
            List<uint> returnedCabinetsIds = [];
            ExcelSheet<Cabinet>? dor = Plugin.DataManager.GetExcelSheet<Cabinet>(ClientLanguage.English);
            using IEnumerator<Cabinet>? cabinetEnumerator = dor?.GetEnumerator();
            if (cabinetEnumerator is null) return returnedCabinetsIds;
            while (cabinetEnumerator.MoveNext())
            {
                Cabinet cabinet = cabinetEnumerator.Current;
                Item? item = cabinet.Item?.Value;
                if (item == null) continue;

                returnedCabinetsIds.Add(item.RowId);
            }

            return returnedCabinetsIds;
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
            if(str.Length == 0) return str;
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

        public static Models.Quest? GetQuest(uint id)
        {
            Models.Quest q = new();
            List<ClientLanguage> langs =
                [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
            foreach (ClientLanguage l in langs)
            {
                ExcelSheet<Quest>? dbe = Plugin.DataManager.GetExcelSheet<Quest>(l);
                Quest? lumina = dbe?.GetRow(id);
                if (lumina == null) return null;
                switch (l)
                {
                    case ClientLanguage.German:
                        {
                            q.GermanName = lumina.Name;
                            break;
                        }
                    case ClientLanguage.English:
                        {
                            q.Id = lumina.RowId;
                            q.Icon = lumina.Icon;
                            q.EnglishName = lumina.Name;
                            break;
                        }
                    case ClientLanguage.French:
                        {
                            q.FrenchName = lumina.Name;
                            break;
                        }
                    case ClientLanguage.Japanese:
                        {
                            q.JapaneseName = lumina.Name;
                            break;
                        }

                }
            }

            return q;
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
                    character.HasQuest((int)QuestIds.MSQ_DAWNTRAIL)
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
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2024),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2024),
                    //character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2024),
                ];
                result.Add(completedQuests);
            }

            return result;
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