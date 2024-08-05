using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
    private List<DepartmentRanking> ranking = new List<DepartmentRanking>();

    //https://openweathermap.org/city/2997943 Pour plus de données
    private void Start()
    {
        //var departmentFiles = CSVLoader.ListCsvFilesInResources(WeatherDataset.WEATHER_PATH);
        Task.Run(new WeatherProcessor().Process);
    }
}
