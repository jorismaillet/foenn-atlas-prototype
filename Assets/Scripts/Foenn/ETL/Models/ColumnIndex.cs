using System;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class ColumnIndex
    {
        public string name, column;

        public ColumnIndex(string indexName, string columnName)
        {
            this.name = indexName;
            this.column = columnName;
        }
    }
}