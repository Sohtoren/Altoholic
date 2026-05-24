using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Helpers
{
    public abstract class Event
    {
        public static List<List<bool>> GetCharactersEventsQuests(List<Character> characters)
        {
            List<List<bool>> result = [];
            foreach (Character character in characters)
            {
                bool allsaintswake = (character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_GRIDANIA) ||
                                      character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_LIMSA) ||
                                      character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2013_ULDAH));
                List<bool> completedQuests =
                [
                    /*character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2010),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2011),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2011),
                    character.HasQuest((int)QuestIds.EVENT_FIREFALL_FAIRE_2011),
                    character.HasQuest((int)QuestIds.EVENT_HUNTER_S_MOON_2011),
                    character.HasQuest((int)QuestIds.EVENT_FOUNDATION_DAY_2011),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2011),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2011),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2012),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2012),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2012),
                    character.HasQuest((int)QuestIds.EVENT_FOUNDATION_DAY_2012),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2013),*/
                    allsaintswake,
                    character.HasQuest((int)QuestIds.EVENT_LIGHTNING_STRIKES_2013),
                    //character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2013),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2014),
                    character.HasQuest((int)QuestIds.EVENT_BURGEONING_DREAD_2014),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2014),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2014),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2014),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2014),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2014),
                    character.HasQuest((int)QuestIds.EVENT_THAT_OLD_BLACK_MAGIC_2014),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2014),
                    character.HasQuest((int)QuestIds.EVENT_LIGHTNING_RETURNS),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2_2014),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2014),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2014),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2015),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2015),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2015),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2015),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2015),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2015),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2015),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2015),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2015),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2016),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2016),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2016),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2016),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2016),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2016),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2016),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2016),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2016),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2016),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2017),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2017),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2017),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2017),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2017),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2017),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2017),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2017),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2017),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2017),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2018),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2018),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2018),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2018),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2018),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2018),
                    character.HasQuest((int)QuestIds.EVENT_THE_HUNT_FOR_RATHALOS),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2018),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2019),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2019),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2019),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2019),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2019),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_PHILOSOPHY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2019),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2019),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2019),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_MYTHOLOGY),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2019),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2019),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2020),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_SOLDIERY),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2020),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2020),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2020),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_LAW),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2020),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2020),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2020),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2020),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2020),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2020),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2020),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2021),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_AND_LITTLE_LADIES_DAY_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_ESOTERICS),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_FESTIVAL_2021_THE_HUNT_FOR_PAGEANTRY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2021),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2021),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2021),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2021),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2021),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_LORE),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2021),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2022),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2021),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_SCRIPTURE),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2022),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_VERITY),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2022),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2022),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2022),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_CREATION),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2022),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2023),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2023),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2023),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2023),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_MENDACITY),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2023),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2023),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2023),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_10TH_ANNIVERSARY_HUNT),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2023),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2023),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2023),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAIDEN_S_RHAPSODY_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_FIRST_HUNT_FOR_GENESIS),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2024),
                    character.HasQuest((int)QuestIds.EVENT_A_NOCTURNE_FOR_HEROES_2024),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_HATCHING_TIDE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_PATH_INFERNAL_2024),
                    character.HasQuest((int)QuestIds.EVENT_YO_KAI_WATCH_GATHER_ONE_GATHER_ALL_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_SECOND_HUNT_FOR_GENESIS),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2024),
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2024),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024_1),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2024),
                    character.HasQuest((int)QuestIds.EVENT_THE_RISING_2024),
                    //character.HasQuest((int)QuestIds.EVENT_MOOGLE_TREASURE_TROVE_THE_HUNT_FOR_GOETIA),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2024),
                    character.HasQuest((int)QuestIds.EVENT_BLUNDERVILLE_2024_2),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_CELEBRATION_2024),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2025),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2025),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2025),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2025),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2025),
                    character.HasQuest((int)QuestIds.EVENT_MOONFIRE_FAIRE_2025),
                    character.HasQuest((int)QuestIds.EVENT_RISING_2025),
                    character.HasQuest((int)QuestIds.EVENT_ALL_SAINTS_WAKE_2025),
                    character.HasQuest((int)QuestIds.EVENT_STARLIGHT_2025),
                    character.HasQuest((int)QuestIds.EVENT_HEAVENSTURN_2026),
                    character.HasQuest((int)QuestIds.EVENT_VALENTIONE_S_DAY_2026),
                    character.HasQuest((int)QuestIds.EVENT_LITTLE_LADIES_DAY_2026),
                    character.HasQuest((int)QuestIds.EVENT_HATCHING_TIDE_2026),
                    character.HasQuest((int)QuestIds.EVENT_THE_MAKE_IT_RAIN_CAMPAIGN_2026),
                ];
                result.Add(completedQuests);
            }

            return result;
        }
        public static uint GetEventCurrencyFromEventId(int msqIndex)
        {
            return msqIndex switch
            {
                134 => 50089,
                _ => 0
            };
        }

        public static void DrawEventReward(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int msqIndex)
        {
            switch (msqIndex)
            {
                case 134:
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 481, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 505, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 517, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 529, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 541, 20);
                        break;
                    }
                case 135:
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 579, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 52, 0);
                        break;
                    }
            }
        }
    }
}
