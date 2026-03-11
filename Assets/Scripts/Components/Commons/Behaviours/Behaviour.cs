namespace Assets.Scripts.Components.Commons.Behaviours
{
    public abstract class Behaviour<T> : BaseBehaviour
    {
        protected void AddBehaviour(GameEvent<T> GameEvent, BehaviourAction behaviourActon)
        {
            AddListener(GameEvent, _ => Behave(behaviourActon));
        }

        protected void AddBehaviour(GameEvent<string> GameEvent, BehaviourAction behaviourActon)
        {
            AddListener(GameEvent, message => Behave(behaviourActon, message));
        }

        protected void AddBehaviour(GameEvent GameEvent, BehaviourAction behaviourActon)
        {
            AddListener(GameEvent, () => Behave(behaviourActon));
        }
    }
}
