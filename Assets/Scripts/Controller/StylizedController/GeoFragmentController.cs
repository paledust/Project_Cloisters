using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GeoFragmentController : MonoBehaviour
{
    public struct RayTrail{
        public Ray ray;
        public float outerDistance;
        public float innerDistance;
        public RayTrail(Ray ray, float outerDistance, float innerDistance){
            this.ray = ray;
            this.outerDistance = outerDistance;
            this.innerDistance = innerDistance;
        }
        public Vector3 GetPos(float normalizedTime){
            float dist = normalizedTime>=0?outerDistance:innerDistance;
            return ray.origin + ray.direction*(normalizedTime*dist);
        }
    }
    public enum GeoControlState{Expand, Dissolve}
    [SerializeField, ShowOnly] private GeoControlState state;
    [SerializeField] private Clickable_Planet clickablePlanet;
    [SerializeField] private Transform center;
    [SerializeField] private GameObject[] geoFrags;
[Header("Expand")]
    [SerializeField] private float expandRadius = 1800;
    [SerializeField] private float expandPosAngleRND;
    [SerializeField] private Vector2 expandRadiusRange;
    [SerializeField] private Vector2 expandPosRatioRND;
    [SerializeField] private float finalExpandFactor = 1.5f;
    [SerializeField] private CurveData geoSizeCurve; 
[Header("Dissolve")]
    [SerializeField] private Vector2 RayTrailRadiusRange;
    [SerializeField] private float dissolveRadius;

    private float expandFactor = 0;
    private float dissolveFactor = 0;
    private float offsetAngle = 0;
    private int[] geoIndexShuffles;
    private RayTrail[] geoTrails;
    private Vector3[] geoPoses;
    private Vector3[] geoScales;
    
    void Awake(){
        geoIndexShuffles = new int[geoFrags.Length];
        for(int i=0; i<geoIndexShuffles.Length; i++){
            geoIndexShuffles[i] = i;
        }
    }
    void Update(){
        switch(state){
            case GeoControlState.Expand:
                expandFactor = (-clickablePlanet.m_accumulateYaw+offsetAngle)/expandRadius;
            //Rotate Geo and expand along the shape
                for(int i=0; i<geoFrags.Length; i++){
                    geoFrags[i].transform.localPosition = geoPoses[i] * Mathf.Lerp(expandRadiusRange.x, expandRadiusRange.y, expandFactor);
                    geoFrags[i].transform.localScale = geoScales[i] * geoSizeCurve.Evaluate(expandFactor);
                }
                break;
            case GeoControlState.Dissolve:
                dissolveFactor = (-clickablePlanet.m_accumulateYaw+offsetAngle)/dissolveRadius;
            //Move geo along the trail
                for(int i=0; i<geoFrags.Length; i++){
                    geoFrags[i].transform.localPosition = geoTrails[i].GetPos(dissolveFactor);
                }
                break;
        }
    }
    public void StartExpand(){
        Service.Shuffle(ref geoIndexShuffles);
        offsetAngle = clickablePlanet.m_accumulateYaw;
    //Regenerate certain amount of geos
        expandFactor = 0;
    //Reposition geos to center of the sphere
        geoPoses = new Vector3[geoFrags.Length];
        geoScales = new Vector3[geoFrags.Length];
        for(int i=0; i<geoPoses.Length; i++){
            geoPoses[geoIndexShuffles[i]] = Quaternion.Euler(0, 0, Random.Range(-expandPosAngleRND, expandPosAngleRND)+360*i/geoPoses.Length) * Vector2.right * expandPosRatioRND.GetRndValueInVector2Range();
            geoScales[i] = geoFrags[i].transform.localScale;
        }
        state = GeoControlState.Expand;
    }
    public void StartTransition(){
        StartCoroutine(coroutineExplodeToDissolve(1f, finalExpandFactor));
    }
    public void StartDissolving(){
        this.enabled = true;

        geoTrails = new RayTrail[geoFrags.Length];
        for(int i=0; i<geoTrails.Length; i++){
            Vector3 geoPos = geoFrags[i].transform.localPosition;
            geoTrails[i] = new RayTrail(new Ray(geoPos, geoPos.normalized), RayTrailRadiusRange.GetRndValueInVector2Range(), geoFrags[i].transform.localPosition.magnitude); 
        }
        offsetAngle = clickablePlanet.m_accumulateYaw;
        state = GeoControlState.Dissolve;
    }
    IEnumerator coroutineExplodeToDissolve(float duration, float finalFactor = 1.5f){
        this.enabled = false;
        Vector3[] originPoses = new Vector3[geoFrags.Length];
        for(int i=0; i<geoFrags.Length; i++){
            originPoses[i] = geoFrags[i].transform.localPosition;
        }
        yield return new WaitForLoop(duration, (t)=>{
            for(int i=0; i<geoFrags.Length; i++){
                geoFrags[i].transform.localPosition = Vector3.Lerp(originPoses[i], originPoses[i]*finalFactor, EasingFunc.Easing.CircEaseOut(t));
            }
        });
    }
    void OnDrawGizmosSelected(){
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.red, expandRadiusRange.y);
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.yellow, expandRadiusRange.y+RayTrailRadiusRange.GetMin());
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.green, expandRadiusRange.y+RayTrailRadiusRange.GetMax());
    }
}
