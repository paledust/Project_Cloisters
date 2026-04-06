using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    private UI_Manager uiManager;
    void Start()
    {
        Cursor.visible = true;
        uiManager = UI_Manager.Instance;
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
