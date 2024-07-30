using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Resources.Models.MLs.AnotherTests
{
    public class HouseData
    {
        public float Temperature { get; set; }
        public float Suitability { get; set; }
    }

    public class Prediction
    {
        [ColumnName("Score")]
        public float Suitability { get; set; }
    }

    public class House
    {
        public static void Go(int predictionTest)
        {
            MLContext mlContext = new MLContext(seed: 0);

            // 1. Import or create training data
            /*HouseData[] houseData = {
               new HouseData() { Temperature = 20, Suitability = 30 },
               new HouseData() { Temperature = 24, Suitability = 100 },
               new HouseData() { Temperature = 26, Suitability = 100 },
               new HouseData() { Temperature = 30, Suitability = 40 },
               new HouseData() { Temperature = 10, Suitability = 0 },
               new HouseData() { Temperature = 40, Suitability = 0 }
            };*/
            HouseData[] bestWithSDCA = {
               new HouseData() { Temperature = 10, Suitability = 0 },
               new HouseData() { Temperature = 20, Suitability = 30 },
               new HouseData() { Temperature = 24, Suitability = 50 },
               new HouseData() { Temperature = 26, Suitability = 70 },
               new HouseData() { Temperature = 30, Suitability = 100 }
            };
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(bestWithSDCA);

            // 2. Specify data preparation and model training pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "Temperature" })
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Suitability", maximumNumberOfIterations: 50));
            //https://learn.microsoft.com/en-us/dotnet/machine-learning/how-to-choose-an-ml-net-algorithm

            // 3. Train model
            var model = pipeline.Fit(trainingData);

            // 4. Make a prediction
            var size = new HouseData() { Temperature = predictionTest };
            var predicted = mlContext.Model.CreatePredictionEngine<HouseData, Prediction>(model).Predict(size);

            Debug.Log($"Predicted suitability for Temperature: {size.Temperature} is {predicted.Suitability}");
        }
    }
}