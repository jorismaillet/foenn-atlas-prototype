using Assets.Scripts.Foenn.Engine.Execution;

namespace Assets.Scripts.Foenn.Engine.Inputs
{
    public interface IInputProvider
    {
        void Initialize(QueryRequest request);
        QueryResult Execute();
    }
}