namespace Assets.Scripts.Foenn.Core.Database
{
    using System.IO;
    using UnityEngine;

    public abstract class DatabaseHelper
    {
        public static string ResolveDatabasePath(string databasePath)
        {
            return Path.IsPathRooted(databasePath)
                ? databasePath
                : Path.Combine(Application.dataPath, databasePath);
        }

        public static void CreateDb(string databasePath)
        {
            var fullPath = ResolveDatabasePath(databasePath);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(fullPath))
                File.Create(fullPath).Close();
        }
    }
}
