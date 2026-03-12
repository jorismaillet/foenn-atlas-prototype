using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Components.Logger;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;
using SqlKata;

namespace Assets.Scripts.OLAP.Engine
{
    public static class QueryRequest
    {
        public static Query From(ITable from)
        {
            return new Query(from.name);
        }

        public static Query Select(this Query query, params Field[] selectedColumns)
        {
            return query.Select(selectedColumns.Select(c => c.name).ToArray());
        }

        public static Query SelectAvg(this Query query, Field field)
        {
            return query.SelectAvg(field.name);
        }

        public static Query Join(this Query query, Field refField)
        {
            query.Join(refField.table.name, refField.name, refField.table.PrimaryKey.name);
            return query;
        }

        public static Query GroupBy(this Query query, params Field[] fields)
        {
            query.GroupBy(fields.Select(c => c.name).ToArray());
            return query;
        }

        public static Query WhereIn(this Query query, Field field, IEnumerable<object> values)
        {
            query.WhereIn(field.name, values);
            return query;
        }

        public static Query WhereEq(this Query query, Field field, object value)
        {
            query.Where(field.name, value);
            return query;
        }

        public static Query WhereNotNull(this Query query, Field field)
        {
            query.WhereNotNull(field.name);
            return query;
        }

        public static Query WhereBetween<T>(this Query query, Field field, T lower, T higher)
        {
            query.WhereBetween(field.name, lower, higher);
            return query;
        }

        public static QueryResult Execute(this Query query, SqliteConnection connection)
        {
            var sql = ToSql();
            UnityEngine.Debug.Log($"Executing query : {sql}");
            var queryTime = new Stopwatch();
            queryTime.Start();
            using (var reader = SqliteHelper.ExecuteReader(connection, sql))
            {
                int nbColumns = reader.FieldCount;
                string[] rawHeaders = new string[nbColumns];
                for (int i = 0; i < nbColumns; i++)
                {
                    rawHeaders[i] = reader.GetName(i);
                }
                var res = new QueryResult(rawHeaders, selectedColumns);
                while (reader.Read())
                {
                    var row = new object[nbColumns];
                    reader.GetValues(row);
                    res.ParseLine(row);
                }
                queryTime.Stop();
                MainThreadLog.Log($"Query completed in {queryTime.ElapsedMilliseconds}ms");
                return res;
            }
        }
    }
}
