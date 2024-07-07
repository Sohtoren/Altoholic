using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Altoholic.Windows
{
    public class ProgressWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public ProgressWindow(
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
            this._plugin = plugin;
            _globalCache = globalCache;

            //_rolesIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/Retainer.uld");
            //Plugin.Log.Debug($"Type: {rolesIcons.Uld?.FileInfo.Type}");
            //rolesTextures.Add(Role.Tank, rolesIcons.LoadTexturePart("ui/uld/fourth/ToggleButton_hr1.tex", 4));
            _rolesTextureWrap = Plugin.TextureProvider.GetFromGame("ui/uld/Retainer_hr1.tex").RentAsync().Result;
        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; set; }
        private Character? _currentCharacter = null;
        //private readonly UldWrapper _rolesIcons;
        /*private Dictionary<RoleIcon, IDalamudTextureWrap?> rolesTextures = [];*/
        private readonly IDalamudTextureWrap? _rolesTextureWrap = null;

        public override void OnClose()
        {
        }

        public void Dispose()
        {
        }

        public override void Draw()
        {
            if (_rolesTextureWrap is null) return;
            _currentLocale = _plugin.Configuration.Language;
            //Plugin.Log.Debug($"rolesIcons.Valid: {_rolesIcons.Valid}");
            /*if (!_rolesIcons.Valid) return;
            for (int i = 0; i < 30; i++)
            {
                ImGui.TextUnformatted($"{i}:");
                ImGui.SameLine();
                IDalamudTextureWrap? icon = _rolesIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", i);
                if (icon != null)
                {
                    ImGui.Image(icon.ImGuiHandle, new Vector2(40, 40));
                }
                else
                {
                    Plugin.Log.Debug($"{i} is null");
                }
            }*/

            (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_rolesTextureWrap.Size, 0, 150, 90, 90);
            ImGui.Image(_rolesTextureWrap.ImGuiHandle, new Vector2(40, 40), uv0, uv1);
            /*var icon = rolesTextures[Role.Tank];
            if (icon == null)
            {
                Plugin.Log.Debug("icon is null");
            }
            else
            {
                Plugin.Log.Debug("icon is not null");
                    ImGui.Image(icon.ImGuiHandle, new Vector2(64,64));
            }*/
        }
    }
}