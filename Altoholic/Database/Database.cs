using System;
using System.Collections.Generic;
using System.IO;
using Altoholic.Models;
using Dapper;
using LiteDB;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Linq;

namespace Altoholic.Database
{
    public class Database
    {
        private const string CharacterTableName = "characters";
        private const string CharactersCurrenciesHistoryTableName = "charactersCurrenciesHistories";
        private const string BlacklistTableName = "blacklist";

        public static bool IsSqLiteDatabase(string pathToFile)
        {
            bool result = false;

            if (!File.Exists(pathToFile))
            {
                return result;
            }

            using FileStream stream = new(pathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] header = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                header[i] = (byte)stream.ReadByte();
            }

            result = System.Text.Encoding.UTF8.GetString(header).Contains("SQLite format 3");

            stream.Close();

            return result;
        }

        public static SqliteConnection CreateDatabaseConnection(string dbPath)
        {
            return new SqliteConnection("Data Source=" + dbPath);
        }

        private static bool DoesTableExist(SqliteConnection db, string tableName)
        {
            Plugin.Log.Debug($"DoesTableExist: {tableName}");
            string sql = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}';";
            string? name = db.QueryFirstOrDefault<string>(sql);
            Plugin.Log.Debug($"DoesTableExist returned name: {name}");
            Plugin.Log.Debug($"DoesTableExist? : {(name != null && name == tableName)}");
            return (name != null && name == tableName);
        }
        /*private static bool DoesColumnExist(SqliteConnection db, string tableName, string columnName)
        {
            Plugin.Log.Debug($"DoesColumnExist: {tableName}, {columnName}");
            return db.GetSchema("Columns").Select($"COLUMN_NAME='{columnName}' AND TABLE_NAME='{tableName}'").Length != 0;
        }*/
        /// <summary>
        /// Checks if the given table contains a column with the given name.
        /// </summary>
        /// <param name="tableName">The table in this database to check.</param>
        /// <param name="columnName">The column in the given table to look for.</param>
        /// <param name="connection">The SQLiteConnection for this database.</param>
        /// <returns>True if the given table contains a column with the given name.</returns>
        public static bool DoesColumnExist(SqliteConnection connection, string tableName, string columnName)
        {
            Plugin.Log.Debug($"Entering DoesColumnExist: {tableName}, {columnName}");
            IDataReader dr = connection.ExecuteReader("PRAGMA table_info(" + tableName + ")");
            while (dr.Read())//loop through the various columns and their info
            {
                object value = dr.GetValue(1);//column 1 from the result contains the column names
                if (!columnName.Equals(value))
                {
                    continue;
                }

                dr.Close();
                Plugin.Log.Debug($"DoesColumnExist: {tableName}, {columnName} yes");
                return true;
            }

            dr.Close();
            Plugin.Log.Debug($"DoesColumnExist: {tableName}, {columnName} no");
            return false;
        }

        public static void CheckOrCreateDatabases(SqliteConnection db)
        {
            if (!DoesTableExist(db, CharacterTableName))
            {
                const string sql = $"""
                                   CREATE TABLE IF NOT EXISTS {CharacterTableName} (
                                                   CharacterId BIGINT PRIMARY KEY,
                                                   FirstName NVARCHAR(255),
                                                   LastName NVARCHAR(255),
                                                   HomeWorld NVARCHAR(255),
                                                   CurrentWorld NVARCHAR(255),
                                                   Datacenter NVARCHAR(255),
                                                   CurrentDatacenter NVARCHAR(255),
                                                   Region NVARCHAR(255),
                                                   CurrentRegion NVARCHAR(255),
                                                   IsSprout BIT,
                                                   IsBattleMentor BIT,
                                                   IsTradeMentor BIT,
                                                   IsReturner BIT,
                                                   LastJob INT,
                                                   LastJobLevel INT,
                                                   FCTag NVARCHAR(255),
                                                   FreeCompany NVARCHAR(255),
                                                   LastOnline BIGINT,
                                                   PlayTime INT,
                                                   LastPlayTimeUpdate BIGINT,
                                                   HasPremiumSaddlebag BIT,
                                                   PlayerCommendations SMALLINT,
                                                   CurrentFacewear TEXT,
                                                   CurrentOrnament SMALLINT,
                                                   Attributes TEXT,
                                                   Currencies TEXT,
                                                   Jobs TEXT,
                                                   Profile TEXT,
                                                   Quests TEXT,
                                                   Inventory TEXT,
                                                   ArmoryInventory TEXT,
                                                   Saddle TEXT,
                                                   Gear TEXT,
                                                   Retainers TEXT,
                                                   Minions TEXT,
                                                   Mounts TEXT,
                                                   TripleTriadCards TEXT,
                                                   Emotes TEXT,
                                                   Bardings TEXT,
                                                   FramerKits TEXT,
                                                   OrchestrionRolls TEXT,
                                                   Ornaments TEXT,
                                                   Glasses TEXT,
                                                   BeastReputations TEXT,
                                                   Duties TEXT,
                                                   DutiesUnlocked TEXT
                                               );
                                   """;
                int result = db.Execute(sql);
                Plugin.Log.Debug($"CREATE TABLE {CharacterTableName} result: {result}");
                if (result == 0)
                {
                    int result2 = db.Execute($"CREATE INDEX idx_{CharacterTableName}_CharacterID ON {CharacterTableName}(CharacterId)");
                    Plugin.Log.Debug($"CREATE INDEX idx_{CharacterTableName}_CharacterID ON {CharacterTableName}(CharacterId) result: {result2}");
                }
            }
            else
            {
                if (!DoesColumnExist(db, CharacterTableName, "Duties"))
                {
                    Plugin.Log.Debug($"Column {CharacterTableName}.Duties does not exist");
                    const string sql11 = $"ALTER TABLE {CharacterTableName} ADD COLUMN Duties TEXT";
                    int result11 = db.Execute(sql11);
                    Plugin.Log.Debug($"ALTER TABLE {CharacterTableName} ADD COLUMN Duties TEXT result: {result11}");
                }
                if (!DoesColumnExist(db, CharacterTableName, "DutiesUnlocked"))
                {
                    Plugin.Log.Debug($"Column {CharacterTableName}.DutiesUnlocked does not exist");
                    const string sql12 = $"ALTER TABLE {CharacterTableName} ADD COLUMN DutiesUnlocked TEXT";
                    int result12 = db.Execute(sql12);
                    Plugin.Log.Debug($"ALTER TABLE {CharacterTableName} ADD COLUMN DutiesUnlocked TEXT result: {result12}");
                }

                if (DoesColumnExist(db, CharacterTableName, "CurrentFawear"))
                {
                    Plugin.Log.Debug($"Column {CharacterTableName}.CurrentFawear exist");
                    const string sql131 = $"ALTER TABLE {CharacterTableName} DROP COLUMN CurrentFawear";
                    int result131 = db.Execute(sql131);
                    Plugin.Log.Debug($"ALTER TABLE {CharacterTableName} DROP COLUMN CurrentFawear result: {result131}");
                }
                if (!DoesColumnExist(db, CharacterTableName, "CurrentFacewear"))
                {
                    Plugin.Log.Debug($"Column {CharacterTableName}.CurrentFacewear does not exist");
                    const string sql13 = $"ALTER TABLE {CharacterTableName} ADD COLUMN CurrentFacewear TEXT";
                    int result13 = db.Execute(sql13);
                    Plugin.Log.Debug($"ALTER TABLE {CharacterTableName} ADD COLUMN CurrentFacewear TEXT result: {result13}");
                }
                if (!DoesColumnExist(db, CharacterTableName, "CurrentOrnament"))
                {
                    Plugin.Log.Debug($"Column {CharacterTableName}.CurrentOrnament does not exist");
                    const string sql14 = $"ALTER TABLE {CharacterTableName} ADD COLUMN CurrentOrnament SMALLINT";
                    int result14 = db.Execute(sql14);
                    Plugin.Log.Debug($"ALTER TABLE {CharacterTableName} ADD COLUMN CurrentOrnament TEXT result: {result14}");
                }
            }

            if(!DoesTableExist(db, CharactersCurrenciesHistoryTableName))
            {
                const string sql2 = $"""
                                        CREATE TABLE IF NOT EXISTS {CharactersCurrenciesHistoryTableName}(
                                            Id BIGINT PRIMARY KEY,
                                            CharacterId BIGINT,
                                            Currencies TEXT,
                                            Datetime BIGINT
                                       );
                                     """;
                int result2 = db.Execute(sql2);
                Plugin.Log.Debug($"CREATE TABLE {CharactersCurrenciesHistoryTableName} result: {result2}");

                if (result2 == 0)
                {
                    int result22 = db.Execute($"CREATE INDEX idx_{CharactersCurrenciesHistoryTableName}_CharacterID ON {CharactersCurrenciesHistoryTableName}(CharacterId)");
                    Plugin.Log.Debug($"CREATE INDEX idx_{CharactersCurrenciesHistoryTableName}_CharacterID ON {CharactersCurrenciesHistoryTableName}(CharacterId) result: {result22}");
                }
            }

            if (DoesTableExist(db, BlacklistTableName))
            {
                return;
            }

            const string sql3 = $"""
                                    CREATE TABLE IF NOT EXISTS {BlacklistTableName}(
                                        CharacterId BIGINT PRIMARY KEY,
                                        Datetime BIGINT
                                   );
                                 """;
            int result3 = db.Execute(sql3);
            Plugin.Log.Debug($"CREATE TABLE {BlacklistTableName} result: {result3}");
            if (result3 == 0)
            {
                int result32 = db.Execute($"CREATE INDEX idx_{BlacklistTableName}_CharacterID ON {BlacklistTableName}(CharacterId)");
                Plugin.Log.Debug($"CREATE INDEX idx_{BlacklistTableName}_CharacterID ON {BlacklistTableName}(CharacterId) result: {result32}");
            }
        }

        public static List<Character> GetDataFromLite(LiteDatabase db)
        {
            List<Character> characters = [];
            ILiteCollection<LiteDBCharacter>? col = db.GetCollection<LiteDBCharacter>("Character");
            if (col == null)
            {
                Plugin.Log.Debug("GetDataFromLite col is null");
                return [];
            }
            Plugin.Log.Debug("GetDataFromLite col is not null");

            using IEnumerator<LiteDBCharacter> liteDbCharactersEnumerator = col.FindAll().GetEnumerator();
            int count = 0;
            while (liteDbCharactersEnumerator.MoveNext())
            {
                count++;
                LiteDBCharacter ldbc = liteDbCharactersEnumerator.Current;
                Character character = new()
                {
                    CharacterId = ldbc.Id,
                    FirstName = ldbc.FirstName,
                    LastName = ldbc.LastName,
                    HomeWorld = ldbc.HomeWorld,
                    CurrentWorld = ldbc.CurrentWorld,
                    Datacenter = ldbc.Datacenter,
                    CurrentDatacenter = ldbc.CurrentDatacenter,
                    Region = ldbc.Region,
                    CurrentRegion = ldbc.CurrentRegion,
                    IsSprout = ldbc.IsSprout,
                    IsBattleMentor = ldbc.IsBattleMentor,
                    IsTradeMentor = ldbc.IsTradeMentor,
                    IsReturner = ldbc.IsReturner,
                    LastJob = ldbc.LastJob,
                    LastJobLevel = ldbc.LastJobLevel,
                    FCTag = ldbc.FCTag,
                    FreeCompany = ldbc.FreeCompany,
                    LastOnline = ldbc.LastOnline,
                    PlayTime = ldbc.PlayTime,
                    LastPlayTimeUpdate = ldbc.LastPlayTimeUpdate,
                    HasPremiumSaddlebag = ldbc.HasPremiumSaddlebag,
                    PlayerCommendations = ldbc.PlayerCommendations,
                    Attributes = ldbc.Attributes,
                    Currencies = ldbc.Currencies,
                    Jobs = ldbc.Jobs,
                    Profile = ldbc.Profile,
                    Quests = [..ldbc.Quests.Select(q => q.Id)],
                    Inventory = ldbc.Inventory,
                    ArmoryInventory = ldbc.ArmoryInventory,
                    Saddle = ldbc.Saddle,
                    Gear = ldbc.Gear,
                    Retainers = ldbc.Retainers,
                    Minions = [..ldbc.Minions],
                    Mounts = [..ldbc.Mounts],
                    TripleTriadCards = [..ldbc.TripleTriadCards],
                    Emotes = [..ldbc.Emotes],
                    Bardings = [..ldbc.Bardings],
                    FramerKits = [..ldbc.FramerKits],
                    OrchestrionRolls = [..ldbc.OrchestrionRolls],
                    Ornaments = [..ldbc.Ornaments],
                    Glasses = [..ldbc.Glasses],
                    BeastReputations = ldbc.BeastReputations,
                    CurrenciesHistory = db.GetCollection<CurrenciesHistory>().Find(c => c.CharacterId == ldbc.Id).AsList()
                };
                characters.Add(character);
            }
            Plugin.Log.Debug($"LiteDB has found {count} characters");
            return characters;
        }

        public static Character? GetCharacter(SqliteConnection db, ulong id)
        {
            Plugin.Log.Debug($"Database/GetCharacter entered with id = {id}");
            try
            {
                Blacklist? b = GetBlacklist(db, id);
                if (b != null) return null;

                const string sql = $"SELECT * FROM {CharacterTableName} WHERE CharacterId = @id";
                DatabaseCharacter? dbCharacter = db.QueryFirstOrDefault<DatabaseCharacter>(sql, new { id });
                if (dbCharacter == null) return null;

                Character character = FormatDatabaseCharacterFromDatabase(dbCharacter);
                character.CurrenciesHistory = GetCharacterCurrenciesHistories(db, id);
                return character;
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Plugin.Log.Error(ex.ToString());
                return null;
            }
        }

        public class DatabaseCurrenciesHistory
        {
            public ulong CharacterId { get; init; }
            public long Datetime { get; init; }
            public string Currencies { get; init; } = string.Empty;
        }

        private static List<CurrenciesHistory> GetCharacterCurrenciesHistories(SqliteConnection db, ulong id)
        {
            List<CurrenciesHistory> currenciesHistories = [];
            const string sql2 = $"SELECT * FROM {CharactersCurrenciesHistoryTableName} WHERE CharacterId = @id";
            using IEnumerator<DatabaseCurrenciesHistory> currenciesHistoryEnumerator = db.Query<DatabaseCurrenciesHistory>(sql2, new { id }).GetEnumerator();
            while (currenciesHistoryEnumerator.MoveNext())
            {
                DatabaseCurrenciesHistory dch = currenciesHistoryEnumerator.Current;
                PlayerCurrencies? currencies = System.Text.Json.JsonSerializer.Deserialize<PlayerCurrencies>(dch.Currencies);
                if (currencies == null) continue;
                currenciesHistories.Add(new CurrenciesHistory()
                {
                    CharacterId = dch.CharacterId,
                    Datetime = dch.Datetime,
                    Currencies = currencies
                });
            }
            return currenciesHistories;
        }

        public static List<Character> GetOthersCharacters(SqliteConnection db, ulong id)
        {
            try
            {
                const string sql = $"SELECT * FROM {CharacterTableName} WHERE CharacterId != @id AND CharacterId NOT IN (SELECT CharacterId FROM {BlacklistTableName})";
                IEnumerable<DatabaseCharacter> dbCharacters = db.Query<DatabaseCharacter>(sql, new { id });
                List<Character> cList = [];
                foreach (DatabaseCharacter dbCharacter in dbCharacters)
                {
                    Character character = FormatDatabaseCharacterFromDatabase(dbCharacter);
                    character.CurrenciesHistory = GetCharacterCurrenciesHistories(db, id);

                    cList.Add(character);
                }

                return cList;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return [];
            }
        }

        public static int DeleteCharacter(SqliteConnection db, ulong id)
        {
            Plugin.Log.Debug($"Database/DeleteCharacter entered with params: db = {db}, id = {id}");
            try
            {
                const string sql = $"DELETE FROM {CharacterTableName} WHERE CharacterId = @id";
                int result = db.Execute(sql, new { id });
                Plugin.Log.Debug($"Database/DeleteCharacter DeleteCharacter result: {result}");
                if (result == 1)
                {
                    int result2 = DeleteCharacterCurrenciesHistories(db, id);
                }

                return result;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return 0;
            }
        }

        public static int DeleteCharacterCurrenciesHistories(SqliteConnection db, ulong id)
        {
            Plugin.Log.Debug($"Entering DeleteCharacterCurrenciesHistories with character id = {id}");
            const string sql = $"DELETE FROM {CharactersCurrenciesHistoryTableName} WHERE characterId = @id";
            int result = db.Execute(sql, new { id });
            Plugin.Log.Debug($"DeleteCharacterCurrenciesHistories result: {result}");
            return result;
        }

        public static int UpdateCharacter(SqliteConnection db, Character character)
        {
            Plugin.Log.Debug($"Entering UpdateCharacter with character : id = {character.CharacterId}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}");
            if (character.CharacterId == 0) return 0;

            try
            {
                const string updateSql = $"UPDATE {CharacterTableName} SET [FirstName] = @FirstName, [LastName] = @LastName, [HomeWorld] = @HomeWorld, [Datacenter] = @Datacenter, [Region] = @Region, [IsSprout] = @IsSprout, [IsBattleMentor] = @IsBattleMentor, [IsTradeMentor] = @IsTradeMentor, [IsReturner] = @IsReturner, [LastJob] = @LastJob, [LastJobLevel] = @LastJobLevel, [FCTag] = @FCTag, [FreeCompany] = @FreeCompany, [LastOnline] = @LastOnline, [PlayTime] = @PlayTime, [LastPlayTimeUpdate] = @LastPlayTimeUpdate, [HasPremiumSaddlebag] = @HasPremiumSaddlebag, [PlayerCommendations] = @PlayerCommendations, [CurrentFacewear] = @CurrentFacewear, [CurrentOrnament] = @CurrentOrnament, [Attributes] = @Attributes, [Currencies] = @Currencies, [Jobs] = @Jobs, [Profile] = @Profile, [Quests] = @Quests, [Inventory] = @Inventory, [ArmoryInventory] = @ArmoryInventory, [Saddle] = @Saddle, [Gear] = @Gear, [Retainers] = @Retainers, [Minions] = @Minions, [Mounts] = @Mounts, [TripleTriadCards] = @TripleTriadCards, [Emotes] = @Emotes, [Bardings] = @Bardings, [FramerKits] = @FramerKits, [OrchestrionRolls] = @OrchestrionRolls, [Ornaments] = @Ornaments, [Glasses] = @Glasses, [BeastReputations] = @BeastReputations, [Duties] = @Duties, [DutiesUnlocked] = @DutiesUnlocked WHERE [CharacterId] = @CharacterId";
                int result = db.Execute(updateSql, FormatCharacterForDatabase(character));
                return result;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return 0;
            }
        }
        public static int UpdatePlaytime(SqliteConnection db, ulong id, uint playTime, long playTimeUpdate)
        {
            if (playTime == 0) return 0;
            Plugin.Log.Debug($"UpdatePlayTime {id} {playTime} {playTimeUpdate}");
            try
            {
                Character? c = GetCharacter(db, id);
                if (c == null) return 0;

                const string updateSql = $"UPDATE {CharacterTableName} SET [PlayTime] = @PlayTime, [LastPlayTimeUpdate] = @LastPlayTimeUpdate WHERE [CharacterId] = @CharacterId";
                int result = db.Execute(updateSql, new
                {
                    PlayTime = playTime,
                    LastPlayTimeUpdate = playTimeUpdate,
                    CharacterId = id
                });
                return result;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return 0; }
        }

        public static Character FormatDatabaseCharacterFromDatabase(DatabaseCharacter databaseCharacter)
        {
            ushort[] currentFacewear = string.IsNullOrEmpty(databaseCharacter.CurrentFacewear)
                ? [0, 0]
                : System.Text.Json.JsonSerializer.Deserialize<ushort[]>(databaseCharacter.CurrentFacewear) ?? [0,0];
            Attributes? attributes = string.IsNullOrEmpty(databaseCharacter.Attributes)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<Attributes>(databaseCharacter.Attributes);
            Plugin.Log.Debug("Attribes deserialized");
            PlayerCurrencies? currencies = string.IsNullOrEmpty(databaseCharacter.Currencies)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<PlayerCurrencies>(databaseCharacter.Currencies);
            Plugin.Log.Debug("Currencies deserialized");
            Jobs? jobs = string.IsNullOrEmpty(databaseCharacter.Jobs)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<Jobs>(databaseCharacter.Jobs);
            Plugin.Log.Debug("Jobs deserialized");
            Profile? profile = string.IsNullOrEmpty(databaseCharacter.Profile)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<Profile>(databaseCharacter.Profile);
            Plugin.Log.Debug("Profile deserialized");
            List<int> quests = string.IsNullOrEmpty(databaseCharacter.Quests)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(databaseCharacter.Quests) ?? [];
            Plugin.Log.Debug("Quests deserialized");
            List<Inventory> inventory = string.IsNullOrEmpty(databaseCharacter.Inventory)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<Inventory>>(databaseCharacter.Inventory) ?? [];
            Plugin.Log.Debug("Inventory deserialized");
            ArmoryGear? armoryInventory = string.IsNullOrEmpty(databaseCharacter.ArmoryInventory)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<ArmoryGear>(databaseCharacter.ArmoryInventory);
            Plugin.Log.Debug("ArmoryInventory deserialized");
            List<Inventory> saddle = string.IsNullOrEmpty(databaseCharacter.Saddle)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<Inventory>>(databaseCharacter.Saddle) ?? [];
            Plugin.Log.Debug("Saddle deserialized");
            List<Gear> gear = string.IsNullOrEmpty(databaseCharacter.Gear)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<Gear>>(databaseCharacter.Gear) ?? [];
            Plugin.Log.Debug("Gear deserialized");
            List<Retainer> retainers = string.IsNullOrEmpty(databaseCharacter.Retainers)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<Retainer>>(databaseCharacter.Retainers) ?? [];
            Plugin.Log.Debug("Retainers deserialized");
            List<uint> minions = string.IsNullOrEmpty(databaseCharacter.Minions)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Minions) ?? [];
            Plugin.Log.Debug("Minions deserialized");
            List<uint> mounts = string.IsNullOrEmpty(databaseCharacter.Mounts)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Mounts) ?? [];
            Plugin.Log.Debug("Mounts deserialized");
            List<uint> tripleTriadCards = string.IsNullOrEmpty(databaseCharacter.TripleTriadCards)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.TripleTriadCards) ?? [];
            Plugin.Log.Debug("TripleTriadCards deserialized");
            List<uint> emotes = string.IsNullOrEmpty(databaseCharacter.Emotes)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Emotes) ?? [];
            Plugin.Log.Debug("Emotes deserialized");
            List<uint> bardings = (string.IsNullOrEmpty(databaseCharacter.Bardings))
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Bardings) ?? [];
            Plugin.Log.Debug("Bardings deserialized");
            List<uint> framerkits = string.IsNullOrEmpty(databaseCharacter.FramerKits)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.FramerKits) ?? [];
            Plugin.Log.Debug("FramerKits deserialized");
            List<uint> orchestrionRolls = string.IsNullOrEmpty(databaseCharacter.OrchestrionRolls)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.OrchestrionRolls) ?? [];
            Plugin.Log.Debug("OrchestrionRolls deserialized");
            List<uint> ornaments = string.IsNullOrEmpty(databaseCharacter.Ornaments)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Ornaments) ?? [];
            Plugin.Log.Debug("Ornaments deserialized");
            List<uint> glasses = string.IsNullOrEmpty(databaseCharacter.Glasses)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Glasses) ?? [];
            Plugin.Log.Debug("Glasses deserialized");
            List<BeastTribeRank> beastReputations = string.IsNullOrEmpty(databaseCharacter.BeastReputations)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<BeastTribeRank>>(databaseCharacter
                    .BeastReputations) ?? [];
            Plugin.Log.Debug("BeastReputations deserialized");
            List<uint> duties = string.IsNullOrEmpty(databaseCharacter.Duties)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.Duties) ?? [];
            Plugin.Log.Debug("Duties deserialized");
            List<uint> dutiesUnlocked = string.IsNullOrEmpty(databaseCharacter.DutiesUnlocked)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<uint>>(databaseCharacter.DutiesUnlocked) ?? [];
            Plugin.Log.Debug("DutiesUnlocked deserialized");

            return new Character()
            {
                CharacterId = databaseCharacter.CharacterId,
                FirstName = databaseCharacter.FirstName,
                LastName = databaseCharacter.LastName,
                HomeWorld = databaseCharacter.HomeWorld,
                CurrentWorld = databaseCharacter.CurrentWorld,
                Datacenter = databaseCharacter.Datacenter,
                CurrentDatacenter = databaseCharacter.CurrentDatacenter,
                Region = databaseCharacter.Region,
                CurrentRegion = databaseCharacter.CurrentRegion,
                IsSprout = databaseCharacter.IsSprout,
                IsBattleMentor = databaseCharacter.IsBattleMentor,
                IsTradeMentor = databaseCharacter.IsTradeMentor,
                IsReturner = databaseCharacter.IsReturner,
                LastJob = databaseCharacter.LastJob,
                LastJobLevel = databaseCharacter.LastJobLevel,
                FCTag = databaseCharacter.FCTag,
                FreeCompany = databaseCharacter.FreeCompany,
                LastOnline = databaseCharacter.LastOnline,
                PlayTime = databaseCharacter.PlayTime,
                LastPlayTimeUpdate = databaseCharacter.LastPlayTimeUpdate,
                HasPremiumSaddlebag = databaseCharacter.HasPremiumSaddlebag,
                PlayerCommendations = databaseCharacter.PlayerCommendations,
                CurrentFacewear = currentFacewear,
                CurrentOrnament = databaseCharacter.CurrentOrnament,
                Attributes = attributes,
                Currencies = currencies,
                Jobs = jobs,
                Profile = profile,
                Quests = [..quests],
                Inventory = inventory,
                ArmoryInventory = armoryInventory,
                Saddle = saddle,
                Gear = gear,
                Retainers = retainers,
                Minions = [..minions],
                Mounts = [.. mounts],
                TripleTriadCards = [.. tripleTriadCards],
                Emotes = [.. emotes],
                Bardings = [.. bardings],
                FramerKits = [.. framerkits],
                OrchestrionRolls = [.. orchestrionRolls],
                Ornaments = [.. ornaments],
                Glasses = [.. glasses],
                BeastReputations = beastReputations,
                Duties = [..duties],
                DutiesUnlocked = [.. dutiesUnlocked]
            };
        }

        public static DatabaseCharacter FormatCharacterForDatabase(Character character)
        {
            string currentFacewear = System.Text.Json.JsonSerializer.Serialize(character.CurrentFacewear);
            string attributes = System.Text.Json.JsonSerializer.Serialize(character.Attributes);
            string currencies = System.Text.Json.JsonSerializer.Serialize(character.Currencies);
            string jobs = System.Text.Json.JsonSerializer.Serialize(character.Jobs);
            string profile = System.Text.Json.JsonSerializer.Serialize(character.Profile);
            string quests = System.Text.Json.JsonSerializer.Serialize(character.Quests);
            string inventory = System.Text.Json.JsonSerializer.Serialize(character.Inventory);
            string armoryInventory = System.Text.Json.JsonSerializer.Serialize(character.ArmoryInventory);
            string saddle = System.Text.Json.JsonSerializer.Serialize(character.Saddle);
            string gear = System.Text.Json.JsonSerializer.Serialize(character.Gear);
            string retainers = System.Text.Json.JsonSerializer.Serialize(character.Retainers);
            string minions = System.Text.Json.JsonSerializer.Serialize(character.Minions);
            string mounts = System.Text.Json.JsonSerializer.Serialize(character.Mounts);
            string tripleTriadCards = System.Text.Json.JsonSerializer.Serialize(character.TripleTriadCards);
            string emotes = System.Text.Json.JsonSerializer.Serialize(character.Emotes);
            string bardings = System.Text.Json.JsonSerializer.Serialize(character.Bardings);
            string framerkits = System.Text.Json.JsonSerializer.Serialize(character.FramerKits);
            string orchestrionRolls = System.Text.Json.JsonSerializer.Serialize(character.OrchestrionRolls);
            string ornaments = System.Text.Json.JsonSerializer.Serialize(character.Ornaments);
            string glasses = System.Text.Json.JsonSerializer.Serialize(character.Glasses);
            string beastReputations = System.Text.Json.JsonSerializer.Serialize(character.BeastReputations);
            string duties = System.Text.Json.JsonSerializer.Serialize(character.Duties);
            string dutiesUnlocked = System.Text.Json.JsonSerializer.Serialize(character.DutiesUnlocked);

            return new DatabaseCharacter()
            {
                CharacterId = character.CharacterId,
                FirstName = character.FirstName,
                LastName = character.LastName,
                HomeWorld = character.HomeWorld,
                CurrentWorld = character.CurrentWorld,
                Datacenter = character.Datacenter,
                CurrentDatacenter = character.CurrentDatacenter,
                Region = character.Region,
                CurrentRegion = character.CurrentRegion,
                IsSprout = character.IsSprout,
                IsBattleMentor = character.IsBattleMentor,
                IsTradeMentor = character.IsTradeMentor,
                IsReturner = character.IsReturner,
                LastJob = character.LastJob,
                LastJobLevel = character.LastJobLevel,
                FCTag = character.FCTag,
                FreeCompany = character.FreeCompany,
                LastOnline = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                PlayTime = character.PlayTime,
                LastPlayTimeUpdate = character.LastPlayTimeUpdate,
                HasPremiumSaddlebag = character.HasPremiumSaddlebag,
                PlayerCommendations = character.PlayerCommendations,
                CurrentFacewear = currentFacewear,
                CurrentOrnament = character.CurrentOrnament,
                Attributes = attributes,
                Currencies = currencies,
                Jobs = jobs,
                Profile = profile,
                Quests = quests,
                Inventory = inventory,
                ArmoryInventory = armoryInventory,
                Saddle = saddle,
                Gear = gear,
                Retainers = retainers,
                Minions = minions,
                Mounts = mounts,
                TripleTriadCards = tripleTriadCards,
                Emotes = emotes,
                Bardings = bardings,
                FramerKits = framerkits,
                OrchestrionRolls = orchestrionRolls,
                Ornaments = ornaments,
                Glasses = glasses,
                BeastReputations = beastReputations,
                Duties = duties,
                DutiesUnlocked = dutiesUnlocked
            };
        }

        public static int AddCharacter(SqliteConnection db, Character character)
        {
            Plugin.Log.Debug("Entering AddCharacter()");
            const string insertQuery = $"INSERT INTO {CharacterTableName}([CharacterId], [FirstName], [LastName], [HomeWorld], [Datacenter], [Region], [IsSprout], [IsBattleMentor], [IsTradeMentor], [IsReturner], [LastJob], [LastJobLevel], [FCTag], [FreeCompany], [LastOnline], [PlayTime], [LastPlayTimeUpdate], [HasPremiumSaddlebag], [PlayerCommendations], [CurrentFacewear], [CurrentOrnament], [Attributes], [Currencies], [Jobs], [Profile], [Quests], [Inventory], [ArmoryInventory], [Saddle], [Gear], [Retainers], [Minions], [Mounts], [TripleTriadCards], [Emotes], [Bardings], [FramerKits], [OrchestrionRolls], [Ornaments], [Glasses], [BeastReputations], [Duties], [DutiesUnlocked]) " +
                                       "VALUES (@CharacterId, @FirstName, @LastName, @HomeWorld, @Datacenter, @Region, @IsSprout, @IsBattleMentor, @IsTradeMentor, @IsReturner, @LastJob, @LastJobLevel, @FCTag, @FreeCompany, @LastOnline, @PlayTime, @LastPlayTimeUpdate, @HasPremiumSaddlebag, @PlayerCommendations, @CurrentFacewear, @CurrentOrnament, @Attributes, @Currencies, @Jobs, @Profile, @Quests, @Inventory, @ArmoryInventory, @Saddle, @Gear, @Retainers, @Minions, @Mounts, @TripleTriadCards, @Emotes, @Bardings, @FramerKits, @OrchestrionRolls, @Ornaments, @Glasses, @BeastReputations, @Duties, @DutiesUnlocked)";

            int result = db.Execute(insertQuery, FormatCharacterForDatabase(character));

            Plugin.Log.Debug($"AddCharacter result: {result}");
            if (result != 1)
            {
                return result;
            }

            if (character.Currencies == null)
            {
                return result;
            }

            int result2 = AddCharacterCurrencyHistory(db, character.CharacterId, character.Currencies);
            Plugin.Log.Debug($"AddCharacter => AddCharacterCurrencyHistory result: {result2}");
            return result;
        }

        public static int AddCharacterWithCurrenciesHistories(SqliteConnection db, Character character)
        {
            Plugin.Log.Debug("Entering AddCharacterWithCurrenciesHistories()");
            /*const string insertQuery = $"INSERT INTO {CharacterTableName}([CharacterId], [FirstName], [LastName], [HomeWorld], [Datacenter], [Region], [IsSprout], [IsBattleMentor], [IsTradeMentor], [IsReturner], [LastJob], [LastJobLevel], [FCTag], [FreeCompany], [LastOnline], [PlayTime], [LastPlayTimeUpdate], [HasPremiumSaddlebag], [PlayerCommendations], [Attributes], [Currencies], [Jobs], [Profile], [Quests], [Inventory], [ArmoryInventory], [Saddle], [Gear], [Retainers], [Minions], [Mounts], [TripleTriadCards], [Emotes], [Bardings], [FramerKits], [OrchestrionRolls], [Ornaments], [Glasses], [BeastReputations], [Duties]) " +
                                       "VALUES (@CharacterId, @FirstName, @LastName, @HomeWorld, @Datacenter, @Region, @IsSprout, @IsBattleMentor, @IsTradeMentor, @IsReturner, @LastJob, @LastJobLevel, @FCTag, @FreeCompany, @LastOnline, @PlayTime, @LastPlayTimeUpdate, @HasPremiumSaddlebag, @PlayerCommendations, @Attributes, @Currencies, @Jobs, @Profile, @Quests, @Inventory, @ArmoryInventory, @Saddle, @Gear, @Retainers, @Minions, @Mounts, @TripleTriadCards, @Emotes, @Bardings, @FramerKits, @OrchestrionRolls, @Ornaments, @Glasses, @BeastReputations, @Duties)";

            int result = db.Execute(insertQuery, FormatCharacterForDatabase(character));*/
            int result = AddCharacter(db, character);

            Plugin.Log.Debug($"AddCharacterWithCurrenciesHistories result: {result}");
            if (result != 1)
            {
                return result;
            }

            foreach (int result2 in character.CurrenciesHistory.Select(currenciesHistory => AddCharacterCurrencyHistory(db, currenciesHistory)))
            {
                Plugin.Log.Debug($"AddCharacterWithCurrenciesHistories => AddCharacterCurrencyHistory result: {result2}");
            }

            return result;
        }

        public static void AddCharacters(SqliteConnection db, List<Character> characters)
        {
            SqliteTransaction transaction = db.BeginTransaction();
            foreach (Character character in characters)
            {
                if (character.CurrenciesHistory.Count == 0)
                {
                    AddCharacter(db, character);
                }
                else
                {
                    AddCharacterWithCurrenciesHistories(db, character);
                }
            }
            transaction.Commit();
        }

        public static int AddCharacterCurrencyHistory(SqliteConnection db, CurrenciesHistory ch)
        {
            Plugin.Log.Debug($"AddCharacterCurrencyHistory with params: {ch.CharacterId}, {ch.Currencies}, {ch.Datetime}");
            string currencies = System.Text.Json.JsonSerializer.Serialize(ch.Currencies);
            const string insertHistoryQuery = $"INSERT INTO {CharactersCurrenciesHistoryTableName}([CharacterId], [DateTime], [Currencies]) VALUES (@CharacterId, @Datetime, @Currencies)";
            int result = db.Execute(insertHistoryQuery, new
            {
                ch.CharacterId,
                ch.Datetime,
                Currencies = currencies
            });
            return result;
        }
        public static int AddCharacterCurrencyHistory(SqliteConnection db, ulong id, PlayerCurrencies pc)
        {
            Plugin.Log.Debug("Entering AddCharacterCurrencyHistory()");
            long datetime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string currencies = System.Text.Json.JsonSerializer.Serialize(pc);
            const string insertHistoryQuery = $"INSERT INTO {CharactersCurrenciesHistoryTableName}([CharacterId], [DateTime], [Currencies]) VALUES (@CharacterId, @Datetime, @Currencies)";
#if DEBUG
            Plugin.Log.Debug($"AddCharacter => AddCharacterCurrencyHistory: values: {id}, {datetime}, {currencies}");
#endif

            int result = db.Execute(insertHistoryQuery, new
            {
                CharacterId = id,
                Datetime = datetime,
                Currencies = currencies
            });
            return result;
        }

        public static int BlacklistCharacter(SqliteConnection db, ulong id)
        {
            Plugin.Log.Debug($"Database/BlacklistCharacter entered with params: db = {db}, id = {id}");
            try
            {
                Blacklist? bl = GetBlacklist(db, id);
                if (bl != null)
                {
                    return DeleteCharacter(db, id);
                }

                const string blsql = $"INSERT INTO {BlacklistTableName}('CharacterId') VALUES (@id)";
                db.Execute(blsql, new { id });

                return DeleteCharacter(db, id);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return 0;
            }
        }

        public static Blacklist? GetBlacklist(SqliteConnection db, ulong id)
        {
            const string sql = $"SELECT * FROM {BlacklistTableName} WHERE CharacterId = @id";
            return db.QueryFirstOrDefault<Blacklist>(sql, new { id });
        }
        public static List<Blacklist> GetBlacklists(SqliteConnection db)
        {
            const string sql = $"SELECT * FROM {BlacklistTableName}";
            return db.Query<Blacklist>(sql).AsList();
        }

        public static bool DeleteBlacklist(SqliteConnection db, ulong id)
        {
            Plugin.Log.Debug($"Database/DeleteBlacklist entered with params: db = {db}, id = {id}");
            const string sql = $"SELECT * FROM {BlacklistTableName} WHERE CharacterId = @id";
            Blacklist? character = db.QueryFirstOrDefault<Blacklist>(sql, new { id });
            if (character == null)
            {
                return false;
            }

            const string sql2 = $"DELETE FROM {BlacklistTableName} WHERE CharacterId = @id";
            db.Execute(sql2, new { id });
            return true;
        }
    }
}
