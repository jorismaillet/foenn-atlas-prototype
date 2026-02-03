using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Utils;
using Assets.Scripts.Unity.Commons.Behaviours;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations
{
    public class CalendarHeatmap : BaseBehaviour
    {
        public Dictionary<Activity, Color> activities;

        private void Heatmap(string city, int department)
        {
            var post = GetPostFromCity(city, department, activities);
            var records = post.records.ToList();
            //DataFor(32, post);

            var points = activities.Sum(activity => post.records.Sum(r => activity.SuitsHour(r) ? activity.weight : 0));
            Debug.Log(points);
            var heatmap = records.Select(record => {
                var date = TimeUtils.Date(record.Get(AttributeKey.AAAAMMJJHH));

                var activity = activities.FirstOrDefault(a => a.SuitsHour(record));

                var res = new HourPoint(date, activity);
                return res;
            }).ToList();

            var image = GetComponent<UnityEngine.UI.Image>();
            var width = (int)image.GetComponent<RectTransform>().sizeDelta.x;
            var height = (int)image.GetComponent<RectTransform>().sizeDelta.y;
            texture = new Texture2D(width, height);

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