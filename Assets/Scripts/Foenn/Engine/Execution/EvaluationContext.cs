using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Requests
{
    public sealed class EvaluationContext
    {
        public DateTimeOffset Time { get; init; }
        public GeoPoint? Location { get; init; }

        // Inputs (weather series, layers, etc.)
        public IReadOnlyDictionary<string, object> Inputs { get; init; } = new Dictionary<string, object>();
    }
}