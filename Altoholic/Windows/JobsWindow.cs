using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Altoholic.Windows;

public class JobsWindow : Window, IDisposable
{
    private Plugin plugin;

    private readonly IPluginLog pluginLog;
    private readonly IDataManager dataManager;
    private readonly ITextureProvider textureProvider;
    private readonly ClientLanguage currentLocale;

    public JobsWindow(
        Plugin plugin,
        string name,
        IPluginLog pluginLog,
        IDataManager dataManager,
        ITextureProvider textureProvider,
        ClientLanguage currentLocale
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
        this.pluginLog = pluginLog;
        this.dataManager = dataManager;
        this.textureProvider = textureProvider;
        this.currentLocale = currentLocale;
    }

    public Func<Character> GetPlayer { get; init; } = null!;
    public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
    private Character? current_character = null;

    public void Dispose()
    {

    }

    public override void Draw()
    {
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
                        if (ImGui.Selectable("All"))
                        {
                            current_character = null;
                        }

                        foreach (Character currChar in chars)
                        {
                            if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}"))
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
            pluginLog.Debug("Altoholic : Exception : {0}", e);
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
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 14055));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GLA, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GLA));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.PLD, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.PLD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MRD, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MRD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WAR, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WAR));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(5);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DRK, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DRK));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(6);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GNB, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GNB));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(7);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CNJ, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CNJ));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(8);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WHM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WHM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(9);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SCH, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SCH));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(10);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.AST, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.AST));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(11);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SGE, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SGE));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(12);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.PGL, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.PGL));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(13);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MNK, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MNK));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(14);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.LNC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.LNC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(15);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DRG, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DRG));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(16);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ROG, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ROG));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(17);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.NIN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.NIN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(18);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SAM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SAM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(19);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.RPR, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.RPR));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(20);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ARC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ARC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(21);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BRD, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BRD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(22);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MCH, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MCH));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(23);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DNC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DNC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(24);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.THM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.THM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(25);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BLM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BLM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(26);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ACN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ACN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(27);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SMN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SMN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(28);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.RDM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.RDM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(29);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BLU));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BLU));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(30);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CRP, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CRP));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(31);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BSM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BSM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(32);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ARM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ARM));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(33);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GSM, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MRD));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(34);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.LTW, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.LTW));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(35);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WVR, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WVR));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(36);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ALC, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ALC));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(37);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CUL, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CUL));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(38);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MIN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MIN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(39);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BTN, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BTN));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(40);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.FSH, true));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.FSH));
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

    private void DrawJobs(Character current_character)
    {
        if (ImGui.BeginTabBar($"####CharactersJobs#JobsTable#{current_character.Id}"))
        {
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1080)}###{current_character.Id}"))// Dow/DoM
            {
                DrawDoWDoMJobs(current_character);
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1081)}###{current_character.Id}"))// DoHDoL
            {
                DrawDoHDoLJobs(current_character);
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawDoWDoMJobs(Character current_character)
    {
        if (current_character is null) return;
        if (ImGui.BeginTable($"###CharactersJobs#DoWDoMJobs#{current_character.Id}", 2, ImGuiTableFlags.ScrollY))
        {
            ImGui.TableSetupColumn($"###CharactersJobs#DoW#{current_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoM#{current_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###CharactersJobs#DoW#RoleTank", 2))
            {
                ImGui.TableSetupColumn($"###CharactersJobs#DoW#RoleTank#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn($"###CharactersJobs#DoW#RoleTank#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(0));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1082)}"); // Tank

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
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(1));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1083)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.GLA);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.CNJ);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.MRD);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.SCH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.DRK);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.AST);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.GNB);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.SGE);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###CharactersJobs#DoW#RoleMelee", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(2));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1084)}");

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
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(3));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1085)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.PGL);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.ARC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.LNC);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.MCH);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.ROG);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.DNC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.SAM);

            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("###CharactersJobs#DoM#RoleMagicalRanged", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(4));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1086)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.RPR);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.THM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.SMN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.RDM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.BLU);

            ImGui.EndTable();
        }
    }

    private void DrawDoHDoLJobs(Character current_character)
    {
        if (current_character is null) return;
        if (ImGui.BeginTable($"###CharactersJobs#DoHDoLJobs#{current_character.Id}", 2))
        {
            ImGui.TableSetupColumn($"###CharactersJobs#DoH#{current_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoL#{current_character.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###CharactersJobs#DoH#RoleDoH", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(5));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 802)}");

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
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(6));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 803)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.CRP);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.MIN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.BSM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.BTN);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.ARM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(current_character, ClassJob.FSH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.GSM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.LTW);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.WVR);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.ALC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(current_character, ClassJob.CUL);


            ImGui.EndTable();
        }
    }
 
    private void DrawJobLine(Character current_character, ClassJob job)
    {
        if (current_character.Jobs is null) return;
        switch (job)
        {
            case ClassJob.GLA:
            case ClassJob.PLD:
            {
                if (current_character.Jobs.Paladin.Level >= 30)
                {
                    DrawJob(current_character.Jobs.Paladin, ClassJob.PLD, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.PLD)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.GLA)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Gladiator.Level > 0);
                    DrawJob(current_character.Jobs.Gladiator, ClassJob.GLA, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.GLA), active);
                }

                break;
            }
            case ClassJob.MRD:
            case ClassJob.WAR:
            {
                if (current_character.Jobs.Marauder.Level >= 30)
                {
                    DrawJob(current_character.Jobs.Warrior, ClassJob.WAR, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.WAR)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MRD)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Marauder.Level > 0);
                    DrawJob(current_character.Jobs.Marauder, ClassJob.MRD, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MRD), active);
                }

                break;
            }
            case ClassJob.DRK:
            {
                bool active = (current_character.Jobs.DarkKnight.Level >= 30);
                DrawJob(current_character.Jobs.DarkKnight, ClassJob.DRK, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.DRK), active);

                break;
            }
            case ClassJob.GNB:
            {
                bool active = (current_character.Jobs.Gunbreaker.Level >= 60);
                DrawJob(current_character.Jobs.Gunbreaker, ClassJob.GNB, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.GNB), active);

                break;
            }
            case ClassJob.PGL:
            case ClassJob.MNK:
            {
                if (current_character.Jobs.Pugilist.Level >= 30)
                {
                    DrawJob(current_character.Jobs.Monk, ClassJob.MNK, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MNK)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.PGL)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Pugilist.Level > 0);
                    DrawJob(current_character.Jobs.Pugilist, ClassJob.PGL, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.PGL), active);
                }

                break;
            }
            case ClassJob.LNC:
            case ClassJob.DRG:
            {
                if (current_character.Jobs.Lancer.Level >= 30)
                {
                    DrawJob(current_character.Jobs.Dragoon, ClassJob.DRG, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.DRG)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.LNC)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Lancer.Level > 0);
                    DrawJob(current_character.Jobs.Lancer, ClassJob.LNC, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.LNC), active);
                }

                break;
            }
            case ClassJob.ROG:
            case ClassJob.NIN:
            {
                if (current_character.Jobs.Lancer.Level >= 30)
                {
                    DrawJob(current_character.Jobs.Ninja, ClassJob.NIN, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.NIN)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ROG)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Lancer.Level > 0);
                    DrawJob(current_character.Jobs.Rogue, ClassJob.ROG, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ROG), active);
                }

                break;
            }
            case ClassJob.SAM:
            {
                bool active = (current_character.Jobs.Samurai.Level >= 50);
                DrawJob(current_character.Jobs.Samurai, ClassJob.SAM, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SAM), active);

                break;
            }
            case ClassJob.RPR:
            {
                bool active = (current_character.Jobs.Reaper.Level >= 70);
                DrawJob(current_character.Jobs.Reaper, ClassJob.RPR, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.RPR), active);

                break;
            }
            case ClassJob.CNJ:
            case ClassJob.WHM:
            {
                if (current_character.Jobs.Lancer.Level >= 30)
                {
                    DrawJob(current_character.Jobs.WhiteMage, ClassJob.WHM, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.WHM)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CNJ)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Conjurer.Level > 0);
                    DrawJob(current_character.Jobs.Conjurer, ClassJob.CNJ, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CNJ), active);
                }

                break;
            }
            case ClassJob.ACN:
            case ClassJob.SCH:
            case ClassJob.SMN:
            {
                if (current_character.Jobs.Arcanist.Level >= 30)
                {
                    switch (job)
                    {
                        case ClassJob.SCH:
                        {
                            DrawJob(current_character.Jobs.Scholar, ClassJob.SCH, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SCH)}", true);
                            break;
                        }
                        case ClassJob.SMN:
                        {
                            DrawJob(current_character.Jobs.Summoner, ClassJob.SMN, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SMN)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ACN)}", true);
                            break;
                        }
                    }
                }
                else
                {
                    bool active = (current_character.Jobs.Arcanist.Level > 0);
                    DrawJob(current_character.Jobs.Arcanist, ClassJob.ACN, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ACN), active);
                }
                break;
            }
            case ClassJob.AST:
            {
                bool active = (current_character.Jobs.Astrologian.Level >= 30) ;
                DrawJob(current_character.Jobs.Astrologian, ClassJob.AST, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.AST), active);

                break;
            }
            case ClassJob.SGE:
            {
                bool active = (current_character.Jobs.Sage.Level >= 70);
                DrawJob(current_character.Jobs.Sage, ClassJob.SGE, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SGE), active);

                break;
            }
            case ClassJob.ARC:
            case ClassJob.BRD:
            {
                if (current_character.Jobs.Archer.Level >= 30)
                {
                    DrawJob(current_character.Jobs.Bard, ClassJob.BRD, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BRD)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ARC)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Archer.Level > 0);
                    DrawJob(current_character.Jobs.Archer, ClassJob.ARC, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ARC), active);
                }

                break;
            }
            case ClassJob.MCH:
            {
                bool active = (current_character.Jobs.Machinist.Level >= 30);
                DrawJob(current_character.Jobs.Machinist, ClassJob.MCH, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MCH), active);

                break;
            }
            case ClassJob.DNC:
            {
                bool active = (current_character.Jobs.Dancer.Level >= 60);
                DrawJob(current_character.Jobs.Dancer, ClassJob.DNC, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.DNC), active);

                break;
            }
            case ClassJob.THM:
            case ClassJob.BLM:
            {
                if (current_character.Jobs.Thaumaturge.Level >= 30)
                {
                    DrawJob(current_character.Jobs.BlackMage, ClassJob.BLM, $"{Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BLM)} / {Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.THM)}", true);
                }
                else
                {
                    bool active = (current_character.Jobs.Thaumaturge.Level > 0);
                    DrawJob(current_character.Jobs.Thaumaturge, ClassJob.THM, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.THM), active);
                }

                break;
            }
            case ClassJob.RDM:
            {
                bool active = (current_character.Jobs.RedMage.Level >= 50);
                DrawJob(current_character.Jobs.RedMage, ClassJob.RDM, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.RDM), active);

                break;
            }
            case ClassJob.BLU:
            {
                bool active = (current_character.Jobs.BlueMage.Level > 0);
                DrawJob(current_character.Jobs.BlueMage, ClassJob.BLU, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BLU), active);

                break;
            }
            case ClassJob.CRP:
            {
                bool active = (current_character.Jobs.Carpenter.Level > 0);
                DrawJob(current_character.Jobs.Carpenter, ClassJob.CRP, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CRP), active);

                break;
            }
            case ClassJob.BSM:
            {
                bool active = (current_character.Jobs.Blacksmith.Level > 0);
                DrawJob(current_character.Jobs.Blacksmith, ClassJob.BSM, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BSM), active);

                break;
            }
            case ClassJob.ARM:
            {
                bool active = (current_character.Jobs.Armorer.Level > 0);
                DrawJob(current_character.Jobs.Armorer, ClassJob.ARM, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ARM), active);

                break;
            }
            case ClassJob.GSM:
            {
                bool active = (current_character.Jobs.Goldsmith.Level > 0);
                DrawJob(current_character.Jobs.Goldsmith, ClassJob.GSM, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.GSM), active);

                break;
            }
            case ClassJob.LTW:
            {
                bool active = (current_character.Jobs.Leatherworker.Level > 0);
                DrawJob(current_character.Jobs.Leatherworker, ClassJob.LTW, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.LTW), active);

                break;
            }
            case ClassJob.WVR:
            {
                bool active = (current_character.Jobs.Weaver.Level > 0);
                DrawJob(current_character.Jobs.Weaver, ClassJob.WVR, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.WVR), active);

                break;
            }
            case ClassJob.ALC:
            {
                bool active = (current_character.Jobs.Alchemist.Level > 0);
                DrawJob(current_character.Jobs.Alchemist, ClassJob.ALC, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ALC), active);

                break;
            }
            case ClassJob.CUL:
            {
                bool active = (current_character.Jobs.Culinarian.Level > 0);
                DrawJob(current_character.Jobs.Culinarian, ClassJob.CUL, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CUL), active);

                break;
            }
            case ClassJob.MIN:
            {
                bool active = (current_character.Jobs.Miner.Level > 0);
                DrawJob(current_character.Jobs.Miner, ClassJob.MIN, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MIN), active);

                break;
            }
            case ClassJob.BTN:
            {
                bool active = (current_character.Jobs.Botanist.Level > 0);
                DrawJob(current_character.Jobs.Botanist, ClassJob.BTN, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BTN), active);

                break;
            }
            case ClassJob.FSH:
            {
                bool active = (current_character.Jobs.Fisher.Level > 0);
                DrawJob(current_character.Jobs.Fisher, ClassJob.FSH, Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.FSH), active);

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
        //pluginLog.Debug($"{job_id} {tooltip} {Utils.GetJobIconWithCornerSmall((uint)job_id)}");
        Vector2 alpha = active switch
        {
            true => new Vector2(1, 1),
            false => new Vector2(0.5f, 0.5f),
        };
        if (ImGui.BeginTable("###CharactersJobs#JobLine", 2))
        {
            ImGui.TableSetupColumn("###CharactersJobs#Icon", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersJobs#LevelNameExp", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            //Utils.DrawIcon(textureProvider, dataManager, pluginLog, new Vector2(42, 42), false, Utils.GetJobIconWithCornerSmall((uint)job_id), alpha);
            // Todo: Fix alpha
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(36, 36), false, Utils.GetJobIconWithCornerSmall((uint)job_id));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(tooltip);
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("###CharactersJobs#JobLevelNameExp", 2))
            {
                ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#Level", ImGuiTableColumnFlags.WidthFixed, 10);
                ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#NameExp", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{job.Level}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)job_id)}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                Utils.DrawLevelProgressBar(job.Exp, Utils.GetJobNextLevelExp(dataManager, pluginLog, currentLocale, job.Level), tooltip);
                ImGui.EndTable();
            }
            ImGui.EndTable();
        }
    }
}
