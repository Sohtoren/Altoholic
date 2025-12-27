using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class JobsWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;
        public JobsWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache) 
            : base(
                name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;
            //_currentLocale = currentLocale;

            _rolesTextureWrap = _globalCache.IconStorage.LoadRoleIconTexture();
        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;
        private IDalamudTextureWrap? _rolesTextureWrap;

        public void Dispose()
        {

        }
        public void Clear()
        {
            Plugin.Log.Info("JobsWindow, Clear() called");
            _currentCharacter = null;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using var charactersJobsTable = ImRaii.Table("###CharactersJobsTable", 2);
                if (!charactersJobsTable) return;
                ImGui.TableSetupColumn("###CharactersJobsTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed,
                    210);
                ImGui.TableSetupColumn("###CharactersJobsTable#Jobs", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (var listbox = ImRaii.ListBox("###CharactersJobsTable#CharactersListBox", new Vector2(200, -1)))
                {
                    if (listbox)
                    {
                        if (chars.Count > 0)
                        {
                            if (ImGui.Selectable(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersJobsTable#CharactersListBox#All",
                                    _currentCharacter == null))
                            {
                                _currentCharacter = null;
                            }

#if DEBUG
                            for (int i = 0; i < 15; i++)
                            {
                                chars.Add(new Character()
                                {
                                    FirstName = $"Dummy {i}",
                                    LastName = $"LN {i}",
                                    HomeWorld = $"Homeworld {i}",
                                });
                            }
#endif

                            foreach (var currChar in chars.Where(currChar =>
                                         ImGui.Selectable(
                                             $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                             currChar == _currentCharacter)))
                            {
                                _currentCharacter = currChar;
                            }
                        }
                    }
                }

                ImGui.TableSetColumnIndex(1);
                if (_currentCharacter is not null)
                {
                    DrawJobs(_currentCharacter);
                }
                else
                {
                    DrawAll(chars);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic : Exception : {0}", e);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            if (chars.Count == 0) return;
            using var charactersJobsAll = ImRaii.Table("###CharactersJobs#All", 43,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersJobsAll) return;
            // ImGuiCol.TableRowBg = new Vector4(255, 255, 255, 1); ;
            ImGui.TableSetupColumn("###CharactersJobs#All#Names", ImGuiTableColumnFlags.WidthFixed, 35);
            ImGui.TableSetupColumn("###CharactersJobs#All#GLA", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#PLD", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#MRD", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#WAR", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#DRK", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#GNB", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#CNJ", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#WHM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#SCH", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#AST", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#SGE", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#PGL", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#MNK", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#LNC", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#DRG", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#ROG", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#NIN", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#SAM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#RPR", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#VPR", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#ARC", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#BRD", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#MCH", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#DNC", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#THM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#BLM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#ACN", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#SMN", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#RDM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#PCT", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#BLU", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#CRP", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#BSM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#ARM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#GSM", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#LTW", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#WVR", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#ALC", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#CUL", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#MIN", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#BTN", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("###CharactersJobs#All#FSH", ImGuiTableColumnFlags.WidthFixed, 25);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14055));
                ImGui.EndTooltip();
            }

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.GLA, true));
            HoverJobName((uint)ClassJob.GLA);

            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.PLD, true));
            HoverJobName((uint)ClassJob.PLD);

            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.MRD, true));
            HoverJobName((uint)ClassJob.MRD);

            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.WAR, true));
            HoverJobName((uint)ClassJob.WAR);

            ImGui.TableSetColumnIndex(5);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.DRK, true));
            HoverJobName((uint)ClassJob.DRK);

            ImGui.TableSetColumnIndex(6);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.GNB, true));
            HoverJobName((uint)ClassJob.GNB);

            ImGui.TableSetColumnIndex(7);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.CNJ, true));
            HoverJobName((uint)ClassJob.CNJ);

            ImGui.TableSetColumnIndex(8);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.WHM, true));
            HoverJobName((uint)ClassJob.WHM);

            ImGui.TableSetColumnIndex(9);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SCH, true));
            HoverJobName((uint)ClassJob.SCH);

            ImGui.TableSetColumnIndex(10);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.AST, true));
            HoverJobName((uint)ClassJob.AST);

            ImGui.TableSetColumnIndex(11);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SGE, true));
            HoverJobName((uint)ClassJob.SGE);

            ImGui.TableSetColumnIndex(12);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.PGL, true));
            HoverJobName((uint)ClassJob.PGL);

            ImGui.TableSetColumnIndex(13);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.MNK, true));
            HoverJobName((uint)ClassJob.MNK);

            ImGui.TableSetColumnIndex(14);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.LNC, true));
            HoverJobName((uint)ClassJob.LNC);

            ImGui.TableSetColumnIndex(15);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.DRG, true));
            HoverJobName((uint)ClassJob.DRG);

            ImGui.TableSetColumnIndex(16);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ROG, true));
            HoverJobName((uint)ClassJob.ROG);

            ImGui.TableSetColumnIndex(17);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.NIN, true));
            HoverJobName((uint)ClassJob.NIN);

            ImGui.TableSetColumnIndex(18);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SAM, true));
            HoverJobName((uint)ClassJob.SAM);

            ImGui.TableSetColumnIndex(19);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.RPR, true));
            HoverJobName((uint)ClassJob.RPR);

            ImGui.TableSetColumnIndex(20);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.VPR, true));
            HoverJobName((uint)ClassJob.VPR);

            ImGui.TableSetColumnIndex(21);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ARC, true));
            HoverJobName((uint)ClassJob.ARC);

            ImGui.TableSetColumnIndex(22);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.BRD, true));
            HoverJobName((uint)ClassJob.BRD);

            ImGui.TableSetColumnIndex(23);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.MCH, true));
            HoverJobName((uint)ClassJob.MCH);

            ImGui.TableSetColumnIndex(24);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.DNC, true));
            HoverJobName((uint)ClassJob.DNC);

            ImGui.TableSetColumnIndex(25);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.THM, true));
            HoverJobName((uint)ClassJob.THM);

            ImGui.TableSetColumnIndex(26);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.BLM, true));
            HoverJobName((uint)ClassJob.BLM);

            ImGui.TableSetColumnIndex(27);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ACN, true));
            HoverJobName((uint)ClassJob.ACN);

            ImGui.TableSetColumnIndex(28);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SMN, true));
            HoverJobName((uint)ClassJob.SMN);

            ImGui.TableSetColumnIndex(29);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.RDM, true));
            HoverJobName((uint)ClassJob.RDM);

            ImGui.TableSetColumnIndex(30);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.PCT, true));
            HoverJobName((uint)ClassJob.PCT);

            ImGui.TableSetColumnIndex(31);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.BLU));
            HoverJobName((uint)ClassJob.BLU);

            ImGui.TableSetColumnIndex(32);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.CRP, true));
            HoverJobName((uint)ClassJob.CRP);

            ImGui.TableSetColumnIndex(33);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.BSM, true));
            HoverJobName((uint)ClassJob.BSM);

            ImGui.TableSetColumnIndex(34);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ARM, true));
            HoverJobName((uint)ClassJob.ARM);

            ImGui.TableSetColumnIndex(35);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.GSM, true));
            HoverJobName((uint)ClassJob.MRD);

            ImGui.TableSetColumnIndex(36);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.LTW, true));
            HoverJobName((uint)ClassJob.LTW);

            ImGui.TableSetColumnIndex(37);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.WVR, true));
            HoverJobName((uint)ClassJob.WVR);

            ImGui.TableSetColumnIndex(38);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ALC, true));
            HoverJobName((uint)ClassJob.ALC);

            ImGui.TableSetColumnIndex(39);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.CUL, true));
            HoverJobName((uint)ClassJob.CUL);

            ImGui.TableSetColumnIndex(40);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.MIN, true));
            HoverJobName((uint)ClassJob.MIN);

            ImGui.TableSetColumnIndex(41);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.BTN, true));
            HoverJobName((uint)ClassJob.BTN);

            ImGui.TableSetColumnIndex(42);
            ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.FSH, true));
            HoverJobName((uint)ClassJob.FSH);

            foreach (Character currChar in chars)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }

                if (currChar.Jobs is null) continue;
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{currChar.Jobs.Gladiator.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.GLA);
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{currChar.Jobs.Paladin.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.PLD);
                ImGui.TableSetColumnIndex(3);
                ImGui.TextUnformatted($"{currChar.Jobs.Marauder.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.MRD);
                ImGui.TableSetColumnIndex(4);
                ImGui.TextUnformatted($"{currChar.Jobs.Warrior.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.WAR);
                ImGui.TableSetColumnIndex(5);
                ImGui.TextUnformatted($"{currChar.Jobs.DarkKnight.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.DRK);
                ImGui.TableSetColumnIndex(6);
                ImGui.TextUnformatted($"{currChar.Jobs.Gunbreaker.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.GNB);
                ImGui.TableSetColumnIndex(7);
                ImGui.TextUnformatted($"{currChar.Jobs.Conjurer.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.CNJ);
                ImGui.TableSetColumnIndex(8);
                ImGui.TextUnformatted($"{currChar.Jobs.WhiteMage.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.WHM);
                ImGui.TableSetColumnIndex(9);
                ImGui.TextUnformatted($"{currChar.Jobs.Scholar.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.SCH);
                ImGui.TableSetColumnIndex(10);
                ImGui.TextUnformatted($"{currChar.Jobs.Astrologian.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.AST);
                ImGui.TableSetColumnIndex(11);
                ImGui.TextUnformatted($"{currChar.Jobs.Sage.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.SGE);
                ImGui.TableSetColumnIndex(12);
                ImGui.TextUnformatted($"{currChar.Jobs.Pugilist.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.PGL);
                ImGui.TableSetColumnIndex(13);
                ImGui.TextUnformatted($"{currChar.Jobs.Monk.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.MNK);
                ImGui.TableSetColumnIndex(14);
                ImGui.TextUnformatted($"{currChar.Jobs.Lancer.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.LNC);
                ImGui.TableSetColumnIndex(15);
                ImGui.TextUnformatted($"{currChar.Jobs.Dragoon.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.DRG);
                ImGui.TableSetColumnIndex(16);
                ImGui.TextUnformatted($"{currChar.Jobs.Rogue.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.ROG);
                ImGui.TableSetColumnIndex(17);
                ImGui.TextUnformatted($"{currChar.Jobs.Ninja.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.NIN);
                ImGui.TableSetColumnIndex(18);
                ImGui.TextUnformatted($"{currChar.Jobs.Samurai.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.SAM);
                ImGui.TableSetColumnIndex(19);
                ImGui.TextUnformatted($"{currChar.Jobs.Reaper.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.RPR);
                ImGui.TableSetColumnIndex(20);
                ImGui.TextUnformatted($"{currChar.Jobs.Viper.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.VPR);
                ImGui.TableSetColumnIndex(21);
                ImGui.TextUnformatted($"{currChar.Jobs.Archer.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.ARC);
                ImGui.TableSetColumnIndex(22);
                ImGui.TextUnformatted($"{currChar.Jobs.Bard.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.BRD);
                ImGui.TableSetColumnIndex(23);
                ImGui.TextUnformatted($"{currChar.Jobs.Machinist.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.MCH);
                ImGui.TableSetColumnIndex(24);
                ImGui.TextUnformatted($"{currChar.Jobs.Dancer.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.DNC);
                ImGui.TableSetColumnIndex(25);
                ImGui.TextUnformatted($"{currChar.Jobs.Thaumaturge.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.THM);
                ImGui.TableSetColumnIndex(26);
                ImGui.TextUnformatted($"{currChar.Jobs.BlackMage.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.BLM);
                ImGui.TableSetColumnIndex(27);
                ImGui.TextUnformatted($"{currChar.Jobs.Arcanist.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.ACN);
                ImGui.TableSetColumnIndex(28);
                ImGui.TextUnformatted($"{currChar.Jobs.Summoner.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.SMN);
                ImGui.TableSetColumnIndex(29);
                ImGui.TextUnformatted($"{currChar.Jobs.RedMage.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.RDM);
                ImGui.TableSetColumnIndex(30);
                ImGui.TextUnformatted($"{currChar.Jobs.Pictomancer.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.PCT);
                ImGui.TableSetColumnIndex(31);
                ImGui.TextUnformatted($"{currChar.Jobs.BlueMage.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.BLU);
                ImGui.TableSetColumnIndex(32);
                ImGui.TextUnformatted($"{currChar.Jobs.Carpenter.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.CRP);
                ImGui.TableSetColumnIndex(33);
                ImGui.TextUnformatted($"{currChar.Jobs.Blacksmith.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.BSM);
                ImGui.TableSetColumnIndex(34);
                ImGui.TextUnformatted($"{currChar.Jobs.Armorer.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.ARM);
                ImGui.TableSetColumnIndex(35);
                ImGui.TextUnformatted($"{currChar.Jobs.Goldsmith.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.GSM);
                ImGui.TableSetColumnIndex(36);
                ImGui.TextUnformatted($"{currChar.Jobs.Leatherworker.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.LTW);
                ImGui.TableSetColumnIndex(37);
                ImGui.TextUnformatted($"{currChar.Jobs.Weaver.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.WVR);
                ImGui.TableSetColumnIndex(38);
                ImGui.TextUnformatted($"{currChar.Jobs.Alchemist.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.ALC);
                ImGui.TableSetColumnIndex(39);
                ImGui.TextUnformatted($"{currChar.Jobs.Culinarian.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.CUL);
                ImGui.TableSetColumnIndex(40);
                ImGui.TextUnformatted($"{currChar.Jobs.Miner.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.MIN);
                ImGui.TableSetColumnIndex(41);
                ImGui.TextUnformatted($"{currChar.Jobs.Botanist.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.BTN);
                ImGui.TableSetColumnIndex(42);
                ImGui.TextUnformatted($"{currChar.Jobs.Fisher.Level}");
                HoverCharNameJobName(currChar,(uint)ClassJob.FSH);
            }
        }

        private void HoverJobName(uint job)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, job));
                ImGui.EndTooltip();
            }
        }
        private void HoverCharNameJobName(Character character, uint job)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, job));
                ImGui.EndTooltip();
            }
        }
        private void DrawJobs(Character selectedCharacter)
        {
            using var tabBar = ImRaii.TabBar($"###CharactersJobs#JobsTabs#{selectedCharacter.CharacterId}");
            if (!tabBar.Success) return;
            using (var DoWDoMTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1080)}###CharactersJobs#JobsTabs#DoWDoM#{selectedCharacter.CharacterId}"))
            {
                if (DoWDoMTab)
                {
                    DrawDoWDoMJobs(selectedCharacter);
                }
            }
            using (var DoHDoLTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1081)}###CharactersJobs#JobsTabs#DoHDoL#{selectedCharacter.CharacterId}"))
            {
                if (DoHDoLTab)
                {
                    DrawDoHDoLJobs(selectedCharacter);
                }
            }
        }

        private void DrawDoWDoMJobs(Character selectedCharacter)
        {
            if (_rolesTextureWrap is null) return;
            using var charactersJobsDoWDoMJobs = ImRaii.Table($"###CharactersJobs#DoWDoMJobs#{selectedCharacter.CharacterId}", 2,
                ImGuiTableFlags.ScrollY);
            if (!charactersJobsDoWDoMJobs) return;
            ImGui.TableSetupColumn($"###CharactersJobs#DoW#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoM#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var charactersJobsDoWRoleTank = ImRaii.Table("###CharactersJobs#DoW#RoleTank", 2))
            {
                if (!charactersJobsDoWRoleTank) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleTank#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleTank#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Tank, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1082)}"); // Tank
            }

            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoMRoleHealer = ImRaii.Table("###CharactersJobs#DoM#RoleHealer", 2))
            {
                if (!charactersJobsDoMRoleHealer) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleHealer#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleHealer#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Heal, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1083)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.GLA);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.CNJ);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.MRD);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.SCH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.DRK);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.AST);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.GNB);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.SGE);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var charactersJobsDoWRoleMelee = ImRaii.Table("###CharactersJobs#DoW#RoleMelee", 2))
            {
                if (!charactersJobsDoWRoleMelee) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Melee, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1084)}");
            }

            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoWRolePhysicalRanged =
                   ImRaii.Table("###CharactersJobs#DoW#RolePhysicalRanged", 2))
            {
                if (!charactersJobsDoWRolePhysicalRanged) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RolePhysicalRanged#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RolePhysicalRanged#Name",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Ranged, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1085)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.PGL);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.ARC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.LNC);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.MCH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.ROG);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.DNC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.SAM);

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoMRoleMagicalRanged = ImRaii.Table("###CharactersJobs#DoM#RoleMagicalRanged", 2))
            {
                if (!charactersJobsDoMRoleMagicalRanged) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Icon", ImGuiTableColumnFlags.WidthFixed,
                    22);
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Name",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Caster, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1086)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.RPR);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.THM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.VPR);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.SMN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.RDM);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.PCT);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.BLU);
        }

        private void DrawDoHDoLJobs(Character selectedCharacter)
        {
            if (_rolesTextureWrap is null) return;
            using var charactersJobsDoHDoLJobs =
                ImRaii.Table($"###CharactersJobs#DoHDoLJobs#{selectedCharacter.CharacterId}", 2);
            if (!charactersJobsDoHDoLJobs) return;
            ImGui.TableSetupColumn($"###CharactersJobs#DoH#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoL#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var charactersJobsDoHRoleDoH = ImRaii.Table("###CharactersJobs#DoH#RoleDoH", 2))
            {
                if (!charactersJobsDoHRoleDoH) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Crafter, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 802)}");
            }

            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoLRoleDoL = ImRaii.Table("###CharactersJobs#DoL#RoleDoL", 2))
            {
                if (!charactersJobsDoLRoleDoL) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoL#RoleDoL#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoL#RoleDoL#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Gatherer, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 803)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.CRP);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.MIN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.BSM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.BTN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.ARM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.FSH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.GSM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.LTW);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.WVR);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.ALC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.CUL);
        }

        private void DrawJobLine(Character selectedCharacter, ClassJob job)
        {
            if (selectedCharacter.Jobs is null) return;
            switch (job)
            {
                case ClassJob.GLA:
                case ClassJob.PLD:
                    {
                        if (selectedCharacter.Jobs.Paladin.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Paladin, ClassJob.PLD, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PLD)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GLA)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Gladiator.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Gladiator, ClassJob.GLA, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GLA), active);
                        }

                        break;
                    }
                case ClassJob.MRD:
                case ClassJob.WAR:
                    {
                        if (selectedCharacter.Jobs.Marauder.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Warrior, ClassJob.WAR, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WAR)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Marauder.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Marauder, ClassJob.MRD, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD), active);
                        }

                        break;
                    }
                case ClassJob.DRK:
                    {
                        bool active = (selectedCharacter.Jobs.DarkKnight.Level >= 30);
                        DrawJob(selectedCharacter.Jobs.DarkKnight, ClassJob.DRK, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRK), active);

                        break;
                    }
                case ClassJob.GNB:
                    {
                        bool active = (selectedCharacter.Jobs.Gunbreaker.Level >= 60);
                        DrawJob(selectedCharacter.Jobs.Gunbreaker, ClassJob.GNB, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GNB), active);

                        break;
                    }
                case ClassJob.PGL:
                case ClassJob.MNK:
                    {
                        if (selectedCharacter.Jobs.Pugilist.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Monk, ClassJob.MNK, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MNK)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PGL)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Pugilist.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Pugilist, ClassJob.PGL, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PGL), active);
                        }

                        break;
                    }
                case ClassJob.LNC:
                case ClassJob.DRG:
                    {
                        if (selectedCharacter.Jobs.Lancer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Dragoon, ClassJob.DRG, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRG)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LNC)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Lancer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Lancer, ClassJob.LNC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LNC), active);
                        }

                        break;
                    }
                case ClassJob.ROG:
                case ClassJob.NIN:
                    {
                        if (selectedCharacter.Jobs.Lancer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Ninja, ClassJob.NIN, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.NIN)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ROG)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Lancer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Rogue, ClassJob.ROG, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ROG), active);
                        }

                        break;
                    }
                case ClassJob.SAM:
                    {
                        bool active = (selectedCharacter.Jobs.Samurai.Level >= 50);
                        DrawJob(selectedCharacter.Jobs.Samurai, ClassJob.SAM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SAM), active);

                        break;
                    }
                case ClassJob.RPR:
                    {
                        bool active = (selectedCharacter.Jobs.Reaper.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Reaper, ClassJob.RPR, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RPR), active);

                        break;
                    }
                case ClassJob.VPR:
                    {
                        bool active = (selectedCharacter.Jobs.Viper.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Viper, ClassJob.VPR, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.VPR), active);

                        break;
                    }
                case ClassJob.CNJ:
                case ClassJob.WHM:
                    {
                        if (selectedCharacter.Jobs.Lancer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.WhiteMage, ClassJob.WHM, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WHM)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CNJ)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Conjurer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Conjurer, ClassJob.CNJ, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CNJ), active);
                        }

                        break;
                    }
                case ClassJob.ACN:
                case ClassJob.SCH:
                case ClassJob.SMN:
                    {
                        if (selectedCharacter.Jobs.Arcanist.Level >= 30)
                        {
                            if (job == ClassJob.SCH)
                            {
                                DrawJob(selectedCharacter.Jobs.Scholar, ClassJob.SCH,
                                    $"{_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SCH)}", true);
                            }
                            else if (job == ClassJob.SMN)
                            {
                                DrawJob(selectedCharacter.Jobs.Summoner, ClassJob.SMN,
                                    $"{_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SMN)} / {_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ACN)}",
                                    true);
                            }
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Arcanist.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Arcanist, ClassJob.ACN, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ACN), active);
                        }
                        break;
                    }
                case ClassJob.AST:
                    {
                        bool active = (selectedCharacter.Jobs.Astrologian.Level >= 30) ;
                        DrawJob(selectedCharacter.Jobs.Astrologian, ClassJob.AST, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.AST), active);

                        break;
                    }
                case ClassJob.SGE:
                    {
                        bool active = (selectedCharacter.Jobs.Sage.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Sage, ClassJob.SGE, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SGE), active);

                        break;
                    }
                case ClassJob.ARC:
                case ClassJob.BRD:
                    {
                        if (selectedCharacter.Jobs.Archer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Bard, ClassJob.BRD, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BRD)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARC)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Archer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Archer, ClassJob.ARC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARC), active);
                        }

                        break;
                    }
                case ClassJob.MCH:
                    {
                        bool active = (selectedCharacter.Jobs.Machinist.Level >= 30);
                        DrawJob(selectedCharacter.Jobs.Machinist, ClassJob.MCH, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MCH), active);

                        break;
                    }
                case ClassJob.DNC:
                    {
                        bool active = (selectedCharacter.Jobs.Dancer.Level >= 60);
                        DrawJob(selectedCharacter.Jobs.Dancer, ClassJob.DNC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DNC), active);

                        break;
                    }
                case ClassJob.THM:
                case ClassJob.BLM:
                    {
                        if (selectedCharacter.Jobs.Thaumaturge.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.BlackMage, ClassJob.BLM, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLM)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.THM)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Thaumaturge.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Thaumaturge, ClassJob.THM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.THM), active);
                        }

                        break;
                    }
                case ClassJob.RDM:
                    {
                        bool active = (selectedCharacter.Jobs.RedMage.Level >= 50);
                        DrawJob(selectedCharacter.Jobs.RedMage, ClassJob.RDM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RDM), active);

                        break;
                    }
                case ClassJob.PCT:
                    {
                        bool active = (selectedCharacter.Jobs.Pictomancer.Level >= 50);
                        DrawJob(selectedCharacter.Jobs.Pictomancer, ClassJob.PCT, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PCT), active);

                        break;
                    }
                case ClassJob.BLU:
                    {
                        bool active = (selectedCharacter.Jobs.BlueMage.Level > 0);
                        DrawJob(selectedCharacter.Jobs.BlueMage, ClassJob.BLU, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLU), active);

                        break;
                    }
                case ClassJob.CRP:
                    {
                        bool active = (selectedCharacter.Jobs.Carpenter.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Carpenter, ClassJob.CRP, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CRP), active);

                        break;
                    }
                case ClassJob.BSM:
                    {
                        bool active = (selectedCharacter.Jobs.Blacksmith.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Blacksmith, ClassJob.BSM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BSM), active);

                        break;
                    }
                case ClassJob.ARM:
                    {
                        bool active = (selectedCharacter.Jobs.Armorer.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Armorer, ClassJob.ARM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARM), active);

                        break;
                    }
                case ClassJob.GSM:
                    {
                        bool active = (selectedCharacter.Jobs.Goldsmith.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Goldsmith, ClassJob.GSM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GSM), active);

                        break;
                    }
                case ClassJob.LTW:
                    {
                        bool active = (selectedCharacter.Jobs.Leatherworker.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Leatherworker, ClassJob.LTW, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LTW), active);

                        break;
                    }
                case ClassJob.WVR:
                    {
                        bool active = (selectedCharacter.Jobs.Weaver.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Weaver, ClassJob.WVR, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WVR), active);

                        break;
                    }
                case ClassJob.ALC:
                    {
                        bool active = (selectedCharacter.Jobs.Alchemist.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Alchemist, ClassJob.ALC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ALC), active);

                        break;
                    }
                case ClassJob.CUL:
                    {
                        bool active = (selectedCharacter.Jobs.Culinarian.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Culinarian, ClassJob.CUL, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CUL), active);

                        break;
                    }
                case ClassJob.MIN:
                    {
                        bool active = (selectedCharacter.Jobs.Miner.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Miner, ClassJob.MIN, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MIN), active);

                        break;
                    }
                case ClassJob.BTN:
                    {
                        bool active = (selectedCharacter.Jobs.Botanist.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Botanist, ClassJob.BTN, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BTN), active);

                        break;
                    }
                case ClassJob.FSH:
                    {
                        bool active = (selectedCharacter.Jobs.Fisher.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Fisher, ClassJob.FSH, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.FSH), active);

                        break;
                    }
                case ClassJob.ADV:
                    break;
            }
        }

        private void DrawJob(Job job, ClassJob jobId, string tooltip, bool active)
        {
            //Plugin.Log.Debug($"{job_id} {tooltip} {Utils.GetJobIconWithCornerSmall((uint)job_id)}");
            Vector4 alpha = active switch
            {
                true => new Vector4(1, 1, 1, 1),
                false => new Vector4(1, 1, 1, 0.5f),
            };
            using var charactersJobsJobLine = ImRaii.Table("###CharactersJobs#JobLine", 2);
            if (!charactersJobsJobLine) return;
                ImGui.TableSetupColumn("###CharactersJobs#Icon", ImGuiTableColumnFlags.WidthFixed, 36);
                ImGui.TableSetupColumn("###CharactersJobs#LevelNameExp", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(Utils.GetJobIconWithCornerSmall((uint)jobId)), new Vector2(36, 36), alpha);
                ImGui.TableSetColumnIndex(1);
                using (var charactersJobsJobLevelNameExp = ImRaii.Table("###CharactersJobs#JobLevelNameExp", 2))
                {
                    if (!charactersJobsJobLevelNameExp) return;
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#Level", ImGuiTableColumnFlags.WidthFixed, 20);
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#NameExp", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (active)
                        ImGui.TextUnformatted($"{job.Level}");
                    else
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                        ImGui.TextUnformatted($"{job.Level}");
                        ImGui.PopStyleVar();
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (active)
                        ImGui.TextUnformatted($"{Utils.Capitalize(_globalCache.JobStorage.GetName(_currentLocale,(uint)jobId))}");
                    else
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                        ImGui.TextUnformatted($"{Utils.Capitalize(_globalCache.JobStorage.GetName(_currentLocale,(uint)jobId))}");
                        ImGui.PopStyleVar();
                    }
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(1);
                    bool maxLevel = (jobId == ClassJob.BLU) ? job.Level == 80 : job.Level == 100;
                    Utils.DrawLevelProgressBar(job.Exp, _globalCache.JobStorage.GetNextLevelExp(job.Level), tooltip, active, maxLevel);
                }
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(tooltip);
                ImGui.EndTooltip();
            }
        }
    }
}
