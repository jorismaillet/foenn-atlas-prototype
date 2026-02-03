using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.OLAP;
using Assets.Scripts.Foenn.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    public class ActivitySuit
    {
        public Activity activity;
        public List<Row> suitRows;

        public ActivitySuit(Activity activity, IEnumerable<Row> records)
        {
            this.activity = activity;
            this.suitRows = records.Where(record => activity.SuitsHour(record)).ToList();
        }
    }
}
