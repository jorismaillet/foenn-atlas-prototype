using Assets.Resources.Activities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Models
{
    public class ActivityModel
    {
        private float[] weights = new float[4];
        private float learningRate = 0.01F;
        private float[] tempRange, windRange, rainRange, cloudRange;

        public ActivityModel(float[] tempRange, float[] windRange, float[] rainRange, float[] cloudRange)
        {
            this.tempRange = tempRange;
            this.windRange = windRange;
            this.rainRange = rainRange;
            this.cloudRange = cloudRange;
        }

        public void Train(List<TrainingExample> examples, int epochs = 1000)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                foreach (var example in examples)
                {
                    var normalizedCondition = Normalize(example.Condition);
                    float predicted = PredictNormalized(normalizedCondition);
                    float error = example.SuitabilityScore - predicted;

                    weights[0] += learningRate * error * normalizedCondition.Temperature;
                    weights[1] += learningRate * error * normalizedCondition.WindSpeed;
                    weights[2] += learningRate * error * normalizedCondition.Rainfall;
                    weights[3] += learningRate * error * normalizedCondition.Cloudiness;
                }
            }
        }

        private WeatherCondition Normalize(WeatherCondition condition)
        {
            return new WeatherCondition(
                NormalizeValue(condition.Temperature, tempRange),
                NormalizeValue(condition.WindSpeed, windRange),
                NormalizeValue(condition.Rainfall, rainRange),
                NormalizeValue(condition.Cloudiness, cloudRange)
            );
        }

        private float NormalizeValue(float value, float[] range)
        {
            if (range[0] == range[1]) return 0;
            return (value - range[0]) / (range[1] - range[0]);
        }

        private float PredictNormalized(WeatherCondition condition)
        {
            return weights[0] * condition.Temperature +
                   weights[1] * condition.WindSpeed +
                   weights[2] * condition.Rainfall +
                   weights[3] * condition.Cloudiness;
        }

        public float Predict(WeatherCondition condition)
        {
            var normalizedCondition = Normalize(condition);
            if (OutOfRange(condition))
            {
                Debug.LogWarning("Condition is out of range");
                return 0;
            }
            return PredictNormalized(normalizedCondition);
        }

        private bool OutOfRange(WeatherCondition condition)
        {
            return
                OutOfRange(condition.Temperature, tempRange) ||
                OutOfRange(condition.WindSpeed, windRange) ||
                OutOfRange(condition.Rainfall, rainRange) ||
                OutOfRange(condition.Cloudiness, cloudRange);
        }

        private bool OutOfRange(float value, float[] range)
        {
            return value < range[0] || value > range[1];
        }
    }
}