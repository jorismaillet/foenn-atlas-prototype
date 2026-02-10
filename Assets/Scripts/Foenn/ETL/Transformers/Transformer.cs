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

        public void TransformHeaders(SchemaDefinition schema)
        {
            schema.headers.Insert(0, new Datafield("ID", Datatype.PRIMARY_KEY));
            schema.headers.Insert(1, new Datafield(datasource.InsertIdColumn(), Datatype.STRING));
        }

        public void TransformLine(SchemaDefinition schema, List<string> line)
        {
            var insertId = datasource.Identifier(schema.headersIndexes, line);
            line.Insert(0, "");
            line.Insert(1, insertId);
        }
    }
}