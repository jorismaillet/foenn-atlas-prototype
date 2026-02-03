using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Attributes
{
    public class Attribute
    {
        private static readonly Dictionary<AttributeKey, string> attributeNames = new()
        {
            { AttributeKey.NUM_POSTE, "Numéro de station" },
            { AttributeKey.NOM_USUEL, "Nom de station" },
            { AttributeKey.AAAAMMJJHH, "Date" },
            { AttributeKey.LAT, "Latitude" },
            { AttributeKey.LON, "Longitude" },
            { AttributeKey.DAY_OF_WEEK, "Jour de la semaine" },
            { AttributeKey.DAY_OF_MONTH, "Jour" },
            { AttributeKey.DAY_OF_YEAR, "Jour de l'année" },
            { AttributeKey.MONTH, "Mois" },
            { AttributeKey.YEAR, "Année" },
            { AttributeKey.DPT, "Département" }
        };

        public string name;
        public AttributeKey key;

        public Attribute(AttributeKey key, string name)
        {
            this.name = name;
            this.key = key;
        }

        public static string Name(AttributeKey key)
        {
            if (attributeNames.TryGetValue(key, out var res))
            {
                return res;
            }
            return key.ToString();
        }
    }
}