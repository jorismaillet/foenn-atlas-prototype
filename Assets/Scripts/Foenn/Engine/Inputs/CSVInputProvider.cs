using Assets.Scripts.Foenn.ETL;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class CSVInputProvider : IInputProvider
    {
        private CSVLoader loader;
        private int year;
        private string csvText;

        public CSVInputProvider(int department, int year) {
            this.loader = new CSVLoader();

            var fileName = WeatherDataset.WeatherFileName(department, year);
            var textAsset = Resources.Load<TextAsset>(fileName);
            this.csvText = textAsset.text;
            this.year = year;
            Resources.UnloadAsset(textAsset);
        }

        public Dataset GetInputs()
        {
            return loader.Extract(csvText, year);
        }
    }
}