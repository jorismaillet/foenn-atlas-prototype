using Assets.Scripts.Foenn.Atlas.Models;
using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Maps;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.ETL.CSV;
using Assets.Scripts.Foenn.ETL.Loaders;

namespace Assets.Scripts.Foenn.Atlas
{
    public class Main
    {
        public void TestScenario()
        {
            var temp = new MetricGroup("Temperature", MetricKey.T, MetricKey.T10, MetricKey.T20, MetricKey.T50, MetricKey.T100);
            var rain = new MetricGroup("Pluie", MetricKey.RR1);
            var wind = new MetricGroup("Vent", MetricKey.FF, MetricKey.FF2);
            var gust = new MetricGroup("Rafales", MetricKey.FXI, MetricKey.FXI2, MetricKey.FXI3S);

            var pasDePluie = new GroupAllCondition(rain, 0, 0);
            var peuDeVent = new GroupAnyCondition(wind, 0, 50);
            var peuDeRafales = new GroupAnyCondition(gust, 0, 30);

            var beauTemps = new NamedCondition("Beau temps", new AllCondition(pasDePluie, peuDeRafales, peuDeVent));

            var brest = new PointLocation("Brest", new GeoPoint(48.3904, -4.4861));

            Location tcQuimper = new PointLocation("TC Quimper", new GeoPoint(48.3904, -4.4861));
            Location tcPontLabbe = new PointLocation("TC Pont l'Abbé", new GeoPoint(48.3904, -4.4861));
            PointLocation maison = new PointLocation("Maison", new GeoPoint(48.3904, -4.4861));
            Location procheMaison = new CircleLocation("Proche Maison", maison, 5000);
            Location plageIleTudy = new PolygonLocation("Plage Ile Tudy", new GeoPoint(48.3904, -4.4861), new GeoPoint(48.3904, -4.4861), new GeoPoint(48.3904, -4.4861));

            var piscine = new Activity("Piscine", beauTemps, new GroupAllCondition(temp, 25, 33));

            var kayak = new Activity("Kayak", beauTemps);

            var plage = new Activity("Plage", beauTemps, new GroupAllCondition(temp, 25, 33), new HourRangeCondition(14, 18));

            var velo = new Activity("Vélo", new GroupAllCondition(temp, 17, 24),
                new GroupAllCondition(rain, 0, 0),
                new GroupAllCondition(wind, 0, 2));

            var jardin = new Activity("Jardin", new GroupAllCondition(temp, 16, 30),
                new GroupAllCondition(rain, 0, 0),
                new GroupAllCondition(wind, 0, 2));

            var tennis = new Activity("Tennis", new GroupAllCondition(temp, 10, 27),
                new GroupAllCondition(wind, 0, 1),
                new GroupAllCondition(rain, 0, 0));

            var ville = new Activity("Ville", new GroupAllCondition(temp, 0, 30),
                new GroupAllCondition(rain, 0, 0),
                new GroupAllCondition(wind, 0, 3));

            var randonee = new Activity("Randonnée", new GroupAllCondition(temp, 10, 24),
                new GroupAllCondition(rain, 0, 0),
                new GroupAllCondition(wind, 0, 1));

            var dinner = new Activity("Diner en extérieur", beauTemps, new HourRangeCondition(18, 22));


            var planningSportif = new Planning();
                planningSportif.plannedActivities.Add(new PlannedActivity(randonee, procheMaison));
                planningSportif.plannedActivities.Add(new PlannedActivity(piscine, maison));
                planningSportif.plannedActivities.Add(new PlannedActivity(tennis, tcQuimper));
                planningSportif.plannedActivities.Add(new PlannedActivity(tennis, tcPontLabbe));
                planningSportif.plannedActivities.Add(new PlannedActivity(velo, procheMaison));
                planningSportif.plannedActivities.Add(new PlannedActivity(kayak, plageIleTudy));

            var ideesDeSorties = new Planning();
                ideesDeSorties.plannedActivities.Add(new PlannedActivity(plage, plageIleTudy));
                ideesDeSorties.plannedActivities.Add(new PlannedActivity(jardin, maison));
                ideesDeSorties.plannedActivities.Add(new PlannedActivity(ville, brest));
                ideesDeSorties.plannedActivities.Add(new PlannedActivity(dinner, brest));

            var map = new Map();

            // 2. Upsert SqLite from CSV
            new ETLProcessor(
                new WeatherHistoryDatasource(),
                new CSVExtractor("Weathers/H_29_latest-2023-2024"),
                new SqliteLoader(new WeatherHistoryDatasource())
            ).ProcessETL();

            // 3. Run SqLite query
            // Build a QueryRequest (example: AVG temperature)
            var result = new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Metric(MetricKey.T, AggregationKey.AVG))
                .Where(new DataFilter(DataFilterMode.INCLUDE, AttributeKey.YEAR, "2019"))
                .Where(new DataFilter(DataFilterMode.INCLUDE, AttributeKey.DPT, "29"))
                .GroupBy(AttributeKey.MONTH)
                .ExecuteOnce(new SqliteConnector());

            foreach (var header in result.rawHeaders)
            {
                UnityEngine.Debug.Log($"Header: {header}");
            }
            foreach (var row in result.rows)
            {
                UnityEngine.Debug.Log($"Time: {row.time.start}");
                UnityEngine.Debug.Log($"Location: {row.geo.numPost}");
                foreach (Attribute attr in row.attributes)
                    UnityEngine.Debug.Log($"Attribute: {attr.key} - {attr.value}");
                foreach (var measure in row.measures)
                    UnityEngine.Debug.Log($"Measure: ({measure.metric.aggregation}) {measure.metric.key} - {measure.value}");
            }
        }
    }
}