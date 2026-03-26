using Assets.Scripts.Helpers;
using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Interface.Components.Views.Navigation;
using UnityEngine;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public static Mutable<ViewKey> selectedView = new Mutable<ViewKey>();

        private void Awake()
        {
            Env.DatabasePath = SqliteHelper.DATABASE_PATH;
            selectedView.Set(ViewKey.HOUR_NO_RAIN);
        }
    }
}
