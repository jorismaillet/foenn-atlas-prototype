using Assets.Scripts.Foenn.Atlas.Models;
using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Activities.Conditions;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Maps;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.ETL.SqLite;
using System.Reflection;
using UnityEngine;

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

            var pasDePluie = new MetricGroupCondition(temp, 0, 0, ConditionImportanceKey.HIGH);
            var peuDeVent = new MetricGroupCondition(wind, 0, 50, ConditionImportanceKey.HIGH);
            var peuDeRafales = new MetricGroupCondition(gust, 0, 30, ConditionImportanceKey.LOW);

            var beauTemps = new WeatherCategory("Beau temps", pasDePluie, peuDeRafales, peuDeVent);

            var brest = new PointLocation("Brest", new GeoPoint(48.3904, -4.4861));
            public Location tcQuimper = new PointLocation("TC Quimper", new GeoPoint(48.3904, -4.4861));
            public Location tcPontLabbe = new PointLocation("TC Pont l'Abbé", new GeoPoint(48.3904, -4.4861));
            public Location maison = new PointLocation("Maison", new GeoPoint(48.3904, -4.4861));
            public Location procheMaison = new CircleLocation("Proche Maison", maison);
            public Location plageIleTudy = new PolygonLocation("Plage Ile Tudy", new GeoPoint(48.3904, -4.4861), new GeoPoint(48.3904, -4.4861), new GeoPoint(48.3904, -4.4861));

        var piscine = new Activity("Piscine", maison, new MetricGroupCondition(temp, 25, 33, ConditionImportanceKey.HIGH),
                new MetricGroupCondition(wind, 0, 2, ConditionImportanceKey.HIGH),
                new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH));

            var kayak = new Activity("Kayak", plageIleTudy,
    new MetricGroupCondition(temp, 23, 31, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 2, ConditionImportanceKey.HIGH));

            var plage = new Activity("Plage", plageIleTudy, new MetricGroupCondition(temp, 23, 30, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 1, ConditionImportanceKey.HIGH));

            var velo = new Activity("Vélo", procheMaison, new MetricGroupCondition(temp, 17, 24, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 2, ConditionImportanceKey.HIGH));

            var jardin = new Activity("Jardin", maison, new MetricGroupCondition(temp, 16, 30, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 2, ConditionImportanceKey.HIGH));

            var tennis = new Activity("Tennis", brest, new MetricGroupCondition(temp, 10, 27, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 1, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH));

            var ville = new Activity("Ville", brest, new MetricGroupCondition(temp, 0, 30, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 3, ConditionImportanceKey.HIGH));

            var randonee = new Activity("Randonnée", brest, new MetricGroupCondition(temp, 10, 24, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
    new MetricGroupCondition(wind, 0, 1, ConditionImportanceKey.HIGH));

            var dinner = new Activity("Diner en extérieur", brest, new MetricGroupCondition(temp, 23, 28, ConditionImportanceKey.HIGH),
                new MetricGroupCondition(rain, 0, 0, ConditionImportanceKey.HIGH),
                new MetricGroupCondition(wind, 0, 0, ConditionImportanceKey.HIGH));


        var planningSportif = new PlanningDefinition();
        planningSportif.Add(new PlannedActivity(randonee, procheMaison);
        planningSportif.Add(new PlannedActivity(piscine, maison);
        planningSportif.Add(new PlannedActivity(tennis, tcQuimper);
        planningSportif.Add(new PlannedActivity(tennis, tcPontLabbe);
        planningSportif.Add(new PlannedActivity(velo, procheMaison);
        planningSportif.Add(new PlannedActivity(kayak, plageIleTudy);

        var ideesDeSorties = new PlanningDefinition();
        ideesDeSorties.Add(new PlannedActivity(plage, plageIleTudy);
        ideesDeSorties.Add(new PlannedActivity(jardin, maison);
        ideesDeSorties.Add(new PlannedActivity(ville, brest);
        ideesDeSorties.Add(new PlannedActivity(dinner, brest);

        var map = new Map();

            // 2. Upsert SqLite from CSV
            string dbPath = "weather.db";
            string csvPath = "Assets/Resources/Weathers/H_29_latest.csv";
            string tableName = "HourlyWeatherData";
            SqLiteDataLoader.CreateTableFromCsvHeader(dbPath, tableName, csvPath);
            SqLiteDataLoader.PopulateTableFromCsvContent(dbPath, tableName, csvPath);

            // 3. Run SqLite query
            // Build a QueryRequest (example: AVG temperature)
            var request = new QueryRequest();
            request.Filters.Add(new DataFilter(FilterMode.INCLUDE, AttributeKey.YEAR, "2019"));
            request.Filters.Add(new DataFilter(FilterMode.INCLUDE, AttributeKey.DPT, "29"));
            request.attributes.Add(new Attribute(AttributeKey.MONTH));
            request.metrics.Add(new Metric(MetricKey.T, AggregationKey.AVG));

            // Use a provider (example: SQLite)
            var provider = new SqLiteProvider(dbPath);
            provider.Initialize(request);
            var result = provider.Execute();

            foreach (var dim in result.dimensions)
            {
                foreach (var attr in dim.attributeValues)
                    Debug.Log($"Attr: {attr.value}");
                foreach (var metric in dim.metricValues)
                    Debug.Log($"Metric: {metric.value}");
            }
        }
    }
}