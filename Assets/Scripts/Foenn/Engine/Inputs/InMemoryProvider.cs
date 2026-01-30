using Assets.Scripts.Foenn.Engine.Execution;

namespace Assets.Scripts.Foenn.Engine.Inputs
{
    public interface InMemoryProvider : IInputProvider
    {
        void Open();
        void Close();
    }
}