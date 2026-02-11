using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn
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
