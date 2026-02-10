using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Models;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Loaders
{
    public class SqliteLoader : Loader
    {

        public SqliteLoader(Datasource datasource, string databasePath = SqliteConnector.DATABASE_PATH) : base(datasource, new SqliteConnector(databasePath))
        {
        }

        public override void CreateTable(Dataset dataset)
        {
            connector.CreateTable(datasource.TableName(), dataset.fields);
        }

        public override void Load(Dataset dataset)
        {
            var tableName = datasource.TableName();
            var pk = datasource.InsertIdColumn();
            var idIndex = dataset.fields.FindIndex(f => f.name == datasource.InsertIdColumn());
            dataset.lines.ForEach(line =>
            {
                if (!connector.Exists(tableName, pk, line[idIndex]))
                {
                    connector.Insert(tableName, dataset.fields.Select(f => f.name).ToList(), line);
                }
            });
        }
    }
}