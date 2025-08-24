using UnityEngine;

public class Bouncer_Goal : MonoBehaviour
{

    [SerializeField] private Bouncer bouncer;
    [SerializeField] private float minhitStep = 0.2f;
    private float lastHitTime = 0f;

    void Awake()
    {
        bouncer = GetComponent<Bouncer>();
        bouncer.onBounce += BounceHandle;
        lastHitTime = Time.time;
    }
    void BounceHandle(BounceBall bounceBall)
    {
        if (Time.time - lastHitTime < minhitStep)
        {
            return;
        }
        lastHitTime = Time.time;
        EventHandler.Call_OnHitGoal();
    }
}