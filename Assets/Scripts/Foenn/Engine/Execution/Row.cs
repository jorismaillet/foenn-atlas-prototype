using Assets.Scripts.Foenn.Engine.Attributes;
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

        public Measure Measure(MetricKey key)
        {
            foreach (var measure in measures)
            {
                if (measure.metric.key.Equals(key))
                {
                    return measure;
                }
            }
            throw new Exception($"No measure with key {key} found in row measures.");
        }
        public AttributeValue AttributeValue(AttributeKey key)
        {
            foreach (var dimension in dimensions)
            {
                foreach (var attributeValue in dimension.attributeValues)
                {
                    if (attributeValue.attribute.key.Equals(key))
                    {
                        return attributeValue;
                    }
                }
            }
            throw new Exception($"No attribute with key {key} found in row dimensions.");
        }
    }
}