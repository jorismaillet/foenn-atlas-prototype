using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Common.Actions
{
    public class InputFieldActions : BaseBehaviour
    {
        public void Clear(InputField input)
        {
            input.text = string.Empty;
        }
    }
}
