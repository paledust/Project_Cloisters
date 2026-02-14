using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorDiamondCatcher : MonoBehaviour
{
    [SerializeField] private Transform rootTrans;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float castRadius;

    private Ray ray;
    private Camera mainCam;
    private MirrorDiamond currentDiamond;

    void Start()
    {
        mainCam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 viewDir = rootTrans.position - mainCam.transform.position;
        Vector3 refDir = GetReflectDir(viewDir);
        
        ray.origin = rootTrans.position;
        ray.direction = refDir;
        if(Physics.SphereCast(ray, castRadius, out RaycastHit hit, 100, layerMask))
        {
            var diamond = hit.collider.GetComponent<MirrorDiamond>();
            if(diamond != null)
            {
                if(currentDiamond!=diamond)
                {
                    TryClearCurrentText();
                    currentDiamond = diamond;
                    currentDiamond.OnFocused();
                }
            }
            else
            {
                TryClearCurrentText();
            }
        }
        else
        {
            TryClearCurrentText();
        }
    }
    //InDirection pointing to mirror, output direction point outside of mirror
    Vector3 GetReflectDir(in Vector3 inDir)
    {
        Vector3 n = Vector3.Dot(rootTrans.forward, inDir) * rootTrans.forward;
        return inDir - n*2;
    }
    void TryClearCurrentText()
    {
        if(currentDiamond != null)
        {
            currentDiamond.OnExitFocus();
            currentDiamond = null;
        }        
    }
}
