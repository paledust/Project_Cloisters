using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var bounceBall = other.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            EventHandler.Call_OnBallDead();
        }
    }
}
