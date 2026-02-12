using System.Data;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class Datafield
    {
        public string name;
        public DbType type;

        public Datafield(string name, DbType type)
        {
            this.name = name;
            this.type = type;
        }
    }
}