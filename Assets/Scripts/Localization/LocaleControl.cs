using UnityEngine.Localization.Settings;
using UnityEngine;

public class LocaleControl : MonoBehaviour
{
    private static int LocaleIndex;
    private static readonly string[] locale = new string[] {"en", "zh", "zh-Hant"};

    public static void NextLocale()
    {
        LocaleIndex ++;
        RefreshLocale();
    }
    public static void PreviousLocale()
    {
        LocaleIndex --;
        RefreshLocale();
    }
    static void RefreshLocale()
    {
        if(LocaleIndex >= locale.Length)
            LocaleIndex = 0;
        if(LocaleIndex < 0)
            LocaleIndex = locale.Length-1;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(locale[LocaleIndex]);
    }
}
