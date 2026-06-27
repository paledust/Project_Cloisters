using SimpleAudioSystem;
using UnityEngine;

public class MirrorDiamondCatcher : MonoBehaviour
{
    [SerializeField] private Transform rootTrans;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float castRadius;

    [Header("Audio")]
    [SerializeField] private AudioSource catcherAudio;
    [SerializeField] private string sfxCharge;
    [SerializeField, Range(0, 1)] private float minChargeVolume;
    [SerializeField, Range(0, 1)] private float maxChargeVolume;

    private Ray ray;
    private Camera mainCam;
    private MirrorDiamond currentDiamond;
    private bool catchedDiamond;
    private float catcherAudioTime;

    void Start()
    {
        mainCam = Camera.main;
        catcherAudioTime = 0;
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

        catchedDiamond = currentDiamond!=null;
        //Audio Control
        if(catchedDiamond)
        {
            if(!catcherAudio.isPlaying)
                AudioManager.Instance.PlaySFXLoop(catcherAudio, sfxCharge, 0, 0, catcherAudioTime);
            
            float volume = Mathf.Lerp(minChargeVolume, maxChargeVolume, currentDiamond.m_focusFactor);
            catcherAudio.volume = Mathf.Lerp(catcherAudio.volume, volume, Time.deltaTime*5);
        }
        if(!catchedDiamond && catcherAudio.isPlaying)
        {
            catcherAudio.volume = Mathf.Lerp(catcherAudio.volume, 0, Time.deltaTime * 2);
            if(catcherAudio.volume < 0.001f)
            {
                catcherAudio.volume = 0;
                catcherAudio.Stop();
            }
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
