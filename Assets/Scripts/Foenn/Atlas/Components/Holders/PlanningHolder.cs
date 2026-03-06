using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Unity.Commons.Containers;
using Assets.Scripts.Unity.Commons.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    public class PlanningHolder : Holder<Planning>
    {
        public void SetTitle(TMP_Text text)
        {
            text.text = element.title;
        }
        public void SetPlannedActivities(PrefabsContainer container)
        {
            container.Initialize(element.plannedActivities);
        }
    }
}
