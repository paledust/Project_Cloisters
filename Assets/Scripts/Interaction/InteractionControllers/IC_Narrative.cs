using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Narrative : IC_Basic
{
    [System.Serializable]
    public struct HeroCircleTransistor{
        public SpriteRenderer heroCircleSprite;
        public Color transitionColor;
    }
    [System.Serializable]
    public class NarrativeTextData{
        public char content;
        public NarrativeText textMesh;
        public void ShowText()
        {
            textMesh.gameObject.SetActive(true);
            textMesh.FadeInText(content.ToString());
        }
    }
    [SerializeField] private RippleParticleController rippleParticleController;
    [SerializeField] private NarrativeCircleManager circleManager;
    [SerializeField] private ParticleSystem p_collideBurst;
    [SerializeField] private ParticleSystem p_explode;
    [SerializeField] private float effectiveCollisionStep = 3;
[Header("Interaction Start")]
    [SerializeField] private Transform[] spawnPointAtStart;
    [SerializeField] private NarrativeTextData[] narrativeTextDatas;
[Header("End")]
    [SerializeField] private float collisionStrength = 0.1f;
    [SerializeField] private NarrativeText finalNarrativeTextData;
    [SerializeField] private PlayableDirector TL_End;
    [SerializeField] private Transform centerTrans;
    [SerializeField] private CinemachineVirtualCamera vc_transist;
[Header("Text Generate")]
    [SerializeField] private Vector2Int TextAmount;
[Header("Connection")]
    [SerializeField] private NarrativeConnectLineController connectLineController;

    private int narrativeCharIndex = 0;
    private float lastCollisionTime;

    protected override void LoadAssets()
    {
        base.LoadAssets();
        rippleParticleController.enabled = true;
        circleManager.enabled = true;
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        rippleParticleController.enabled = false;
    }
    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        EventHandler.E_OnClickableCircleCollide += OnCircleCollide;
        EventHandler.E_OnNarrativeExplode += OnNarrativeCircleExplode;
        lastCollisionTime = Time.time - effectiveCollisionStep;
        foreach(var spawnPoint in spawnPointAtStart)
        {
            circleManager.SpawnAtPoint(spawnPoint.position, Random.Range(3f, 4f), NarrativeCircleManager.SpawnStyle.FloatUp);
        }
        narrativeCharIndex = 0;
        Service.Shuffle(ref narrativeTextDatas);
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        circleManager.enabled = false;
        EventHandler.E_OnClickableCircleCollide -= OnCircleCollide;
        EventHandler.E_OnNarrativeExplode -= OnNarrativeCircleExplode;
    }
    void OnNarrativeCircleExplode(CollidableCircle circle)
    {
        PlayExplodeParticleAtPos(circle.transform.position);

        var collider = Physics.OverlapSphere(circle.transform.position, 3f);
        foreach(var col in collider)
        {
            var narrativeCircle = col.GetComponent<CollidableCircle>();
            if(narrativeCircle!=null && narrativeCircle!=circle && narrativeCircle.m_circle.m_circleType == Clickable_Circle.CircleType.Narrative)
            {
                narrativeCircle.ExplodeCircle();
            }
        }

        if(narrativeCharIndex>=narrativeTextDatas.Length)
            StartCoroutine(coroutineEndInteraction());
    }
    void PlayExplodeParticleAtPos(Vector3 position)
    {
        p_explode.transform.position = position;
        p_explode.Play();
    }
    void OnCircleCollide(Clickable_Circle collidedCircle, Clickable_Circle controlledCircle, Collision collision){
        float strength = collision.relativeVelocity.magnitude;
        //Check if collision too frequent
        if(Time.time - lastCollisionTime<=effectiveCollisionStep) 
            return;
        if(strength >= collisionStrength){
            //bounce off the other cirlce
            var collidableCircle = collidedCircle.GetComponent<CollidableCircle>();
            if(collidableCircle!=null)
            {
                if(collidableCircle.m_hasCollided)
                    return;
                //Play Collision Particle
                Vector3 diff = controlledCircle.transform.position - collidedCircle.transform.position;
                p_collideBurst.transform.position = collision.contacts[0].point;
                p_collideBurst.transform.rotation = Quaternion.Euler(0,0,Vector3.SignedAngle(Vector3.right, diff, Vector3.forward));
                p_collideBurst.Play(true);
                lastCollisionTime = Time.time;
                //Stop Input and bounce off the circle
                EventHandler.Call_OnFlushInput();
                Vector3 force = collision.impulse.normalized * 5;
                controlledCircle.m_rigid.velocity = -force;
                collidableCircle.OnCollideWithControlledCircle(controlledCircle, collision.contacts[0].point, strength);
                //Spawn Circle
                //Don't spawn if no text needed
                if(narrativeCharIndex >= narrativeTextDatas.Length)
                {
                    return;
                }
                int spawnAmount = Random.Range(1, 3);
                float spawnAngle = Mathf.Sign(Random.value) * Random.Range(10, 35f);
                //Modify Circle type
                switch(collidedCircle.m_circleType)
                {
                    case Clickable_Circle.CircleType.Controlled:
                        return;
                    case Clickable_Circle.CircleType.Normal:
                        bool hasTarget = false;
                        for(int i=0; i<spawnAmount; i++)
                        {
                            var circle = PopupCircleAtPosAndPushedAway(collidedCircle.transform.position-Quaternion.Euler(0,0,spawnAngle-spawnAngle*i)*diff.normalized,
                                collidedCircle.transform.position);
                            if(!hasTarget && Random.value>0.3f)
                            {
                                circle.m_circle.ChangeCircleType(Clickable_Circle.CircleType.Target);
                                hasTarget = true;
                            }
                        }
                        break;
                    case Clickable_Circle.CircleType.Target:
                    //Reroll the spawn amount
                        spawnAmount = TextAmount.GetRndValueInVector2Range();
                    //Precreate one circle that has text in there
                        for(int i=0; i<spawnAmount; i++)
                        {
                            NarrativeTextData narrativeText = GetNextNarrativeTextData();
                            if(narrativeText==null)
                                continue;
                            var narrativeCircle = PopupCircleAtPosAndPushedAway(collidedCircle.transform.position-Quaternion.Euler(0,0,spawnAngle)*diff.normalized,
                                                                                collidedCircle.transform.position);
                            narrativeCircle.m_circle.ChangeCircleType(Clickable_Circle.CircleType.Narrative);
                            narrativeCircle.ShowText(narrativeText.content);
                            narrativeCircle.OnExplode(narrativeText.ShowText);
                        }

                        PopupCircleAtPosAndPushedAway(collidedCircle.transform.position-Quaternion.Euler(0,0,Random.Range(-spawnAngle, spawnAngle))*diff.normalized,
                            collidedCircle.transform.position);
                        break;
                    case Clickable_Circle.CircleType.Narrative:
                        //No spawn
                        return;
                }
                //Create Connection Line
                connectLineController.BuildConnectLine(controlledCircle, collidedCircle);
            }
        }
    }
    protected CollidableCircle PopupCircleAtPosAndPushedAway(Vector3 Pos, Vector3 sourcePos)
    {
        var circle = circleManager.SpawnAtPoint(Pos, Random.Range(0.8f, 1.2f), NarrativeCircleManager.SpawnStyle.PopUp);
        Vector3 dir = (circle.transform.position - sourcePos).normalized;
        circle.m_rigidbody.AddForce(dir * Random.Range(12, 20), ForceMode.VelocityChange);
        return circle;
    }
    public NarrativeTextData GetNextNarrativeTextData()
    {
        if(narrativeCharIndex>=narrativeTextDatas.Length)
            return null;
        else
        {
            var narrativeText = narrativeTextDatas[narrativeCharIndex];
            narrativeCharIndex ++;
            return narrativeText;
        }
    }
    IEnumerator coroutineEndInteraction(){
        EventHandler.Call_OnEndInteraction(this);
        //Fade In Text
        finalNarrativeTextData.gameObject.SetActive(true);
        finalNarrativeTextData.FadeInText("Narrative");

        yield return new WaitForSeconds(2f);
        //Do Circle Sequence
        var circles = circleManager.GetCircleInDistanceOrder(centerTrans.position);
        //Set Up Transition Camera
        vc_transist.m_Follow = circles[0].transform;
        //Explode Circles
        for(int i = circles.Length-1; i >= 1; i--)
        {
            if(!circles[i].gameObject.activeSelf)
                continue;
            PlayExplodeParticleAtPos(circles[i].transform.position);
            circles[i].ExplodeCircle();
            connectLineController.CheckConnectLine(circles[i].transform);
            yield return new WaitForSeconds(Random.Range(0, 0.2f));
        }
        circles[0].m_circle.TransitionCircles(1);
        
        TL_End.Play();
        yield return new WaitForSeconds(2f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Test End")]
    public void Debug_End()
    {
        StartCoroutine(coroutineEndInteraction());
    }
#endif
}