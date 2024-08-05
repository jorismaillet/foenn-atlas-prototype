using Assets.Resources.Weathers;
using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Assets.Scripts;

public class CSVLoader : MonoBehaviour
{
    public static char[] STRING_SPLIT = { ';' };
    private const string DECIMAL_SPLIT = ".";

    private static CSVLoader Instance;

    private void Start()
    {
        Instance = this;
    }

    public void Load(string path, int postId, Action<CSVResult> Call)
    {
        StartCoroutine();
    }

    private async Task<string> LoadResourceTextAsync(string path)
    {
        Debug.Log("A1");
        // Appel à Resources.Load sur le thread principal
        try
        {
            var textAsset = await Task.FromResult(Resources.Load<TextAsset>(path));
            Debug.Log("A2");
            if (textAsset == null)
            {
                Debug.LogError($"Resource at path {path} not found.");
                return null;
            }

            Debug.Log("A3");
            return textAsset.text;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception in LoadResourceTextAsync: {ex.Message}");
            throw;
        }

    }

    public async Task<IEnumerable<string>> Load(string path, int postID = 0)
    {
        Debug.Log("A");
        this.postId = postID;
        Debug.Log("B");
        var rawtext = await LoadResourceTextAsync(path);
        Debug.Log("C");
        if (rawtext == null)
        {
            Debug.Log("D");
            throw new FileNotFoundException(path);
        }
        Debug.Log("E");
        var reader = new StringReader(rawtext);
        Debug.Log("F");
        header = reader.ReadLine().Split(STRING_SPLIT);
        Debug.Log("G");
        Debug.Log("6");
        if (postId > 0)
        {
            return await AllRemainingLines(reader);
        }
        else
        {
            var res = await AllRemainingLines(reader);
            Debug.Log("1");
            return res.Where(line => Equals(line[0], postId));
        }
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

    public async Task<IEnumerable<string>> AllRemainingLines(StringReader reader)
    {
        Debug.Log("7");
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            var records = new List<string>();
            await foreach (var record in csv.GetRecordsAsync<string>())
            {
                records.Add(record);
            }
            return records;
        }
    }
}