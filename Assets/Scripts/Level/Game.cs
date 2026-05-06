using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private string startRoomtone;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private AmbienceHandler ambienceHandler;

    void Start()
    {
        ambienceHandler.PlayAmbience(startRoomtone, volume);
        Cursor.visible = false;
        UI_Manager.Instance.ShowCursor();
        int progress = LevelProgressionManager.Instance.LevelProgress;
        UI_Manager.Instance.ChangeCursorColor(progress!=7);
    }
}
