using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private VFX_FallIntoHole vfx_fallInto;
    void OnTriggerEnter(Collider other)
    {
        var bounceBall = other.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            vfx_fallInto.PlayFallVFX();
            EventHandler.Call_OnBallFall();
        }
    }
}
