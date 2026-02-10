using Assets.Scripts.Foenn.ETL.Transformers;

namespace Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory
{
    public class WeatherHistoryTransformer : Transformer
    {
        public WeatherHistoryTransformer(Datasource datasource) : base(datasource)
        {
        }
    }
}