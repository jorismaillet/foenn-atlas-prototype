using Assets.Scripts.App.Translations;
using System.Globalization;

namespace Assets.Scripts.Common.Utils {
    public class FloatUtil {

        private static readonly NumberFormatInfo defaultFormatter = new NumberFormatInfo() {
            NumberDecimalSeparator = Locale.Get(StringId.DECIMAL_SEPARATOR),
            NumberDecimalDigits = 2,
            NumberGroupSeparator = Locale.Get(StringId.THOUSANDS_SEPARATOR)
        };

        public static readonly NumberFormatInfo oneDigitFormatter = new NumberFormatInfo() {
            NumberDecimalSeparator = Locale.Get(StringId.DECIMAL_SEPARATOR),
            NumberDecimalDigits = 1,
            NumberGroupSeparator = Locale.Get(StringId.THOUSANDS_SEPARATOR)
        };

        public static string ToString(float f) {
            return string.Format(defaultFormatter, "{0:N}", f);
        }

        public static string MillisToSeconds(int millisValue) {
            float sValue = millisValue / 1000.0F;
            if (millisValue % 1000 == 0) {
                return ((int)sValue).ToString();
            }
            else if (millisValue % 100 == 0) {
                return string.Format(oneDigitFormatter, "{0:N}", sValue);
            }
            else {
                return ToString(sValue);
            }
        }

        public static float NormalizeHex(float f) {
            return f / 255F;
        }
    }
}