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
using ImGuiNET;
using Dalamud.Game.ClientState.Conditions;

namespace Altoholic
{
    public sealed class Plugin : IDalamudPlugin
    {
        public static string Name => "Altoholic Plugin";
        private const string CommandName = "/altoholic";
        private readonly int[] QuestIds = [65970, 66045, 66216, 66217, 66218, 66640, 66641, 66642, 66754, 66789, 66857, 66911, 66968, 66969, 66970, 67023, 67099, 67100, 67101, 67658, 67700, 67791, 67856, 68509, 68572, 68633, 68734, 68817, 69133, 69219, 69330, 69432, 70081, 70137, 70217, 69208, 67631, 69208];
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
        private Hook<PlaytimeDelegate> PlaytimeHook;

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

        private readonly Service altoholicService = null!;
        private readonly LiteDatabase db;

        private Character localPlayer = null!;
        private Utf8String? localPlayerFreeCompanyTest;

        private PeriodicTimer? periodicTimer = null;
        public List<Character> otherCharacters = [];
        private static ClientLanguage currentLocale;
        private readonly Localization Localization = new();

        public Plugin(
        /*DalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IClientState ClientState,
        IFramework framework,
        ILog Log,
        IDataManager Plugin.DataManager,
        ITextureProvider textureProvider,
        INotificationManager notificationManager*/
        )
        {
            nint playtimePtr = SigScanner.ScanText(PlaytimeSig);
            //if (playtimePtr == nint.Zero) return;
            PlaytimeHook = Hook.HookFromAddress<PlaytimeDelegate>(playtimePtr, PlaytimePacket);
            PlaytimeHook.Enable();

            var dbpath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "altoholic.db");
            /*try
            {*/

            db = new LiteDatabase(dbpath);
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

            /*PluginInterface = pluginInterface;
            this.ClientState = ClientState;
            this.framework = framework;
            this.commandManager = commandManager;
            this.Log = Log;
            this.Plugin.DataManager = Plugin.DataManager;
            this.textureProvider = textureProvider;
            this.notificationManager = notificationManager;*/

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);

            if (Configuration.Language != ClientState.ClientLanguage)
            {
                Configuration.Language = ClientState.ClientLanguage;
                Configuration.Save();
            }
            currentLocale = Configuration.Language;


            Localization.SetupWithLangCode(PluginInterface.UiLanguage);

            altoholicService = new Service(
                () => this.localPlayer,
                () => this.otherCharacters);

            ConfigWindow = new ConfigWindow(this, $"{Name} configuration");
            CharactersWindow = new CharactersWindow(this, $"{Name} characters", db)
            {
                GetPlayer = () => this.altoholicService.GetPlayer(),
                GetOthersCharactersList = () => this.altoholicService.GetOthersCharacters(),
            };

            DetailsWindow = new DetailsWindow(this, $"{Name} characters details")
            {
                GetPlayer = () => this.altoholicService.GetPlayer(),
                GetOthersCharactersList = () => this.altoholicService.GetOthersCharacters(),
            };

            JobsWindow = new JobsWindow(this, $"{Name} characters jobs")
            {
                GetPlayer = () => this.altoholicService.GetPlayer(),
                GetOthersCharactersList = () => this.altoholicService.GetOthersCharacters(),
            };

            CurrenciesWindow = new CurrenciesWindow(this, $"{Name} characters currencies")
            {
                GetPlayer = () => this.altoholicService.GetPlayer(),
                GetOthersCharactersList = () => this.altoholicService.GetOthersCharacters(),
            };

            InventoriesWindow = new InventoriesWindow(this, $"{Name} characters inventories")
            {
                GetPlayer = () => this.altoholicService.GetPlayer(),
                GetOthersCharactersList = () => this.altoholicService.GetOthersCharacters(),
            };

            RetainersWindow = new RetainersWindow(this, $"{Name} characters retainers")
            {
                GetPlayer = () => this.altoholicService.GetPlayer(),
                GetOthersCharactersList = () => this.altoholicService.GetOthersCharacters(),
            };


            MainWindow = new MainWindow(
                this,
                $"{Name} characters",
                /*Log,
                Plugin.DataManager,*/
                CharactersWindow,
                DetailsWindow,
                JobsWindow,
                CurrenciesWindow,
                InventoriesWindow,
                RetainersWindow,
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
            PluginInterface.LanguageChanged += Localization.SetupWithLangCode;

            // Todo: On retainers retainer window close
            ClientState.Login += OnCharacterLogin;
            ClientState.Logout += OnCharacterLogout;
            Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            PlaytimeHook.Dispose();
            WindowSystem.RemoveAllWindows();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            PluginInterface.UiBuilder.OpenMainUi -= DrawMainUI;
            PluginInterface.LanguageChanged -= Localization.SetupWithLangCode;

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
            db.Dispose();
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

        private void DrawUI()
        {
            WindowSystem.Draw();
        }

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
            if (localPlayer == null || localPlayer.FirstName == null)
            {
                var p = GetCharacterFromGameOrDB();
                if (p is not null)
                {
                    localPlayer = p;
                    //Plugin.Log.Debug($"localPlayerPlayTime : {localPlayerPlayTime}");
                    if (localPlayer.PlayTime == 0)//still needed?
                    {
                        Character? chara = Database.Database.GetCharacter(Log, db, localPlayer.Id);
                        if (chara is not null)
                        {
                            localPlayer.PlayTime = chara.PlayTime;
                            localPlayer.LastPlayTimeUpdate = chara.LastPlayTimeUpdate;
                        }
                    }

                    Plugin.Log.Debug($"Character localLastPlayTimeUpdate : {localPlayer.LastPlayTimeUpdate}");
#if DEBUG
                    foreach (var inventory in localPlayer.Inventory)
                    {
                        //Plugin.Log.Debug($"{inventory.ItemId} {lumina.Singular} {inventory.HQ} {inventory.Quantity}");
                    }
#endif
                }
            }
            if (localPlayer is not null)
            {
                if (otherCharacters.Count == 0)
                {
                    otherCharacters = Database.Database.GetOthersCharacters(Log, db, localPlayer.Id);
                }
                //Plugin.Log.Debug($"otherCharacters count {otherCharacters.Count}");

                //Plugin.Log.Debug($"localPlayer.Quests.Count: {localPlayer.Quests.Count}");
                if (localPlayer.Quests.Count == 0)
                {
                    //Plugin.Log.Debug("No quest found, fetching from db");
                    Character? chara = Database.Database.GetCharacter(Log, db, localPlayer.Id);
                    if (chara != null)
                    {
                        localPlayer.Quests = chara.Quests;
                    }
                }
                //Plugin.Log.Debug($"localPlayer.Quests.Count: {localPlayer.Quests.Count}");
                if (localPlayer.Retainers.Count == 0)
                {
                    Character? chara = Database.Database.GetCharacter(Log, db, localPlayer.Id);
                    if (chara != null)
                    {
                        localPlayer.Retainers = chara.Retainers;
                    }
                }
                foreach (Gear i in localPlayer.Gear)
                {
                    Plugin.Log.Debug($"Gear: {i.ItemId} {Enum.GetName(typeof(GearSlot), i.Slot)} {i.Slot}");
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
                    localPlayer.Attributes = new()
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
                    if (localPlayer.ArmoryInventory != null)
                    {
                        Log.Debug($"localPlayer.ArmoryInventory.MainHand.Count : {localPlayer.ArmoryInventory.MainHand.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Head.Count : {localPlayer.ArmoryInventory.Head.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Body.Count : {localPlayer.ArmoryInventory.Body.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Hands.Count : {localPlayer.ArmoryInventory.Hands.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Legs.Count : {localPlayer.ArmoryInventory.Legs.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Feets.Count : {localPlayer.ArmoryInventory.Feets.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.OffHand.Count : {localPlayer.ArmoryInventory.OffHand.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Ear.Count : {localPlayer.ArmoryInventory.Ear.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Neck.Count : {localPlayer.ArmoryInventory.Neck.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Wrist.Count : {localPlayer.ArmoryInventory.Wrist.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Rings.Count : {localPlayer.ArmoryInventory.Rings.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.SoulCrystal.Count : {localPlayer.ArmoryInventory.SoulCrystal.FindAll(i => i.ItemId != 0).Count}");
                        /*foreach (var inventory in localPlayer.ArmoryInventory.Hands)
                        {
                            Log.Debug($"{inventory.ItemId} {Utils.GetItemNameFromId(inventory.ItemId)} {inventory.HQ}");
                        }*/
                    }
#endif
                }

                if (ClientState.IsLoggedIn)
                {
                    localPlayer.LastOnline = 0;
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
                if (localPlayer.PlayTime == 0)
                {
                    var p = GetCharacterFromGameOrDB();
                    if (p is not null)
                    {
                        localPlayer.PlayTime = p.PlayTime;
                    }
                }
            }
            MainWindow.IsOpen = true;
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            PlayerCharacter? lPlayer = ClientState.LocalPlayer;
            if (lPlayer != null)
            {
                localPlayer ??= new()
                {
                    Id = ClientState.LocalContentId
                };

                var name = lPlayer.Name.TextValue ?? string.Empty;
                if (string.IsNullOrEmpty(name)) return;
                var names = name.Split(" ");
                if (names.Length == 2)
                {
                    localPlayer.FirstName = names[0];
                    localPlayer.LastName = names[1];
                }
                var hw = lPlayer.HomeWorld;
                if (hw != null)
                {
                    var hwgd = hw.GameData;
                    if (hwgd != null)
                    {
                        localPlayer.HomeWorld = hwgd.Name ?? string.Empty;
                        localPlayer.Datacenter = Utils.GetDatacenterFromWorld(localPlayer.HomeWorld);
                        localPlayer.Region = Utils.GetRegionFromWorld(localPlayer.HomeWorld);
                    }
                }
                var cw = lPlayer.CurrentWorld;
                if (cw != null)
                {
                    var cwhd = cw.GameData;
                    if (cwhd != null)
                    {
                        localPlayer.CurrentWorld = cwhd.Name ?? string.Empty;
                        localPlayer.CurrentDatacenter = Utils.GetDatacenterFromWorld(localPlayer.CurrentWorld);
                        localPlayer.CurrentRegion = Utils.GetRegionFromWorld(localPlayer.CurrentWorld);
                    }
                }

                localPlayer.LastJob = lPlayer.ClassJob.Id;
                localPlayer.LastJobLevel = lPlayer.Level;
                localPlayer.FCTag = lPlayer.CompanyTag.TextValue ?? string.Empty;
                //Plugin.Log.Debug($"localPlayerFreeCompanyTag : {localPlayerFreeCompanyTag}");
                //localPlayerFreeCompany = localPlayer..TextValue ?? string.Empty;
                try
                {
                    unsafe
                    {
                        ref readonly AgentInspect agentInspect = ref *AgentInspect.Instance();
                        localPlayerFreeCompanyTest = agentInspect.FreeCompany.GuildName;
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
                localPlayer.Attributes = new()
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
        }

        private unsafe void GetPlayerAttributesProfileAndJobs()
        {
            if (localPlayer is null) return;
            unsafe
            {
                var title = string.Empty;
                var prefixTitle = false;
                var raptureAtkModule = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetRaptureAtkModule();
                if (raptureAtkModule != null)
                {
                    var npi = raptureAtkModule->NamePlateInfoEntriesSpan;
                    if (npi != null)
                    {
                        for (var i = 0; i < 50 && i < raptureAtkModule->NameplateInfoCount; i++)
                        {
                            ref NamePlateInfo namePlateInfo = ref npi[i];
                            if (ClientState.LocalPlayer == null) continue;
                            if (namePlateInfo.ObjectID.ObjectID == ClientState.LocalPlayer.ObjectId)
                            {
                                var t = namePlateInfo.Title.ToString();//this sometime get player name??? to recheck
                                //Plugin.Log.Debug($"t: {t}");
                                if (t != $"{localPlayer.FirstName} {localPlayer.LastName}")
                                {
                                    title = t;
                                    prefixTitle = namePlateInfo.IsPrefixTitle;
                                }
                            }
                        }
                    }
                }
                ref readonly UIState uistate = ref *UIState.Instance();//nullcheck?
                var player = uistate.PlayerState;
                localPlayer.IsSprout = player.IsNovice();
                localPlayer.HasPremiumSaddlebag = player.HasPremiumSaddlebag;
                localPlayer.Profile = new()
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
                localPlayer.Jobs = new()
                {
                    Adventurer = new() { Level = player.ClassJobLevelArray[-1], Exp = player.ClassJobExpArray[-1] },
                    Gladiator = new() { Level = player.ClassJobLevelArray[1], Exp = player.ClassJobExpArray[1] },
                    Pugilist = new() { Level = player.ClassJobLevelArray[0], Exp = player.ClassJobExpArray[0] },
                    Marauder = new() { Level = player.ClassJobLevelArray[2], Exp = player.ClassJobExpArray[2] },
                    Lancer = new() { Level = player.ClassJobLevelArray[4], Exp = player.ClassJobExpArray[4] },
                    Archer = new() { Level = player.ClassJobLevelArray[3], Exp = player.ClassJobExpArray[3] },
                    Conjurer = new() { Level = player.ClassJobLevelArray[6], Exp = player.ClassJobExpArray[6] },
                    Thaumaturge = new() { Level = player.ClassJobLevelArray[5], Exp = player.ClassJobExpArray[5] },
                    Carpenter = new() { Level = player.ClassJobLevelArray[7], Exp = player.ClassJobExpArray[7] },
                    Blacksmith = new() { Level = player.ClassJobLevelArray[8], Exp = player.ClassJobExpArray[8] },
                    Armorer = new() { Level = player.ClassJobLevelArray[9], Exp = player.ClassJobExpArray[9] },
                    Goldsmith = new() { Level = player.ClassJobLevelArray[10], Exp = player.ClassJobExpArray[10] },
                    Leatherworker = new() { Level = player.ClassJobLevelArray[11], Exp = player.ClassJobExpArray[11] },
                    Weaver = new() { Level = player.ClassJobLevelArray[12], Exp = player.ClassJobExpArray[12] },
                    Alchemist = new() { Level = player.ClassJobLevelArray[13], Exp = player.ClassJobExpArray[13] },
                    Culinarian = new() { Level = player.ClassJobLevelArray[14], Exp = player.ClassJobExpArray[14] },
                    Miner = new() { Level = player.ClassJobLevelArray[15], Exp = player.ClassJobExpArray[15] },
                    Botanist = new() { Level = player.ClassJobLevelArray[16], Exp = player.ClassJobExpArray[16] },
                    Fisher = new() { Level = player.ClassJobLevelArray[17], Exp = player.ClassJobExpArray[17] },
                    Paladin = new() { Level = player.ClassJobLevelArray[1], Exp = player.ClassJobExpArray[1] },
                    Monk = new() { Level = player.ClassJobLevelArray[0], Exp = player.ClassJobExpArray[0] },
                    Warrior = new() { Level = player.ClassJobLevelArray[2], Exp = player.ClassJobExpArray[2] },
                    Dragoon = new() { Level = player.ClassJobLevelArray[4], Exp = player.ClassJobExpArray[4] },
                    Bard = new() { Level = player.ClassJobLevelArray[3], Exp = player.ClassJobExpArray[3] },
                    WhiteMage = new() { Level = player.ClassJobLevelArray[6], Exp = player.ClassJobExpArray[6] },
                    BlackMage = new() { Level = player.ClassJobLevelArray[5], Exp = player.ClassJobExpArray[5] },
                    Arcanist = new() { Level = player.ClassJobLevelArray[18], Exp = player.ClassJobExpArray[18] },
                    Summoner = new() { Level = player.ClassJobLevelArray[18], Exp = player.ClassJobExpArray[18] },
                    Scholar = new() { Level = player.ClassJobLevelArray[18], Exp = player.ClassJobExpArray[18] },
                    Rogue = new() { Level = player.ClassJobLevelArray[19], Exp = player.ClassJobExpArray[19] },
                    Ninja = new() { Level = player.ClassJobLevelArray[19], Exp = player.ClassJobExpArray[19] },
                    Machinist = new() { Level = player.ClassJobLevelArray[20], Exp = player.ClassJobExpArray[20] },
                    DarkKnight = new() { Level = player.ClassJobLevelArray[21], Exp = player.ClassJobExpArray[21] },
                    Astrologian = new() { Level = player.ClassJobLevelArray[22], Exp = player.ClassJobExpArray[22] },
                    Samurai = new() { Level = player.ClassJobLevelArray[23], Exp = player.ClassJobExpArray[23] },
                    RedMage = new() { Level = player.ClassJobLevelArray[24], Exp = player.ClassJobExpArray[24] },
                    BlueMage = new() { Level = player.ClassJobLevelArray[25], Exp = player.ClassJobExpArray[25] },
                    Gunbreaker = new() { Level = player.ClassJobLevelArray[26], Exp = player.ClassJobExpArray[26] },
                    Dancer = new() { Level = player.ClassJobLevelArray[27], Exp = player.ClassJobExpArray[27] },
                    Reaper = new() { Level = player.ClassJobLevelArray[28], Exp = player.ClassJobExpArray[28] },
                    Sage = new() { Level = player.ClassJobLevelArray[29], Exp = player.ClassJobExpArray[29] }
                };
                //player.Attributes.
                //Plugin.Log.Debug();
                /*foreach (var a in player.Attributes)
                {

                }*/                
            }
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
            foreach (int id in QuestIds)
            {
                Plugin.Log.Debug($"Checking quest {id}");
                /*if(localPlayer.HasQuest(id) && localPlayer.IsQuestCompleted(id))
                    Plugin.Log.Debug($"{id} is completed");*/

                if (!localPlayer.HasQuest(id) || localPlayer.HasQuest(id) && !localPlayer.IsQuestCompleted(id))
                {
                    Plugin.Log.Debug($"Quest not in store or not completed, checking if quest {id} is completed");
                    bool complete = Utils.IsQuestCompleted(id);
                    if (!localPlayer.HasQuest(id))
                    {
                        localPlayer.Quests.Add(new()
                        {
                            Id = id,
                            Completed = complete
                        });
                    }
                    else
                    {
                        var q = localPlayer.Quests.Find(q => q.Id == id);
                        if (q != null)
                        {
                            q.Completed = complete;
                        }
                    }
                }
            }
        }

        private unsafe void GetPlayerEquippedGear()
        {
            unsafe
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.EquippedItems);
                List<Gear> gear_items = [];
                //for (var i = 0; i < inv->Size; i++)
                for (var i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    var ii = inv.Items[i];
                    var flags = ii.Flags;
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
                    gear_items.Add(currGear);
                }
                localPlayer.Gear = gear_items;
            }
        }

        private unsafe void GetPlayerInventory()
        {
            unsafe
            {
                var invs = new[] {
                        InventoryType.Inventory1,
                        InventoryType.Inventory2,
                        InventoryType.Inventory3,
                        InventoryType.Inventory4,
                        InventoryType.KeyItems,
                        InventoryType.Crystals,
                    };
                List<Inventory> items = [];
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                foreach (var kind in invs)
                {
                    //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                    //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                    InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                    //for (var i = 0; i < inv->Size; i++)
                    for (var i = 0; i < inv.Size; i++)
                    {
                        //var ii = inv->Items[i];
                        var ii = inv.Items[i];
                        var flags = ii.Flags;
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
                localPlayer.Inventory = items;

                localPlayer.Currencies = GetPlayerCurrencies();
            }
        }
        private unsafe void GetPlayerSaddleInventory()
        {
            unsafe
            {
                if (
                    Condition[ConditionFlag.BoundByDuty] ||
                    Condition[ConditionFlag.BoundByDuty56] ||
                    Condition[ConditionFlag.BoundByDuty95]
                )
                {
                    return;
                }
                var invs = new[] {
                        InventoryType.SaddleBag1,
                        InventoryType.SaddleBag2,
                        InventoryType.PremiumSaddleBag1,
                        InventoryType.PremiumSaddleBag2,
                    };
                List<Inventory> items = [];
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                foreach (var kind in invs)
                {
                    //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                    //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                    InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                    //for (var i = 0; i < inv->Size; i++)
                    for (var i = 0; i < inv.Size; i++)
                    {
                        //var ii = inv->Items[i];
                        var ii = inv.Items[i];
                        var flags = ii.Flags;
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
                localPlayer.Saddle = items;
            }
        }

        private unsafe void GetPlayerArmoryInventory()
        {
            unsafe
            {
                var invs = new[] {
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
                    };
                List<Gear> MH = [];
                List<Gear> Head = [];
                List<Gear> Body = [];
                List<Gear> Hands = [];
                List<Gear> Legs = [];
                List<Gear> Feets = [];
                List<Gear> OH = [];
                List<Gear> Ear = [];
                List<Gear> Neck = [];
                List<Gear> Wrist = [];
                List<Gear> Rings = [];
                List<Gear> SC = [];
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                foreach (var kind in invs)
                {
                    InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                    for (var i = 0; i < inv.Size; i++)
                    {
                        var ii = inv.Items[i];
                        var flags = ii.Flags;
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
                            case InventoryType.ArmoryMainHand: MH.Add(currGear);break;
                            case InventoryType.ArmoryHead: Head.Add(currGear); break;
                            case InventoryType.ArmoryBody: Body.Add(currGear); break;
                            case InventoryType.ArmoryHands: Hands.Add(currGear); break;
                            case InventoryType.ArmoryLegs: Legs.Add(currGear); break;
                            case InventoryType.ArmoryFeets: Feets.Add(currGear); break;
                            case InventoryType.ArmoryOffHand: OH.Add(currGear); break;
                            case InventoryType.ArmoryEar: Ear.Add(currGear); break;
                            case InventoryType.ArmoryNeck: Neck.Add(currGear); break;
                            case InventoryType.ArmoryWrist: Wrist.Add(currGear); break;
                            case InventoryType.ArmoryRings: Rings.Add(currGear); break;
                            case InventoryType.ArmorySoulCrystal: SC.Add(currGear);break;
                            default: break;
                        };
                    }
                }

                localPlayer.ArmoryInventory = new()
                {
                    MainHand = MH,
                    Head = Head,
                    Body = Body,
                    Hands = Hands,
                    Legs = Legs,
                    Feets = Feets,
                    OffHand = OH,
                    Ear = Ear,
                    Neck = Neck,
                    Wrist = Wrist,
                    Rings = Rings,
                    SoulCrystal = SC,
                };
            }
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
                var retainersCount = retainerManager.GetRetainerCount();
                if (retainersCount == 0) return;
                //Log.Debug($"Retainers count {retainersCount}");
                for (uint i = 0; i < 10; i++)
                {
                    var current_retainer = retainerManager.GetRetainerBySortedIndex(i);
                    //if (!current_retainer->Available) continue;
                    var retainerId = current_retainer->RetainerID;
                    var name = MemoryHelper.ReadSeStringNullTerminated((nint)current_retainer->Name).TextValue;

                    //Log.Debug($"current_retainer name: {name} id: {retainerId}");
                    if (name == "RETAINER") continue;

                    Retainer? r = localPlayer.Retainers.Find(r => r.Id == retainerId);
                    r ??= new()
                    {
                        Id = current_retainer->RetainerID
                    };

                    r.Available = current_retainer->Available;
                    r.Name = name;
                    r.ClassJob = current_retainer->ClassJob;
                    r.Level = current_retainer->Level;
                    r.Gils = current_retainer->Gil;
                    r.Town = current_retainer->Town;
                    r.MarketItemCount = current_retainer->MarkerItemCount;// Todo: change the typo once dalamud update CS
                    r.MarketExpire = current_retainer->MarketExpire;
                    r.VentureID = current_retainer->VentureID;
                    r.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    r.DisplayOrder = retainerManager.DisplayOrder[i];

                    if (r.Id == retainerManager.LastSelectedRetainerId)
                    {
                        r.Inventory = GetPlayerRetainerInventory();
                        r.Gear = GetPlayerRetainerEquippedGear();
                        r.MarketInventory = GetPlayerRetainerMarketInventory();
                    }

                    if (localPlayer.Retainers.Find(r => r.Id == retainerId) == null)
                    {
                        localPlayer.Retainers.Add(r);
                    }
                }
            }
        }
        private unsafe List<Inventory> GetPlayerRetainerInventory()
        {
            unsafe
            {
                var invs = new[] {
                        InventoryType.RetainerPage1,
                        InventoryType.RetainerPage2,
                        InventoryType.RetainerPage3,
                        InventoryType.RetainerPage4,
                        InventoryType.RetainerPage5,
                        InventoryType.RetainerPage6,
                        InventoryType.RetainerPage7,
                        InventoryType.RetainerCrystals,
                    };
                List<Inventory> items = [];
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                foreach (var kind in invs)
                {
                    //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                    //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                    InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                    //for (var i = 0; i < inv->Size; i++)
                    for (var i = 0; i < inv.Size; i++)
                    {
                        //var ii = inv->Items[i];
                        var ii = inv.Items[i];
                        var flags = ii.Flags;
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
            }

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
                List<Inventory> market_items = [];
                //for (var i = 0; i < inv->Size; i++)
                for (var i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    var ii = inv.Items[i];
                    var flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemID,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HQ),
                        Quantity = ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    market_items.Add(currInv);
                }
                return market_items;
            }
        }
        
        private unsafe List<Gear> GetPlayerRetainerEquippedGear()
        {
            unsafe
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
                ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.RetainerEquippedItems);
                List<Gear> gear_items = [];
                //for (var i = 0; i < inv->Size; i++)
                for (var i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    var ii = inv.Items[i];
                    var flags = ii.Flags;
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
                    gear_items.Add(currGear);
                }
                return gear_items;
            }
        }

        private void CleanLastLocalCharacter()
        {
            localPlayer = new Character
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
                Gear = [],
                Retainers = [],
            };

            localPlayer = null!;
            localPlayerFreeCompanyTest = null;
            otherCharacters = [];
        }

        private void OnCharacterLogin()
        {
            //Log.Info("Altoholic : OnCharacterLogin called");
            var p = GetCharacterFromGameOrDB();
            if (p is not null)
            {
                localPlayer = p;
                //Log.Info($"Character id is : {localPlayer.Id}");
                otherCharacters = Database.Database.GetOthersCharacters(Log, db, localPlayer.Id);
                //Log.Info("Altoholic : Found {0} others players", otherCharacters.Count);
            }
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
            Plugin.Log.Debug("Altoholic : OnCharacterLogout called");
            GetPlayerCompletedQuest();
            UpdateCharacter();

            //PlaytimeCommand();
            CleanLastLocalCharacter();

            MainWindow.IsOpen = false;
            RetainersWindow.Dispose();
            InventoriesWindow.Dispose();
            JobsWindow.Dispose();
            ConfigWindow.Dispose();
            CurrenciesWindow.Dispose();
            DetailsWindow.Dispose();
            CharactersWindow.Dispose();
            periodicTimer?.Dispose();
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
            var result = PlaytimeHook.Original(param1, param2, param3);
            if (param1 != 11)
                return result;

            var player = GetLocalPlayerNameWorldRegion();
            if (player.Item1.Length == 0)
                return result;

            Log.Debug($"Extracted Player Name: {player.Item1}{(char)SeIconChar.CrossWorld}{player.Item2}");

            var totalPlaytime = (uint)Marshal.ReadInt32((nint)param2 + 0x10);
            Log.Debug($"Value from address {totalPlaytime}");
            var playtime = TimeSpan.FromMinutes(totalPlaytime);
            Log.Debug($"Value from timespan {playtime}");

            var names = player.Item1.Split(' ');
            if (names.Length == 0)
                return result;

            localPlayer.PlayTime = totalPlaytime;

            ulong id = ClientState.LocalContentId;

            long newPlayTimeUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            localPlayer.LastPlayTimeUpdate = newPlayTimeUpdate;
            Log.Debug($"Updating playtime with {player.Item1}, {player.Item2}, {totalPlaytime}, {newPlayTimeUpdate}.");
            Database.Database.UpdatePlaytime(Log, db, id, totalPlaytime, newPlayTimeUpdate);
            return result;
        }

        public (string, string, string) GetLocalPlayerNameWorldRegion()
        {
            PlayerCharacter? local = ClientState.LocalPlayer;
            if (local == null || local.HomeWorld == null || local.HomeWorld.GameData == null)
                return (string.Empty, string.Empty, string.Empty);
            else
            {
                var homeworld = local.HomeWorld.GameData.Name;

                return ($"{local.Name}", $"{homeworld}", $"{Utils.GetRegionFromWorld(homeworld)}");
            }
        }

        public void UpdateCharacter()
        {
            if (localPlayer == null || localPlayer.FirstName.Length == 0) return;

            Plugin.Log.Debug($"Updating characters with {localPlayer.Id} {localPlayer.FirstName} {localPlayer.LastName}{(char)SeIconChar.CrossWorld}{localPlayer.HomeWorld}, {Utils.GetRegionFromWorld(localPlayer.HomeWorld)}.");
            Database.Database.UpdateCharacter(Log, db, localPlayer);
        }

        public Character? GetCharacterFromGameOrDB()
        {
            if (ClientState is null) return null;

            var player = GetLocalPlayerNameWorldRegion();
            if (string.IsNullOrEmpty(player.Item1) || string.IsNullOrEmpty(player.Item2) || string.IsNullOrEmpty(player.Item3)) return null;

            //Plugin.Log.Debug($"Altoholic : Character names => 0: {player.Item1}{(char)SeIconChar.CrossWorld}{player.Item2});
            var names = player.Item1.Split(' '); // Todo: recheck when Dalamud API7 is out https://discord.com/channels/581875019861328007/653504487352303619/1061422333748850708
            if (names.Length == 2)
            {
                //Plugin.Log.Debug("Altoholic : Character names : 0 : {0}, 1: {1}, 2: {2}, 3: {3}", names[0], names[1], player.Item2, player.Item3);
                Character? chara = Database.Database.GetCharacter(Log, db, ClientState.LocalContentId);
                if (chara != null)
                {
                    //Plugin.Log.Debug($"GetCharacterFromDB : id = {chara.Id}, FirstName = {chara.FirstName}, LastName = {chara.LastName}, HomeWorld = {chara.HomeWorld}, DataCenter = {chara.Datacenter}, LastJob = {chara.LastJob}, LastJobLevel = {chara.LastJobLevel}, FCTag = {chara.FCTag}, FreeCompany = {chara.FreeCompany}, LastOnline = {chara.LastOnline}, PlayTime = {chara.PlayTime}, LastPlayTimeUpdate = {chara.LastPlayTimeUpdate}");
                    return chara;
                }
                else
                {
                    return new()
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
                        IsSprout = localPlayer.IsSprout,
                        LastJob = localPlayer.LastJob,
                        LastJobLevel = localPlayer.LastJobLevel,
                        FCTag = localPlayer.FCTag,
                        FreeCompany = localPlayer.FreeCompany,
                        LastOnline = localPlayer.LastOnline,
                        PlayTime = localPlayer.PlayTime,
                        LastPlayTimeUpdate = localPlayer.LastPlayTimeUpdate,
                        HasPremiumSaddlebag = localPlayer.HasPremiumSaddlebag,
                        Attributes = localPlayer.Attributes,
                        Currencies = localPlayer.Currencies,
                        Jobs = localPlayer.Jobs,
                        Profile = localPlayer.Profile,
                        Quests = localPlayer.Quests,
                        Inventory = localPlayer.Inventory,
                        Saddle = localPlayer.Saddle,
                        Gear = localPlayer.Gear,
                        Retainers = localPlayer.Retainers,
                    };
                }
            }
            else
            {
                return null;
            }
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

            Localization.SetupWithLangCode(isoLang);
        }
    }
}

