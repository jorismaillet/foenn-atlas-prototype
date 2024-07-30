using System.Collections.Generic;

namespace Assets.Resources.Weathers
{
    public class WeatherFieldDescription
    {
        public string shortDescription;
        public string officialDescription;
        public List<string> valueDescription;

        public WeatherFieldDescription(string shortDescription, string officialDescription, List<string> valueDescription = null)
        {
            this.shortDescription = shortDescription;
            this.officialDescription = officialDescription;
            this.valueDescription = valueDescription;
        }
    }
}