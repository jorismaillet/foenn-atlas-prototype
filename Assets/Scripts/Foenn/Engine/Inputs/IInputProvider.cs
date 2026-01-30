using Assets.Scripts.Foenn.Engine.Requests;
using Assets.Scripts.Foenn.Engine.Results;
using Assets.Scripts.Foenn.ETL;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public interface IInputProvider
    {
        void Initialize(QueryRequest request);
        QueryResult Execute();
    }
}