using UnityEngine;

public class WhimsicalTextSpoter : MonoBehaviour
{
    [SerializeField] private LayerMask spotLayer;
    private WhimsicalText_SpotDetection currentSpot;

    void OnDisable()
    {
        ClearCurrentSpot();
    }
    void Update()
    {
        var ray = new Ray(transform.position, -transform.forward);
        if(Physics.Raycast(ray, out var hit, 100, spotLayer))
        {
            if(hit.collider.TryGetComponent<WhimsicalText_SpotDetection>(out var spot))
            {
                if(currentSpot!=null)
                {
                    if(currentSpot!=spot)
                        ClearCurrentSpot();
                }
                else
                {
                    currentSpot = spot;
                    currentSpot.OnDetected(this);
                }
            }
            else
            {
                ClearCurrentSpot();
            }
        }
        else
        {
            ClearCurrentSpot();
        }
    }
    void ClearCurrentSpot()
    {
        if(currentSpot!=null)
        {
            currentSpot.OnNotDetected();
            currentSpot = null;
        }
    }
    public void OnConsumed()
    {
        ClearCurrentSpot();
        Destroy(gameObject);
    }
}
