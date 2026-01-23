using Assets.Resources.Weathers;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CSVLoader
{
    public static char[] STRING_SPLIT = { ';' };
    private const string DECIMAL_SPLIT = ".";


    public CSVLoader()
    {
    }

    public CSVResult LoadCSV(string text, int year, List<WeatherRecordFieldKey> keysToLoad)
    {
        var reader = new StringReader(text);


        var line = reader.ReadLine();
        //Debug.Log(line);
        var header = line
            .Split(STRING_SPLIT)
            .Select(rawKey => {
                if (Enum.TryParse<WeatherRecordFieldKey>(rawKey, out var key))
                {
                    return key;
                }
                else
                {
                    //Debug.LogWarning($"[CSV] Clé invalide détectée : '{rawKey}'");
                    return WeatherRecordFieldKey.UNSUPPORTED;
                }
                return Enum.Parse<WeatherRecordFieldKey>(rawKey);
            })
            .ToList();
        //Debug.Log("OK");


        var result = new CSVResult(header, FilteredRemainingLines(reader, header, year, keysToLoad));
        reader.Close();
        return result;
    }

    [MenuItem("Tools/List CSV Files in Resources")]
    public static List<string> ListCsvFilesInResources(string folderPath)
    {
        string resourcesPath = Application.dataPath + "/Resources/" + folderPath; // Chemin relatif au dossier Resources
        List<string> csvFiles = GetCsvFileNames(resourcesPath);

        return csvFiles;
    }

    private static List<string> GetCsvFileNames(string folderPath)
    {
        List<string> csvFiles = new List<string>();

        try
        {
            // Vérifier si le dossier existe
            if (Directory.Exists(folderPath))
            {
                // Récupérer tous les fichiers CSV dans le dossier
                string[] files = Directory.GetFiles(folderPath, "*.csv");

                // Ajouter les noms de fichiers à la liste
                foreach (string file in files)
                {
                    csvFiles.Add(Path.GetFileName(file));
                }
            }
            else
            {
                Debug.LogError("Le dossier spécifié n'existe pas.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Une erreur s'est produite : " + ex.Message);
        }

        return csvFiles;
    }

    private List<WeatherRecord> FilteredRemainingLines(StringReader reader, List<WeatherRecordFieldKey> header, int year, List<WeatherRecordFieldKey> keysToLoad)
    {
        List<WeatherRecord> result = new List<WeatherRecord>();
        int dateIndex = header.IndexOf(WeatherRecordFieldKey.AAAAMMJJHH);
        var valueIndexes = keysToLoad.Select(key => header.IndexOf(key)).ToList();
        string comparison = year.ToString();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var columns = line.Split(STRING_SPLIT);
            var currentDate = columns[dateIndex];
            if (currentDate.StartsWith(comparison))
            {
                var record = new WeatherRecord();
                foreach (var index in valueIndexes)
                {
                    var value = columns[index];
                    if (!string.IsNullOrEmpty(value)) // Else skip the post ?
                    {
                        record.values.Add(header[index], value);
                    }
                }
                result.Add(record);
            }
        }
        return result;
    }
}