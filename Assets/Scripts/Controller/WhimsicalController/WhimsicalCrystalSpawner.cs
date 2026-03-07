using DG.Tweening;
using UnityEngine;

public class WhimsicalCrystalSpawner : MonoBehaviour
{
    [SerializeField] private AnimationCurve easeCurve;
    [SerializeField] private float spanwDuration = 1;
    [SerializeField] private GameObject prefabCrystal;
    [SerializeField] private Transform crystalParent;
    [SerializeField] private Clickable_ObjectRotator heroDiamond;
    [SerializeField] private int maxCrystalCount = 5;
    [SerializeField] private float spawnRate;
    [SerializeField] private float stopRate;
    [SerializeField] private float minAngularSpeed;

    [Header("Spawn Radius")]
    [SerializeField] private Transform[] spawnPoints;

    private float spawnTimer = 0;
    private int spawnCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;    
        Service.Shuffle(ref spawnPoints);
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(heroDiamond.m_angularSpeed) > minAngularSpeed)
        {
            spawnTimer += spawnRate * Time.deltaTime;
            if(spawnTimer >= 1)
            {
                spawnTimer = 0;
                var crystal = Instantiate(prefabCrystal, heroDiamond.transform.position, Quaternion.identity, crystalParent).GetComponent<Clickable_Crystal>();
                crystal.enabled = false;
                Vector3 taretPos = spawnPoints[spawnCount].position + (Vector3)Random.insideUnitCircle * 0.5f;
                crystal.transform.DOMove(taretPos, spanwDuration + Random.Range(-0.4f, 0.4f)).SetEase(easeCurve).OnComplete(() =>
                {
                    crystal.enabled = true;
                });

                spawnCount++;
                if(spawnCount >= maxCrystalCount)
                {
                    spawnCount = 0;
                    Destroy(this);
                }
            }
        }
        else
        {
            if(spawnTimer>0)
            {
                spawnTimer -= stopRate * Time.deltaTime;
                spawnTimer = Mathf.Max(0, spawnTimer);
            }
        }
    }
}
