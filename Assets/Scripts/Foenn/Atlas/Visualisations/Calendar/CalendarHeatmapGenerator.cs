using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.Utils;
using Mono.Data.Sqlite;
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
            return null;
        }
    }
}