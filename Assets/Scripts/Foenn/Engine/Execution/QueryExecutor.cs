using Assets.Scripts.Foenn.Engine.Inputs;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryExecutor
    {
        private readonly IConnector inputProvider;

        public QueryExecutor(IConnector inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public QueryResult Execute(QueryRequest request)
        {
            inputProvider.Initialize(request);
            return inputProvider.Execute();
        }
    }
}