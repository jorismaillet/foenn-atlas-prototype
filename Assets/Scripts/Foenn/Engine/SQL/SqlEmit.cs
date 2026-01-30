using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Foenn.Engine.SQL
{

    public sealed class SqlEmit
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _p;

        public List<SqlParam> Parameters { get; } = new List<SqlParam>();

        public void Append(string s) => _sb.Append(s);
        public void AppendLine(string s) => _sb.AppendLine(s);

        public string AddParam(object value)
        {
            string name = "@p" + _p++;
            Parameters.Add(new SqlParam(name, value));
            return name;
        }

        public override string ToString() => _sb.ToString();
    }
}