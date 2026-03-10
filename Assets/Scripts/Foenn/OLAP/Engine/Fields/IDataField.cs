using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.OLAP.Engine.Sql
{
    public interface IDataField
    {
        public DbType dbType { get; }
        public string ToSql();
    }
}
