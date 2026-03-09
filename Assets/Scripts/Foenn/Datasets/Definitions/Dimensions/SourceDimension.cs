
using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Atlas.Datasets.Definitions.Metadata
{
    public class SourceDimension : IDimension
    {
        public static string tableName = "source";
        public PrimaryKey primaryKey = new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);
        public Datafield csvFileName = new Datafield("csv_file_name", DbType.String, ColumnType.ATTRIBUTE);

        public string Name => tableName;
        public PrimaryKey PrimaryKey => primaryKey;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>();

        public List<Datafield> Columns => new List<Datafield>() { primaryKey, csvFileName };

        public List<Reference> References => throw new NotImplementedException();
    }
}
