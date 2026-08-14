using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    [SerializeField] private float activationDelay;
    [SerializeField] private UI_Intro uiIntro;
    private UI_Manager uiManager;
    
    void Start()
    {
        uiManager = UI_Manager.Instance;
        StartCoroutine(coroutineActivation());
    }
    void Update()
    {
        if(Cursor.visible)
        {
            Cursor.visible = false;
        }
        uiManager.UpdateCursorPos(Mouse.current.position.ReadValue());
    }

    IEnumerator coroutineActivation()
    {
        yield return new WaitForSeconds(activationDelay);
        uiIntro.ActivateCanvasRaycast();
        uiManager.ChangeCursorColor(true);
        uiManager.ShowCursor();
    }
}
