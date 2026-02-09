using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.ETL.Datasources;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Loaders
{
    public class SqliteLoader : Loader
    {

        public SqliteLoader(Datasource datasource, string databasePath = SqliteConnector.DATABASE_PATH) : base(datasource, new SqliteConnector(databasePath))
        {
        }

        //TODO Move sql into connectors

        public override void CreateTable(Dataset dataset)
        {
            var pk = "ID INTEGER PRIMARY KEY AUTOINCREMENT";
            var columns = new List<string> { pk };
            dataset.fields.ForEach(field => {
                columns.Add($"{field.name} {connector.typeToSql(field.type)}");
            });
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{datasource.TableName()}\" ({string.Join(", ", columns)});";
            connector.ExecuteOperation(createTableSql);
        }

        public override void Load(Dataset dataset)
        {
            var tableName = datasource.TableName();
            var pk = datasource.IdColumn();
            var columnsString = string.Join(", ", dataset.fields.Select(f => f.name));
            var idIndex = dataset.fields.FindIndex(f => f.name == datasource.IdColumn());
            dataset.lines.ForEach(line =>
            {
                if (!connector.Exists(tableName, pk, line[idIndex]))
                {
                    var values = string.Join(", ", line.Select(val => string.IsNullOrEmpty(val) ? "NULL" : val));
                    var sql = $"INSERT INTO \"{tableName}\" ({columnsString}) VALUES ({values})";
                    connector.ExecuteOperation(sql);
                }
            });
        }
    }
}