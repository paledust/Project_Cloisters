using UnityEngine;

public class GameDemo : MonoBehaviour
{
    [SerializeField] private IC_Manager interactionManager;

    void Start()
    {
        Cursor.visible = false;
        UI_Manager.Instance.ShowCursor();
        int progress = LevelProgressionManager.Instance.LevelProgress;
        UI_Manager.Instance.ChangeCursorColor(progress!=7);
    }

#region Level Control
    public void GoBackToMainMenu()
    {
        if(GameManager.Instance.IsSwitchingScene)
        {
            Debug.LogWarning("Scene is switching, cannot reset.");
            return;
        }
        interactionManager.CleanUpImmediately();
        GameManager.Instance.SwitchingScene("Intro");
    }
    public void RestartLevel()
    {
        if(GameManager.Instance.IsSwitchingScene)
        {
            Debug.LogWarning("Scene is switching, cannot restart level.");
            return;
        }
        interactionManager.CleanUpImmediately();
        GameManager.Instance.RestartLevel();   
    }
#endregion
}
