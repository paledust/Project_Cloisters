using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableCircle : MonoBehaviour
{
    public enum CollidableCircleState{Floating, Idle}
    [SerializeField, ShowOnly] 
    private CollidableCircleState state = CollidableCircleState.Floating;
    [SerializeField] private SpriteRenderer m_bigCircleRenderer;
    [SerializeField] private SphereCollider m_collider;
    [SerializeField] private Transform circleRoot;
    [SerializeField] private Clickable_Circle m_circle;
    [SerializeField] private Rigidbody m_rigid;
    [SerializeField] private Animation growAnim;
    [SerializeField] private float growCollisionStrength = 2;

    public bool Collidable{get{return m_collider.enabled;}}
    public bool IsGrowing{get; private set;} = false;
    public bool IsVisible{get{return m_bigCircleRenderer.isVisible;}}

    private const string GrowClassTwoClip = "CircleGrow_Class_2";
    private const string GrowClassThreeClip = "CircleGrow_Class_3";
    private const string ClassOneIdle = "CircleIdle_Class_1";
    private const string ClassTwoIdle = "CircleIdle_Class_2";

    void OnCollisionEnter(Collision other){
        float strength = other.rigidbody.velocity.magnitude;

        if(strength<growCollisionStrength) return;
        var otherCircle = other.gameObject.GetComponent<Clickable_Circle>();
        if(otherCircle.IsGrownCircle){
            state = CollidableCircleState.Idle;
            if(!IsGrowing){
                IsGrowing = true;
                switch(m_circle.m_circleClass){
                    case 1:
                        growAnim.Play(GrowClassTwoClip);
                        break;
                    case 2:
                        growAnim.Play(GrowClassThreeClip);
                        break;
                }
            }
        }
    }
    void Update(){
        if(IsVisible && !m_collider.enabled){
            m_collider.enabled = true;
        }
    }
    public void ResetSize(float size){
        circleRoot.transform.localScale = Vector3.one * size;
        m_collider.radius = 0.16f*size;
    }
    public void ResetMotion(float floatingForce){
        state = CollidableCircleState.Floating;
        m_rigid.velocity = Vector3.zero;
    }
    public void ResetGrowingAndWobble(){
        m_circle.ResetWobble();
        IsGrowing = false;
        int currentClass = m_circle.m_circleClass;
        switch(currentClass){
            case 1:
                growAnim.Play(ClassOneIdle);
                break;
            case 2:
                growAnim.Play(ClassTwoIdle);
                break;
        }
    }
    public void AE_GrowingDone(){
        IsGrowing = false;
        int circleClass = m_circle.IncreaseCircleClass();

        switch(circleClass){
            case 2:
                m_rigid.mass = 3;
                break;
            case 3:
                m_rigid.mass = 8;
                m_circle.enabled = true;
                m_circle.EnableRaycast();
                Destroy(growAnim);

                break;
        }
    }
}
