using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Experimental : IC_Basic
{
[Header("Stage")]
    [SerializeField] private ExperimentalStageBasic[] stages;
[Header("Geo Info")]
    [SerializeField] private ConnectBody[] connectBodies;
    [SerializeField] private Clickable_CollectingText[] draggableText;

    private int stageIndex = 0;
    private List<ConnectBody> activeBodies;
    [SerializeField] private List<Clickable_ConnectionBreaker> connections;


    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        connections = new List<Clickable_ConnectionBreaker>();
        activeBodies = new List<ConnectBody>(FindObjectsOfType<ConnectBody>(false));
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
        if(connections.Contains(connectionBreaker))
        {
            connections.Remove(connectionBreaker);
        }
    }
    void OnBuildConnectionBreaker(Clickable_ConnectionBreaker connectionBreaker)
    {
        if(!connections.Contains(connectionBreaker))
        {
            connections.Add(connectionBreaker);
        }
        //connection number bigger than active geometry-1
        if(connections.Count >= activeBodies.Count-1)
        {
            if(stages[stageIndex].IsDone())
            {
                EventHandler.Call_OnTransitionBegin();
                stages[stageIndex].CompleteStage();
            }
        }
    }
    void OnCollectionText(char collectKey)
    {

    }
    public void BreakConnectionAndPopText()=>StartCoroutine(coroutineBreakConnectionAndPopText());
    IEnumerator coroutineBreakConnectionAndPopText()
    {
        for(int i=connections.Count-1; i>=0; i--)
        {
            connections[i].OnClick(null, connections[i].transform.position);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
        EventHandler.Call_OnTransitionEnd();
    }
}