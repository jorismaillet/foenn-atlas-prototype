using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public sealed class SqlEmit
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private int parameterIndex;

        public List<SqlParam> parameters = new List<SqlParam>();

        public void Append(string s) => stringBuilder.Append(s);
        public void AppendLine(string s) => stringBuilder.AppendLine(s);

        public string AddParam(object value)
        {
            string name = "@p" + parameterIndex++;
            parameters.Add(new SqlParam(name, value));
            return name;
        }

        public override string ToString() => stringBuilder.ToString();
    }
}