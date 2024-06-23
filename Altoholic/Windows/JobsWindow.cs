using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using System.Linq;

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

            _rolesTextureWrap = Plugin.TextureProvider.GetTextureFromGame("ui/uld/fourth/ToggleButton_hr1.tex");
        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;
        private IDalamudTextureWrap? _rolesTextureWrap;

        public void Dispose()
        {

        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                if (ImGui.BeginTable("###CharactersJobsTable", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobsTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed, 210);
                    ImGui.TableSetupColumn("###CharactersJobsTable#Jobs", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (ImGui.BeginListBox("###CharactersJobsTable#CharactersListBox", new Vector2(200, -1)))
                    {
                        ImGui.SetScrollY(0);
                        if (chars.Count > 0)
                        {
                            if (ImGui.Selectable($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersJobsTable#CharactersListBox#All", _currentCharacter == null))
                            {
                                _currentCharacter = null;
                            }

                            foreach (var currChar in chars.Where(currChar => ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == _currentCharacter)))
                            {
                                _currentCharacter = currChar;
                            }
                        }

                        ImGui.EndListBox();
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

                    ImGui.EndTable();
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic : Exception : {0}", e);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            if (ImGui.BeginTable("###CharactersJobs#All", 41, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
            {
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
                ImGui.TableSetupColumn("###CharactersJobs#All#ARC", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#BRD", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#MCH", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#DNC", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#THM", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#BLM", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#ACN", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#SMN", ImGuiTableColumnFlags.WidthFixed, 25);
                ImGui.TableSetupColumn("###CharactersJobs#All#RDM", ImGuiTableColumnFlags.WidthFixed, 25);
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
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GLA, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GLA));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PLD, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PLD));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(3);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(4);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WAR, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WAR));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(5);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRK, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRK));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(6);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GNB, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GNB));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(7);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CNJ, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CNJ));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(8);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WHM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WHM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(9);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SCH, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SCH));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(10);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.AST, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.AST));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(11);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SGE, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SGE));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(12);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PGL, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PGL));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(13);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MNK, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MNK));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(14);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LNC, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LNC));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(15);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRG, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRG));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(16);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ROG, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ROG));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(17);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.NIN, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.NIN));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(18);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SAM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SAM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(19);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RPR, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RPR));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(20);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARC, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARC));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(21);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BRD, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BRD));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(22);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MCH, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MCH));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(23);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DNC, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DNC));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(24);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.THM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.THM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(25);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(26);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ACN, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ACN));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(27);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SMN, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SMN));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(28);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RDM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RDM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(29);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLU));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLU));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(30);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CRP, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CRP));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(31);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BSM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BSM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(32);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARM));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(33);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GSM, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(34);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LTW, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LTW));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(35);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WVR, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WVR));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(36);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ALC, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ALC));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(37);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CUL, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CUL));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(38);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MIN, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MIN));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(39);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BTN, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BTN));
                    ImGui.EndTooltip();
                }
                ImGui.TableSetColumnIndex(40);
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.FSH, true));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.FSH));
                    ImGui.EndTooltip();
                }
                foreach (Character currChar in chars)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                        ImGui.EndTooltip();
                    }
                    if (currChar.Jobs is null) continue;
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{currChar.Jobs.Gladiator.Level}");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted($"{currChar.Jobs.Paladin.Level}");
                    ImGui.TableSetColumnIndex(3);
                    ImGui.TextUnformatted($"{currChar.Jobs.Marauder.Level}");
                    ImGui.TableSetColumnIndex(4);
                    ImGui.TextUnformatted($"{currChar.Jobs.Warrior.Level}");
                    ImGui.TableSetColumnIndex(5);
                    ImGui.TextUnformatted($"{currChar.Jobs.DarkKnight.Level}");
                    ImGui.TableSetColumnIndex(6);
                    ImGui.TextUnformatted($"{currChar.Jobs.Gunbreaker.Level}");
                    ImGui.TableSetColumnIndex(7);
                    ImGui.TextUnformatted($"{currChar.Jobs.Conjurer.Level}");
                    ImGui.TableSetColumnIndex(8);
                    ImGui.TextUnformatted($"{currChar.Jobs.WhiteMage.Level}");
                    ImGui.TableSetColumnIndex(9);
                    ImGui.TextUnformatted($"{currChar.Jobs.Scholar.Level}");
                    ImGui.TableSetColumnIndex(10);
                    ImGui.TextUnformatted($"{currChar.Jobs.Astrologian.Level}");
                    ImGui.TableSetColumnIndex(11);
                    ImGui.TextUnformatted($"{currChar.Jobs.Sage.Level}");
                    ImGui.TableSetColumnIndex(12);
                    ImGui.TextUnformatted($"{currChar.Jobs.Pugilist.Level}");
                    ImGui.TableSetColumnIndex(13);
                    ImGui.TextUnformatted($"{currChar.Jobs.Monk.Level}");
                    ImGui.TableSetColumnIndex(14);
                    ImGui.TextUnformatted($"{currChar.Jobs.Lancer.Level}");
                    ImGui.TableSetColumnIndex(15);
                    ImGui.TextUnformatted($"{currChar.Jobs.Dragoon.Level}");
                    ImGui.TableSetColumnIndex(16);
                    ImGui.TextUnformatted($"{currChar.Jobs.Rogue.Level}");
                    ImGui.TableSetColumnIndex(17);
                    ImGui.TextUnformatted($"{currChar.Jobs.Ninja.Level}");
                    ImGui.TableSetColumnIndex(18);
                    ImGui.TextUnformatted($"{currChar.Jobs.Samurai.Level}");
                    ImGui.TableSetColumnIndex(19);
                    ImGui.TextUnformatted($"{currChar.Jobs.Reaper.Level}");
                    ImGui.TableSetColumnIndex(20);
                    ImGui.TextUnformatted($"{currChar.Jobs.Archer.Level}");
                    ImGui.TableSetColumnIndex(21);
                    ImGui.TextUnformatted($"{currChar.Jobs.Bard.Level}");
                    ImGui.TableSetColumnIndex(22);
                    ImGui.TextUnformatted($"{currChar.Jobs.Machinist.Level}");
                    ImGui.TableSetColumnIndex(23);
                    ImGui.TextUnformatted($"{currChar.Jobs.Dancer.Level}");
                    ImGui.TableSetColumnIndex(24);
                    ImGui.TextUnformatted($"{currChar.Jobs.Thaumaturge.Level}");
                    ImGui.TableSetColumnIndex(25);
                    ImGui.TextUnformatted($"{currChar.Jobs.BlackMage.Level}");
                    ImGui.TableSetColumnIndex(26);
                    ImGui.TextUnformatted($"{currChar.Jobs.Arcanist.Level}");
                    ImGui.TableSetColumnIndex(27);
                    ImGui.TextUnformatted($"{currChar.Jobs.Summoner.Level}");
                    ImGui.TableSetColumnIndex(28);
                    ImGui.TextUnformatted($"{currChar.Jobs.RedMage.Level}");
                    ImGui.TableSetColumnIndex(29);
                    ImGui.TextUnformatted($"{currChar.Jobs.BlackMage.Level}");
                    ImGui.TableSetColumnIndex(30);
                    ImGui.TextUnformatted($"{currChar.Jobs.Carpenter.Level}");
                    ImGui.TableSetColumnIndex(31);
                    ImGui.TextUnformatted($"{currChar.Jobs.Blacksmith.Level}");
                    ImGui.TableSetColumnIndex(32);
                    ImGui.TextUnformatted($"{currChar.Jobs.Armorer.Level}");
                    ImGui.TableSetColumnIndex(33);
                    ImGui.TextUnformatted($"{currChar.Jobs.Goldsmith.Level}");
                    ImGui.TableSetColumnIndex(34);
                    ImGui.TextUnformatted($"{currChar.Jobs.Leatherworker.Level}");
                    ImGui.TableSetColumnIndex(35);
                    ImGui.TextUnformatted($"{currChar.Jobs.Weaver.Level}");
                    ImGui.TableSetColumnIndex(36);
                    ImGui.TextUnformatted($"{currChar.Jobs.Alchemist.Level}");
                    ImGui.TableSetColumnIndex(37);
                    ImGui.TextUnformatted($"{currChar.Jobs.Culinarian.Level}");
                    ImGui.TableSetColumnIndex(38);
                    ImGui.TextUnformatted($"{currChar.Jobs.Miner.Level}");
                    ImGui.TableSetColumnIndex(39);
                    ImGui.TextUnformatted($"{currChar.Jobs.Botanist.Level}");
                    ImGui.TableSetColumnIndex(40);
                    ImGui.TextUnformatted($"{currChar.Jobs.Fisher.Level}");
                }
                ImGui.EndTable();
            }
        }

        private void DrawJobs(Character selectedCharacter)
        {
            using var tabBar = ImRaii.TabBar($"###CharactersJobs#JobsTabs#{selectedCharacter.Id}");
            if (!tabBar.Success) return;
            using (var DoWDoMTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1080)}###CharactersJobs#JobsTabs#DoWDoM#{selectedCharacter.Id}"))
            {
                if (DoWDoMTab)
                {
                    DrawDoWDoMJobs(selectedCharacter);
                }
            };
            using (var DoHDoLTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1081)}###CharactersJobs#JobsTabs#DoHDoL#{selectedCharacter.Id}"))
            {
                if (DoHDoLTab)
                {
                    DrawDoHDoLJobs(selectedCharacter);
                }
            };
            /*if (ImGui.BeginTabBar($"###CharactersJobs#JobsTable#{selected_character.Id}"))
            {
                if (ImGui.BeginTabItem($"{_globalCache.AddonStorage.LoadAddonString(1080)}###{selected_character.Id}"))// Dow/DoM
                {
                    DrawDoWDoMJobs(selected_character);
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem($"{_globalCache.AddonStorage.LoadAddonString(1081)}###{selected_character.Id}"))// DoHDoL
                {
                    DrawDoHDoLJobs(selected_character);
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }*/
        }

        private void DrawDoWDoMJobs(Character selectedCharacter)
        {
            if (_rolesTextureWrap is null) return;
            if (ImGui.BeginTable($"###CharactersJobs#DoWDoMJobs#{selectedCharacter.Id}", 2, ImGuiTableFlags.ScrollY))
            {
                ImGui.TableSetupColumn($"###CharactersJobs#DoW#{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###CharactersJobs#DoM#{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginTable("###CharactersJobs#DoW#RoleTank", 2))
                {
                    ImGui.TableSetupColumn($"###CharactersJobs#DoW#RoleTank#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn($"###CharactersJobs#DoW#RoleTank#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(0));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Tank, new Vector2(20,20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1082)}"); // Tank

                    ImGui.EndTable();
                }
                ImGui.Separator();

                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable("###CharactersJobs#DoM#RoleHealer", 2))
                {
                    ImGui.TableSetupColumn($"###CharactersJobs#DoM#RoleHealer#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn($"###CharactersJobs#DoM#RoleHealer#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(1));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Heal, new Vector2(20, 20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1083)}");

                    ImGui.EndTable();
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
                if (ImGui.BeginTable("###CharactersJobs#DoW#RoleMelee", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(2));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Melee, new Vector2(20, 20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1084)}");

                    ImGui.EndTable();
                }
                ImGui.Separator();

                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable("###CharactersJobs#DoW#RolePhysicalRanged", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobs#DoW#RolePhysicalRanged#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn("###CharactersJobs#DoW#RolePhysicalRanged#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(3));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Ranged, new Vector2(20, 20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1085)}");

                    ImGui.EndTable();
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
                if (ImGui.BeginTable("###CharactersJobs#DoM#RoleMagicalRanged", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(4));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Caster, new Vector2(20, 20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1086)}");

                    ImGui.EndTable();
                }
                ImGui.Separator();

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawJobLine(selectedCharacter, ClassJob.RPR);
                ImGui.TableSetColumnIndex(1);
                DrawJobLine(selectedCharacter, ClassJob.THM);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                DrawJobLine(selectedCharacter, ClassJob.SMN);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                DrawJobLine(selectedCharacter, ClassJob.RDM);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                DrawJobLine(selectedCharacter, ClassJob.BLU);

                ImGui.EndTable();
            }
        }

        private void DrawDoHDoLJobs(Character selectedCharacter)
        {
            if (_rolesTextureWrap is null) return;
            if (ImGui.BeginTable($"###CharactersJobs#DoHDoLJobs#{selectedCharacter.Id}", 2))
            {
                ImGui.TableSetupColumn($"###CharactersJobs#DoH#{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###CharactersJobs#DoL#{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginTable("###CharactersJobs#DoH#RoleDoH", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(5));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Crafter, new Vector2(20, 20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 802)}");

                    ImGui.EndTable();
                }
                ImGui.Separator();

                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable("###CharactersJobs#DoL#RoleDoL", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobs#DoL#RoleDoL#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                    ImGui.TableSetupColumn("###CharactersJobs#DoL#RoleDoL#Name", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(6));
                    Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Gatherer, new Vector2(20, 20));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 803)}");

                    ImGui.EndTable();
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


                ImGui.EndTable();
            }
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
            if (ImGui.BeginTable("###CharactersJobs#JobLine", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#Icon", ImGuiTableColumnFlags.WidthFixed, 36);
                ImGui.TableSetupColumn("###CharactersJobs#LevelNameExp", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //Utils.DrawIcon( Plugin.DataManager, new Vector2(42, 42), false, Utils.GetJobIconWithCornerSmall((uint)job_id), alpha);
                //Utils.DrawIcon(new Vector2(36, 36), false, Utils.GetJobIconWithCornerSmall((uint)job_id));
                Utils.DrawIcon_test(_globalCache.IconStorage.LoadIcon(Utils.GetJobIconWithCornerSmall((uint)jobId)), new Vector2(36, 36), alpha);
                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable("###CharactersJobs#JobLevelNameExp", 2))
                {
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#Level", ImGuiTableColumnFlags.WidthFixed, 10);
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
                    //ImGui.TextUnformatted($"{_globalCache.JobStorage.GetName(currentLocale,(uint)job_id)}");
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
                    //Utils.DrawLevelProgressBar(job.Exp, Utils.GetJobNextLevelExp(job.Level), tooltip);
                    bool maxLevel = (jobId == ClassJob.BLU) ? job.Level == 80 : job.Level == 90;
                    Utils.DrawLevelProgressBar(job.Exp, _globalCache.JobStorage.GetNextLevelExp(job.Level), tooltip, active, maxLevel);
                    ImGui.EndTable();
                }
                ImGui.EndTable();
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
