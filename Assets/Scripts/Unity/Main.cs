using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity {
    class Main {
        public Measure Rain = new Measure()

        public Activity tennis
        public static Activity Tennis = new Activity("Tennis", 1, balleTennis, 0, 3, new TimeCondition(9, 20),
            
            
            new List<WeatherFieldCondition>() { new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.T, WeatherRecordFieldKey.T10, WeatherRecordFieldKey.T20, WeatherRecordFieldKey.T50, WeatherRecordFieldKey.T100, }, (measures) => (min(measures)));
                new WeatherFieldCondition(new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.T, WeatherRecordFieldKey.T10, WeatherRecordFieldKey.T20, WeatherRecordFieldKey.T50, WeatherRecordFieldKey.T100,  }, 10, 27),
                new WeatherFieldCondition(new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.FF, WeatherRecordFieldKey.FF2, WeatherRecordFieldKey.FXI3S }, 0, 1)
            },
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(WeatherRecordFieldKey.RR1, 0, 0)
            }
        );
        public static Activity Velo = new Activity("Vélo", 1, asphalteClaire, 10, 2, new TimeCondition(14, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.T, WeatherRecordFieldKey.T10, WeatherRecordFieldKey.T20, WeatherRecordFieldKey.T50, WeatherRecordFieldKey.T100,  }, 17, 24),
                new WeatherFieldCondition(WeatherRecordFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.FF, WeatherRecordFieldKey.FF2, WeatherRecordFieldKey.FXI3S}, 0, 2) // Certaines valeurs ne sont pas disponibles dans les posts
            },
            new List<WeatherFieldCondition>() {
            }
        );
        public static Activity Plage = new Activity("Plage", 6, sable, 0, 2, new TimeCondition(11, 19),
            new List<WeatherFieldCondition>() {
                new WeatherFieldCondition(new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.T, WeatherRecordFieldKey.T10, WeatherRecordFieldKey.T20, WeatherRecordFieldKey.T50, WeatherRecordFieldKey.T100,  }, 23, 30),
                new WeatherFieldCondition(WeatherRecordFieldKey.RR1, 0, 0),
                new WeatherFieldCondition(new List<WeatherRecordFieldKey>() { WeatherRecordFieldKey.FF, WeatherRecordFieldKey.FF2, WeatherRecordFieldKey.FXI3S }, 0, 1)/*,
                new WeatherFieldCondition(new List<WeatherFieldKey>() { WeatherFieldKey.N, WeatherFieldKey.N1, WeatherFieldKey.NBAS }, 0, 3)*/
            },
            new List<WeatherFieldCondition>() {
            }
        );

        public Location tcQuimper = new PointLocation();
        public Location tcPontLabbe = new PointLocation();
        public Location plageIleTudy = new PolygonLocation();
        public Location procheMaison = new CircleLocation();


        public Planning PlanningTennis = new Planning();
        PlanningTennis.Add(new ActivityTracking(tennis, tcQuimper);
        PlanningTennis.Add(new ActivityTracking(tennis, tcPontLabbe);
        PlanningSortie.Add(new ActivityTracking(plage, plageIleTudy);
        PlanningSortie.Add(new ActivityTracking(vélo, procheMaison);
        
    }
}
