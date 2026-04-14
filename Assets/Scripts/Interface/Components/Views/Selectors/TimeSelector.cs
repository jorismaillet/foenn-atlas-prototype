using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Selectors
{
    public class TimeSelector : MonoBehaviour
    {
        public TMPro.TMP_Dropdown yearDropdown, monthDropdown, dayDropdown;

        private void Start()
        {
            yearDropdown.ClearOptions();
            monthDropdown.ClearOptions();
            dayDropdown.ClearOptions();
            yearDropdown.AddOptions(TimeService.ListYears().Select(y => y.ToString()).ToList());
            monthDropdown.AddOptions(months);
            dayDropdown.AddOptions(Enumerable.Range(1, 31).Select(m => m.ToString()).ToList());
        }

        private readonly List<string> months = new()
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };
    }
}
