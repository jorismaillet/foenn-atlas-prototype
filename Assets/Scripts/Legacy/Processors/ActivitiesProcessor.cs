using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions;
using Assets.Scripts.Foenn.Engine.Weathers;
using Assets.Scripts.Foenn.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Processors
{
    public class ActivitiesProcessor
    {
        public string name;
        public int weight;
        public IActivityCondition condition;
        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<WeatherFieldCondition> hourlyConditions, cumulatedHourConditions;
        public UnityEngine.Color color;

        private static UnityEngine.Color
            VertJardin = ColorUtils.Get(58, 251, 3),
            bleuCiel = ColorUtils.Get(3, 251, 248),
            bleuMer = ColorUtils.Get(15, 75, 227),
            sable = ColorUtils.Get(241, 255, 0),
            asphalteClaire = ColorUtils.Get(234, 234, 234),
            balleTennis = ColorUtils.Get(218, 255, 0),
            murs = ColorUtils.Get(144, 144, 144),
            vertForet = ColorUtils.Get(29, 122, 29),
            crepuscule = ColorUtils.Get(255, 183, 0),
            rouge = ColorUtils.Get(255, 0, 0);


        public static ActivitiesProcessor JusteBien = new ActivitiesProcessor("Juste bien", 1, UnityEngine.Color.white, 0, 1, new TimeRangeCondition(9, 21),
            new List<WeatherFieldCondition>() {
                //new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 5, 28),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0)
                //new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FXI, WeatherFieldKey.FXI2, WeatherFieldKey.FXI3S}, 0, 4),
                //new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2}, 0, 2)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );

        public static ActivitiesProcessor Moustiques1 = new ActivitiesProcessor("Moustiques tigres (Matin)", -15, rouge, 0, 1, new TimeRangeCondition(0, 7),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 20, 50),
            },
            new List<WeatherFieldCondition>()
            {
            }
        ); public static ActivitiesProcessor Moustiques2 = new ActivitiesProcessor("Moustiques tigres (Soir)", -15, rouge, 0, 1, new TimeRangeCondition(21, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 20, 50),
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static ActivitiesProcessor TropChaud = new ActivitiesProcessor("Trop chaud", -15, rouge, 0, 1, new TimeRangeCondition(8, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 30, 50),
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity TropDePluie = new Activity("Trop de pluie", -4, ColorUtils.Get(0, 0, 200), 0, 1, new TimeRangeCondition(0, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(AttributeKey.RR1, 2, 1000),
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity TropDeVent = new Activity("Trop de vent", -2, ColorUtils.Get(0, 255, 0), 0, 1, new TimeRangeCondition(0, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2}, 6, 1000)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity Tempete = new Activity("Tempête", -8, ColorUtils.Get(0, 255, 0), 0, 1, new TimeRangeCondition(0, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FXI, AttributeKey.FXI2, AttributeKey.FXI3S}, 17, 1000)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static ActivitiesProcessor Piscine = new ActivitiesProcessor("Piscine", 6, bleuCiel, 0, 2, new TimeRangeCondition(9, 22),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 25, 33),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S }, 0, 2)/*,
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)*/
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0)
            }
        );
        public static ActivitiesProcessor Kayak = new ActivitiesProcessor("Kayak", 2, bleuMer, 0, 3, new TimeRangeCondition(14, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 23, 31),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S}, 0, 2)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static ActivitiesProcessor Plage = new ActivitiesProcessor("Plage", 6, sable, 0, 2, new TimeRangeCondition(11, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 23, 30),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S }, 0, 1)/*,
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)*/
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static ActivitiesProcessor Velo = new ActivitiesProcessor("Vélo", 1, asphalteClaire, 10, 2, new TimeRangeCondition(14, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 17, 24),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S}, 0, 2) // Certaines valeurs ne sont pas disponibles dans les posts
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        //-----------------------------------//
        public static ActivitiesProcessor Jardin = new ActivitiesProcessor("Jardin", 1, VertJardin, 0, 1, new TimeRangeCondition(10, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 16, 30),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S}, 0, 2)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static ActivitiesProcessor Tennis = new ActivitiesProcessor("Tennis", 1, balleTennis, 0, 3, new TimeRangeCondition(9, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 10, 27),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S }, 0, 1)
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0)
            }
        );
        public static ActivitiesProcessor Ville = new ActivitiesProcessor("Ville", 1, murs, 0, 1, new TimeRangeCondition(11, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 0, 30),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S }, 0, 3)
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static ActivitiesProcessor Randonnee = new ActivitiesProcessor("Randonnée", 1, vertForet, 0, 2, new TimeRangeCondition(13, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 10, 24),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S }, 0, 1)
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static ActivitiesProcessor DinerExterieur = new ActivitiesProcessor("Diner en extérieur", 5, crepuscule, 0, 2, new TimeRangeCondition(18, 21),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.T, AttributeKey.T10, AttributeKey.T20, AttributeKey.T50, AttributeKey.T100,  }, 23, 28),
                new WeatherFieldCondition(AttributeKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<AttributeKey>() { AttributeKey.FF, AttributeKey.FF2, AttributeKey.FXI3S }, 0, 0)
            },
            new List<WeatherFieldCondition>() {
            }
        );


        public ActivitiesProcessor(string name, int weight, UnityEngine.Color color, int minWeekFrequencyPerYear, int minContinuousHours, IActivityCondition timeCondition, List<WeatherFieldCondition> hourlyConditions, List<WeatherFieldCondition> cumulatedHourConditions)
        {
            this.name = name;
            this.weight = weight;
            this.color = color;
            this.minWeekFrequencyPerYear = minWeekFrequencyPerYear;
            this.minContinousHours = minContinuousHours;
            this.condition = timeCondition;
            this.hourlyConditions = hourlyConditions;
            this.cumulatedHourConditions = cumulatedHourConditions;
        }

        private bool SuitsHour(string AAAAMMJJHH)
        {
            if (condition == null)
            {
                return true;
            }
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.Hour >= condition.minHour && dateTime.Hour <= condition.maxHour;
        }

        public bool Suits(WeatherRecord hourRecord, List<WeatherFieldCondition> conditions)
        {
            return conditions.All(condition => condition.Match(hourRecord));
        }

        public bool SuitsHour(WeatherRecord hourRecord)
        {
            return SuitsHour(hourRecord.Get(AttributeKey.AAAAMMJJHH)) && Suits(hourRecord, hourlyConditions);
        }

        public bool SuitsDay(IEnumerable<WeatherRecord> selectedHourRecordsForDay)
        {
            return selectedHourRecordsForDay.All(record => Suits(record, cumulatedHourConditions));
        }

        public IEnumerable<AttributeKey> Keys()
        {
            return hourlyConditions.SelectMany(c => c.keys).Union(cumulatedHourConditions.SelectMany(c => c.keys)).Distinct();
        }
    }
}