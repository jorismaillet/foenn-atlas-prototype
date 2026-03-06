using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;

namespace Assets.Scripts.Foenn.Atlas
{
    public class Seed
    {
        public static MetricGroup temp = new MetricGroup("Temperature", WeatherHistoryMetricKey.T, WeatherHistoryMetricKey.T10, WeatherHistoryMetricKey.T20, WeatherHistoryMetricKey.T50, WeatherHistoryMetricKey.T100);
        public static MetricGroup rain = new MetricGroup("Pluie", WeatherHistoryMetricKey.RR1);
        public static MetricGroup wind = new MetricGroup("Vent", WeatherHistoryMetricKey.FF, WeatherHistoryMetricKey.FF2);
        public static MetricGroup gust = new MetricGroup("Rafales", WeatherHistoryMetricKey.FXI, WeatherHistoryMetricKey.FXI2, WeatherHistoryMetricKey.FXI3S);

        public static GroupAllCondition pasDePluie = new GroupAllCondition(rain, 0, 0);
        public static GroupAnyCondition peuDeVent = new GroupAnyCondition(wind, 0, 50);
        public static GroupAnyCondition peuDeRafales = new GroupAnyCondition(gust, 0, 30);

        public static NamedCondition beauTemps = new NamedCondition("Beau temps", new AllCondition(pasDePluie, peuDeRafales, peuDeVent));

        public static PointLocation brest = new PointLocation("Brest", 48.3874F, -4.4952F);

        public static PointLocation tcQuimper = new PointLocation("TC Quimper",47.9729F, -4.0982F);
        public static PointLocation tcPontLabbe = new PointLocation("TC Pont l'Abbé", 47.8708F, -4.2192F);
        public static PointLocation maison = new PointLocation("Maison", 47.9369F, -4.1544F);
        public static CircleLocation procheMaison = new CircleLocation("Proche Maison", maison, 5000);
        public static PolygonLocation plageIleTudy = new PolygonLocation("Plage Ile Tudy", new GeoPoint(47.8518F, -4.1495F), new GeoPoint(47.8465F, -4.1617F), new GeoPoint(47.8383F, -4.1661F), new GeoPoint(47.8412F, -4.1726F), new GeoPoint(47.8515F, -4.1682F));

        public static Activity piscine = new Activity("Piscine", beauTemps, new GroupAllCondition(temp, 25, 33));
        public static Activity kayak = new Activity("Kayak", beauTemps);
        public static Activity plage = new Activity("Plage", beauTemps, new GroupAllCondition(temp, 25, 33), new HourRangeCondition(14, 18));

        public static Activity velo = new Activity("Vélo", new GroupAllCondition(temp, 17, 24),
            new GroupAllCondition(rain, 0, 0),
            new GroupAllCondition(wind, 0, 2));

        public static Activity jardin = new Activity("Jardin", new GroupAllCondition(temp, 16, 30),
            new GroupAllCondition(rain, 0, 0),
            new GroupAllCondition(wind, 0, 2));

        public static Activity tennis = new Activity("Tennis", new GroupAllCondition(temp, 10, 27),
            new GroupAllCondition(wind, 0, 1),
            new GroupAllCondition(rain, 0, 0));

        public static Activity ville = new Activity("Ville", new GroupAllCondition(temp, 0, 30),
            new GroupAllCondition(rain, 0, 0),
            new GroupAllCondition(wind, 0, 3));

        public static Activity randonee = new Activity("Randonnée", new GroupAllCondition(temp, 10, 24),
            new GroupAllCondition(rain, 0, 0),
            new GroupAllCondition(wind, 0, 1));

        public static Activity dinner = new Activity("Diner en extérieur", beauTemps, new HourRangeCondition(18, 22));

        public static Planning planningSportif = new Planning("Planning sportif",
            new PlannedActivity(randonee, procheMaison),
            new PlannedActivity(piscine, maison),
            new PlannedActivity(tennis, tcQuimper),
            new PlannedActivity(tennis, tcPontLabbe),
            new PlannedActivity(velo, procheMaison),
            new PlannedActivity(kayak, plageIleTudy)
        );
        public static Planning ideesDeSorties = new Planning("Idées de sorties",
            new PlannedActivity(plage, plageIleTudy),
            new PlannedActivity(jardin, maison),
            new PlannedActivity(ville, brest),
            new PlannedActivity(dinner, brest)
        );
    }
}
