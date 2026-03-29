using UnityEngine;

public class CameraPanWithPointer : MonoBehaviour
{
    [SerializeField] private Transform transVC;
    [SerializeField] private float panSmooth = 1f;
    [SerializeField] private Vector2 maxPanDist = Vector2.one;
    [SerializeField] private float panScale = 1f;
    [SerializeField] private float cursorDepth = 35f;
    private Vector3 originalVCamPos;
    private Vector3 targetPos;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        originalVCamPos = transVC.position;
        targetPos = originalVCamPos;
    }

    void LateUpdate()
    {
        Vector3 cursorPos = PlayerManager.Instance.GetCursorScreenPos();
        cursorPos.z = cursorDepth;
        Vector3 wrdPos = mainCam.ScreenToWorldPoint(cursorPos);
        targetPos.x = originalVCamPos.x + Mathf.Clamp((wrdPos.x - originalVCamPos.x) * panScale, -maxPanDist.x, maxPanDist.x);
        targetPos.y = originalVCamPos.y + Mathf.Clamp((wrdPos.y - originalVCamPos.y) * panScale, -maxPanDist.y, maxPanDist.y);
        transVC.position = Vector3.Lerp(transVC.position, targetPos, panSmooth * Time.deltaTime);
    }
}