using Assets.Resources.Activities;
using Assets.Resources.Weathers;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int department = 29;
    public int year = 2023;

    //https://openweathermap.org/city/2997943 Pour plus de données
    private void Start()
    {
        //var departmentFiles = CSVLoader.ListCsvFilesInResources(WeatherDataset.WEATHER_PATH);
        var fileText = Resources.Load<TextAsset>(WeatherDataset.WeatherFileName(department, year)).text;
        new Task(new WeatherProcessor(year, department, Activity.Kayak, fileText).Process).Start();
    }
}
