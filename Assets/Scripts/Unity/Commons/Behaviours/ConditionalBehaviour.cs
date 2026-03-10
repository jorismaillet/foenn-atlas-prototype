namespace Assets.Scripts.Unity.Commons.Behaviours
{
    public abstract class ConditionalBehaviour<T> : Behaviour<T>
    {
        public abstract bool Conditional(T t);

        protected void AddConditionalBehaviour(GameEvent<T> GameEvent, ConditionalBehaviourAction behaviourActon)
        {
            AddListener(GameEvent, t => Behave(t, behaviourActon));
        }

        protected void Behave(T t, ConditionalBehaviourAction action)
        {
            switch (action)
            {
                case ConditionalBehaviourAction.SetActive:
                    gameObject.SetActive(Conditional(t));
                    break;
            }
        }
    }
}
