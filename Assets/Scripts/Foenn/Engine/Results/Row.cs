using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Results
{
    public sealed record Row(
        DateTimeOffset Time,
        GeoPoint? Location,
        IReadOnlyDictionary<string, object> MetricValues
    );
}