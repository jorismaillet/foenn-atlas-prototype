using Assets.Scripts.Foenn.Engine.Execution;

namespace Assets.Scripts.Foenn.Engine.Inputs.InMemory
{
    public abstract class InMemoryProvider : IInputProvider
    {
        public abstract void OpenFile();
        public abstract void CloseFile();
        public abstract void Initialize(QueryRequest request);
        public abstract QueryResult Execute();
    }
}