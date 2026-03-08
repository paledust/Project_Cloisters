using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhimsicalTextSpoter : MonoBehaviour
{
    [SerializeField] private LayerMask spotLayer;
    private WhimsicalText_SpotDetection currentSpot;

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(transform.position, -transform.forward);
        if(Physics.Raycast(ray, out var hit, 100, spotLayer))
        {
            var spot = hit.collider.GetComponent<WhimsicalText_SpotDetection>();
            if(spot!=null)
            {
                if(currentSpot!=null)
                {
                    if(currentSpot!=spot)
                        ClearCurrentSpot();
                }
                else
                {
                    currentSpot = spot;
                    currentSpot.OnDetected();
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
}
