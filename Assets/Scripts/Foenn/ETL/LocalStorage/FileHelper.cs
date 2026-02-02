using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Assets.Scripts.Foenn.ETL.LocalStorage
{
    public class FileHelper
    {
        [MenuItem("Tools/List CSV Files in Resources")]
        public static List<string> ListCsvFilesInResources(string folderPath)
        {
            string resourcesPath = Application.dataPath + "/Resources/" + folderPath; // Chemin relatif au dossier Resources
            List<string> csvFiles = GetCsvFileNames(resourcesPath);

            return csvFiles;
        }

        private static int? ExtractDepartmentFromFileName(string file)
        {
            // Try patterns like H_75_latest... or H_07_latest etc.
            var m = Regex.Match(file, @"H_(\d+)_");
            if (m.Success && int.TryParse(m.Groups[1].Value, out var d)) return d;
            // fallback: try to find first number sequence
            m = Regex.Match(file, "(\\d+)");
            if (m.Success && int.TryParse(m.Groups[1].Value, out d)) return d;
            return null;
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
    }
}