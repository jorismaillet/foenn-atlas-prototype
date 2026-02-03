using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.OLAP
{
    public class Row
    {
        public List<Dimension> dimensions = new List<Dimension>();
        public List<Measure> measures = new List<Measure>();
    }
}