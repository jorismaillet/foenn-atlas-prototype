namespace Assets.Scripts.Foenn.Atlas.Datasets.Definitions.Metadata
{
    using Assets.Scripts.Foenn.Atlas.Datasets.Common;
    using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
    using Assets.Scripts.Foenn.ETL.Dimensions;
    using Assets.Scripts.Foenn.ETL.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class SourceDimension : IDimension
    {
        public static string tableName = "source";

        public PrimaryKey primaryKey = new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public Field csvFileName = new Field("csv_file_name", DbType.String, ColumnType.ATTRIBUTE);

        public string Name => tableName;

        public PrimaryKey PrimaryKey => primaryKey;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>();

        public List<Field> Columns => new List<Field>() { primaryKey, csvFileName };

        public List<Reference> References => throw new NotImplementedException();
    }
}
