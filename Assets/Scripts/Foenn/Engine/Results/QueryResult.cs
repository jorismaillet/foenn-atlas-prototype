using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Results
{
    public sealed class QueryResult
    {
        // Un résultat simple: une table (Time, Geo, MetricId -> Value)
        public required IReadOnlyList<Row> Rows { get; init; }

        
    }
}