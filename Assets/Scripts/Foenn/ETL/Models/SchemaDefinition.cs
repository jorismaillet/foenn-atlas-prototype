using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class SchemaDefinition
    {
        public string tableName;
        public List<Datafield> headers = new List<Datafield>();
        public List<ColumnIndex> indexes = new List<ColumnIndex>();
        public Dictionary<string, int> headersIndexes;

        public SchemaDefinition(string tableName)
        {
            this.tableName = tableName;
        }

        public void AddHeaders(List<Datafield> headesr)
        {
            this.headers = headesr;
            this.headersIndexes = headers.Select((f, i) => new { f.name, index = i }).ToDictionary(x => x.name, x => x.index);
        }
    }
}