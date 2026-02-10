using Assets.Scripts.Foenn.ETL.Transformers;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory
{
    public class WeatherHistoryTransformer : Transformer
    {
        public WeatherHistoryTransformer(Datasource datasource) : base(datasource)
        {
        }
    }
}