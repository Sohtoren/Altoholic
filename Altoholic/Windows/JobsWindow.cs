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
            if (ImGui.BeginTable("CharactersJobs", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginListBox("", new Vector2(200, -1)))
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
        if (ImGui.BeginTable("AllJobs", 41, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY))
        {
            // ImGuiCol.TableRowBg = new Vector4(255, 255, 255, 1); ;
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 35);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 25);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1898));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetAddonString(dataManager, pluginLog, currentLocale, 14055)
                );
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GLA, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.GLA)
                );
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.PLD, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.PLD)
                );
            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MRD, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MRD)
                );
            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WAR, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.WAR)
                );
            ImGui.TableSetColumnIndex(5);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DRK, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.DRK)
                );
            ImGui.TableSetColumnIndex(6);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GNB, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.GNB)
                );
            ImGui.TableSetColumnIndex(7);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CNJ, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CNJ)
                );
            ImGui.TableSetColumnIndex(8);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WHM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.WHM)
                );
            ImGui.TableSetColumnIndex(9);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SCH, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SCH)
                );
            ImGui.TableSetColumnIndex(10);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.AST, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.AST)
                );
            ImGui.TableSetColumnIndex(11);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SGE, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SGE)
                );
            ImGui.TableSetColumnIndex(12);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.PGL, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.PGL)
                );
            ImGui.TableSetColumnIndex(13);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MNK, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MNK)
                );
            ImGui.TableSetColumnIndex(14);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.LNC, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.LNC)
                );
            ImGui.TableSetColumnIndex(15);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DRG, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.DRG)
                );
            ImGui.TableSetColumnIndex(16);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ROG, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ROG)
                );
            ImGui.TableSetColumnIndex(17);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.NIN, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.NIN)
                );
            ImGui.TableSetColumnIndex(18);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SAM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SAM)
                );
            ImGui.TableSetColumnIndex(19);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.RPR, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.RPR)
                );
            ImGui.TableSetColumnIndex(20);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ARC, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ARC)
                );
            ImGui.TableSetColumnIndex(21);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BRD, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BRD)
                );
            ImGui.TableSetColumnIndex(22);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MCH, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MCH)
                );
            ImGui.TableSetColumnIndex(23);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.DNC, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.DNC)
                );
            ImGui.TableSetColumnIndex(24);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.THM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.THM)
                );
            ImGui.TableSetColumnIndex(25);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BLM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BLM)
                );
            ImGui.TableSetColumnIndex(26);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ACN, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ACN)
                );
            ImGui.TableSetColumnIndex(27);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.SMN, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.SMN)
                );
            ImGui.TableSetColumnIndex(28);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.RDM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.RDM)
                );
            ImGui.TableSetColumnIndex(29);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BLU));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BLU)
                );
            ImGui.TableSetColumnIndex(30);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CRP, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CRP)
                );
            ImGui.TableSetColumnIndex(31);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BSM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BSM)
                );
            ImGui.TableSetColumnIndex(32);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ARM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ARM)
                );
            ImGui.TableSetColumnIndex(33);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.GSM, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MRD)
                );
            ImGui.TableSetColumnIndex(34);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.LTW, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.LTW)
                );
            ImGui.TableSetColumnIndex(35);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.WVR, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.WVR)
                );
            ImGui.TableSetColumnIndex(36);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.ALC, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.ALC)
                );
            ImGui.TableSetColumnIndex(37);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.CUL, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.CUL)
                );
            ImGui.TableSetColumnIndex(38);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.MIN, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.MIN)
                );
            ImGui.TableSetColumnIndex(39);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.BTN, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.BTN)
                );
            ImGui.TableSetColumnIndex(40);
            ImGui.TextUnformatted(Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)ClassJob.FSH, true));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetJobNameFromId(dataManager, pluginLog,  currentLocale, (uint)ClassJob.FSH)
                );
            /*ImGui.TableSetupColumn("PLD", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("MRD", ImGuiTableColumnFlags.WidthFixed, 22);
            ImGui.TableSetupColumn("WAR", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("DRK", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("GNB", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("CNJ", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("WHM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("SCH", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("AST", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("SGE", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("PUG", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("MNK", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("LNC", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("DRG", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("ROG", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("NIN", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("SAM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("RPR", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("BRD", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("ARC", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("MCH", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("DNC", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("BLM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("THM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("SMN", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("ACN", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("RDM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("BLU", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("CRP", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("BSM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("ARM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("GSM", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("LTW", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("WVR", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("ALC", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("CUL", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("MIN", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("BTN", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableSetupColumn("FSH", ImGuiTableColumnFlags.WidthFixed, 20);*/
            //ImGui.TableHeadersRow();
            foreach (Character currChar in chars)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}"
                    );
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
        if (ImGui.BeginTabBar($"#attributesprofile"))
        {
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1080)}"))
            {
                DrawDoWDoMJobs(current_character);
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1081)}"))
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
        if (ImGui.BeginTable("DoWDoMJobs", 2, ImGuiTableFlags.ScrollY))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("RoleTank", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(0));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1082)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("RoleHealer", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
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
            if (ImGui.BeginTable("RoleMelee", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(2));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 1084)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("RolePhysicalRanged", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
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
            if (ImGui.BeginTable("RoleMagicalRanged", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
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
        if (ImGui.BeginTable("DoHDoLJobs", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("RoleDoH", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetRoleIcon(5));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 802)}");

                ImGui.EndTable();
            }
            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("RoleDoL", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
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
        if (ImGui.BeginTable("JobLine", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            //Utils.DrawIcon(textureProvider, dataManager, pluginLog, new Vector2(42, 42), false, Utils.GetJobIconWithCornerSmall((uint)job_id), alpha);
            // Todo: Fix alpha
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(36, 36), false, Utils.GetJobIconWithCornerSmall((uint)job_id));
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(
                    tooltip
                );
            }
            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("JobLevelNameExp", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 10);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{job.Level}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, (uint)job_id)}");
                ImGui.TableNextRow();
                ImGui.TextUnformatted($"{job.Exp}");
                ImGui.EndTable();
            }
            ImGui.EndTable();
        }
    }
}
