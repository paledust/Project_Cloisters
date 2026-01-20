using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class Hoverable_PlaySFX : MonoBehaviour
{
    [SerializeField] private string hoverSFX;
    [SerializeField, Range(0, 1)] private float volume = 1f;
    private Basic_Clickable self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Basic_Clickable>();
        self.onHover += PlayHoverSFX;
    }
    void OnDestroy()
    {
        self.onHover -= PlayHoverSFX;
    }
    void PlayHoverSFX()
    {
        AudioManager.Instance.PlaySoundEffect(hoverSFX, volume);
    }
}
