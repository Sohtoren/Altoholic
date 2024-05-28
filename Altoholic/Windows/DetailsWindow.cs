using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Altoholic.Models;
using Dalamud;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using ImGuiScene;

namespace Altoholic.Windows;

public class DetailsWindow : Window, IDisposable
{
    private Plugin plugin;

    private readonly IPluginLog pluginLog;
    private readonly IDataManager dataManager;
    private readonly ITextureProvider textureProvider;
    private readonly ClientLanguage currentLocale;

    public DetailsWindow(
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
    private Character current_character = null!;


    public void Dispose()
    {
        //current_character = null!;
    }

    public override void Draw()
    {
        var chars = new List<Character>();
        chars.Insert(0, GetPlayer.Invoke());
        chars.AddRange(GetOthersCharactersList.Invoke());

        try
        {
            if (ImGui.BeginTable("CharactersDetails", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginListBox("", new Vector2(200, -1)))
                {
                    ImGui.SetScrollY(0);
                    foreach (Character currChar in chars)
                    {
                        if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}\uE05D{currChar.HomeWorld}"))
                        {
                            current_character = currChar;
                        }
                    }

                    ImGui.EndListBox();
                }
                ImGui.TableSetColumnIndex(1);

                if (current_character is not null)
                {
                    DrawDetails(current_character);
                }

                ImGui.EndTable();
            }
        }
        catch (Exception e)
        {
            pluginLog.Debug("Altoholic : Exception : {0}", e);
        }
    }

    private void DrawDetails(Character current_character)
    {
        if (current_character is null) return;
        if (ImGui.BeginTable("AttributesProfile", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTabBar($"#attributesprofile"))
            {
                /*if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 758)}"))
                {
                    //DrawAttributes(current_character);
                    ImGui.Text("");
                    ImGui.EndTabItem();
                }*/
                if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 759)}"))
                {
                    DrawProfile(current_character);
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
            ImGui.TableSetColumnIndex(1);
            DrawGear(current_character);

            ImGui.EndTable();
        }
    }

    private void DrawAttributes(Character current_character)
    {
        if (ImGui.BeginTable("HealthManaBars", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text($"HP      {current_character.Attributes.Hp}");
            ImGui.TableNextColumn();
            ImGui.Text($"MP      {current_character.Attributes.Mp}");
            ImGui.EndTable();
        }
        
        if (ImGui.BeginTable("TribalCurrencyTable", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.Text("Attributes");
            ImGui.Separator();
            ImGui.TableNextColumn();
            if (ImGui.BeginTable(string.Empty, 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Strength");
                ImGui.TableNextColumn();
                ImGui.Text($"{current_character.Attributes.Strength}");
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Dexterity");
                ImGui.TableNextColumn();
                ImGui.Text($"{current_character.Attributes.Dexterity}");
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Vitality");
                ImGui.TableNextColumn();
                ImGui.Text($"{current_character.Attributes.Vitality}");
                ImGui.EndTable();
            }
            ImGui.TableNextColumn();
            if (ImGui.BeginTable(string.Empty, 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Intelligence");
                ImGui.TableNextColumn();
                ImGui.Text($"{current_character.Attributes.Intelligence}");
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Mind");
                ImGui.TableNextColumn();
                ImGui.Text($"{current_character.Attributes.Mind}");
                ImGui.EndTable();
            }
            ImGui.EndTable();
        }
    }

    private void DrawProfile(Character current_character)
    {
        if (current_character.Profile is null) return;

        ImGui.Text($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 790)}");//Title
        ImGui.Separator();        
        if (!string.IsNullOrEmpty(current_character.Profile.Title))
        {
            ImGui.Text($"{current_character.Profile.Title}");
        }
        if (!string.IsNullOrEmpty(current_character.FirstName) && !string.IsNullOrEmpty(current_character.LastName))
        {
            ImGui.Text($"{current_character.FirstName} {current_character.LastName}");
        }

        if (current_character.Profile.Grand_Company is not 0)
        {
            ImGui.Text("");
            ImGui.Text($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 791)}");//Grand Company
            ImGui.Separator();
            if (ImGui.BeginTable(string.Empty, 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(40, 40), false, Utils.GetGrandCompanyIcon(current_character.Profile.Grand_Company));

                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable(string.Empty, 2))
                {
                    ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"{Utils.GetGrandCompany(dataManager, pluginLog, currentLocale, current_character.Profile.Grand_Company)}");
                    ImGui.Text($"{Utils.Capitalize(Utils.GetGrandCompanyRank(dataManager, pluginLog, currentLocale, current_character.Profile.Grand_Company, current_character.Profile.Grand_Company_Rank, current_character.Profile.Gender))}");
                    ImGui.TableSetColumnIndex(1);
                    Utils.DrawIcon(textureProvider, pluginLog, new Vector2(48, 48), false, Utils.GetGrandCompanyRankIcon(current_character.Profile.Grand_Company, current_character.Profile.Grand_Company_Rank));

                    ImGui.EndTable();
                }

                ImGui.EndTable();
            }
        }
        ImGui.Text("");
        ImGui.Text($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 793)}");//Race/Clan/Gender
        ImGui.Separator();
        ImGui.Text($"{Utils.GetRace(dataManager, pluginLog, currentLocale, current_character.Profile.Gender, current_character.Profile.Race)} / {Utils.GetTribe(dataManager, pluginLog, currentLocale, current_character.Profile.Gender, current_character.Profile.Tribe)} / {Utils.GetGender(current_character.Profile.Gender)}");
        ImGui.Text("");
        ImGui.Text($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 794)}");//City-state
        ImGui.Separator();
        if (ImGui.BeginTable(string.Empty, 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(36, 36), false, Utils.GetTownIcon(current_character.Profile.City_State));
            ImGui.TableSetColumnIndex(1);
            ImGui.Text($"{Utils.GetTown(dataManager, pluginLog, currentLocale, current_character.Profile.City_State)}");

            ImGui.EndTable();
        }
        ImGui.Text($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 795)}");//Nameday
        ImGui.Separator();
        ImGui.Text($"{Utils.GetNameday(current_character.Profile.Nameday_Day, current_character.Profile.Nameday_Month)}");
        ImGui.Text("");
        ImGui.Text($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 796)}");//Guardian
        ImGui.Separator();
        if (ImGui.BeginTable(string.Empty, 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(40, 40), false, Utils.GetGuardianIcon(current_character.Profile.Guardian));
            ImGui.TableSetColumnIndex(1);
            ImGui.Text($"{Utils.GetGuardian(dataManager, pluginLog, currentLocale, current_character.Profile.Guardian)}");

            ImGui.EndTable();
        }
    }

    private void DrawGear(Character current_character)
    {
        if (current_character.Gear.Count == 0) return;
        if (ImGui.BeginTable(string.Empty, 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            var MH = current_character.Gear.First(g => g.Slot == (short)GearSlot.MH);
            if (MH == null || MH.ItemId == 0)
            {
                Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(40, 40), false, 13775);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(
                        Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11524)
                    );
                }
            }
            else
            {
                Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(40, 40), MH.HQ, MH.ItemId);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(
                        Utils.GetItemNameFromId(dataManager, pluginLog, currentLocale, MH.ItemId)
                    );
                }
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.Text($"Level {current_character.LastJobLevel}");
            if (ImGui.BeginTable(string.Empty, 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(20, 20), false, Utils.GetJobIcon(current_character.LastJob));
                ImGui.TableSetColumnIndex(1);
                ImGui.Text($"{Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, current_character.LastJob)}");

                ImGui.EndTable();
            }
            

            ImGui.EndTable();
        }

        if (ImGui.BeginTable(string.Empty, 3))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            if (ImGui.BeginTable(string.Empty, 1))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 42);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.HEAD, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11525), new Vector2(40, 40), 10032);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.BODY, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11526), new Vector2(40, 40), 10033);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.HANDS, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11527), new Vector2(40, 40), 10034);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.LEGS, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11528), new Vector2(40, 40), 10035);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.FEET, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11529), new Vector2(40, 40), 10035);

                ImGui.EndTable();
            }
            ImGui.TableSetColumnIndex(1);
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(300, 350), false, 055396);
            ImGui.TableSetColumnIndex(2);
            if (ImGui.BeginTable(string.Empty, 1))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 42);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.OH, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 12227), new Vector2(40, 40), 30067);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.EARS, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11530), new Vector2(40, 40), 9293);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.NECK, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11531), new Vector2(40, 40), 9292);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.WRISTS, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11532), new Vector2(40, 40), 9294);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.RIGHT_RING, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11533), new Vector2(40, 40), 9295);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.LEFT_RING, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11534), new Vector2(40, 40), 9295);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawGearPiece(current_character, GearSlot.SOUL_CRYSTAL, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 12238), new Vector2(40, 40), 9295);//Todo: Find Soul Crystal empty icon

                ImGui.EndTable();
            }

            ImGui.EndTable();
        }
    }

    private void DrawGearPiece(Character current_character, GearSlot slot, string tooltip, Vector2 icon_size, uint fallback_icon)
    {
        var GEAR = current_character.Gear.First(g => g.Slot == (short)slot);
        if (GEAR == null || GEAR.ItemId == 0)
        {
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, icon_size, false, fallback_icon);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(
                    tooltip
                );
            }
        }
        else
        {
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, icon_size, GEAR.HQ, GEAR.ItemId);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(
                    Utils.GetItemNameFromId(dataManager, pluginLog, currentLocale, GEAR.ItemId)
                );
            }
        }
    }
}
