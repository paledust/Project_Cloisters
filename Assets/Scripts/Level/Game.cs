using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [SerializeField] private IC_Manager interactionManager;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] private InputActionMap debugActions;
#endif
    void Start()
    {
        Cursor.visible = false;
        UI_Manager.Instance.ShowCursor();
        int progress = LevelProgressionManager.Instance.LevelProgress;
        UI_Manager.Instance.ChangeCursorColor(progress!=7);
    }
    void Awake(){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["restart"].performed += Debug_RestartLevel;
        debugActions["reset"].performed += Debug_Reset;
        debugActions.Enable();
#endif
    }
    void OnDestroy(){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        debugActions["restart"].performed -= Debug_RestartLevel;
        debugActions["reset"].performed -= Debug_Reset;
        debugActions.Disable();
#endif
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

#region Debug Function
    void Debug_RestartLevel(InputAction.CallbackContext callback){
        if(callback.ReadValueAsButton())
            RestartLevel();
    }
    void Debug_Reset(InputAction.CallbackContext callback){
        if(callback.ReadValueAsButton())
            GoBackToMainMenu();
    }
#endregion
}
