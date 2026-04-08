using System;
using UnityEngine;

public class ShapeInteractionHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Transform outlineMaskTrans;
    public event Action onRelease;
    public bool isControlling { get; private set; }
    private Vector3 pos;

    public void OnHover()
    {
        
    }
    public void OnExitHover()
    {
        
    }
    public void OnControlled()
    {
        isControlling = true;
    }
    public void OnRelease()
    {
        isControlling = false;
        onRelease?.Invoke();
    }
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.2f);
    }
}