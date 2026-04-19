using System.Collections.Generic;
using UnityEngine;

public class NarrativeCircleManager : Basic_ObjectPool<CollidableCircle>
{
    [System.Serializable]
    public class TextCharSprite
    {
        public char character;
        public Sprite sprite;
    }
    public enum SpawnStyle
    {
        FloatUp,
        PopUp
    }
    private IC_Narrative narrative;
[Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnSize;
    
[Header("Border Setting")]
    [SerializeField] private NarrativeRect rectSelector;

[Header("Force Field")]
    [SerializeField] private NarrativeRandomForceField forceField;
    [SerializeField, ShowOnly] private List<CollidableCircle> listCircles = new List<CollidableCircle>();

[Header("Spawn Text")]
    [SerializeField] private TextCharSprite[] textCharSprites;
    
    private Dictionary<char, Sprite> dictTextSprite = new Dictionary<char, Sprite>();

    void Start()
    {
        narrative = GetComponent<IC_Narrative>();
        foreach(var item in textCharSprites)
        {
            if(!dictTextSprite.ContainsKey(item.character))
            {
                dictTextSprite.Add(item.character, item.sprite);
            }
        }
    }
    void LateUpdate()
    {
        //Remove Destroyed Circles
        for(int i = listCircles.Count - 1; i>=0; i--)
        {
            if(listCircles[i] == null)
            {
                listCircles.RemoveAt(i);
            }
        }
        //Add Force Field to all circle
        for(int i = listCircles.Count - 1; i>=0; i--)
        {
            var circle = listCircles[i];
            if(circle.isPined)
            {
                if(!circle.m_circle.isControlling)
                {
                    forceField.ApplyForce(circle.m_rigidbody, 5);
                }
                continue;
            }
            if(!circle.m_circle.isControlling)
            {
                forceField.ApplyForce(circle.m_rigidbody, 1);
            }

            //Boundry Detection
            //Boundry Teleportation
            Vector3 pos = circle.transform.position;
            if(pos.x < rectSelector.MinX ||
               pos.x > rectSelector.MaxX ||
               pos.y < rectSelector.MinY ||
               pos.y > rectSelector.MaxY)
            {
                circle.ExplodeCircle();
            }
        }
    }
    public CollidableCircle SpawnAtPoint(Vector3 point, float duration, SpawnStyle style)
    {
        var go = GetObjFromPool(x=>false);
        if(go!=null) {
            go.transform.position = point;
            go.gameObject.SetActive(true);
            switch(style)
            {
                case SpawnStyle.PopUp:
                    go.PopUp(duration);
                    break;
                case SpawnStyle.FloatUp:
                    go.FloatUp(duration);
                    break;
            }
        }
        listCircles.Add(go);
        go.RegisterOnExplode(() => OnCircleExplode(go));
        return go;
    }
    void OnCircleExplode(CollidableCircle circle)
    {
        listCircles.Remove(circle);

        if(narrative.m_isDone)
            return;
        if(listCircles.Count<2)
        {
            Debug.LogWarning("Too few circles left, spawn more circle");
            SpawnMakeUpCircle();
        }
        else
        {
            foreach(var content in listCircles)
            {
                if(content.m_circle.m_circleType != Clickable_Circle.CircleType.Hollow)
                {
                    return;
                }
            }
            SpawnMakeUpCircle();
        }
    }
    void SpawnMakeUpCircle()
    {
        int spawnCount = Mathf.Max(0, 2-listCircles.Count) + Random.Range(0, 3);
        for(int i=0; i<spawnCount; i++)
        {
            SpawnAtPoint(new Vector3(Random.Range(rectSelector.MinX, rectSelector.MaxX)*0.5f, Random.Range(rectSelector.MinY, rectSelector.MaxY)*0.5f, 35), Random.Range(3f, 4f), SpawnStyle.FloatUp);
        }
    }
    protected override void PrepareTarget(CollidableCircle target)
    {
        base.PrepareTarget(target);
        target.GetComponent<Clickable_Circle>().DisableHitbox();
        target.ResetGrowingAndWobble();
        target.ResetMotion();
        target.ResetSize(spawnSize.GetRndValueInVector2Range());
        target.m_circle.ChangeCircleType(Clickable_Circle.CircleType.Normal);
    }
    public Sprite GetTextSprite(char character)
    {
        if(dictTextSprite.ContainsKey(character))
        {
            return dictTextSprite[character];
        }
        return null;
    }
    public CollidableCircle[] GetCircleInDistanceOrder(Vector3 position)
    {
        listCircles.RemoveAll(x=>!x.gameObject.activeSelf);
        CollidableCircle[] circles = listCircles.ToArray();
        System.Array.Sort(circles, (a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, position);
            float distB = Vector3.Distance(b.transform.position, position);
            return distA.CompareTo(distB);
        });
        return circles;
    }
    public List<CollidableCircle> GetAllCircles() => listCircles;
# if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        rectSelector.DrawGizmo(new Color(0,1,0,0.25f));
    }
#endif
}