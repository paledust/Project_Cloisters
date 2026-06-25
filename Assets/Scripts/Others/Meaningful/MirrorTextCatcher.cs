using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;

public class MirrorTextCatcher : MonoBehaviour
{
    [SerializeField] private Transform mirrorTrans;
    [SerializeField] private Transform rootTrans;
    [SerializeField] private float castRadius;
    [SerializeField] private float showTextDistance = 1;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private MirrorTextShowController mirrorTextShowConstroller;

    [Header("VFX")]
    [SerializeField] private ParticleSystem vfxSparkle;
    [SerializeField] private float bounceScale = 0.05f;
    [SerializeField] private float bounceDuration = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource catcherAudio;
    [SerializeField] private string sfxHint;
    [SerializeField, Range(0, 1)] private float hintVolume;
    [SerializeField] private string sfxCharge;
    [SerializeField, Range(0, 1)] private float minChargeVolume;
    [SerializeField, Range(0, 1)] private float maxChargeVolume;
    [SerializeField] private string sfxSpot;
    [SerializeField] private float focusVolumeLerpSpeed = 5f;
    [SerializeField] private float focusVolumeFadeLerp = 1f;

    private MirrorText currentText;
    private Camera mainCam;
    private Ray ray;
    private float hintAudioTime;

    [SerializeField, ShowOnly] private float mirrorTimeScale = 1f;

    void Start()
    {
        mainCam = Camera.main;
        EventHandler.E_OnMirrorText += OnTextFound;
        EventHandler.E_OnMirrorTextPop += OnTextPop;
    }
    void OnDestroy()
    {
        EventHandler.E_OnMirrorText -= OnTextFound;
        EventHandler.E_OnMirrorTextPop -= OnTextPop;
    }
    void Update()
    {
        Vector3 viewDir = mirrorTrans.position - mainCam.transform.position;
        Vector3 refDir = GetReflectDir(viewDir);
        
        ray.origin = mirrorTrans.position;
        ray.direction = refDir;
        if(Physics.SphereCast(ray, castRadius, out RaycastHit hit, 100, layerMask))
        {
            var mirrorText = hit.collider.GetComponent<MirrorText>();
            if(mirrorText != null)
            {
                if(!vfxSparkle.isPlaying)
                    vfxSparkle.Play();
                if(currentText!=mirrorText)
                {
                    if(currentText == null)
                    {
                        rootTrans.DOPunchScale(Vector3.one*bounceScale, bounceDuration, 1);
                        AudioManager.Instance.PlaySFX(sfxHint, hintVolume);
                    }

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

        if(mirrorTimeScale < 1f)
        {
            mirrorTimeScale += Time.deltaTime*0.25f;
            if(mirrorTimeScale > 1f)
                mirrorTimeScale = 1f;
        }
        
        bool catchedText = currentText!=null;
        if(catchedText)
        {
            Vector3 dir = GetReflectDir(mirrorTrans.position-currentText.transform.position);
            Vector3 up  = GetReflectDir(currentText.transform.up);

            if(currentText.TryFocusMirrorText(hit.point, mirrorTimeScale))
                mirrorTextShowConstroller.FocusText();
            else
                mirrorTextShowConstroller.UnfocusText();
            
            mirrorTextShowConstroller.TintText(currentText.m_focusDist);
            mirrorTextShowConstroller.PlaceText(mirrorTrans.position + dir * showTextDistance, Quaternion.LookRotation(-GetReflectDir(currentText.transform.forward), up));
        }

        //Audio Control
        if(catchedText)
        {
            if(!catcherAudio.isPlaying)
                AudioManager.Instance.PlaySFXLoop(catcherAudio, sfxCharge, 0, 0, hintAudioTime);
            
            float volume = Mathf.Lerp(minChargeVolume, maxChargeVolume, currentText.m_focusFactor);
            catcherAudio.volume = Mathf.Lerp(catcherAudio.volume, volume, Time.deltaTime*focusVolumeLerpSpeed);
        }
        if(!catchedText && catcherAudio.isPlaying)
        {
            catcherAudio.volume = Mathf.Lerp(catcherAudio.volume, 0, Time.deltaTime * focusVolumeFadeLerp);
            if(catcherAudio.volume < 0.001f)
            {
                catcherAudio.volume = 0;
                catcherAudio.Stop();
            }
        }
    }
    void OnTextFound(MirrorText mirrorText) => mirrorTimeScale = 0.25f;
    void OnTextPop(MirrorText mirrorText) => AudioManager.Instance.PlaySFX(sfxSpot, 1);
    void TryClearCurrentText()
    {
        if(currentText != null)
        {
            vfxSparkle.Stop();
            currentText.OnMirrorTextHide();
            currentText = null;

            mirrorTextShowConstroller.UnfocusText();
            mirrorTextShowConstroller.HideMirrorText();
        }        
    }
    //InDirection pointing to mirror, output direction point outside of mirror
    Vector3 GetReflectDir(in Vector3 inDir)
    {
        Vector3 n = Vector3.Dot(mirrorTrans.forward, inDir) * mirrorTrans.forward;
        return inDir - n*2;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction);
    }
}
