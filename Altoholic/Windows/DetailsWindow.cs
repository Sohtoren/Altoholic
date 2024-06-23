using System;
using System.Collections.Generic;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Linq;

namespace Altoholic.Windows;

public class DetailsWindow : Window, IDisposable
{
    private readonly Plugin _plugin;
    private ClientLanguage _currentLocale;
    private GlobalCache _globalCache;

    public DetailsWindow(
        Plugin plugin,
        string name,
        GlobalCache globalCache
        ) 
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

        _characterIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/Character.uld");
        _characterTextures.Add(GearSlot.MH, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 17));
        _characterTextures.Add(GearSlot.HEAD, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 19));
        _characterTextures.Add(GearSlot.BODY, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 20));
        _characterTextures.Add(GearSlot.HANDS, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 21));
        _characterTextures.Add(GearSlot.BELT, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 22));
        _characterTextures.Add(GearSlot.LEGS, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 23));
        _characterTextures.Add(GearSlot.FEET, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 24));
        _characterTextures.Add(GearSlot.OH, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 18));
        _characterTextures.Add(GearSlot.EARS, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 25));
        _characterTextures.Add(GearSlot.NECK, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 26));
        _characterTextures.Add(GearSlot.WRISTS, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 27));
        _characterTextures.Add(GearSlot.LEFT_RING, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 28));
        _characterTextures.Add(GearSlot.RIGHT_RING, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 28));
        _characterTextures.Add(GearSlot.SOUL_CRYSTAL, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 29));
        _characterTextures.Add(GearSlot.EMPTY, Plugin.TextureProvider.GetTextureFromGame("ui/uld/fourth/DragTargetA_hr1.tex"));
    }

    public Func<Character>? GetPlayer { get; init; }
    public Func<List<Character>>? GetOthersCharactersList { get; set; }
    private Character? _currentCharacter;
    private readonly UldWrapper _characterIcons;
    private Dictionary<GearSlot, IDalamudTextureWrap?> _characterTextures = [];

    public override void OnClose()
    {
        Plugin.Log.Debug("DetailsWindow, OnClose() called");
        _currentCharacter = null;
    }

    public void Dispose()
    {
        _currentCharacter = null;
        foreach(KeyValuePair<GearSlot, IDalamudTextureWrap?> loadedTexture in _characterTextures) loadedTexture.Value?.Dispose();
        _characterIcons.Dispose();
    }

    public override void Draw()
    {
        if(GetPlayer?.Invoke() == null) return;
        if(GetOthersCharactersList?.Invoke() == null) return;
        _currentLocale = _plugin.Configuration.Language;
        //Plugin.Log.Debug($"DrawDetails character with c : id = {character.Id}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}, Quests = {character.Quests.Count}, Inventory = {character.Inventory.Count}, Gear {character.Gear.Count}, Retainers = {character.Retainers.Count}");
        List<Character> chars = [];
        chars.Insert(0, GetPlayer.Invoke());
        chars.AddRange(GetOthersCharactersList.Invoke());
        try
        {
            using var characterDetailsTable = ImRaii.Table("###CharactersDetailsTable", 2);
            if (!characterDetailsTable) return;
            ImGui.TableSetupColumn("###CharactersDetailsTable#CharacterLists", ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn("###CharactersDetailsTable#Details", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox = ImRaii.ListBox("###CharactersDetailsTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {
                    ImGui.SetScrollY(0);
                    foreach (Character currChar in chars.Where(currChar =>
                                 ImGui.Selectable(
                                     $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                     currChar == _currentCharacter)))
                    {
                        _currentCharacter = currChar;
                    }
                }
            }
            ImGui.TableSetColumnIndex(1);
            if (_currentCharacter is not null)
            {
                DrawDetails(_currentCharacter);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.Debug("Altoholic : Exception : {0}", e);
        }
    }

    private void DrawDetails(Character selectedCharacter)
    {
        using var charactersDetailsTableProfileTable = ImRaii.Table("###CharactersDetailsTable#ProfileTable", 2);
        if (!charactersDetailsTableProfileTable) return;
        ImGui.TableSetupColumn("###CharactersDetailsTable#ProfileTable#ProfileCol", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("###CharactersDetailsTable#ProfileTable#GearCol", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        using var tabBar = ImRaii.TabBar($"###CharactersDetailsTable#ProfileTable#ProfileCol#ProfileTabBar");
        if (!tabBar.Success) return;
        /*if (ImGui.BeginTabItem($"{_globalCache.AddonStorage.LoadAddonString(currentLocale,758)}"))
        {
            //DrawAttributes(current_character);
            ImGui.TextUnformatted("");
            ImGui.EndTabItem();
        }*/
        using var profileTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 759)}");
        if (profileTab.Success)
        {
            DrawProfile(selectedCharacter);
        }

        ImGui.TableSetColumnIndex(1);
        Utils.DrawGear(_currentLocale, ref _globalCache, ref _characterTextures, selectedCharacter.Gear,
            selectedCharacter.LastJob, selectedCharacter.LastJobLevel, 300, 350);
    }

    private void DrawAttributes(Character selectedCharacter)
    {
        if (selectedCharacter.Attributes is null) return;
        using var attributesHealthManaBarsTable = ImRaii.Table("###Attributes#HealthManaBarsTable", 2);
        if (!attributesHealthManaBarsTable) return;
        ImGui.TableSetupColumn("###HealthManaBarsTable#HPHeader", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("###HealthManaBarsTable#MPHeader", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"HP      {selectedCharacter.Attributes.Hp}");
        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"MP      {selectedCharacter.Attributes.Mp}");

        using var attributesStrDexVitIntMindTable = ImRaii.Table("###Attributes#StrDexVitIntMindTable", 2);
        if (!attributesStrDexVitIntMindTable) return;
        ImGui.TableSetupColumn("###Attributes#StrDexVitIntMindTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("###Attributes#StrDexVitIntMindTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableNextRow();
        ImGui.TextUnformatted("Attributes");
        ImGui.Separator();
        ImGui.TableSetColumnIndex(0);
        using (var attributesStrDexVitTable = ImRaii.Table("###Attributes#StrDexVitTable", 2))
        {
            if (!attributesStrDexVitTable) return;
            ImGui.TableSetupColumn("###Attributes#StrDexVitTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###Attributes#StrDexVitTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Strength");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{selectedCharacter.Attributes.Strength}");
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Dexterity");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{selectedCharacter.Attributes.Dexterity}");
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Vitality");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{selectedCharacter.Attributes.Vitality}");
        }

        ImGui.TableSetColumnIndex(1);
        using (var attributesIntMindTable = ImRaii.Table("###Attributes#IntMindTable", 2))
        {
            if (!attributesIntMindTable) return;
            ImGui.TableSetupColumn("###Attributes#IntMindTable#LabelHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###Attributes#IntMindTable#ValueHeader", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Intelligence");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{selectedCharacter.Attributes.Intelligence}");
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Mind");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{selectedCharacter.Attributes.Mind}");
        }
    }

    private void DrawProfile(Character selectedCharacter)
    {
        if (selectedCharacter.Profile is null) return;
        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 790)}"); //Title
        ImGui.Separator();
        //Plugin.Log.Debug($"Title: {current_character.Profile.Title}, isprefix: {current_character.Profile.TitleIsPrefix}");
        if (!string.IsNullOrEmpty(selectedCharacter.Profile.Title))
        {
            switch (selectedCharacter.Profile.TitleIsPrefix)
            {
                case true:
                    {
                        ImGui.TextUnformatted($"{selectedCharacter.Profile.Title}");
                        if (!string.IsNullOrEmpty(selectedCharacter.FirstName) &&
                            !string.IsNullOrEmpty(selectedCharacter.LastName))
                        {
                            ImGui.TextUnformatted(
                                $"            {selectedCharacter.FirstName} {selectedCharacter.LastName}");
                        }

                        break;
                    }
                case false:
                    {
                        if (!string.IsNullOrEmpty(selectedCharacter.FirstName) &&
                            !string.IsNullOrEmpty(selectedCharacter.LastName))
                        {
                            ImGui.TextUnformatted($"{selectedCharacter.FirstName} {selectedCharacter.LastName}");
                            ImGui.TextUnformatted($"            {selectedCharacter.Profile.Title}");
                        }

                        break;
                    }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(selectedCharacter.FirstName) &&
                !string.IsNullOrEmpty(selectedCharacter.LastName))
            {
                ImGui.TextUnformatted($"{selectedCharacter.FirstName} {selectedCharacter.LastName}");
            }
        }

        if (selectedCharacter.Profile.Grand_Company is not 0)
        {
            ImGui.TextUnformatted("");
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 791)}"); //Grand Company
            ImGui.Separator();
            using var profileTableGrandCompanyTable = ImRaii.Table("###ProfileTable#GrandCompanyTable", 2);
            if (!profileTableGrandCompanyTable) return;
            ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#Rank", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            //Utils.DrawIcon(new Vector2(40, 40), false, Utils.GetGrandCompanyIcon(selected_character.Profile.Grand_Company));
            Utils.DrawIcon_test(
                _globalCache.IconStorage.LoadIcon(Utils.GetGrandCompanyIcon(selectedCharacter.Profile.Grand_Company)),
                new Vector2(40, 40));

            ImGui.TableSetColumnIndex(1);
            using var profileTableGrandCompanyTableRankTable =
                ImRaii.Table("###ProfileTable#GrandCompanyTable#RankTable", 2);
            if (!profileTableGrandCompanyTableRankTable) return;
            ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#RankTable#Name",
                ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn("###ProfileTable#GrandCompanyTable#RankTable#Icon",
                ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{Utils.GetGrandCompany(_currentLocale, selectedCharacter.Profile.Grand_Company)}");
            ImGui.TextUnformatted(
                $"{Utils.Capitalize(Utils.GetGrandCompanyRank(_currentLocale, selectedCharacter.Profile.Grand_Company, selectedCharacter.Profile.Grand_Company_Rank, selectedCharacter.Profile.Gender))}");
            ImGui.TableSetColumnIndex(1);
            //Utils.DrawIcon(new Vector2(48, 48), false, Utils.GetGrandCompanyRankIcon(selected_character.Profile.Grand_Company, selected_character.Profile.Grand_Company_Rank));
            Utils.DrawIcon_test(
                _globalCache.IconStorage.LoadIcon(Utils.GetGrandCompanyRankIcon(
                    selectedCharacter.Profile.Grand_Company, selectedCharacter.Profile.Grand_Company_Rank)),
                new Vector2(48, 48));
        }

        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 793)}"); //Race/Clan/Gender
        ImGui.Separator();
        ImGui.TextUnformatted(
            $"{Utils.GetRace(_currentLocale, selectedCharacter.Profile.Gender, selectedCharacter.Profile.Race)} / {Utils.GetTribe(_currentLocale, selectedCharacter.Profile.Gender, selectedCharacter.Profile.Tribe)} / {Utils.GetGender(selectedCharacter.Profile.Gender)}");
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 794)}"); //City-state
        ImGui.Separator();
        using (var profileTableProfileColumnProfileTable =
               ImRaii.Table("###ProfileTable#ProfileColumn#ProfileTable", 2))
        {
            if (!profileTableProfileColumnProfileTable) return;
            ImGui.TableSetupColumn("###ProfileTable#ProfileColumn#ProfileTable#CityIcon",
                ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("###ProfileTable#ProfileColumn#ProfileTable#CityName",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            //Utils.DrawIcon(new Vector2(36, 36), false, Utils.GetTownIcon(selected_character.Profile.City_State));
            Utils.DrawIcon_test(
                _globalCache.IconStorage.LoadIcon(Utils.GetTownIcon(selectedCharacter.Profile.City_State)),
                new Vector2(36, 36));
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetTown(_currentLocale, selectedCharacter.Profile.City_State)}");
        }

        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 795)}"); //Nameday
        ImGui.Separator();
        ImGui.TextUnformatted(
            $"{Utils.GetNameday(selectedCharacter.Profile.Nameday_Day, selectedCharacter.Profile.Nameday_Month)}");
        ImGui.TextUnformatted("");
        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 796)}"); //Guardian
        ImGui.Separator();
        using var profileTableGuardianTable = ImRaii.Table("###ProfileTable#GuardianTable", 2);
        if (!profileTableGuardianTable) return;
        ImGui.TableSetupColumn("###ProfileTable#GuardianTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
        ImGui.TableSetupColumn("###ProfileTable#GuardianTable#Name", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        //Utils.DrawIcon(new Vector2(40, 40), false, Utils.GetGuardianIcon(selected_character.Profile.Guardian));
        Utils.DrawIcon_test(
            _globalCache.IconStorage.LoadIcon(Utils.GetGuardianIcon(selectedCharacter.Profile.Guardian)),
            new Vector2(36, 36));
        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"{Utils.GetGuardian(_currentLocale, selectedCharacter.Profile.Guardian)}");
    }
}
