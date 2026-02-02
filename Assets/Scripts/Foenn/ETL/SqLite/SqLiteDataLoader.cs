using Assets.Resources.Weathers;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.Foenn.ETL.SqLite
{
public static class SqLiteDataLoader
{
    private static bool CheckDbExists(string dbPath)
    {
        return File.Exists(dbPath);
    }

    private static void CreateDb(string dbPath)
    {
        string connString = $"Data Source={dbPath};Version=3;";
        using (var conn = new SQLiteConnection(connString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Departments(department INTEGER PRIMARY KEY, file TEXT, seededAt TEXT);";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    public static void CreateTableFromCsvHeader(string dbPath, string tableName, string csvPath)
    {
        var headerLine = File.ReadLines(csvPath).First();
        var headers = headerLine.Split(';');
        var columns = string.Join(", ", headers.Select(h => $"\"{h}\" TEXT"));
        var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{tableName}\" ({columns});";

        string connString = $"Data Source={dbPath};Version=3;";
        using (var conn = new SQLiteConnection(connString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = createTableSql;
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    public static void PopulateTableFromCsvContent(string dbPath, string tableName, string csvPath)
    {
        var lines = File.ReadAllLines(csvPath);
        var headers = lines[0].Split(';');
        string connString = $"Data Source={dbPath};Version=3;";
        using (var conn = new SQLiteConnection(connString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    var values = lines[i].Split(';');
                    var columns = string.Join(", ", headers.Select(h => $"\"{h}\""));
                    var paramNames = string.Join(", ", headers.Select((h, idx) => $"@p{idx}"));
                    cmd.CommandText = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({paramNames})";
                    cmd.Parameters.Clear();
                    for (int j = 0; j < values.Length; j++)
                    {
                        cmd.Parameters.AddWithValue($"@p{j}", values[j]);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            conn.Close();
        }
    }

    }
}