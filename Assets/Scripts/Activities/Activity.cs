using Assets.Resources.Weathers;
using Assets.Scripts;
using Assets.Scripts.Activities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Assets.Resources.Activities
{
    public class Activity
    {
        public string name;
        public int weight;
        public TimeCondition timeCondition;
        public int minContinousHours;
        public int minWeekFrequencyPerYear;
        public List<WeatherFieldCondition> hourlyConditions, cumulatedHourConditions;
        public UnityEngine.Color color;

        private static UnityEngine.Color
            VertJardin = ColorHelper.Get(58, 251, 3),
            bleuCiel = ColorHelper.Get(3, 251, 248),
            bleuMer = ColorHelper.Get(15, 75, 227),
            sable = ColorHelper.Get(241, 255, 0),
            asphalteClaire = ColorHelper.Get(234, 234, 234),
            balleTennis = ColorHelper.Get(218, 255, 0),
            murs = ColorHelper.Get(144, 144, 144),
            vertForet = ColorHelper.Get(29, 122, 29),
            crepuscule = ColorHelper.Get(255, 183, 0),
            rouge = ColorHelper.Get(255, 0, 0);

        public static Activity Moustiques1 = new Activity("Moustiques tigres (Matin)", -10, rouge, 0, 1, new TimeCondition(0, 8),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 18, 50),
            },
            new List<WeatherFieldCondition>()
            {
            }
        ); public static Activity Moustiques2 = new Activity("Moustiques tigres (Soir)", -10, rouge, 0, 1, new TimeCondition(21, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 18, 50),
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity TropChaud = new Activity("Trop chaud", -15, rouge, 0, 1, new TimeCondition(9, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 30, 50),
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity TropDePluie = new Activity("Trop de pluie", -5, rouge, 0, 1, new TimeCondition(0, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(WeatherFieldKey.RR1, 5, 1000),
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity Tempete = new Activity("Tempête", -5, rouge, 0, 1, new TimeCondition(0, 23),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S}, 4, 100)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity Jardin = new Activity("Jardin", 1, VertJardin, 0, 1, new TimeCondition(10, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 16, 30),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S}, 0, 3)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity Piscine = new Activity("Piscine", 4, bleuCiel, 0, 2, new TimeCondition(9, 22),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 25, 33),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 2)/*,
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)*/
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
            }
        );
        public static Activity Kayak = new Activity("Kayak", 2, bleuMer, 0, 3, new TimeCondition(14, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 23, 31),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S}, 0, 2)
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity Plage = new Activity("Plage", 3, sable, 0, 2, new TimeCondition(11, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 23, 30),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 1)/*,
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)*/
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static Activity Velo = new Activity("Vélo", 1, asphalteClaire, 10, 2, new TimeCondition(14, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 17, 24),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S}, 0, 3) // Certaines valeurs ne sont pas disponibles dans les posts
            },
            new List<WeatherFieldCondition>()
            {
            }
        );
        public static Activity Tennis = new Activity("Tennis", 1, balleTennis, 0, 3, new TimeCondition(9, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 10, 27),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 2)
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0)
            }
        );
        public static Activity Ville = new Activity("Ville", 1, murs, 0, 1, new TimeCondition(11, 20),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 0, 30),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 1),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 4)
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static Activity Randonnee = new Activity("Randonnée", 1, vertForet, 0, 2, new TimeCondition(13, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 10, 24),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 2)
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static Activity DinerExterieur = new Activity("Diner en extérieur", 4, crepuscule, 0, 2, new TimeCondition(18, 21),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.T, WeatherFieldKey.T10, WeatherFieldKey.T20, WeatherFieldKey.T50, WeatherFieldKey.T100,  }, 23, 28),
                new WeatherFieldCondition(WeatherFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.FF, WeatherFieldKey.FF2, WeatherFieldKey.FXI3S }, 0, 1)
            },
            new List<WeatherFieldCondition>() {
            }
        );


        public Activity(string name, int weight, UnityEngine.Color color, int minWeekFrequencyPerYear, int minContinuousHours, TimeCondition timeCondition, List<WeatherFieldCondition> hourlyConditions, List<WeatherFieldCondition> cumulatedHourConditions)
        {
            this.name = name;
            this.weight = weight;
            this.color = color;
            this.minWeekFrequencyPerYear = minWeekFrequencyPerYear;
            this.minContinousHours = minContinuousHours;
            this.timeCondition = timeCondition;
            this.hourlyConditions = hourlyConditions;
            this.cumulatedHourConditions = cumulatedHourConditions;
        }

        private bool SuitsHour(string AAAAMMJJHH)
        {
            if (timeCondition == null)
            {
                return true;
            }
            string format = "yyyyMMddHH";
            DateTime dateTime = DateTime.ParseExact(AAAAMMJJHH, format, CultureInfo.InvariantCulture);
            return dateTime.Hour >= timeCondition.minHour && dateTime.Hour <= timeCondition.maxHour;
        }

        public bool Suits(WeatherRecord hourRecord, List<WeatherFieldCondition> conditions)
        {
            return conditions.All(condition => condition.Match(hourRecord));
        }

        public bool SuitsHour(WeatherRecord hourRecord)
        {
            return SuitsHour(hourRecord.Get(WeatherFieldKey.AAAAMMJJHH)) && Suits(hourRecord, hourlyConditions);
        }

        public bool SuitsDay(IEnumerable<WeatherRecord> selectedHourRecordsForDay)
        {
            return selectedHourRecordsForDay.All(record => Suits(record, cumulatedHourConditions));
        }

        public IEnumerable<WeatherFieldKey> Keys()
        {
            return hourlyConditions.SelectMany(c => c.keys).Union(cumulatedHourConditions.SelectMany(c => c.keys)).Distinct();
        }
    }
}