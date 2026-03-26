using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Views.Selectors
{
    public class LocationSelector : MonoBehaviour
    {
        public TMPro.TMP_InputField departmentInput;
        public TMPro.TMP_Dropdown postDropdown;

        private LocationDimension location;
        private Dictionary<int, Row> posts;

        private void Start()
        {
            location = WeatherHistoryDataset.Instance.location;
            posts = PostService.ListPosts();
            SetAllPosts();
            departmentInput.onValueChanged.AddListener((department) =>
            {
                if (string.IsNullOrEmpty(department)) SetAllPosts(); else SetPostsForDepartment(department);
            });
        }

        private void SetAllPosts()
        {
            postDropdown.ClearOptions();
            postDropdown.AddOptions(posts.Values.Select(p => p.StringValue(location.PostName)).ToList());
        }

        private void SetPostsForDepartment(string department)
        {
            postDropdown.ClearOptions();
            postDropdown.AddOptions(posts.Values.Where(p => p.values[location.Department].Equals(department)).Select(p => p.StringValue(location.PostName)).ToList());
        }
    }
}
