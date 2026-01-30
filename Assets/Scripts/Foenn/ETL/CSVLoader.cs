using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.Weathers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.ETL
{
public class CSVLoader
{
    public static char[] STRING_SPLIT = { ';' };
    private const string DECIMAL_SPLIT = ".";

    public CSVLoader()
    {
    }

    public Dataset Extract(string text, int year)
    {
        var reader = new StringReader(text);

        var line = reader.ReadLine();

        var header = line
            .Split(STRING_SPLIT)
            .ToList();

        var result = new Dataset(header, Transform(reader, header, year));
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

    private List<DataLine> Transform(StringReader reader, List<string> header, int year)
    {
        // Transform header into either a metric key or an attribute key to fill the csv line
        // On crée deux listes pour stocker les clés reconnues
        var metricKeys = new Dictionary<int, MetricKey>();
        var attributeKeys = new Dictionary<int, DataAttributeKey>();

        for (int i = 0; i < header.Count; i++)
        {
            var col = header[i];
            if (Enum.TryParse<MetricKey>(col, out var metricKey))
            {
                metricKeys[i] = metricKey;
            }
            else if (Enum.TryParse<DataAttributeKey>(col, out var attrKey))
            {
                attributeKeys[i] = attrKey;
            }
        }
        // metricKeys et attributeKeys sont maintenant des dictionnaires index -> enum

        List<DataLine> result = new List<DataLine>();
        int dateIndex = header.IndexOf(DataAttributeKey.AAAAMMJJHH);
        var valueIndexes = keysToLoad.Select(key => header.IndexOf(key)).ToList();
        string comparison = year.ToString();
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            var columns = line.Split(STRING_SPLIT);
            var currentDate = columns[dateIndex];
            if (currentDate.StartsWith(comparison))
            {
                var line = new DataLine();
                foreach (var index in valueIndexes)
                {
                    var value = columns[index];
                    if (metricKeys.ContainsKey(index))
                    {
                        if (float.TryParse(value.Replace(DECIMAL_SPLIT, ","), out var floatValue))
                        {
                            line.metrics.Add(metricKeys[index], floatValue);
                        }
                    }
                    else if (attributeKeys.ContainsKey(index))
                    {
                        line.attributes.Add(attributeKeys[index], value);
                    }

                }
                result.Add(record);
            }
        }
        return result;
    }
}
}