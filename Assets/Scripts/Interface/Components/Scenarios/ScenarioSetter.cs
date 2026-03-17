using UnityEngine;

namespace Assets.Scripts.Interface.Components.Scenarios
{
    public class ScenarioSetter : MonoBehaviour
    {
        public ScenarioKey scenarioKey;

        public void SetScenario()
        {
            Main.selectedScenario.Set(scenarioKey);
        }
    }
}
