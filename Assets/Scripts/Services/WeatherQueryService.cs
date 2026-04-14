using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.Services
{
    public class WeatherQueryService
    {
        private static WeatherHistoryDataset Dataset => WeatherHistoryDataset.Instance;

        private static int ResolveLocationId(string postName)
        {
            var dataset = Dataset;
            return Convert.ToInt32(TableService.ValueFor(
                dataset.location,
                dataset.location.PrimaryKey,
                dataset.location.PostName,
                postName));
        }

        private static QueryRequest BuildGeoGroupedQuery(int year)
        {
            var dataset = Dataset;
            var coreFact = dataset.coreFact;

            return new QueryRequest(coreFact)
                .SelectGroup(
                    dataset.location.PrimaryKey,
                    dataset.location.Longitude,
                    dataset.location.Latitude,
                    dataset.location.PostName)
                .Join(coreFact.locationRef)
                .Join(coreFact.timeRef)
                .WhereEq(dataset.time.year, year);
        }

        public static List<GeoMeasure> HoursWithoutRain(int year)
        {
            var dataset = Dataset;
            var coreFact = dataset.coreFact;
            var rainField = coreFact.rain;

            var query = BuildGeoGroupedQuery(year)
                .SelectCount(coreFact.PrimaryKey)
                .WhereBetween(dataset.time.hour, 9, 22)
                .WhereEq(rainField, 0);

            using var connection = SqliteHelper.CreateConnection();
            var result = query.Execute(connection);
            return result.ToPostMeasures(dataset.location, coreFact.PrimaryKey);
        }

        public static List<GeoMeasure> YearMeasure(int year, Field coreFactField, FieldAggregation aggregation)
        {
            var dataset = Dataset;
            var yearlyFact = dataset.yearlyFact;
            var aggregatedField = yearlyFact.aggregatedField[(coreFactField, aggregation)];

            var query = new QueryRequest(yearlyFact)
                .SelectGroup(
                    dataset.location.PrimaryKey,
                    dataset.location.Longitude,
                    dataset.location.Latitude,
                    dataset.location.PostName)
                .Select(aggregatedField)
                .Join(yearlyFact.locationRef)
                .WhereEq(yearlyFact.year, year)
                .WhereNotNull(aggregatedField);

            using var connection = SqliteHelper.CreateConnection();
            var result = query.Execute(connection);
            return result.ToPostMeasures(dataset.location, aggregatedField);
        }

        public static List<Row> DayObservationsForPost(string postName, int dayOfMonth, int month, int year)
        {
            var dataset = Dataset;
            var coreFact = dataset.coreFact;
            int locationId = ResolveLocationId(postName);

            var query = new QueryRequest(coreFact)
                .SelectGroup(dataset.time.hour)
                .Select(
                    coreFact.temperature,
                    coreFact.rain,
                    coreFact.windSpeed)
                .Join(coreFact.timeRef)
                .WhereEq(coreFact.locationRef, locationId)
                .WhereEq(dataset.time.day, dayOfMonth)
                .WhereEq(dataset.time.month, month)
                .WhereEq(dataset.time.year, year)
                .OrderByAsc(dataset.time.hour);

            using var connection = SqliteHelper.CreateConnection();
            return query.Execute(connection).rows;
        }

        public static List<float> HoursTempForYear(string postName, int year)
        {
            var dataset = Dataset;
            var coreFact = dataset.coreFact;
            int locationId = ResolveLocationId(postName);

            var query = new QueryRequest(coreFact)
                .Select(coreFact.temperature)
                .Join(coreFact.timeRef)
                .WhereEq(coreFact.locationRef, locationId)
                .WhereEq(dataset.time.year, year)
                .WhereNotNull(coreFact.temperature)
                .OrderByAsc(dataset.time.month)
                .OrderByAsc(dataset.time.day)
                .OrderByAsc(dataset.time.hour);

            using var connection = SqliteHelper.CreateConnection();
            var result = query.Execute(connection);
            return result.rows
                .Select(r => r.FloatValue(coreFact.temperature))
                .ToList();
        }

        public static List<GeoMeasure> HoursOfActivity(Activity activity, int year)
        {
            var dataset = Dataset;
            var coreFact = dataset.coreFact;

            var query = BuildGeoGroupedQuery(year)
                .SelectCount(coreFact.PrimaryKey);

            activity.AddToQuery(query);

            using var connection = SqliteHelper.CreateConnection();
            var result = query.Execute(connection);
            return result.ToPostMeasures(dataset.location, coreFact.PrimaryKey);
        }

        public static List<int> HoursForActivity(Activity activity, string postName, int year, int month, int dayOfMonth)
        {
            var dataset = Dataset;
            var coreFact = dataset.coreFact;
            int locationId = ResolveLocationId(postName);

            var query = new QueryRequest(coreFact)
                .SelectGroup(dataset.time.hour)
                .Join(coreFact.timeRef)
                .WhereEq(coreFact.locationRef, locationId)
                .WhereEq(dataset.time.day, dayOfMonth)
                .WhereEq(dataset.time.month, month)
                .WhereEq(dataset.time.year, year)
                .OrderByAsc(dataset.time.hour);

            activity.AddToQuery(query);

            using var connection = SqliteHelper.CreateConnection();
            var result = query.Execute(connection);
            return result.rows
                .Select(r => r.IntValue(dataset.time.hour))
                .ToList();
        }
    }
}
