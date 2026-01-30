using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.Results;
using Assets.Scripts.Foenn.Engine.Weathers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Requests
{
    public sealed class QueryRequest
    {
        public required IReadOnlyList<AggregationKey> MetricAggregations { get; init; }
        public required IReadOnlyList<DataAttributeKey> DataAttributes { get; init; }
        public required IReadOnlyList<GeoAttributeKey> GeoAttributes { get; init; }
        public required IReadOnlyList<TimeAttributeKey> TimeAttributes { get; init; }
        public IReadOnlyList<Filter> Filters { get; init; } = Array.Empty<Filter>();

        public QueryResult Execute(IQueryExecutor executor)
            => executor.Execute(this);
    }
}