using UnityEngine;

public class Game : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        UI_Manager.Instance.ShowCursor();
        int progress = LevelProgressionManager.Instance.LevelProgress;
        UI_Manager.Instance.ChangeCursorColor(progress!=7);
    }
}
