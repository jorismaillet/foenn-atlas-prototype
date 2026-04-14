using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;

namespace Assets.Scripts.Services
{
    public class PostService
    {
        public static Dictionary<int, Row> ListPosts()
        {
            return TableService.RetrieveIdAndFields(
                WeatherHistoryDataset.Instance.location,
                WeatherHistoryDataset.Instance.location.PostName,
                WeatherHistoryDataset.Instance.location.Department
            );
        }
    }
}
