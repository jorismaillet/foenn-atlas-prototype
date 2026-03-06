using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Assets.Scripts.Foenn.ETL.Datasources
{
    public abstract class Datasource
    {
        public abstract string TableName();

        public Datasource()
        {

        }

        public abstract void PrepareSchema(SchemaDefinition schema);

        public abstract string[] GetExtraColumns(string[] columns);
    }
}