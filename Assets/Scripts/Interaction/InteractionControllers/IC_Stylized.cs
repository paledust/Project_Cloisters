using System.Collections;
using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Stylized : IC_Basic
{
    public enum StylizedState
    {
        //Drag circle to explode
        Intro,
        //Knock circle to explode
        Drum,
        //Prepare to the next drum expand
        Extending,
    }
    [SerializeField, ShowOnly] private StylizedState stylizedState;

    [Header("Script Control")]
    [SerializeField] private Clickable_ObjectRotator clickablePlanet;
    [SerializeField] private CircleExpandingController circleExpandingController;
    [SerializeField] private CircleExplodeController circleExplodeController;
    [SerializeField] private StylizedDrumController drumController;
    [SerializeField] private GeoFragmentController geoFragmentController;
    [SerializeField] private GeoTextController geoTextController;
    [SerializeField] private PlayableDirector tl_end;

    [Header("Giant Drum")]
    [SerializeField] private Hoverable_DrumInteraction giantDrum;

    [Header("Expanding Control")]
    [SerializeField] private float expandRange = 1800;
    [SerializeField] private float geoExpandDrumFactor = 0.65f;

    [Header("Drum Explode")]
    [SerializeField] private float maxDrumKnockRadius = 0.8f;

    [Header("Text Order")]
    [SerializeField] private int[] textShowOrder;

    [Header("Audio")]
    [SerializeField] private string ambKey;
    [SerializeField] private string sfxEnd;
    [SerializeField] private Vector2 ambFade = Vector2.one;
    [SerializeField] private float finalAmbVolume = 0.2f;
    [SerializeField] private AmbienceHandler ambienceHandler;

    private int textShowIndex = 0;
    private float introOffsetPlanetAngle;
    private float minAmbVolume;
    private bool isExtending = false;
    private bool transitioning = false;

    void Awake()
    {
        this.enabled = false;
    }
    protected override void LoadAssets()
    {
        base.LoadAssets();
        StartExpanding();
        introOffsetPlanetAngle = clickablePlanet.m_accumulateYaw;
        geoFragmentController.enabled = true;
        geoTextController.enabled = true;
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        circleExpandingController.enabled = false;
        circleExplodeController.enabled = false;
        geoFragmentController.enabled = false;
        geoTextController.enabled = false;
        drumController.enabled = false;
    }
    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        this.enabled = true;
        transitioning = false;
        stylizedState = StylizedState.Intro;
        drumController.enabled = true;
        clickablePlanet.EnableHitbox();

        EventHandler.E_OnDrumKnocked += DrumKnockedHandler;
        EventHandler.E_OnBassChargeBeat += BassChargeBeatHandler;

        minAmbVolume = ambFade.x;
        ambienceHandler.PlayAmbience(ambKey, 0.2f);
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        this.enabled = false;
        clickablePlanet.DisableHitbox();
        EventHandler.E_OnDrumKnocked -= DrumKnockedHandler;
        EventHandler.E_OnBassChargeBeat -= BassChargeBeatHandler;
    }
    void Update()
    {
        float introExpandFactor = 0;
        float geoExpandFactor = 0;
        switch(stylizedState)
        {
            case StylizedState.Intro:
                introExpandFactor = (-clickablePlanet.m_accumulateYaw+introOffsetPlanetAngle)/expandRange;
                geoExpandFactor = introExpandFactor;
                if(!isExtending && circleExpandingController.enabled)
                    circleExpandingController.UpdateExpand(introExpandFactor);
                break;
            case StylizedState.Drum:
                introExpandFactor = giantDrum.m_accumulatePower;
                geoExpandFactor = introExpandFactor*geoExpandDrumFactor;
                break;
        }
        if(!m_isDone)
        {
            ambienceHandler.ChangeAmbienceVolume(Mathf.Lerp(minAmbVolume, ambFade.y, EasingFunc.Easing.QuadEaseIn(introExpandFactor)));
        }
        geoFragmentController.UpdateExpand(geoExpandFactor);
    }

    #region State Change
    public void StartExtending()
    {
        stylizedState = StylizedState.Extending;
    }
    void StartDrumCharge()
    {
        if(transitioning){
            transitioning = false;
            circleExpandingController.enabled = false;
            circleExplodeController.enabled = false;
        }
        stylizedState = StylizedState.Drum;
        isExtending = false;
        StartExpanding();
    }
    public void StylizedExplode(){
        if(stylizedState == StylizedState.Drum)
        {
            int count = textShowOrder[textShowIndex];
            for(int i=0; i<count; i++){
                geoTextController.PopText();
            }
            textShowIndex++;
        }

        minAmbVolume = (ambFade.y-ambFade.x)/textShowOrder.Length * textShowIndex + ambFade.x;

        if(textShowIndex < textShowOrder.Length)
        {
            StartCoroutine(coroutineFadeAmbToInitVolume(0.2f));
            geoFragmentController.PopGeos();
        }
        else
        {
            //all text pop out, start ending sequence
            EventHandler.Call_OnFlushInput();
            EventHandler.Call_OnEndInteraction(this);
            StartCoroutine(coroutineEnd());
        }
        transitioning = true;
    }
    #endregion

    void BassChargeBeatHandler()
    {
        circleExplodeController.Explode();
    }
    void DrumKnockedHandler(float strength)
    {
        if(stylizedState == StylizedState.Extending && !isExtending)
        {
            isExtending = true;
            circleExpandingController.ExpandCircleOut(StartDrumCharge);
        }
    }
    public void StartExpanding(){
        circleExplodeController.ResetController();
        circleExpandingController.ResetController();

        circleExpandingController.enabled = true;
        circleExplodeController.enabled = true;
    }
    IEnumerator coroutineFadeAmbToInitVolume(float delay)
    {
        yield return new WaitForSeconds(delay);
        ambienceHandler.FadeAmbience(minAmbVolume, 2f);
    }
    IEnumerator coroutineEnd(){
        ambienceHandler.FadeAmbience(finalAmbVolume, 1f);
        yield return new WaitForSeconds(1.2f);
        drumController.QueueBeat(sfxEnd, 1f, geoTextController.PutTextTogether);
        yield return new WaitForSeconds(1.2f);
        geoTextController.PunchTextTogether();
        circleExplodeController.ExplodeFinal();
        yield return new WaitForSeconds(1.8f);
        ambienceHandler.FadeOutAmbience(2f, true);
        tl_end.Play();
        yield return new WaitForSeconds(3.5f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
}
