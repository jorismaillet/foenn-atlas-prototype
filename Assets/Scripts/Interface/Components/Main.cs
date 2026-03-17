using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Components.Commons.Mutables;
using Assets.Scripts.Database;
using Assets.Scripts.Interface.Components.Scenarios;
using Assets.Scripts.OLAP.Datasets;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using UnityEngine;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public static Mutable<ScenarioKey> selectedScenario = new Mutable<ScenarioKey>();

        private void Awake()
        {
            Env.DatabasePath = SqliteHelper.DATABASE_PATH;
            selectedScenario.Set(ScenarioKey.HOUR_NO_RAIN);
        }
    }
}
