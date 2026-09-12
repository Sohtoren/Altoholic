using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altoholic.Windows
{
    public class TodoWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;

        public TodoWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache)
            : base(name, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(40, 40),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;
        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; init; }
        private Character? _currentCharacter;

        public override void OnClose()
        {
        }

        public void Dispose()
        {
        }

        public override void Draw()
        {
            if (GetPlayer?.Invoke() == null) return;
            if (GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;

            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());
            using var charactersJobsTable = ImRaii.Table("###CharactersJobsTable", 2);
            if (!charactersJobsTable) return;
            ImGui.TableSetupColumn("###CharactersJobsTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed,
                210);
            ImGui.TableSetupColumn("###CharactersJobsTable#Jobs", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listbox = ImRaii.ListBox("###CharactersJobsTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listbox)
                {
                    if (chars.Count > 0)
                    {
                        if (ImGui.Selectable(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersJobsTable#CharactersListBox#All",
                                _currentCharacter == null))
                        {
                            _currentCharacter = null;
                        }

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

                        foreach (var currChar in chars.Where(currChar =>
                                     ImGui.Selectable(
                                         $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                         currChar == _currentCharacter)))
                        {
                            _currentCharacter = currChar;
                        }
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentCharacter is not null)
            {
                DrawTodos(_currentCharacter);
            }
            else
            {
                DrawAll(chars);
            }
        }

        private void DrawTodos(Character currentCharacter)
        {
            
        }

        private void DrawAll(List<Character> chars)
        {
            if (chars.Count == 0) return;
        }
    }
}
