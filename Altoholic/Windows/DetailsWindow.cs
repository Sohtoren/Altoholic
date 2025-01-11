using System;
using System.Collections.Generic;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Altoholic.Windows
{
    public class DetailsWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;

        private readonly SqliteConnection _db;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public DetailsWindow(
            Plugin plugin,
            string name,
            SqliteConnection db,
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
            _db = db;
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
            _characterTextures.Add(GearSlot.FACEWEAR, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 55));
            _characterTextures.Add(GearSlot.EMPTY, Plugin.TextureProvider.GetFromGame("ui/uld/fourth/DragTargetA_hr1.tex").RentAsync().Result);
        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; set; }
        private Character? _currentCharacter;
        private readonly UldWrapper _characterIcons;
        private Dictionary<GearSlot, IDalamudTextureWrap?> _characterTextures = [];

        /*public override void OnClose()
        {
            Plugin.Log.Debug("DetailsWindow, OnClose() called");
            _currentCharacter = null;
        }*/

        public void Clear()
        {
            Plugin.Log.Info("DetailsWindow, Clear() called");
            _currentCharacter = null;
        }

        public void Dispose()
        {
            Plugin.Log.Info("DetailsWindow, Dispose() called");
            _currentCharacter = null;
            foreach(KeyValuePair<GearSlot, IDalamudTextureWrap?> loadedTexture in _characterTextures) loadedTexture.Value?.Dispose();
            _characterIcons.Dispose();
        }

        public override void Draw()
        {
            if(GetPlayer?.Invoke() == null) return;
            if(GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;
            //Plugin.Log.Debug($"DrawDetails character with c : id = {character.CharacterId}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}, Quests = {character.Quests.Count}, Inventory = {character.Inventory.Count}, Gear {character.Gear.Count}, Retainers = {character.Retainers.Count}");
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
            using var tabBar = ImRaii.TabBar("###CharactersDetailsTable#ProfileTable#ProfileCol#ProfileTabBar");
            if (!tabBar.Success) return;
            /*if (ImGui.BeginTabItem($"{_globalCache.AddonStorage.LoadAddonString(currentLocale,758)}"))
            {
                //DrawAttributes(current_character);
                ImGui.TextUnformatted("");
                ImGui.EndTabItem();
            }*/
            using (var profileTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 759)}###{selectedCharacter.CharacterId}#Profile"))
            {
                if (profileTab.Success)
                {
                    DrawProfile(selectedCharacter);
                }
            }

            if (selectedCharacter.Houses.Count > 0)
            {
                using var housingTab =
                    ImRaii.TabItem(
                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1999)}###{selectedCharacter.CharacterId}#Housing");
                if (housingTab.Success)
                {
                    DrawHousing(selectedCharacter);
                }
            }

            ImGui.TableSetColumnIndex(1);
            Utils.DrawGear(_currentLocale, ref _globalCache, ref _characterTextures, selectedCharacter.Gear,
                selectedCharacter.LastJob, selectedCharacter.LastJobLevel, 300, 350, false, 0, selectedCharacter.CurrentFacewear);
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

            if (selectedCharacter.Profile.GrandCompany is not 0)
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
                Utils.DrawIcon(
                    _globalCache.IconStorage.LoadIcon(Utils.GetGrandCompanyIcon(selectedCharacter.Profile.GrandCompany)),
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
                ImGui.TextUnformatted($"{Utils.GetGrandCompany(_currentLocale, selectedCharacter.Profile.GrandCompany)}");
                ImGui.TextUnformatted(
                    $"{Utils.Capitalize(Utils.GetGrandCompanyRank(_currentLocale, selectedCharacter.Profile.GrandCompany, selectedCharacter.Profile.GrandCompanyRank, selectedCharacter.Profile.Gender))}");
                ImGui.TableSetColumnIndex(1);
                Utils.DrawIcon(
                    _globalCache.IconStorage.LoadIcon(Utils.GetGrandCompanyRankIcon(
                        selectedCharacter.Profile.GrandCompany, selectedCharacter.Profile.GrandCompanyRank)),
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
                Utils.DrawIcon(
                    _globalCache.IconStorage.LoadIcon(Utils.GetTownIcon(selectedCharacter.Profile.CityState)),
                    new Vector2(36, 36));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetTown(_currentLocale, selectedCharacter.Profile.CityState)}");
            }

            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 795)}"); //Nameday
            ImGui.Separator();
            ImGui.TextUnformatted(
                $"{Utils.GetNameday(_currentLocale, selectedCharacter.Profile.NamedayDay, selectedCharacter.Profile.NamedayMonth)}");
            ImGui.TextUnformatted("");
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 796)}"); //Guardian
            ImGui.Separator();
            using var profileTableGuardianTable = ImRaii.Table("###ProfileTable#GuardianTable", 2);
            if (!profileTableGuardianTable) return;
            ImGui.TableSetupColumn("###ProfileTable#GuardianTable#Icon", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("###ProfileTable#GuardianTable#Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(
                _globalCache.IconStorage.LoadIcon(Utils.GetGuardianIcon(selectedCharacter.Profile.Guardian)),
                new Vector2(36, 36));
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetGuardian(_currentLocale, selectedCharacter.Profile.Guardian)}");
        }

        private void DrawHousing(Character selectedCharacter)
        {
            using var charactersDetailsTableProfileTable = ImRaii.Table("###CharactersDetailsTable#Profile#HousingTable", 3);
            if (!charactersDetailsTableProfileTable) return;
            ImGui.TableSetupColumn("###CharactersDetailsTable#Profile#HousingTable#Icon", ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###CharactersDetailsTable#Profile#HousingTable#Text", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###CharactersDetailsTable#Profile#HousingTable#Delete", ImGuiTableColumnFlags.WidthFixed, 40);
            foreach (Housing house in selectedCharacter.Houses.ToList())
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                IDalamudTextureWrap icon = (house.Plot == -127 || house.Plot == -128 || house.Room != 0)
                    ? _globalCache.IconStorage.LoadHighResIcon(066403)
                    : _globalCache.IconStorage.LoadHighResIcon(066458);
                Utils.DrawIcon(icon, new Vector2(48,48));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(
                    //$"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8495)}: {GetHouseTerritoryString(house.TerritoryId)}" +
                    $"{GetHouseTerritoryString(house.TerritoryId, house.Division)}");
                ImGui.TextUnformatted(
                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 6369)} {_globalCache.AddonStorage.LoadAddonString(_currentLocale, 6351)}: {house.Ward + 1}");
                if (house.Plot != -127 && house.Plot != -128)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted($"- {_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14312)}: {house.Plot+1}");
                }
                if (house.Room != 0)
                {
                    ImGui.SameLine();
                    string room = _currentLocale switch
                    {
                        ClientLanguage.German => "Zimmer",
                        ClientLanguage.English => "Room",
                        ClientLanguage.French => "Chambre",
                        ClientLanguage.Japanese => "Room",
                        _ => "Room"
                    };
                    ImGui.TextUnformatted($"- {room} {_globalCache.AddonStorage.LoadAddonString(_currentLocale, 6454)} {house.Room}");
                }
                ImGui.Separator();

                ImGui.TableSetColumnIndex(2);
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Ban.ToIconString());
                if (ImGui.IsItemClicked())
                {
                    ImGui.OpenPopup(
                        $"Remove###DHModal_{house.Id}");
                    Plugin.Log.Debug(
                        $"Remove {_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14312)}: {house.Plot + 1} hit");
                }
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"Remove");
                    ImGui.EndTooltip();
                }

                using ImRaii.IEndObject removeHousing = ImRaii.PopupModal($"###DHModal_{house.Id}");
                if (!removeHousing)
                {
                    continue;
                }

                ImGui.TextUnformatted("Are you sure you want to remove this housing?");
                ImGui.TextUnformatted(
                    "This will not prevent this housing to be added in the future if you own it");
                ImGui.Separator();

                if (ImGui.Button("OK", new Vector2(120, 0)))
                {
                    bool r = selectedCharacter.Houses.Remove(house);
                    int result = Database.Database.UpdateCharacter(_db, selectedCharacter);
                    //SetBlacklistedCharacter(character.CharacterId);
                    //this.SetOthersCharactersList(oC);
                    Utils.ChatMessage("Housing removed");
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SetItemDefaultFocus();
                ImGui.SameLine();
                if (ImGui.Button("Cancel", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); }
            }
        }

        private string GetHouseTerritoryString(uint territoryId, int division)
        {
            return territoryId switch
            {
                282 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1100), //"Private Cottage - Mist",
                283 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1101), //"Private House - Mist",
                284 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1102), //"Private Mansion - Mist",
                384 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1157), //"Private Chambers - Mist",
                608 => (division == 2)
                    ? _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1806)
                    : _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1812), //"Topmast Apartment",
                342 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1106), //"Private Cottage - The Lavender Beds",
                343 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1107), //"Private House - The Lavender Beds",
                344 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1108), //"Private Mansion - The Lavender Beds",
                385 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1159), //"Private Chambers - The Lavender Beds",
                609 => (division == 2)
                    ? _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1808)
                    : _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1814), //"Lily Hills Apartment",
                345 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1103), //"Private Cottage - The Goblet",
                346 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1104), //"Private House - The Goblet",
                347 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1105), //"Private Mansion - The Goblet",
                386 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1158), //"Private Chambers - The Goblet",
                610 => (division == 2)
                    ? _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1191)
                    : _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                        1816), //"Sultana's Breath Apartment",
                649 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1893), //"Private Cottage - Shirogane",
                650 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1894), //"Private House - Shirogane",
                651 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1895), //"Private Mansion - Shirogane",
                652 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    2270), //"Private Chambers - Shirogane",
                655 => (division == 2)
                    ? _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 1921)
                    : _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 2273), //"Kobai Goten Apartment",
                980 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    3689), //"Private Cottage - Empyreum",
                981 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    3690), //"Private House - Empyreum",
                982 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    3694), //"Private Mansion - Empyreum",
                983 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    3692), //"Private Chambers - Empyreum",
                999 => (division == 2)
                    ? _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 3726)
                    : _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale, 3695), //"Ingleside Apartment",
                423 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1227), //"Company Workshop - Mist",
                424 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1228), //"Company Workshop - The Goblet",
                425 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    1229), //"Company Workshop - The Lavender Beds",
                653 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    2271), //"Company Workshop - Shirogane",
                984 => _globalCache.PlaceNameStorage.GetPlaceNameOnly(_currentLocale,
                    3693), //"Company Workshop - Empyreum",
                _ => string.Empty
            };
        }
    }
}
