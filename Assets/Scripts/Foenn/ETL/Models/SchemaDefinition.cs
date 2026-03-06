using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class SchemaDefinition
    {
        public string tableName;
        public List<Datafield> columns = new List<Datafield>();
        public List<IndexDefinition> indexes = new List<IndexDefinition>();
        public Dictionary<string, int> headersIndexes;

        public SchemaDefinition(string tableName)
        {
            this.tableName = tableName;
        }

        public void AddColumns(List<Datafield> columns)
        {
            this.columns = columns;
            this.headersIndexes = columns.Select((f, i) => new { f.name, index = i }).ToDictionary(x => x.name, x => x.index);
        }
    }
}