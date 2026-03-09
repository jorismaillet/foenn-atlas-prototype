using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes
{
    public class Attribute
    {
        public static readonly Dictionary<WeatherHistoryAttributeKey, string> Translate = new()
        {
            { WeatherHistoryAttributeKey.NUM_POSTE, "Numéro de station" },
            { WeatherHistoryAttributeKey.NOM_USUEL, "Nom de station" },
            { WeatherHistoryAttributeKey.AAAAMMJJHH, "Date" },
            { WeatherHistoryAttributeKey.LAT, "Latitude" },
            { WeatherHistoryAttributeKey.LON, "Longitude" },
            { WeatherHistoryAttributeKey.DPT, "Département" }
        };

        public WeatherHistoryAttributeKey key;

        public Attribute(WeatherHistoryAttributeKey key)
        {
            this.key = key;
        }
    }
}