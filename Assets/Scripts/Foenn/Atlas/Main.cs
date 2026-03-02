using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Maps;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap;
using Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Extractors;
using Assets.Scripts.Foenn.ETL.Loaders;
using Assets.Scripts.Foenn.ETL.Transformers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Commons.Behaviours;
using Assets.Scripts.Unity.Commons.Holders;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Foenn.Atlas
{
    public class Main : BaseHolder
    {
        public RawImage pointmapImage;
        public RawImage heatmapImage;

        public void TestModels()
        {
            var temp = new MetricGroup("Temperature", WeatherHistoryMetricKey.T, WeatherHistoryMetricKey.T10, WeatherHistoryMetricKey.T20, WeatherHistoryMetricKey.T50, WeatherHistoryMetricKey.T100);
            var rain = new MetricGroup("Pluie", WeatherHistoryMetricKey.RR1);
            var wind = new MetricGroup("Vent", WeatherHistoryMetricKey.FF, WeatherHistoryMetricKey.FF2);
            var gust = new MetricGroup("Rafales", WeatherHistoryMetricKey.FXI, WeatherHistoryMetricKey.FXI2, WeatherHistoryMetricKey.FXI3S);

            var pasDePluie = new GroupAllCondition(rain, 0, 0);
            var peuDeVent = new GroupAnyCondition(wind, 0, 50);
            var peuDeRafales = new GroupAnyCondition(gust, 0, 30);

            var beauTemps = new NamedCondition("Beau temps", new AllCondition(pasDePluie, peuDeRafales, peuDeVent));

            var brest = new PointLocation("Brest", new GeoPoint(48.3904F, -4.4861F));

            Location tcQuimper = new PointLocation("TC Quimper", new GeoPoint(48.3904F, -4.4861F));
            Location tcPontLabbe = new PointLocation("TC Pont l'Abbé", new GeoPoint(48.3904F, -4.4861F));
            PointLocation maison = new PointLocation("Maison", new GeoPoint(48.3904F, -4.4861F));
            Location procheMaison = new CircleLocation("Proche Maison", maison, 5000);
            Location plageIleTudy = new PolygonLocation("Plage Ile Tudy", new GeoPoint(48.3904F, -4.4861F), new GeoPoint(48.3904F, -4.4861F), new GeoPoint(48.3904F, -4.4861F));

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

        }

        public IEnumerator TestETL()
        {
            MainThreadLog.Log("TestETL");
            var files = new List<string>() { "Weathers/H_29_latest-2023-2024.csv" };
            var datasource = new WeatherHistoryDatasource();
            foreach (var file in files)
            {
                MainThreadLog.Log(file);
                yield return LoadFile(datasource, file);
            }
            MainThreadLog.Log("Done");
        }

        public void TestEngine() {
            // 3. Run SqLite query
            // Build a QueryRequest (example: AVG temperature)
            var dataPoints = TemperatureDataPoints("2023010110");

            UnityEngine.Debug.Log("Temperature data point for france for 2023/01/01 at 10am");
            foreach (var row in dataPoints.rows)
            {
                //UnityEngine.Debug.Log($"Time: {row.time.start}");
                //UnityEngine.Debug.Log($"Location: {row.geo.numPost}");
                foreach (AttributeValue attr in row.attributes)
                    UnityEngine.Debug.Log($"Attribute: {attr.attribute.key} - {attr.value}");
                foreach (var measure in row.measures)
                    UnityEngine.Debug.Log($"Measure: ({measure.metric.aggregation}) {measure.metric.key} - {measure.value}");
            }
        }

        private QueryResult TemperatureDataPoints(string AAAAMMJJHH)
        {
            return new SqliteConnector().ExecuteQuery(new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Attribute(WeatherHistoryAttributeKey.LON))
                .Select(new Attribute(WeatherHistoryAttributeKey.LAT))
                .Select(new Attribute(WeatherHistoryAttributeKey.NOM_USUEL))
                .Select(new Metric(WeatherHistoryMetricKey.T))
                .Where(new ExcludeNullFilter(WeatherHistoryMetricKey.T))
                .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.AAAAMMJJHH, AAAAMMJJHH)));
        }


        private void TestRender()
        {
            var renderSettings = new Visualisations.RenderSettings(1024, 1024, BBox.France);
            var heatmapSettings = new HeatmapSettings(renderSettings, 2f, 16, 120f, 0.85f, -10f, 40f, 32);
            var pointmapSettings = new PointmapSettings(renderSettings, PointmapSettings.defaultColor);

            // Optionnel: masque France (PNG N/B importé en Texture2D)
            Texture2D mask = null;// franceMaskTex;
            List<GeoMeasure> measures = GetMeasuresFor("2023010110", WeatherHistoryMetricKey.T);
            Texture2D heatmapTexture = HeatmapGenerator.BuildHeatmapTexture(measures, heatmapSettings, mask);
            Texture2D pointmapTexture = PointmapGenerator.BuildPointmapTexture(measures, pointmapSettings, mask);

            pointmapImage.texture = pointmapTexture;
            heatmapImage.texture = heatmapTexture;
        }

        private List<GeoMeasure> GetMeasuresFor(string AAAAMMJJHH, WeatherHistoryMetricKey metricKey)
        {
            var res = new List<GeoMeasure>();
            foreach (var item in TemperatureDataPoints("2023010110").rows)
            {
                string station = item.attributes.Find(a => a.attribute.key.Equals(WeatherHistoryAttributeKey.NOM_USUEL)).value;
                float lon = float.Parse(item.attributes.Find(a => a.attribute.key.Equals(WeatherHistoryAttributeKey.LON)).value, CultureInfo.InvariantCulture);
                float lat = float.Parse(item.attributes.Find(a => a.attribute.key.Equals(WeatherHistoryAttributeKey.LAT)).value, CultureInfo.InvariantCulture);
                var measure = item.measures.Find(a => a.metric.key.Equals(WeatherHistoryMetricKey.T));
                if (measure == null)
                {
                    throw new System.Exception("measure not found");
                }
                float? temp = item.measures.Find(a => a.metric.key.Equals(WeatherHistoryMetricKey.T)).value;
                if (!temp.HasValue)
                {
                    throw new System.Exception("no T found for " + station);
                }
                res.Add(new GeoMeasure(new GeoPoint(lat, lon), temp.Value));
            }
            return res;
        }

        void Start()
        {
            Env.SetDatabasePath(SqliteConnector.DATABASE_PATH);
            UnityEngine.Debug.Log("ok");

            TestRender();

            return;

            TestModels();
            StartCoroutine(TestETL());
            TestEngine();
        }

        private CancellationTokenSource ct;
        private Task task;

        void OnApplicationQuit()
        {
            ct?.Cancel();
            ct?.Dispose();
            ct = null;

            if (SqlConnector.connection != null)
            {
                SqlConnector.connection.Close();
                SqlConnector.connection.Dispose();
                SqlConnector.connection = null;
            }
        }

        public IEnumerator LoadFile(Datasource datasource, string file)
        {
            MainThreadLog.Log("ETLProcessor");
            var processor = new ETLProcessor(
                    datasource,
                    new CSVExtractor(file),
                    new Transformer(datasource),
                    new SqliteLoader(datasource)
                );
            if (!processor.ShouldProcess())
                yield break;

            MainThreadLog.Log("Process");
            ct = new CancellationTokenSource();
            task = Task.Run(() => processor.ProcessETL(ct.Token), ct.Token);

            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                UnityEngine.Debug.LogException(task.Exception);

            UnityEngine.Debug.Log("ETL finished: " + file);
        }
    }
}