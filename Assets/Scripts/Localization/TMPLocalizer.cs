using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace SimpleLocalization
{
    public class TMPLocalizaer : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private LocalizedString localizedString;
        void Reset()
        {
            tmpText =  GetComponent<TMP_Text>();
        }
        void Start()
        {
            ApplyFont(LocalizeManager.Instance.CurrentFont);
            LocalizeManager.OnLocaleUpdate += ApplyFont;
        }
        void OnDestroy()
        {
            LocalizeManager.OnLocaleUpdate -= ApplyFont;
        }
        void ApplyFont(TMP_FontAsset font)
        {
            tmpText.font = font;
            tmpText.SetAllDirty();
            tmpText.ForceMeshUpdate();
            tmpText.text = localizedString.GetLocalizedString();
        }
    }
}
