using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public struct TextPopParam
{
    public char key;
    public Vector3 position;
}
public class IC_Experimental : IC_Basic
{
[Header("Stage")]
    [SerializeField] private ExperimentalStageData[] stages;
    [SerializeField] private Color blinkColor1;
    [SerializeField] private Color blinkColor2;

[Header("Constraints")]
    [SerializeField] private RangeDetection rangeDetection;
    [SerializeField] private int enlargeStage = 2;

[Header("Geo Info")]
    [SerializeField] private GeoThrowPoint[] throwPoints;
    [SerializeField] private ConnectBody[] connectBodies;

[Header("Collect Text")]
    [SerializeField] private Vector2 textCollectDelay;
    [SerializeField] private Vector2 textPopRect;
    [SerializeField] private float forbidZone;
    [SerializeField] private ParticleSystem p_collectText;
    [SerializeField] private CollectableText[] popTexts;
    [SerializeField] private List<CollectableText> completedTexts;

    private int shapeFront;
    private int stageIndex = 0;
    private int textIndex = 0;
    private List<ConnectBody> activeBodies;
    private List<Clickable_ConnectionBreaker> builtConnections;

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        Service.Shuffle(ref popTexts);
        Service.Shuffle(ref throwPoints);
        builtConnections = new List<Clickable_ConnectionBreaker>();
        activeBodies = new List<ConnectBody>(FindObjectsOfType<ConnectBody>(false));
        shapeFront = activeBodies.Count;
        stageIndex = 0;
        textIndex = 0;

        rangeDetection.RangeAppear(0.8f);
        rangeDetection.InitRangeDetect(activeBodies.Count);

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
        if (!builtConnections.Contains(connectionBreaker))
        {
            builtConnections.Add(connectionBreaker);
        }
        //All bodies are connected together
        if(builtConnections.Count >= activeBodies.Count-1)
        {
            if (rangeDetection.CheckRange())
            {
                int textAmount = stages[stageIndex].textAmount;
                int throwBodyAmount = stages[stageIndex].throwBodiesAmount;
                StartCoroutine(coroutineCompletingStage(textAmount, throwBodyAmount));
            }
            else
            {
                StartCoroutine(coroutineFailStage());
            }
        }
    }
    void OnCollectionText(CollectableText collectableText)
    {
        p_collectText.transform.position = collectableText.transform.position;
        p_collectText.Play(true);

        var key = collectableText.m_collectKey;
        Destroy(collectableText.gameObject);

        //Pop Complete Text
        var matchText = completedTexts.Find(x=>!x.gameObject.activeSelf && x.m_collectKey == key);
        float targetScale = matchText.transform.localScale.x;
        matchText.transform.localScale = Vector3.zero;
        matchText.gameObject.SetActive(true);
        matchText.transform.DOScale(Vector3.one*targetScale, 0.3f).SetEase(Ease.OutBack, 2);
    }
    IEnumerator coroutineFailStage()
    {
        foreach(var shape in activeBodies)
        {
            shape.BlinkShape(blinkColor1, blinkColor2);
        }
        yield return new WaitForSeconds(1);
        for(int i=builtConnections.Count-1; i>=0; i--)
        {
            //Break Connections
            var connection = builtConnections[i];
            connection.BreakConnection(10f, 0, connection.transform.position);
            yield return new WaitForSeconds(Random.Range(0f, 0.05f));
        }
    }
    IEnumerator coroutineCompletingStage(int textAmount, int throwBodyAmount)
    {
        EventHandler.Call_OnTransitionBegin();
        yield return new WaitForSeconds(0.4f);

        //Break Connections
        for(int i=builtConnections.Count-1; i>=0; i--)
        {
            var connection = builtConnections[i];
            connection.BreakConnection(10f, 0, connection.transform.position);
            yield return new WaitForSeconds(Random.Range(0f, 0.05f));
        }

        //Pop texts
        for(int i=0; i<textAmount; i++)
        {
            var text = popTexts[textIndex];
            Vector3 position = new Vector3(((Random.value>0.5f)?1:-1) *  Random.Range(forbidZone*0.5f, textPopRect.x*0.5f), 
                                            0.5f*Random.Range(-textPopRect.y, textPopRect.y) , 
                                            0);
            Vector2 dir = Quaternion.Euler(0,0,Random.Range(0f, 360f)) * Vector2.right;
            text.transform.localPosition = position;
            text.transform.localScale = Vector3.one * 0.5f;
            text.transform.localRotation = Quaternion.Euler(0,0,Random.Range(-20, 20));
            text.gameObject.SetActive(true);
            text.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack, 10);
            text.transform.DOLocalMove(position + (Vector3)Random.insideUnitCircle*2, 1f).SetEase(Ease.OutCirc).OnComplete(()=>
            {
                text.CollectText(textCollectDelay.GetRndValueInVector2Range());
            });
            textIndex ++;
        }

        //Throw Bodies out
        yield return new WaitForSeconds(2f);

        for(int i=0; i<throwBodyAmount; i++)
        {
            //Pop shape
            Vector3 pos = throwPoints[shapeFront].transform.position;
            Vector3 dir = throwPoints[shapeFront].GetThrowDirection();
            if(shapeFront<connectBodies.Length)
            {
                var shape = connectBodies[shapeFront];
                shape.transform.position = pos;
                shape.transform.localScale = Vector3.zero;
                shape.gameObject.SetActive(true);
                shape.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                activeBodies.Add(shape);
                shapeFront ++;
                StartCoroutine(coroutineThrowShape(shape.m_rigid, dir*Random.Range(35, 40), Random.Range(-4, 4), 0.5f, 0.5f));
            }
        }

        yield return new WaitForSeconds(0.25f);
        
        stageIndex ++;

        if(stageIndex == stages.Length)
        {
            EventHandler.Call_OnTransitionEnd();
            EventHandler.Call_OnEndInteraction(this);
        }
        else
        {
            if(stageIndex>=enlargeStage && rangeDetection.CanEnlarge())
                rangeDetection.EnlargeDetection();

            rangeDetection.InitRangeDetect(activeBodies.Count);

            EventHandler.Call_OnTransitionEnd();
        }
    }
    IEnumerator coroutineThrowShape(Rigidbody m_rigid, Vector3 maxForce, float maxTorque, float duration, float daccDuration)
    {
        Vector3 force = maxForce;
        float torque = maxTorque;
        yield return new WaitForLoop(duration, (t)=>{
            m_rigid.AddForce(force, ForceMode.Acceleration);
            m_rigid.AddTorque(Vector3.forward * torque, ForceMode.Acceleration);
        });
        yield return new WaitForLoop(daccDuration, (t)=>{
            force = Vector3.Lerp(maxForce, Vector3.zero, t);
            torque = Mathf.Lerp(maxTorque, 0, t);
            m_rigid.AddForce(force, ForceMode.Acceleration);
            m_rigid.AddTorque(Vector3.forward * torque, ForceMode.Acceleration);
        });
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,1,0,0.5f);
        Vector3 pos = Camera.main.transform.position;
        pos += Camera.main.transform.forward*interact_depth;
        Gizmos.DrawWireCube(pos, new Vector3(interact_rect.x, interact_rect.y, 0.01f));
        Gizmos.color = new Color(0,1,1,0.5f);
        Gizmos.DrawWireCube(pos, new Vector3(textPopRect.x, textPopRect.y, 0.01f));
        Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawWireCube(pos, new Vector3(forbidZone, textPopRect.y, 0.01f));
    }
}