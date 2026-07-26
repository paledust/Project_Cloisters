using UnityEngine;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

using TMPro;
using SimpleSaveSystem;

namespace SimpleLocalization
{
    public class LocalizeManager : Singleton<LocalizeManager>, ISaveable
    {
        [SerializeField] private LocalizedAsset<TMP_FontAsset> localeFont;
        [SerializeField, ShowOnly] private string byteGuid = Guid.NewGuid().ToString();
        public Guid guid{get{return new Guid(byteGuid);}}
        private TMP_FontAsset currentFont;

        private static int localeIndex = 0;
        private static readonly string[] localeKey = new string[] {"en", "ja", "zh", "zh-Hant"};

        public TMP_FontAsset CurrentFont => currentFont;
        public static event Action<TMP_FontAsset> OnLocaleUpdate;

        protected override void Awake()
        {
            base.Awake();
            localeFont.AssetChanged += OnFontChange;
            currentFont = localeFont.LoadAsset();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            localeFont.AssetChanged += OnFontChange;
        }

#region Locale Setting
        public static void NextLocale()
        {
            localeIndex ++;
            RefreshLocale();
            SaveManager.SaveGameState(0);
        }
        public static void PreviousLocale()
        {
            localeIndex --;
            RefreshLocale();
            SaveManager.SaveGameState(0);
        }
        static void RefreshLocale()
        {
            if(localeIndex >= localeKey.Length)
                localeIndex = 0;
            if(localeIndex < 0)
                localeIndex = localeKey.Length-1;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeKey[localeIndex]);
        }
        internal static string GetSelectedLocale() => localeKey[localeIndex];
#endregion

        void OnFontChange(TMP_FontAsset font)
        {
            currentFont = font;
            OnLocaleUpdate?.Invoke(font);
        }

#region Save
        public void RestoreState(PlayerSaveData state)
        {
            localeIndex = state.localeIndex;
            RefreshLocale();
        }
        public void CaptureState(ref PlayerSaveData saveData)
        {
            saveData.localeIndex = localeIndex;
        }
#endregion
    }
}