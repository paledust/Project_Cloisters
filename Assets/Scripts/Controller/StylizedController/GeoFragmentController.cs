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
            float dist = normalizedTime>=0?outerDistance:(-innerDistance);
            return ray.origin + ray.direction*(normalizedTime*dist);
        }
    }
    public enum GeoControlState{Expand, Dissolve}
    [SerializeField] private GeoControlState state;
    [SerializeField] private Transform center;
    [SerializeField] private GameObject[] geoFrags;
    [SerializeField] private float expandPosAngleRND;
    [SerializeField] private Vector2 expandRadiusRange;
    [SerializeField] private Vector2 expandPosRatioRND;
    [SerializeField] private Vector2 RayTrailRadiusRange;

    [SerializeField, Range(0, 1)] 
    private float expandFactor = 0;
    private RayTrail[] geoTrails;
    private Vector3[] geoPoses;

    void Update(){
        switch(state){
            case GeoControlState.Expand:
            //Rotate Geo and expand along the shape
                for(int i=0; i<geoFrags.Length; i++){
                    geoFrags[i].transform.localPosition = geoPoses[i] * Mathf.Lerp(expandRadiusRange.x, expandRadiusRange.y, expandFactor);
                }
                break;
            case GeoControlState.Dissolve:
            //Move geo along the trail
                for(int i=0; i<geoFrags.Length; i++){
                }
                break;
        }
    }
    public void StartExpand(){
        state = GeoControlState.Expand;
    //Regenerate certain amount of geos
    //Reposition geos to center of the sphere
        geoPoses = new Vector3[geoFrags.Length];
        for(int i=0; i<geoPoses.Length; i++){
            geoPoses[i] = Quaternion.Euler(0, 0, Random.Range(-expandPosAngleRND, expandPosAngleRND)+360*i/(geoPoses.Length-1)) * Vector2.right * expandPosRatioRND.GetRndValueInVector2Range();
        }
    }
    public void StartDissolve(){
        state = GeoControlState.Dissolve;
    //Generate Ray trail and place geos on the right track
        geoTrails = new RayTrail[geoFrags.Length];

        for(int i=0; i<geoTrails.Length; i++){
            Vector3 geoPos = geoFrags[i].transform.localPosition;

            geoTrails[i] = new RayTrail(new Ray(geoPos, geoPos.normalized), RayTrailRadiusRange.GetRndValueInVector2Range(), geoFrags[i].transform.position.magnitude); 
        }
    }
    void OnDrawGizmosSelected(){
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.red, expandRadiusRange.y);
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.yellow, expandRadiusRange.y+RayTrailRadiusRange.GetMin());
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.green, expandRadiusRange.y+RayTrailRadiusRange.GetMax());
    }
}
