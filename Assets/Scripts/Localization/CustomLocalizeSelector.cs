using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace SimpleLocalization
{
    public class CustomLocalizeSelector : IStartupLocaleSelector
    {
        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            string locale = LocalizeManager.GetSelectedLocale();
            return availableLocales.GetLocale(locale);
        }
    }
}
