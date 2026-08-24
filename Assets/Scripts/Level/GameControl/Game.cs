using UnityEngine;
using UnityEngine.InputSystem;

public class Game : GameControlBasic
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] private InputActionMap debugActions;
#endif

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
