using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Atlas.Datasets.Common.Schema;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;
using System.Data;

namespace Assets.Scripts.Foenn.ETL.Dimensions
{
    public class LocationDimension : IDimension
    {
        public Datafield Latitude = new Datafield("lat", DbType.Double, ColumnType.ATTRIBUTE);
        public Datafield Longitude = new Datafield("lon", DbType.Double, ColumnType.ATTRIBUTE);
        public Datafield PostName = new Datafield("post_name", DbType.String, ColumnType.ATTRIBUTE);
        public Datafield PostNumber = new Datafield("post_number", DbType.Int16, ColumnType.ATTRIBUTE);
        public Datafield Department = new Datafield("department", DbType.String, ColumnType.ATTRIBUTE);
        public Datafield Altitude = new Datafield("altitude", DbType.Int16, ColumnType.ATTRIBUTE);

        public string Name => "location";

        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(false, Department),
            new IndexDefinition(false, PostNumber),
            new IndexDefinition(false, PostName),
            new IndexDefinition(false, Latitude, Longitude)
        };

        public List<Datafield> Columns => new List<Datafield>() { Latitude, Longitude, PostName, PostNumber, Department, Altitude };
        public List<Reference> References => new List<Reference>();
    }
}
