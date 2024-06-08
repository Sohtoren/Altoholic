using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;

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

    public Func<Character>? GetPlayer { get; init; }
    public Func<List<Character>>? GetOthersCharactersList { get; set; }
    private Character? current_character = null;


    public override void OnClose()
    {
        pluginLog.Debug("DetailsWindow, OnClose() called");
        current_character = null;
    }

    public void Dispose()
    {
        current_character = null;
    }

    public override void Draw()
    {
        if (GetPlayer is null) return;
        if (GetOthersCharactersList is null) return;
        if (dataManager is null) return;
        if (textureProvider is null) return;
        Character character = GetPlayer.Invoke();
        if (character == null) return;
        //pluginLog.Debug($"DrawDetails character with c : id = {character.Id}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}, Quests = {character.Quests.Count}, Inventory = {character.Inventory.Count}, Gear {character.Gear.Count}, Retainers = {character.Retainers.Count}");
        var chars = new List<Character>();
        chars.Insert(0, character);
        chars.AddRange(GetOthersCharactersList.Invoke());
        try
        {
            if (ImGui.BeginTable("CharactersDetailsTable", 2))
            {
                ImGui.TableSetupColumn("##CharactersDetailsTable#CharacterListHeader", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn("##CharactersDetailsTable#DetailsHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginListBox("##CharactersDetailsTable#CharacterListBox", new Vector2(200, -1)))
                {
                    ImGui.SetScrollY(0);
                    foreach (Character currChar in chars)
                    {
                        if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}"))
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
        if (ImGui.BeginTable("##AttributesProfileTable", 2))
        {
            ImGui.TableSetupColumn("##AttributesProfileTable#ProfileColumn", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("##AttributesProfileTable#GearColumn", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTabBar($"##AttributesProfileTable#ProfileColumn#ProfileTabBar"))
            {
                /*if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 758)}"))
                {
                    //DrawAttributes(current_character);
                    ImGui.TextUnformatted("");
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
        if (current_character.Attributes is null) return;
        if (ImGui.BeginTable("##Attributes#HealthManaBarsTable", 2))
        {
            ImGui.TableSetupColumn("##HealthManaBarsTable#HPHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("##HealthManaBarsTable#MPHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"HP      {current_character.Attributes.Hp}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"MP      {current_character.Attributes.Mp}");
            ImGui.EndTable();
        }
        
        if (ImGui.BeginTable("##Attributes#StrDexVitIntMindTable", 2))
        {
            ImGui.TableSetupColumn("##Attributes#StrDexVitIntMindTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("##Attributes#StrDexVitIntMindTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TextUnformatted("Attributes");
            ImGui.Separator();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("##Attributes#StrDexVitTable", 2))
            {
                ImGui.TableSetupColumn("##Attributes#StrDexVitTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("##Attributes#StrDexVitTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Strength");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{current_character.Attributes.Strength}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Dexterity");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{current_character.Attributes.Dexterity}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Vitality");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{current_character.Attributes.Vitality}");
                ImGui.EndTable();
            }
            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("##Attributes#IntMindTable", 2))
            {
                ImGui.TableSetupColumn("##Attributes#IntMindTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("##Attributes#IntMindTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Intelligence");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{current_character.Attributes.Intelligence}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Mind");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{current_character.Attributes.Mind}");
                ImGui.EndTable();
            }
            ImGui.EndTable();
        }
    }

    private void DrawProfile(Character current_character)
    {
        if (current_character.Profile is null) return;
        ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 790)}");//Title
        ImGui.Separator();        
        if (!string.IsNullOrEmpty(current_character.Profile.Title))
        {
            ImGui.TextUnformatted($"{current_character.Profile.Title}");
        }
        if (!string.IsNullOrEmpty(current_character.FirstName) && !string.IsNullOrEmpty(current_character.LastName))
        {
            ImGui.TextUnformatted($"{current_character.FirstName} {current_character.LastName}");
        }

        if (current_character.Profile.Grand_Company is not 0)
        {
            ImGui.TextUnformatted("");
            ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 791)}");//Grand Company
            ImGui.Separator();
            if (ImGui.BeginTable("##ProfileTable#GrandCompanyTable", 2))
            {
                ImGui.TableSetupColumn("##ProfileTable#GrandCompanyTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn("##ProfileTable#GrandCompanyTable#Rank", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(40, 40), false, Utils.GetGrandCompanyIcon(current_character.Profile.Grand_Company));

                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable("##ProfileTable#GrandCompanyTable#RankTable", 2))
                {
                    ImGui.TableSetupColumn("##ProfileTable#GrandCompanyTable#RankTable#Name", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn("##ProfileTable#GrandCompanyTable#RankTable#Icon", ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{Utils.GetGrandCompany(dataManager, pluginLog, currentLocale, current_character.Profile.Grand_Company)}");
                    ImGui.TextUnformatted($"{Utils.Capitalize(Utils.GetGrandCompanyRank(dataManager, pluginLog, currentLocale, current_character.Profile.Grand_Company, current_character.Profile.Grand_Company_Rank, current_character.Profile.Gender))}");
                    ImGui.TableSetColumnIndex(1);
                    Utils.DrawIcon(textureProvider, pluginLog, new Vector2(48, 48), false, Utils.GetGrandCompanyRankIcon(current_character.Profile.Grand_Company, current_character.Profile.Grand_Company_Rank));

                    ImGui.EndTable();
                }

                ImGui.EndTable();
            }
        }
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 793)}");//Race/Clan/Gender
        ImGui.Separator();
        ImGui.TextUnformatted($"{Utils.GetRace(dataManager, pluginLog, currentLocale, current_character.Profile.Gender, current_character.Profile.Race)} / {Utils.GetTribe(dataManager, pluginLog, currentLocale, current_character.Profile.Gender, current_character.Profile.Tribe)} / {Utils.GetGender(current_character.Profile.Gender)}");
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 794)}");//City-state
        ImGui.Separator();
        if (ImGui.BeginTable("##ProfileTable#ProfileColumn#ProfileTable", 2))
        {
            ImGui.TableSetupColumn("##ProfileTable#ProfileColumn#ProfileTable#CityIcon", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("##ProfileTable#ProfileColumn#ProfileTable#CityName", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(36, 36), false, Utils.GetTownIcon(current_character.Profile.City_State));
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetTown(dataManager, pluginLog, currentLocale, current_character.Profile.City_State)}");

            ImGui.EndTable();
        }
        ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 795)}");//Nameday
        ImGui.Separator();
        ImGui.TextUnformatted($"{Utils.GetNameday(current_character.Profile.Nameday_Day, current_character.Profile.Nameday_Month)}");
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 796)}");//Guardian
        ImGui.Separator();
        if (ImGui.BeginTable("##ProfileTable#GuardianTable", 2))
        {
            ImGui.TableSetupColumn("##ProfileTable#GuardianTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("##ProfileTable#GuardianTable#Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(textureProvider, pluginLog, new Vector2(40, 40), false, Utils.GetGuardianIcon(current_character.Profile.Guardian));
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetGuardian(dataManager, pluginLog, currentLocale, current_character.Profile.Guardian)}");

            ImGui.EndTable();
        }
    }

    private void DrawGear(Character current_character)
    {
        if (current_character.Gear.Count == 0) return;
        if (ImGui.BeginTable("##GearTableHeader", 2))
        {
            ImGui.TableSetupColumn("##GearTableHeader#MHColumn", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("##GearTableHeader#RoleIconNameColumn", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawGearPiece(current_character, GearSlot.MH, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 11524), new Vector2(40, 40), 13775);
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"Level {current_character.LastJobLevel}");
            if (ImGui.BeginTable("##GearTable#RoleIconNameTable", 2))
            {
                ImGui.TableSetupColumn("##GearTable#RoleColumn#RoleIcon", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn("##GearTable#RoleColumn#RoleName", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(40, 40), false, Utils.GetJobIcon(current_character.LastJob));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetJobNameFromId(dataManager, pluginLog, currentLocale, current_character.LastJob)}");

                ImGui.EndTable();
            }

            ImGui.EndTable();
        }

        if (ImGui.BeginTable("##GearTable", 3))
        {
            ImGui.TableSetupColumn("##GearTable#LeftGearColumnHeader", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("##GearTable#CentralColumnHeader", ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableSetupColumn("##GearTable#RightGearColumnHeader", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("##GearTable#LeftGearColumn", 1))
            {
                ImGui.TableSetupColumn("##GearTable#LeftGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
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
            if (ImGui.BeginTable("##GearTable#RightGearColumn", 1))
            {
                ImGui.TableSetupColumn("##GearTable#RightGearColum#Column", ImGuiTableColumnFlags.WidthFixed, 42);
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
                DrawGearPiece(current_character, GearSlot.SOUL_CRYSTAL, Utils.GetAddonString(dataManager, pluginLog, currentLocale, 12238), new Vector2(40, 40), 55396);//Todo: Find Soul Crystal empty icon
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
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(tooltip);
                ImGui.EndTooltip();
            }
        }
        else
        {
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, icon_size, GEAR.HQ, GEAR.ItemId);
            if (ImGui.IsItemHovered())
            {
                Utils.DrawItemTooltip(textureProvider, dataManager, pluginLog, currentLocale, GEAR);
            }
        }
    }
}
