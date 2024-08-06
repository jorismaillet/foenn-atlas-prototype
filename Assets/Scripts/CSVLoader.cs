using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CSVLoader
{
    public static char[] STRING_SPLIT = { ';' };
    private const string DECIMAL_SPLIT = ".";

    private int postID;

    public CSVLoader(int postID = 0)
    {
        this.postID = postID;
    }

    public CSVResult LoadCSV(string text)
    {
        var reader = new StringReader(text);
        var header = reader.ReadLine().Split(STRING_SPLIT);
        if (postID > 0)
        {
            return new CSVResult(header, FilteredRemainingLines(reader, postID.ToString()));
        }
        else
        {
            return new CSVResult(header, AllRemainingLines(reader));
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


    //TODO Filter remaining linges by year
    public IEnumerable<string> AllRemainingLines(StringReader reader)
    {
        while (reader.Peek() != -1)
        {
            yield return reader.ReadLine();
        }
        yield break;
    }

    private IEnumerable<string> FilteredRemainingLines(StringReader reader, string postID)
    {
        string line;
        bool startAdding = false;
        while ((line = reader.ReadLine()) != null)
        {
            var columns = line.Split(STRING_SPLIT);
            var currentID = columns[0]; // Assumant que l'ID est dans la première colonne
            if (currentID == postID)
            {
                startAdding = true;
            }
            else if (startAdding && currentID != postID)
            {
                // Une fois que nous avons trouvé les lignes qui commencent par targetID, nous pouvons arrêter dès que l'ID change
                yield break;
            }
            if (startAdding)
            {
                yield return line;
            }
        }
    }
}