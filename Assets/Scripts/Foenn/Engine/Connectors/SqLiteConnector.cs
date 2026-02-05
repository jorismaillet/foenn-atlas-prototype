using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL;
using Assets.Scripts.Foenn.ETL.WeatherHistory;
using System.Data.SQLite;
using System.IO;

namespace Assets.Scripts.Foenn.Engine.Inputs.Databases
{
    public class SqLiteConnector : SqlConnector
    {
        public const string dbPath = "Resources/sqlite/weather.db";

        public SqLiteConnector() : base(new SqliteDialect())
        {
        }

        public override void OpenSession()
        {
            string connString = $"Data Source={dbPath};Version=3;";
            connection = new SQLiteConnection(connString);
            connection.Open();
        }

        public override void CloseSession()
        {
            connection.Close();
        }

        public bool CheckDbExists()
        {
            return File.Exists(dbPath);
        }

        public void CreateDb()
        {
            OpenSession();
            ExecuteOperation($"CREATE DATABASE {Database.databaseName};");
            CloseSession();
        }

        public void CreateWeatherTable(Dataset dataset)
        {
            OpenSession();
            var columns = dataset.headers;
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{WeatherHistoryMetaData.table_name}\" ({columns});";
            ExecuteOperation(createTableSql);
            CloseSession();
        }
    }
}