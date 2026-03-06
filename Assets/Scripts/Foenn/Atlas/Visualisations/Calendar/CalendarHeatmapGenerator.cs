using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.Utils;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations
{
    public class CalendarHeatmapGenerator
    {
        public static Texture2D BuildCalendarHeatmapTexture(string city, int department, Dictionary<Activity, Color32> activities, RenderSettings settings)
        {
            var request = new QueryRequest(WeatherHistoryDatasource.tableName);
            request.filters.Add(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.NOM_USUEL, city));
            request.filters.Add(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, department.ToString()));
            var result = new SqliteConnector().ExecuteQuery(request);
            var points = new List<int>();
            foreach (var ac in activities)
            {
                var activity = ac.Key;
                var color = ac.Value;
                points.Add(result.rows.Sum(row => activity.conditions.IsMatch(row) ? 1 : 0));
            }
            Debug.Log(points);
            var heatmap = result.rows.Select(row =>
            {
                var date = TimeUtils.Date(row.attributes[WeatherHistoryAttributeKey.AAAAMMJJHH].value);
                var activity = activities.FirstOrDefault(a => a.Key.conditions.IsMatch(row));
                return activity.Value;
            }).ToArray();

            var texture = new Texture2D(settings.width, settings.height);

            return RenderOperation.CreateTexture(heatmap, settings);
        }
    }
}