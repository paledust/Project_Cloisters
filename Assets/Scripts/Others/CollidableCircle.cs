using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableCircle : MonoBehaviour
{
    [System.Serializable]
    public struct ResizableTrans{
        public Transform circleTrans;
        public float classOneSize;
        public float classTwoSize;
        public void ResetSize(int circleClass){
            switch(circleClass){
                case 1:
                    circleTrans.localScale = Vector3.one*classOneSize;
                    break;
                case 2:
                    circleTrans.localScale = Vector3.one*classTwoSize;
                    break;
            }
        }
    }
    [SerializeField] private SpriteRenderer m_bigCircleRenderer;
    [SerializeField] private SphereCollider m_collider;
    [SerializeField] private Transform renderRoot;
    [SerializeField] private Clickable_Circle m_circle;
    [SerializeField] private Rigidbody m_rigid;
    [SerializeField] private Animation circleAnime;
[Header("Float")]
    [SerializeField] private Transform circleRoot;
    [SerializeField] private float noiseFreq = 2f;
    [SerializeField] private float noiseAmp = 0.1f;
    [SerializeField] private PerRendererOpacity[] circleOpacity;
[Header("Grow")]
    [SerializeField] private float growCollisionStrength = 2;
[Header("Reset Size")]
    [SerializeField] private ResizableTrans[] resetCircles;
    public bool Collidable{get{return m_collider.enabled;}}
    public bool IsGrowing{get; private set;} = false;
    public bool IsVisible{get{return m_bigCircleRenderer.isVisible;}}

    private const string GrowClassTwoClip = "CircleGrow_Class_2";
    private const string GrowClassThreeClip = "CircleGrow_Class_3";
    private const string CircleFloat = "CircleFloat";

    void OnCollisionEnter(Collision other){
        float strength = other.rigidbody.velocity.magnitude;

        if(strength<growCollisionStrength) return;
        var otherCircle = other.gameObject.GetComponent<Clickable_Circle>();
        if(otherCircle.IsGrownCircle){
            if(!IsGrowing){
                IsGrowing = true;
                switch(m_circle.m_circleClass){
                    case 1:
                        circleAnime.Play(GrowClassTwoClip);
                        break;
                    case 2:
                        circleAnime.Play(GrowClassThreeClip);
                        break;
                }
            }
        }
    }
    public void ResetSize(float size){
        renderRoot.transform.localScale = Vector3.one * size;
        m_collider.radius = 0.16f*size;
    }
    public void ResetMotion(){
        m_rigid.velocity = Vector3.zero;
    }
    public void ResetGrowingAndWobble(){
        for(int i=0; i<circleOpacity.Length; i++)
            circleOpacity[i].opacity = 0;

        m_circle.ResetWobble();
        IsGrowing = false;
        int currentClass = m_circle.m_circleClass;
        for(int i=0; i<resetCircles.Length; i++){
            resetCircles[i].ResetSize(currentClass);
        }
    }
    public void FloatUp(float duration){
        StartCoroutine(coroutineFloatingUp(duration));
    }
    public void AE_EnableHitbox(){
        float size = renderRoot.transform.localScale.x;
        StartCoroutine(coroutineGrowHitbox(2f, size));
    }
    public void AE_GrowingDone(){
        IsGrowing = false;
        int circleClass = m_circle.IncreaseCircleClass();

        switch(circleClass){
            case 2:
                m_rigid.mass = 3;
                m_rigid.drag = 6;
                break;
            case 3:
                m_rigid.mass = 8;
                m_rigid.drag = 8;
                m_circle.enabled = true;
                m_circle.EnableRaycast();

                break;
        }
    }
    IEnumerator coroutineGrowHitbox(float duration, float scaleFactor){
        m_collider.radius = 0;
        m_circle.EnableHitbox();
        yield return new WaitForLoop(duration, (t)=>{
            m_collider.radius = Mathf.Lerp(0, 0.16f*scaleFactor, t);
        });
    }
    IEnumerator coroutineFloatingUp(float duration){
        Vector3 circlePos = Vector3.zero;
        Vector2 seed = Random.insideUnitCircle;
        circleAnime[CircleFloat].speed = circleAnime[CircleFloat].length/duration;
        circleAnime.Play(CircleFloat);
        IsGrowing = true;
        yield return new WaitForLoop(duration, (t)=>{
            circlePos.x = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, seed.x)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circlePos.y = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, 0.12345f+seed.y)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circleRoot.localPosition = circlePos;
        });
        IsGrowing = false;
    }
}
