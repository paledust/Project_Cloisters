using SimpleAudioSystem;
using UnityEngine;

public class MirrorFlip : MonoBehaviour
{
    [SerializeField] private Transform mirrorTrans;
    [SerializeField] private AudioSource sfxFlip;
    [SerializeField] private string sfxOnFlip;
    [SerializeField, Range(0, 1)] private float sfxVolume;
    // Update is called once per frame
    void Update()
    {
        if(Vector3.Dot(Vector3.forward, mirrorTrans.forward)>0){
            mirrorTrans.localRotation *= Quaternion.Euler(180,0,0);
            sfxFlip.pitch = Random.Range(.95f, 1.05f);
            AudioManager.Instance.PlaySFX(sfxFlip, sfxOnFlip, sfxVolume);
        }
    }
}
