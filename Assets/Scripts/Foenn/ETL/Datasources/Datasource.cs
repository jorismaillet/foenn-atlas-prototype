using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Datasources
{
    public abstract class Datasource
    {
        public abstract string TableName();
        public abstract string InsertIdColumn();
        public abstract string Identifier(Dictionary<string, int> headerIndexes, string[] line);

        private string[] extraColumns;
        public Datasource(int nbExtraColumns)
        {
            extraColumns = new string[nbExtraColumns];
        }

        public abstract void PrepareTranformer(SchemaDefinition schema);

        public abstract string[] GetExtraColumns(string[] columns);
    }
}