using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Transformers
{
    public class Transformer
    {
        public Datasource datasource;

        public Transformer(Datasource datasource)
        {
            this.datasource = datasource;
        }

        public void TransformColumns(SchemaDefinition schema)
        {
            schema.primaryKey = new PrimaryKey("ID", DbType.String, false);
            schema.columns.Add(schema.primaryKey);
            schema.indexes.Add(schema.primaryKey);
        }

        public void TransformLine(SchemaDefinition schema, string[] line)
        {

        }
    }
}