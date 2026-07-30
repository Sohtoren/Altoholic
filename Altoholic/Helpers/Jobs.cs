using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Altoholic.Helpers
{
    public class Jobs
    {
        /*public static Dictionary<uint, uint> GetPhantomJobsLevelExperience()
        {
            Dictionary<uint, uint> levels = new Dictionary<uint, uint>();
            ExcelSheet<MKDGrowData> dj = Plugin.DataManager.GetExcelSheet<MKDGrowData>(ClientLanguage.English);
            using IEnumerator<MKDGrowData> phEnumerator = dj.GetEnumerator();

            while (phEnumerator.MoveNext())
            {
                MKDGrowData data = phEnumerator.Current;
                levels.Add(data.RowId, data.Unknown0);
            }
            return levels;
        }*/
        public static Dictionary<uint, uint[]> GetPhantomJobsLevelExperience()
        {
            Dictionary<uint, uint[]> jobsExperiences = new Dictionary<uint, uint[]>();
            jobsExperiences.Add(1, [400, 2400, 3200, 4800, 6000, 16800]);
            jobsExperiences.Add(2, [2400, 12000, 0, 0, 0, 14400]);
            jobsExperiences.Add(3, [400, 2000, 4000, 4800, 5600, 16800]);
            jobsExperiences.Add(4, [400, 2000, 3200, 4800, 6400, 16800]);
            jobsExperiences.Add(5, [400, 3600, 6800, 8400, 0, 19200]);
            jobsExperiences.Add(6, [400, 4400, 9600, 0, 0, 14400]);
            jobsExperiences.Add(7, [400, 2400, 4800, 6800, 0, 14400]);
            jobsExperiences.Add(8, [400, 3600, 5600, 7200, 0, 16800]);
            jobsExperiences.Add(9, [400, 2000, 2800, 4000, 5200, 14400]);
            jobsExperiences.Add(10, [400, 5200, 8400, 0, 0, 14000]);
            jobsExperiences.Add(11, [400, 3600, 5600, 7200, 0, 16800]);
            jobsExperiences.Add(12, [400, 2400, 3200, 4800, 8400, 19200]);
            jobsExperiences.Add(13, [400, 4400, 9600, 0, 0, 14400]);
            jobsExperiences.Add(14, [400, 5200, 8800, 0, 0, 14400]);
            jobsExperiences.Add(15, [400, 6000, 8000, 0, 0, 14400]);
            jobsExperiences.Add(16, [600, 3600, 6000, 7800, 10800, 28800]);
            jobsExperiences.Add(17, [600, 5400, 8400, 10800, 0, 25200]);
            jobsExperiences.Add(18, [600, 5400, 8400, 10800, 0, 25200]);
            jobsExperiences.Add(19, [600, 9300, 11700, 0, 0, 21600]);
            jobsExperiences.Add(20, [1200, 6600, 9300, 11700, 0, 28800]);
            jobsExperiences.Add(21, [9900, 11700, 0, 0, 0, 21600]);
            jobsExperiences.Add(22, [600, 3600, 7200, 10200, 10200, 31800]);
            jobsExperiences.Add(23, [600, 7200, 9600, 11400, 0, 28800]);
            return jobsExperiences;
        }
        /*public static Dictionary<uint, uint> GetPhantomJobsLevelExperience()
        {
            Dictionary<uint, uint> levels = new Dictionary<uint, uint>();
            var dj = Plugin.DataManager.GetExcelSheet<MKDGrowDataSJob>(ClientLanguage.English);
            using IEnumerator<MKDGrowDataSJob> phEnumerator = dj.GetEnumerator();

            while (phEnumerator.MoveNext())
            {
                MKDGrowDataSJob data = phEnumerator.Current;
                data.
                levels.Add(data.RowId, data.Unknown0);
            }
            return levels;
        }*/

        public static Dictionary<uint, Models.PhantomJob> GetPhantomJobs()
        {
            Dictionary<uint, PhantomJob> jobs = [];
            for (uint i = 0; i < 24; i++)
            {
                List<ClientLanguage> langs =
                    [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
                Models.PhantomJob job = new();
                foreach (ClientLanguage l in langs)
                {
                    ExcelSheet<MKDSupportJob> dj = Plugin.DataManager.GetExcelSheet<MKDSupportJob>(l);
                    MKDSupportJob lumina = dj.GetRow(i);
                    switch (l)
                    {
                        case ClientLanguage.German:
                            {
                                job.Names.GermanName = lumina.Name.ExtractText();
                                job.Names.GermanNameShort = lumina.NameShort.ExtractText();
                                job.Names.GermanDescription = lumina.Description.ExtractText();
                                break;
                            }
                        case ClientLanguage.English:
                            {
                                job.Names.EnglishName = lumina.Name.ExtractText();
                                job.Names.EnglishNameShort = lumina.NameShort.ExtractText();
                                job.Names.EnglishDescription = lumina.Description.ExtractText();
                                job.JobIndex = lumina.JobIndex;
                                job.LevelMax = lumina.LevelMax;
                                for (int j = 0; j < 5; j++)
                                {
                                    var item = lumina.Actions[j];
                                    job.Action[j] = item.Action.RowId;
                                    job.LevelUnlock[j] = item.LevelUnlock;
                                }
                                break;
                            }
                        case ClientLanguage.French:
                            {
                                job.Names.FrenchName = lumina.Name.ExtractText();
                                job.Names.FrenchNameShort = lumina.NameShort.ExtractText();
                                job.Names.FrenchDescription = lumina.Description.ExtractText();
                                break;
                            }
                        case ClientLanguage.Japanese:
                            {
                                job.Names.JapaneseName = lumina.Name.ExtractText();
                                job.Names.JapaneseNameShort = lumina.NameShort.ExtractText();
                                job.Names.JapaneseDescription = lumina.Description.ExtractText();
                                break;
                            }
                    }
                }
                jobs.Add(i, job);
            }
            return jobs;
        }

        private static void DrawPhantomJobsFromTexture(ref IDalamudTextureWrap texture, uint job, Vector2 size)
        {
            (Vector2 uv0, Vector2 uv1) = job switch
            {
                0 => Utils.GetTextureCoordinate(texture.Size, 0, 0, 80, 116),
                1 => Utils.GetTextureCoordinate(texture.Size, 80, 0, 80, 116),
                2 => Utils.GetTextureCoordinate(texture.Size, 160, 0, 80, 116),
                3 => Utils.GetTextureCoordinate(texture.Size, 240, 0, 80, 116),
                4 => Utils.GetTextureCoordinate(texture.Size, 320, 0, 80, 116),
                5 => Utils.GetTextureCoordinate(texture.Size, 400, 0, 80, 116),
                6 => Utils.GetTextureCoordinate(texture.Size, 480, 0, 80, 116),
                7 => Utils.GetTextureCoordinate(texture.Size, 560, 0, 80, 116),
                8 => Utils.GetTextureCoordinate(texture.Size, 640, 0, 80, 116),
                9 => Utils.GetTextureCoordinate(texture.Size, 720, 0, 80, 116),
                10 => Utils.GetTextureCoordinate(texture.Size, 0, 116, 80, 116),
                11 => Utils.GetTextureCoordinate(texture.Size, 80, 116, 80, 116),
                12 => Utils.GetTextureCoordinate(texture.Size, 160, 116, 80, 116),
                13 => Utils.GetTextureCoordinate(texture.Size, 240, 116, 80, 116),
                14 => Utils.GetTextureCoordinate(texture.Size, 320, 116, 80, 116),
                15 => Utils.GetTextureCoordinate(texture.Size, 400, 116, 80, 116),
                16 => Utils.GetTextureCoordinate(texture.Size, 480, 116, 80, 116),
                17 => Utils.GetTextureCoordinate(texture.Size, 560, 116, 80, 116),
                18 => Utils.GetTextureCoordinate(texture.Size, 640, 116, 80, 116),
                19 => Utils.GetTextureCoordinate(texture.Size, 720, 116, 80, 116),
                20 => Utils.GetTextureCoordinate(texture.Size, 0, 232, 80, 116),
                21 => Utils.GetTextureCoordinate(texture.Size, 80, 232, 80, 116),
                22 => Utils.GetTextureCoordinate(texture.Size, 160, 232, 80, 116),
                23 => Utils.GetTextureCoordinate(texture.Size, 240, 232, 80, 116),
                24 => Utils.GetTextureCoordinate(texture.Size, 320, 232, 80, 116),
                25 => Utils.GetTextureCoordinate(texture.Size, 400, 232, 80, 116),
                26 => Utils.GetTextureCoordinate(texture.Size, 480, 232, 80, 116),
                27 => Utils.GetTextureCoordinate(texture.Size, 560, 232, 80, 116),
                28 => Utils.GetTextureCoordinate(texture.Size, 640, 232, 80, 116),
                29 => Utils.GetTextureCoordinate(texture.Size, 720, 232, 80, 116),
                30 => Utils.GetTextureCoordinate(texture.Size, 0, 348, 80, 116),
                31 => Utils.GetTextureCoordinate(texture.Size, 80, 348, 80, 116),
                32 => Utils.GetTextureCoordinate(texture.Size, 160, 348, 80, 116),
                33 => Utils.GetTextureCoordinate(texture.Size, 240, 348, 80, 116),
                34 => Utils.GetTextureCoordinate(texture.Size, 320, 348, 80, 116),
                35 => Utils.GetTextureCoordinate(texture.Size, 400, 348, 80, 116),
                36 => Utils.GetTextureCoordinate(texture.Size, 480, 348, 80, 116),
                37 => Utils.GetTextureCoordinate(texture.Size, 560, 348, 80, 116),
                38 => Utils.GetTextureCoordinate(texture.Size, 640, 348, 80, 116),
                39 => Utils.GetTextureCoordinate(texture.Size, 720, 348, 80, 116),
                40 => Utils.GetTextureCoordinate(texture.Size, 0, 464, 80, 116),
                41 => Utils.GetTextureCoordinate(texture.Size, 80, 464, 80, 116),
                42 => Utils.GetTextureCoordinate(texture.Size, 160, 464, 80, 116),
                43 => Utils.GetTextureCoordinate(texture.Size, 240, 464, 80, 116),
                44 => Utils.GetTextureCoordinate(texture.Size, 320, 464, 80, 116),
                45 => Utils.GetTextureCoordinate(texture.Size, 400, 464, 80, 116),
                46 => Utils.GetTextureCoordinate(texture.Size, 480, 464, 80, 116),
                47 => Utils.GetTextureCoordinate(texture.Size, 560, 464, 80, 116),
                48 => Utils.GetTextureCoordinate(texture.Size, 640, 464, 80, 116),
                49 => Utils.GetTextureCoordinate(texture.Size, 720, 464, 80, 116),
                50 => Utils.GetTextureCoordinate(texture.Size, 0, 580, 80, 116),
                51 => Utils.GetTextureCoordinate(texture.Size, 80, 580, 80, 116),
                52 => Utils.GetTextureCoordinate(texture.Size, 160, 580, 80, 116),
                53 => Utils.GetTextureCoordinate(texture.Size, 240, 580, 80, 116),
                54 => Utils.GetTextureCoordinate(texture.Size, 320, 580, 80, 116),
                55 => Utils.GetTextureCoordinate(texture.Size, 400, 580, 80, 116),
                56 => Utils.GetTextureCoordinate(texture.Size, 480, 580, 80, 116),
                57 => Utils.GetTextureCoordinate(texture.Size, 560, 580, 80, 116),
                58 => Utils.GetTextureCoordinate(texture.Size, 640, 580, 80, 116),
                59 => Utils.GetTextureCoordinate(texture.Size, 720, 580, 80, 116),
                _ => Utils.GetTextureCoordinate(texture.Size, 0, 0, 0, 0)
            };
            ImGui.Image(texture.Handle, size, uv0, uv1);
        }
        private static void DrawPhantomJobsIconsFromTexture(ref IDalamudTextureWrap texture, uint job, Vector2 size)
        {
            (Vector2 uv0, Vector2 uv1) = job switch
            {
                0 => Utils.GetTextureCoordinate(texture.Size, 0, 0, 56, 56),
                1 => Utils.GetTextureCoordinate(texture.Size, 0, 56, 56, 56),
                2 => Utils.GetTextureCoordinate(texture.Size, 112, 0, 56, 56),
                3 => Utils.GetTextureCoordinate(texture.Size, 168, 0, 56, 56),
                4 => Utils.GetTextureCoordinate(texture.Size, 224, 0, 56, 56),
                5 => Utils.GetTextureCoordinate(texture.Size, 0, 56, 56, 56),
                6 => Utils.GetTextureCoordinate(texture.Size, 56, 56, 56, 56),
                7 => Utils.GetTextureCoordinate(texture.Size, 112, 56, 56, 56),
                8 => Utils.GetTextureCoordinate(texture.Size, 168, 56, 56, 56),
                9 => Utils.GetTextureCoordinate(texture.Size, 224, 56, 56, 56),
                10 => Utils.GetTextureCoordinate(texture.Size, 0, 112, 56, 56),
                11 => Utils.GetTextureCoordinate(texture.Size, 56, 112, 56, 56),
                12 => Utils.GetTextureCoordinate(texture.Size, 112, 112, 56, 56),
                13 => Utils.GetTextureCoordinate(texture.Size, 168, 112, 56, 56),
                14 => Utils.GetTextureCoordinate(texture.Size, 224, 112, 56, 56),
                15 => Utils.GetTextureCoordinate(texture.Size, 0, 168, 56, 56),
                16 => Utils.GetTextureCoordinate(texture.Size, 56, 168, 56, 56),
                17 => Utils.GetTextureCoordinate(texture.Size, 112, 168, 56, 56),
                18 => Utils.GetTextureCoordinate(texture.Size, 168, 168, 56, 56),
                19 => Utils.GetTextureCoordinate(texture.Size, 224, 168, 56, 56),
                20 => Utils.GetTextureCoordinate(texture.Size, 0, 224, 56, 56),
                21 => Utils.GetTextureCoordinate(texture.Size, 56, 224, 56, 56),
                22 => Utils.GetTextureCoordinate(texture.Size, 112, 224, 56, 56),
                23 => Utils.GetTextureCoordinate(texture.Size, 168, 224, 56, 56),
                24 => Utils.GetTextureCoordinate(texture.Size, 224, 224, 56, 56),
                25 => Utils.GetTextureCoordinate(texture.Size, 0, 280, 56, 56),
                26 => Utils.GetTextureCoordinate(texture.Size, 56, 280, 56, 56),
                27 => Utils.GetTextureCoordinate(texture.Size, 112, 280, 56, 56),
                28 => Utils.GetTextureCoordinate(texture.Size, 168, 280, 56, 56),
                29 => Utils.GetTextureCoordinate(texture.Size, 224, 280, 56, 56),
                _ => Utils.GetTextureCoordinate(texture.Size, 0, 0, 0, 0)
            };
            ImGui.Image(texture.Handle, size, uv0, uv1);
        }

        public static void DrawPhantomJobs(IDalamudTextureWrap? phantomJobTexture, IDalamudTextureWrap? phantomJobsIconsTexture, Cache.GlobalCache globalCache, ClientLanguage currentLocale, Character selectedCharacter)
        {
            if (phantomJobTexture is null) return;
            if (phantomJobsIconsTexture is null) return;
            if (selectedCharacter.OccultCrescent is null) return;

            using var charactersJobsPhantomJobs = ImRaii.Table($"###CharactersJobs#PhantomJobs#{selectedCharacter.CharacterId}", 3,
                ImGuiTableFlags.ScrollY);
            if (!charactersJobsPhantomJobs) return;

            ImGui.TableSetupColumn($"###CharactersJobs#PhantomJobs#Phantom#{selectedCharacter.CharacterId}#1", ImGuiTableColumnFlags.WidthFixed, 250);
            ImGui.TableSetupColumn($"###CharactersJobs#PhantomJobs#Phantom#{selectedCharacter.CharacterId}#2", ImGuiTableColumnFlags.WidthFixed, 250);
            ImGui.TableSetupColumn($"###CharactersJobs#PhantomJobs#Phantom#{selectedCharacter.CharacterId}#3", ImGuiTableColumnFlags.WidthFixed, 250);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawPhantomJob(phantomJobTexture, phantomJobsIconsTexture, globalCache, currentLocale, selectedCharacter, 0);
            int count = 1;
            for (uint i = 1; i < 24; i++)
            {
                if (selectedCharacter.OccultCrescent.Jobs[i] == 0) 
                {
                    count -= 1;
                    continue; 
                }
                if (count % 3 == 0)
                {
                    ImGui.TableNextRow();
                }
                ImGui.TableNextColumn();
                DrawPhantomJob(phantomJobTexture, phantomJobsIconsTexture, globalCache, currentLocale, selectedCharacter, i);
                count++;
            }
        }

        private static void DrawPhantomJob(IDalamudTextureWrap? phantomJobTexture, IDalamudTextureWrap? phantomJobsIconsTexture, Cache.GlobalCache globalCache, ClientLanguage currentLocale, Character selectedCharacter, uint jobId)
        {
            if (phantomJobTexture is null) return;
            if (phantomJobsIconsTexture is null) return;
            if (selectedCharacter.OccultCrescent is null) return;

            PhantomJob pj = globalCache.JobStorage.GetPhantomJob(jobId);
            (string name, string description) = currentLocale switch
            {
                ClientLanguage.German => (pj.Names.GermanName, pj.Names.GermanDescription),
                ClientLanguage.English => (pj.Names.EnglishName, pj.Names.EnglishDescription),
                ClientLanguage.French => (pj.Names.FrenchName, pj.Names.FrenchDescription),
                ClientLanguage.Japanese => (pj.Names.JapaneseName, pj.Names.JapaneseDescription),
                _ => (pj.Names.EnglishName, pj.Names.EnglishDescription)
            };
            using (var charactersJobsJobLine = ImRaii.Table("###CharactersJobs#JobLine", 2))
            {
                if (!charactersJobsJobLine) return;
                ImGui.TableSetupColumn("###CharactersJobs#Icon", ImGuiTableColumnFlags.WidthFixed, 45);
                ImGui.TableSetupColumn("###CharactersJobs#LevelNameExp", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawPhantomJobsFromTexture(ref phantomJobTexture, jobId, new Vector2(40, 58));
                ImGui.TableSetColumnIndex(1);
                using (var charactersJobsJobLevelNameExp = ImRaii.Table("###CharactersJobs#JobLevelNameExp", 3))
                {
                    if (!charactersJobsJobLevelNameExp) return;
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#Icon", ImGuiTableColumnFlags.WidthFixed, 30);
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#Level", ImGuiTableColumnFlags.WidthFixed, 20);
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#NameExp", ImGuiTableColumnFlags.WidthFixed, 180);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawPhantomJobsIconsFromTexture(ref phantomJobsIconsTexture, jobId, new Vector2(28, 28));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{globalCache.AddonStorage.LoadAddonString(currentLocale, 464)}: {selectedCharacter.OccultCrescent.Jobs[jobId]}");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(description);
                    ImGui.EndTooltip();
                }
                if (jobId > 0)
                {
                    Utils.DrawLevelProgressBar((int)selectedCharacter.OccultCrescent.JobsExperiences[jobId],
                        (int)globalCache.JobStorage.GetPhantomJobExperience(jobId, selectedCharacter.OccultCrescent.Jobs[jobId]),
                        name, (int)selectedCharacter.OccultCrescent.JobsExperiences[jobId] > 0, selectedCharacter.OccultCrescent.Jobs[jobId] == pj.LevelMax);
                }
            }
        }

        public static Job? GetCharacterJob(uint i, Character character)
        {
            if (character.Jobs is null) return null;
            return i switch
            {
                1 => character.Jobs.Gladiator,
                19 => character.Jobs.Paladin,
                3 => character.Jobs.Marauder,
                21 => character.Jobs.Warrior,
                32 => character.Jobs.DarkKnight,
                37 => character.Jobs.Gunbreaker,
                6 => character.Jobs.Conjurer,
                24 => character.Jobs.WhiteMage,
                28 => character.Jobs.Scholar,
                33 => character.Jobs.Astrologian,
                40 => character.Jobs.Sage,
                2 => character.Jobs.Pugilist,
                20 => character.Jobs.Monk,
                4 => character.Jobs.Lancer,
                22 => character.Jobs.Dragoon,
                29 => character.Jobs.Rogue,
                30 => character.Jobs.Ninja,
                34 => character.Jobs.Samurai,
                39 => character.Jobs.Reaper,
                41 => character.Jobs.Viper,
                43 => character.Jobs.Beastmaster,
                5 => character.Jobs.Archer,
                31 => character.Jobs.Machinist,
                23 => character.Jobs.Bard,
                38 => character.Jobs.Dancer,
                7 => character.Jobs.Thaumaturge,
                25 => character.Jobs.BlackMage,
                26 => character.Jobs.Arcanist,
                27 => character.Jobs.Summoner,
                35 => character.Jobs.RedMage,
                42 => character.Jobs.Pictomancer,
                36 => character.Jobs.BlueMage,
                8 => character.Jobs.Carpenter,
                9 => character.Jobs.Blacksmith,
                10 => character.Jobs.Armorer,
                11 => character.Jobs.Goldsmith,
                12 => character.Jobs.Leatherworker,
                13 => character.Jobs.Weaver,
                14 => character.Jobs.Alchemist,
                15 => character.Jobs.Culinarian,
                16 => character.Jobs.Miner,
                17 => character.Jobs.Botanist,
                18 => character.Jobs.Fisher,
                _ => null
            };
        }
    }
}
