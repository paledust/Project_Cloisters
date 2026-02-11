using System.Collections;
using System.Linq;
using DG.Tweening;
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
        public Clickable_Stylized[] geos;
    }
    [SerializeField] private Transform center;
[Header("Controlling Geos")]
    [SerializeField] private GeoFragGroup[] geoFragGroups;
[Header("Expand")]
    [SerializeField] private float expandPosAngleRND;
    [SerializeField] private Vector2 expandRadiusRange;
    [SerializeField] private Vector2 expandPosRatioRND;
    [SerializeField] private float finalExpandFactor = 1.5f;
    [SerializeField] private CurveData geoSizeCurve; 
[Header("Dissolve")]
    [SerializeField] private Vector2 RayTrailRadiusRange;

    private int geoGroupIndex = 0;
    private int geoIndexOffset = 0;
    private float controlFactor = 0;
    private Vector3[] geoPoses;
    private Clickable_Stylized[] controllingGeos;
    
    void Start(){
        int count = 0;
        foreach(var geoGroup in geoFragGroups){
            count += geoGroup.geos.Length;
            foreach(var geo in geoGroup.geos)
            {
                geo.DisableHitbox();
            }
        }
    //Reposition geos to center of the sphere
        geoPoses = new Vector3[count];
        for(int i=0; i<geoPoses.Length; i++){
            geoPoses[i] = Quaternion.Euler(0, 0, Random.Range(-expandPosAngleRND, expandPosAngleRND)+360*i/geoPoses.Length) * Vector2.right * expandPosRatioRND.GetRndValueInVector2Range();
        }
        Service.Shuffle(ref geoPoses);

        foreach(var geoGroup in geoFragGroups){
            foreach(var geo in geoGroup.geos){
                geo.gameObject.SetActive(false);
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
            controllingGeos[i].transform.localScale = Vector3.one * geoSizeCurve.Evaluate(controlFactor);
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
            controllingGeos[i].gameObject.SetActive(true);
            controllingGeos[i].transform.localPosition = geoPoses[i+geoIndexOffset] * expandRadiusRange.x;
            controllingGeos[i].transform.localScale = Vector3.one * geoSizeCurve.Evaluate(0);
        }
    }
    public void ExplodeGeo(){
        var explodeGeos = new Clickable_Stylized[controllingGeos.Length];
        for(int i=0; i<controllingGeos.Length; i++){
            explodeGeos[i] = controllingGeos[i];
        }
        controllingGeos = null;
        StartCoroutine(coroutineExplodeToDissolve(explodeGeos, 1f, finalExpandFactor));
    }

    IEnumerator coroutineExplodeToDissolve(Clickable_Stylized[] selectGeos, float duration, float finalFactor = 1.5f){
        Vector3[] originPoses = new Vector3[selectGeos.Length];
        for(int i=0; i<selectGeos.Length; i++){
            var geo = selectGeos[i];
            originPoses[i] = geo.transform.localPosition;
            geo.transform.DOScale(Vector3.one * 1.2f, Random.Range(0.15f, 0.25f)).SetEase(Ease.OutBack).OnComplete(() =>
            {
                geo.enabled = true;
                geo.EnableHitbox();
            });
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
