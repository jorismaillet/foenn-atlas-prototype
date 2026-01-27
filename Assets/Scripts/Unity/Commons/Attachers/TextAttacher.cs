using Assets.Scripts.Unity.Attachers;
using Assets.Scripts.Unity.Utils;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Commons.Attachers {
    public abstract class TextAttacher<Element> : Attacher<Element> where Element : class {
        public override void Initialize(Element element) {
            SetText(Text(element));
        }

        public abstract string Text(Element element);
    }
}
