using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IC_Experimental : IC_Basic
{
[Header("Stage")]
    [SerializeField] private ExperimentalStageBasic[] stages;

[Header("Geo Info")]
    [SerializeField] private GeoThrowPoint[] throwPoints;
    [SerializeField] private ConnectBody[] connectBodies;

[Header("Collect Text")]
    [SerializeField] private ParticleSystem p_collectText;
    [SerializeField] private Clickable_CollectingText[] draggableText;
    [SerializeField] private List<Clickable_CollectingText> completedTexts;

    private int hiddenShapeFront;
    private int collectedCount = 0;
    private int stageIndex = 0;
    private int textIndex = 0;
    private List<ConnectBody> activeBodies;
    private List<Clickable_ConnectionBreaker> builtConnections;

    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        Service.Shuffle(ref draggableText);
        Service.Shuffle(ref throwPoints);
        builtConnections = new List<Clickable_ConnectionBreaker>();
        activeBodies = new List<ConnectBody>(FindObjectsOfType<ConnectBody>(false));
        hiddenShapeFront = activeBodies.Count;
        EventHandler.E_OnCollectExperimentalText += OnCollectionText;
        EventHandler.E_OnBuildConnectionBreaker += OnBuildConnectionBreaker;
        EventHandler.E_OnBreakConnectionBreaker += OnBreakConnectionBreaker;
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        EventHandler.E_OnCollectExperimentalText -= OnCollectionText;
        EventHandler.E_OnBuildConnectionBreaker -= OnBuildConnectionBreaker;
        EventHandler.E_OnBreakConnectionBreaker -= OnBreakConnectionBreaker;
    }
    void OnBreakConnectionBreaker(Clickable_ConnectionBreaker connectionBreaker, Vector3 breakPoint)
    {
        if(builtConnections.Contains(connectionBreaker))
        {
            builtConnections.Remove(connectionBreaker);
        }
    }
    void OnBuildConnectionBreaker(Clickable_ConnectionBreaker connectionBreaker)
    {
        if(!builtConnections.Contains(connectionBreaker))
        {
            builtConnections.Add(connectionBreaker);
        }
        //connection number bigger than active geometry-1
        if(builtConnections.Count >= activeBodies.Count-1)
        {
            if(stages[stageIndex].IsDone())
            {
                EventHandler.Call_OnTransitionBegin();
                stages[stageIndex].CompleteStage();
            }
        }
    }
    void OnGUI()
    {
        if(builtConnections!=null)
        GUILayout.Label(builtConnections.Count.ToString());
    }
    void OnCollectionText(Clickable_CollectingText collectText)
    {
        p_collectText.transform.position = collectText.transform.position;
        p_collectText.Play(true);

        char c = collectText.m_collectKey;
        var showText = completedTexts.Find(x=>x.m_collectKey == c && !x.gameObject.activeSelf);
        showText.gameObject.SetActive(true);
        collectedCount ++;
        if(collectedCount >= completedTexts.Count)
        {
            Debug.LogWarning("Win!!");
        }
    }
    public void BlinkShapes()
    {
        foreach(var shape in activeBodies)
        {
            shape.BlinkShape();
        }
    }
    public void BreakConnectionAndPopText()=>StartCoroutine(coroutineBreakConnectionAndPopText());
    IEnumerator coroutineBreakConnectionAndPopText()
    {
        for(int i=builtConnections.Count-1; i>=0; i--)
        {
            //Break Connections
            var connection = builtConnections[i];
            connection.OnClick(null, connection.transform.position);

            //Pop texts
            var text = draggableText[textIndex];
            text.DisableHitbox();
            text.transform.position = connection.transform.position;
            text.transform.localScale = Vector3.one * 0.5f;
            text.gameObject.SetActive(true);
            text.transform.DOMove(text.transform.position + (Vector3)Random.insideUnitCircle*4, 0.5f).SetEase(Ease.OutQuad);
            text.transform.DORotateQuaternion(Quaternion.Euler(0,0,Random.Range(270, 480))*text.transform.rotation, 0.5f).SetEase(Ease.OutQuad);
            text.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).OnComplete(()=>text.EnableHitbox());
            textIndex ++;

            //wait for next connection break
            yield return new WaitForSeconds(Random.Range(0f, 0.2f));
        }
        yield return new WaitForSeconds(0.5f);
        yield return coroutineThrowShape();
        
        stageIndex ++;
        stages[stageIndex].EnterStage();

        EventHandler.Call_OnTransitionEnd();
    }
    IEnumerator coroutineThrowShape()
    {
        for(int i=0; i<2; i++)
        {
            //Pop shape
            Vector3 pos = throwPoints[hiddenShapeFront].transform.position;
            Vector3 dir = throwPoints[hiddenShapeFront].GetThrowDirection();
            if(hiddenShapeFront<connectBodies.Length)
            {
                var shape = connectBodies[hiddenShapeFront];
                shape.transform.position = pos;
                shape.transform.localScale = Vector3.zero;
                shape.gameObject.SetActive(true);
                shape.m_rigid.AddForce(dir*Random.Range(35, 40), ForceMode.VelocityChange);
                shape.m_rigid.AddTorque(Vector3.forward * Random.Range(-4, 4), ForceMode.VelocityChange);
                shape.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                activeBodies.Add(shape);
                hiddenShapeFront ++;
                yield return new WaitForSeconds(Random.Range(0f, 0.1f));
            }
        }
    }
}