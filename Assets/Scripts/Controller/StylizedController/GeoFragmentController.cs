using System.Collections;
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
    [System.Serializable]
    public struct GeoFragGroup
    {
        public GameObject[] geos;
    }
    [SerializeField] private Clickable_ObjectRotator clickablePlanet;
    [SerializeField] private Transform center;
[Header("Controlling Geos")]
    [SerializeField] private GeoFragGroup[] geoFragGroups;
    [SerializeField] private GameObject[] geoFrags;
[Header("Expand")]
    [SerializeField] private float expandPosAngleRND;
    [SerializeField] private Vector2 expandRadiusRange;
    [SerializeField] private Vector2 expandPosRatioRND;
    [SerializeField] private float finalExpandFactor = 1.5f;
    [SerializeField] private CurveData geoSizeCurve; 
    [SerializeField] private float expandSize = 1.2f;
[Header("Dissolve")]
    [SerializeField] private Vector2 RayTrailRadiusRange;
    [SerializeField] private float dissolveRadius;

    private float controlFactor = 0;
    [SerializeField, ShowOnly] private GameObject[] controllingGeos;
    private Vector3[] geoPoses;
    private int geoGroupIndex = 0;
    private int geoIndexOffset = 0;
    
    void Start(){
    //Reposition geos to center of the sphere
        geoPoses = new Vector3[geoFrags.Length];
        for(int i=0; i<geoPoses.Length; i++){
            geoPoses[i] = Quaternion.Euler(0, 0, Random.Range(-expandPosAngleRND, expandPosAngleRND)+360*i/geoPoses.Length) * Vector2.right * expandPosRatioRND.GetRndValueInVector2Range();
        }
        Service.Shuffle(ref geoPoses);

        foreach(var geoGroup in geoFragGroups){
            foreach(var geo in geoGroup.geos){
                geo.SetActive(false);
            }
        }
        PrepareNextGeo();
    }
    public void UpdateExpand(float expandFactor){
        if(controllingGeos == null) return;
        controlFactor = Mathf.Lerp(controlFactor, expandFactor, Time.deltaTime*5);
    //Rotate Geo and expand along the shape
        for(int i=0; i<controllingGeos.Length; i++){
            controllingGeos[i].transform.localPosition = geoPoses[i+geoIndexOffset] * Mathf.Lerp(expandRadiusRange.x, expandRadiusRange.y, controlFactor);
            controllingGeos[i].transform.localScale = Vector3.one * expandSize * geoSizeCurve.Evaluate(controlFactor);
        }
    }
    void PrepareNextGeo(){
        if(geoGroupIndex>=geoFragGroups.Length){
            controllingGeos = null;
            return;
        }
        controllingGeos = geoFragGroups[geoGroupIndex].geos;
        controlFactor = 0;
        for(int i=0; i<controllingGeos.Length; i++){
            controllingGeos[i].SetActive(true);
            controllingGeos[i].transform.localPosition = geoPoses[i+geoIndexOffset] * expandRadiusRange.x;
            controllingGeos[i].transform.localScale = Vector3.one * expandSize * geoSizeCurve.Evaluate(0);
        }
    }
    public void ExplodeGeo(){
        GameObject[] explodeGroup = new GameObject[controllingGeos.Length];
        for(int i=0; i<controllingGeos.Length; i++){
            explodeGroup[i] = controllingGeos[i];
        }
        controllingGeos = null;
        StartCoroutine(coroutineExplodeToDissolve(explodeGroup, 1f, finalExpandFactor));
    }

    IEnumerator coroutineExplodeToDissolve(GameObject[] selectGeos, float duration, float finalFactor = 1.5f){
        Vector3[] originPoses = new Vector3[selectGeos.Length];
        for(int i=0; i<selectGeos.Length; i++){
            originPoses[i] = selectGeos[i].transform.localPosition;
        }
        yield return new WaitForLoop(duration, (t)=>{
            for(int i=0; i<selectGeos.Length; i++){
                selectGeos[i].transform.localPosition = Vector3.Lerp(originPoses[i], originPoses[i]*finalFactor, EasingFunc.Easing.CircEaseOut(t));
            }
        });
        geoGroupIndex ++;
        PrepareNextGeo();
        geoIndexOffset += selectGeos.Length;
    }
    void OnDrawGizmosSelected(){
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.red, expandRadiusRange.y);
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.yellow, expandRadiusRange.y+RayTrailRadiusRange.GetMin());
        DebugExtension.DrawCircle(center.position, Vector3.forward, Color.green, expandRadiusRange.y+RayTrailRadiusRange.GetMax());
    }
}
