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
                    character.HasQuest((int)QuestIds.EVENT_BREAKING_BRICK_MOUNTAINS_2026),
                ];
                result.Add(completedQuests);
            }

            return result;
        }
        public static uint GetEventCurrencyFromEventId(int msqIndex)
        {
            return msqIndex switch
            {
                132 => 47863,
                133 => 50082,
                134 => 50089,
                136 => 1,
                _ => 0
            };
        }

        public static void DrawEventReward(ClientLanguage currentLocale, GlobalCache globalCache, List<Character> chars, int msqIndex)
        {
            switch (msqIndex)

            {
                case 0: /*All Saints' Wake (2013)*/
                    {
                        break;
                    }
                case 1:/*Lightning Strikes (2013)*/
                    {
                        break;
                    }
                /*case 2: //Starlight Celebration (2013)
                {
                    break;
                }*/
                case 2:/*Heavensturn (2014)*/
                    {
                        break;
                    }
                case 3:/*Burgeoning Dread (2014)*/
                    {
                        break;
                    }
                case 4:/*Breaking Brick Mountains (2014)*/
                    {
                        break;
                    }
                case 5:/*Valentione's Day (2014)*/
                    {
                        break;
                    }
                case 6:/*Little Ladies' Day (2014)*/
                    {
                        break;
                    }
                case 7:/*Hatching-tide (2014)*/
                    {
                        break;
                    }
                case 8:/*Moonfire Faire (2014)*/
                    {
                        break;
                    }
                case 9:/*That Old Black Magic (2014)*/
                    {
                        break;
                    }
                case 10:/*The Rising (2014)*/
                    {
                        break;
                    }
                case 12:/*Lightning Returns";11;       "Breaking Brick Mountains (2014)*/
                    {
                        break;
                    }
                case 13:/*All Saints' Wake (2014)*/
                    {
                        break;
                    }
                case 14:/*Starlight Celebration (2014)*/
                    {
                        break;
                    }
                case 15:/*Heavensturn (2015)*/
                    {
                        break;
                    }
                case 16:/*Valentione's Day (2015)*/
                    {
                        break;
                    }
                case 17:/*Little Ladies' Day (2015)*/
                    {
                        break;
                    }
                case 18:/*Hatching-tide (2015)*/
                    {
                        break;
                    }
                case 19:/*Moonfire Faire (2015)*/
                    {
                        break;
                    }
                case 20:/*The Rising (2015)*/
                    {
                        break;
                    }
                case 21:/*All Saints' Wake (2015)*/
                    {
                        break;
                    }
                case 22:/*The Maiden's Rhapsody (2015)*/
                    {
                        break;
                    }
                case 23:/*Starlight Celebration (2015)*/
                    {
                        break;
                    }
                case 24:/*Heavensturn (2016)*/
                    {
                        break;
                    }
                case 25:/*Valentione's Day (2016)*/
                    {
                        break;
                    }
                case 26:/*Little Ladies' Day (2016)*/
                    {
                        break;
                    }
                case 27:/*Hatching-tide (2016)*/
                    {
                        break;
                    }
                case 28:/*The Make It Rain Campaign" 2016*/
                    {
                        break;
                    }
                case 29:/*Yo-kai Watch: Gather One;Gather All! (2016) **/
                    {
                        break;
                    }
                case 30:/*Moonfire Faire (2016)*/
                    {
                        break;
                    }
                case 31:/*The Rising (2016)*/
                    {
                        break;
                    }
                case 32:/*All Saints' Wake (2016)*/
                    {
                        break;
                    }
                case 33:/*Starlight Celebration (2016)*/
                    {
                        break;
                    }
                case 34:/*Heavensturn (2017)*/
                    {
                        break;
                    }
                case 35:/*Valentione's Day (2017)*/
                    {
                        break;
                    }
                case 36:/*Little Ladies' Day (2017)*/
                    {
                        break;
                    }
                case 37:/*Hatching-tide (2017)*/
                    {
                        break;
                    }
                case 38:/*The Make It Rain Campaign (2017)*/
                    {
                        break;
                    }
                case 39:/*Moonfire Faire (2017)*/
                    {
                        break;
                    }
                case 40:/*The Rising (2017)*/
                    {
                        break;
                    }
                case 41:/*Yo-kai Watch: Gather One Gather All! (2017)*/
                    {
                        break;
                    }
                case 42:/*All Saints' Wake (2017)*/
                    {
                        break;
                    }
                case 43:/*The Maiden's Rhapsody (2017)*/
                    {
                        break;
                    }
                case 44:/*Breaking Brick Mountains (2017)*/
                    {
                        break;
                    }
                case 45:/*Starlight Celebration (2017)*/
                    {
                        break;
                    }
                case 46:/*Heavensturn (2018)*/
                    {
                        break;
                    }
                case 47:/*Valentione's Day (2018)*/
                    {
                        break;
                    }
                case 48:/*Little Ladies' Day (2018)*/
                    {
                        break;
                    }
                case 49:/*Hatching-tide (2018)*/
                    {
                        break;
                    }
                case 50:/*The Make It Rain Campaign (2018)*/
                    {
                        break;
                    }
                case 51:/*Moonfire Faire (2018)*/
                    {
                        break;
                    }
                case 52:/*The Rising (2018)*/
                    {
                        break;
                    }
                case 53:/*All Saints' Wake (2018)*/
                    {
                        break;
                    }
                case 55:/*The Hunt For Rathalos";54;       "Starlight Celebration (2018)*/
                    {
                        break;
                    }
                case 56:/*Heavensturn (2019)*/
                    {
                        break;
                    }
                case 57:/*Valentione's Day (2019)*/
                    {
                        break;
                    }
                case 58:/*Little Ladies' Day (2019)*/
                    {
                        break;
                    }
                case 59:/*Hatching-tide (2019)*/
                    {
                        break;
                    }
                case 60:/*A Nocturne for Heroes (2019) **/
                    {
                        break;
                    }
                case 61:/*The Make It Rain Campaign (2019)*/
                    {
                        break;
                    }
                case 62:/*Moonfire Faire (2019)*/
                    {
                        break;
                    }
                case 63:/*The Rising (2019)*/
                    {
                        break;
                    }
                case 64:/*All Saints' Wake (2019)*/
                    {
                        break;
                    }
                case 65:/*Starlight Celebration (2019)*/
                    {
                        break;
                    }
                case 66:/*Heavensturn (2020)*/
                    {
                        break;
                    }
                case 67:/*Valentione's Day (2020)*/
                    {
                        break;
                    }
                case 68:/*Little Ladies' Day (2020)*/
                    {
                        break;
                    }
                case 69:/*Hatching-tide (2020)*/
                    {
                        break;
                    }
                case 70:/*The Maiden's Rhapsody (2020)*/
                    {
                        break;
                    }
                case 71:/*Breaking Brick Mountains (2020)*/
                    {
                        break;
                    }
                case 72:/*Moonfire Faire (2020)*/
                    {
                        break;
                    }
                case 73:/*Yo-kai Watch: Gather One;Gather All! (2020) **/
                    {
                        break;
                    }
                case 74:/*The Rising (2020)*/
                    {
                        break;
                    }
                case 75:/*The Make It Rain Campaign (2020)*/
                    {
                        break;
                    }
                case 76:/*Starlight Celebration (2020)*/
                    {
                        break;
                    }
                case 77:/*Heavensturn (2021)*/
                    {
                        break;
                    }
                case 78:/*Valentione's and Little Ladies' Day (2021)*/
                    {
                        break;
                    }
                case 79:/*Hatching-tide (2021)*/
                    {
                        break;
                    }
                case 80:/*The Make It Rain Campaign" 2021*/
                    {
                        break;
                    }
                case 81:/*Moonfire Faire (2021)*/
                    {
                        break;
                    }
                case 82:/*The Rising (2021)*/
                    {
                        break;
                    }
                case 83:/*A Nocturne for Heroes (2021) **/
                    {
                        break;
                    }
                case 84:/*Breaking Brick Mountains (2021)*/
                    {
                        break;
                    }
                case 85:/*Starlight Celebration (2021)*/
                    {
                        break;
                    }
                case 86:/*Heavensturn (2022)*/
                    {
                        break;
                    }
                case 87:/*All Saints' Wake (2021 delayed)*/
                    {
                        break;
                    }
                case 88:/*Valentione's Day (2022)*/
                    {
                        break;
                    }
                case 89:/*Little Ladies' Day (2022)*/
                    {
                        break;
                    }
                case 90:/*Hatching-tide (2022)*/
                    {
                        break;
                    }
                case 91:/*The Maiden's Rhapsody (2022)*/
                    {
                        break;
                    }
                case 92:/*The Make It Rain Campaign" 2022*/
                    {
                        break;
                    }
                case 93:/*Moonfire Faire (2022)*/
                    {
                        break;
                    }
                case 94:/*The Rising (2022)*/
                    {
                        break;
                    }
                case 95:/*All Saints' Wake (2022)*/
                    {
                        break;
                    }
                case 96:/*Starlight Celebration (2022)*/
                    {
                        break;
                    }
                case 97:/*Heavensturn (2023)*/
                    {
                        break;
                    }
                case 98:/*Valentione's Day (2023)*/
                    {
                        break;
                    }
                case 99:/*Little Ladies' Day (2023)*/
                    {
                        break;
                    }
                case 100: /*Hatching-tide (2023)*/
                    {
                        break;
                    }
                case 101: /*The Make It Rain Campaign (2023)*/
                    {
                        break;
                    }
                case 102: /*Moonfire Faire (2023)*/
                    {
                        break;
                    }
                case 103: /*The Rising (2023)*/
                    {
                        break;
                    }
                case 104: /*All Saints' Wake (2023)*/
                    {
                        break;
                    }
                case 105: /*Blunderville" ***/
                    {
                        break;
                    }
                case 106: /*Starlight Celebration (2023)*/
                    {
                        break;
                    }
                case 107: /*Heavensturn (2024)*/
                    {
                        break;
                    }
                case 108: /*The Maiden's Rhapsody (2024)*/
                    {
                        break;
                    }
                case 109: /*Valentione's Day (2024)*/
                    {
                        break;
                    }
                case 110: /*A Nocturne for Heroes (2024) **/
                    {
                        break;
                    }
                case 111: /*Little Ladies' Day & Hatching-tide (2024)*/
                    {
                        break;
                    }
                case 112: /*The Path Infernal (2024)*/
                    {
                        break;
                    }
                case 113: /*Yo-kai Watch: Gather One;Gather All! (2024) **/
                    {
                        break;
                    }
                case 114: /*The Make It Rain Campaign" 2024*/
                    {
                        break;
                    }
                case 115: /*Breaking Brick Mountains (2024)*/
                    {
                        break;
                    }
                case 116: /*Blunderville" ***/
                    {
                        break;
                    }
                case 117: /*Moonfire Faire (2024)*/
                    {
                        break;
                    }
                case 118: /*The Rising (2024)*/
                    {
                        break;
                    }
                case 119: /*All Saints' Wake (2024)*/
                    {
                        break;
                    }
                case 120: /*Blunderville" ***/
                    {
                        break;
                    }
                case 121: /*Starlight Celebration (2024)*/
                    {
                        break;
                    }
                case 122: /*Heavensturn (2025)*/
                    {
                        break;
                    }
                case 123: /*Valentione's Day (2025)*/
                    {
                        break;
                    }
                case 124: /*Little Ladies' Day (2025)*/
                    {
                        break;
                    }
                case 125: /*Hatching-tide (2025)*/
                    {
                        break;
                    }
                case 126: /*The Make It Rain Campaign (2025)*/
                    {
                        break;
                    }
                case 127: /*Moonfire Faire (2025)*/
                    {

                        break;
                    }
                case 128: /*The Rising (2025)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 546, 0);
                        break;
                    }
                case 129: /*All Saints' Wake (2025)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Mount, 396, 0);
                        break;
                    }
                case 130: /*Starlight Celebration (2025)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 298, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 299, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 300, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 301, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 302, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 303, 0);
                        break;
                    }
                case 131: /*Heavensturn (2026)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 529, 0);

                        break;
                    }
                case 132: /*Valentione's Day (2026)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 825, 2);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 826, 2);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 827, 2);
                        break;
                    }
                case 133: /*Little Ladies' Day (2026)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 322, 1);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Emote, 324, 1);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Orchestrion, 805, 2);
                        break;
                    }
                case 134: /*Hatching-tide (2026)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 481, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 505, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 517, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 529, 20);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Glass, 541, 20);
                        break;
                    }
                case 135: /*The Make It Rain Campaign (2026)*/
                    {
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Minion, 579, 0);
                        Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.Ornament, 52, 0);
                        break;
                    }
                case 136: /*Breaking Brick Mountains (2026)*/
                    {
                        uint? fkId = globalCache.FramerKitStorage.GetFramerKitIdFromItemId(51998);
                        if (fkId is not null)
                        {
                            Helpers.Reward.DrawAllCharsCollectible(currentLocale, globalCache, chars, CharacterCollectible.FramerKit, fkId.Value, 0);
                        }
                        break;
                    }

            }
        }
    }
}