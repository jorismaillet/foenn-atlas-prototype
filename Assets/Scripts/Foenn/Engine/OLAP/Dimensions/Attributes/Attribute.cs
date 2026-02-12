using Assets.Scripts.Foenn.Engine.Sql.Dialects;
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
            { WeatherHistoryAttributeKey.DAY_OF_WEEK, "Jour de la semaine" },
            { WeatherHistoryAttributeKey.DAY_OF_MONTH, "Jour" },
            { WeatherHistoryAttributeKey.DAY_OF_YEAR, "Jour de l'année" },
            { WeatherHistoryAttributeKey.MONTH, "Mois" },
            { WeatherHistoryAttributeKey.YEAR, "Année" },
            { WeatherHistoryAttributeKey.DPT, "Département" }
        };

        public WeatherHistoryAttributeKey key;

        public Attribute(WeatherHistoryAttributeKey key)
        {
            this.key = key;
        }
    }
}