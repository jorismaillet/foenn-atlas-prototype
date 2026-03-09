using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.ETL.Models;
using Codice.Client.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.ETL.Dimensions
{
    public class TimeDimension : IDimension
    {
        public Datafield timestamp = new Datafield("timestamp", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield year = new Datafield("year", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield month = new Datafield("month", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield day = new Datafield("day", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield hour = new Datafield("hour", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield dayOfWeek = new Datafield("day_of_week", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield dayOfYear = new Datafield("day_of_year", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield weekOfYear = new Datafield("week_of_year", DbType.Int16, ColumnType.ATTRIBUTE);

        public string Name => "time";

        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
        };

        public List<Datafield> Columns => new List<Datafield>() { timestamp, year, month, day, hour, dayOfWeek, dayOfYear, weekOfYear };
        public List<Reference> References => new List<Reference>();
    }
}
