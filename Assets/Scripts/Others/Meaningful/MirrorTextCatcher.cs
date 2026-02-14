using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorTextCatcher : MonoBehaviour
{
    [SerializeField] private Transform rootTrans;
    [SerializeField] private float castRadius;
    [SerializeField] private float showTextDistance = 1;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private MirrorTextShowController mirrorTextShowConstroller;

    private MirrorText currentText;
    private Ray ray;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        Vector3 viewDir = rootTrans.position - mainCam.transform.position;
        Vector3 refDir = GetReflectDir(viewDir);
        
        ray.origin = rootTrans.position;
        ray.direction = refDir;
        if(Physics.SphereCast(ray, castRadius, out RaycastHit hit, 100, layerMask))
        {
            var mirrorText = hit.collider.GetComponent<MirrorText>();
            if(mirrorText != null)
            {
                if(currentText!=mirrorText)
                {
                    TryClearCurrentText();
                    currentText = mirrorText;

                    mirrorTextShowConstroller.ShowMirrorText(currentText.TextChar);
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
        
        if(currentText!=null)
        {
            Vector3 dir = GetReflectDir(rootTrans.position-currentText.transform.position);
            Vector3 up  = GetReflectDir(currentText.transform.up);

            if(currentText.TryFocusMirrorText(hit.point))
            {
                mirrorTextShowConstroller.FocusText();
            }
            else
            {
                mirrorTextShowConstroller.UnfocusText();
            }
            
            mirrorTextShowConstroller.TintText(currentText.m_focusFactor);
            mirrorTextShowConstroller.PlaceText(rootTrans.position + dir * showTextDistance, Quaternion.LookRotation(-GetReflectDir(currentText.transform.forward), up));
        }
    }
    void TryClearCurrentText()
    {
        if(currentText != null)
        {
            currentText.OnMirrorTextHide();
            currentText = null;

            mirrorTextShowConstroller.UnfocusText();
            mirrorTextShowConstroller.HideMirrorText();
        }        
    }
    //InDirection pointing to mirror, output direction point outside of mirror
    Vector3 GetReflectDir(in Vector3 inDir)
    {
        Vector3 n = Vector3.Dot(rootTrans.forward, inDir) * rootTrans.forward;
        return inDir - n*2;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction);
    }
}
