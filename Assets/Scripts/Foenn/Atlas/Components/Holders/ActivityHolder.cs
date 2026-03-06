using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Unity.Commons.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    public class ActivityHolder : Holder<Activity>
    {
        public void SetName(TMP_Text text)
            {
                text.text = element.name;
        }
    }
}
