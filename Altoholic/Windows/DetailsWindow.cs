using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;

namespace Altoholic.Windows;

public class DetailsWindow : Window, IDisposable
{
    private Plugin plugin;
    private ClientLanguage currentLocale;
    private GlobalCache _globalCache;

    public DetailsWindow(
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

        characterIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/Character.uld");
        characterTextures.Add(GearSlot.MH, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 17));
        characterTextures.Add(GearSlot.HEAD, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 19));
        characterTextures.Add(GearSlot.BODY, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 20));
        characterTextures.Add(GearSlot.HANDS, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 21));
        characterTextures.Add(GearSlot.BELT, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 22));
        characterTextures.Add(GearSlot.LEGS, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 23));
        characterTextures.Add(GearSlot.FEET, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 24));
        characterTextures.Add(GearSlot.OH, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 18));
        characterTextures.Add(GearSlot.EARS, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 25));
        characterTextures.Add(GearSlot.NECK, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 26));
        characterTextures.Add(GearSlot.WRISTS, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 27));
        characterTextures.Add(GearSlot.LEFT_RING, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 28));
        characterTextures.Add(GearSlot.RIGHT_RING, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 28));
        characterTextures.Add(GearSlot.SOUL_CRYSTAL, characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 29));
        characterTextures.Add(GearSlot.EMPTY, Plugin.TextureProvider.GetTextureFromGame("ui/uld/fourth/DragTargetA_hr1.tex"));
    }

    public Func<Character>? GetPlayer { get; init; }
    public Func<List<Character>>? GetOthersCharactersList { get; set; }
    private Character? current_character = null;
    private readonly UldWrapper characterIcons;
    private Dictionary<GearSlot, IDalamudTextureWrap?> characterTextures = [];

    public override void OnClose()
    {
        Plugin.Log.Debug("DetailsWindow, OnClose() called");
        current_character = null;
    }

    public void Dispose()
    {
        current_character = null;
        foreach(var loadedTexture in characterTextures) loadedTexture.Value?.Dispose();
        characterIcons.Dispose();
    }

    public override void Draw()
    {
        currentLocale = plugin.Configuration.Language;
        if (GetPlayer is null) return;
        if (GetOthersCharactersList is null) return;
        if (Plugin.DataManager is null) return;
        if (Plugin.TextureProvider is null) return;
        Character character = GetPlayer.Invoke();
        if (character == null) return;
        //Plugin.Log.Debug($"DrawDetails character with c : id = {character.Id}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}, Quests = {character.Quests.Count}, Inventory = {character.Inventory.Count}, Gear {character.Gear.Count}, Retainers = {character.Retainers.Count}");
        var chars = new List<Character>();
        chars.Insert(0, character);
        chars.AddRange(GetOthersCharactersList.Invoke());
        try
        {
            if (ImGui.BeginTable("###CharactersDetailsTable", 2))
            {
                ImGui.TableSetupColumn("###CharactersDetailsTable#CharacterLists", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn("###CharactersDetailsTable#Details", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginListBox("###CharactersDetailsTable#CharactersListBox", new Vector2(200, -1)))
                {
                    ImGui.SetScrollY(0);
                    foreach (Character currChar in chars)
                    {
                        if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == current_character))
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
            Plugin.Log.Debug("Altoholic : Exception : {0}", e);
        }
    }

    private void DrawDetails(Character selected_character)
    {
        if (ImGui.BeginTable("###CharactersDetailsTable#ProfileTable", 2))
        {
            ImGui.TableSetupColumn("###CharactersDetailsTable#ProfileTable#ProfileCol", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###CharactersDetailsTable#ProfileTable#GearCol", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTabBar($"###CharactersDetailsTable#ProfileTable#ProfileCol#ProfileTabBar"))
            {
                /*if (ImGui.BeginTabItem($"{Utils.GetAddonString(758)}"))
                {
                    //DrawAttributes(current_character);
                    ImGui.TextUnformatted("");
                    ImGui.EndTabItem();
                }*/
                if (ImGui.BeginTabItem($"{Utils.GetAddonString(759)}"))
                {
                    DrawProfile(selected_character);
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
            ImGui.TableSetColumnIndex(1);
            Utils.DrawGear(ref _globalCache, ref characterTextures, selected_character.Gear, selected_character.LastJob, selected_character.LastJobLevel, 300, 350);

            ImGui.EndTable();
        }
    }

    private void DrawAttributes(Character selected_character)
    {
        if (selected_character.Attributes is null) return;
        if (ImGui.BeginTable("###Attributes#HealthManaBarsTable", 2))
        {
            ImGui.TableSetupColumn("###HealthManaBarsTable#HPHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###HealthManaBarsTable#MPHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"HP      {selected_character.Attributes.Hp}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"MP      {selected_character.Attributes.Mp}");
            ImGui.EndTable();
        }
        
        if (ImGui.BeginTable("###Attributes#StrDexVitIntMindTable", 2))
        {
            ImGui.TableSetupColumn("###Attributes#StrDexVitIntMindTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###Attributes#StrDexVitIntMindTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TextUnformatted("Attributes");
            ImGui.Separator();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.BeginTable("###Attributes#StrDexVitTable", 2))
            {
                ImGui.TableSetupColumn("###Attributes#StrDexVitTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("###Attributes#StrDexVitTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Strength");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{selected_character.Attributes.Strength}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Dexterity");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{selected_character.Attributes.Dexterity}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Vitality");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{selected_character.Attributes.Vitality}");
                ImGui.EndTable();
            }
            ImGui.TableSetColumnIndex(1);
            if (ImGui.BeginTable("###Attributes#IntMindTable", 2))
            {
                ImGui.TableSetupColumn("###Attributes#IntMindTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("###Attributes#IntMindTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Intelligence");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{selected_character.Attributes.Intelligence}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Mind");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{selected_character.Attributes.Mind}");
                ImGui.EndTable();
            }
            ImGui.EndTable();
        }
    }

    private void DrawProfile(Character selected_character)
    {
        if (selected_character.Profile is null) return;
        ImGui.TextUnformatted($"{Utils.GetAddonString(790)}");//Title
        ImGui.Separator();
        //Plugin.Log.Debug($"Title: {current_character.Profile.Title}, isprefix: {current_character.Profile.TitleIsPrefix}");
        if (!string.IsNullOrEmpty(selected_character.Profile.Title) && selected_character.Profile.TitleIsPrefix)
        {
            ImGui.TextUnformatted($"{selected_character.Profile.Title}");
            if (!string.IsNullOrEmpty(selected_character.FirstName) && !string.IsNullOrEmpty(selected_character.LastName))
            {
                ImGui.TextUnformatted($"            {selected_character.FirstName} {selected_character.LastName}");
            }
        }
        else if (!string.IsNullOrEmpty(selected_character.Profile.Title) && !selected_character.Profile.TitleIsPrefix)
        {
            if (!string.IsNullOrEmpty(selected_character.FirstName) && !string.IsNullOrEmpty(selected_character.LastName))
            {
                ImGui.TextUnformatted($"{selected_character.FirstName} {selected_character.LastName}");
                ImGui.TextUnformatted($"            {selected_character.Profile.Title}");
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(selected_character.FirstName) && !string.IsNullOrEmpty(selected_character.LastName))
            {
                ImGui.TextUnformatted($"{selected_character.FirstName} {selected_character.LastName}");
            }
        }

        if (selected_character.Profile.Grand_Company is not 0)
        {
            ImGui.TextUnformatted("");
            ImGui.TextUnformatted($"{Utils.GetAddonString(791)}");//Grand Company
            ImGui.Separator();
            if (ImGui.BeginTable("###ProfileTable#GrandCompanyTable", 2))
            {
                ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
                ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#Rank", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                //Utils.DrawIcon(new Vector2(40, 40), false, Utils.GetGrandCompanyIcon(selected_character.Profile.Grand_Company));
                Utils.DrawIcon_test(_globalCache.IconStorage.LoadIcon(Utils.GetGrandCompanyIcon(selected_character.Profile.Grand_Company)), new Vector2(40, 40));

                ImGui.TableSetColumnIndex(1);
                if (ImGui.BeginTable("###ProfileTable#GrandCompanyTable#RankTable", 2))
                {
                    ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#RankTable#Name", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#RankTable#Icon", ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{Utils.GetGrandCompany(selected_character.Profile.Grand_Company)}");
                    ImGui.TextUnformatted($"{Utils.Capitalize(Utils.GetGrandCompanyRank(selected_character.Profile.Grand_Company, selected_character.Profile.Grand_Company_Rank, selected_character.Profile.Gender))}");
                    ImGui.TableSetColumnIndex(1);
                    //Utils.DrawIcon(new Vector2(48, 48), false, Utils.GetGrandCompanyRankIcon(selected_character.Profile.Grand_Company, selected_character.Profile.Grand_Company_Rank));
                    Utils.DrawIcon_test(_globalCache.IconStorage.LoadIcon(Utils.GetGrandCompanyRankIcon(selected_character.Profile.Grand_Company, selected_character.Profile.Grand_Company_Rank)), new Vector2(48, 48));

                    ImGui.EndTable();
                }

                ImGui.EndTable();
            }
        }
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{Utils.GetAddonString(793)}");//Race/Clan/Gender
        ImGui.Separator();
        ImGui.TextUnformatted($"{Utils.GetRace(selected_character.Profile.Gender, selected_character.Profile.Race)} / {Utils.GetTribe(selected_character.Profile.Gender, selected_character.Profile.Tribe)} / {Utils.GetGender(selected_character.Profile.Gender)}");
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{Utils.GetAddonString(794)}");//City-state
        ImGui.Separator();
        if (ImGui.BeginTable("###ProfileTable#ProfileColumn#ProfileTable", 2))
        {
            ImGui.TableSetupColumn("###ProfileTable#ProfileColumn#ProfileTable#CityIcon", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("###ProfileTable#ProfileColumn#ProfileTable#CityName", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            //Utils.DrawIcon(new Vector2(36, 36), false, Utils.GetTownIcon(selected_character.Profile.City_State));
            Utils.DrawIcon_test(_globalCache.IconStorage.LoadIcon(Utils.GetTownIcon(selected_character.Profile.City_State)), new Vector2(36, 36));
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetTown(selected_character.Profile.City_State)}");

            ImGui.EndTable();
        }
        ImGui.TextUnformatted($"{Utils.GetAddonString(795)}");//Nameday
        ImGui.Separator();
        ImGui.TextUnformatted($"{Utils.GetNameday(selected_character.Profile.Nameday_Day, selected_character.Profile.Nameday_Month)}");
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{Utils.GetAddonString(796)}");//Guardian
        ImGui.Separator();
        if (ImGui.BeginTable("###ProfileTable#GuardianTable", 2))
        {
            ImGui.TableSetupColumn("###ProfileTable#GuardianTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("###ProfileTable#GuardianTable#Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            //Utils.DrawIcon(new Vector2(40, 40), false, Utils.GetGuardianIcon(selected_character.Profile.Guardian));
            Utils.DrawIcon_test(_globalCache.IconStorage.LoadIcon(Utils.GetGuardianIcon(selected_character.Profile.Guardian)), new Vector2(36, 36));
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetGuardian(selected_character.Profile.Guardian)}");

            ImGui.EndTable();
        }
    }
}
