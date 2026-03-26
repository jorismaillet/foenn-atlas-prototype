using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Navigation
{
    public class ShowForView : MonoBehaviour
    {
        public List<ViewKey> cases;

        private void Start()
        {
            Main.selectedView.onChange.AddListener((selectedCase) => gameObject.SetActive(this.cases.Contains(selectedCase)));
        }
    }
}
