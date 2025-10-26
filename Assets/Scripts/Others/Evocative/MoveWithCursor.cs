using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithCursor : MonoBehaviour
{
    [SerializeField] private bool FollowX = false;
    [SerializeField] private bool FollowY = false;
    [SerializeField] private float heightLimit = 4;
    [SerializeField] private float widthLimit = 8;
    [SerializeField] private float widthOffset = 0;
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
        if (FollowY)
        {
            worldPos.x = m_rigid.position.x;
            worldPos.y = Mathf.Clamp(worldPos.y, -heightLimit, heightLimit);
        }
        if(FollowX)
        {
            worldPos.x = Mathf.Clamp(worldPos.x, -widthLimit+widthOffset, widthLimit+widthOffset);
            worldPos.y = m_rigid.position.y;
        }
        m_rigid.MovePosition(worldPos);
    }
}
