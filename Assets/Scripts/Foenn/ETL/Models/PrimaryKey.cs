using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class PrimaryKey : Datafield
    {
        public bool autoIncrement;

        public PrimaryKey(string name, DbType type, bool autoIncrement) : base(name, type)
        {
            this.autoIncrement = autoIncrement;
        }
    }
}
