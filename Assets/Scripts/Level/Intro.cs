using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    private UI_Manager uiManager;
    void Start()
    {
        uiManager = UI_Manager.Instance;
        uiManager.ChangeCursorColor(true);
        uiManager.ShowCursor();
    }
    void Update()
    {
        if(Cursor.visible)
        {
            Cursor.visible = false;
        }
        uiManager.UpdateCursorPos(Mouse.current.position.ReadValue());
    }
}
