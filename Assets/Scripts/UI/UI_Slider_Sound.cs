using UnityEngine;
using UnityEngine.UI;

using SimpleAudioSystem;

public class UI_Slider_Sound : MonoBehaviour
{
    [SerializeField] private AudioSettingType settingType;
    [SerializeField] private Slider slider;
    void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChange);
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChange);
    }
    void OnSliderValueChange(float value)
    {
        AudioSettingService.SetAudioVolume(settingType, value);
    }
}
