using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using Dalamud.Interface.Utility.Raii;

namespace Altoholic.Windows;

public class JobsWindow : Window, IDisposable
{
    private Plugin plugin;
    private ClientLanguage currentLocale;
    private GlobalCache _globalCache;
    public JobsWindow(
        Plugin plugin,
        string name,
        GlobalCache globalCache
        ) 
        : base(
        name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(1000, 450),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.plugin = plugin;
        this._globalCache = globalCache;

        rolesTextureWrap = Plugin.TextureProvider.GetTextureFromGame("ui/uld/fourth/ToggleButton_hr1.tex");
    }

    public Func<Character> GetPlayer { get; init; } = null!;
    public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
    private Character? current_character = null;
    private IDalamudTextureWrap? rolesTextureWrap = null;

    public void Dispose()
    {

    }

    public override void Draw()
    {
        currentLocale = plugin.Configuration.Language;
        var chars = new List<Character>();
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
                        if (ImGui.Selectable($"{Utils.GetAddonString(970)}###CharactersJobsTable#CharactersListBox#All"))
                        {
                            current_character = null;
                        }

                        foreach (Character currChar in chars)
                        {
                            if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == current_character))
                            {
                                current_character = currChar;
                            }
                        }
                    }

                    ImGui.EndListBox();
                }
                ImGui.TableSetColumnIndex(1);
                if (current_character is not null)
                {
                    DrawJobs(current_character);
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
            ImGui.TextUnformatted(Utils.GetAddonString(1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetAddonString(14055));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.GLA, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.GLA));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.PLD, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.PLD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MRD, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MRD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.WAR, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.WAR));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(5);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.DRK, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.DRK));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(6);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.GNB, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.GNB));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(7);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.CNJ, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.CNJ));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(8);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.WHM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.WHM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(9);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SCH, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SCH));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(10);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.AST, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.AST));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(11);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SGE, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SGE));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(12);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.PGL, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.PGL));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(13);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MNK, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MNK));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(14);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.LNC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.LNC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(15);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.DRG, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.DRG));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(16);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ROG, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ROG));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(17);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.NIN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.NIN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(18);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SAM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SAM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(19);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.RPR, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.RPR));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(20);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ARC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ARC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(21);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BRD, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BRD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(22);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MCH, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MCH));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(23);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.DNC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.DNC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(24);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.THM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.THM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(25);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BLM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BLM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(26);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ACN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ACN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(27);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SMN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.SMN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(28);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.RDM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.RDM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(29);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BLU));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BLU));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(30);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.CRP, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.CRP));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(31);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BSM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BSM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(32);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ARM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ARM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(33);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.GSM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MRD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(34);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.LTW, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.LTW));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(35);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.WVR, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.WVR));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(36);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ALC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.ALC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(37);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.CUL, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.CUL));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(38);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MIN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.MIN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(39);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BTN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.BTN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(40);
            ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.FSH, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId((uint)ClassJob.FSH));
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

    private void DrawJobs(Character selected_character)
    {
        using var tabBar = ImRaii.TabBar($"###CharactersJobs#JobsTabs#{selected_character.Id}");
        if (!tabBar.Success) return;
        using (var DoWDoMTab = ImRaii.TabItem($"{Utils.GetAddonString(1080)}###CharactersJobs#JobsTabs#DoWDoM#{selected_character.Id}"))
        {
            if (DoWDoMTab)
            {
                DrawDoWDoMJobs(selected_character);
            }
        };
        using (var DoHDoLTab = ImRaii.TabItem($"{Utils.GetAddonString(1081)}###CharactersJobs#JobsTabs#DoHDoL#{selected_character.Id}"))
        {
            if (DoHDoLTab)
            {
                DrawDoHDoLJobs(selected_character);
            }
        };
        /*if (ImGui.BeginTabBar($"###CharactersJobs#JobsTable#{selected_character.Id}"))
        {
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(1080)}###{selected_character.Id}"))// Dow/DoM
            {
                DrawDoWDoMJobs(selected_character);
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(1081)}###{selected_character.Id}"))// DoHDoL
            {
                DrawDoHDoLJobs(selected_character);
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }*/
    }

    private void DrawDoWDoMJobs(Character selected_character)
    {
        if (rolesTextureWrap is null) return;
        if (selected_character is null) return;
        if (ImGui.BeginTable($"###CharactersJobs#DoWDoMJobs#{selected_character.Id}", 2, ImGuiTableFlags.ScrollY))
        {
            ImGui.TableSetupColumn($"###CharactersJobs#DoW#{selected_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoM#{selected_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###CharactersJobs#DoW#RoleTank", 2))
            {
                ImGui.TableSetupColumn($"###CharactersJobs#DoW#RoleTank#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn($"###CharactersJobs#DoW#RoleTank#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(0));
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Tank, new Vector2(20,20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(1082)}"); // Tank

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
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Heal, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(1083)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.GLA);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.CNJ);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.MRD);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.SCH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.DRK);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.AST);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.GNB);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.SGE);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###CharactersJobs#DoW#RoleMelee", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(2));
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Melee, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(1084)}");

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
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Ranged, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(1085)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.PGL);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.ARC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.LNC);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.MCH);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.ROG);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.DNC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.SAM);

            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("###CharactersJobs#DoM#RoleMagicalRanged", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(4));
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Caster, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(1086)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.RPR);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.THM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.SMN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.RDM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.BLU);

            ImGui.EndTable();
        }
    }

    private void DrawDoHDoLJobs(Character selected_character)
    {
        if (rolesTextureWrap is null) return;
        if (selected_character is null) return;
        if (ImGui.BeginTable($"###CharactersJobs#DoHDoLJobs#{selected_character.Id}", 2))
        {
            ImGui.TableSetupColumn($"###CharactersJobs#DoH#{selected_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoL#{selected_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###CharactersJobs#DoH#RoleDoH", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //Utils.DrawIcon(new Vector2(20, 20), false, Utils.GetRoleIcon(5));
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Crafter, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(802)}");

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
                Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Gatherer, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(803)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.CRP);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.MIN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.BSM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.BTN);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.ARM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selected_character, ClassJob.FSH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.GSM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.LTW);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.WVR);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.ALC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selected_character, ClassJob.CUL);


            ImGui.EndTable();
        }
    }
 
    private void DrawJobLine(Character selected_character, ClassJob job)
    {
        if (selected_character.Jobs is null) return;
        switch (job)
        {
            case ClassJob.GLA:
            case ClassJob.PLD:
            {
                if (selected_character.Jobs.Paladin.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.Paladin, ClassJob.PLD, $"{Utils.GetJobNameFromId((uint)ClassJob.PLD)} / {Utils.GetJobNameFromId((uint)ClassJob.GLA)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Gladiator.Level > 0);
                    DrawJob(selected_character.Jobs.Gladiator, ClassJob.GLA, Utils.GetJobNameFromId((uint)ClassJob.GLA), active);
                }

                break;
            }
            case ClassJob.MRD:
            case ClassJob.WAR:
            {
                if (selected_character.Jobs.Marauder.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.Warrior, ClassJob.WAR, $"{Utils.GetJobNameFromId((uint)ClassJob.WAR)} / {Utils.GetJobNameFromId((uint)ClassJob.MRD)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Marauder.Level > 0);
                    DrawJob(selected_character.Jobs.Marauder, ClassJob.MRD, Utils.GetJobNameFromId((uint)ClassJob.MRD), active);
                }

                break;
            }
            case ClassJob.DRK:
            {
                bool active = (selected_character.Jobs.DarkKnight.Level >= 30);
                DrawJob(selected_character.Jobs.DarkKnight, ClassJob.DRK, Utils.GetJobNameFromId((uint)ClassJob.DRK), active);

                break;
            }
            case ClassJob.GNB:
            {
                bool active = (selected_character.Jobs.Gunbreaker.Level >= 60);
                DrawJob(selected_character.Jobs.Gunbreaker, ClassJob.GNB, Utils.GetJobNameFromId((uint)ClassJob.GNB), active);

                break;
            }
            case ClassJob.PGL:
            case ClassJob.MNK:
            {
                if (selected_character.Jobs.Pugilist.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.Monk, ClassJob.MNK, $"{Utils.GetJobNameFromId((uint)ClassJob.MNK)} / {Utils.GetJobNameFromId((uint)ClassJob.PGL)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Pugilist.Level > 0);
                    DrawJob(selected_character.Jobs.Pugilist, ClassJob.PGL, Utils.GetJobNameFromId((uint)ClassJob.PGL), active);
                }

                break;
            }
            case ClassJob.LNC:
            case ClassJob.DRG:
            {
                if (selected_character.Jobs.Lancer.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.Dragoon, ClassJob.DRG, $"{Utils.GetJobNameFromId((uint)ClassJob.DRG)} / {Utils.GetJobNameFromId((uint)ClassJob.LNC)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Lancer.Level > 0);
                    DrawJob(selected_character.Jobs.Lancer, ClassJob.LNC, Utils.GetJobNameFromId((uint)ClassJob.LNC), active);
                }

                break;
            }
            case ClassJob.ROG:
            case ClassJob.NIN:
            {
                if (selected_character.Jobs.Lancer.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.Ninja, ClassJob.NIN, $"{Utils.GetJobNameFromId((uint)ClassJob.NIN)} / {Utils.GetJobNameFromId((uint)ClassJob.ROG)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Lancer.Level > 0);
                    DrawJob(selected_character.Jobs.Rogue, ClassJob.ROG, Utils.GetJobNameFromId((uint)ClassJob.ROG), active);
                }

                break;
            }
            case ClassJob.SAM:
            {
                bool active = (selected_character.Jobs.Samurai.Level >= 50);
                DrawJob(selected_character.Jobs.Samurai, ClassJob.SAM, Utils.GetJobNameFromId((uint)ClassJob.SAM), active);

                break;
            }
            case ClassJob.RPR:
            {
                bool active = (selected_character.Jobs.Reaper.Level >= 70);
                DrawJob(selected_character.Jobs.Reaper, ClassJob.RPR, Utils.GetJobNameFromId((uint)ClassJob.RPR), active);

                break;
            }
            case ClassJob.CNJ:
            case ClassJob.WHM:
            {
                if (selected_character.Jobs.Lancer.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.WhiteMage, ClassJob.WHM, $"{Utils.GetJobNameFromId((uint)ClassJob.WHM)} / {Utils.GetJobNameFromId((uint)ClassJob.CNJ)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Conjurer.Level > 0);
                    DrawJob(selected_character.Jobs.Conjurer, ClassJob.CNJ, Utils.GetJobNameFromId((uint)ClassJob.CNJ), active);
                }

                break;
            }
            case ClassJob.ACN:
            case ClassJob.SCH:
            case ClassJob.SMN:
            {
                if (selected_character.Jobs.Arcanist.Level >= 30)
                {
                    switch (job)
                    {
                        case ClassJob.SCH:
                        {
                            DrawJob(selected_character.Jobs.Scholar, ClassJob.SCH, $"{Utils.GetJobNameFromId((uint)ClassJob.SCH)}", true);
                            break;
                        }
                        case ClassJob.SMN:
                        {
                            DrawJob(selected_character.Jobs.Summoner, ClassJob.SMN, $"{Utils.GetJobNameFromId((uint)ClassJob.SMN)} / {Utils.GetJobNameFromId((uint)ClassJob.ACN)}", true);
                            break;
                        }
                    }
                }
                else
                {
                    bool active = (selected_character.Jobs.Arcanist.Level > 0);
                    DrawJob(selected_character.Jobs.Arcanist, ClassJob.ACN, Utils.GetJobNameFromId((uint)ClassJob.ACN), active);
                }
                break;
            }
            case ClassJob.AST:
            {
                bool active = (selected_character.Jobs.Astrologian.Level >= 30) ;
                DrawJob(selected_character.Jobs.Astrologian, ClassJob.AST, Utils.GetJobNameFromId((uint)ClassJob.AST), active);

                break;
            }
            case ClassJob.SGE:
            {
                bool active = (selected_character.Jobs.Sage.Level >= 70);
                DrawJob(selected_character.Jobs.Sage, ClassJob.SGE, Utils.GetJobNameFromId((uint)ClassJob.SGE), active);

                break;
            }
            case ClassJob.ARC:
            case ClassJob.BRD:
            {
                if (selected_character.Jobs.Archer.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.Bard, ClassJob.BRD, $"{Utils.GetJobNameFromId((uint)ClassJob.BRD)} / {Utils.GetJobNameFromId((uint)ClassJob.ARC)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Archer.Level > 0);
                    DrawJob(selected_character.Jobs.Archer, ClassJob.ARC, Utils.GetJobNameFromId((uint)ClassJob.ARC), active);
                }

                break;
            }
            case ClassJob.MCH:
            {
                bool active = (selected_character.Jobs.Machinist.Level >= 30);
                DrawJob(selected_character.Jobs.Machinist, ClassJob.MCH, Utils.GetJobNameFromId((uint)ClassJob.MCH), active);

                break;
            }
            case ClassJob.DNC:
            {
                bool active = (selected_character.Jobs.Dancer.Level >= 60);
                DrawJob(selected_character.Jobs.Dancer, ClassJob.DNC, Utils.GetJobNameFromId((uint)ClassJob.DNC), active);

                break;
            }
            case ClassJob.THM:
            case ClassJob.BLM:
            {
                if (selected_character.Jobs.Thaumaturge.Level >= 30)
                {
                    DrawJob(selected_character.Jobs.BlackMage, ClassJob.BLM, $"{Utils.GetJobNameFromId((uint)ClassJob.BLM)} / {Utils.GetJobNameFromId((uint)ClassJob.THM)}", true);
                }
                else
                {
                    bool active = (selected_character.Jobs.Thaumaturge.Level > 0);
                    DrawJob(selected_character.Jobs.Thaumaturge, ClassJob.THM, Utils.GetJobNameFromId((uint)ClassJob.THM), active);
                }

                break;
            }
            case ClassJob.RDM:
            {
                bool active = (selected_character.Jobs.RedMage.Level >= 50);
                DrawJob(selected_character.Jobs.RedMage, ClassJob.RDM, Utils.GetJobNameFromId((uint)ClassJob.RDM), active);

                break;
            }
            case ClassJob.BLU:
            {
                bool active = (selected_character.Jobs.BlueMage.Level > 0);
                DrawJob(selected_character.Jobs.BlueMage, ClassJob.BLU, Utils.GetJobNameFromId((uint)ClassJob.BLU), active);

                break;
            }
            case ClassJob.CRP:
            {
                bool active = (selected_character.Jobs.Carpenter.Level > 0);
                DrawJob(selected_character.Jobs.Carpenter, ClassJob.CRP, Utils.GetJobNameFromId((uint)ClassJob.CRP), active);

                break;
            }
            case ClassJob.BSM:
            {
                bool active = (selected_character.Jobs.Blacksmith.Level > 0);
                DrawJob(selected_character.Jobs.Blacksmith, ClassJob.BSM, Utils.GetJobNameFromId((uint)ClassJob.BSM), active);

                break;
            }
            case ClassJob.ARM:
            {
                bool active = (selected_character.Jobs.Armorer.Level > 0);
                DrawJob(selected_character.Jobs.Armorer, ClassJob.ARM, Utils.GetJobNameFromId((uint)ClassJob.ARM), active);

                break;
            }
            case ClassJob.GSM:
            {
                bool active = (selected_character.Jobs.Goldsmith.Level > 0);
                DrawJob(selected_character.Jobs.Goldsmith, ClassJob.GSM, Utils.GetJobNameFromId((uint)ClassJob.GSM), active);

                break;
            }
            case ClassJob.LTW:
            {
                bool active = (selected_character.Jobs.Leatherworker.Level > 0);
                DrawJob(selected_character.Jobs.Leatherworker, ClassJob.LTW, Utils.GetJobNameFromId((uint)ClassJob.LTW), active);

                break;
            }
            case ClassJob.WVR:
            {
                bool active = (selected_character.Jobs.Weaver.Level > 0);
                DrawJob(selected_character.Jobs.Weaver, ClassJob.WVR, Utils.GetJobNameFromId((uint)ClassJob.WVR), active);

                break;
            }
            case ClassJob.ALC:
            {
                bool active = (selected_character.Jobs.Alchemist.Level > 0);
                DrawJob(selected_character.Jobs.Alchemist, ClassJob.ALC, Utils.GetJobNameFromId((uint)ClassJob.ALC), active);

                break;
            }
            case ClassJob.CUL:
            {
                bool active = (selected_character.Jobs.Culinarian.Level > 0);
                DrawJob(selected_character.Jobs.Culinarian, ClassJob.CUL, Utils.GetJobNameFromId((uint)ClassJob.CUL), active);

                break;
            }
            case ClassJob.MIN:
            {
                bool active = (selected_character.Jobs.Miner.Level > 0);
                DrawJob(selected_character.Jobs.Miner, ClassJob.MIN, Utils.GetJobNameFromId((uint)ClassJob.MIN), active);

                break;
            }
            case ClassJob.BTN:
            {
                bool active = (selected_character.Jobs.Botanist.Level > 0);
                DrawJob(selected_character.Jobs.Botanist, ClassJob.BTN, Utils.GetJobNameFromId((uint)ClassJob.BTN), active);

                break;
            }
            case ClassJob.FSH:
            {
                bool active = (selected_character.Jobs.Fisher.Level > 0);
                DrawJob(selected_character.Jobs.Fisher, ClassJob.FSH, Utils.GetJobNameFromId((uint)ClassJob.FSH), active);

                break;
            }            
            default:
            {
                break; 
            }
        }
    }

    private void DrawJob(Job job, ClassJob job_id, string tooltip, bool active)
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
            // Todo: Fix alpha
            //Utils.DrawIcon(new Vector2(36, 36), false, Utils.GetJobIconWithCornerSmall((uint)job_id));
            Utils.DrawIcon_test(_globalCache.IconStorage.LoadIcon(Utils.GetJobIconWithCornerSmall((uint)job_id)), new Vector2(36, 36), alpha);
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
                //ImGui.TextUnformatted($"{Utils.GetJobNameFromId((uint)job_id)}");
                if (active)
                    ImGui.TextUnformatted($"{Utils.Capitalize(_globalCache.JobStorage.GetName((uint)job_id))}");
                else
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                    ImGui.TextUnformatted($"{Utils.Capitalize(_globalCache.JobStorage.GetName((uint)job_id))}");
                    ImGui.PopStyleVar();
                }
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                //Utils.DrawLevelProgressBar(job.Exp, Utils.GetJobNextLevelExp(job.Level), tooltip);
                bool maxLevel = (job_id == ClassJob.BLU) ? job.Level == 80 : job.Level == 90;
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
