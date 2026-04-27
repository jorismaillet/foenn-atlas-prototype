using System;
using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Schema.Fields;
using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations
{
    public class CustomGradient : ColorPicker
    {
        public static CustomGradient CumulativeYearHours = new CustomGradient(CreateCumulativeYearHoursGradient(), "Hours", "h", 0f, 365f * 3f, 1);

        private static Dictionary<Field, Func<Gradient>> Gradients = new Dictionary<Field, Func<Gradient>>()
        {
            { WeatherHistoryDataset.Instance.coreFact.temperature, CreateTemperatureGradient },
            { WeatherHistoryDataset.Instance.coreFact.rain, CreateRainGradient },
            { WeatherHistoryDataset.Instance.coreFact.windSpeed, CreateWindGradient }
        };

        private Gradient gradient;

        public float minValue;

        public float maxValue;

        public CustomGradient(Field field, float multiplier = 1) : base(field.displayName, field.format)
        {
            var range = new FieldRange(field, multiplier);
            this.minValue = range.minValue;
            this.maxValue = range.maxValue;
            this.gradient = Gradients[field].Invoke();
        }

        public CustomGradient(Gradient gradient, string title, string format, float minValue, float maxValue, float multiplier) : base(title, format)
        {
            this.minValue = minValue * multiplier;
            this.maxValue = maxValue * multiplier;
            this.gradient = gradient;
        }

        public override Color GetColor(float value)
        {
            if (gradient == null)
                return Color.white;

            if (Math.Abs(maxValue - minValue) < Mathf.Epsilon)
                return gradient.Evaluate(0f);

            var normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);
            return gradient.Evaluate(normalizedValue);
        }

        private static Gradient CreateTemperatureGradient()
        {
            var result = new Gradient();
            result.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0f, 0.3f, 1f), 0f),
                    new GradientColorKey(Color.cyan, 0.33f),
                    new GradientColorKey(Color.yellow, 0.66f),
                    new GradientColorKey(Color.red, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            return result;
        }

        private static Gradient CreateRainGradient()
        {
            var result = new Gradient();
            result.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.95f, 0.95f, 0.95f), 0f),
                    new GradientColorKey(new Color(0.6f, 0.8f, 1f), 0.33f),
                    new GradientColorKey(new Color(0.2f, 0.5f, 1f), 0.66f),
                    new GradientColorKey(new Color(0f, 0.2f, 0.7f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            return result;
        }

        private static Gradient CreateWindGradient()
        {
            var result = new Gradient();
            result.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.8f, 1f, 0.8f), 0f),
                    new GradientColorKey(Color.yellow, 0.33f),
                    new GradientColorKey(new Color(1f, 0.6f, 0f), 0.66f),
                    new GradientColorKey(Color.red, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            return result;
        }

        public static Gradient CreateCumulativeYearHoursGradient()
        {
            var result = new Gradient();
            result.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.red, 0f),
                    new GradientColorKey(Color.green, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            return result;
        }

        public static Gradient CreateDroughtGradient()
        {
            var result = new Gradient();
            result.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.56f, 0.27f, 0.68f), 0f),
                    new GradientColorKey(Color.blue, 0.5f),
                    new GradientColorKey(new Color(1f, 0.85f, 0.2f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            return result;
        }
    }
}
