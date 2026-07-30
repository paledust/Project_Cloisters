using UnityEngine;
using UnityEngine.UI;

using SimpleAudioSystem;

public class UI_Slider_Sound : MonoBehaviour
{
    [SerializeField] private AudioSettingType settingType;
    [SerializeField] private Slider slider;
    [SerializeField, Range(0, 2)] private float valueRematch = 1;
    void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChange);
    }
    void Start()
    {
        slider.SetValueWithoutNotify(AudioSettingService.GetAudioVolume(settingType));
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChange);
    }
    void OnSliderValueChange(float value)
    {
        AudioSettingService.SetAudioVolume(settingType, value * valueRematch);
    }
}
