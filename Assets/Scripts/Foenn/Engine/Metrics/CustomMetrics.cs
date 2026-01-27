using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine
{
    public class CustomMetrics
    {
        public static double CalculerTemperatureRessentie(double temperature, double vitesseVent, double humiditeRelative)
        {
            if (temperature <= 10)
            {
                // Indice de refroidissement éolien
                return 13.12 + 0.6215 * temperature - 11.37 * Math.Pow(vitesseVent, 0.16) + 0.3965 * temperature * Math.Pow(vitesseVent, 0.16);
            }
            else
            {
                // Indice de chaleur
                double T = temperature * 9 / 5 + 32; // Convertir la température en °F
                double HI = -42.379 + 2.04901523 * T + 10.14333127 * humiditeRelative
                            - 0.22475541 * T * humiditeRelative - 0.00683783 * T * T
                            - 0.05481717 * humiditeRelative * humiditeRelative
                            + 0.00122874 * T * T * humiditeRelative
                            + 0.00085282 * T * humiditeRelative * humiditeRelative
                            - 0.00000199 * T * T * humiditeRelative * humiditeRelative;

                // Convertir l'indice de chaleur en °C
                HI = (HI - 32) * 5 / 9;
                return HI;
            }
        }
    }
}