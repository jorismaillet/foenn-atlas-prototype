using System;

namespace Assets.Scripts.Foenn.Engine.Metrics.CustomMetrics
{
    public class TemperatureRessentie : CustomMetric
    {
        private double temperature;
        private double windSpeed;
        private double relativeHumidity;

        public TemperatureRessentie(double temperature, double windSpeed, double relativeHumidity)
        {
            this.temperature = temperature;
            this.windSpeed = windSpeed;
            this.relativeHumidity = relativeHumidity;
        }

        public override float Value()
        {
            if (temperature <= 10)
            {
                // Indice de refroidissement éolien
                return 13.12 + 0.6215 * temperature - 11.37 * Math.Pow(windSpeed, 0.16) + 0.3965 * temperature * Math.Pow(windSpeed, 0.16);
            }
            else
            {
                // Indice de chaleur
                double T = temperature * 9 / 5 + 32; // Convertir la température en °F
                double HI = -42.379 + 2.04901523 * T + 10.14333127 * relativeHumidity
                            - 0.22475541 * T * relativeHumidity - 0.00683783 * T * T
                            - 0.05481717 * relativeHumidity * relativeHumidity
                            + 0.00122874 * T * T * relativeHumidity
                            + 0.00085282 * T * relativeHumidity * relativeHumidity
                            - 0.00000199 * T * T * relativeHumidity * relativeHumidity;

                // Convertir l'indice de chaleur en °C
                HI = (HI - 32) * 5 / 9;
                return HI;
            }
        }
    }
}