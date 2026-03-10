namespace Assets.Scripts.Unity.Scenes.Home.World.Actions
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class DebounceAction : DebouncedBehaviour
    {
        public UnityEvent DebounceSuccessActions;

        public Image debounceImage;

        public override double debounceTimeMillis { get; } = 1000;

        protected void Awake()
        {
            Scale(0);
        }

        public override void OnDebounceSuccess()
        {
            Scale(0);
            DebounceSuccessActions.Invoke();
        }

        public override void OnDebounceAttemp()
        {
            Scale(0);
            base.OnDebounceAttemp();
        }

        public override void OnDebounceProgress(double elapsedTime)
        {
            Scale((float)Normalize(elapsedTime, debounceTimeMillis));
        }

        private void Scale(float coef)
        {
            ScaleWidth(debounceImage.gameObject, debounceImage.transform.parent.gameObject, coef);
        }
    }
}
