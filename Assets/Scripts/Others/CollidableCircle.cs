using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableCircle : MonoBehaviour
{
    [SerializeField] private Clickable_Circle m_circle;
    [SerializeField] private float growCollisionStrength = 2;
    [SerializeField] private Animation growAnim;
    private const string GrowClassTwoClip = "CircleGrow_Class_2";
    private const string GrowClassThreeClip = "CircleGrow_Class_3";
    private bool isGrowing = false;
    void OnCollisionEnter(Collision other){
        float strength = other.relativeVelocity.magnitude;

        if(strength<growCollisionStrength) return;
        var otherCircle = other.gameObject.GetComponent<Clickable_Circle>();
        if(otherCircle.IsGrownCircle){
            if(!isGrowing){
                isGrowing = true;
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
    public void AE_GrowingDone(){
        isGrowing = false;
        int circleClass = m_circle.IncreaseCircleClass();

        if(circleClass == 3){
            m_circle.enabled = true;
            m_circle.EnableRaycast();
            Destroy(growAnim);
            Destroy(this);
        }
    }
}
