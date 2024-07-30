using Assets.Resources.Activities;
using Assets.Resources.Models;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.FromTuto
{
    class MLProgram
    {
        public static WeatherData ToData(WeatherCondition condition, float SuitabilityScore)
        {
            return new WeatherData
            {
                Temperature = condition.Temperature,
                /*WindSpeed = example.Condition.WindSpeed,
                Rainfall = example.Condition.Rainfall,
                Cloudiness = example.Condition.Cloudiness,*/
                SuitabilityScore = SuitabilityScore
            };
        }

        public static void Start()
        {
            Debug.Log(Environment.CurrentDirectory);

            // <Snippet3>
            MLContext mlContext = new MLContext(seed: 0);
            // </Snippet3>

            var trainingExamples = new List<TrainingExample>
        {
            /*//Add your perfect conditions
            //No Rain AND No Wind And (23 AND No CLoud) OR (26)
            new TrainingExample(new WeatherCondition(23, 0, 0, 0), 100),
            new TrainingExample(new WeatherCondition(26, 0, 0, 100), 100),

            //Add your 0 conditions
            //Rain OR (Wind AND Cold) OR (Hot AND No Cloud)
            new TrainingExample(new WeatherCondition(20, 0, 1, 0), 0),
            new TrainingExample(new WeatherCondition(20, 20, 0, 100), 0),
            new TrainingExample(new WeatherCondition(30, 0, 0, 0), 0),

            //Generated Examples with user giving suitability
            new TrainingExample(new WeatherCondition(22, 0, 0, 50), 40)*/
            
            new TrainingExample(new WeatherCondition(20, 0, 1, 0), 20),
            new TrainingExample(new WeatherCondition(21, 0, 1, 0), 21),
            new TrainingExample(new WeatherCondition(22, 0, 1, 0), 22),
            new TrainingExample(new WeatherCondition(23, 0, 1, 0), 23),
            new TrainingExample(new WeatherCondition(24, 0, 1, 0), 24),
            new TrainingExample(new WeatherCondition(25, 0, 1, 0), 25),
            new TrainingExample(new WeatherCondition(26, 0, 1, 0), 26),
            new TrainingExample(new WeatherCondition(27, 0, 1, 0), 27),
            new TrainingExample(new WeatherCondition(28, 0, 1, 0), 28),
            new TrainingExample(new WeatherCondition(29, 0, 1, 0), 29),
            new TrainingExample(new WeatherCondition(30, 0, 1, 0), 30),
            new TrainingExample(new WeatherCondition(40, 0, 1, 0), 40),
            new TrainingExample(new WeatherCondition(50, 0, 1, 0), 50),
            new TrainingExample(new WeatherCondition(60, 0, 1, 0), 60),
            new TrainingExample(new WeatherCondition(70, 0, 1, 0), 70),

        };


            var trainingData = new List<WeatherData>();
            foreach (var example in trainingExamples)
            {
                trainingData.Add(ToData(example.Condition, example.SuitabilityScore));
            }


            // <Snippet5>
            var model = Train(mlContext, trainingData);
            // </Snippet5>

            // <Snippet14>
            Evaluate(mlContext, model, trainingData);
            // </Snippet14>

            // <Snippet20>
            TestSinglePrediction(mlContext, model);
            // </Snippet20>
        }

        public static ITransformer Train(MLContext mlContext, List<WeatherData> data)
        {
            // <Snippet6>
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);
            // </Snippet6>

            // <Snippet7>
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "SuitabilityScore")
                    // </Snippet7>
                    // <Snippet8>
                    // </Snippet8>
                    // <Snippet9>
                    .Append(mlContext.Transforms.Concatenate("Features", "Temperature"))
                    // </Snippet9>
                    // <Snippet10>
                    .Append(mlContext.Regression.Trainers.FastTree());
            // </Snippet10>

            Debug.Log("=============== Create and Train the Model ===============");

            // <Snippet11>
            var model = pipeline.Fit(dataView);
            // </Snippet11>

            Debug.Log("=============== End of training ===============");
            // <Snippet12>
            return model;
            // </Snippet12>
        }

        private static void Evaluate(MLContext mlContext, ITransformer model, List<WeatherData> data)
        {
            // <Snippet15>
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);
            // </Snippet15>

            // <Snippet16>
            var predictions = model.Transform(dataView);
            // </Snippet16>
            // <Snippet17>
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
            // </Snippet17>

            Debug.Log($"*************************************************");
            Debug.Log($"*       Model quality metrics evaluation         ");
            Debug.Log($"*------------------------------------------------");
            // <Snippet18>
            Debug.Log($"*       RSquared Score:      {metrics.RSquared:0.##}");
            // </Snippet18>
            // <Snippet19>
            Debug.Log($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
            // </Snippet19>
            Debug.Log($"*************************************************");
        }

        private static void TestSinglePrediction(MLContext mlContext, ITransformer model)
        {
            //Prediction test
            // Create prediction function and make prediction.
            // <Snippet22>
            var predictionFunction = mlContext.Model.CreatePredictionEngine<WeatherData, WeatherSuitabilityPrediction>(model);
            // </Snippet22>
            //Sample:
            //vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
            //VTS,1,1,1140,3.75,CRD,15.5
            // <Snippet23>
            var sample = ToData(new WeatherCondition(30, 2, 0, 50), 0);
            // </Snippet23>
            // <Snippet24>
            var prediction = predictionFunction.Predict(sample);
            // </Snippet24>
            // <Snippet25>
            Debug.Log($"**********************************************************************");
            Debug.Log($"Predicted fare: {prediction.SuitabilityScore:0.####}, actual fare: 15.5");
            Debug.Log($"**********************************************************************");
            // </Snippet25>
        }
    }
}