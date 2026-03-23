using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
