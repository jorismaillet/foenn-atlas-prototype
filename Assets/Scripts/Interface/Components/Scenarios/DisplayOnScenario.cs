using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Scenarios
{
    public class DisplayOnScenario : MonoBehaviour
    {
        public List<ScenarioKey> scenarios;

        private void Start()
        {
            Main.selectedScenario.onChange.AddListener((scenario) => gameObject.SetActive(this.scenarios.Contains(scenario)));
        }
    }
}
