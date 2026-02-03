namespace Assets.Scripts.App.Translations {
    public class Locale {
        public static Mutable<Language> language = new Mutable<Language>(Language.ENGLISH);

        public static string Get(UIStringId key) {
            return language.Value switch {
                Language.ENGLISH => key switch {
                    UIStringId.HELLO => "Hello:",
                    _ => throw new System.NotImplementedException("Missing translation!"),
                },
                Language.FRENCH => throw new System.NotImplementedException(),
                _ => throw new System.NotImplementedException("Unsupported language!"),
            };
        }

        public static string Get(StringId key, params object[] args) {
            return string.Format(Get(key), args);
        }
    }
}