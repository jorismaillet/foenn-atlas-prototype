using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Condition;
using Assets.Scripts.Foenn.Atlas.Models.Condition.Definitions;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Maps;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
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
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas
{
    public class Main : BaseHolder
    {
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

        }

        public IEnumerator TestETL()
        {
            MainThreadLog.Log("TestETL");
            var files = new List<string>() { "Weathers/Test.csv" };// "Weathers/H_29_latest-2023-2024.csv" };
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
            var result = new SqliteConnector().ExecuteQuery(new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Metric(WeatherHistoryMetricKey.T, AggregationKey.AVG))
                .Select(new Attribute(WeatherHistoryAttributeKey.NUM_POSTE))
                .GroupBy(WeatherHistoryAttributeKey.NUM_POSTE));
            //.Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.YEAR, "2019"))
            //.Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, "29"))

            foreach (var row in result.rows)
            {
                UnityEngine.Debug.Log($"Time: {row.time.start}");
                UnityEngine.Debug.Log($"Location: {row.geo.numPost}");
                foreach (AttributeValue attr in row.attributes)
                    UnityEngine.Debug.Log($"Attribute: {attr.attribute.key} - {attr.value}");
                foreach (var measure in row.measures)
                    UnityEngine.Debug.Log($"Measure: ({measure.metric.aggregation}) {measure.metric.key} - {measure.value}");
            }
        }

        //SqliteException: The database disk image is malformed
        //database disk image is malformed
        //Mono.Data.Sqlite.SQLite3.Reset(Mono.Data.Sqlite.SqliteStatement stmt) (at<99f7d8f9a65b4d91ae5b1d55c424866f>:0)
        //Mono.Data.Sqlite.SQLite3.Step(Mono.Data.Sqlite.SqliteStatement stmt) (at<99f7d8f9a65b4d91ae5b1d55c424866f>:0)
        //Mono.Data.Sqlite.SqliteDataReader.NextResult() (at<99f7d8f9a65b4d91ae5b1d55c424866f>:0)
        //(wrapper remoting-invoke-with-check) Mono.Data.Sqlite.SqliteDataReader.NextResult()
        //Mono.Data.Sqlite.SqliteDataReader..ctor(Mono.Data.Sqlite.SqliteCommand cmd, System.Data.CommandBehavior behave) (at<99f7d8f9a65b4d91ae5b1d55c424866f>:0)
        //(wrapper remoting-invoke-with-check) Mono.Data.Sqlite.SqliteDataReader..ctor(Mono.Data.Sqlite.SqliteCommand, System.Data.CommandBehavior)
        //Mono.Data.Sqlite.SqliteCommand.ExecuteReader(System.Data.CommandBehavior behavior) (at<99f7d8f9a65b4d91ae5b1d55c424866f>:0)
        //Mono.Data.Sqlite.SqliteCommand.ExecuteNonQuery() (at<99f7d8f9a65b4d91ae5b1d55c424866f>:0)
        //Assets.Scripts.Foenn.Engine.Connectors.SqlConnector.ExecuteOperation(System.String sql) (at Assets/Scripts/Foenn/Engine/Connectors/SqlConnector.cs:30)
        //Assets.Scripts.Foenn.Engine.Connectors.SqliteConnector.DropStagingTable(Assets.Scripts.Foenn.ETL.Models.SchemaDefinition schema) (at Assets/Scripts/Foenn/Engine/Connectors/SqliteConnector.cs:205)
        //Assets.Scripts.Foenn.ETL.ETLProcessor.ProcessETL(System.Threading.CancellationToken cancellationToken) (at Assets/Scripts/Foenn/ETL/ETLProcessor.cs:78)
        //Assets.Scripts.Foenn.Atlas.Main+<>c__DisplayClass7_0.<LoadFile>b__0() (at Assets/Scripts/Foenn/Atlas/Main.cs:176)
        //System.Threading.Tasks.Task.InnerInvoke() (at<8ce0bd04a7a04b4b9395538239d3fdd8>:0)
        //System.Threading.Tasks.Task.Execute() (at<8ce0bd04a7a04b4b9395538239d3fdd8>:0)
        //Rethrow as AggregateException: One or more errors occurred. (The database disk image is malformed
        //database disk image is malformed)
        //UnityEngine.Debug:LogException(Exception)
        //Assets.Scripts.Foenn.Atlas.<LoadFile>d__7:MoveNext() (at Assets/Scripts/Foenn/Atlas/Main.cs:182)
        //UnityEngine.SetupCoroutine:InvokeMoveNext(IEnumerator, IntPtr)

        void Start()
        {
            Env.SetDatabasePath(SqliteConnector.DATABASE_PATH);
            UnityEngine.Debug.Log("ok");
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