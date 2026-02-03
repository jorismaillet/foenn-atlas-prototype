using System;

namespace Assets.Scripts.Common.Utils {
    public static class MathUtil {
        public static float Coef(float firstLimit, float secondLimit, float value) {
            if (firstLimit > secondLimit) {
                return Coef(secondLimit, firstLimit, firstLimit - value);
            }
            else if (firstLimit == secondLimit) {
                return 1.0F;
            }
            return Math.Abs(Restrict(firstLimit, secondLimit, value) - firstLimit) / Math.Abs(secondLimit - firstLimit);
        }

        public static float Lerp(float firstLimit, float secondLimit, float valueInLimit, float minResult, float maxResult) {
            float coef = Coef(firstLimit, secondLimit, valueInLimit);
            return coef * maxResult + (1 - coef) * minResult;
        }

        public static float Restrict(float firstLimit, float secondLimit, float value) {
            float minLimit = Math.Min(firstLimit, secondLimit);
            float maxLimit = Math.Max(firstLimit, secondLimit);
            return Math.Min(Math.Max(minLimit, value), maxLimit);
        }

        public static bool Between(int value, int minValue, int maxValue) {
            return value >= minValue && value <= maxValue;
        }
    }
}