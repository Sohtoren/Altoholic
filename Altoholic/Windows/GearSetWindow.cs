using System;
using System.Collections.Generic;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Altoholic.Windows
{
    public class GearSetWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public GearSetWindow(
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
                MinimumSize = new Vector2(1000, 450), MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
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
            _characterTextures.Add(GearSlot.RIGHT_RING,
                _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 28));
            _characterTextures.Add(GearSlot.SOUL_CRYSTAL,
                _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 29));
            _characterTextures.Add(GearSlot.FACEWEAR, _characterIcons.LoadTexturePart("ui/uld/Character_hr1.tex", 55));
            _characterTextures.Add(GearSlot.EMPTY,
                Plugin.TextureProvider.GetFromGame("ui/uld/img03/DragTargetA_hr1.tex").RentAsync().Result);

            _glamourSetIcon = Plugin.TextureProvider.GetFromGame("ui/uld/img03/GearSetList_hr1.tex").RentAsync().Result;
            (_glamourSetIconUv0, _glamourSetIconUv1) = Utils.GetTextureCoordinate(_glamourSetIcon.Size, 280, 0, 48, 48);

        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; init; }
        private Character? _currentCharacter;
        private GearSet? _currentGearSet;
        private readonly UldWrapper _characterIcons;
        private Dictionary<GearSlot, IDalamudTextureWrap?> _characterTextures = [];
        private readonly IDalamudTextureWrap _glamourSetIcon;
        private readonly Vector2 _glamourSetIconUv0;
        private readonly Vector2 _glamourSetIconUv1;

        public void Clear()
        {
            Plugin.Log.Info("GearSetWindow, Clear() called");
            _currentGearSet = null;
            _currentCharacter = null;
        }

        public void Dispose()
        {
            Plugin.Log.Info("GearSetWindow, Dispose() called");
            _currentGearSet = null;
            _currentCharacter = null;
            foreach (KeyValuePair<GearSlot, IDalamudTextureWrap?> loadedTexture in _characterTextures)
                loadedTexture.Value?.Dispose();
            _characterIcons.Dispose();
        }

        public override void Draw()
        {
            if (GetPlayer?.Invoke() == null) return;
            if (GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;

            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            using var characterDetailsTable = ImRaii.Table("###CharactersGearSetTable", 2);
            if (!characterDetailsTable) return;
            ImGui.TableSetupColumn("###CharactersGearSetTable#CharacterLists", ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn("###CharactersGearSetTable#Details", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox = ImRaii.ListBox("###CharactersGearSetTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {
                    foreach (Character currChar in chars.Where(currChar =>
                                 ImGui.Selectable(
                                     $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                     currChar == _currentCharacter)))
                    {
                        _currentCharacter = currChar;
                        _currentGearSet = null;
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentCharacter is not null)
            {
                DrawGearSet(_currentCharacter);
            }
        }

        private void DrawGearSet(Character selectedCharacter)
        {
            using var charactersGearSetTableProfileTable = ImRaii.Table("###CharactersGearSetTable#ProfileTable", 2);
            if (!charactersGearSetTableProfileTable) return;
            ImGui.TableSetupColumn("###CharactersGearSetTable#ProfileTable#ProfileCol",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###CharactersGearSetTable#ProfileTable#GearCol",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            if (selectedCharacter.GearSets.Count > 0)
            {
                DrawGearSetList(selectedCharacter);
            }
            else
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4148)}");
            }

            ImGui.TableSetColumnIndex(1);
            if(_currentGearSet == null) return;
            Utils.DrawGear(_currentLocale, ref _globalCache, ref _characterTextures, _currentGearSet.Gears,
                _currentGearSet.ClassJob, 0, 300, 350, false, 0,
                _currentGearSet.GlassesIds, true, _currentGearSet.ItemLevel);
        }

        private void DrawGearSetList(Character selectedCharacter)
        {
            using var charactersGearSetTableGearSetListTable =
                ImRaii.Table("###CharactersGearSetTable#Profile#GearSetListTable", 6, ImGuiTableFlags.ScrollY);
            if (!charactersGearSetTableGearSetListTable) return;
            ImGui.TableSetupColumn("###CharactersGearSetTable#Profile#GearSetListTable#Index",
                ImGuiTableColumnFlags.WidthFixed, 10);
            ImGui.TableSetupColumn("###CharactersGearSetTable#Profile#GearSetListTable#ClassJobIcon",
                ImGuiTableColumnFlags.WidthFixed, 42);
            ImGui.TableSetupColumn("###CharactersGearSetTable#Profile#GearSetListTable#Name",
                ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("###CharactersGearSetTable#Profile#GearSetListTable#Ilvl",
                ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn("###CharactersGearSetTable#Profile#GearSetListTable#Open",
                ImGuiTableColumnFlags.WidthFixed, 30);
            ImGui.TableSetupColumn("###CharactersGearSetTable#Profile#GearSetListTable#Open",
                ImGuiTableColumnFlags.WidthFixed, 100);

            for (int i = 0; i < selectedCharacter.GearSets.Count; i++)
            {
                if (!selectedCharacter.GearSets.TryGetValue(i, out GearSet? gs))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(gs.Name)) continue;
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{i + 1}");
                ImGui.TableSetColumnIndex(1);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(Utils.GetJobIcon(gs.ClassJob)),
                    new Vector2(40, 40));
                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted($"{gs.Name}");
                ImGui.TableSetColumnIndex(3);
                ImGui.TextUnformatted($"{(char)SeIconChar.ItemLevel} {gs.ItemLevel}");
                ImGui.TableSetColumnIndex(4);
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted($"{FontAwesomeIcon.Search.ToIconString()}");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4360) }");
                    ImGui.EndTooltip();
                }
                if (ImGui.IsItemClicked())
                {
                    _currentGearSet = gs;
                }
                ImGui.TableSetColumnIndex(5);
                if (gs.GlamourSetLink == 0)
                {
                    continue;
                }

                Vector2 p = ImGui.GetCursorPos();
                ImGui.Image(_glamourSetIcon.Handle, new Vector2(23, 23), _glamourSetIconUv0, _glamourSetIconUv1);
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted($"{Loc.Localize("GlamourSetGlamourPlateLinked1", "Glamour plate ")}{gs.GlamourSetLink}{Loc.Localize("GlamourSetGlamourPlateLinked2", " is linked to this gearset")}");
                    ImGui.EndTooltip();
                }
                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 15));
                ImGui.TextUnformatted($"{gs.GlamourSetLink}");
                ImGui.SetCursorPos(p);
            }
        }
    }
}
