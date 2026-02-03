namespace Assets.Scripts.Unity.Commons.Attachers {
    public abstract class AttributesAttacher<T> : Attacher<T> where T : class {
        public override void Initialize(T element) {
            onInitialize.Invoke();
        }
    }
}
