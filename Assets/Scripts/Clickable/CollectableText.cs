using DG.Tweening;
using UnityEngine;

public class CollectableText : MonoBehaviour
{
    [SerializeField] private char collectKey;
    public char m_collectKey=>collectKey;

    public void CollectText(float delay)
    {
        transform.DOPunchScale(Vector3.one*1.1f, 0.25f, 1, 1)
        .SetDelay(delay).OnComplete(()=>{
            EventHandler.Call_OnCollectExperimentalText(this);
        });
    }
}