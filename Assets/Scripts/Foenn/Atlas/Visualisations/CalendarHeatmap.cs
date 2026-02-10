using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.Utils;
using Assets.Scripts.Unity.Commons.Behaviours;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations
{
    public class CalendarHeatmap : BaseBehaviour
    {
        public Dictionary<Activity, Color> activities;

        private void Heatmap(string city, int department)
        {
            var request = new QueryRequest(WeatherHistoryDatasource.tableName);
            request.filters.Add(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.NOM_USUEL, city));
            request.filters.Add(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, department.ToString()));
            var result = request.ExecuteOnce(new SqliteConnector());
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
                var date = TimeUtils.Date(row.Attribute(WeatherHistoryAttributeKey.AAAAMMJJHH).value);

                var activity = activities.FirstOrDefault(a => a.Key.conditions.IsMatch(row));

                var res = new ColoredPixel(date, activity.Value);
                return res;
            }).ToList();

            var image = GetComponent<UnityEngine.UI.Image>();
            var width = (int)image.GetComponent<RectTransform>().sizeDelta.x;
            var height = (int)image.GetComponent<RectTransform>().sizeDelta.y;
            var texture = new Texture2D(width, height);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

            foreach (var point in heatmap)
            {
                ColorPixel(texture, point.x, point.y, width, height, point.color);
            }
            image.sprite = sprite;
            texture.Apply();
        }

        private void ColorPixel(Texture2D texture, int x, int y, int width, int height, UnityEngine.Color color)
        {
            float scaleX = width / 365.0F;
            float scaleY = height / 24.0F;
            int minX = (int)(Mathf.Floor(x * scaleX));
            int maxX = (int)(Mathf.Ceil((x + 1) * scaleX));
            int minY = (int)(Mathf.Floor(y * scaleY));
            int maxY = (int)(Mathf.Ceil((y + 1) * scaleY));
            for (int XPixel = minX; XPixel < maxX; XPixel++)
            {
                for (int YPixel = minY; YPixel < maxY; YPixel++)
                {
                    texture.SetPixel(XPixel, YPixel, color);
                }
            }
        }
    }
}