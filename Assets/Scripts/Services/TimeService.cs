using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;

namespace Assets.Scripts.Services
{
    public class TimeService
    {
        public static List<int> ListYears()
        {
            return TableService.RetrieveMembers<int>(
                WeatherHistoryDataset.Instance.time,
                WeatherHistoryDataset.Instance.time.year
            );
        }
    }
}
