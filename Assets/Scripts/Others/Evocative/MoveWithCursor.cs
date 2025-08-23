using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithCursor : MonoBehaviour
{
    [SerializeField] private bool verticalOnly = false;
    [SerializeField] private float heightLimit = 4;
    private float depth = 32;
    private Rigidbody m_rigid;
    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 worldPos = PlayerManager.Instance.GetCursorWorldPos(depth);
        if (verticalOnly)
        {
            worldPos.x = m_rigid.position.x;
        }
        worldPos.y = Mathf.Clamp(worldPos.y, -heightLimit, heightLimit);
        m_rigid.MovePosition(worldPos);
    }
}
