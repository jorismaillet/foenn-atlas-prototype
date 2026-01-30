using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Requests;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations
{
    public Dictionary<Activity, Color[]> Activities = new Dictionary<Activity, Color[]>();

    public required TimeAttribute TimeAttribute { get; init; } // prédéfini (ex: année, pas horaire)
        public IReadOnlyList<IFilter> Filters { get; init; } = Array.Empty<IFilter>();

        public CalendarHeatmap Execute(IQueryExecutor executor)
        {
            // 1) On exécute chaque activité (même timeAttr, geoAttr propre à l’activité)
            // 2) On choisit, pour chaque tick, la première activité "validée" (selon priorité)
            // Ici on suppose que la metric renvoie un bool ou un score avec seuil.
            // À adapter: ton "validée" peut être (score > 0) ou catégorie != "NoGo"

            var activitiesSorted = new List<ActivityDefinition>(Activities);
            activitiesSorted.Sort((a, b) => b.Priority.CompareTo(a.Priority)); // higher first

            var ticks = TimeAttribute.Ticks();
            var cells = new List<CalendarHeatmap.Cell>();

            foreach (var t in ticks)
            {
                string? chosenActivityId = null;

                foreach (var act in activitiesSorted)
                {
                    var req = new QueryRequest
                    {
                        Metrics = new[] { act.SuitabilityMetric },
                        Attributes = new Engine.Attributes.Attribute[]
                        {
                            TimeAttribute,
                            new FixedGeoAttribute { Id = $"geo:{act.Id}", Location = act.Location }
                        },
                        Filters = Filters
                    };

                    var result = req.Execute(executor);
                    var row = result.Rows.Count > 0 ? result.Rows[0] : null;

                    if (row == null) continue;

                    var v = row.MetricValues[act.SuitabilityMetric.Id];

                    if (IsValidated(v))
                    {
                        chosenActivityId = act.Id;
                        break;
                    }
                }

                cells.Add(new CalendarHeatmap.Cell(t, chosenActivityId));
            }

            return new CalendarHeatmap { Cells = cells };
        }

        private static bool IsValidated(object metricValue)
        {
            return metricValue switch
            {
                bool b => b,
                int i => i > 0,
                double d => d > 0.0,
                _ => metricValue is not null
            };
        }
    }

    public sealed class CalendarHeatmap
{
    public required IReadOnlyList<Cell> Cells { get; init; }

    // timeTick -> chosen activity id (null = none)
    public sealed record Cell(DateTimeOffset Time, string? ActivityId);
}
}