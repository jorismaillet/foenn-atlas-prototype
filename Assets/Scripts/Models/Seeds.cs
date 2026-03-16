using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Condition;
using Assets.Scripts.Models.Condition.Definitions;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.Models.Plannings;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;

namespace Assets.Scripts
{
    public class Seeds
    {
        public static FloatRangeCondition noRain = new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.rain, 0, 0);

        public static FloatRangeCondition lowWind = new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.windSpeed, 0, 20);

        public static FloatRangeCondition lowGust = new FloatRangeCondition(WeatherHistoryDataset.Instance.windFact.gust, 0, 20);

        public static NamedCondition niceWeather = new NamedCondition("Beau temps", new AllCondition(noRain, lowGust, lowWind));

        public static PointLocation brestCenter = new PointLocation("Brest", 48.3874F, -4.4952F);

        public static PointLocation quimperTennis = new PointLocation("TC Quimper", 47.9729F, -4.0982F);

        public static PointLocation pontLabbeTennis = new PointLocation("TC Pont l'Abbé", 47.8708F, -4.2192F);

        public static PointLocation plomelinCenter = new PointLocation("Centre de Plomelin", 47.9369F, -4.1544F);

        public static CircleLocation plomelinSurroundings = new CircleLocation("Alentours de Plomelin", plomelinCenter, 5000);

        public static PolygonLocation tudyBeach = new PolygonLocation("Plage Ile Tudy", new GeoPoint(47.8518F, -4.1495F), new GeoPoint(47.8465F, -4.1617F), new GeoPoint(47.8383F, -4.1661F), new GeoPoint(47.8412F, -4.1726F), new GeoPoint(47.8515F, -4.1682F));

        public static Activity swimming = new Activity("Piscine", niceWeather, new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 25, 33));

        public static Activity kayak = new Activity("Kayak", niceWeather);

        public static Activity beach = new Activity("Plage", niceWeather, new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 25, 33), new IntRangeCondition(WeatherHistoryDataset.Instance.time.hour, 14, 18));

        public static Activity biking = new Activity("Vélo", new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 17, 24),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.rain, 0, 0),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.windSpeed, 0, 2));

        public static Activity gardening = new Activity("Jardin", new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 16, 30),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.rain, 0, 0),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.windSpeed, 0, 2));

        public static Activity tennis = new Activity("Tennis", new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 10, 27),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.rain, 0, 1),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.windSpeed, 0, 0));

        public static Activity cityPromenade = new Activity("Promenade en ville", new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 0, 30),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.rain, 0, 0),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.windSpeed, 0, 3));

        public static Activity hiking = new Activity("Randonnée", new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.temperature, 10, 24),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.rain, 0, 0),
            new FloatRangeCondition(WeatherHistoryDataset.Instance.coreFact.windSpeed, 0, 1));

        public static Activity dinner = new Activity("Diner en extérieur", niceWeather, new IntRangeCondition(WeatherHistoryDataset.Instance.time.hour, 18, 22));

        public static Planning sportsPlanning = new Planning("Planning sportif",
            new PlannedActivity(hiking, plomelinSurroundings),
            new PlannedActivity(swimming, plomelinCenter),
            new PlannedActivity(tennis, quimperTennis),
            new PlannedActivity(tennis, pontLabbeTennis),
            new PlannedActivity(biking, plomelinSurroundings),
            new PlannedActivity(kayak, tudyBeach)
        );

        public static Planning outsideActivities = new Planning("Idées de sorties",
            new PlannedActivity(beach, tudyBeach),
            new PlannedActivity(gardening, plomelinCenter),
            new PlannedActivity(cityPromenade, brestCenter),
            new PlannedActivity(dinner, brestCenter)
        );
    }
}
