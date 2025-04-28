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
    private int pendingTextAmount = 0;
    private List<ConnectBody> activeBodies;
    private List<Clickable_ConnectionBreaker> builtConnections;

    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        Service.Shuffle(ref popTexts);
        Service.Shuffle(ref throwPoints);
        builtConnections = new List<Clickable_ConnectionBreaker>();
        activeBodies = new List<ConnectBody>(FindObjectsOfType<ConnectBody>(false));
        shapeFront = activeBodies.Count;

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
        if(!builtConnections.Contains(connectionBreaker))
        {
            builtConnections.Add(connectionBreaker);
        }
        //All bodies are connected together
        if(builtConnections.Count >= activeBodies.Count-1)
        {
            if(rangeDetection.CheckRange())
            {
                int textAmount = stages[stageIndex].textAmount;
                pendingTextAmount += textAmount;
                StartCoroutine(coroutineCompletingStage(textAmount));
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

        Destroy(collectableText.gameObject);

        //Pop Complete Text
        Debug.Log(collectableText.m_collectKey);
        var matchText = completedTexts.Find(x=>x.m_collectKey == collectableText.m_collectKey);
        matchText.transform.localScale = Vector3.zero;
        matchText.gameObject.SetActive(true);
        matchText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack, 2);

        //Throw Shapes
        pendingTextAmount --;
        if(pendingTextAmount <= 0)
        {
            StartCoroutine(coroutineThrowNewShapes(stages[stageIndex].throwBodiesAmount));
        }
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
    IEnumerator coroutineCompletingStage(int textAmount)
    {
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
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }
    IEnumerator coroutineThrowNewShapes(int throwBodyAmount)
    {
        yield return new WaitForSeconds(0.5f);
        EventHandler.Call_OnTransitionBegin();
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
                shape.m_rigid.AddForce(dir*Random.Range(35, 40), ForceMode.VelocityChange);
                shape.m_rigid.AddTorque(Vector3.forward * Random.Range(-4, 4), ForceMode.VelocityChange);
                shape.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                activeBodies.Add(shape);
                shapeFront ++;
                yield return new WaitForSeconds(Random.Range(0.3f, 0.4f));
            }
        }

        yield return new WaitForSeconds(0.25f);
        
        stageIndex ++;
        rangeDetection.InitRangeDetect(activeBodies.Count);

        EventHandler.Call_OnTransitionEnd();
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