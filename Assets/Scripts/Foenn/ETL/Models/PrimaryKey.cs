using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class PrimaryKey : Datafield
    {
        public bool autoIncrement;

        public PrimaryKey(string name, Datatype type, bool autoIncrement) : base(name, type)
        {
            this.autoIncrement = autoIncrement;
        }
    }
}
