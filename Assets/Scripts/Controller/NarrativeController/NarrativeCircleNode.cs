using UnityEngine;

public class NarrativeCircleNode : MonoBehaviour
{
    private float radius;
    public float m_radius => radius;
    public Vector3 m_position => transform.position;

    public void InitNode(float radius)
    {
        this.radius = radius;
    }
    void OnDestroy()
    {
        Debug.LogWarning("On Destroyed");
        EventHandler.Call_OnNarrativeNodeBreak(this);
    }
}
