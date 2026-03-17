using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;

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
