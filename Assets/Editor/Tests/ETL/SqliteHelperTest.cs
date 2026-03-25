using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Schema.Fields;
using NUnit.Framework;

namespace Assets.Editor.Tests.ETL
{
    public class SqliteHelperTest
    {
        [Test]
        public void InsertFromTableSQL_BuildsDerivedFactQuery()
        {
            var dataset = WeatherHistoryDataset.Instance;
            var yearlyFact = dataset.yearlyFact;
            var coreFact = dataset.coreFact;

            var insertColumns = new List<string>();
            var selectExpressions = new List<string>();

            foreach (var entry in yearlyFact.aggregatedField)
            {
                var (sourceField, aggregation) = entry.Key;
                insertColumns.Add(entry.Value.fieldName);
                selectExpressions.Add($"{aggregation.ToString().ToUpper()}(cf.{sourceField.fieldName})");
            }

            var filterIds = new HashSet<int> { 1, 2, 3 };

            var sql = SqliteHelper.InsertFromTableSQL(
                filterIds, yearlyFact,
                coreFact, dataset.time, coreFact.timeRef,
                yearlyFact.year, yearlyFact.locationRef,
                insertColumns, selectExpressions);

            var normalized = Regex.Replace(sql.Trim(), @"\s+", " ");

            StringAssert.AreEqualIgnoringCase(
                "INSERT OR IGNORE INTO weather_yearly_facts (" +
                    "year, location_id, " +
                    "temperature_min, temperature_avg, temperature_max, " +
                    "dew_point_min, dew_point_avg, dew_point_max, " +
                    "humidity_min, humidity_avg, humidity_max, " +
                    "rain_min, rain_avg, rain_max, " +
                    "wind_speed_min, wind_speed_avg, wind_speed_max) " +
                "SELECT " +
                    "t.year, cf.location_id, " +
                    "MIN(cf.temperature), AVG(cf.temperature), MAX(cf.temperature), " +
                    "MIN(cf.dew_point), AVG(cf.dew_point), MAX(cf.dew_point), " +
                    "MIN(cf.humidity), AVG(cf.humidity), MAX(cf.humidity), " +
                    "MIN(cf.rain), AVG(cf.rain), MAX(cf.rain), " +
                    "MIN(cf.wind_speed), AVG(cf.wind_speed), MAX(cf.wind_speed) " +
                "FROM weather_history_facts cf " +
                "JOIN time_dimension t ON cf.time_id = t.id " +
                "WHERE cf.location_id IN (1,2,3) " +
                "GROUP BY t.year, cf.location_id",
                normalized);
        }
    }
}
