using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using UnityEngine;
using Assets.Scripts;
using Assets.Resources.Weathers;

namespace Assets.Scripts.Foenn.ETL
{
public static class DbSeeder
{
    // Seed departments from CSV files in Resources/Weathers/ into SQLite DB at dbPath
    public static void SeedDepartments(string dbPath)
    {
        try
        {
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string connString = $"Data Source={dbPath};Version=3;";
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Departments(department INTEGER PRIMARY KEY, file TEXT, seededAt TEXT);";
                    cmd.ExecuteNonQuery();

                    var files = CSVLoader.ListCsvFilesInResources(WeatherDataset.WEATHER_PATH);
                    Debug.Log($"DbSeeder: found {files.Count} csv files to inspect");

                    var inserted = 0;
                    foreach (var file in files)
                    {
                        int? dep = ExtractDepartmentFromFileName(file);
                        if (!dep.HasValue) continue;

                        cmd.CommandText = "SELECT COUNT(1) FROM Departments WHERE department = @dep";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@dep", dep.Value);
                        var exists = Convert.ToInt64(cmd.ExecuteScalar());
                        if (exists == 0)
                        {
                            cmd.CommandText = "INSERT INTO Departments(department, file, seededAt) VALUES(@d, @f, @s)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@d", dep.Value);
                            cmd.Parameters.AddWithValue("@f", file);
                            cmd.Parameters.AddWithValue("@s", DateTime.UtcNow.ToString("o"));
                            cmd.ExecuteNonQuery();
                            inserted++;
                            Debug.Log($"DbSeeder: inserted department {dep.Value} (file: {file})");
                        }
                        else
                        {
                            Debug.Log($"DbSeeder: department {dep.Value} already exists, skipping");
                        }
                    }

                    Debug.Log($"DbSeeder: seeding finished, inserted {inserted} new departments into {dbPath}");
                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"DbSeeder: seeding failed: {ex}");
        }
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
}
}