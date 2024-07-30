using Microsoft.ML.Data;

namespace Assets.Resources.Models
{
    public class WeatherData
    {
        [LoadColumn(0)]
        public float Temperature;
        /*[LoadColumn(1)]
        public float WindSpeed { get; set; }
        [LoadColumn(2)]
        public float Rainfall { get; set; }
        [LoadColumn(3)]
        public float Cloudiness { get; set; }*/

        //
        // Features
        /*[LoadColumn(4)]
        public float TemperatureSquared => Temperature * Temperature;
        [LoadColumn(5)]
        public float TemperatureWindSpeed => Temperature * WindSpeed;
        [LoadColumn(6)]
        public float TemperatureRainfall => Temperature * Rainfall;
        [LoadColumn(7)]
        public float TemperatureCloudiness => Temperature * Cloudiness;
        [LoadColumn(8)]
        public float WindSpeedSquared => WindSpeed * WindSpeed;
        [LoadColumn(9)]
        public float WindSpeedRainfall => WindSpeed * Rainfall;
        [LoadColumn(10)]
        public float WindSpeedCloudiness => WindSpeed * Cloudiness;
        [LoadColumn(11)]
        public float RainfallSquared => Rainfall * Rainfall;
        [LoadColumn(12)]
        public float RainfallCloudiness => Rainfall * Cloudiness;
        [LoadColumn(13)]
        public float CloudinessSquared => Cloudiness * Cloudiness;*/

        [LoadColumn(1)]
        public float SuitabilityScore;

    }
    public class WeatherSuitabilityPrediction
    {
        [ColumnName("Score")]
        public float SuitabilityScore;
    }
}