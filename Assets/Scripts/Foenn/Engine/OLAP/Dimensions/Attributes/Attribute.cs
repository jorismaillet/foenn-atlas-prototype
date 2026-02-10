using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes
{
    public class Attribute
    {
        private static readonly Dictionary<WeatherHistoryAttributeKey, string> attributeNames = new()
        {
            { WeatherHistoryAttributeKey.NUM_POSTE, "Numéro de station" },
            { WeatherHistoryAttributeKey.NOM_USUEL, "Nom de station" },
            { WeatherHistoryAttributeKey.AAAAMMJJHH, "Date" },
            { WeatherHistoryAttributeKey.LAT, "Latitude" },
            { WeatherHistoryAttributeKey.LON, "Longitude" },
            { WeatherHistoryAttributeKey.DAY_OF_WEEK, "Jour de la semaine" },
            { WeatherHistoryAttributeKey.DAY_OF_MONTH, "Jour" },
            { WeatherHistoryAttributeKey.DAY_OF_YEAR, "Jour de l'année" },
            { WeatherHistoryAttributeKey.MONTH, "Mois" },
            { WeatherHistoryAttributeKey.YEAR, "Année" },
            { WeatherHistoryAttributeKey.DPT, "Département" }
        };

        public WeatherHistoryAttributeKey key;
        public string value;

        public Attribute(WeatherHistoryAttributeKey key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public static string Name(WeatherHistoryAttributeKey key)
        {
            if (attributeNames.TryGetValue(key, out var res))
            {
                return res;
            }
            return key.ToString();
        }
    }
}