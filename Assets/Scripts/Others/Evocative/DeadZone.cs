using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private IC_Evocative iC_Evocative;
    void OnTriggerEnter(Collider other)
    {
        var bounceBall = other.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            iC_Evocative.RespawnGame();
        }
    }
}
