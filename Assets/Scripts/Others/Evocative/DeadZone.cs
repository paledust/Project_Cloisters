using SimpleAudioSystem;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private VFX_FallIntoHole vfx_fallInto;
    [SerializeField] private string sfxBallDead;
    void OnTriggerEnter(Collider other)
    {
        var bounceBall = other.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            vfx_fallInto.PlayFallVFXAt(other.transform.position);
            AudioManager.Instance.PlaySFX(sfxBallDead, 1);
            EventHandler.Call_OnBallFall();
        }
    }
}
