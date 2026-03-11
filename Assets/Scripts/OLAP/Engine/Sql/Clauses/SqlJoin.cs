using System.Collections.Generic;
using Assets.Scripts.OLAP.Engine.Sql.Joins;

namespace Assets.Scripts.OLAP.Engine.Sql.Clauses
{
    public class SqlJoin
    {
        public readonly string clause;

        public SqlJoin(List<JoinDefinition> joins)
        {
            var statement = new List<string>();
            foreach (var join in joins)
            {
                statement.Add($"{JoinTypeToSql(join.joinType)} ON {join.leftField.ToSql()} = {join.rightField.ToSql()}");
            }
            clause = string.Join("", statement);
        }

        private string JoinTypeToSql(JoinType joinType)
        {
            switch (joinType)
            {
                case JoinType.INNER:
                    return "INNER JOIN";
                case JoinType.OUTER:
                    return "OUTER JOIN";
                case JoinType.LEFT:
                    return "LEFT JOIN";
                case JoinType.RIGHT:
                    return "RIGHT JOIN";
                default:
                    throw new System.Exception($"Unsupported join type: {joinType}");
            }
        }
    }
}
