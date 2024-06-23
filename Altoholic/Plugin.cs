using Dalamud.Game.Command;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Altoholic.Windows;
using LiteDB;
using Altoholic.Models;
using System.Collections.Generic;
using System.Threading;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Game;
using System.Runtime.InteropServices;
using Dalamud;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using static FFXIVClientStructs.FFXIV.Client.UI.RaptureAtkModule;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Dalamud.Memory;
using Dalamud.Game.ClientState.Conditions;
using Altoholic.Cache;
using Dalamud.Game.ClientState.Resolvers;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using Quest = Altoholic.Models.Quest;

namespace Altoholic
{
    public sealed class Plugin : IDalamudPlugin
    {
        public static string Name => "Altoholic Plugin";
        private const string CommandName = "/altoholic";
        private readonly int[] _questIds = [65970, 66045, 66216, 66217, 66218, 66640, 66641, 66642, 66754, 66789, 66857, 66911, 66968, 66969, 66970, 67023, 67099, 67100, 67101, 67658, 67700, 67791, 67856, 68509, 68572, 68633, 68734, 68817, 69133, 69219, 69330, 69432, 70081, 70137, 70217, 69208, 67631, 69208];
        //private DalamudPluginInterface PluginInterface { get; init; }
        //private readonly ICommandManager commandManager;
        //private readonly IClientState ClientState;
        //private readonly IFramework framework;
        //private readonly ILog Log;
        //private readonly IDataManager Plugin.DataManager;
        //private readonly ITextureProvider textureProvider;
        //private readonly INotificationManager notificationManager;
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static IFramework Framework { get; private set; } = null!;
        [PluginService] public static IPluginLog Log { get; private set; } = null!;
        [PluginService] public static IDataManager DataManager { get; private set; } = null!;
        [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
        [PluginService] public static INotificationManager NotificationManager { get; private set; } = null!;
        [PluginService] public static ICondition Condition { get; set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IGameInteropProvider Hook { get; private set; } = null!;

        private const string PlaytimeSig = "E8 ?? ?? ?? ?? B9 ?? ?? ?? ?? 48 8B D3";
        private delegate long PlaytimeDelegate(uint param1, long param2, uint param3);
        private readonly Hook<PlaytimeDelegate> _playtimeHook;

        public Configuration Configuration { get; set; }
        public WindowSystem WindowSystem = new("Altoholic");

        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }
        private CharactersWindow CharactersWindow { get; init; }
        private DetailsWindow DetailsWindow { get; init; }
        private JobsWindow JobsWindow { get; init; }
        private CurrenciesWindow CurrenciesWindow { get; init; }
        private InventoriesWindow InventoriesWindow { get; init; }
        private RetainersWindow RetainersWindow { get; init; }
        private CollectionWindow CollectionWindow { get; init; }

        private readonly Service altoholicService = null!;
        private readonly LiteDatabase _db;

        private Character _localPlayer = new();
        private Utf8String? _localPlayerFreeCompanyTest;

        private readonly PeriodicTimer? _periodicTimer = null;
        public List<Character> OtherCharacters = [];
        private static ClientLanguage _currentLocale;
        private readonly Localization _localization = new();
        private readonly GlobalCache _globalCache;

        public Plugin()
        {
            _globalCache = new GlobalCache
            {
                IconStorage = new IconStorage(TextureProvider),
                ItemStorage = new ItemStorage(),
                JobStorage = new JobStorage(),
                AddonStorage = new AddonStorage(),
                StainStorage = new StainStorage(),
            };

            nint playtimePtr = SigScanner.ScanText(PlaytimeSig);
            //if (playtimePtr == nint.Zero) return;
            _playtimeHook = Hook.HookFromAddress<PlaytimeDelegate>(playtimePtr, PlaytimePacket);
            _playtimeHook.Enable();

            string dbpath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "altoholic.db");
            /*try
            {*/

            _db = new LiteDatabase(dbpath);
            /*}
            catch (Exception)
            {
                _ = new XivChatEntry
                {
                    Message = $"Error accessing the database",
                    Type = XivChatType.Echo,
                };
            }*/
            // Todo: Make sure this don't crash the game when db is already opened

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);

            if (Configuration.Language != ClientState.ClientLanguage)
            {
                Configuration.Language = ClientState.ClientLanguage;
                Configuration.Save();
            }
            _currentLocale = Configuration.Language;
            _localization.SetupWithLangCode(PluginInterface.UiLanguage);

            altoholicService = new Service(
                () => _localPlayer,
                () => OtherCharacters);

            ConfigWindow = new ConfigWindow(this, $"{Name} configuration", _globalCache);
            CharactersWindow = new CharactersWindow(this, $"{Name} characters", _db, _globalCache)
            {
                GetPlayer = () => altoholicService.GetPlayer(),
                GetOthersCharactersList = () => altoholicService.GetOthersCharacters(),
            };

            DetailsWindow = new DetailsWindow(this, $"{Name} characters details", _globalCache)
            {
                GetPlayer = () => altoholicService.GetPlayer(),
                GetOthersCharactersList = () => altoholicService.GetOthersCharacters(),
            };

            JobsWindow = new JobsWindow(this, $"{Name} characters jobs", _globalCache)
            {
                GetPlayer = () => altoholicService.GetPlayer(),
                GetOthersCharactersList = () => altoholicService.GetOthersCharacters(),
            };

            CurrenciesWindow = new CurrenciesWindow(this, $"{Name} characters currencies", _globalCache)
            {
                GetPlayer = () => altoholicService.GetPlayer(),
                GetOthersCharactersList = () => altoholicService.GetOthersCharacters(),
            };

            InventoriesWindow = new InventoriesWindow(this, $"{Name} characters inventories", _globalCache)
            {
                GetPlayer = () => altoholicService.GetPlayer(),
                GetOthersCharactersList = () => altoholicService.GetOthersCharacters(),
            };

            RetainersWindow = new RetainersWindow(this, $"{Name} characters retainers", _globalCache)
            {
                GetPlayer = () => altoholicService.GetPlayer(),
                GetOthersCharactersList = () => altoholicService.GetOthersCharacters(),
            };

            CollectionWindow = new CollectionWindow(this, $"{Name} characters colletion", _globalCache)
            {
            };


            MainWindow = new MainWindow(
                this,
                $"{Name} characters",
                _globalCache,
                /*Log,
                Plugin.DataManager,*/
                CharactersWindow,
                DetailsWindow,
                JobsWindow,
                CurrenciesWindow,
                InventoriesWindow,
                RetainersWindow,
                CollectionWindow,
                ConfigWindow);

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Show/hide Altoholic"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            PluginInterface.UiBuilder.OpenMainUi += DrawMainUI;
            PluginInterface.LanguageChanged += _localization.SetupWithLangCode;

            // Todo: On retainers retainer window close
            ClientState.Login += OnCharacterLogin;
            ClientState.Logout += OnCharacterLogout;
            Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            _playtimeHook.Dispose();
            WindowSystem.RemoveAllWindows();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            PluginInterface.UiBuilder.OpenMainUi -= DrawMainUI;
            PluginInterface.LanguageChanged -= _localization.SetupWithLangCode;

            _globalCache.IconStorage.Dispose();
            _globalCache.ItemStorage.Dispose();
            _globalCache.JobStorage.Dispose();
            _globalCache.AddonStorage.Dispose();
            _globalCache.StainStorage.Dispose();

            RetainersWindow.Dispose();
            InventoriesWindow.Dispose();
            JobsWindow.Dispose();
            ConfigWindow.Dispose();
            CurrenciesWindow.Dispose();
            DetailsWindow.Dispose();
            CharactersWindow.Dispose();
            MainWindow.Dispose();

            CleanLastLocalCharacter();

            CommandManager.RemoveHandler(CommandName);

            ClientState.Login -= OnCharacterLogin;
            ClientState.Logout -= OnCharacterLogout;
            Framework.Update -= OnFrameworkUpdate;
            _db.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            Log.Debug("OnCommand called");
            if (MainWindow.IsOpen)
            {
                MainWindow.IsOpen = false;
            }
            else
            {
                DrawMainUI();
            }
        }

        // ReSharper disable once InconsistentNaming
        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        // ReSharper disable once InconsistentNaming
        private void DrawMainUI()
        {
            if (!ClientState.IsLoggedIn)
            {
                Log.Error("No character logged in, doing nothing");

                NotificationManager.AddNotification(new Notification()
                {
                    Title = "Altoholic",
                    Content = "This plugin need a character to be logged in",
                    Type = NotificationType.Error,
                    Minimized = false,
                    InitialDuration = TimeSpan.FromSeconds(3)
                });
                return;
            }

            //Plugin.Log.Debug($"localPlayerName : {localPlayer.FirstName} {localPlayer.LastName}");
            if (_localPlayer.Id == 0 || string.IsNullOrEmpty(_localPlayer.FirstName))
            {
                Character? p = GetCharacterFromGameOrDB();
                if (p is not null)
                {
                    _localPlayer = p;
                    //Plugin.Log.Debug($"localPlayerPlayTime : {localPlayerPlayTime}");
                    if (_localPlayer.PlayTime == 0)//still needed?
                    {
                        Character? chara = Database.Database.GetCharacter(Log, _db, _localPlayer.Id);
                        if (chara is not null)
                        {
                            _localPlayer.PlayTime = chara.PlayTime;
                            _localPlayer.LastPlayTimeUpdate = chara.LastPlayTimeUpdate;
                        }
                    }

                    Log.Debug($"Character localLastPlayTimeUpdate : {_localPlayer.LastPlayTimeUpdate}");
#if DEBUG
                    /*foreach (Inventory inventory in _localPlayer.Inventory)
                    {
                        Plugin.Log.Debug($"{inventory.ItemId} {lumina.Singular} {inventory.HQ} {inventory.Quantity}");
                    }*/
#endif
                }
            }
            if (_localPlayer.Id != 0)
            {
                if (OtherCharacters.Count == 0)
                {
                    OtherCharacters = Database.Database.GetOthersCharacters(Log, _db, _localPlayer.Id);
                }
                //Plugin.Log.Debug($"otherCharacters count {otherCharacters.Count}");

                //Plugin.Log.Debug($"localPlayer.Quests.Count: {localPlayer.Quests.Count}");
                if (_localPlayer.Quests.Count == 0)
                {
                    //Plugin.Log.Debug("No quest found, fetching from db");
                    Character? chara = Database.Database.GetCharacter(Log, _db, _localPlayer.Id);
                    if (chara != null)
                    {
                        _localPlayer.Quests = chara.Quests;
                    }
                }
                //Plugin.Log.Debug($"localPlayer.Quests.Count: {localPlayer.Quests.Count}");
                if (_localPlayer.Retainers.Count == 0)
                {
                    Character? chara = Database.Database.GetCharacter(Log, _db, _localPlayer.Id);
                    if (chara != null)
                    {
                        _localPlayer.Retainers = chara.Retainers;
                    }
                }
                foreach (Gear i in _localPlayer.Gear)
                {
                    Log.Debug($"Gear: {i.ItemId} {Enum.GetName(typeof(GearSlot), i.Slot)} {i.Slot}");
                }
#if DEBUG
                /*Plugin.Log.Debug($"Title {localPlayer.Profile.Title}");
                Plugin.Log.Debug($"Title Length {localPlayer.Profile.Title.Length}");
                Plugin.Log.Debug($"Grand Company {localPlayer.Profile.Grand_Company}");
                Plugin.Log.Debug($"Grand Company Rank {localPlayer.Profile.Grand_Company_Rank}");
                Plugin.Log.Debug($"Gender {Utils.GetGender(localPlayer.Profile.Gender)}");
                Plugin.Log.Debug($"Race {Utils.GetRace(localPlayer.Profile.Gender, localPlayer.Profile.Race)}");
                Plugin.Log.Debug($"Tribe {Utils.GetTribe(localPlayer.Profile.Gender, localPlayer.Profile.Tribe)}");
                Plugin.Log.Debug($"City State {Utils.GetTown(localPlayer.Profile.City_State)}");
                Plugin.Log.Debug($"Nameday_Day {localPlayer.Profile.Nameday_Day}");
                Plugin.Log.Debug($"Nameday_Month {localPlayer.Profile.Nameday_Month}");
                Plugin.Log.Debug($"Guardian {Utils.GetGuardian(localPlayer.Profile.Guardian)}");*/
#endif
                PlayerCharacter? lPlayer = ClientState.LocalPlayer;
                if (lPlayer != null)
                {
                    _localPlayer.Attributes = new Attributes
                    {
                        Hp = lPlayer.MaxHp,
                        Mp = lPlayer.MaxMp
                    };

                    GetPlayerAttributesProfileAndJobs();
                    GetPlayerEquippedGear();
                    GetPlayerInventory();
                    GetPlayerSaddleInventory();
                    GetPlayerArmoryInventory();
                    GetPlayerGlamourInventory();
                    GetPlayerArmoireInventory();
                    GetPlayerCompletedQuest();
                    GetPlayerRetainer();

                    /*
                    Log.Debug($"localPlayer.Inventory.Count : {localPlayer.Inventory.Count}");
                    Log.Debug($"localPlayer.Saddle.Count: {localPlayer.Saddle.Count}");
                    foreach (var inventory in localPlayer.Saddle)
                    {
                        Log.Debug($"{inventory.ItemId} {inventory.HQ} {inventory.Quantity}");
                    }
                    */
/*Log.Debug($"localPlayer retainers count : {localPlayer.Retainers.Count}");
foreach(Retainer retainer in localPlayer.Retainers)
{
    Log.Debug($"{retainer.Name} job:{Enum.GetName(typeof(ClassJob), retainer.ClassJob)}, displayorder: {retainer.DisplayOrder}, items: {retainer.Inventory.Count}, gils: {retainer.Gils}");
}*/
#if DEBUG
                    if (_localPlayer.ArmoryInventory != null)
                    {
                        Log.Debug($"localPlayer.ArmoryInventory.MainHand.Count : {_localPlayer.ArmoryInventory.MainHand.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Head.Count : {_localPlayer.ArmoryInventory.Head.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Body.Count : {_localPlayer.ArmoryInventory.Body.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Hands.Count : {_localPlayer.ArmoryInventory.Hands.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Legs.Count : {_localPlayer.ArmoryInventory.Legs.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Feets.Count : {_localPlayer.ArmoryInventory.Feets.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.OffHand.Count : {_localPlayer.ArmoryInventory.OffHand.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Ear.Count : {_localPlayer.ArmoryInventory.Ear.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Neck.Count : {_localPlayer.ArmoryInventory.Neck.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Wrist.Count : {_localPlayer.ArmoryInventory.Wrist.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Rings.Count : {_localPlayer.ArmoryInventory.Rings.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.SoulCrystal.Count : {_localPlayer.ArmoryInventory.SoulCrystal.FindAll(i => i.ItemId != 0).Count}");
                        /*foreach (var inventory in localPlayer.ArmoryInventory.Hands)
                        {
                            Log.Debug($"{inventory.ItemId} {Utils.GetItemNameFromId(inventory.ItemId)} {inventory.HQ}");
                        }*/
                    }
#endif
                }

                if (ClientState.IsLoggedIn)
                {
                    _localPlayer.LastOnline = 0;
                }
#if DEBUG
                /*Plugin.Log.Debug($"Gladiator : {localPlayer.Jobs.Gladiator.Level}");
                Plugin.Log.Debug($"Pugilist : {localPlayer.Jobs.Pugilist.Level}");
                Plugin.Log.Debug($"Marauder : {localPlayer.Jobs.Marauder.Level}");
                Plugin.Log.Debug($"Lancer : {localPlayer.Jobs.Lancer.Level}");
                Plugin.Log.Debug($"Archer : {localPlayer.Jobs.Archer.Level}");
                Plugin.Log.Debug($"Conjurer : {localPlayer.Jobs.Conjurer.Level}");
                Plugin.Log.Debug($"Thaumaturge : {localPlayer.Jobs.Thaumaturge.Level}");
                Plugin.Log.Debug($"Carpenter : {localPlayer.Jobs.Carpenter.Level}");
                Plugin.Log.Debug($"Blacksmith : {localPlayer.Jobs.Blacksmith.Level}");
                Plugin.Log.Debug($"Armorer : {localPlayer.Jobs.Armorer.Level}");
                Plugin.Log.Debug($"Goldsmith : {localPlayer.Jobs.Goldsmith.Level}");
                Plugin.Log.Debug($"Leatherworker : {localPlayer.Jobs.Leatherworker.Level}");
                Plugin.Log.Debug($"Weaver : {localPlayer.Jobs.Weaver.Level}");
                Plugin.Log.Debug($"Alchemist : {localPlayer.Jobs.Alchemist.Level}");
                Plugin.Log.Debug($"Culinarian : {localPlayer.Jobs.Culinarian.Level}");
                Plugin.Log.Debug($"Miner : {localPlayer.Jobs.Miner.Level}");
                Plugin.Log.Debug($"Botanist : {localPlayer.Jobs.Botanist.Level}");
                Plugin.Log.Debug($"Fisher : {localPlayer.Jobs.Fisher.Level}");
                Plugin.Log.Debug($"Paladin : {localPlayer.Jobs.Paladin.Level}");
                Plugin.Log.Debug($"Monk : {localPlayer.Jobs.Monk.Level}");
                Plugin.Log.Debug($"Warrior : {localPlayer.Jobs.Warrior.Level}");
                Plugin.Log.Debug($"Dragoon : {localPlayer.Jobs.Dragoon.Level}");
                Plugin.Log.Debug($"Bard : {localPlayer.Jobs.Bard.Level}");
                Plugin.Log.Debug($"WhiteMage : {localPlayer.Jobs.WhiteMage.Level}");
                Plugin.Log.Debug($"BlackMage : {localPlayer.Jobs.BlackMage.Level}");
                Plugin.Log.Debug($"Arcanist : {localPlayer.Jobs.Arcanist.Level}");
                Plugin.Log.Debug($"Summoner : {localPlayer.Jobs.Summoner.Level}");
                Plugin.Log.Debug($"Scholar : {localPlayer.Jobs.Scholar.Level}");
                Plugin.Log.Debug($"Rogue : {localPlayer.Jobs.Rogue.Level}");
                Plugin.Log.Debug($"Ninja : {localPlayer.Jobs.Ninja.Level}");
                Plugin.Log.Debug($"Machinist : {localPlayer.Jobs.Machinist.Level}");
                Plugin.Log.Debug($"DarkKnight : {localPlayer.Jobs.DarkKnight.Level}");
                Plugin.Log.Debug($"Astrologian : {localPlayer.Jobs.Astrologian.Level}");
                Plugin.Log.Debug($"Samurai : {localPlayer.Jobs.Samurai.Level}");
                Plugin.Log.Debug($"RedMage : {localPlayer.Jobs.RedMage.Level}");
                Plugin.Log.Debug($"BlueMage : {localPlayer.Jobs.BlueMage.Level}");
                Plugin.Log.Debug($"Gunbreaker : {localPlayer.Jobs.Gunbreaker.Level}");
                Plugin.Log.Debug($"Dancer : {localPlayer.Jobs.Dancer.Level}");
                Plugin.Log.Debug($"Reaper : {localPlayer.Jobs.Reaper.Level}");
                Plugin.Log.Debug($"Sage : {localPlayer.Jobs.Sage.Level}");*/
#endif
                if (_localPlayer.PlayTime == 0)
                {
                    Character? p = GetCharacterFromGameOrDB();
                    if (p is not null)
                    {
                        _localPlayer.PlayTime = p.PlayTime;
                    }
                }
            }
            MainWindow.IsOpen = true;
        }

        // ReSharper disable once InconsistentNaming
        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            PlayerCharacter? lPlayer = ClientState.LocalPlayer;
            if (lPlayer == null)
            {
                return;
            }

            if (_localPlayer.Id == 0)
            {
                _localPlayer = new Character { Id = ClientState.LocalContentId };
            }

            string name = lPlayer.Name.TextValue;
            if (string.IsNullOrEmpty(name)) return;
            string[] names = name.Split(" ");
            if (names.Length == 2)
            {
                _localPlayer.FirstName = names[0];
                _localPlayer.LastName = names[1];
            }
            ExcelResolver<World> hw = lPlayer.HomeWorld;
            {
                World? hwgd = hw.GameData;
                if (hwgd != null)
                {
                    _localPlayer.HomeWorld = hwgd.Name ?? string.Empty;
                    _localPlayer.Datacenter = Utils.GetDatacenterFromWorld(_localPlayer.HomeWorld);
                    _localPlayer.Region = Utils.GetRegionFromWorld(_localPlayer.HomeWorld);
                }
            }
            ExcelResolver<World> cw = lPlayer.CurrentWorld;
            {
                World? cwhd = cw.GameData;
                if (cwhd != null)
                {
                    _localPlayer.CurrentWorld = cwhd.Name ?? string.Empty;
                    _localPlayer.CurrentDatacenter = Utils.GetDatacenterFromWorld(_localPlayer.CurrentWorld);
                    _localPlayer.CurrentRegion = Utils.GetRegionFromWorld(_localPlayer.CurrentWorld);
                }
            }

            _localPlayer.LastJob = lPlayer.ClassJob.Id;
            _localPlayer.LastJobLevel = lPlayer.Level;
            _localPlayer.FCTag = lPlayer.CompanyTag.TextValue;
            //Plugin.Log.Debug($"localPlayerFreeCompanyTag : {localPlayerFreeCompanyTag}");
            //localPlayerFreeCompany = localPlayer..TextValue ?? string.Empty;
            try
            {
                unsafe
                {
                    ref readonly AgentInspect agentInspect = ref *AgentInspect.Instance();
                    _localPlayerFreeCompanyTest = agentInspect.FreeCompany.GuildName;
                    /*Plugin.Log.Debug($"localPlayerFreeCompanyTest???");
                        if(localPlayerFreeCompanyTest != null)
                        {
                            Plugin.Log.Debug($"localPlayerFreeCompanyTest : {localPlayerFreeCompanyTest}");
                        }*/
                    //localPlayerFreeCompanyTest = FFXIVClientStructs.FFXIV.Client.UI.Agent.AgentInspect.Instance()->FreeCompany.GuildName;
                    //Log.Debug($"localPlayerFreeCompanyTest : {}");
                    //Plugin.Log.Debug(System.Text.Encoding.UTF8.GetString(AgentInspect.Instance()->FreeCompany.GuildName));
                    //Plugin.Log.Debug(System.Text.Encoding.UTF8.GetString(localPlayerFreeCompanyTest)); ;
                    //Plugin.Log.Debug($"localPlayerFreeCompany : {localPlayerFreeCompanyTest}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not get free company name");
            }
            _localPlayer.Attributes = new Attributes
            {
                Hp = lPlayer.MaxHp,
                Mp = lPlayer.MaxMp
            };

            GetPlayerAttributesProfileAndJobs();
            GetPlayerEquippedGear();
            GetPlayerInventory();
            GetPlayerArmoryInventory();
            GetPlayerGlamourInventory();
            GetPlayerArmoireInventory();
            GetPlayerSaddleInventory();
            GetPlayerRetainer();
        }

        private unsafe void GetPlayerAttributesProfileAndJobs()
        {
            if (_localPlayer.Id == 0) return;
            string title = string.Empty;
            bool prefixTitle = false;
            RaptureAtkModule* raptureAtkModule = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetRaptureAtkModule();
            if (raptureAtkModule != null)
            {
                Span<NamePlateInfo> npi = raptureAtkModule->NamePlateInfoEntriesSpan;
                if (npi != null)
                {
                    for (int i = 0; i < 50 && i < raptureAtkModule->NameplateInfoCount; i++)
                    {
                        ref NamePlateInfo namePlateInfo = ref npi[i];
                        if (ClientState.LocalPlayer == null) continue;
                        if (namePlateInfo.ObjectID.ObjectID != ClientState.LocalPlayer.ObjectId)
                        {
                            continue;
                        }

                        string t = namePlateInfo.Title.ToString();//this sometime get player name??? to recheck
                        //Plugin.Log.Debug($"t: {t}");
                        if (t == $"{_localPlayer.FirstName} {_localPlayer.LastName}")
                        {
                            continue;
                        }

                        title = t;
                        prefixTitle = namePlateInfo.IsPrefixTitle;
                    }
                }
            }
            ref readonly UIState uistate = ref *UIState.Instance();//nullcheck?
            PlayerState player = uistate.PlayerState;
            _localPlayer.IsSprout = player.IsNovice();
            _localPlayer.HasPremiumSaddlebag = player.HasPremiumSaddlebag;
            _localPlayer.Profile = new Profile
            {
                Title = title,
                TitleIsPrefix = prefixTitle,
                Grand_Company = player.GrandCompany,
                Grand_Company_Rank = player.GetGrandCompanyRank(),
                Race = player.Race,
                Tribe = player.Tribe,
                Gender = player.Sex,
                City_State = player.StartTown,
                Nameday_Day = player.BirthDay,
                Nameday_Month = player.BirthMonth,
                Guardian = player.GuardianDeity
            };
            _localPlayer.Jobs = new Jobs
            {
                Adventurer = new Job { Level = player.ClassJobLevelArray[-1], Exp = player.ClassJobExpArray[-1] },
                Gladiator = new Job { Level = player.ClassJobLevelArray[1], Exp = player.ClassJobExpArray[1] },
                Pugilist = new Job { Level = player.ClassJobLevelArray[0], Exp = player.ClassJobExpArray[0] },
                Marauder = new Job { Level = player.ClassJobLevelArray[2], Exp = player.ClassJobExpArray[2] },
                Lancer = new Job { Level = player.ClassJobLevelArray[4], Exp = player.ClassJobExpArray[4] },
                Archer = new Job { Level = player.ClassJobLevelArray[3], Exp = player.ClassJobExpArray[3] },
                Conjurer = new Job { Level = player.ClassJobLevelArray[6], Exp = player.ClassJobExpArray[6] },
                Thaumaturge = new Job { Level = player.ClassJobLevelArray[5], Exp = player.ClassJobExpArray[5] },
                Carpenter = new Job() { Level = player.ClassJobLevelArray[7], Exp = player.ClassJobExpArray[7] },
                Blacksmith = new Job { Level = player.ClassJobLevelArray[8], Exp = player.ClassJobExpArray[8] },
                Armorer = new Job { Level = player.ClassJobLevelArray[9], Exp = player.ClassJobExpArray[9] },
                Goldsmith = new Job { Level = player.ClassJobLevelArray[10], Exp = player.ClassJobExpArray[10] },
                Leatherworker = new Job { Level = player.ClassJobLevelArray[11], Exp = player.ClassJobExpArray[11] },
                Weaver = new Job { Level = player.ClassJobLevelArray[12], Exp = player.ClassJobExpArray[12] },
                Alchemist = new Job { Level = player.ClassJobLevelArray[13], Exp = player.ClassJobExpArray[13] },
                Culinarian = new Job { Level = player.ClassJobLevelArray[14], Exp = player.ClassJobExpArray[14] },
                Miner = new Job { Level = player.ClassJobLevelArray[15], Exp = player.ClassJobExpArray[15] },
                Botanist = new Job { Level = player.ClassJobLevelArray[16], Exp = player.ClassJobExpArray[16] },
                Fisher = new Job { Level = player.ClassJobLevelArray[17], Exp = player.ClassJobExpArray[17] },
                Paladin = new Job { Level = player.ClassJobLevelArray[1], Exp = player.ClassJobExpArray[1] },
                Monk = new Job { Level = player.ClassJobLevelArray[0], Exp = player.ClassJobExpArray[0] },
                Warrior = new Job { Level = player.ClassJobLevelArray[2], Exp = player.ClassJobExpArray[2] },
                Dragoon = new Job { Level = player.ClassJobLevelArray[4], Exp = player.ClassJobExpArray[4] },
                Bard = new Job { Level = player.ClassJobLevelArray[3], Exp = player.ClassJobExpArray[3] },
                WhiteMage = new Job { Level = player.ClassJobLevelArray[6], Exp = player.ClassJobExpArray[6] },
                BlackMage = new Job { Level = player.ClassJobLevelArray[5], Exp = player.ClassJobExpArray[5] },
                Arcanist = new Job { Level = player.ClassJobLevelArray[18], Exp = player.ClassJobExpArray[18] },
                Summoner = new Job { Level = player.ClassJobLevelArray[18], Exp = player.ClassJobExpArray[18] },
                Scholar = new Job { Level = player.ClassJobLevelArray[18], Exp = player.ClassJobExpArray[18] },
                Ninja = new Job { Level = player.ClassJobLevelArray[19], Exp = player.ClassJobExpArray[19] },
                Rogue = new Job { Level = player.ClassJobLevelArray[19], Exp = player.ClassJobExpArray[19] },
                Machinist = new Job { Level = player.ClassJobLevelArray[20], Exp = player.ClassJobExpArray[20] },
                DarkKnight = new Job { Level = player.ClassJobLevelArray[21], Exp = player.ClassJobExpArray[21] },
                Astrologian = new Job { Level = player.ClassJobLevelArray[22], Exp = player.ClassJobExpArray[22] },
                Samurai = new Job { Level = player.ClassJobLevelArray[23], Exp = player.ClassJobExpArray[23] },
                RedMage = new Job { Level = player.ClassJobLevelArray[24], Exp = player.ClassJobExpArray[24] },
                BlueMage = new Job { Level = player.ClassJobLevelArray[25], Exp = player.ClassJobExpArray[25] },
                Gunbreaker = new Job { Level = player.ClassJobLevelArray[26], Exp = player.ClassJobExpArray[26] },
                Dancer = new Job { Level = player.ClassJobLevelArray[27], Exp = player.ClassJobExpArray[27] },
                Reaper = new Job { Level = player.ClassJobLevelArray[28], Exp = player.ClassJobExpArray[28] },
                Sage = new Job { Level = player.ClassJobLevelArray[29], Exp = player.ClassJobExpArray[29] }
            };
            _localPlayer.Jobs.Rogue.Level = player.ClassJobLevelArray[19];
            _localPlayer.Jobs.Rogue.Exp = player.ClassJobExpArray[19];
            _localPlayer.Jobs.Weaver.Level = player.ClassJobLevelArray[12];
            _localPlayer.Jobs.Weaver.Exp = player.ClassJobExpArray[12];
            //player.Attributes.
            //Plugin.Log.Debug();
            /*foreach (var a in player.Attributes)
                {

                }*/
        }

        private unsafe PlayerCurrencies GetPlayerCurrencies()
        {
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            return new PlayerCurrencies
            {
                Achievement_Certificate = inventoryManager.GetInventoryItemCount((uint)Currencies.ACHIEVEMENT_CERTIFICATE),
                Allagan_Tomestone_Of_Allegory = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_ALLEGORY),
                Allagan_Tomestone_Of_Aphorism = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_APHORISM),
                Allagan_Tomestone_Of_Astronomy = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_ASTRONOMY),
                Allagan_Tomestone_Of_Causality = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_CAUSALITY),
                Allagan_Tomestone_Of_Comedy = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_COMEDY),
                Allagan_Tomestone_Of_Creation = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_CREATION),
                Allagan_Tomestone_Of_Esoterics = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_ESOTERICS),
                Allagan_Tomestone_Of_Genesis = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_GENESIS),
                Allagan_Tomestone_Of_Goetia = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_GOETIA),
                Allagan_Tomestone_Of_Law = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_LAW),
                Allagan_Tomestone_Of_Lore = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_LORE),
                Allagan_Tomestone_Of_Mendacity = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_MENDACITY),
                Allagan_Tomestone_Of_Mythology = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_MYTHOLOGY),
                Allagan_Tomestone_Of_Phantasmagoria = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_PHANTASMAGORIA),
                Allagan_Tomestone_Of_Philosophy = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_PHILOSOPHY),
                Allagan_Tomestone_Of_Poetics = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_POETICS),
                Allagan_Tomestone_Of_Revelation = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_REVELATION),
                Allagan_Tomestone_Of_Scripture = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_SCRIPTURE),
                Allagan_Tomestone_Of_Soldiery = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_SOLDIERY),
                Allagan_Tomestone_Of_Verity = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_VERITY),
                Allied_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLIED_SEAL),
                Ananta_Dreamstaff = inventoryManager.GetInventoryItemCount((uint)Currencies.ANANTA_DREAMSTAFF),
                Arkasodara_Pana = inventoryManager.GetInventoryItemCount((uint)Currencies.ARKASODARA_PANA),
                Bicolor_Gemstone = inventoryManager.GetInventoryItemCount((uint)Currencies.BICOLOR_GEMSTONE),
                Black_Copper_Gil = inventoryManager.GetInventoryItemCount((uint)Currencies.BLACK_COPPER_GIL),
                Bozjan_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.BOZJAN_CLUSTER),
                Carved_Kupo_Nut = inventoryManager.GetInventoryItemCount((uint)Currencies.CARVED_KUPO_NUT),
                Centurio_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.CENTURIO_SEAL),
                Earth_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.EARTH_CLUSTER),
                Earth_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.EARTH_CRYSTAL),
                Earth_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.EARTH_SHARD),
                Fae_Fancy = inventoryManager.GetInventoryItemCount((uint)Currencies.FAE_FANCY),
                Faux_Leaf = inventoryManager.GetInventoryItemCount((uint)Currencies.FAUX_LEAF),
                Felicitous_Token = inventoryManager.GetInventoryItemCount((uint)Currencies.FELICITOUS_TOKEN),
                Fire_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.FIRE_CLUSTER),
                Fire_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.FIRE_CRYSTAL),
                Fire_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.FIRE_SHARD),
                Flame_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.FLAME_SEAL),
                Gil = inventoryManager.GetInventoryItemCount((uint)Currencies.GIL),
                Hammered_Frogment = inventoryManager.GetInventoryItemCount((uint)Currencies.HAMMERED_FROGMENT),
                Ice_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.ICE_CLUSTER),
                Ice_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.ICE_CRYSTAL),
                Ice_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.ICE_SHARD),
                Irregular_Tomestone_Of_Creation = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_CREATION),
                Irregular_Tomestone_Of_Esoterics = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_ESOTERICS),
                Irregular_Tomestone_Of_Genesis_i = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_I),
                Irregular_Tomestone_Of_Genesis_ii = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_II),
                Irregular_Tomestone_Of_Law = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_LAW),
                Irregular_Tomestone_Of_Lore = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_LORE),
                Irregular_Tomestone_Of_Mendacity = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_MENDACITY),
                Irregular_Tomestone_Of_Mythology = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_MYTHOLOGY),
                Irregular_Tomestone_Of_Pageantry = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_PAGEANTRY),
                Irregular_Tomestone_Of_Philosophy = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_PHILOSOPHY),
                Irregular_Tomestone_Of_Scripture = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_SCRIPTURE),
                Irregular_Tomestone_Of_Soldiery = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_SOLDIERY),
                Irregular_Tomestone_Of_Tenfold_pageantry = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_TENFOLD_PAGEANTRY),
                Irregular_Tomestone_Of_Verity = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_VERITY),
                Islanders_Cowrie = inventoryManager.GetInventoryItemCount((uint)Currencies.ISLANDERS_COWRIE),
                Ixali_Oaknot = inventoryManager.GetInventoryItemCount((uint)Currencies.IXALI_OAKNOT),
                Kojin_Sango = inventoryManager.GetInventoryItemCount((uint)Currencies.KOJIN_SANGO),
                Lightning_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.LIGHTNING_CLUSTER),
                Lightning_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.LIGHTNING_CRYSTAL),
                Lightning_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.LIGHTNING_SHARD),
                Loporrit_Carat = inventoryManager.GetInventoryItemCount((uint)Currencies.LOPORRIT_CARAT),
                MGF = inventoryManager.GetInventoryItemCount((uint)Currencies.MGF),
                MGP = inventoryManager.GetInventoryItemCount((uint)Currencies.MGP),
                Namazu_Koban = inventoryManager.GetInventoryItemCount((uint)Currencies.NAMAZU_KOBAN),
                Omicron_Omnitoken = inventoryManager.GetInventoryItemCount((uint)Currencies.OMICRON_OMNITOKEN),
                Purple_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.PURPLE_CRAFTERS_SCRIP),
                Purple_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.PURPLE_GATHERERS_SCRIP),
                Qitari_Compliment = inventoryManager.GetInventoryItemCount((uint)Currencies.QITARI_COMPLIMENT),
                Rainbowtide_Psashp = inventoryManager.GetInventoryItemCount((uint)Currencies.RAINBOWTIDE_PSASHP),
                Sack_of_Nuts = inventoryManager.GetInventoryItemCount((uint)Currencies.SACK_OF_NUTS),
                Seafarers_Cowrie = inventoryManager.GetInventoryItemCount((uint)Currencies.SEAFARERS_COWRIE),
                Serpent_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.SERPENT_SEAL),
                Skybuilders_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.SKYBUILDERS_SCRIP),
                Steel_Amaljok = inventoryManager.GetInventoryItemCount((uint)Currencies.STEEL_AMALJOK),
                Storm_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.STORM_SEAL),
                Sylphic_Goldleaf = inventoryManager.GetInventoryItemCount((uint)Currencies.SYLPHIC_GOLDLEAF),
                Titan_Cobaltpiece = inventoryManager.GetInventoryItemCount((uint)Currencies.TITAN_COBALTPIECE),
                Trophy_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.TROPHY_CRYSTAL),
                Vanu_Whitebone = inventoryManager.GetInventoryItemCount((uint)Currencies.VANU_WHITEBONE),
                Venture = inventoryManager.GetInventoryItemCount((uint)Currencies.VENTURE),
                Water_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.WATER_CLUSTER),
                Water_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.WATER_CRYSTAL),
                Water_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.WATER_SHARD),
                White_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.WHITE_CRAFTERS_SCRIP),
                White_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.WHITE_GATHERERS_SCRIP),
                Wind_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.WIND_CLUSTER),
                Wind_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.WIND_CRYSTAL),
                Wind_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.WIND_SHARD),
                Wolf_Mark = inventoryManager.GetInventoryItemCount((uint)Currencies.WOLF_MARK),
                Yellow_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.YELLOW_CRAFTERS_SCRIP),
                Yellow_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.YELLOW_GATHERERS_SCRIP),
                Yo_Kai_Legendary_Jibanyan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_JIBANYAN_MEDAL),
                Yo_Kai_Legendary_Komasan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_KOMASAN_MEDAL),
                Yo_Kai_Legendary_Whisper_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_WHISPER_MEDAL),
                Yo_Kai_Legendary_Blizzaria_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_BLIZZARIA_MEDAL),
                Yo_Kai_Legendary_Kyubi_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_KYUBI_MEDAL),
                Yo_Kai_Legendary_Komajiro_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_KOMAJIRO_MEDAL),
                Yo_Kai_Legendary_Manjimutt_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_MANJIMUTT_MEDAL),
                Yo_Kai_Legendary_Noko_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_NOKO_MEDAL),
                Yo_Kai_Legendary_Venoct_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_VENOCT_MEDAL),
                Yo_Kai_Legendary_Shogunyan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_SHOGUNYAN_MEDAL),
                Yo_Kai_Legendary_Hovernyan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_HOVERNYAN_MEDAL),
                Yo_Kai_Legendary_Robonyan_f_type_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_ROBONYAN_F_TYPE_MEDAL),
                Yo_Kai_Legendary_Usapyon_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_USAPYON_MEDAL),
                Yo_Kai_Legendary_Zazel_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_ZAZEL_MEDAL),
                Yo_Kai_Legendary_lord_ananta_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_LORD_ANANTA_MEDAL),
                Yo_Kai_Legendary_Lord_enma_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_LORD_ENMA_MEDAL),
                Yo_Kai_Legendary_Damona_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_DAMONA_MEDAL),
                Yo_Kai_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_MEDAL),
            };
        }

        private void GetPlayerCompletedQuest()
        {
            foreach (int id in _questIds)
            {
                Plugin.Log.Debug($"Checking quest {id}");
                /*if(localPlayer.HasQuest(id) && localPlayer.IsQuestCompleted(id))
                    Plugin.Log.Debug($"{id} is completed");*/

                if (_localPlayer.HasQuest(id) && _localPlayer.IsQuestCompleted(id))
                {
                    continue;
                }

                Log.Debug($"Quest not in store or not completed, checking if quest {id} is completed");
                bool complete = Utils.IsQuestCompleted(id);
                if (!_localPlayer.HasQuest(id))
                {
                    _localPlayer.Quests.Add(new Quest() { Id = id, Completed = complete });
                }
                else
                {
                    Quest? q = _localPlayer.Quests.Find(q => q.Id == id);
                    if (q != null)
                    {
                        q.Completed = complete;
                    }
                }
            }
        }

        private unsafe void GetPlayerEquippedGear()
        {
            //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.EquippedItems);
            List<Gear> gearItems = [];
            //for (var i = 0; i < inv->Size; i++)
            for (int i = 0; i < inv.Size; i++)
            {
                //var ii = inv->Items[i];
                InventoryItem ii = inv.Items[i];
                InventoryItem.ItemFlags flags = ii.Flags;
                Gear currGear = new()
                {
                    ItemId = ii.ItemID,
                    HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                    CompanyCrestApplied = flags.HasFlag(InventoryItem.ItemFlags.CompanyCrestApplied),
                    Slot = ii.Slot,
                    Spiritbond = ii.Spiritbond,
                    Condition = ii.Condition,
                    CrafterContentID = ii.CrafterContentID,
                    Materia = (ushort)ii.Materia,
                    MateriaGrade = (byte)ii.MateriaGrade,
                    Stain = ii.Stain,
                    GlamourID = ii.GlamourID,
                };
                gearItems.Add(currGear);
            }
            _localPlayer.Gear = gearItems;
        }

        private unsafe void GetPlayerInventory()
        {
            InventoryType[] invs =
            [
                InventoryType.Inventory1,
                InventoryType.Inventory2,
                InventoryType.Inventory3,
                InventoryType.Inventory4,
                InventoryType.KeyItems,
                InventoryType.Crystals
            ];
            List<Inventory> items = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        Quantity = ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    items.Add(currInv);
                }
            }
            _localPlayer.Inventory = items;

            _localPlayer.Currencies = GetPlayerCurrencies();
        }
        private unsafe void GetPlayerSaddleInventory()
        {
            if (
                Condition[ConditionFlag.BoundByDuty] ||
                Condition[ConditionFlag.BoundByDuty56] ||
                Condition[ConditionFlag.BoundByDuty95]
            )
            {
                return;
            }
            InventoryType[] invs =
            [
                InventoryType.SaddleBag1,
                InventoryType.SaddleBag2,
                InventoryType.PremiumSaddleBag1,
                InventoryType.PremiumSaddleBag2
            ];
            List<Inventory> items = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        Quantity = ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    items.Add(currInv);
                }
            }
            _localPlayer.Saddle = items;
        }

        private unsafe void GetPlayerArmoryInventory()
        {
            InventoryType[] invs =
            [
                InventoryType.ArmoryMainHand,
                InventoryType.ArmoryHead,
                InventoryType.ArmoryBody,
                InventoryType.ArmoryHands,
                InventoryType.ArmoryLegs,
                InventoryType.ArmoryFeets,
                InventoryType.ArmoryOffHand,
                InventoryType.ArmoryEar,
                InventoryType.ArmoryNeck,
                InventoryType.ArmoryWrist,
                InventoryType.ArmoryRings,
                InventoryType.ArmorySoulCrystal
            ];
            List<Gear> mainHand = [];
            List<Gear> head = [];
            List<Gear> body = [];
            List<Gear> hands = [];
            List<Gear> legs = [];
            List<Gear> feets = [];
            List<Gear> oh = [];
            List<Gear> ear = [];
            List<Gear> neck = [];
            List<Gear> wrist = [];
            List<Gear> rings = [];
            List<Gear> soulCrystal = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                for (int i = 0; i < inv.Size; i++)
                {
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Gear currGear = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        CompanyCrestApplied = flags.HasFlag(InventoryItem.ItemFlags.CompanyCrestApplied),
                        Slot = ii.Slot,
                        Spiritbond = ii.Spiritbond,
                        Condition = ii.Condition,
                        CrafterContentID = ii.CrafterContentID,
                        Materia = (ushort)ii.Materia,
                        MateriaGrade = (byte)ii.MateriaGrade,
                        Stain = ii.Stain,
                        GlamourID = ii.GlamourID,
                    };

                    switch(kind)
                    {
                        case InventoryType.ArmoryMainHand: mainHand.Add(currGear);break;
                        case InventoryType.ArmoryHead: head.Add(currGear); break;
                        case InventoryType.ArmoryBody: body.Add(currGear); break;
                        case InventoryType.ArmoryHands: hands.Add(currGear); break;
                        case InventoryType.ArmoryLegs: legs.Add(currGear); break;
                        case InventoryType.ArmoryFeets: feets.Add(currGear); break;
                        case InventoryType.ArmoryOffHand: oh.Add(currGear); break;
                        case InventoryType.ArmoryEar: ear.Add(currGear); break;
                        case InventoryType.ArmoryNeck: neck.Add(currGear); break;
                        case InventoryType.ArmoryWrist: wrist.Add(currGear); break;
                        case InventoryType.ArmoryRings: rings.Add(currGear); break;
                        case InventoryType.ArmorySoulCrystal: soulCrystal.Add(currGear);break;
                    };
                }
            }

            _localPlayer.ArmoryInventory = new ArmoryGear
            {
                MainHand = mainHand,
                Head = head,
                Body = body,
                Hands = hands,
                Legs = legs,
                Feets = feets,
                OffHand = oh,
                Ear = ear,
                Neck = neck,
                Wrist = wrist,
                Rings = rings,
                SoulCrystal = soulCrystal,
            };
        }
        private unsafe void GetPlayerGlamourInventory()
        {
            
        }
        private unsafe void GetPlayerArmoireInventory()
        {
            
        }

        private unsafe void GetPlayerRetainer()
        {
            unsafe
            {
                ref readonly RetainerManager retainerManager = ref *RetainerManager.Instance();
                byte retainersCount = retainerManager.GetRetainerCount();
                if (retainersCount == 0) return;
                //Log.Debug($"Retainers count {retainersCount}");
                for (uint i = 0; i < 10; i++)
                {
                    RetainerManager.Retainer* currentRetainer = retainerManager.GetRetainerBySortedIndex(i);
                    //if (!current_retainer->Available) continue;
                    ulong retainerId = currentRetainer->RetainerID;
                    string name = MemoryHelper.ReadSeStringNullTerminated((nint)currentRetainer->Name).TextValue;

                    //Log.Debug($"current_retainer name: {name} id: {retainerId}");
                    if (name == "RETAINER") continue;

                    Retainer? r = _localPlayer.Retainers.Find(r => r.Id == retainerId);
                    r ??= new Retainer
                    {
                        Id = currentRetainer->RetainerID
                    };

                    r.Available = currentRetainer->Available;
                    r.Name = name;
                    r.ClassJob = currentRetainer->ClassJob;
                    r.Level = currentRetainer->Level;
                    r.Gils = currentRetainer->Gil;
                    r.Town = currentRetainer->Town;
                    r.MarketItemCount = currentRetainer->MarkerItemCount;// Todo: change the typo once dalamud update CS
                    r.MarketExpire = currentRetainer->MarketExpire;
                    r.VentureID = currentRetainer->VentureID;
                    r.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    r.DisplayOrder = retainerManager.DisplayOrder[i];

                    if (r.Id == retainerManager.LastSelectedRetainerId)
                    {
                        r.Inventory = GetPlayerRetainerInventory();
                        r.Gear = GetPlayerRetainerEquippedGear();
                        r.MarketInventory = GetPlayerRetainerMarketInventory();
                    }

                    if (_localPlayer.Retainers.Find(ret => ret.Id == retainerId) == null)
                    {
                        _localPlayer.Retainers.Add(r);
                    }
                }
            }
        }
        private unsafe List<Inventory> GetPlayerRetainerInventory()
        {
            InventoryType[] invs =
            [
                InventoryType.RetainerPage1,
                InventoryType.RetainerPage2,
                InventoryType.RetainerPage3,
                InventoryType.RetainerPage4,
                InventoryType.RetainerPage5,
                InventoryType.RetainerPage6,
                InventoryType.RetainerPage7,
                InventoryType.RetainerCrystals
            ];
            List<Inventory> items = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        Quantity = ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    items.Add(currInv);
                }
            }
            return items;

            /*
             * 
                RetainerMarket = 12002,
            */
        }
        
        private unsafe List<Inventory> GetPlayerRetainerMarketInventory()
        {
            unsafe
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.RetainerMarket);
                List<Inventory> marketItems = [];
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        Quantity = ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    marketItems.Add(currInv);
                }
                return marketItems;
            }
        }
        
        private unsafe List<Gear> GetPlayerRetainerEquippedGear()
        {
            unsafe
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.RetainerEquippedItems);
                List<Gear> gearItems = [];
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Gear currGear = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        CompanyCrestApplied = flags.HasFlag(InventoryItem.ItemFlags.CompanyCrestApplied),
                        Slot = ii.Slot,
                        Spiritbond = ii.Spiritbond,
                        Condition = ii.Condition,
                        CrafterContentID = ii.CrafterContentID,
                        Materia = (ushort)ii.Materia,
                        MateriaGrade = (byte)ii.MateriaGrade,
                        Stain = ii.Stain,
                        GlamourID = ii.GlamourID,
                    };
                    gearItems.Add(currGear);
                }
                return gearItems;
            }
        }

        private void CleanLastLocalCharacter()
        {
            /*localPlayer = new Character
            {
                Id = 0,
                FirstName = string.Empty,
                LastName = string.Empty,
                HomeWorld = string.Empty,
                Datacenter = string.Empty,
                Region = string.Empty,
                LastJob = 0,
                LastJobLevel = 0,
                FCTag = string.Empty,
                FreeCompany = string.Empty,
                PlayTime = 0,
                LastPlayTimeUpdate = 0,
                Attributes = null,
                Currencies = null,
                Jobs = null,
                Profile = null,
                Quests = [],
                Inventory = [],
                ArmoryInventory = [],
                Saddle = [],
                Gear = [],
                Retainers = [],
            };*/

            _localPlayer = null!;
            _localPlayerFreeCompanyTest = null;
            OtherCharacters = [];
        }

        private void OnCharacterLogin()
        {
            //Log.Info("Altoholic : OnCharacterLogin called");
            Character? p = GetCharacterFromGameOrDB();
            if (p is null)
            {
                return;
            }

            _localPlayer = p;
            //Log.Info($"Character id is : {localPlayer.Id}");
            OtherCharacters = Database.Database.GetOthersCharacters(Log, _db, _localPlayer.Id);
            //Log.Info("Altoholic : Found {0} others players", otherCharacters.Count);
            //Todo: send /playtime command
            /*_ = new XivChatEntry
            {
                Message = $"Starting altoholic timer",
                Type = XivChatType.Echo,
            };*/

            //RunInBackground(TimeSpan.FromMinutes(5));
            //RunInBackground(TimeSpan.FromSeconds(20));
        }

        /*private async void RunInBackground(TimeSpan timeSpan)
        {
            periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                Database.Database.UpdateCharacter(db, character);
                _ = new XivChatEntry
                {
                    Message = $"Tick from the time loop",
                    Type = XivChatType.Echo,
                }
                Plugin.Log.Debug("Altoholic : Tick from the time loop");
            }

            Plugin.Log.Debug($"{periodicTimer}");
        }*/

        private void OnCharacterLogout()
        {
            Log.Debug("Altoholic : OnCharacterLogout called");
            GetPlayerCompletedQuest();
            UpdateCharacter();

            //PlaytimeCommand();
            CleanLastLocalCharacter();

            MainWindow.IsOpen = false;
            RetainersWindow.IsOpen = false;
            InventoriesWindow.IsOpen = false;
            JobsWindow.IsOpen = false;
            ConfigWindow.IsOpen = false;
            CurrenciesWindow.IsOpen = false;
            DetailsWindow.IsOpen = false;
            CharactersWindow.IsOpen = false;
            _periodicTimer?.Dispose();
        }

        /*private void PlaytimeCommand()
        {
            // send playtime command after user uses btime command
            Plugin.Log.Debug($"Requesting playtime from server.");
            xivCommon.Functions.Chat.SendMessage("/playtime");
            SendChatCommand = true;
        }*/

        public void ReloadConfig()
        {
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
        }

        private long PlaytimePacket(uint param1, long param2, uint param3)
        {
            long result = _playtimeHook.Original(param1, param2, param3);
            if (param1 != 11)
                return result;

            (string, string, string) player = GetLocalPlayerNameWorldRegion();
            if (player.Item1.Length == 0)
                return result;

            Log.Debug($"Extracted Player Name: {player.Item1}{(char)SeIconChar.CrossWorld}{player.Item2}");

            uint totalPlaytime = (uint)Marshal.ReadInt32((nint)param2 + 0x10);
            Log.Debug($"Value from address {totalPlaytime}");
            TimeSpan playtime = TimeSpan.FromMinutes(totalPlaytime);
            Log.Debug($"Value from timespan {playtime}");

            string[] names = player.Item1.Split(' ');
            if (names.Length == 0)
                return result;

            _localPlayer.PlayTime = totalPlaytime;

            ulong id = ClientState.LocalContentId;

            long newPlayTimeUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _localPlayer.LastPlayTimeUpdate = newPlayTimeUpdate;
            Log.Debug($"Updating playtime with {player.Item1}, {player.Item2}, {totalPlaytime}, {newPlayTimeUpdate}.");
            Database.Database.UpdatePlaytime(Log, _db, id, totalPlaytime, newPlayTimeUpdate);
            return result;
        }

        public (string, string, string) GetLocalPlayerNameWorldRegion()
        {
            PlayerCharacter? local = ClientState.LocalPlayer;
            if (local == null || local.HomeWorld?.GameData == null)
                return (string.Empty, string.Empty, string.Empty);
            SeString? homeworld = local.HomeWorld.GameData.Name;

            return ($"{local.Name}", $"{homeworld}", $"{Utils.GetRegionFromWorld(homeworld)}");
        }

        public void UpdateCharacter()
        {
            if (_localPlayer.Id == 0 || _localPlayer.FirstName.Length == 0) return;

            Log.Debug($"Updating characters with {_localPlayer.Id} {_localPlayer.FirstName} {_localPlayer.LastName}{(char)SeIconChar.CrossWorld}{_localPlayer.HomeWorld}, {Utils.GetRegionFromWorld(_localPlayer.HomeWorld)}.");
            Database.Database.UpdateCharacter(Log, _db, _localPlayer);
        }

        public Character? GetCharacterFromGameOrDB()
        {
            (string, string, string) player = GetLocalPlayerNameWorldRegion();
            if (string.IsNullOrEmpty(player.Item1) || string.IsNullOrEmpty(player.Item2) || string.IsNullOrEmpty(player.Item3)) return null;

            //Plugin.Log.Debug($"Altoholic : Character names => 0: {player.Item1}{(char)SeIconChar.CrossWorld}{player.Item2});
            string[] names = player.Item1.Split(' '); // Todo: recheck when Dalamud API7 is out https://discord.com/channels/581875019861328007/653504487352303619/1061422333748850708
            if (names.Length == 2)
            {
                //Plugin.Log.Debug("Altoholic : Character names : 0 : {0}, 1: {1}, 2: {2}, 3: {3}", names[0], names[1], player.Item2, player.Item3);
                Character? chara = Database.Database.GetCharacter(Log, _db, ClientState.LocalContentId);
                if (chara != null)
                {
                    //Plugin.Log.Debug($"GetCharacterFromDB : id = {chara.Id}, FirstName = {chara.FirstName}, LastName = {chara.LastName}, HomeWorld = {chara.HomeWorld}, DataCenter = {chara.Datacenter}, LastJob = {chara.LastJob}, LastJobLevel = {chara.LastJobLevel}, FCTag = {chara.FCTag}, FreeCompany = {chara.FreeCompany}, LastOnline = {chara.LastOnline}, PlayTime = {chara.PlayTime}, LastPlayTimeUpdate = {chara.LastPlayTimeUpdate}");
                    return chara;
                }

                return new Character
                {
                    Id = ClientState.LocalContentId,
                    FirstName = names[0],
                    LastName = names[1],
                    HomeWorld = player.Item2,
                    CurrentWorld = player.Item2,
                    Datacenter = Utils.GetDatacenterFromWorld(player.Item2),
                    CurrentDatacenter = Utils.GetDatacenterFromWorld(player.Item2),
                    Region = player.Item3,
                    CurrentRegion = player.Item3,
                    IsSprout = _localPlayer.IsSprout,
                    LastJob = _localPlayer.LastJob,
                    LastJobLevel = _localPlayer.LastJobLevel,
                    FCTag = _localPlayer.FCTag,
                    FreeCompany = _localPlayer.FreeCompany,
                    LastOnline = _localPlayer.LastOnline,
                    PlayTime = _localPlayer.PlayTime,
                    LastPlayTimeUpdate = _localPlayer.LastPlayTimeUpdate,
                    HasPremiumSaddlebag = _localPlayer.HasPremiumSaddlebag,
                    Attributes = _localPlayer.Attributes,
                    Currencies = _localPlayer.Currencies,
                    Jobs = _localPlayer.Jobs,
                    Profile = _localPlayer.Profile,
                    Quests = _localPlayer.Quests,
                    Inventory = _localPlayer.Inventory,
                    Saddle = _localPlayer.Saddle,
                    Gear = _localPlayer.Gear,
                    Retainers = _localPlayer.Retainers,
                };
            }

            return null;
        }
        public void ChangeLanguage(ClientLanguage newLanguage)
        {
            string isoLang = newLanguage switch
            {
                ClientLanguage.German => "de",
                ClientLanguage.English => "en",
                ClientLanguage.French => "fr",
                ClientLanguage.Japanese => "ja",
                _ => "en",
            };

            _localization.SetupWithLangCode(isoLang);
        }
    }
}

