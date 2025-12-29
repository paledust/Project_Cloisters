using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Narrative : IC_Basic
{
    [System.Serializable]
    public struct HeroCircleTransistor{
        public SpriteRenderer heroCircleSprite;
        public Color transitionColor;
    }
    [SerializeField] private RippleParticleController rippleParticleController;
    [SerializeField] private NarrativeCircleSpawner circleSpawner;
    [SerializeField] private ParticleSystem p_collideBurst;
    [SerializeField] private float effectiveCollisionStep = 3;
[Header("Interaction Start")]
    [SerializeField] private Transform spawnPointAtStart;
[Header("End")]
    [SerializeField] private float transition = 10;
    [SerializeField] private float collisionStrength = 0.1f;
    [SerializeField] private PlayableDirector TL_End;
[Header("Hero Circle Control")]
    [SerializeField] private HeroCircleTransistor[] heroCircleSprites;

    private float lastCollisionTime;

    protected override void LoadAssets()
    {
        base.LoadAssets();
        rippleParticleController.enabled = true;
        circleSpawner.enabled = true;
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
        lastCollisionTime = Time.time - effectiveCollisionStep;
        circleSpawner.SpawnAtPoint(spawnPointAtStart.position, 5f, NarrativeCircleSpawner.SpawnStyle.FloatUp);
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        circleSpawner.enabled = false;
        EventHandler.E_OnClickableCircleCollide -= OnCircleCollide;
    }
    void OnCircleCollide(Clickable_Circle collidedCircle, Clickable_Circle controlledCircle, Collision collision){
        float strength = collision.relativeVelocity.magnitude;
        if(strength >= collisionStrength){
            //Play Collision Particle
            Vector3 diff = controlledCircle.transform.position - collidedCircle.transform.position;
            p_collideBurst.transform.position = collision.contacts[0].point;
            p_collideBurst.transform.rotation = Quaternion.Euler(0,0,Vector3.SignedAngle(Vector3.right, diff, Vector3.forward));
            p_collideBurst.Play(true);
            //Check if collision too frequent
            if(Time.time - lastCollisionTime<=effectiveCollisionStep) 
                return;
            lastCollisionTime = Time.time;
            //Stop Input and bounce off the circle
            EventHandler.Call_OnFlushInput();
            Vector3 force = collision.impulse.normalized * 12;
            controlledCircle.m_rigid.velocity = -force;
            //bounce off the other cirlce
            var collidableCircle = collidedCircle.GetComponent<CollidableCircle>();
            if(collidableCircle!=null)
            {
                collidableCircle.OnCollideWithControlledCircle(controlledCircle, collision.contacts[0].point, strength);
                if(collidableCircle.m_hasCollided)
                    return;
                //Spawn Circle
                int spawnAmount = Random.Range(1, 3);
                float spawnAngle = Mathf.Sign(Random.value) * Random.Range(10, 35f);

                switch(collidedCircle.m_circleType)
                {
                    case Clickable_Circle.CircleType.Controlled:
                        return;
                    case Clickable_Circle.CircleType.Normal:
                        bool hasTarget = false;
                        for(int i=0; i<spawnAmount; i++)
                        {
                            var circle = PopupCircleAtPosAndPushedAway(collidedCircle.transform.position-Quaternion.Euler(0,0,spawnAngle-spawnAngle*i)*diff.normalized*0.2f,
                                collidedCircle.transform.position);
                            if(!hasTarget && Random.value>0.5f)
                            {
                                circle.m_circle.ChangeCircleType(Clickable_Circle.CircleType.Target);
                                hasTarget = true;
                            }
                        }
                        break;
                    case Clickable_Circle.CircleType.Target:
                    //Precreate one circle that has text in there
                        var narrativeCircle = PopupCircleAtPosAndPushedAway(collidedCircle.transform.position-Quaternion.Euler(0,0,spawnAngle)*diff.normalized*0.2f,
                                                                            collidedCircle.transform.position);
                        narrativeCircle.m_circle.ChangeCircleType(Clickable_Circle.CircleType.Narrative);
                        narrativeCircle.ShowText();
                                                                                            
                        for(int i=1; i<spawnAmount; i++)
                        {
                            PopupCircleAtPosAndPushedAway(collidedCircle.transform.position-Quaternion.Euler(0,0,spawnAngle-spawnAngle*i)*diff.normalized*0.2f,
                                collidedCircle.transform.position);
                        }
                        break;
                    case Clickable_Circle.CircleType.Narrative:
                        //No spawn
                        return;
                }
            }

        }
    }
    protected CollidableCircle PopupCircleAtPosAndPushedAway(Vector3 Pos, Vector3 sourcePos)
    {
        var circle = circleSpawner.SpawnAtPoint(Pos, Random.Range(0.8f, 1.2f), NarrativeCircleSpawner.SpawnStyle.PopUp);
        Vector3 dir = (circle.transform.position - sourcePos).normalized;
        circle.m_rigidbody.AddForce(dir * Random.Range(18, 24), ForceMode.VelocityChange);
        return circle;
    }
    IEnumerator coroutineEndInteraction(){
        yield return new WaitForSeconds(transition);
        EventHandler.Call_OnEndInteraction(this);
        yield return new WaitForSeconds(2f);
        TL_End.Play();
        yield return new WaitForSeconds(1f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
    IEnumerator coroutineTransitionCircle(HeroCircleTransistor circleTransistor, float duration){
        Color initColor = circleTransistor.heroCircleSprite.color;
        yield return new WaitForLoop(duration, (t)=>{
            circleTransistor.heroCircleSprite.color = Color.Lerp(initColor, circleTransistor.transitionColor, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
}