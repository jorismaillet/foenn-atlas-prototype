using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Requests;
using Assets.Scripts.Foenn.Engine.Results;
using Assets.Scripts.Foenn.Engine.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Execution
{
public sealed class SimpleQueryExecutor : IQueryExecutor
{
    public SimpleQueryExecutor() { }

    public QueryResult Execute(QueryRequest request, System.Data.IDbConnection connection, SQL.ISqlDialect dialect)
    {
        // 1. Générer la requête SQL à partir du QueryRequest
        var compiled = new SQLGenerator(dialect).Generate(request);

        // 2. Exécuter la requête SQL sur la connexion
        var command = connection.CreateCommand();
        command.CommandText = compiled.Sql;
        foreach (var param in compiled.Parameters)
        {
            var dbParam = command.CreateParameter();
            dbParam.ParameterName = param.Name;
            dbParam.Value = param.Value;
            command.Parameters.Add(dbParam);
        }

        var rows = new List<QueryResult.Row>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                // À adapter selon le schéma de retour
                var metricValues = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    metricValues[reader.GetName(i)] = reader.GetValue(i);
                }
                rows.Add(new QueryResult.Row(DateTimeOffset.MinValue, null, metricValues));
            }
        }
        return new QueryResult { Rows = rows };
    }
    }
}