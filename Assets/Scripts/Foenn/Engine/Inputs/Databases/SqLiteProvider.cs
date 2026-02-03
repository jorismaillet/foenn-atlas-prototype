using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Data;

namespace Assets.Scripts.Foenn.Engine.Inputs.Databases
{
    public class SqLiteProvider : SqlProvider
    {
        public const string dbPath = "Resources/sqlite/weather.db";

        public SqLiteProvider() : base(new SqliteDialect())
        {
        }

        public override void CloseSession()
        {
            throw new System.NotImplementedException();
        }

        public override void OpenSession()
        {
            throw new System.NotImplementedException();
        }
    }
}