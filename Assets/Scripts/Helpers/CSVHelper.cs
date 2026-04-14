using System;

namespace Assets.Scripts.Helpers
{
    public class CSVHelper
    {
        public static int FindCsvIndex(string fieldName, string[] csvFieldNames)
        {
            return Array.FindIndex(csvFieldNames, name => name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
