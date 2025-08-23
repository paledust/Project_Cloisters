using UnityEngine;

public class Bouncer_Goal : MonoBehaviour
{
    [System.Serializable]
    public struct ActiveRender
    {
        public SpriteRenderer renderRoot;
        public SpriteRenderer blinker;
        public void Deactivate()
        {
            renderRoot.gameObject.SetActive(false);
            blinker.gameObject.SetActive(false);
        }
    }
    [SerializeField] private ActiveRender[] activeRenders;
    [SerializeField] private Bouncer bouncer;

    private int activeIndex = 0;
    private ActiveRender currentRender;

    void Awake()
    {
        bouncer = GetComponent<Bouncer>();
        bouncer.onBounce += BounceHandle;
        currentRender = activeRenders[activeIndex];
    }
    void BounceHandle(BounceBall bounceBall)
    {
        if (activeIndex >= activeRenders.Length - 1)
        {
            return;
        }
        currentRender.Deactivate();
        activeIndex++;
        currentRender = activeRenders[activeIndex];
        SwitchRender(activeIndex);

    }
    void SwitchRender(int index)
    {
        bouncer.SwapRender(activeRenders[index].renderRoot, activeRenders[index].blinker);
    }
}