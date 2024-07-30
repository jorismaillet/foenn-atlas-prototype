using Assets.Resources.Activities;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Models
{
    public class TrainingExample
    {
        public WeatherCondition Condition { get; set; }
        public float SuitabilityScore { get; set; } // Score entre 0 et 100

        public TrainingExample(WeatherCondition condition, float suitabilityScore)
        {
            Condition = condition;
            SuitabilityScore = suitabilityScore;
        }
    }
}