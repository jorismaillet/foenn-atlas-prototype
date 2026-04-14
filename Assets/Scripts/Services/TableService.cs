using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.Services
{
    public class TableService
    {
        public static Dictionary<int, Row> RetrieveIdAndFields(Table table, params Field[] fields)
        {
            var query = new QueryRequest(table).SelectGroup(table.PrimaryKey).SelectGroup(fields).Distinct();
            List<Row> rows;
            using (var connection = SqliteHelper.CreateConnection())
            {
                rows = query.Execute(connection).rows;
            }
            var res = new Dictionary<int, Row>();
            foreach (var row in rows)
            {
                res[row.IntValue(table.PrimaryKey)] = row;
            }
            return res;
        }

        public static List<T> RetrieveMembers<T>(Table table, Field field)
        {
            var query = new QueryRequest(table).SelectGroup(field).Distinct();
            using (var connection = SqliteHelper.CreateConnection())
            {
                return query.Execute(connection).rows.Select(row =>
                {
                    var val = row.values[field];
                    return val == null ? default(T) : (T)Convert.ChangeType(val, typeof(T));
                }).ToList();
            }
        }

        public static object ValueFor(Table table, Field targetField, Field searchField, string searchValue)
        {
            return new QueryRequest(table)
                .Select(targetField)
                .WhereEq(searchField, searchValue)
                .Execute(SqliteHelper.CreateConnection())
                .rows.First()
                .values[targetField];
        }
    }
}
