using Assets.Resources.Activities;
using Assets.Resources.Models;
using Assets.Resources.Models.MLs.AnotherTests;
using Assets.Resources.Weathers;
using Assets.Scripts.FromTuto;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Hello : MonoBehaviour
{
    void Start()
    {
        House.Go(22);
        House.Go(25);
        House.Go(29);
        House.Go(35);
        return;
        /*Program.Start();
        return;*/

        /*var post = "PLOMELIN-INRAE";
        var department = 29;
        var year = 2023;
        Debug.Log($"Loading file {WeatherDataset.WeatherFileName(department, year)}");
        var weather = new WeatherDataset(department, year);
        Debug.Log($"There are {weather.EntriesQuantity()} entries for {weather.PostsQuantity()} posts in {year} for department {department}");
        Debug.Log($"Loading records for post {post}");
        var postWeather = new WeatherPostDataset(weather, post);
        Debug.Log($"The post {post} has {postWeather.records.Count()} entries and the average temperature recorded in {year} is {postWeather.AverageTemperature()}");*/

        var activity = new Activity("Kayak", new float[2] { 20, 30 }, new float[2] { 0, 20 }, new float[2] { 0, 0 }, new float[2] { 0, 100 }); // TODO RecordType with default range
        Debug.Log($"Training new activity {activity.name}");
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

         var mlContext = new MLContext();
        var trainingDataView = mlContext.Data.LoadFromEnumerable(trainingData);

        // Define the learning pipeline with polynomial features
        var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "SuitabilityScore")
                    // </Snippet7>
                    // <Snippet8>
                    // </Snippet8>
                    // <Snippet9>
                    .Append(mlContext.Transforms.Concatenate("Features", nameof(WeatherData.Temperature)/*, nameof(WeatherData.WindSpeed), nameof(WeatherData.Rainfall), nameof(WeatherData.Cloudiness)*/))/*,
                 nameof(WeatherData.TemperatureSquared), nameof(WeatherData.WindSpeedSquared), nameof(WeatherData.RainfallSquared), nameof(WeatherData.CloudinessSquared),
                 nameof(WeatherData.TemperatureWindSpeed), nameof(WeatherData.TemperatureRainfall), nameof(WeatherData.TemperatureCloudiness),
                 nameof(WeatherData.WindSpeedRainfall), nameof(WeatherData.WindSpeedCloudiness), nameof(WeatherData.RainfallCloudiness)))*/
                    // </Snippet9>
                    // <Snippet10>
                    .Append(mlContext.Regression.Trainers.FastTree()); //Tested FastTree & SDCA

         // Train the model
         var model = pipeline.Fit(trainingDataView);

        //Verify Model
        var predictions = model.Transform(trainingDataView);
        var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
        Debug.Log($"*************************************************");
        Debug.Log($"*       Model quality metrics evaluation         ");
        Debug.Log($"*------------------------------------------------");
        Debug.Log($"*       RSquared Score:      {metrics.RSquared:0.##}");
        Debug.Log($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:0.##}");

        // Use the model for predictions
        var predictionEngine = mlContext.Model.CreatePredictionEngine<WeatherData, WeatherSuitabilityPrediction>(model);




        //Test single prediction
        var prediction = predictionEngine.Predict(trainingData.First());
        Debug.Log($"**********************************************************************");
        Debug.Log($"Predicted fare: {prediction.SuitabilityScore:0.####}, actual score: {trainingData.First().SuitabilityScore}");
        Debug.Log($"**********************************************************************");
       
        // Debug: Print the model coefficients
        /*var olsModel = model.LastTransformer.Model;
        Debug.Log("Model Weights:");
        for (int i = 0; i < olsModel.Weights.Count; i++)
        {
            Debug.Log($"Weight[{i}]: {olsModel.Weights[i]}");
        }
        Debug.Log($"Bias: {olsModel.Bias}");*/


        // Example prediction
        prediction = predictionEngine.Predict(ToData(new WeatherCondition(23, 0, 0, 0 ), 0));
         Debug.Log($"La météo actuelle convient à l'activité à {prediction.SuitabilityScore}%.");

        prediction = predictionEngine.Predict(ToData(new WeatherCondition(20, 2, 0, 50), 0));
         Debug.Log($"La météo actuelle convient à l'activité à {prediction.SuitabilityScore}%.");

        prediction = predictionEngine.Predict(ToData(new WeatherCondition(25, 2, 0, 50), 0));
         Debug.Log($"La météo actuelle convient à l'activité à {prediction.SuitabilityScore}%.");

        prediction = predictionEngine.Predict(ToData(new WeatherCondition(30, 2, 0, 50), 0));
         Debug.Log($"La météo actuelle convient à l'activité à {prediction.SuitabilityScore}%.");

        activity.model.Train(trainingExamples);
        Debug.Log($"Training done");
        activity.Suitability(new WeatherCondition(23, 0, 0, 0));
        activity.Suitability(new WeatherCondition(20, 2, 0, 50));
        activity.Suitability(new WeatherCondition(25, 2, 0, 50));
        activity.Suitability(new WeatherCondition(30, 2, 0, 50));
    }

    public WeatherData ToData(WeatherCondition condition, float SuitabilityScore)
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
}
