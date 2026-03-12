using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Components.Logger;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Schema;
using Mono.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;

namespace Assets.Scripts.OLAP.Engine
{
    public class QueryRequest
    {
        private Query query;
        private List<Field> selectedFields;

        public QueryRequest(ITable from)
        {
            query = new Query(from.name);
            selectedFields = new List<Field>();
        }

        public QueryRequest Select(params Field[] selectedColumns)
        {
            selectedFields.AddRange(selectedColumns);
            foreach (var item in selectedColumns)
            {
                query.Select(item.name);
            }
            return this;
        }

        public QueryRequest SelectAvg(Field field)
        {
            selectedFields.Add(field);
            query.SelectAvg(field.name);
            return this;
        }

        public QueryRequest Join(Field refField)
        {
            query.Join(refField.referencedDimension.name, $"{refField.table.name}.{refField.name}", $"{refField.referencedDimension.name}.{refField.referencedDimension.PrimaryKey.name}");
            return this;
        }

        public QueryRequest GroupBy(params Field[] fields)
        {
            query.GroupBy(fields.Select(c => c.name).ToArray());
            return this;
        }

        public QueryRequest WhereIn(Field field, IEnumerable<object> values)
        {
            query.WhereIn(field.name, values);
            return this;
        }

        public QueryRequest WhereEq(Field field, object value)
        {
            query.Where(field.name, value);
            return this;
        }

        public QueryRequest WhereNotNull(Field field)
        {
            query.WhereNotNull(field.name);
            return this;
        }

        public QueryRequest WhereBetween<T>(Field field, T lower, T higher)
        {
            query.WhereBetween(field.name, lower, higher);
            return this;
        }


        public QueryResult Execute(SqliteConnection connection)
        {
            var queryTime = new Stopwatch();
            queryTime.Start();
            using (var reader = SqliteHelper.ExecuteReader(connection, query))
            {
                int nbColumns = reader.FieldCount;
                string[] rawHeaders = new string[nbColumns];
                for (int i = 0; i < nbColumns; i++)
                {
                    rawHeaders[i] = reader.GetName(i);
                }
                var result = new QueryResult(selectedFields);
                while (reader.Read())
                {
                    var row = new object[nbColumns];
                    reader.GetValues(row);
                    result.ParseLine(row);
                }
                queryTime.Stop();
                MainThreadLog.Log($"Query completed in {queryTime.ElapsedMilliseconds}ms");
                return result;
            }
        }
    }
}
