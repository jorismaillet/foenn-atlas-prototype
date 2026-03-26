using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Navigation
{
    public class ViewActivator : MonoBehaviour
    {
        public ViewKey caseKey;

        public void SetScenario()
        {
            Main.selectedView.Set(caseKey);
        }
    }
}
