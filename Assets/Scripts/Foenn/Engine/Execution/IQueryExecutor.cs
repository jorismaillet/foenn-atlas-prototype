using Assets.Scripts.Foenn.Engine.Requests;
using Assets.Scripts.Foenn.Engine.Results;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public interface IQueryExecutor
    {
        QueryResult Execute(QueryRequest request);
    }
}