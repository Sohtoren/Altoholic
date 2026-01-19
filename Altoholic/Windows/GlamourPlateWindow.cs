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
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Common.Lua;
using Lumina.Excel.Sheets;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Altoholic.Windows
{
    public class GlamourPlateWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public GlamourPlateWindow(
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

            //_glamourIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/MiragePrismPlate2.uld");
            _glamourIcons = Plugin.TextureProvider.GetFromGame("ui/uld/MiragePrismPlate2_hr1.tex").RentAsync().Result;
            /*_glamourTextures.Add(0, _characterIcons.LoadTexturePart("ui/uld/MiragePrismPlate2_hr1.tex", 4));
            _glamourTextures.Add(1, _characterIcons.LoadTexturePart("ui/uld/MiragePrismPlate2_hr1.tex", 5));*/
            (_glamourTexturesUv0, _glamourTexturesUv1) = Utils.GetTextureCoordinate(_glamourIcons.Size, 0, 52, 112, 52);
            (_glamourTexturesUv2, _glamourTexturesUv3) = Utils.GetTextureCoordinate(_glamourIcons.Size, 112, 52,112, 52);

            ResetCurrentTexture();

            _miragePrismBoxSetIcon = Plugin.TextureProvider.GetFromGame("ui/uld/MiragePrismBoxIcon_hr1.tex").RentAsync().Result;
            (_miragePrismBoxSetIconUv0, _miragePrismBoxSetIconUv1) = Utils.GetTextureCoordinate(_miragePrismBoxSetIcon.Size, 96, 96, 36, 36);
            (_miragePrismBoxSetIconUv2, _miragePrismBoxSetIconUv3) = Utils.GetTextureCoordinate(_miragePrismBoxSetIcon.Size, 96, 132, 36, 36);
        }

        private void ResetCurrentTexture()
        {
            for (int i = 0; i < 20; i++)
            {
                _currentTextures[i] = 0;
            }
        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; init; }
        private Character? _currentCharacter;
        private GlamourPlate? _currentGlamourPlate;
        private readonly UldWrapper _characterIcons;
        private Dictionary<GearSlot, IDalamudTextureWrap?> _characterTextures = [];
        //private readonly UldWrapper _glamourIcons;
        private readonly IDalamudTextureWrap _glamourIcons;
        //private Dictionary<int, IDalamudTextureWrap?> _glamourTextures = [];
        private readonly Vector2 _glamourTexturesUv0;
        private readonly Vector2 _glamourTexturesUv1;
        private readonly Vector2 _glamourTexturesUv2;
        private readonly Vector2 _glamourTexturesUv3;
        private int[] _currentTextures = new int[20];
        private readonly IDalamudTextureWrap _miragePrismBoxSetIcon;
        private readonly Vector2 _miragePrismBoxSetIconUv0;
        private readonly Vector2 _miragePrismBoxSetIconUv1;
        private readonly Vector2 _miragePrismBoxSetIconUv2;
        private readonly Vector2 _miragePrismBoxSetIconUv3;



        public void Clear()
        {
            Plugin.Log.Info("GlamourPlateWindow, Clear() called");
            _currentGlamourPlate = null;
            _currentCharacter = null;
            ResetCurrentTexture();
        }

        public void Dispose()
        {
            Plugin.Log.Info("GlamourPlateWindow, Dispose() called");
            _currentGlamourPlate = null;
            _currentCharacter = null;
            ResetCurrentTexture();
            foreach (KeyValuePair<GearSlot, IDalamudTextureWrap?> loadedTexture in _characterTextures)
                loadedTexture.Value?.Dispose();
            _characterIcons.Dispose();
            _glamourIcons.Dispose();
            _miragePrismBoxSetIcon.Dispose();
        }

        public override void Draw()
        {
            if (GetPlayer?.Invoke() == null) return;
            if (GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;

            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            using var characterDetailsTable = ImRaii.Table("###CharactersGlamourPlateTable", 2);
            if (!characterDetailsTable) return;
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#CharacterLists", ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Details", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox = ImRaii.ListBox("###CharactersGlamourPlateTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {
                    foreach (Character currChar in chars.Where(currChar =>
                                 ImGui.Selectable(
                                     $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                     currChar == _currentCharacter)))
                    {
                        _currentCharacter = currChar;
                        _currentGlamourPlate = null;
                        ResetCurrentTexture();
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentCharacter is not null)
            {
                DrawGlamourPlates(_currentCharacter);
            }
        }

        private void DrawGlamourPlates(Character selectedCharacter)
        {
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11955));
            using var charactersGlamourPlateTableProfileTable = ImRaii.Table("###CharactersGlamourPlateTable#ProfileTable", 1);
            if (!charactersGlamourPlateTableProfileTable) return;
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#ProfileTable#ProfileCol",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            if (selectedCharacter.GlamourPlates.Count > 0)
            {
                DrawGlamourPlateList(selectedCharacter);
            }
            else
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4148)}");
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (_currentGlamourPlate == null) return;
            Utils.DrawPlate(_currentLocale, ref _globalCache, ref _characterTextures, _currentGlamourPlate,
                300, 350, _miragePrismBoxSetIcon, _miragePrismBoxSetIconUv0, _miragePrismBoxSetIconUv1);
        }

        private void DrawGlamourPlateList(Character selectedCharacter)
        {
            using var charactersGlamourPlateTableGlamourPlateListTable =
                ImRaii.Table("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable", 10);
            if (!charactersGlamourPlateTableGlamourPlateListTable) return;
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#1",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#2",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#3",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#4",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#5",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#6",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#7",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#8",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#9",
                ImGuiTableColumnFlags.WidthFixed, 46);
            ImGui.TableSetupColumn("###CharactersGlamourPlateTable#Profile#GlamourPlateListTable#10",
                ImGuiTableColumnFlags.WidthFixed, 46);

            ImGui.TableNextRow();
            for (byte i = 0; i < 20; i++)
            {
                if (i == 10)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Vector2 p = ImGui.GetCursorPos();
                (Vector2, Vector2) texture = (_currentTextures[i] == 0)
                    ? (_glamourTexturesUv0, _glamourTexturesUv1)
                    : (_glamourTexturesUv2, _glamourTexturesUv3);

                ImGui.Image(_glamourIcons.Handle, new Vector2(46, 26), texture.Item1, texture.Item2);
                if (ImGui.IsItemClicked())
                {
                    ResetCurrentTexture();
                    _currentTextures[i] = 1;
                    if (selectedCharacter.GlamourPlates.TryGetValue(i, out GlamourPlate? plate))
                    {
                        _currentGlamourPlate = plate;
                    }
                }

                int j = i + 1;
                Vector2 textPlacement = (j < 10) ? new Vector2(p.X + 20, p.Y + 4) : new Vector2(p.X + 17, p.Y + 4);
                ImGui.SetCursorPos(textPlacement);
                ImGui.PushStyleColor(ImGuiCol.Text, 0xFF000000);
                ImGui.TextUnformatted($"{j}");
                ImGui.PopStyleColor();
                ImGui.SetCursorPos(p);
            }
        }
    }
}
