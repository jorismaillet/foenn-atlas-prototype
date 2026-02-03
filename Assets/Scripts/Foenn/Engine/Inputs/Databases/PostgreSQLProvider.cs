using Assets.Scripts.Foenn.Engine.Sql.Dialects;

namespace Assets.Scripts.Foenn.Engine.Inputs.Databases
{
    public class PostgreSQLProvider : SqlProvider
    {
        public PostgreSQLProvider() : base(new PostgresDialect())
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