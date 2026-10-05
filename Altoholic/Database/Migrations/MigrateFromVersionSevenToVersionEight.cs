using Altoholic.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using static Altoholic.Database.Database;

namespace Altoholic.Database.Migrations
{
    public static class MigrateFromVersionSevenToVersionEight
    {
        #pragma warning disable CA1812
        // ReSharper disable once ClassNeverInstantiated.Local
        private class PlayerCurrencies
        {
            public int Achievement_Certificate { get; init; }
            public int Allagan_Tomestone_Of_Aesthetics { get; init; }
            public int Allagan_Tomestone_Of_Allegory { get; init; }
            public int Allagan_Tomestone_Of_Aphorism { get; init; }
            public int Allagan_Tomestone_Of_Astronomy { get; init; }
            public int Allagan_Tomestone_Of_Causality { get; init; }
            public int Allagan_Tomestone_Of_Comedy { get; init; }
            public int Allagan_Tomestone_Of_Creation { get; init; }
            public int Allagan_Tomestone_Of_Esoterics { get; init; }
            public int Allagan_Tomestone_Of_Genesis { get; init; }
            public int Allagan_Tomestone_Of_Goetia { get; init; }
            public int Allagan_Tomestone_Of_Heliometry { get; init; }
            public int Allagan_Tomestone_Of_Law { get; init; }
            public int Allagan_Tomestone_Of_Lore { get; init; }
            public int Allagan_Tomestone_Of_Mathematics { get; init; }
            public int Allagan_Tomestone_Of_Mendacity { get; init; }
            public int Allagan_Tomestone_Of_Mnemonics { get; init; }
            public int Allagan_Tomestone_Of_Mythology { get; init; }
            public int Allagan_Tomestone_Of_Phantasmagoria { get; init; }
            public int Allagan_Tomestone_Of_Philosophy { get; init; }
            public int Allagan_Tomestone_Of_Poetics { get; init; }
            public int Allagan_Tomestone_Of_Revelation { get; init; }
            public int Allagan_Tomestone_Of_Scripture { get; init; }
            public int Allagan_Tomestone_Of_Soldiery { get; init; }
            public int Allagan_Tomestone_Of_Verity { get; init; }
            public int Allied_Seal { get; init; }
            public int Ananta_Dreamstaff { get; init; }
            public int Arkasodara_Pana { get; init; }
            public int Auxesia_Credit { get; init; }
            public int Bicolor_Gemstone { get; init; }
            public int Black_Copper_Gil { get; init; }
            public int Bozjan_Cluster { get; init; }
            public int Carved_Kupo_Nut { get; init; }
            public int Centurio_Seal { get; init; }
            public int Cosmocredit { get; init; }
            public int Earth_Cluster { get; init; }
            public int Earth_Crystal { get; init; }
            public int Earth_Shard { get; init; }
            public int Fae_Fancy { get; init; }
            public int Faux_Leaf { get; init; }
            public int Felicitous_Token { get; init; }
            public int Fire_Cluster { get; init; }
            public int Fire_Crystal { get; init; }
            public int Fire_Shard { get; init; }
            public int Flame_Seal { get; init; }
            public int Gil { get; init; }
            public int Hammered_Frogment { get; init; }
            public int Ice_Cluster { get; init; }
            public int Ice_Crystal { get; init; }
            public int Ice_Shard { get; init; }
            public int Irregular_Tomestone_Of_Allegory { get; init; }
            public int Irregular_Tomestone_Of_Aphorism { get; init; }
            public int Irregular_Tomestone_Of_Astronomy_I { get; init; }
            public int Irregular_Tomestone_Of_Astronomy_II { get; init; }
            public int Irregular_Tomestone_Of_Creation { get; init; }
            public int Irregular_Tomestone_Of_Esoterics { get; init; }
            public int Irregular_Tomestone_Of_Genesis_I { get; init; }
            public int Irregular_Tomestone_Of_Genesis_II { get; init; }
            public int Irregular_Tomestone_Of_Goetia { get; init; }
            public int Irregular_Tomestone_Of_Law { get; init; }
            public int Irregular_Tomestone_Of_Lore { get; init; }
            public int Irregular_Tomestone_Of_Mendacity { get; init; }
            public int Irregular_Tomestone_Of_Mythology { get; init; }
            public int Irregular_Tomestone_Of_Pageantry { get; init; }
            public int Irregular_Tomestone_Of_Phantasmagoria { get; init; }
            public int Irregular_Tomestone_Of_Philosophy { get; init; }
            public int Irregular_Tomestone_Of_Revelation { get; init; }
            public int Irregular_Tomestone_Of_Scripture { get; init; }
            public int Irregular_Tomestone_Of_Soldiery { get; init; }
            public int Irregular_Tomestone_Of_Tenfold_Pageantry { get; init; }
            public int Irregular_Tomestone_Of_Verity { get; init; }
            public int Islanders_Cowrie { get; init; }
            public int Ixali_Oaknot { get; init; }
            public int Kojin_Sango { get; init; }
            public int Lightning_Cluster { get; init; }
            public int Lightning_Crystal { get; init; }
            public int Lightning_Shard { get; init; }
            public int Loporrit_Carat { get; init; }
            public int Lunar_Credit { get; init; }
            public int Mamool_Ja_Nanook { get; init; }
            public int MGF { get; init; }
            public int MGP { get; init; }
            public int Namazu_Koban { get; init; }
            public int Occult_Enlightenment_Silver_Piece { get; set; }
            public int Occult_Enlightenment_Gold_Piece { get; set; }
            public int Occult_Enlightenment_Silver_Obol { get; set; }
            public int Occult_Enlightenment_Gold_Obol { get; set; }
            public int Occult_Sanguine_Cipher { get; set; }
            public int Oizys_Credit { get; init; }
            public int Omicron_Omnitoken { get; init; }
            public int Orange_Crafters_Scrip { get; init; }
            public int Orange_Gatherers_Scrip { get; init; }
            public int Phaenna_Credit { get; init; }
            public int Pelu_Pelplume { get; init; }
            public int Purple_Crafters_Scrip { get; init; }
            public int Purple_Gatherers_Scrip { get; init; }
            public int Qitari_Compliment { get; init; }
            public int Rainbowtide_Psashp { get; init; }
            public int Sack_of_Nuts { get; init; }
            public int Seafarers_Cowrie { get; init; }
            public int Serpent_Seal { get; init; }
            public int Skybuilders_Scrip { get; init; }
            public int Steel_Amaljok { get; init; }
            public int Storm_Seal { get; init; }
            public int Sylphic_Goldleaf { get; init; }
            public int Titan_Cobaltpiece { get; init; }
            public int Trophy_Crystal { get; init; }
            public int Vanu_Whitebone { get; init; }
            public int Venture { get; init; }
            public int Water_Cluster { get; init; }
            public int Water_Crystal { get; init; }
            public int Water_Shard { get; init; }
            public int White_Crafters_Scrip { get; init; }
            public int White_Gatherers_Scrip { get; init; }
            public int Wind_Cluster { get; init; }
            public int Wind_Crystal { get; init; }
            public int Wind_Shard { get; init; }
            public int Wolf_Mark { get; init; }
            public int Yellow_Crafters_Scrip { get; init; }
            public int Yellow_Gatherers_Scrip { get; init; }
            public int Yo_Kai_Legendary_Jibanyan_Medal { get; init; }
            public int Yo_Kai_Legendary_Komasan_Medal { get; init; }
            public int Yo_Kai_Legendary_Whisper_Medal { get; init; }
            public int Yo_Kai_Legendary_Blizzaria_Medal { get; init; }
            public int Yo_Kai_Legendary_Kyubi_Medal { get; init; }
            public int Yo_Kai_Legendary_Komajiro_Medal { get; init; }
            public int Yo_Kai_Legendary_Manjimutt_Medal { get; init; }
            public int Yo_Kai_Legendary_Noko_Medal { get; init; }
            public int Yo_Kai_Legendary_Venoct_Medal { get; init; }
            public int Yo_Kai_Legendary_Shogunyan_Medal { get; init; }
            public int Yo_Kai_Legendary_Hovernyan_Medal { get; init; }
            public int Yo_Kai_Legendary_Robonyan_f_type_Medal { get; init; }
            public int Yo_Kai_Legendary_Usapyon_Medal { get; init; }
            public int Yo_Kai_Legendary_Zazel_Medal { get; init; }
            public int Yo_Kai_Legendary_Lord_Ananta_Medal { get; init; }
            public int Yo_Kai_Legendary_Lord_Enma_Medal { get; init; }
            public int Yo_Kai_Legendary_Damona_Medal { get; init; }
            public int Yo_Kai_Medal { get; init; }
            public int Yok_Huy_Ward { get; init; }
            public int Weekly_Acquired_Tomestone { get; init; }
            public int Weekly_Limit_Tomestone { get; init; }

            /*public override string ToString()
            {
                return string.Format("Name: {0}, Type: {1}, Cost: {2}, UserName: {3}", productName, productType, productCost, userName);
            }*/
        }

        private class OldCurrenciesHistory
        {
            public ulong CharacterId { get; init; }
            public PlayerCurrencies? Currencies { get; init; }
            public long Datetime { get; init; }
        }
        // ReSharper disable once ClassNeverInstantiated.Local
        #pragma warning restore CA1812

        public static bool Do(SqliteConnection db, string characterTableName, string charactersCurrenciesHistoryTableName)
        {
            try
            {
                string sql = $"SELECT * FROM {characterTableName}";
                IEnumerable<DatabaseCharacter> dbCharacters = db.Query<DatabaseCharacter>(sql);
                foreach (DatabaseCharacter dbCharacter in dbCharacters)
                {
                    Character character = MigratePlayerCurrenciesToDictionary(dbCharacter);
                    Update(db, characterTableName, character);
                    
                    string sqlHistory = $"SELECT * FROM {charactersCurrenciesHistoryTableName} WHERE CharacterId = @CharacterId";
                    IEnumerable<DatabaseCurrenciesHistory> dbHistories = db.Query<DatabaseCurrenciesHistory>(sql, new { character.CharacterId });

                    List<CurrenciesHistory> newHistories = [];
                    foreach (DatabaseCurrenciesHistory dbHistory in dbHistories)
                    {
                        PlayerCurrencies? currencies =
                            System.Text.Json.JsonSerializer.Deserialize<PlayerCurrencies>(dbHistory.Currencies);
                        CurrenciesHistory currenciesHistory = new()
                        {
                            CharacterId = dbHistory.CharacterId,
                            Datetime = dbHistory.Datetime,
                            Currencies = currencies is not null ? PcToDictionary(currencies) : []
                        };

                        newHistories.Add(currenciesHistory);
                    }
                    string sqlDelete = $"DELETE FROM {charactersCurrenciesHistoryTableName} WHERE CharacterId = @CharacterId";
                    int resultDelete = db.Execute(sqlDelete, new { dbCharacter.CharacterId });
                    if (resultDelete != 0)
                    {
                        foreach (CurrenciesHistory history in newHistories)
                        {
                            string curr = System.Text.Json.JsonSerializer.Serialize(history.Currencies);
                            string insertHistoryQuery =
                                $"INSERT INTO {charactersCurrenciesHistoryTableName}([CharacterId], [DateTime], [Currencies]) VALUES (@CharacterId, @Datetime, @Currencies)";
                            int resultInsert = db.Execute(insertHistoryQuery,
                                new { history.CharacterId, history.Datetime, Currencies = curr });
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return false;
            }
        }

        private static Character MigratePlayerCurrenciesToDictionary(DatabaseCharacter dbCharacter)
        {
            Character character = new() { CharacterId = dbCharacter.CharacterId };
            PlayerCurrencies? currencies = string.IsNullOrEmpty(dbCharacter.Currencies)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<PlayerCurrencies>(dbCharacter.Currencies);


            if (currencies is not null)
            {
                character.Weekly_Acquired_Tomestone = currencies.Weekly_Acquired_Tomestone;
                character.Weekly_Limit_Tomestone = currencies.Weekly_Limit_Tomestone;
                character.Currencies = PcToDictionary(currencies);
            }
            return character;
        }

        private static Dictionary<uint, int> PcToDictionary(PlayerCurrencies currencies)
        {
            Dictionary<uint, int> currenciesDict = [];
            currenciesDict.Add((uint)Currencies.ACHIEVEMENT_CERTIFICATE, currencies.Achievement_Certificate);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_AESTHETICS, currencies.Allagan_Tomestone_Of_Aesthetics);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_ALLEGORY, currencies.Allagan_Tomestone_Of_Allegory);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_APHORISM, currencies.Allagan_Tomestone_Of_Aphorism);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_ASTRONOMY, currencies.Allagan_Tomestone_Of_Astronomy);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_CAUSALITY, currencies.Allagan_Tomestone_Of_Causality);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_COMEDY, currencies.Allagan_Tomestone_Of_Comedy);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_CREATION, currencies.Allagan_Tomestone_Of_Creation);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_ESOTERICS, currencies.Allagan_Tomestone_Of_Esoterics);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_GENESIS, currencies.Allagan_Tomestone_Of_Genesis);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_GOETIA, currencies.Allagan_Tomestone_Of_Goetia);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_HELIOMETRY, currencies.Allagan_Tomestone_Of_Heliometry);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_LAW, currencies.Allagan_Tomestone_Of_Law);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_LORE, currencies.Allagan_Tomestone_Of_Lore);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_MATHEMATICS, currencies.Allagan_Tomestone_Of_Mathematics);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_MENDACITY, currencies.Allagan_Tomestone_Of_Mendacity);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_MNEMONICS, currencies.Allagan_Tomestone_Of_Mnemonics);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_MYTHOLOGY, currencies.Allagan_Tomestone_Of_Mythology);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_PHANTASMAGORIA, currencies.Allagan_Tomestone_Of_Phantasmagoria);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_PHILOSOPHY, currencies.Allagan_Tomestone_Of_Philosophy);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_POETICS, currencies.Allagan_Tomestone_Of_Poetics);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_REVELATION, currencies.Allagan_Tomestone_Of_Revelation);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_SCRIPTURE, currencies.Allagan_Tomestone_Of_Scripture);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_SOLDIERY, currencies.Allagan_Tomestone_Of_Soldiery);
            currenciesDict.Add((uint)Currencies.ALLAGAN_TOMESTONE_OF_VERITY, currencies.Allagan_Tomestone_Of_Verity);
            currenciesDict.Add((uint)Currencies.ALLIED_SEAL, currencies.Allied_Seal);
            currenciesDict.Add((uint)Currencies.ANANTA_DREAMSTAFF, currencies.Ananta_Dreamstaff);
            currenciesDict.Add((uint)Currencies.ARKASODARA_PANA, currencies.Arkasodara_Pana);
            currenciesDict.Add((uint)Currencies.AUXESIA_CREDIT, currencies.Auxesia_Credit);
            currenciesDict.Add((uint)Currencies.BICOLOR_GEMSTONE, currencies.Bicolor_Gemstone);
            currenciesDict.Add((uint)Currencies.BLACK_COPPER_GIL, currencies.Black_Copper_Gil);
            currenciesDict.Add((uint)Currencies.BOZJAN_CLUSTER, currencies.Bozjan_Cluster);
            currenciesDict.Add((uint)Currencies.CARVED_KUPO_NUT, currencies.Carved_Kupo_Nut);
            currenciesDict.Add((uint)Currencies.CENTURIO_SEAL, currencies.Centurio_Seal);
            currenciesDict.Add((uint)Currencies.COSMOCREDIT, currencies.Cosmocredit);
            currenciesDict.Add((uint)Currencies.EARTH_CLUSTER, currencies.Earth_Cluster);
            currenciesDict.Add((uint)Currencies.EARTH_CRYSTAL, currencies.Earth_Crystal);
            currenciesDict.Add((uint)Currencies.EARTH_SHARD, currencies.Earth_Shard);
            currenciesDict.Add((uint)Currencies.FAE_FANCY, currencies.Fae_Fancy);
            currenciesDict.Add((uint)Currencies.FAUX_LEAF, currencies.Faux_Leaf);
            currenciesDict.Add((uint)Currencies.FELICITOUS_TOKEN, currencies.Felicitous_Token);
            currenciesDict.Add((uint)Currencies.FIRE_CLUSTER, currencies.Fire_Cluster);
            currenciesDict.Add((uint)Currencies.FIRE_CRYSTAL, currencies.Fire_Crystal);
            currenciesDict.Add((uint)Currencies.FIRE_SHARD, currencies.Fire_Shard);
            currenciesDict.Add((uint)Currencies.FLAME_SEAL, currencies.Flame_Seal);
            currenciesDict.Add((uint)Currencies.GIL, currencies.Gil);
            currenciesDict.Add((uint)Currencies.HAMMERED_FROGMENT, currencies.Hammered_Frogment);
            currenciesDict.Add((uint)Currencies.ICE_CLUSTER, currencies.Ice_Cluster);
            currenciesDict.Add((uint)Currencies.ICE_CRYSTAL, currencies.Ice_Crystal);
            currenciesDict.Add((uint)Currencies.ICE_SHARD, currencies.Ice_Shard);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_ALLEGORY, currencies.Irregular_Tomestone_Of_Allegory);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_APHORISM, currencies.Irregular_Tomestone_Of_Aphorism);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_ASTRONOMY_I, currencies.Irregular_Tomestone_Of_Astronomy_I);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_ASTRONOMY_II, currencies.Irregular_Tomestone_Of_Astronomy_II);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_CREATION, currencies.Irregular_Tomestone_Of_Creation);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_ESOTERICS, currencies.Irregular_Tomestone_Of_Esoterics);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_I, currencies.Irregular_Tomestone_Of_Genesis_I);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_II, currencies.Irregular_Tomestone_Of_Genesis_II);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_GOETIA, currencies.Irregular_Tomestone_Of_Goetia);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_LAW, currencies.Irregular_Tomestone_Of_Law);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_LORE, currencies.Irregular_Tomestone_Of_Lore);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_MENDACITY, currencies.Irregular_Tomestone_Of_Mendacity);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_MYTHOLOGY, currencies.Irregular_Tomestone_Of_Mythology);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_PAGEANTRY, currencies.Irregular_Tomestone_Of_Pageantry);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_PHANTASMAGORIA, currencies.Irregular_Tomestone_Of_Phantasmagoria);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_PHILOSOPHY, currencies.Irregular_Tomestone_Of_Philosophy);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_REVELATION, currencies.Irregular_Tomestone_Of_Revelation);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_SCRIPTURE, currencies.Irregular_Tomestone_Of_Scripture);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_SOLDIERY, currencies.Irregular_Tomestone_Of_Soldiery);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_TENFOLD_PAGEANTRY, currencies.Irregular_Tomestone_Of_Tenfold_Pageantry);
            currenciesDict.Add((uint)Currencies.IRREGULAR_TOMESTONE_OF_VERITY, currencies.Irregular_Tomestone_Of_Verity);
            currenciesDict.Add((uint)Currencies.ISLANDERS_COWRIE, currencies.Islanders_Cowrie);
            currenciesDict.Add((uint)Currencies.IXALI_OAKNOT, currencies.Ixali_Oaknot);
            currenciesDict.Add((uint)Currencies.KOJIN_SANGO, currencies.Kojin_Sango);
            currenciesDict.Add((uint)Currencies.LIGHTNING_CLUSTER, currencies.Lightning_Cluster);
            currenciesDict.Add((uint)Currencies.LIGHTNING_CRYSTAL, currencies.Lightning_Crystal);
            currenciesDict.Add((uint)Currencies.LIGHTNING_SHARD, currencies.Lightning_Shard);
            currenciesDict.Add((uint)Currencies.LOPORRIT_CARAT, currencies.Loporrit_Carat);
            currenciesDict.Add((uint)Currencies.LUNAR_CREDIT, currencies.Lunar_Credit);
            currenciesDict.Add((uint)Currencies.MAMOOL_JA_NANOOK, currencies.Mamool_Ja_Nanook);
            currenciesDict.Add((uint)Currencies.MGF, currencies.MGF);
            currenciesDict.Add((uint)Currencies.MGP, currencies.MGP);
            currenciesDict.Add((uint)Currencies.NAMAZU_KOBAN, currencies.Namazu_Koban);
            currenciesDict.Add((uint)Currencies.OCCULT_ENLIGHTENMENT_SILVER_PIECE, currencies.Occult_Enlightenment_Silver_Piece);
            currenciesDict.Add((uint)Currencies.OCCULT_ENLIGHTENMENT_GOLD_PIECE, currencies.Occult_Enlightenment_Gold_Piece);
            currenciesDict.Add((uint)Currencies.OCCULT_ENLIGHTENMENT_SILVER_OBOL, currencies.Occult_Enlightenment_Silver_Obol);
            currenciesDict.Add((uint)Currencies.OCCULT_ENLIGHTENMENT_GOLD_OBOL, currencies.Occult_Enlightenment_Gold_Obol);
            currenciesDict.Add((uint)Currencies.OCCULT_SANGUINE_CIPHER, currencies.Occult_Sanguine_Cipher);
            currenciesDict.Add((uint)Currencies.OIZYS_CREDIT, currencies.Oizys_Credit);
            currenciesDict.Add((uint)Currencies.OMICRON_OMNITOKEN, currencies.Omicron_Omnitoken);
            currenciesDict.Add((uint)Currencies.ORANGE_CRAFTERS_SCRIP, currencies.Orange_Crafters_Scrip);
            currenciesDict.Add((uint)Currencies.ORANGE_GATHERERS_SCRIP, currencies.Orange_Gatherers_Scrip);
            currenciesDict.Add((uint)Currencies.PHAENNA_CREDIT, currencies.Phaenna_Credit);
            currenciesDict.Add((uint)Currencies.PELU_PELPLUME, currencies.Pelu_Pelplume);
            currenciesDict.Add((uint)Currencies.PURPLE_CRAFTERS_SCRIP, currencies.Purple_Crafters_Scrip);
            currenciesDict.Add((uint)Currencies.PURPLE_GATHERERS_SCRIP, currencies.Purple_Gatherers_Scrip);
            currenciesDict.Add((uint)Currencies.QITARI_COMPLIMENT, currencies.Qitari_Compliment);
            currenciesDict.Add((uint)Currencies.RAINBOWTIDE_PSASHP, currencies.Rainbowtide_Psashp);
            currenciesDict.Add((uint)Currencies.SACK_OF_NUTS, currencies.Sack_of_Nuts);
            currenciesDict.Add((uint)Currencies.SEAFARERS_COWRIE, currencies.Seafarers_Cowrie);
            currenciesDict.Add((uint)Currencies.SERPENT_SEAL, currencies.Serpent_Seal);
            currenciesDict.Add((uint)Currencies.SKYBUILDERS_SCRIP, currencies.Skybuilders_Scrip);
            currenciesDict.Add((uint)Currencies.STEEL_AMALJOK, currencies.Steel_Amaljok);
            currenciesDict.Add((uint)Currencies.STORM_SEAL, currencies.Storm_Seal);
            currenciesDict.Add((uint)Currencies.SYLPHIC_GOLDLEAF, currencies.Sylphic_Goldleaf);
            currenciesDict.Add((uint)Currencies.TITAN_COBALTPIECE, currencies.Titan_Cobaltpiece);
            currenciesDict.Add((uint)Currencies.TROPHY_CRYSTAL, currencies.Trophy_Crystal);
            currenciesDict.Add((uint)Currencies.VANU_WHITEBONE, currencies.Vanu_Whitebone);
            currenciesDict.Add((uint)Currencies.VENTURE, currencies.Venture);
            currenciesDict.Add((uint)Currencies.WATER_CLUSTER, currencies.Water_Cluster);
            currenciesDict.Add((uint)Currencies.WATER_CRYSTAL, currencies.Water_Crystal);
            currenciesDict.Add((uint)Currencies.WATER_SHARD, currencies.Water_Shard);
            currenciesDict.Add((uint)Currencies.WHITE_CRAFTERS_SCRIP, currencies.White_Crafters_Scrip);
            currenciesDict.Add((uint)Currencies.WHITE_GATHERERS_SCRIP, currencies.White_Gatherers_Scrip);
            currenciesDict.Add((uint)Currencies.WIND_CLUSTER, currencies.Wind_Cluster);
            currenciesDict.Add((uint)Currencies.WIND_CRYSTAL, currencies.Wind_Crystal);
            currenciesDict.Add((uint)Currencies.WIND_SHARD, currencies.Wind_Shard);
            currenciesDict.Add((uint)Currencies.WOLF_MARK, currencies.Wolf_Mark);
            currenciesDict.Add((uint)Currencies.YELLOW_CRAFTERS_SCRIP, currencies.Yellow_Crafters_Scrip);
            currenciesDict.Add((uint)Currencies.YELLOW_GATHERERS_SCRIP, currencies.Yellow_Gatherers_Scrip);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_JIBANYAN_MEDAL, currencies.Yo_Kai_Legendary_Jibanyan_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_KOMASAN_MEDAL, currencies.Yo_Kai_Legendary_Komasan_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_WHISPER_MEDAL, currencies.Yo_Kai_Legendary_Whisper_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_BLIZZARIA_MEDAL, currencies.Yo_Kai_Legendary_Blizzaria_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_KYUBI_MEDAL, currencies.Yo_Kai_Legendary_Kyubi_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_KOMAJIRO_MEDAL, currencies.Yo_Kai_Legendary_Komajiro_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_MANJIMUTT_MEDAL, currencies.Yo_Kai_Legendary_Manjimutt_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_NOKO_MEDAL, currencies.Yo_Kai_Legendary_Noko_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_VENOCT_MEDAL, currencies.Yo_Kai_Legendary_Venoct_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_SHOGUNYAN_MEDAL, currencies.Yo_Kai_Legendary_Shogunyan_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_HOVERNYAN_MEDAL, currencies.Yo_Kai_Legendary_Hovernyan_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_ROBONYAN_F_TYPE_MEDAL, currencies.Yo_Kai_Legendary_Robonyan_f_type_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_USAPYON_MEDAL, currencies.Yo_Kai_Legendary_Usapyon_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_ZAZEL_MEDAL, currencies.Yo_Kai_Legendary_Zazel_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_LORD_ANANTA_MEDAL, currencies.Yo_Kai_Legendary_Lord_Ananta_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_LORD_ENMA_MEDAL, currencies.Yo_Kai_Legendary_Lord_Enma_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_LEGENDARY_DAMONA_MEDAL, currencies.Yo_Kai_Legendary_Damona_Medal);
            currenciesDict.Add((uint)Currencies.YO_KAI_MEDAL, currencies.Yo_Kai_Medal);
            currenciesDict.Add((uint)Currencies.YOK_HUY_WARD, currencies.Yok_Huy_Ward);

            return currenciesDict;
        }

        private static void Update(SqliteConnection db, string characterTableName, Character character)
        {
            string currencies = System.Text.Json.JsonSerializer.Serialize(character.Currencies);
            ulong characterId = character.CharacterId;

            try
            {
                string updateSql = $"UPDATE {characterTableName} SET [Currencies] = @currencies WHERE [CharacterId] = @characterId";
                db.Execute(updateSql, new { currencies, characterId });

            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
            }
        }
    }
}
