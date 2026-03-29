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
    [SerializeField] private float lerpSpeed = 50;
    [SerializeField, Range(0, 1), ShowOnly] private float controlValue = 0;
    private float depth = 32;
    private Rigidbody m_rigid;
    private Camera mainCam;
    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        mainCam = Camera.main;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 cursorPos = PlayerManager.Instance.GetCursorScreenPos();
        cursorPos.z = depth;
        Vector3 worldPos = mainCam.ScreenToWorldPoint(cursorPos);
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
        m_rigid.MovePosition(Vector3.Lerp(m_rigid.position, worldPos, lerpSpeed * controlValue * Time.fixedDeltaTime));
    }
    public void StartControl(float duration)
    {
        StartCoroutine(coroutineStartControl(duration));
    }
    IEnumerator coroutineStartControl(float duration)
    {
        yield return new WaitForLoop(duration, (t)=>{
            controlValue = t;
        });
    }
}
