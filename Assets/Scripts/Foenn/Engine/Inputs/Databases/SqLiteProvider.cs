using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Data;

namespace Assets.Scripts.Foenn.Engine.Inputs.Databases
{
    public class SqLiteProvider : SqlProvider
    {
        public SqLiteProvider(IDbConnection connection) : base(connection, new SqliteDialect())
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