using System;

namespace Assets.Scripts
{
    public class Env
    {
        private static string DATABASE_PATH = null;

        public static string DatabasePath()
        {
            if (DATABASE_PATH == null)
                throw new Exception("Database path not set. Please set it using SetDatabasePath before calling DatabasePath.");
            return DATABASE_PATH;
        }

        public static void SetDatabasePath(string path)
        {
            DATABASE_PATH = path;
        }
    }
}
