using UnityEngine;

public class Intro : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        UI_Manager.Instance.HideCursor();
    }
}
