using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class InventoriesWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public InventoriesWindow(
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

            _armouryBoard = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/ArmouryBoard.uld");
            _armoryTabTextures.Add(InventoryType.ArmoryMainHand, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 0));
            _armoryTabTextures.Add(InventoryType.ArmoryHead, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 1));
            _armoryTabTextures.Add(InventoryType.ArmoryBody, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 2));
            _armoryTabTextures.Add(InventoryType.ArmoryHands, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 3));
            _armoryTabTextures.Add(InventoryType.ArmoryLegs, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 5));
            _armoryTabTextures.Add(InventoryType.ArmoryFeets, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 6));
            _armoryTabTextures.Add(InventoryType.ArmoryOffHand, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 7));
            _armoryTabTextures.Add(InventoryType.ArmoryEar, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 8));
            _armoryTabTextures.Add(InventoryType.ArmoryNeck, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 9));
            _armoryTabTextures.Add(InventoryType.ArmoryWrist, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 10));
            _armoryTabTextures.Add(InventoryType.ArmoryRings, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 11));
            _armoryTabTextures.Add(InventoryType.ArmorySoulCrystal, _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 12));
        }

        public Func<Character> GetPlayer { get; set; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;
        private IEnumerable<Item>? _currentItems;
        private uint? _currentItem;
        private string _searchedItem = string.Empty;
        private string _lastSearchedItem = string.Empty;
        private Dictionary<string, List<Gear>>? _selectedArmory;

        private readonly UldWrapper _armouryBoard;
        private readonly Dictionary<InventoryType, IDalamudTextureWrap?> _armoryTabTextures = [];
        private InventoryType? _selectedTab;

        /*public override void OnClose()
        {
            Plugin.Log.Debug("InventoryWindow, OnClose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _selectedArmory = null;
            _selectedTab = null;
        }*/

        public void Dispose()
        {
            Plugin.Log.Info("InventoriesWindow, Dispose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _selectedArmory = null;
            _selectedTab = null;

            foreach (var loadedTexture in _armoryTabTextures) loadedTexture.Value?.Dispose();
            _armouryBoard.Dispose();
        }

        public void Clear()
        {
            Plugin.Log.Info("InventoriesWindow, Clear() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _selectedArmory = null;
            _selectedTab = null;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using var table = ImRaii.Table("###CharactersInventoryTable", 2);
                if (!table) return;

                ImGui.TableSetupColumn("###CharactersInventoryTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (var listBox = ImRaii.ListBox("###CharactersInventoryTable#CharactersListBox", new Vector2(200, -1)))
                {
                    if (listBox)
                    {
                        ImGui.SetScrollY(0);
                        if (chars.Count > 0)
                        {
                            if (ImGui.Selectable($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersInventoryTable#CharactersListBox#All", _currentCharacter == null))
                            {
                                _currentCharacter = null;
                            }

                            foreach (Character currChar in chars.Where(currChar => ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == _currentCharacter)))
                            {
                                _currentCharacter = currChar;
                                _currentItem = null;
                                _currentItems = null;
                                _searchedItem = string.Empty;
                                _lastSearchedItem = string.Empty;
                            }
                        }
                    }
                }
                ImGui.TableSetColumnIndex(1);
                if (_currentCharacter is not null)
                {
                    DrawInventories(_currentCharacter);
                }
                else
                {
                    DrawAll(chars);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic CharactersInventoryTable Exception : {0}", e);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            if (ImGui.InputText(Utils.Capitalize(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3635)), ref _searchedItem, 512,
                    ImGuiInputTextFlags.EnterReturnsTrue))
            {
                switch (_searchedItem.Length)
                {
                    case >= 3 when _searchedItem is "Gil" or "MGP" or "MGF" || _lastSearchedItem == _searchedItem:
                        return;
                    case >= 3:
                        {
                            IEnumerable<Item>? items = Utils.GetItemsFromName(_currentLocale, _searchedItem);
                            if (items != null)
                            {
                                List<Item> currentItems = items.ToList();
                                if (currentItems.Count > 0)
                                {
                                    _currentItems = currentItems;
                                }
                            }

                            _lastSearchedItem = _searchedItem;
                            break;
                        }
                    case 0:
                        _currentItems = null;
                        break;
                }
            }

            if (_currentItems == null)
            {
                return;
            }

            List<Item> currentItemsList = _currentItems.ToList();

            using var searchItemsTable =
                ImRaii.Table("###CharactersInventory#All#SearchItemsTable", 2, ImGuiTableFlags.Borders);
            if (!searchItemsTable) return;
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#Item", ImGuiTableColumnFlags.WidthFixed,
                300);
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems",
                ImGuiTableColumnFlags.WidthFixed, 350);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var itemlistbox = ImRaii.ListBox("###CharactersInventory#All#SearchItemsTable#Item#Listbox",
                       new Vector2(300, -1)))
            {
                if (itemlistbox)
                {
                    ImGui.SetScrollY(0);
                    foreach (Item item in currentItemsList.Where(
                                 item => ImGui.Selectable(item.Name, item.RowId == _currentItem)))
                    {
                        _currentItem = item.RowId;
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentItem is null)
            {
                return;
            }

            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)_currentItem);
            if (itm == null) return;
            using (var itemIconTable =
                   ImRaii.Table("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName", 2,
                       ImGuiTableFlags.NoBordersInBody))
            {
                if (!itemIconTable) return;
                ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 50);
                ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName#Name",
                    ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon), new Vector2(36, 36));

                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{itm.Name}");
            }

            List<Character> characters = chars.FindAll(c => c.Inventory.FindAll(ci => ci.ItemId == _currentItem).Count > 0);
            if (characters.Count == 0)
            {
                ImGui.TextUnformatted($"{Loc.Localize("NoItemOnAnyCharacter", "Item not found on any characters.\r\nCheck if inventories are available and updated.")}");
                return;
            }
            using var table = ImRaii.Table("###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table", 2,
                ImGuiTableFlags.Borders);
            if (!table) return;
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table#CharacterName",
                ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table#CharacterItem",
                ImGuiTableColumnFlags.WidthFixed, 50);
            foreach (Character character in characters)
            {
                uint totalAmount = character.Inventory.FindAll(i => i.ItemId == _currentItem).Aggregate<Inventory, uint>(0, (current, inv) => current + inv.Quantity);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(
                    $"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{totalAmount}");
            }
        }

        private void DrawInventories(Character selectedCharacter)
        {
            using var tab = ImRaii.TabBar($"###CharactersInventoryTable#Inventories#{selectedCharacter.Id}#TabBar");
            if (!tab) return;
            using (var inventoriesTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}###CharactersInventoryTable#Inventories#{selectedCharacter.Id}#TabBar#MainInventoriesTab"))
            {
                if (inventoriesTab)
                {
                    DrawMainInventories(selectedCharacter);
                }
            };
            using (var armoryTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1370)}###CharactersInventoryTable#Inventories#{selectedCharacter.Id}#TabBar#ArmoryInventoryTab"))
            {
                if (armoryTab)
                {
                    using var table =
                        ImRaii.Table(
                            $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.Id}", 3);
                    if (!table) return;
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.Id}#Col1",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.Id}#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 600);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.Id}#Col3",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TableSetColumnIndex(1);
                    DrawArmory(_globalCache, selectedCharacter);
                    ImGui.TableSetColumnIndex(2);
                }
            };

            using (var glamourTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3735)}###CharactersInventoryTable#Inventories#{selectedCharacter.Id}#TabBar#GlamourInventoryTab"))
            {
                if (glamourTab)
                {
                    // Todo: implement
                }
            };

            using (var armoireTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3734)}###CharactersInventoryTable#Inventories#{selectedCharacter.Id}#TabBar#ArmoireInventoryTab"))
            {
                if (armoireTab)
                {
                    // Todo: implement
                }
            };
        }

        private void DrawMainInventories(Character selectedCharacter)
        {
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#{selectedCharacter.Id}", 2, ImGuiTableFlags.ScrollY);
            if (!table) return;

            if (selectedCharacter.Inventory.Count != 278) return;
            List<Inventory> inventory = [.. selectedCharacter.Inventory[..140].OrderByDescending(i => i.Quantity)];//Todo: Use ItemOrderModule
            List<Inventory> keysitems = [.. selectedCharacter.Inventory.Slice(141, 105).OrderByDescending(k => k.Quantity)];
            bool isKeyItemEmpty = (keysitems.FindAll(k => k.Quantity == 0).Count == 105);
            //List<Inventory> crystals = selected_character.Inventory.Slice(246, 32);
            List<Inventory> saddleBag = [.. selectedCharacter.Saddle.OrderByDescending(s => s.Quantity)];

            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Bags_{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthFixed, 450);
            if (isKeyItemEmpty)
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#KeyItems_{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthFixed, 180);
            else
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#KeyItems_{selectedCharacter.Id}", ImGuiTableColumnFlags.WidthStretch);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}");
            DrawInventory($"Inventory_{selectedCharacter.Id}", inventory);
            using (var inventoryAmountTable =
                   ImRaii.Table(
                       $"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selectedCharacter.Id}#Amount", 2))
            {
                if (inventoryAmountTable)
                {
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selectedCharacter.Id}#Amount#Col1",
                        ImGuiTableColumnFlags.WidthFixed, 390);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selectedCharacter.Id}#Amount#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 80);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/140");
                }
            }

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 536)}");
            //Plugin.Log.Debug($"{selected_character.FirstName} KeyItems: {keysitems.FindAll(k => k.Quantity == 0).Count}");
            if (isKeyItemEmpty)
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1959)}");
            }
            else
            {
                DrawKeyInventory($"Keyitem_{selectedCharacter.Id}", keysitems);
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2882)}");
            DrawCrystals(selectedCharacter);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12212)}");
            //Plugin.Log.Debug($"{selected_character.FirstName} Saddle: {selected_character.Saddle.FindAll(k => k.Quantity == 0).Count}");
            int saddleCount = selectedCharacter.Saddle.FindAll(s => s.Quantity == 0).Count;
            if (saddleCount is 0 or 140)
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5448)} {Loc.Localize("OpenSaddlebag", "The Saddlebag may be empty or open to update")}");
            }
            else
            {
                DrawInventory($"Saddle_{selectedCharacter.Id}", saddleBag, true, selectedCharacter.HasPremiumSaddlebag);
                using var saddleAmountTable = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selectedCharacter.Id}#Amount", 2);
                if (!saddleAmountTable)
                {
                    return;
                }

                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selectedCharacter.Id}#Amount#Col1", ImGuiTableColumnFlags.WidthFixed, 390);
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selectedCharacter.Id}#Amount#Col2", ImGuiTableColumnFlags.WidthFixed, 80);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{saddleBag.FindAll(i => i.ItemId != 0).Count}/{((selectedCharacter.HasPremiumSaddlebag) ? "140" : "70")}");
            }
        }

        private void DrawInventory(string label, List<Inventory> inventory, bool saddle = false, bool premium = false)
        {
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#{label}Table", 10, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner);
            if (!table) return;

            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col4", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col5", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col6", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col7", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col8", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col9", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col10", ImGuiTableColumnFlags.WidthFixed, 36);

            for (int i = 0; i < inventory.Count; i++)
            {
                Inventory item = inventory[i];
                if (saddle && !premium && i > 69) continue;
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }
                ImGui.TableNextColumn();
                if (item.ItemId == 0)
                {
                    //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                    ImGui.Text("");
                    /*var list = ImGui.GetWindowDrawList();
                    list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                }
                else
                {
                    Vector2 p = ImGui.GetCursorPos();
                    Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, item.ItemId);
                    if (itm == null)
                    {
                        continue;
                    }

                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon, item.HQ), new Vector2(36, 36));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(_currentLocale, ref _globalCache, item);
                    }

                    if (itm.StackSize <= 1)
                    {
                        continue;
                    }

                    ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                    ImGui.TextUnformatted($"{item.Quantity}");
                    ImGui.SetCursorPos(p);

                }
            }
        }

        private void DrawKeyInventory(string label, List<Inventory> inventory)
        {
            int itemCount = inventory.FindAll(i => i.ItemId != 0).ToList().Count;
            int rows = (int)Math.Ceiling(itemCount / (double)7);
            int height = rows * 36;
            /*int height = inventory.FindAll(i => i.ItemId != 0).ToList().Count switch
            {
                int i when i is >= 0 and <= 7 => 36,
                int i when i is >= 8 and <= 14 => 72,
                int i when i is >= 15 and <= 21 => 108,
                int i when i is >= 22 and <= 28 => 144,
                int i when i is >= 29 and <= 35 => 180,
                int i when i is >= 36 and <= 42 => 216,
                int i when i is >= 43 and <= 49 => 252,
                int i when i is >= 44 and <= 56 => 288,
                int i when i is >= 57 and <= 63 => 234,
                int i when i is >= 64 and <= 70 => 360,
                int i when i is >= 71 and <= 77 => 396,
                int i when i is >= 78 and <= 84 => 432,
                int i when i is >= 85 and <= 91 => 468,
                int i when i is >= 92 and <= 98 => 504,
                >= 99 and <= 105 => 540,
                _ => -1
            };*/
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#{label}Table", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(310, height));
            if (!table) return;

            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col4", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col5", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col6", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col7", ImGuiTableColumnFlags.WidthFixed, 36);
            for (int i = 0; i < inventory.Count; i++)
            {
                Inventory item = inventory[i];
                if (item.ItemId == 0) continue;
                if (i % 7 == 0)
                {
                    ImGui.TableNextRow();
                }
                ImGui.TableNextColumn();
                if (item.ItemId == 0)
                {
                    //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                    ImGui.Text("");
                    /*var list = ImGui.GetWindowDrawList();
                    list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                }
                else
                {
                    Vector2 p = ImGui.GetCursorPos();
                    EventItem? itm = _globalCache.ItemStorage.LoadEventItem(_currentLocale, item.ItemId);
                    if (itm == null)
                    {
                        continue;
                    }

                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon, item.HQ), new Vector2(36, 36));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawEventItemTooltip(_currentLocale, _globalCache, item);
                    }

                    if (itm.StackSize <= 1)
                    {
                        continue;
                    }

                    ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                    ImGui.TextUnformatted($"{item.Quantity}");
                    ImGui.SetCursorPos(p);
                }
            }
        }

        private void DrawCrystals(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null) return;
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories_{selectedCharacter.Id}", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(180, 180));
            if (!table) return;
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selectedCharacter.Id}#Crystals#Icons", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selectedCharacter.Id}#Crystals#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selectedCharacter.Id}#Crystals#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selectedCharacter.Id}#Crystals#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(20034), new Vector2(36, 36));
            ImGui.TableSetColumnIndex(2);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(20019), new Vector2(36, 36));
            ImGui.TableSetColumnIndex(3);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(20020), new Vector2(36, 36));

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60651), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(2, selectedCharacter.Currencies.Fire_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(8, selectedCharacter.Currencies.Fire_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(14, selectedCharacter.Currencies.Fire_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60652), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(3, selectedCharacter.Currencies.Ice_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(9, selectedCharacter.Currencies.Ice_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(15, selectedCharacter.Currencies.Ice_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60653), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(4, selectedCharacter.Currencies.Wind_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(10, selectedCharacter.Currencies.Wind_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(16, selectedCharacter.Currencies.Wind_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60654), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(5, selectedCharacter.Currencies.Earth_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(11, selectedCharacter.Currencies.Earth_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(17, selectedCharacter.Currencies.Earth_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60655), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(6, selectedCharacter.Currencies.Lightning_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(12, selectedCharacter.Currencies.Lightning_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(18, selectedCharacter.Currencies.Lightning_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60656), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(7, selectedCharacter.Currencies.Water_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(13, selectedCharacter.Currencies.Water_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(19, selectedCharacter.Currencies.Water_Cluster);
        }
        private void DrawCrystal(uint itemid, int amount)
        {
            ImGui.TextUnformatted($"{amount}");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                if (amount == 0)
                {
                    ImGui.TextUnformatted(Utils.GetCrystalName(_currentLocale, _globalCache, itemid));
                }
                else
                {
                    Utils.DrawCrystalTooltip(_currentLocale, ref _globalCache, itemid, amount);
                }
                ImGui.EndTooltip();
            }
        }

        private void DrawArmory(GlobalCache globalCache, Character selectedCharacter)
        {
            if (selectedCharacter.ArmoryInventory == null) return;
            Plugin.Log.Debug($"{_armoryTabTextures.Count}");
            using var table = ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable", 3, ImGuiTableFlags.ScrollY);
            if (!table) return;

            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col2", ImGuiTableColumnFlags.WidthFixed, 275);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var armoryLeftTable =
                   ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1Table", 1))
            {
                if (armoryLeftTable)
                {
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1Table#Column",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryMainHand],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11524), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.MainHand, InventoryType.ArmoryMainHand);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryHead],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11525), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Head, InventoryType.ArmoryHead);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryBody],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11526), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Body, InventoryType.ArmoryBody);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryHands],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11527), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Hands, InventoryType.ArmoryHands);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryLegs],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11528), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Legs, InventoryType.ArmoryLegs);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryFeets],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11529), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Feets, InventoryType.ArmoryFeets);
                }
            }

            ImGui.TableSetColumnIndex(1);
            DrawArmoryInventory(globalCache);

            ImGui.TableSetColumnIndex(2);
            using var armoryRightTable = ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3Table", 1);
            if (!armoryRightTable) return;

            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3Table#Column", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryOffHand], globalCache.AddonStorage.LoadAddonString(_currentLocale, 11530), new Vector2(44, 44), selectedCharacter.ArmoryInventory.OffHand, InventoryType.ArmoryOffHand);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryEar], globalCache.AddonStorage.LoadAddonString(_currentLocale, 11531), new Vector2(44, 44), selectedCharacter.ArmoryInventory.Ear, InventoryType.ArmoryEar);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryNeck], globalCache.AddonStorage.LoadAddonString(_currentLocale, 11532), new Vector2(44, 44), selectedCharacter.ArmoryInventory.Neck, InventoryType.ArmoryNeck);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryWrist], globalCache.AddonStorage.LoadAddonString(_currentLocale, 11533), new Vector2(44, 44), selectedCharacter.ArmoryInventory.Wrist, InventoryType.ArmoryWrist);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryRings], globalCache.AddonStorage.LoadAddonString(_currentLocale, 11534), new Vector2(44, 44), selectedCharacter.ArmoryInventory.Rings, InventoryType.ArmoryRings);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmorySoulCrystal], globalCache.AddonStorage.LoadAddonString(_currentLocale, 12238), new Vector2(44, 44), selectedCharacter.ArmoryInventory.SoulCrystal, InventoryType.ArmorySoulCrystal);
        }
        private void DrawArmouryIcon(IDalamudTextureWrap? texture, string tooltip, Vector2 size, List<Gear> gear, InventoryType type)
        {
            Vector4 inactiveColor = Vector4.One with { W = 0.33f };
            Vector4 activeColor = Vector4.One;
            if (texture is null) return;
            ImGui.Image(texture.ImGuiHandle, size, Vector2.Zero, Vector2.One, (_selectedTab == type) ? activeColor : inactiveColor);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(tooltip);
                ImGui.EndTooltip();
            }
            if (ImGui.IsItemClicked())
            {
                Plugin.Log.Debug($"{tooltip} clicked");
                _selectedTab = type;
                _selectedArmory = new Dictionary<string, List<Gear>> { { tooltip, gear } };
            }
            ImGui.TextUnformatted($"{gear.FindAll(i => i.ItemId != 0).Count}");
        }

        private void DrawArmoryInventory(GlobalCache globalCache)
        {
            if (_selectedArmory == null || _selectedArmory.Count == 0) return;
            string? name = _selectedArmory.Keys.FirstOrDefault();
            if (name == null) return;
            List<Gear>? inventory = _selectedArmory.Values.FirstOrDefault();
            if (inventory == null) return;
            inventory = [.. inventory.OrderByDescending(i => i.ItemId != 0)];
            int height = inventory.FindAll(i => i.ItemId != 0).ToList().Count switch
            {
                int i when i >= 0 && i <= 5 => 48,
                int i when i >= 6 && i <= 10 => 96,
                int i when i >= 11 && i <= 15 => 144,
                int i when i >= 16 && i <= 20 => 192,
                int i when i >= 21 && i <= 25 => 240,
                int i when i >= 26 && i <= 30 => 288,
                int i when i >= 31 && i <= 35 => 336,
                int i when i >= 36 && i <= 40 => 384,
                int i when i >= 41 && i <= 45 => 432,
                int i when i >= 46 && i <= 50 => 480,
                _ => -1
            };

            using (var tableHeader =
                   ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}##Amount", 3))
            {
                if (tableHeader)
                {
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col1",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col3",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
                    ImGui.TableSetColumnIndex(2);
                }
            }

            using (var table = ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable", 5,
                       ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                       ImGuiTableFlags.ScrollY, new Vector2(266, height)))
            {
                if (table)
                {
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col4",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col5",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    for (int i = 0; i < inventory.Count; i++)
                    {
                        Gear gear = inventory[i];
                        if (gear.ItemId == 0) continue;
                        if (i is 0 or 5 or 10 or 15 or 20 or 25 or 30 or 35 or 40 or 45
                           )
                        {
                            ImGui.TableNextRow();
                        }

                        ImGui.TableNextColumn();
                        if (gear.ItemId == 0)
                        {
                            //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                            ImGui.Text("");
                            /*var list = ImGui.GetWindowDrawList();
                            list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                        }
                        else
                        {

                            /*Utils.DrawItemIcon(new Vector2(44, 44), gear.HQ, gear.ItemId);
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawGearTooltip(gear);
                            }*/
                            ItemItemLevel? itm = globalCache.ItemStorage.LoadItemWithItemLevel(_currentLocale, gear.ItemId);
                            if (itm == null) return;
                            IDalamudTextureWrap icon = globalCache.IconStorage.LoadIcon(itm.Item.Icon, gear.HQ);
                            Utils.DrawIcon(icon, new Vector2(44, 44));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawGearTooltip(_currentLocale, ref globalCache, gear, itm);
                            }
                        }
                    }
                }
            }

            using var tableFooter = ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#{name}##Amount", 2);
            if (!tableFooter)
            {
                return;
            }

            {
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Amount#Col1", ImGuiTableColumnFlags.WidthFixed, 220);
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Amount#Col2", ImGuiTableColumnFlags.WidthFixed, 60);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/{inventory.Count}");
            }
        }
    }
}