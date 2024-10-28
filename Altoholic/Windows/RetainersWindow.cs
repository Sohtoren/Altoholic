using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class RetainersWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public RetainersWindow(
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
            _characterTextures.Add(GearSlot.EMPTY, Plugin.TextureProvider.GetFromGame("ui/uld/fourth/DragTargetA_hr1.tex").RentAsync().Result);

            
        }

        public Func<Character> GetPlayer { get; set; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;
        private Retainer? _currentRetainer;
        private IEnumerable<Item>? _currentItems;
        private uint? _currentItem;
        private string _searchedItem = string.Empty;
        private string _lastSearchedItem = string.Empty;

        private readonly UldWrapper _characterIcons;
        private Dictionary<GearSlot, IDalamudTextureWrap?> _characterTextures = [];

        /*public override void OnClose()
        {
            Plugin.Log.Debug("RetainerWindow, OnClose() called");
            _currentCharacter = null;
            _currentRetainer = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
        }*/
        public void Clear()
        {
            Plugin.Log.Info("RetainerWindow, Clear() called");
            _currentCharacter = null;
            _currentRetainer = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
        }

        public void Dispose()
        {
            Plugin.Log.Info("RetainerWindow, Dispose() called");
            _currentCharacter = null;
            _currentRetainer = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;

            foreach (KeyValuePair<GearSlot, IDalamudTextureWrap?> loadedTexture in _characterTextures) loadedTexture.Value?.Dispose();
            _characterIcons.Dispose();
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using var table = ImRaii.Table("###CharactersRetainerTable", 2);
                if (!table) return;

                ImGui.TableSetupColumn("###CharactersRetainerTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn("###CharactersRetainerTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (var listBox =
                       ImRaii.ListBox("###CharactersRetainerTable#CharactersListBox", new Vector2(200, -1)))
                {
                    if (listBox)
                    {
                        if (chars.Count > 0)
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
                                _currentRetainer = null;
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
                    DrawRetainers(_currentCharacter);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic CharactersRetainerTable Exception : {0}", e);
            }    
        }

        private void DrawAll(List<Retainer> retainers)
        {
            if (ImGui.InputText(Utils.Capitalize(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3635)), ref _searchedItem, 512, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                switch (_searchedItem.Length)
                {
                    //case >= 3 when _searchedItem is "Gil" or "MGP" or "MGF" || _lastSearchedItem == _searchedItem:
                    case >= 3 when _searchedItem is "MGP" or "MGF" || _lastSearchedItem == _searchedItem:
                        return;
                    case >= 3:
                        {
                            IEnumerable<Item>? items = Utils.GetItemsFromName(_currentLocale, _searchedItem);//Todo: change this to item cache
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

            using var searchItemsTable = ImRaii.Table("###CharactersRetainer#All#SearchItemsTable", 2, ImGuiTableFlags.Borders);
            if (!searchItemsTable) return;
            ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#Item", ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#CharacterItems", ImGuiTableColumnFlags.WidthFixed, 350);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var itemlistbox = ImRaii.ListBox("###CharactersRetainer#All#SearchItemsTable#Item#Listbox",
                       new Vector2(300, -1)))
            {
                if (itemlistbox)
                {
                    foreach (Item item in currentItemsList.Where(item =>
                                 ImGui.Selectable(item.Name, item.RowId == _currentItem)))
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
                ImRaii.Table("###CharactersRetainer#All#SearchItemsTable#CharacterItems#ItemIconName", 2, ImGuiTableFlags.NoBordersInBody))
            {
                if (itemIconTable)
                {
                    ImGui.TableSetupColumn(
                        "###CharactersRetainer#All#SearchItemsTable#CharacterItems#ItemIconName#Icon",
                        ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableSetupColumn(
                        "###CharactersRetainer#All#SearchItemsTable#CharacterItems#ItemIconName#Name",
                        ImGuiTableColumnFlags.WidthFixed, 300);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon), new Vector2(36, 36));

                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{itm.Name}");
                }
            }

            List<Retainer> retainersWithItems = (itm.RowId == 1) ? retainers : retainers.FindAll(r => r.Inventory.FindAll(ri => ri.ItemId == _currentItem).Count > 0);
            if (retainersWithItems.Count == 0)
            {
                ImGui.TextUnformatted($"{Loc.Localize("NoItemOnAnyRetainer", "Item not found on any retainers.\r\nCheck if inventories are available and updated.")}");
                return;
            }

            long overallAmount = 0;
            using (ImRaii.IEndObject table =
                   ImRaii.Table("###CharactersRetainer#All#SearchItemsTable#CharacterItems#Item#Table", 2,
                       ImGuiTableFlags.Borders))
            {
                if (!table) return;
                ImGui.TableSetupColumn(
                    "###CharactersRetainer#All#SearchItemsTable#CharacterItems#Item#Table#CharacterName",
                    ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn(
                    "###CharactersRetainer#All#SearchItemsTable#CharacterItems#Item#Table#CharacterItem",
                    ImGuiTableColumnFlags.WidthStretch);
                //Plugin.Log.Debug($"retainers_with_items: {retainers_with_items}");
                foreach (Retainer retainer in retainersWithItems)
                {
                    long totalAmount = (itm.RowId == 1) ? retainers.Select(r => (long)r.Gils).Sum() : retainer.Inventory.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Inventory, uint>(0, (current, inv) => current + inv.Quantity);
                    overallAmount += totalAmount;
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{retainer.Name}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{totalAmount}");
                }
            }

            using var overallAmountTable = ImRaii.Table("###CharactersInventory#All#SearchItemsTable#CharacterItems#OverallAmountTable", 2);
            if (!overallAmountTable) return;
            ImGui.TableSetupColumn(
                "###CharactersInventory#All#SearchItemsTable#CharacterItems#OverallAmountTable#Text",
                ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn(
                "###CharactersInventory#All#SearchItemsTable#CharacterItems#OverallAmountTable#Amount",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{overallAmount}");
        }

        public void DrawRetainers(Character currentCharacter)
        {
            try
            {
                if (currentCharacter.Retainers.FindAll(r => r.Name != "RETAINER").Count > 0)
                {
                    using var table = ImRaii.Table("###CharactersRetainerTable", 2);
                    if (!table) return;

                    ImGui.TableSetupColumn("###CharactersRetainerTable#RetainersListHeader", ImGuiTableColumnFlags.WidthFixed, 130);
                    ImGui.TableSetupColumn("###CharactersRetainerTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    using (var listBox = ImRaii.ListBox("###CharactersRetainerTable#RetainerssListBox", new Vector2(200, -1)))
                    {
                        if (listBox)
                        {
                            ImGui.SetScrollY(0);
                            if (currentCharacter.Retainers.FindAll(r => r.Name != "RETAINER").Count > 0)
                            {
                                if (ImGui.Selectable($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersRetainerTable#RetainerssListBox#All", _currentRetainer == null))
                                {
                                    _currentRetainer = null;
                                }

                                foreach (Retainer currRetainer in currentCharacter.Retainers.Where(currRetainer => currRetainer.Name != "RETAINER").Where(currRetainer => ImGui.Selectable($"{currRetainer.Name}", currRetainer == _currentRetainer)))
                                {
                                    _currentRetainer = currRetainer;
                                    _currentItem = null;
                                    _currentItems = null;
                                    _searchedItem = string.Empty;
                                    _lastSearchedItem = string.Empty;
                                }
                            }
                        }
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (_currentRetainer is not null)
                    {
                        DrawRetainer(_currentRetainer, currentCharacter);
                    }
                    else
                    {
                        if (_currentCharacter is not null)
                            DrawAll(currentCharacter.Retainers);
                    }
                }
                else
                {
                    ImGui.TextUnformatted("No retainer found, please visit the retainer bell");//Todo: localization
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic CharactersRetainerTable Exception : {0}", e);
            }
        }

        private void DrawRetainer(Retainer selectedRetainer, Character retainerOwner)
        {
            using var tab = ImRaii.TabBar($"###Retainer#{selectedRetainer.Id}#TabBar");
            if (!tab) return;
            using (var detailsTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 6361)}###Retainer#{selectedRetainer.Id}#TabBar#DetailsTab"))
            {
                if (detailsTab.Success)
                {
                    DrawRetainerDetails(selectedRetainer, retainerOwner);
                }
            }
            using (var inventoryTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}###Retainer#{selectedRetainer.Id}#TabBar#InventoryTab"))
            {
                if (inventoryTab.Success)
                {
                    DrawInventories(selectedRetainer);
                }
            }
            if (selectedRetainer.MarketItemCount <= 0)
            {
                return;
            }

            using var marketTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 6556)}###Retainer#{selectedRetainer.Id}#TabBar#MarketTab");
            if (marketTab.Success)
            {
                DrawMarket(selectedRetainer);
            }
        }
        private void DrawRetainerDetails(Retainer selectedRetainer, Character retainerOwner)
        {
            using var charactersRetainerTable = ImRaii.Table($"###CharactersRetainerTable#RetainerTable{selectedRetainer.Id}", 2, ImGuiTableFlags.None, new Vector2(-1, 300));
            if (!charactersRetainerTable)
            {
                return;
            }

            ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Info_{selectedRetainer.Id}", ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{selectedRetainer.Name}");
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(065002), new Vector2(18, 18)); 
            ImGui.SameLine(); ImGui.TextUnformatted($"{selectedRetainer.Gils}");
            ImGui.TableSetColumnIndex(1);

            using (var ventureTable = ImRaii.Table($"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Venture", 3, ImGuiTableFlags.None, new Vector2(-1, 50)))
            {
                if (!ventureTable) return;
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Venture#Col1", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Venture#Col2", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Venture#Col3", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2322)}");
                RetainerTask? venture = Utils.GetRetainerTask(_currentLocale, selectedRetainer.VentureID);
                if (venture != null)
                {
                    if (venture.RetainerLevel > 0)
                    {
                        ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 464)} {venture.RetainerLevel}");
                    }
                    if (!venture.IsRandom)
                    {
                        RetainerTaskNormal? task = Utils.GetRetainerTaskNormal(_currentLocale, venture.Task);
                        if (task != null)
                        {
                            ImGui.SameLine();
                            Item? item = task.Item.Value;
                            if (item != null)
                                ImGui.TextUnformatted($"{item.Name}");
                        }
                    }
                    else
                    {
                        RetainerTaskRandom? task = Utils.GetRetainerTaskRandom(_currentLocale, venture.Task);
                        if (task != null)
                        {
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{task.Name}");
                        }
                    }
                }
                ImGui.TableSetColumnIndex(2);
            }

            using var gearTable =
                ImRaii.Table($"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Gear", 3,
                    ImGuiTableFlags.None, new Vector2(-1, 50));
            if (!gearTable) return;
            ImGui.TableSetupColumn(
                $"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Gear#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(
                $"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Gear#Col2",
                ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn(
                $"###CharactersRetainerTable#RetainerTable#Gear_{selectedRetainer.Id}#Gear#Col3",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableSetColumnIndex(1);
            if (selectedRetainer.ClassJob != 0 && selectedRetainer.Gear.Count > 0)
            {
                Utils.DrawGear(_currentLocale, ref _globalCache, ref _characterTextures,
                    selectedRetainer.Gear, selectedRetainer.ClassJob, selectedRetainer.Level, 200, 180,
                    true, Utils.GetRetainerJobMaxLevel(selectedRetainer.ClassJob, retainerOwner));
            }
            else
            {
                ImGui.TextUnformatted(
                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5448)} {Loc.Localize("OpenRetainer", "Open the retainer to update the gear")}");
            }
        }

        private void DrawInventories(Retainer selectedRetainer)
        {
            if (selectedRetainer.Inventory.Count != 193)
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5448)} {Loc.Localize("OpenRetainer", "Open the retainer to update")}");
            }
            else
            {
                List<Inventory> inventory = [.. selectedRetainer.Inventory[..175].OrderByDescending(i => i.Quantity)];//Todo: Use ItemOrderModule
                using var table = ImRaii.Table($"###CharactersRetainerTable#Inventories#{selectedRetainer.Id}", 2, ImGuiTableFlags.ScrollY);
                if (!table) return;
                ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Bags_{selectedRetainer.Id}", ImGuiTableColumnFlags.WidthFixed, 450);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Crystals_{selectedRetainer.Id}", ImGuiTableColumnFlags.WidthFixed, 180);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}");

                DrawRetainerInventory($"Retainer_{selectedRetainer.Id}", inventory);
                using (var charactersRetainerTableInventoriesBagTable = ImRaii.Table($"###CharactersRetainerTable#Inventories#Bags_{selectedRetainer.Id}#Amount", 2))
                {
                    if (charactersRetainerTableInventoriesBagTable)
                    {
                        ImGui.TableSetupColumn(
                            $"###CharactersRetainerTable#Inventories#Bags_{selectedRetainer.Id}#Amount#Col1",
                            ImGuiTableColumnFlags.WidthFixed, 390);
                        ImGui.TableSetupColumn(
                            $"###CharactersRetainerTable#Inventories#Bags_{selectedRetainer.Id}#Amount#Col2",
                            ImGuiTableColumnFlags.WidthFixed, 80);
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text("");
                        ImGui.TableSetColumnIndex(1);
                        ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/175");
                    }
                }

                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2882)}");
                DrawCrystals(selectedRetainer);
            }
        }

        private void DrawRetainerInventory(string label, List<Inventory> inventory)
        {
            using var table = ImRaii.Table($"###CharactersRetainerTable#Inventories#Inventory#{label}Table", 10, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner);
            if (!table) return;

            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col4", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col5", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col6", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col7", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col8", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col9", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col10", ImGuiTableColumnFlags.WidthFixed, 36);
            for (int i = 0; i < inventory.Count; i++)
            {
                Inventory item = inventory[i];
                if (i is 0 or 10 or 20 or 30 or 40 or 50 or 60 or 70 or 80 or 90 or 100 or 110 or 120 or 130 or 140 or 150 or 160 or 170
                   )
                {
                    ImGui.TableNextRow();
                }
                ImGui.TableNextColumn();
                if (item.ItemId == 0)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(0), new Vector2(36, 36));
                }
                else
                {
                    Vector2 p = ImGui.GetCursorPos();
                    Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, item.ItemId);
                    if (itm == null)
                    {
                        continue;
                    }

                    bool armoire = _globalCache.ItemStorage.CanBeInArmoire(itm.RowId);
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon, item.HQ), new Vector2(36, 36));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawInventoryItemTooltip(_currentLocale, ref _globalCache, item, armoire);
                    }

                    if (itm.StackSize > 1)
                    {
                        ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                        ImGui.TextUnformatted($"{item.Quantity}");
                        ImGui.SetCursorPos(p);
                    }

                    if (!armoire)
                    {
                        continue;
                    }
                    ImGui.SetCursorPos(p with { X = p.X + 20 });
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(066460), new Vector2(16, 16));
                    ImGui.SetCursorPos(p);
                }
            }
            ImGui.TableSetColumnIndex(6);
            ImGui.TableSetColumnIndex(7);
            ImGui.TableSetColumnIndex(8);
            ImGui.TableSetColumnIndex(9);
            ImGui.TableSetColumnIndex(10);
        }

        private void DrawCrystals(Retainer selectedRetainer)
        {
            using var table = ImRaii.Table($"###CharactersRetainerTable#Inventories_{selectedRetainer.Id}", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(180, 180));
            if (!table) return;
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selectedRetainer.Id}#Crystals#Icons", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selectedRetainer.Id}#Crystals#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selectedRetainer.Id}#Crystals#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selectedRetainer.Id}#Crystals#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
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
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60651), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(2, selectedRetainer.Inventory[175].Quantity);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(8, selectedRetainer.Inventory[181].Quantity);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(14, selectedRetainer.Inventory[187].Quantity);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60652), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(3, selectedRetainer.Inventory[176].Quantity);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(9, selectedRetainer.Inventory[182].Quantity);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(15, selectedRetainer.Inventory[188].Quantity);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60653), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(4, selectedRetainer.Inventory[177].Quantity);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(10, selectedRetainer.Inventory[183].Quantity);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(16, selectedRetainer.Inventory[189].Quantity);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60654), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(5, selectedRetainer.Inventory[178].Quantity);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(11, selectedRetainer.Inventory[184].Quantity);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(17, selectedRetainer.Inventory[190].Quantity);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60655), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(6, selectedRetainer.Inventory[179].Quantity);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(12, selectedRetainer.Inventory[185].Quantity);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(18, selectedRetainer.Inventory[191].Quantity);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60656), new Vector2(40, 40));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(7, selectedRetainer.Inventory[180].Quantity);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(13, selectedRetainer.Inventory[186].Quantity);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(19, selectedRetainer.Inventory[192].Quantity);
        }

        private void DrawCrystal(uint itemid, uint amount)
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
                    Utils.DrawCrystalTooltip(_currentLocale, ref _globalCache, itemid, (int)amount);
                }
                ImGui.EndTooltip();
            }
        }
        private void DrawMarket(Retainer selectedRetainer)
        {
            if (selectedRetainer.MarketItemCount == 0)
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12596)}");
                return;
            }
            if (selectedRetainer.MarketInventory.Count > 0)
            {
                ImGui.TextUnformatted($"On the market until: {Utils.UnixTimeStampToDateTime(selectedRetainer.MarketExpire)}");
                int count = 0;
                using (var table = ImRaii.Table($"###Retainer#{selectedRetainer.Id}#MarketTable", 2))
                {
                    if (!table) return;
                    ImGui.TableSetupColumn(
                        $"###Retainer#{selectedRetainer.Id}#MarketTable#Col1",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn(
                        $"###Retainer#{selectedRetainer.Id}#MarketTable#Col2",
                        ImGuiTableColumnFlags.WidthStretch);

                    foreach (Inventory item in selectedRetainer.MarketInventory.Where(item => item.ItemId != 0))
                    {
                        if(count % 2 == 0) ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(count % 2 == 0 ? 0 : 1);
                        Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, item.ItemId);
                        if (itm == null) continue;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon, item.HQ), new Vector2(24, 24));
                        if (ImGui.IsItemHovered())
                        {
                            Utils.DrawInventoryItemTooltip(_currentLocale, ref _globalCache, item);
                        }
                        count++;
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{itm.Name}");
                        if (item.Quantity <= 1)
                        {
                            continue;
                        }

                        ImGui.SameLine();
                        ImGui.TextUnformatted($"{item.Quantity}");
                    }
                }
                string selstr = _currentLocale switch
                {
                    ClientLanguage.German => $"{count} {(count == 1 ? "Gegenstand" : "Gegenstände")}",
                    ClientLanguage.English => $"Selling {count} {(count == 1 ? "item" : "items")}",
                    ClientLanguage.French => $"{count} objet{(count > 1 ? "s" : "")} en vente",
                    ClientLanguage.Japanese => $"出品中（{count}件）",
                    _ => ""
                };
                ImGui.TextUnformatted($"{selstr}");
            }
            else
            {
                ImGui.TextUnformatted($"{selectedRetainer.MarketItemCount} found. {Loc.Localize("OpenRetainerToUpdateList", "Open the retainer to list")}");
            }
        }
    }
}