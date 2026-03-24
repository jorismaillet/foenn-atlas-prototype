using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Database;
using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.Services
{
    public class WeatherQueryService
    {
        public static List<GeoMeasure> HoursWithoutRain(int year)
        {
            var dataset = WeatherHistoryDataset.Instance;
            var coreFact = dataset.coreFact;
            var fieldToMeasure = dataset.coreFact.rain;
            var query = new QueryRequest(coreFact)
                .SelectGroup(
                    dataset.location.Longitude,
                    dataset.location.Latitude,
                    dataset.location.PostName)
                .SelectCount(coreFact.PrimaryKey)
                .Join(coreFact.locationRef)
                .Join(coreFact.timeRef)
                .WhereEq(dataset.time.year, year)
                .WhereBetween(dataset.time.hour, 9, 22)
                .WhereEq(fieldToMeasure, 0)
                .WhereNotNull(fieldToMeasure);
            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                return result.ToPostMeasures(dataset.location, coreFact.PrimaryKey);
            }
        }
        public static List<GeoMeasure> YearMeasure(int year, Field coreFactField, string aggregation)
        {
            var dataset = WeatherHistoryDataset.Instance;
            var coreFact = dataset.coreFact;
            var query = new QueryRequest(coreFact)
                .SelectGroup(
                    dataset.location.Longitude,
                    dataset.location.Latitude,
                    dataset.location.PostName)
                .Join(coreFact.locationRef)
                .Join(coreFact.timeRef)
                .WhereEq(dataset.time.year, year)
                .WhereNotNull(coreFactField);

            if (aggregation == "Minimum") query = query.SelectMin(coreFactField);
            else if (aggregation == "Maximum") query = query.SelectMax(coreFactField);
            else query = query.SelectAvg(coreFactField);

            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                return result.ToPostMeasures(dataset.location, coreFactField);
            }
        }
        public static List<Row> DayObservationsForPost(string postName, int dayOfMonth, int month, int year)
        {
            var dataset = WeatherHistoryDataset.Instance;
            int locationId = Convert.ToInt32(TableService.ValueFor(
                dataset.location,
                dataset.location.PrimaryKey,
                dataset.location.PostName,
                postName));
            var coreFact = dataset.coreFact;
            var query = new QueryRequest(dataset.coreFact)
                .SelectGroup(
                    dataset.time.hour)
                .Select(
                    dataset.coreFact.temperature,
                    dataset.coreFact.rain,
                    dataset.coreFact.windSpeed)
                .Join(coreFact.timeRef)
                .Join(coreFact.locationRef)
                .WhereEq(dataset.location.PrimaryKey, locationId)
                .WhereEq(dataset.time.day, dayOfMonth)
                .WhereEq(dataset.time.month, month)
                .WhereEq(dataset.time.year, year)
                .OrderByAsc(dataset.time.hour);

            using (var connection = SqliteHelper.CreateConnection())
            {
                return query.Execute(connection).rows;
            }
        }
        public static List<float> HoursTempForYear(string postName, int year)
        {
            var dataset = WeatherHistoryDataset.Instance;
            int locationId = Convert.ToInt32(TableService.ValueFor(
                dataset.location, 
                dataset.location.PrimaryKey,
                dataset.location.PostName,
                postName));
            var coreFact = dataset.coreFact;
            var fieldToMeasure = dataset.coreFact.temperature;
            var query = new QueryRequest(coreFact)
                .Select(
                    coreFact.temperature
                )
                .Join(coreFact.locationRef)
                .Join(coreFact.timeRef)
                .WhereEq(dataset.location.PrimaryKey, locationId)
                .WhereEq(dataset.time.year, year)
                .OrderByAsc(dataset.time.year)
                .OrderByAsc(dataset.time.month)
                .OrderByAsc(dataset.time.day);

            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                return result.rows.Select(r => r.FloatValue(coreFact.temperature)).ToList();
            }
        }
        public static List<GeoMeasure> HoursOfActivity(Activity activity, int year)
        {
            var dataset = WeatherHistoryDataset.Instance;
            var coreFact = dataset.coreFact;
            var query = new QueryRequest(coreFact)
                .SelectGroup(
                    dataset.location.Longitude,
                    dataset.location.Latitude,
                    dataset.location.PostName)
                .SelectCount(coreFact.PrimaryKey)
                .Join(coreFact.locationRef)
                .Join(coreFact.timeRef)
                .WhereEq(dataset.time.year, year);
            activity.AddToQuery(query);
            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                return result.ToPostMeasures(dataset.location, coreFact.PrimaryKey);
            }
        }
        public static List<int> HoursForActivity(Activity activity, string postName, int year, int month, int dayOfMonth)
        {
            var dataset = WeatherHistoryDataset.Instance;
            int locationId = Convert.ToInt32(TableService.ValueFor(
                dataset.location,
                dataset.location.PrimaryKey,
                dataset.location.PostName,
                postName));
            var coreFact = dataset.coreFact;
            var query = new QueryRequest(dataset.coreFact)
                .SelectGroup(
                    dataset.time.hour)
                .Join(coreFact.timeRef)
                .Join(coreFact.locationRef)
                .WhereEq(dataset.location.PrimaryKey, locationId)
                .WhereEq(dataset.time.day, dayOfMonth)
                .WhereEq(dataset.time.month, month)
                .WhereEq(dataset.time.year, year)
                .OrderByAsc(dataset.time.hour);
            activity.AddToQuery(query);
            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                return result.rows.Select(r => r.IntValue(dataset.time.hour)).ToList();
            }
        }
    }
}
